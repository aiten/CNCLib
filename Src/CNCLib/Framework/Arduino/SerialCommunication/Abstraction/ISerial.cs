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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Framework.Arduino.SerialCommunication.Abstraction
{
    public interface ISerial : IDisposable
    {
        #region Setup/Init

        Task ConnectAsync(string portName);
        Task DisconnectAsync();
        void AbortCommands();
        void ResumeAfterAbort();

        #endregion

        #region Pubic

        Task<IEnumerable<SerialCommand>> QueueCommandsAsync(IEnumerable<string> commands);
        Task<IEnumerable<SerialCommand>> SendCommandsAsync(IEnumerable<string>  commands, int maxMilliseconds);

        Task<string> WaitUntilResponseAsync(int maxMilliseconds);
        Task<bool> WaitUntilQueueEmptyAsync(int maxMilliseconds);

        #endregion;

        #region Events

        // The event we publish
        event CommandEventHandler WaitForSend;
        event CommandEventHandler CommandSending;
        event CommandEventHandler CommandSent;
        event CommandEventHandler WaitCommandSent;
        event CommandEventHandler ReplyReceived;
        event CommandEventHandler ReplyOK;
        event CommandEventHandler ReplyError;
        event CommandEventHandler ReplyInfo;
        event CommandEventHandler ReplyUnknown;
        event CommandEventHandler CommandQueueChanged;
        event CommandEventHandler CommandQueueEmpty;

        #endregion

        #region Properties

        bool IsConnected { get; }

        int CommandsInQueue { get; }

        IEnumerable<SerialCommand> PendingCommands { get; }

        bool Pause    { get; set; }
        bool SendNext { get; set; }

        int BaudRate { get; set; }

        /// <summary>
        /// Dtr is used as reset, e.g Arduino Uno, Mega, ..., not reset for Arduino Zero
        /// </summary>
        bool DtrIsReset { get; set; }

        /// <summary>
        /// Reset on Connect (only possible if DtrIsReset is true)
        /// </summary>
        bool ResetOnConnect { get; set; }

        string OkTag                  { get; set; }
        string ErrorTag               { get; set; }
        string InfoTag                { get; set; }
        bool   CommandToUpper         { get; set; }
        bool   ErrorIsReply           { get; set; }
        int    MaxCommandHistoryCount { get; set; }
        int    ArduinoBufferSize      { get; set; }
        int    ArduinoLineSize        { get; set; }
        bool   Aborted                { get; }

        #endregion

        #region CommandHistory

        SerialCommand       LastCommand        { get; }
        List<SerialCommand> CommandHistoryCopy { get; }
        void ClearCommandHistory();

        #endregion
    }
}