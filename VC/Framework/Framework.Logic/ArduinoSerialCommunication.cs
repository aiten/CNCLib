////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.IO.Ports;
using System.Diagnostics;

// TODO: _autoEvent only works in ping pong 
//		 sending 2 commands will faile (Reset) => fast timeout

namespace Framework.Logic
{
    #region EventArg

    public class ArduinoSerialCommunicationEventArgs : EventArgs
    {
        public ArduinoSerialCommunicationEventArgs(string info, ArduinoSerialCommunication.Command cmd)
        {
            Command = cmd;
            if (cmd != null && string.IsNullOrEmpty(info))
                this.Info = cmd.CommandText;
            else
                this.Info = info;
            Continue = false;
            Abort = false;
        }

        public bool Continue { get; set; }
        public bool Abort { get; set; }
        public string Result { get; set; }
		
		public readonly string Info;

        public ArduinoSerialCommunication.Command Command { get; private set; }
    }

    #endregion

    public class ArduinoSerialCommunication : IDisposable
    {
        #region Private Members

        bool _continue;
        SerialPort _serialPort;
        Thread _readThread;
        Thread _writeThread;
		AutoResetEvent _autoEvent = new AutoResetEvent(false);

        [Flags]
		public enum EReplyType
		{
            NoReply=1,
			ReplyOK=2,
			ReplyError=4,
			ReplyInfo=8,
			ReplyUnkown=16
		};

		List<Command> _pendingCommands = new List<Command>();

        #endregion

        #region Events

        public delegate void CommandEventHandler(object com, ArduinoSerialCommunicationEventArgs info);

        // The event we publish
        public event CommandEventHandler WaitForSend;
        public event CommandEventHandler CommandSending;
        public event CommandEventHandler CommandSent;
        public event CommandEventHandler CommandWaitReply;
        public event CommandEventHandler ReplyReceived;
        public event CommandEventHandler ReplyOK;
        public event CommandEventHandler ReplyError;
        public event CommandEventHandler ReplyInfo;
        public event CommandEventHandler ReplyUnknown;

        #endregion

        #region ctr

        public ArduinoSerialCommunication()
        {
            BaudRate = 115200;
            OkTag = @"ok";
            ErrorTag = @"Error:";
            InfoTag = @"Info:";
            MaxCommandHistoryCount = int.MaxValue;
			ErrorIsReply = false;		// each command must end with ok

			CommandToUpper = false;
			ResetOnConnect = false;
            ArduinoBuffersize = 64;
		}

        #endregion 

        #region Properties

		public class Command
		{
            public DateTime SentTime { get; set; }
            public String CommandText { get; set; }

            public EReplyType ReplyType { get; set; }
            public DateTime ReplyReceivedTime { get; set; }

            public String ResultText { get; set; }
		}

        public int BaudRate { get; set; }
		public bool ResetOnConnect { get; set; }
		public String OkTag { get; set; }
        public String ErrorTag { get; set; }
        public String InfoTag { get; set; }

		public bool CommandToUpper { get; set; }


		public bool ErrorIsReply { get; set; }

//      public bool IsConnected { get { return true; }  }
		public bool IsConnected { get { return _serialPort != null && _serialPort.IsOpen; } }

        public int MaxCommandHistoryCount { get; set; }

        public int ArduinoBuffersize { get; set; }

        #endregion

        #region Control Properties

        public bool Abort { get; protected set; }

        #endregion

        #region Setup/Init Methodes

        public void Connect(string portname)
        {
            // Create a new SerialPort object with default settings.
			Abort = false;

			SetupCom(portname);

            _serialPort.Open();
            _readThread = new Thread(Read);
            _writeThread = new Thread(Write);

           _continue = true;
            _readThread.Start();
            _writeThread.Start();

			if (!ResetOnConnect)
			{
				_serialPort.DiscardOutBuffer();
				_serialPort.WriteLine("");
			}

			lock (_pendingCommands)
			{
				_pendingCommands.Clear();
			}
			lock (_commands)
			{
				_commands.Clear();
			}
		}
        
        public void Disconnect()
        {
            Abort = true;
            _continue = false;
            if (_readThread != null)
                _readThread.Join();
            _readThread = null;

			if (_writeThread != null)
			{
				while (!_writeThread.Join(100))
				{
					_autoEvent.Set();
				}
			}
			_writeThread = null;

            if (_serialPort != null)
            {
				try
				{
					_serialPort.Close();
				}
				catch (IOException)
				{
					// ignore exception
				}
                _serialPort.Dispose();
            }
            _serialPort = null;
        }

        public void AbortCommands()
        {
			lock (_pendingCommands)
			{
				_pendingCommands.Clear();
			}
			
			Abort = true;
        }

        virtual protected void SetupCom(string portname)
        {
            _serialPort = new SerialPort();

            _serialPort.PortName = portname;
            _serialPort.BaudRate = BaudRate;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;

			if (ResetOnConnect)
				_serialPort.DtrEnable = true;

            // Set the read/write timeouts
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
        }

        public void Dispose()
        {
            Disconnect();
        }

        public void ResumAfterAbort()
        {
            if (!Abort) return;

            while (true)
            {
                lock (_pendingCommands)
                {
                    if (_pendingCommands.Count == 0) break;
                }
            }
            Abort = false;
        }

        #endregion

        #region overrides

        protected virtual string[] SplitCommand(string line)
        {
            return new string[] { line };
        }

        #endregion

		#region SendCommandAndRead

		public string SendCommandAndRead(string line)
		{
			string message = null;
			var checkresponse = new ArduinoSerialCommunication.CommandEventHandler((obj, e) =>
			{
				message = e.Info;
			});
			ReplyOK += checkresponse;
			SendCommand(line);
			ReplyOK -= checkresponse;
			return message;
		}

		#endregion

		#region Send Command(s)

		public void SendCommand(string line)
        {
            AsyncSendCommand(line);
            WaitUntilNoPendingCommands();
        }

        public void SendCommands(string[] commands)
        {
            if (commands != null)
            {
                foreach (string cmd in commands)
                {
                    AsyncSendCommand(cmd);
                    if (Abort)
                        break;
                }
				WaitUntilNoPendingCommands();
            }
        }

		public void SendFile(string filename)
		{
			using (StreamReader sr = new StreamReader(filename))
			{
				Abort = false;
				String line;
				List<String> lines = new List<string>();
				while ((line = sr.ReadLine()) != null && !Abort)
				{
					lines.Add(line);
				}
				SendCommands(lines.ToArray());
			}
		}

        #endregion

        #region Internals

        private void AsyncSendCommand(string line)
        {
            string[] cmds = SplitCommand(line);
            foreach (string cmd in cmds)
            {
                Command pcmd = CreateCommand(cmd);
                if (pcmd != null)
                {
                    // sending is done in Write-Thread
                }
            }
        }
        private Command CreateCommand(string cmd)
        {
            if (string.IsNullOrEmpty(cmd))
                return null;

			if (CommandToUpper)
				cmd = cmd.ToUpper();

			cmd = cmd.Replace('\t', ' ');

            lock (_pendingCommands)
            {
				if (_pendingCommands.Count==0)
					_autoEvent.Set();			// start Async task now!

                Command c = new Command() { CommandText = cmd };
                _pendingCommands.Add(c);

                return c;
            }
        }

		private  void SendCommand(Command cmd)
        {
			ArduinoSerialCommunicationEventArgs eventarg = new ArduinoSerialCommunicationEventArgs(null,cmd);
			OnCommandSending(eventarg);

			if (eventarg.Abort || Abort) return;

			lock (_commands)
			{
				if (_commands.Count > MaxCommandHistoryCount)
				{
					_commands.RemoveAt(0);
				}
				_commands.Add(cmd);
			}

             cmd.SentTime = DateTime.Now;

			 string commandtext = cmd.CommandText;

			 while (commandtext.Length > ArduinoBuffersize-1)
			 {
				 // give "control" class the chance to read from arduino to control buffer

				 int firstSize = ArduinoBuffersize * 2 / 3;
				 string firstX = commandtext.Substring(0, firstSize);
				 commandtext = commandtext.Substring(firstSize);
				 _serialPort.Write(firstX);
				 Thread.Sleep(250);
			 }

			 _serialPort.WriteLine(commandtext);

Console.WriteLine(cmd.CommandText);

            eventarg = new ArduinoSerialCommunicationEventArgs(null,cmd);
			OnCommandSent(eventarg);
        }


        private void WaitUntilNoPendingCommands()
		{
			while (_continue)
            {
                Command cmd = null; ;
                lock (_pendingCommands)
                {
					if (_pendingCommands.Count > 0)
					{
						cmd = _pendingCommands[0];
					}
                }

				if (cmd == null) break;

				var eventarg = new ArduinoSerialCommunicationEventArgs(null,cmd);
                OnCommandWaitReply(eventarg);
                if (Abort || eventarg.Abort) return;

				_autoEvent.WaitOne(10);
            }
		}

        private void Write()
        {
            while (_continue)
            {
                Command nextcmd=null;
                int queuedcmdlenght = 0;
                lock(_pendingCommands)
                {
					foreach (Command cmd in _pendingCommands)
                    {
                        if (cmd.SentTime == new DateTime())
                        {
                            nextcmd = cmd;
                            break;
                        }
                        else
                        {
                            queuedcmdlenght += cmd.CommandText.Length;
                            queuedcmdlenght += 2; // CRLF

                        }
                    }
                }
				if (nextcmd != null)
				{
					// send everyting if queue is empty
					// or send command if pending commands + this fit into arduino queue
					if (queuedcmdlenght == 0 || queuedcmdlenght + nextcmd.CommandText.Length + 2 < ArduinoBuffersize)
					{
						SendCommand(nextcmd);
					}
					else
					{
						var eventarg = new ArduinoSerialCommunicationEventArgs(null, nextcmd);
						OnWaitForSend(eventarg);
						if (Abort || eventarg.Abort) return;

						lock (_pendingCommands)
						{
							if (_pendingCommands.Count > 0 && _pendingCommands[0].SentTime != new DateTime())
								_autoEvent.Reset();			// expect an answer
						}

						_autoEvent.WaitOne(10);
					}
				}
				else
				{
					_autoEvent.WaitOne(100);		// no command in queue => wait => CreateCommand(...) will set Autoevent
				}
            }
        }

        private void Read()
        {
            while (_continue)
            {
                string message=null;
				try
				{
					message = _serialPort.ReadLine();
				}
				catch (TimeoutException) { }
				catch (InvalidOperationException) { }
				catch (IOException) { }
/*
				string message="";
				while (_continue)
				{
					try
					{
						char ch = (char) _serialPort.ReadByte();
						if (ch == '\n' || ch == '\r')
							break;
						message += ch;
					}
					catch (TimeoutException) { }
					catch (InvalidOperationException) { }
				}
*/
				if (!string.IsNullOrEmpty(message))
                {
					bool endcommand = false;
                    Command cmd = null;
                    lock(_pendingCommands)
                    {
                        if (_pendingCommands.Count  > 0)
                            cmd = _pendingCommands[0];
                    }

                    message = message.Trim();

//Console.Write(message); if (cmd != null) { Console.Write("=>"); Console.Write(cmd.CommandText); } Console.WriteLine();

					if (cmd != null)
					{
						string result = cmd.ResultText;
						if (string.IsNullOrEmpty(result))
							result = message;
						else
							result = result + "\n" + message;

						cmd.ResultText = result;
						cmd.ReplyReceivedTime = DateTime.Now;
					}

					OnReplyReceived(new ArduinoSerialCommunicationEventArgs(message,cmd));

                    if (message.StartsWith((OkTag)))
                    {
                        endcommand = true;
                        OnReplyDone(new ArduinoSerialCommunicationEventArgs(message, cmd));
                    }
                    else if (message.StartsWith(ErrorTag))
                    {
						if (ErrorIsReply)
							endcommand = true;
                        OnReplyError(new ArduinoSerialCommunicationEventArgs(message, cmd));
                    }
                    else if (message.StartsWith(InfoTag))
                    {
						if (cmd != null)
						{
							cmd.ReplyType |= EReplyType.ReplyInfo;
						}
						
						OnReplyInfo(new ArduinoSerialCommunicationEventArgs(message, cmd));
                    }
                    else
                    {
                        OnReplyUnknown(new ArduinoSerialCommunicationEventArgs(message, cmd));
                    }

                    if (endcommand && cmd != null)
                    {
                        lock (_pendingCommands)
                        {
                            _pendingCommands.RemoveAt(0);
							_autoEvent.Set();
						}
                    }
                }
            }
        }

        #endregion

        #region OnEvents
        protected virtual void OnWaitForSend(ArduinoSerialCommunicationEventArgs info)
        {
            if (WaitForSend != null)
            {
                WaitForSend(this, info);
            }
        }

        protected virtual void OnCommandSending(ArduinoSerialCommunicationEventArgs info)
        {
            if (CommandSending != null)
            {
                CommandSending(this, info);
            }
        }
        protected virtual void OnCommandSent(ArduinoSerialCommunicationEventArgs info)
        {
            if (CommandSent != null)
            {
                CommandSent(this, info);
            }
        }
        protected virtual void OnCommandWaitReply(ArduinoSerialCommunicationEventArgs info)
        {
            if (CommandWaitReply != null)
            {
                CommandWaitReply(this, info);
            }
        }
        protected virtual void OnReplyReceived(ArduinoSerialCommunicationEventArgs info)
        {
            if (ReplyReceived != null)
            {
                ReplyReceived(this, info);
            }
        }

        protected virtual void OnReplyInfo(ArduinoSerialCommunicationEventArgs info)
        {
            if (ReplyInfo != null)
            {
                ReplyInfo(this, info);
            }
        }
        protected virtual void OnReplyError(ArduinoSerialCommunicationEventArgs info)
        {
			if (_commands.Count > 0)
			{
				_commands.Last().ReplyType |= EReplyType.ReplyError;
			}
			
			if (ReplyError != null)
            {
                ReplyError(this, info);
            }
        }
        protected virtual void OnReplyDone(ArduinoSerialCommunicationEventArgs info)
        {
			if (_commands.Count > 0)
			{
				_commands.Last().ReplyType |= EReplyType.ReplyOK;
			}
			
			if (ReplyOK != null)
            {
                ReplyOK(this, info);
            }
        }
        protected virtual void OnReplyUnknown(ArduinoSerialCommunicationEventArgs info)
        {
			if (_commands.Count > 0)
			{
				_commands.Last().ReplyType |= EReplyType.ReplyUnkown;
			}
			
			if (ReplyUnknown != null)
            {
                ReplyUnknown(this, info);
            }
        }

        #endregion

        #region Command History 

        List<Command> _commands = new List<Command>();
        public List<Command> CommandHistory { get { return _commands; } }

        public void ClearCommandHistory() 
		{
			lock (this)
			{
				_commands.Clear();
			}
		}

		public void WriteCommandHistory(string filename)
		{
			lock (this)
			{
				using (StreamWriter sr = new StreamWriter(filename))
				{
                    foreach (Framework.Logic.ArduinoSerialCommunication.Command cmds in CommandHistory)
					{
						sr.Write(cmds.SentTime); sr.Write(":");
						sr.Write(cmds.CommandText); sr.Write(" => ");
						sr.WriteLine(cmds.ResultText);
					}
				}
			}
		}

        #endregion

    }
}
