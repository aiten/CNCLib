////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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
using System.Threading.Tasks;
using System.Diagnostics;

namespace Framework.Arduino
{
	public class ArduinoSerialCommunication : IDisposable
    {
        #region Private Members

        SerialPort _serialPort;
		CancellationTokenSource _serialPortCancellationTokenSource;
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
		}

		List<Command> _pendingCommands = new List<Command>();

        #endregion

        #region Events

        public delegate void CommandEventHandler(object sender, ArduinoSerialCommunicationEventArgs e);

        // The event we publish
        public event CommandEventHandler WaitForSend;
        public event CommandEventHandler CommandSending;
        public event CommandEventHandler CommandSent;
        public event CommandEventHandler WaitCommandSent;
        public event CommandEventHandler ReplyReceived;
        public event CommandEventHandler ReplyOK;
        public event CommandEventHandler ReplyError;
        public event CommandEventHandler ReplyInfo;
        public event CommandEventHandler ReplyUnknown;
		public event CommandEventHandler CommandQueueChanged;
		public event CommandEventHandler CommandQueueEmpty;

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
            public string CommandText { get; set; }

            public EReplyType ReplyType { get; set; }
            public DateTime? ReplyReceivedTime { get; set; }

            public string ResultText { get; set; }

			public object Tag { get; set; }
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
		public string OkTag { get; set; }				= @"ok";
		public string ErrorTag { get; set; }			= @"error:";
		public string InfoTag { get; set; }				= @"info:";
		public bool CommandToUpper { get; set; }		= false;
		public bool ErrorIsReply { get; set; }			= true;			// each command must end with "ok" of "Error"
        public int MaxCommandHistoryCount { get; set; } = int.MaxValue;
		public int ArduinoBuffersize { get; set; }		= 64;
        public int ArduinoLineSize { get; set; } = 128;
        public TraceStream Trace { get { return _trace; } }

		public bool Pause { get; set; } = false;
		public bool SendNext { get; set; } = false;

		private bool Continue {  get {	return (_serialPortCancellationTokenSource != null && !_serialPortCancellationTokenSource.IsCancellationRequested);	} 	}

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

			_serialPortCancellationTokenSource = new CancellationTokenSource();
			_serialPort.Open();

			_readThread = new Thread(Read);
			_writeThread = new Thread(Write);

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

			OnComandQueueEmpty(new ArduinoSerialCommunicationEventArgs(null, null));
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
			_serialPortCancellationTokenSource.Cancel();


			if (join && _readThread != null)
			{
				_readThread.Abort();
			}
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
				_serialPortCancellationTokenSource.Dispose();
				_serialPort = null;
				_serialPortCancellationTokenSource = null;

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

		protected virtual void SetupCom(string portname)
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

		/// <summary>
		/// write all pending (command with no reliy) to file
		/// Intended to be used if user abort queue because of an error
		/// </summary>
		/// <param name="filename"></param>
		public void WritePendingCommandsToFile(string filename)
		{
			using (StreamWriter sw = new StreamWriter(Environment.ExpandEnvironmentVariables(filename)))
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
	    /// Queue must be empty
	    /// </summary>
	    /// <param name="line">command line</param>
	    /// <param name="waitForMilliseconds"></param>
	    /// <returns>ok result from arduino or empty(if error)</returns>
	    public async Task<string> SendCommandAndReadOKReplyAsync(string line, int waitForMilliseconds=int.MaxValue)
		{
			string message = null;
			if (await WaitUntilNoPendingCommandsAsync(waitForMilliseconds))
			{
				var commands = QueueCommand(line);
				if (await WaitUntilCommandsDoneAsync(commands, waitForMilliseconds))
				{
					var lastCmd = commands.Last();
					if (lastCmd.ReplyType == EReplyType.ReplyOK)
						message = lastCmd.ResultText;
				}
			}
			return message;
		}

		/// <summary>
		/// Send command and wait until the command is transfered and we got a reply (no command pending)
		/// </summary>
		/// <param name="line">command line to send</param>
		public async Task<IEnumerable<Command>> SendCommandAsync(string line, int waitForMilliseconds = int.MaxValue)
        {
            var ret = SplitAndQueueCommand(line);
			if (!await WaitUntilNoPendingCommandsAsync(waitForMilliseconds))
				throw new TimeoutException();

			return ret;
        }

		/// <summary>
		/// Send command and wait until the command is transfered and we got a reply (no command pending)
		/// </summary>
		/// <param name="line">command line to send</param>
		public IEnumerable<Command> SendCommand(string line)
		{
			return SendCommandAsync(line).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		/// <summary>
		/// Queue command - do not wait - not for transfer and not for replay
		/// </summary>
		/// <param name="line">command line to send</param>
		public IEnumerable<Command> QueueCommand(string line)
		{
			return SplitAndQueueCommand(line);
		}

		/// <summary>
		/// Send multiple command lines to the arduino. Wait until the commands are transferrd and we got a reply (no command pending)
		/// </summary>
		/// <param name="commands"></param>
		public async Task<IEnumerable<Command>> SendCommandsAsync(IEnumerable<string> commands)
        {
            if (commands != null)
            {
				var ret = QueueCommands(commands);
				await WaitUntilNoPendingCommandsAsync();
				return ret;
            }
			return new List<Command>();
		}

		/// <summary>
		/// Send multiple command lines to the arduino. Do no wait
		/// </summary>
		/// <param name="commands"></param>
		public IEnumerable<Command> QueueCommands(IEnumerable<string> commands)
		{
			var list = new List<Command>();

			if (commands != null)
			{
				foreach (string cmd in commands)
				{
					var cmds = SplitAndQueueCommand(cmd);
					if (Aborted)
						break;
					list.AddRange(cmds);
				}
			}
			return list;
		}

		/// <summary>
		/// Send commands stored in a file. Wait until the commands are transferrd and we got a reply (no command pending)
		/// </summary>
		/// <param name="filename">used for a StreamReader</param>
		public async Task<IEnumerable<Command>> SendFileAsync(string filename)
		{
			var list = QueueFile(filename);
			await WaitUntilNoPendingCommandsAsync();
			return list;
		}

		/// <summary>
		/// Send commands stored in a file. Wait until the commands are transferrd and we got a reply (no command pending)
		/// </summary>
		/// <param name="filename">used for a StreamReader</param>
		public IEnumerable<Command> QueueFile(string filename)
		{
			using (StreamReader sr = new StreamReader(filename))
			{
				Aborted = false;
                string line;
				List<string> lines = new List<string>();
				while ((line = sr.ReadLine()) != null && !Aborted)
				{
					lines.Add(line);
				}
				return QueueCommands(lines.ToArray());
			}
		}

		/// <summary>
		/// Wait until any response is received from the arduino
		/// Use the function e.g. after a reset to receive the boot message
		/// No command must be sent
		/// </summary>
		/// <param name="maxMilliseconds"></param>
		/// <returns></returns>
		public async Task<string> WaitUntilResonseAsync(int maxMilliseconds = int.MaxValue)
		{
			string message = null;
			var checkresponse = new ArduinoSerialCommunication.CommandEventHandler((obj, e) =>
			{
				message = e.Info;
			});

			try
			{
				ReplyReceived += checkresponse;

				var sw = Stopwatch.StartNew();
				while (Continue && message == null && sw.ElapsedMilliseconds < maxMilliseconds)
				{
					if (_autoEvent.WaitOne(10) == false)
						await Task.Delay(1);
				}
			}
			finally
			{
				ReplyReceived -= checkresponse;
			}

			return message;
		}


		#endregion

		#region Internals

		private IEnumerable<Command> SplitAndQueueCommand(string line)
        {
			var cmdlist = new List<Command>();
            string[] cmds = SplitCommand(line);
            foreach (string cmd in cmds)
            {
                Command pcmd = QueueCommandString(cmd);
                if (pcmd != null)
                {
					// sending is done in Write-Thread
					cmdlist.Add(pcmd);
                }
            }
			return cmdlist;
        }
        private Command QueueCommandString(string cmd)
        {
            if (string.IsNullOrEmpty(cmd))
                return null;

			if (CommandToUpper)
				cmd = cmd.ToUpper();

			cmd = cmd.Replace('\t', ' ');

			Command c = new Command() { CommandText = cmd };

			lock (_pendingCommands)
            {
				if (_pendingCommands.Count==0)
					_autoEvent.Set();			// start Async task now!

                _pendingCommands.Add(c);
            }
			Trace.WriteTrace("Queue", cmd);
			OnComandQueueChanged(new ArduinoSerialCommunicationEventArgs(null, c));
			return c;
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

            const int CRLF_SIZE = 2;

            if (commandtext.Length >= ArduinoLineSize + CRLF_SIZE) 
                commandtext = commandtext.Substring(0, ArduinoLineSize - 1 - CRLF_SIZE);

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
                Trace.WriteTraceFlush("WriteInvalidOperationException", $@"{commandtext} => {e.Message}");
                Disconnect(false);
            }
            catch (IOException e)
            {
                Trace.WriteTraceFlush("WriteIOException", $@"{commandtext} => {e.Message}");
                ErrorSerial();
            }
            catch (Exception e)
            {
                Trace.WriteTraceFlush("WriteException", $@"{commandtext} => {e.GetType().ToString()} {e.Message}");
            }
            return false;
        }

        private bool WriteLineSerial(string commandtext)
        {
            Trace.WriteTrace("Write", $@"{commandtext}\n");
            try
            {
                _serialPort.WriteLine(commandtext);
                return true;
            }
            catch (InvalidOperationException e)
            {
                Trace.WriteTraceFlush("WriteInvalidOperationException", $@"{commandtext}\n => {e.Message}");
                Disconnect(false);
            }
            catch (IOException e)
            {
                Trace.WriteTraceFlush("WriteIOException", $@"{commandtext}\n => {e.Message}");
                ErrorSerial();
            }
            catch (Exception e)
            {
                Trace.WriteTraceFlush("WriteException", $@"{commandtext}\n => {e.GetType().ToString()} {e.Message}");
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

		/// <summary>
		/// Wait until command queue is empty
		/// </summary>
		/// <param name="maxMilliseconds"></param>
		/// <returns>true = ok, false = timeout or aborting</returns>
		private async Task<bool> WaitUntilNoPendingCommandsAsync(int maxMilliseconds=int.MaxValue)
		{
			var sw = Stopwatch.StartNew();
			while (Continue)
            {
                Command cmd = null;
                lock (_pendingCommands)
                {
					if (_pendingCommands.Count > 0)
					{
						cmd = _pendingCommands[0];
					}
                }

				if (cmd == null)
					return true;

				var eventarg = new ArduinoSerialCommunicationEventArgs(null,cmd);
                OnWaitCommandSent(eventarg);
                if (Aborted || eventarg.Abort)
					return false;

				if (_autoEvent.WaitOne(10) == false)
					await Task.Delay(1);

				if (sw.ElapsedMilliseconds > maxMilliseconds)
					return false;
            }
			return false; // aborting
		}

		private async Task<bool> WaitUntilCommandsDoneAsync(IEnumerable<Command> commands, int maxMilliseconds = int.MaxValue)
		{
			var sw = Stopwatch.StartNew();
			while (Continue)
			{
				var noReplayCmd = commands.FirstOrDefault((cmd) => cmd.ReplyType == EReplyType.NoReply);

				if (noReplayCmd == null)
					return true;

				// wait
				var eventarg = new ArduinoSerialCommunicationEventArgs(null, noReplayCmd);
				OnWaitCommandSent(eventarg);
				if (Aborted || eventarg.Abort)
					return false;

				if (_autoEvent.WaitOne(10) == false)
					await Task.Delay(1);

				if (sw.ElapsedMilliseconds > maxMilliseconds)
					return false;
			}
			return false; // aborting
		}

		private void Write()
        {
			// Aync write thread to send commands to the arduino

            while (Continue)
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

				if (nextcmd != null && (!Pause || SendNext))
				{
					if (queuedcmdlenght == 0 || queuedcmdlenght + nextcmd.CommandText.Length + 2 < ArduinoBuffersize)
					{
						// send everyting if queue is empty
						// or send command if pending commands + this fit into arduino queue
						SendCommand(nextcmd);
						SendNext = false;
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

		private string ReadFromSerial()
		{
			int readmaxsize = 256;
			byte[] buffer = new byte[readmaxsize];
			int readsize = _serialPort.BaseStream.ReadAsync(buffer, 0, readmaxsize, _serialPortCancellationTokenSource.Token).ConfigureAwait(false).GetAwaiter().GetResult();
			return System.Text.Encoding.Default.GetString(buffer, 0, readsize);

		}

        private void Read()
        {
            var sb = new StringBuilder();

			while (Continue)
			{
				try
				{
					sb.Append(ReadFromSerial());
				}
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

				var inputbuffer = sb.ToString();

				int idx;
				while ((idx= inputbuffer.IndexOf('\n')) >= 0)
				{
					string message = inputbuffer.Substring(0, idx+1);
					sb.Remove(0,idx+1);
					inputbuffer = sb.ToString();
					MessageReceived(message);
				}
			}
		}

		private void MessageReceived(string message)
		{
			Command cmd = null;
			lock (_pendingCommands)
			{
				if (_pendingCommands.Count > 0)
					cmd = _pendingCommands[0];
			}

			if (string.IsNullOrEmpty(message))
			{
				if (cmd == null)
				{
					// timeout and no queue
					Framework.Tools.WinAPIWrapper.AllowIdle();
				}
			}
			else
			{
				Trace.WriteTrace("Read", message.Replace("\n", @"\n").Replace("\r", @"\r").Replace("\t", @"\t"));

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


				OnReplyReceived(new ArduinoSerialCommunicationEventArgs(message, cmd));

				if (message.StartsWith((OkTag)))
				{
					endcommand = true;
					OnReplyDone(new ArduinoSerialCommunicationEventArgs(message, cmd));
				}
				else if (message.StartsWith(ErrorTag, StringComparison.OrdinalIgnoreCase))
				{
					if (ErrorIsReply)
						endcommand = true;
					OnReplyError(new ArduinoSerialCommunicationEventArgs(message, cmd));
				}
				else if (message.StartsWith(InfoTag, StringComparison.OrdinalIgnoreCase))
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
					bool isEmpty = false;
					lock (_pendingCommands)
					{
						if (_pendingCommands.Count > 0) // may cause because of a reset
							_pendingCommands.RemoveAt(0);

						isEmpty = _pendingCommands.Count == 0;
						_autoEvent.Set();
					}
					OnComandQueueChanged(new ArduinoSerialCommunicationEventArgs(null, cmd));

					if (isEmpty)
						OnComandQueueEmpty(new ArduinoSerialCommunicationEventArgs(null, cmd));
				}
			}
		}

		#endregion

		#region OnEvents
		protected virtual void OnWaitForSend(ArduinoSerialCommunicationEventArgs info)
        {
			if (WaitForSend!=null)
				Task.Run(() => WaitForSend?.Invoke(this, info));
        }

        protected virtual void OnCommandSending(ArduinoSerialCommunicationEventArgs info)
        {
			if (CommandSending != null)
				Task.Run(() => CommandSending?.Invoke(this, info));
        }
        protected virtual void OnCommandSent(ArduinoSerialCommunicationEventArgs info)
        {
			if (CommandSent != null)
				Task.Run(() => CommandSent?.Invoke(this, info));
        }
        protected virtual void OnWaitCommandSent(ArduinoSerialCommunicationEventArgs info)
        {
			if (WaitCommandSent != null)
				Task.Run(() => WaitCommandSent?.Invoke(this, info));
        }
        protected virtual void OnReplyReceived(ArduinoSerialCommunicationEventArgs info)
        {
			if (ReplyReceived != null)
				Task.Run(() => ReplyReceived?.Invoke(this, info));
        }

        protected virtual void OnReplyInfo(ArduinoSerialCommunicationEventArgs info)
        {
			if (ReplyInfo != null)
				Task.Run(() => ReplyInfo?.Invoke(this, info));
        }
        protected virtual void OnReplyError(ArduinoSerialCommunicationEventArgs info)
        {
			WriteLastCommandReplyType(EReplyType.ReplyError);

			if (ReplyError != null)
				Task.Run(() => ReplyError?.Invoke(this, info));
        }
        protected virtual void OnReplyDone(ArduinoSerialCommunicationEventArgs info)
        {
			WriteLastCommandReplyType(EReplyType.ReplyOK);

			if (ReplyOK != null)
				Task.Run(() => ReplyOK?.Invoke(this, info));
        }
        protected virtual void OnReplyUnknown(ArduinoSerialCommunicationEventArgs info)
        {
			WriteLastCommandReplyType(EReplyType.ReplyUnkown);

			if (ReplyUnknown != null)
				Task.Run(() => ReplyUnknown?.Invoke(this, info));
        }

		protected virtual void OnComandQueueChanged(ArduinoSerialCommunicationEventArgs info)
		{
			if (CommandQueueChanged!=null)
				Task.Run(()=>CommandQueueChanged?.Invoke(this, info));
		}
		protected virtual void OnComandQueueEmpty(ArduinoSerialCommunicationEventArgs info)
		{
			if (CommandQueueEmpty != null)
				Task.Run(() => CommandQueueEmpty?.Invoke(this, info));
		}

		private void WriteLastCommandReplyType(EReplyType replytype)
		{
			lock (_commands)
			{
				if (_commands.Count > 0)
				{
					_commands.Last().ReplyType |= replytype;
				}
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
				using (StreamWriter sr = new StreamWriter(Environment.ExpandEnvironmentVariables(filename)))
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
