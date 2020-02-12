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

using CNCLib.Serial.WebAPI.Hubs;

using Framework.Arduino.SerialCommunication.Abstraction;
using Framework.Dependency;
using Framework.Tools;

using Microsoft.AspNetCore.SignalR;

namespace CNCLib.Serial.WebAPI.SerialPort
{
    public class SerialPortWrapper
    {
        #region ctr/SignalR

        public static Func<IHubContext<CNCLibHub, ICNCLibHubClient>> OnCreateHub { get; set; }

        public void InitPort()
        {
            if (Serial == null)
            {
                Serial = AppService.GetRequiredService<ISerial>();

                Serial.CommandQueueEmpty += async (sender, e) => { await OnCreateHub().Clients.All.QueueEmpty(Id); };
                Serial.CommandQueueChanged += (sender, e) =>
                {
                    _delayExecuteQueueChanged.Execute(
                        1000,
                        () => _pendingLastQueueLength = e.QueueLength,
                        () => { OnCreateHub().Clients.All.QueueChanged(Id, _pendingLastQueueLength); });
                };
                Serial.CommandSending += (sender, e) =>
                {
                    _delayExecuteSendingCommand.Execute(
                        1000,
                        () => _pendingSendingCommandSeqId = e.SeqId,
                        () => { OnCreateHub().Clients.All.SendingCommand(Id, _pendingSendingCommandSeqId); });
                };
            }
        }

        readonly DelayedExecution _delayExecuteQueueChanged = new DelayedExecution();
        int                       _pendingLastQueueLength;

        readonly DelayedExecution _delayExecuteSendingCommand = new DelayedExecution();
        int                       _pendingSendingCommandSeqId;

        #endregion

        #region Properties

        public int Id { get; set; }

        public string PortName { get; set; }

        public ISerial Serial { get; set; }

        public bool IsConnected => Serial?.IsConnected ?? false;

        public bool IsAborted => Serial?.Aborted ?? false;

        public bool IsSingleStep => Serial?.Pause ?? false;

        public string GCodeCommandPrefix { get; set; } = "";

        public int CommandsInQueue => Serial?.CommandsInQueue ?? 0;

        #endregion
    }
}