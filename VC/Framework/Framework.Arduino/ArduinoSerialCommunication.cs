////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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
using Framework.Tools;

namespace Framework.Arduino
{
	public class ArduinoSerialCommunication : IDisposable
    {
        #region Private Members

        bool _continue;
        SerialPort _serialPort;
        Thread _readThread;
        Thread _writeThread;
		AutoResetEvent _autoEvent = new AutoResetEvent(false);
		TraceStream _trace = new TraceStream();

		[Flags]
		public enum EReplyType
		{
            NoReply=0,              // no reply received (other options must not be set)
			ReplyOK=1,
			ReplyError=2,
			ReplyInfo=4,
			ReplyUnkown=8
		};

		List<Command> _pendingCommands = new List<Command>();

        #endregion

        #region Events

        public delegate void CommandEventHandler(object sender, ArduinoSerialCommunicationEventArgs e);

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
		public event CommandEventHandler CommandQueueChanged;

		#endregion

		#region ctr

		public ArduinoSerialCommunication()
        {
		}

        #endregion 

        #region Properties

		public class Command
		{
            public DateTime? SentTime { get; set; }
            public String CommandText { get; set; }

            public EReplyType ReplyType { get; set; }
            public DateTime? ReplyReceivedTime { get; set; }

            public String ResultText { get; set; }
		}

		public int CommandsInQueue
		{
			get { return _pendingCommands.Count;  }
		}

		//      public bool IsConnected { get { return true; }  }
		public bool IsConnected { get { return _serialPort != null && _serialPort.IsOpen; } }
		public bool Aborted { get; protected set; }


		public int BaudRate { get; set; }				= 115200;
		public bool ResetOnConnect { get; set; }		= false;
		public String OkTag { get; set; }				= @"ok";
		public String ErrorTag { get; set; }			= @"Error:";
		public String InfoTag { get; set; }				= @"Info:";
		public bool CommandToUpper { get; set; }		= false;
		public bool ErrorIsReply { get; set; }			= false;     // each command must end with ok
        public int MaxCommandHistoryCount { get; set; } = int.MaxValue;
		public int ArduinoBuffersize { get; set; }		= 64;
        public int ArduinoLineSize { get; set; } = 128;
        public TraceStream Trace { get { return _trace; } }

		#endregion

		#region Setup/Init Methodes

		/// <summary>
		/// Connect to the Arduino serial port 
		/// </summary>
		/// <param name="portname">e.g. Com1</param>
		public void Connect(string portname)
        {
            // Create a new SerialPort object with default settings.
			Aborted = false;

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

			bool wasempty;

			lock (_pendingCommands)
			{
				wasempty = _pendingCommands.Count == 0;
                _pendingCommands.Clear();
			}
			lock (_commands)
			{
				_commands.Clear();
			}

			if (!wasempty)
				OnComandQueueChanged(new ArduinoSerialCommunicationEventArgs(null, null));
		}

        /// <summary>
		/// Disconnect from arduino 
		/// </summary>
        public void Disconnect()
        {
            Disconnect(true);
        }

        private void Disconnect(bool join)
        {
            Trace.WriteTraceFlush("Disconnecting",join.ToString());
            Aborted = true;
            _continue = false;

            if (join && _readThread != null)
                _readThread.Join();
            _readThread = null;

            if (join && _writeThread != null)
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
            Trace.WriteTraceFlush("Disconnected", join.ToString());
        }

        /// <summary>
        /// Start "Abort", but leave communication opend. but do not send and command.
        /// All pending commands are removed
        /// </summary>
        public void AbortCommands()
        {
			bool wasempty;
			lock (_pendingCommands)
			{
				wasempty = _pendingCommands.Count == 0;
				_pendingCommands.Clear();
			}

			if (!wasempty)
				OnComandQueueChanged(new ArduinoSerialCommunicationEventArgs(null, null));

			Aborted = true;
        }

		/// <summary>
		/// ResumeAfterAbort must be called after AbortCommands to continune. 
		/// </summary>
		public void ResumeAfterAbort()
		{
			if (!Aborted) return;

			while (true)
			{
				lock (_pendingCommands)
				{
					if (_pendingCommands.Count == 0) break;
				}
			}
			Aborted = false;
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
			if (_trace!=null)
			{
				_trace.Dispose();
				_trace = null;
			}
		}

		public void WritePendingCommandsToFile(string filename)
		{
			using (StreamWriter sw = new StreamWriter(filename))
			{
				lock (_pendingCommands)
				{
					foreach (Command cmd in _pendingCommands)
					{
						sw.WriteLine(cmd.CommandText);
					}
				}
			}
		}

		#endregion

		#region overrides

		protected virtual string[] SplitCommand(string line)
        {
            return new string[] { line };
        }

		#endregion

		#region Send Command(s)

		/// <summary>
		/// Send a command to the arduino and wait until a (OK) reply
		/// </summary>
		/// <param name="line">command line</param>
		/// <returns></returns>
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

		/// <summary>
		/// Send command and wait until the command is transfered (do not wait on reply)
		/// </summary>
		/// <param name="line">command line to send</param>
		public void SendCommand(string line)
        {
            AsyncSendCommand(line);
            WaitUntilNoPendingCommands();
        }

		/// <summary>
		/// Send multiple command lines to the arduino. Wait until the commands are transferrd (do not wait on reply)
		/// </summary>
		/// <param name="commands"></param>
		public void SendCommands(IEnumerable<string> commands)
        {
            if (commands != null)
            {
                foreach (string cmd in commands)
                {
                    AsyncSendCommand(cmd);
                    if (Aborted)
                        break;
                }
				WaitUntilNoPendingCommands();
            }
        }

		/// <summary>
		/// Send commands stored in a file. Wait until the commands are transferrd (do not wait on reply)
		/// </summary>
		/// <param name="filename">used for a StreamReader</param>
		public void SendFile(string filename)
		{
			using (StreamReader sr = new StreamReader(filename))
			{
				Aborted = false;
				String line;
				List<String> lines = new List<string>();
				while ((line = sr.ReadLine()) != null && !Aborted)
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

				Trace.WriteTrace("Queue", cmd);

				OnComandQueueChanged(new ArduinoSerialCommunicationEventArgs(null, c));
                return c;
            }
        }

		private  void SendCommand(Command cmd)
        {
            // SendCommands is called in the async Write thread 

            ArduinoSerialCommunicationEventArgs eventarg = new ArduinoSerialCommunicationEventArgs(null, cmd);
            OnCommandSending(eventarg);

            if (eventarg.Abort || Aborted) return;

            lock (_commands)
            {
                if (_commands.Count > MaxCommandHistoryCount)
                {
                    _commands.RemoveAt(0);
                }
                _commands.Add(cmd);
            }

            string commandtext = cmd.CommandText;

            const int crlf_size = 2;

            if (commandtext.Length >= ArduinoLineSize + crlf_size) 
                commandtext = commandtext.Substring(0, ArduinoLineSize - 1 - crlf_size);

            while (commandtext.Length > ArduinoBuffersize - 1)
            {
                // give "control" class the chance to read from arduino to control buffer

                int firstSize = ArduinoBuffersize * 2 / 3;
                string firstX = commandtext.Substring(0, firstSize);
                commandtext = commandtext.Substring(firstSize);

                if (!WriteSerial(firstX))
                    return;

               Thread.Sleep(250);
            }

            if (WriteLineSerial(commandtext))
            {
                cmd.SentTime = DateTime.Now;
                eventarg = new ArduinoSerialCommunicationEventArgs(null, cmd);
                OnCommandSent(eventarg);
            }
        }

        private bool WriteSerial(string commandtext)
        {
            Trace.WriteTrace("Write", commandtext);
            try
            {
                _serialPort.Write(commandtext);
                return true;
            }
            catch (InvalidOperationException e)
            {
                Trace.WriteTraceFlush("WriteInvalidOperationException", commandtext + @" => " + e.Message);
                Disconnect(false);
            }
            catch (IOException e)
            {
                Trace.WriteTraceFlush("WriteIOException", commandtext + @" => " + e.Message);
                ErrorSerial();
            }
            catch (Exception e)
            {
                Trace.WriteTraceFlush("WriteException", commandtext + @" => " + e.GetType().ToString() + @" " + e.Message);
            }
            return false;
        }

        private bool WriteLineSerial(string commandtext)
        {
            Trace.WriteTrace("Write", commandtext + @"\n");
            try
            {
                _serialPort.WriteLine(commandtext);
                return true;
            }
            catch (InvalidOperationException e)
            {
                Trace.WriteTraceFlush("WriteInvalidOperationException", commandtext + @"\n => " + e.Message);
                Disconnect(false);
            }
            catch (IOException e)
            {
                Trace.WriteTraceFlush("WriteIOException", commandtext + @"\n => " + e.Message);
                ErrorSerial();
            }
            catch (Exception e)
            {
                Trace.WriteTraceFlush("WriteException", commandtext + @"\n => " + e.GetType().ToString() + @" " + e.Message);
            }
            return false;
        }

        private void ErrorSerial()
        {
            try
            {
                _serialPort.Close();
                _serialPort.DtrEnable = false;
                _serialPort.Open();
            }
            catch (Exception)
            {
                Disconnect(false);
            }
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
                if (Aborted || eventarg.Abort) return;

				_autoEvent.WaitOne(10);
            }
		}

        private void Write()
        {
			// Aync write thread to send commands to the arduino

            while (_continue)
            {
                Command nextcmd=null;
                int queuedcmdlenght = 0;

				// commands are sent to the arduino until the buffer is full
				// In the _pendingCommand list also the commands are still stored with no reply.

				lock (_pendingCommands)
                {
					foreach (Command cmd in _pendingCommands)
                    {
                        if (cmd.SentTime.HasValue)
                        {
                            queuedcmdlenght += cmd.CommandText.Length;
                            queuedcmdlenght += 2; // CRLF
                        }
                        else
                        {
                            nextcmd = cmd;
                            break;
                        }
                    }
                }

				// nextcmd			=> next command to be sent
				// queuedcmdlenght	=> lenght of command in the arduino buffer

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
						if (Aborted || eventarg.Abort) return;

						lock (_pendingCommands)
						{
							if (_pendingCommands.Count > 0 && _pendingCommands[0].SentTime.HasValue)
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
            // Aync read thread to read 8command) results from the arduino

            var sb = new StringBuilder();

			while (_continue)
            {
                string message=null;
                char ch;
                try
                {
                    ch = (char) _serialPort.ReadChar();
                    if (ch == '\n')
                    {
                        message = sb.ToString();
                        sb.Clear();
                    }
                    else
                    {
                        sb.Append(ch);
                    }
                    // have problem with :
                    //					message = _serialPort.ReadLine();
				}
				catch (TimeoutException) { }
				catch (InvalidOperationException e)
				{
					Trace.WriteTraceFlush("ReadInvalidOperationException", e.Message);
                    Thread.Sleep(250);
                }
                catch (IOException e)
				{
					Trace.WriteTraceFlush("ReadIOException", e.Message);
                    Thread.Sleep(250);
                }
                catch (Exception e)
                {
                    Trace.WriteTraceFlush("ReadException", e.Message);
                    Thread.Sleep(250);
                }

                Command cmd = null;
                lock (_pendingCommands)
                {
                    if (_pendingCommands.Count > 0)
                        cmd = _pendingCommands[0];
                }

                if (string.IsNullOrEmpty(message))
                {
                    if (cmd==null)
                    {
                        // timeout and no queue
                        Framework.Tools.WinAPIWrapper.AllowIdle();
                    }
                }
                else
                {
					Trace.WriteTrace("Read",message.Replace("\n",@"\n").Replace("\r", @"\r").Replace("\t", @"\t"));

					bool endcommand = false;

                    message = message.Trim();

					if (cmd != null)
					{
                        Framework.Tools.WinAPIWrapper.KeepAlive();

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
						OnComandQueueChanged(new ArduinoSerialCommunicationEventArgs(null, cmd));
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

		protected virtual void OnComandQueueChanged(ArduinoSerialCommunicationEventArgs info)
		{
			if (CommandQueueChanged != null)
			{
				CommandQueueChanged(this, info);
			}
		}

		#endregion

		#region Command History 

		List<Command> _commands = new List<Command>();
        public List<Command> CommandHistoryCopy { get { lock (_commands) { return new List<Command>(_commands); } } }

		public Command LastCommand
		{
			get
			{
				lock (_commands)
				{
					return _commands.Last();
				}
			}
		}

		public void ClearCommandHistory() 
		{
			lock (_commands)
			{
				_commands.Clear();
			}
		}

		public void WriteCommandHistory(string filename)
		{
			lock (_commands)
			{
				using (StreamWriter sr = new StreamWriter(filename))
				{
                    foreach (ArduinoSerialCommunication.Command cmds in _commands)
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
