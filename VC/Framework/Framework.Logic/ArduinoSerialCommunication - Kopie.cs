using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.IO.Ports;
using System.Diagnostics;

namespace Framework.Logic
{
    #region EventArg

    public class ArduinoSerialCommunicationEventArgs : EventArgs
    {
        public ArduinoSerialCommunicationEventArgs(string info)
        {
            this.Info = info;
            Continue = false;
            Abort = false;
        }

        public bool Continue { get; set; }
        public bool Abort { get; set; }
        public string Result { get; set; }
		
		public readonly string Info;
    }

    #endregion

    public class ArduinoSerialCommunication : IDisposable
    {
        #region Private Members

        bool _continue;
        SerialPort _serialPort;
        Thread _readThread;

        int _replyTimeout = 300000;
        bool _replyreceived;
        bool _abort=false;

        #endregion

        #region Events

        public delegate void CommandEventHandler(object com, ArduinoSerialCommunicationEventArgs info);

        // The event we publish
        public event CommandEventHandler CommandSending;
        public event CommandEventHandler CommandSent;
        public event CommandEventHandler CommandWaitSingleStepContinue;
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

			ResetOnConnect = false;
        }

        #endregion 

        #region Properties

        public int BaudRate { get; set; }

		public bool ResetOnConnect { get; set; }
		public String OkTag { get; set; }
        public String ErrorTag { get; set; }
        public String InfoTag { get; set; }

		public bool ErrorIsReply { get; set; }

//      public bool IsConnected { get { return true; }  }
		public bool IsConnected { get { return _serialPort != null && _serialPort.IsOpen; } }

        public int MaxCommandHistoryCount { get; set; }

		[Flags]
		public enum EReplyType
		{
			ReplyOK=1,
			ReplyError=2,
			ReplyInfo=4,
			ReplyUnkown=8
		};

        #endregion

        #region Control Properties

        public bool Abort
        {
            get { return _abort; }
            protected set { _abort = value; }
        }

        #endregion

        #region Setup/Init Methodes

        public void Connect(string portname)
        {
            // Create a new SerialPort object with default settings.
            SetupCom(portname);

            _serialPort.Open();
            _readThread = new Thread(Read);
            _continue = true;
            _readThread.Start();

			if (!ResetOnConnect)
			{
				_serialPort.DiscardOutBuffer();
				_serialPort.WriteLine("");
			}
        }
        
        public void Disconnect()
        {
            Abort = true;
            _continue = false;
            if (_readThread != null)
                _readThread.Join();
            _readThread = null;

            if (_serialPort != null)
            {
                _serialPort.Close();
                _serialPort.Dispose();
            }
            _serialPort = null;
        }

        public void AbortCommand()
        {
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

        #endregion

        #region overrides

        protected virtual string[] SplitCommand(string line)
        {
            return new string[] { line };
        }

        #endregion

        #region Send Command(s)

        public void SendCommand(string line)
        {
            MySendCommand(line,false);
        }

        public void SendCommands(string[] commands, bool singleStep)
        {
            if (commands != null)
            {
                foreach (string cmd in commands)
                {
                    MySendCommand(cmd, singleStep);
                    if (Abort)
                        break;
                }
            }
        }

		public void SendFile(string filename, bool singleStep)
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
				SendCommands(lines.ToArray(), singleStep);
			}
		}

        #endregion

        #region Internals

        private void MySendCommand(string line, bool singleStep)
        {
            string[] cmds = SplitCommand(line);
            foreach (string cmd in cmds)
            {
                if (OutCommand(cmd,singleStep))
                    break;
            }
        }

        private bool OutCommand(string cmd, bool singleStep)
        {
			lock (this)
			{
				if (string.IsNullOrEmpty(cmd))
					return true;

				Abort = false;
				_replyreceived = false;

				cmd = cmd.Replace('\t', ' ');

				var eventarg = new ArduinoSerialCommunicationEventArgs(cmd);
				OnCommandSending(eventarg);
				if (eventarg.Abort) return false;

				while (singleStep && !Abort)
				{
					eventarg = new ArduinoSerialCommunicationEventArgs(cmd);
					OnCommandWaitSingleStepContinue(eventarg);
					if (eventarg.Abort) return false;
					if (eventarg.Continue)
						break;
					Thread.Sleep(100);
				}

				if (Abort)
					return false;

				_serialPort.WriteLine(cmd);

				eventarg = new ArduinoSerialCommunicationEventArgs(cmd);
				OnCommandSent(eventarg);

				if (eventarg.Abort) return false;

				Stopwatch sw = new Stopwatch();
				sw.Start();
				while (!_replyreceived && !Abort)
				{
					Thread.Sleep(10);
					eventarg = new ArduinoSerialCommunicationEventArgs(cmd);
					OnCommandWaitReply(eventarg);
					if (eventarg.Abort) return false;

					if (sw.ElapsedMilliseconds > _replyTimeout)
						throw new TimeoutException();
				}

				return Abort;
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
                    message = message.Trim();
                    OnReplyReceived(new ArduinoSerialCommunicationEventArgs(message));

                    if (message.StartsWith((OkTag)))
                    {
                        _replyreceived = true;
						OnReplyDone(new ArduinoSerialCommunicationEventArgs(message));
                    }
                    else if (message.StartsWith(ErrorTag))
                    {
						if (ErrorIsReply)
							_replyreceived = true;
                        OnReplyError(new ArduinoSerialCommunicationEventArgs(message));
                    }
                    else if (message.StartsWith(InfoTag))
                    {
                        OnReplyInfo(new ArduinoSerialCommunicationEventArgs(message));
                    }
                    else
                    {
                        OnReplyUnknown(new ArduinoSerialCommunicationEventArgs(message));
                    }
                }
            }
        }

        #endregion

        #region OnEvents

        protected virtual void OnCommandSending(ArduinoSerialCommunicationEventArgs info)
        {
            if (_commands.Count > MaxCommandHistoryCount)
            {
                _commands.RemoveAt(0);
            }

            _commands.Add(new SentCommands() { Command = info.Info, SentTime = DateTime.Now });

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
        protected virtual void OnCommandWaitSingleStepContinue(ArduinoSerialCommunicationEventArgs info)
        {
            if (CommandWaitSingleStepContinue != null)
            {
                CommandWaitSingleStepContinue(this, info);
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
            if (_commands.Count > 0)
            {
                string result = _commands.Last().Result;
                if (string.IsNullOrEmpty(result))
                    result = info.Info;
                else
                    result = result + "\n" + info.Info;

                _commands.Last().Result = result;
            }
 
            if (ReplyReceived != null)
            {
                ReplyReceived(this, info);
            }
        }

        protected virtual void OnReplyInfo(ArduinoSerialCommunicationEventArgs info)
        {
            if (_commands.Count > 0)
			{
                _commands.Last().ReplyType |= EReplyType.ReplyInfo;
			}

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
        public class SentCommands
        {
            public DateTime SentTime { get; set; }
            public String Command { get; set; }
            public String Result { get; set; }
			public EReplyType ReplyType { get; set; }
        }

        List<SentCommands> _commands = new List<SentCommands>();
        public List<SentCommands> CommandHistory { get { return _commands; } }

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
					foreach (Framework.Logic.ArduinoSerialCommunication.SentCommands cmds in CommandHistory)
					{
						sr.Write(cmds.SentTime); sr.Write(":");
						sr.Write(cmds.Command); sr.Write(" => ");
						sr.WriteLine(cmds.Result);
					}
				}
			}
		}

        #endregion

    }
}
