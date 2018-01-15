﻿////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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
using System.Threading.Tasks;

namespace Framework.Arduino.SerialCommunication
{
    public interface ISerial : IDisposable
    {
        #region Setup/Init
        void Connect(string portname);
        void Disconnect();
        void AbortCommands();
        void ResumeAfterAbort();

        #endregion

        #region Pubic

        IEnumerable<SerialCommand> SendCommand(string line);
        IEnumerable<SerialCommand> QueueCommand(string line);
        Task<IEnumerable<SerialCommand>> SendCommandAsync(string line, int waitForMilliseconds = int.MaxValue);
        Task<string> SendCommandAndReadOKReplyAsync(string line, int waitForMilliseconds = int.MaxValue);
        Task<IEnumerable<SerialCommand>> SendCommandsAsync(IEnumerable<string> commands);
        Task<string> WaitUntilResponseAsync(int maxMilliseconds = int.MaxValue);
        Task<IEnumerable<SerialCommand>> SendFileAsync(string filename);

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

        bool Pause { get; set; }
        bool SendNext { get; set; }

        Tools.Helpers.TraceStream Trace { get; }

        int BaudRate { get; set; }
        bool ResetOnConnect { get; set; }
        string OkTag { get; set; }
        string ErrorTag { get; set; }
        string InfoTag { get; set; }
        bool CommandToUpper { get; set; }
        bool ErrorIsReply { get; set; }
        int MaxCommandHistoryCount { get; set; }
        int ArduinoBuffersize { get; set; }
        int ArduinoLineSize { get; set; }

        #endregion

        #region CommandHistory

        SerialCommand LastCommand { get; }
        void WriteCommandHistory(string filename);
        List<SerialCommand> CommandHistoryCopy { get; }
        void ClearCommandHistory();
        void WritePendingCommandsToFile(string filename);

        #endregion
    }
}
