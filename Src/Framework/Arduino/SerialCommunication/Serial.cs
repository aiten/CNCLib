/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

namespace Framework.Arduino.SerialCommunication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Diagnostics;

    using Arduino.SerialCommunication.Abstraction;

    using Logging.Abstraction;

    using WinAPI;
    using Framework.Pattern;

    public class Serial : ISerial
    {
        #region Private Members

        ISerialPort             _serialPort;
        IFactory<ISerialPort>   _serialPortFactory;
        IScope<ISerialPort>     _serialPortScope;
        CancellationTokenSource _serialPortCancellationTokenSource;
        Thread                  _readThread;
        Thread                  _writeThread;
        int                     _commandSeqId;

        readonly AutoResetEvent      _autoEvent       = new AutoResetEvent(false);
        readonly List<SerialCommand> _pendingCommands = new List<SerialCommand>();

        #endregion

        public Serial(IFactory<ISerialPort> serialPortFactory, ILogger<Serial> logger)
        {
            Logger = logger;
            _serialPortFactory = serialPortFactory ?? throw new ArgumentException();
        }

        #region Events

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

        #region Properties

        public const int DefaultTimeout = 10 * 60 * 1000;

        public int CommandsInQueue
        {
            get
            {
                lock (_pendingCommands)
                {
                    return _pendingCommands.Count;
                }
            }
        }

        public IEnumerable<SerialCommand> PendingCommands
        {
            get
            {
                lock (_pendingCommands)
                {
                    return _pendingCommands.ToArray();
                }
            }
        }

        //public bool IsConnected { get { return true; }  }
        public bool IsConnected => _serialPort != null && _serialPort.IsOpen;

        public bool Aborted { get; protected set; }

        public int  BaudRate       { get; set; } = 115200;
        public bool ResetOnConnect { get; set; } = false;
        public bool DtrIsReset     { get; set; } = true; // true for Arduino Uno, Mega, false for Arduino Zero 

        public string OkTag                  { get; set; } = @"ok";
        public string ErrorTag               { get; set; } = @"error:";
        public string InfoTag                { get; set; } = @"info:";
        public bool   CommandToUpper         { get; set; } = false;
        public bool   ErrorIsReply           { get; set; } = true; // each command must end with "ok" of "Error"
        public int    MaxCommandHistoryCount { get; set; } = int.MaxValue;
        public int    ArduinoBufferSize      { get; set; } = 64;
        public int    ArduinoLineSize        { get; set; } = 128;
        public bool   Pause                  { get; set; } = false;
        public bool   SendNext               { get; set; } = false;

        private bool Continue => _serialPortCancellationTokenSource != null && !_serialPortCancellationTokenSource.IsCancellationRequested;

        protected ILogger Logger { get; }

        #endregion

        #region Setup/Init Methodes

        /// <summary>
        /// Connect to the Arduino serial port 
        /// </summary>
        /// <param name="portName">e.g. Com1</param>
        public async Task ConnectAsync(string portName)
        {
            await Task.Delay(0); // avoid CS1998
            Logger?.Trace($@"Connect: {portName}");

            // Create a new SerialPort object with default settings.
            Aborted = false;

            SetupCom(portName);

            _serialPortCancellationTokenSource = new CancellationTokenSource();

            _serialPort.Open();

            _readThread  = new Thread(Read);
            _writeThread = new Thread(Write);

            _readThread.Start();
            _writeThread.Start();

            // we need Dtr if not used as Reset
            // otherwise reset if dtr is set
            if (!DtrIsReset || ResetOnConnect)
            {
                _serialPort.DtrEnable = true;
            }
            else
            {
                _serialPort.DiscardOutBuffer();
                _serialPort.WriteLine("");
            }

            //_serialPort.DtrEnable = true;
            _serialPort.RtsEnable = true;

            bool wasEmpty;

            lock (_pendingCommands)
            {
                wasEmpty = _pendingCommands.Count == 0;
                _pendingCommands.Clear();
            }

            lock (_commands)
            {
                _commands.Clear();
            }

            if (!wasEmpty)
            {
                OnCommandQueueChanged(new SerialEventArgs(0, null));
            }

            OnCommandQueueEmpty(new SerialEventArgs(0, null));
        }

        /// <summary>
        /// Disconnect from arduino 
        /// </summary>
        public async Task DisconnectAsync()
        {
            await Disconnect(true);
        }

        private async Task Disconnect(bool join)
        {
            await Task.Delay(5);
            Logger?.Trace($"Disconnecting: {join.ToString()}");
            Aborted = true;
            _serialPortCancellationTokenSource?.Cancel();

            if (join && _readThread != null)
            {
                try
                {
                    _readThread.Abort();
                }
                catch (PlatformNotSupportedException)
                {
                    // ignore
                }
            }

            _readThread = null;

            if (join && _writeThread != null)
            {
                while (!_writeThread.Join(100))
                {
                    await Task.Delay(1);
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

                _serialPortCancellationTokenSource?.Dispose();
                _serialPortScope?.Dispose();
                _serialPort                        = null;
                _serialPortCancellationTokenSource = null;
            }

            Logger?.Trace($"Disconnected: {join.ToString()}");
        }

        /// <summary>
        /// Start "Abort", but leave communication opened. but do not send and command.
        /// All pending commands are removed
        /// </summary>
        public void AbortCommands()
        {
            bool wasEmpty;
            lock (_pendingCommands)
            {
                wasEmpty = _pendingCommands.Count == 0;
                _pendingCommands.Clear();
            }

            if (!wasEmpty)
            {
                OnCommandQueueChanged(new SerialEventArgs(0, null));
            }

            OnCommandQueueChanged(new SerialEventArgs(0, null));

            Aborted = true;
        }

        /// <summary>
        /// ResumeAfterAbort must be called after AbortCommands to continue. 
        /// </summary>
        public void ResumeAfterAbort()
        {
            if (!Aborted)
            {
                return;
            }

            while (true)
            {
                lock (_pendingCommands)
                {
                    if (_pendingCommands.Count == 0)
                    {
                        break;
                    }
                }
            }

            Aborted = false;
        }

        protected virtual void SetupCom(string portName)
        {
            _serialPortScope = _serialPortFactory.Create();
            _serialPort = _serialPortScope.Instance;

            _serialPort.PortName  = portName;
            _serialPort.BaudRate  = BaudRate;
            _serialPort.Parity    = Parity.None;
            _serialPort.DataBits  = 8;
            _serialPort.StopBits  = StopBits.One;
            _serialPort.Handshake = Handshake.None;

            // Set the read/write timeouts
            _serialPort.ReadTimeout  = 500;
            _serialPort.WriteTimeout = 500;
        }

        public void Dispose()
        {
            DisconnectAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        #endregion

        #region overrides

        protected virtual string[] SplitCommand(string line)
        {
            return new[] { line };
        }

        #endregion

        #region Send Command(s)

        /// <summary>
        /// Send multiple command lines to the arduino. Wait until the commands are transferred and we got a reply (no command pending)
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="waitForMilliseconds"></param>
        public async Task<IEnumerable<SerialCommand>> SendCommandsAsync(IEnumerable<string> commands, int waitForMilliseconds)
        {
            if (commands != null)
            {
                var ret = await QueueCommandsAsync(commands);
                await WaitUntilQueueEmptyAsync(waitForMilliseconds);
                return ret;
            }

            return new List<SerialCommand>();
        }

        /// <summary>
        /// Send multiple command lines to the arduino. Do no wait
        /// </summary>
        /// <param name="commands"></param>
        public async Task<IEnumerable<SerialCommand>> QueueCommandsAsync(IEnumerable<string> commands)
        {
            var list = new List<SerialCommand>();

            if (commands != null)
            {
                int commandIndex = 0;
                foreach (string cmd in commands)
                {
                    var cmds = SplitAndQueueCommand(cmd);
                    if (Aborted)
                    {
                        break;
                    }

                    foreach (SerialCommand serialCommand in cmds)
                    {
                        serialCommand.CommandIndex = commandIndex;
                        list.Add(serialCommand);
                    }

                    commandIndex++;
                }
            }
            else
            {
                await Task.Delay(0); // avoid CS1998
            }

            return list;
        }

        /// <summary>
        /// Wait until any response is received from the arduino
        /// Use the function e.g. after a reset to receive the boot message
        /// No command must be sent
        /// </summary>
        /// <param name="maxMilliseconds"></param>
        /// <returns></returns>
        public async Task<string> WaitUntilResponseAsync(int maxMilliseconds)
        {
            string message       = null;
            var    checkResponse = new CommandEventHandler((obj, e) => { message = e.Info; });

            try
            {
                ReplyReceived += checkResponse;

                var sw = Stopwatch.StartNew();
                while (Continue && message == null && sw.ElapsedMilliseconds < maxMilliseconds)
                {
                    if (_autoEvent.WaitOne(10) == false)
                    {
                        await Task.Delay(1);
                    }
                }
            }
            finally
            {
                ReplyReceived -= checkResponse;
            }

            return message;
        }

        #endregion

        #region Internals

        private IEnumerable<SerialCommand> SplitAndQueueCommand(string line)
        {
            var      cmdList = new List<SerialCommand>();
            string[] cmds    = SplitCommand(line);
            foreach (string cmd in cmds)
            {
                SerialCommand cmdToQueue = QueueCommandString(cmd);
                if (cmdToQueue != null)
                {
                    // sending is done in Write-Thread
                    cmdList.Add(cmdToQueue);
                }
            }

            return cmdList;
        }

        private SerialCommand QueueCommandString(string cmd)
        {
            if (string.IsNullOrEmpty(cmd))
            {
                return null;
            }

            if (CommandToUpper)
            {
                cmd = cmd.ToUpper();
            }

            cmd = cmd.Replace('\t', ' ');

            var c = new SerialCommand { SeqId = _commandSeqId++, CommandText = cmd };
            int queueLength;

            lock (_pendingCommands)
            {
                queueLength = _pendingCommands.Count;
                if (queueLength == 0)
                {
                    _autoEvent.Set(); // start Async task now!
                }

                _pendingCommands.Add(c);
                queueLength++;
            }

            Logger?.Trace($"Queue: {cmd}");
            OnCommandQueueChanged(new SerialEventArgs(queueLength, c));
            return c;
        }

        private void SendCommand(SerialCommand cmd)
        {
            // SendCommands is called in the async Write thread 

            var eventArgs = new SerialEventArgs(cmd);
            OnCommandSending(eventArgs);

            if (eventArgs.Abort || Aborted)
            {
                return;
            }

            lock (_commands)
            {
                if (_commands.Count > MaxCommandHistoryCount)
                {
                    _commands.RemoveAt(0);
                }

                _commands.Add(cmd);
            }

            string commandText = cmd.CommandText;

            const int CRLF_SIZE = 2;

            if (commandText.Length >= ArduinoLineSize + CRLF_SIZE)
            {
                commandText = commandText.Substring(0, ArduinoLineSize - 1 - CRLF_SIZE);
            }

            while (commandText.Length > ArduinoBufferSize - 1)
            {
                // give "control" class the chance to read from arduino to control buffer

                int    firstSize = ArduinoBufferSize * 2 / 3;
                string firstX    = commandText.Substring(0, firstSize);
                commandText = commandText.Substring(firstSize);

                if (!WriteSerial(firstX, false))
                {
                    return;
                }

                Thread.Sleep(250);
            }

            if (WriteSerial(commandText, true))
            {
                cmd.SentTime = DateTime.Now;
                eventArgs    = new SerialEventArgs(cmd);
                OnCommandSent(eventArgs);
            }
        }

        private bool WriteSerial(string commandText, bool addNewLine)
        {
            Logger?.Trace($"Write: {commandText}");
            try
            {
                if (addNewLine)
                {
                    commandText = commandText + _serialPort.NewLine;
                }

                WriteToSerialAsync(commandText).ConfigureAwait(false).GetAwaiter().GetResult();
                return true;
            }
            catch (InvalidOperationException e)
            {
                Logger?.Error($"WriteInvalidOperationException: {commandText} => {e.Message}");
                Disconnect(false).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (IOException e)
            {
                Logger?.Error($"WriteIOException: {commandText} => {e.Message}");
                ErrorSerial();
            }
            catch (Exception e)
            {
                Logger?.Error($"WriteException: {commandText} => {e.GetType()} {e.Message}");
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
                Disconnect(false).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Wait until command queue is empty
        /// </summary>
        /// <param name="maxMilliseconds"></param>
        /// <returns>true = ok, false = timeout or aborting</returns>
        public async Task<bool> WaitUntilQueueEmptyAsync(int maxMilliseconds = DefaultTimeout)
        {
            var sw = Stopwatch.StartNew();
            while (Continue)
            {
                SerialCommand cmd = null;
                lock (_pendingCommands)
                {
                    if (_pendingCommands.Count > 0)
                    {
                        cmd = _pendingCommands[0];
                    }
                }

                if (cmd == null)
                {
                    return true;
                }

                var eventArgs = new SerialEventArgs(cmd);
                OnWaitCommandSent(eventArgs);
                if (Aborted || eventArgs.Abort)
                {
                    return false;
                }

                if (_autoEvent.WaitOne(10) == false)
                {
                    await Task.Delay(1);
                }

                if (sw.ElapsedMilliseconds > maxMilliseconds)
                {
                    return false;
                }
            }

            return false; // aborting
        }

        private async Task<bool> WaitUntilCommandsDoneAsync(IEnumerable<SerialCommand> commands, int maxMilliseconds)
        {
            var sw = Stopwatch.StartNew();
            while (Continue)
            {
                var noReplayCmd = commands.FirstOrDefault(cmd => cmd.ReplyType == EReplyType.NoReply);

                if (noReplayCmd == null)
                {
                    return true;
                }

                // wait
                var eventArgs = new SerialEventArgs(noReplayCmd);
                OnWaitCommandSent(eventArgs);
                if (Aborted || eventArgs.Abort)
                {
                    return false;
                }

                if (_autoEvent.WaitOne(10) == false)
                {
                    await Task.Delay(1);
                }

                if (sw.ElapsedMilliseconds > maxMilliseconds)
                {
                    return false;
                }
            }

            return false; // aborting
        }

        private void Write()
        {
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

            // Async write thread to send commands to the arduino

            while (Continue)
            {
                SerialCommand nextCmd         = null;
                int           queuedCmdLength = 0;

                // commands are sent to the arduino until the buffer is full
                // In the _pendingCommand list also the commands are still stored with no reply.

                lock (_pendingCommands)
                {
                    foreach (SerialCommand cmd in _pendingCommands)
                    {
                        if (cmd.SentTime.HasValue)
                        {
                            queuedCmdLength += cmd.CommandText.Length;
                            queuedCmdLength += 2; // CRLF
                        }
                        else
                        {
                            nextCmd = cmd;
                            break;
                        }
                    }
                }

                // nextCmd			=> next command to be sent
                // queuedCmdLength	=> length of command in the arduino buffer

                if (nextCmd != null && (!Pause || SendNext))
                {
                    if (queuedCmdLength == 0 || queuedCmdLength + nextCmd.CommandText.Length + 2 < ArduinoBufferSize)
                    {
                        // send everything if queue is empty
                        // or send command if pending commands + this fit into arduino queue
                        SendCommand(nextCmd);
                        SendNext = false;
                    }
                    else
                    {
                        var eventArgs = new SerialEventArgs(nextCmd);
                        OnWaitForSend(eventArgs);
                        if (Aborted || eventArgs.Abort)
                        {
                            return;
                        }

                        lock (_pendingCommands)
                        {
                            if (_pendingCommands.Count > 0 && _pendingCommands[0].SentTime.HasValue)
                            {
                                _autoEvent.Reset(); // expect an answer
                            }
                        }

                        _autoEvent.WaitOne(10);
                    }
                }
                else
                {
                    _autoEvent.WaitOne(100); // no command in queue => wait => CreateCommand(...) will set AutoEvent
                }
            }
        }

        public async Task WriteToSerialAsync(string str)
        {
            byte[] encodedStr = _serialPort.Encoding.GetBytes(str);

            await _serialPort.BaseStream.WriteAsync(encodedStr, 0, encodedStr.Length, _serialPortCancellationTokenSource.Token);
            await _serialPort.BaseStream.FlushAsync();
        }

        private async Task<string> ReadFromSerialAsync()
        {
            int readMaxSize = 256;
            var buffer      = new byte[readMaxSize];

            int readSize = await _serialPort.BaseStream.ReadAsync(buffer, 0, readMaxSize, _serialPortCancellationTokenSource.Token);
            return _serialPort.Encoding.GetString(buffer, 0, readSize);
        }

        private void Read()
        {
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

            var sb = new StringBuilder();

            while (Continue)
            {
                try
                {
                    sb.Append(ReadFromSerialAsync().ConfigureAwait(false).GetAwaiter().GetResult());
                }
                catch (InvalidOperationException e)
                {
                    Logger?.Error($"ReadInvalidOperationException: {e.Message}");
                    Thread.Sleep(250);
                }
                catch (ThreadAbortException)
                {
                    // terminated by user request
                    throw;
                }
                catch (IOException e)
                {
                    Logger?.Error($"ReadIOException: {e.Message}");
                    Thread.Sleep(250);
                }
                catch (Exception e)
                {
                    Logger?.Error($"ReadException: {e.Message}");
                    Thread.Sleep(250);
                }

                string inputBuffer = sb.ToString();

                int idx;
                while ((idx = inputBuffer.IndexOf('\n')) >= 0)
                {
                    string message = inputBuffer.Substring(0, idx + 1);
                    sb.Remove(0, idx + 1);
                    inputBuffer = sb.ToString();
                    MessageReceived(message);
                }
            }
        }

        private void MessageReceived(string message)
        {
            SerialCommand cmd = null;
            lock (_pendingCommands)
            {
                if (_pendingCommands.Count > 0)
                {
                    cmd = _pendingCommands[0];
                }
            }

            if (string.IsNullOrEmpty(message) == false)
            {
                Logger?.Trace($"Read: {message.Replace("\n", @"\n").Replace("\r", @"\r").Replace("\t", @"\t")}");

                bool endCommand = false;

                message = message.Trim();

                if (cmd != null)
                {
                    SetSystemKeepAlive();

                    string result = cmd.ResultText;
                    if (string.IsNullOrEmpty(result))
                    {
                        result = message;
                    }
                    else
                    {
                        result = result + "\n" + message;
                    }

                    cmd.ResultText        = result;
                    cmd.ReplyReceivedTime = DateTime.Now;
                }

                OnReplyReceived(new SerialEventArgs(message, cmd));

                if (message.StartsWith(OkTag))
                {
                    endCommand = true;
                    if (cmd != null)
                    {
                        cmd.ReplyType |= EReplyType.ReplyOK;
                    }

                    OnReplyDone(new SerialEventArgs(message, cmd));
                }
                else if (message.StartsWith(ErrorTag, StringComparison.OrdinalIgnoreCase))
                {
                    if (ErrorIsReply)
                    {
                        endCommand = true;
                    }

                    if (cmd != null)
                    {
                        cmd.ReplyType |= EReplyType.ReplyError;
                    }

                    OnReplyError(new SerialEventArgs(message, cmd));
                }
                else if (message.StartsWith(InfoTag, StringComparison.OrdinalIgnoreCase))
                {
                    if (cmd != null)
                    {
                        cmd.ReplyType |= EReplyType.ReplyInfo;
                    }

                    OnReplyInfo(new SerialEventArgs(message, cmd));
                }
                else
                {
                    if (cmd != null)
                    {
                        cmd.ReplyType |= EReplyType.ReplyUnknown;
                    }

                    OnReplyUnknown(new SerialEventArgs(message, cmd));
                }

                if (endCommand && cmd != null)
                {
                    int queueLength;
                    lock (_pendingCommands)
                    {
                        if (_pendingCommands.Count > 0) // may cause because of a reset
                        {
                            _pendingCommands.RemoveAt(0);
                        }

                        queueLength = _pendingCommands.Count;
                        _autoEvent.Set();
                    }

                    OnCommandQueueChanged(new SerialEventArgs(queueLength, cmd));

                    if (queueLength == 0)
                    {
                        OnCommandQueueEmpty(new SerialEventArgs(queueLength, cmd));
                        ClearSystemKeepAlive();
                    }
                }
            }
        }

        private static void SetSystemKeepAlive()
        {
            WinAPIWrapper.KeepAlive();
            WinAPIWrapper.ResetTimer();
        }

        private static void ClearSystemKeepAlive()
        {
            WinAPIWrapper.AllowIdle();
        }

        #endregion

        #region OnEvents

        protected virtual void OnWaitForSend(SerialEventArgs info)
        {
            if (WaitForSend != null)
            {
                Task.Run(() => WaitForSend?.Invoke(this, info));
            }
        }

        protected virtual void OnCommandSending(SerialEventArgs info)
        {
            if (CommandSending != null)
            {
                Task.Run(() => CommandSending?.Invoke(this, info));
            }
        }

        protected virtual void OnCommandSent(SerialEventArgs info)
        {
            if (CommandSent != null)
            {
                Task.Run(() => CommandSent?.Invoke(this, info));
            }
        }

        protected virtual void OnWaitCommandSent(SerialEventArgs info)
        {
            if (WaitCommandSent != null)
            {
                Task.Run(() => WaitCommandSent?.Invoke(this, info));
            }
        }

        protected virtual void OnReplyReceived(SerialEventArgs info)
        {
            if (ReplyReceived != null)
            {
                Task.Run(() => ReplyReceived?.Invoke(this, info));
            }
        }

        protected virtual void OnReplyInfo(SerialEventArgs info)
        {
            if (ReplyInfo != null)
            {
                Task.Run(() => ReplyInfo?.Invoke(this, info));
            }
        }

        protected virtual void OnReplyError(SerialEventArgs info)
        {
            if (ReplyError != null)
            {
                Task.Run(() => ReplyError?.Invoke(this, info));
            }
        }

        protected virtual void OnReplyDone(SerialEventArgs info)
        {
            if (ReplyOK != null)
            {
                Task.Run(() => ReplyOK?.Invoke(this, info));
            }
        }

        protected virtual void OnReplyUnknown(SerialEventArgs info)
        {
            if (ReplyUnknown != null)
            {
                Task.Run(() => ReplyUnknown?.Invoke(this, info));
            }
        }

        protected virtual void OnCommandQueueChanged(SerialEventArgs info)
        {
            if (CommandQueueChanged != null)
            {
                Task.Run(() => CommandQueueChanged?.Invoke(this, info));
            }
        }

        protected virtual void OnCommandQueueEmpty(SerialEventArgs info)
        {
            if (CommandQueueEmpty != null)
            {
                Task.Run(() => CommandQueueEmpty?.Invoke(this, info));
            }
        }

        #endregion

        #region Command History 

        readonly List<SerialCommand> _commands = new List<SerialCommand>();

        public List<SerialCommand> CommandHistoryCopy
        {
            get
            {
                lock (_commands)
                {
                    return new List<SerialCommand>(_commands);
                }
            }
        }

        public SerialCommand LastCommand
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

        #endregion
    }
}