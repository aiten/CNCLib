////////////////////////////////////////////////////////
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

using System.Threading;
using System.Threading.Tasks;
using CNCLib.Serial.Server.Controllers;
using CNCLib.Serial.Server.Hubs;
using Framework.Arduino.SerialCommunication;
using Framework.Tools.Dependency;
using Framework.Tools.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace CNCLib.Serial.Server.SerialPort
{
    public class SerialPortWrapper
    {
        #region ctr/SignalR

        public void InitPort()
        {
            if (Serial == null)
            {
                Serial = Dependency.Container.Resolve<ISerial>();

                Serial.CommandQueueEmpty += async (sender, e) =>
                {
                    await Startup.Hub.Clients.All.SendAsync("queueEmpty", Id);
                };
                Serial.CommandQueueChanged += (sender, e) =>
                {
                    _delayExecuteQueueChanged.Execute(1000, () => _pendingLastQueueLength = e.QueueLenght,
                                                      () =>
                                                      {
                                                          Startup.Hub.Clients.All.SendAsync("queueChanged", Id,
                                                                                            _pendingLastQueueLength);
                                                      });
                };
                Serial.CommandSending += (sender, e) =>
                {
                    _delayExecuteSendingCommand.Execute(1000, () => _pendingSendingCommandSeqId = e.SeqId,
                                                        () =>
                                                        {
                                                            Startup.Hub.Clients.All.SendAsync("sendingCommand", Id,
                                                                                              _pendingSendingCommandSeqId);
                                                        });
                };
            }
        }

        DelayedExecution _delayExecuteQueueChanged = new DelayedExecution();
        int              _pendingLastQueueLength;

        DelayedExecution _delayExecuteSendingCommand = new DelayedExecution();
        int              _pendingSendingCommandSeqId;

        #endregion

        #region Properties

        public int Id { get; set; }

        public string PortName { get; set; }

        public ISerial Serial { get; set; }

        public bool IsConnected => Serial != null ? Serial.IsConnected : false;

        public bool IsAborted       => Serial != null ? Serial.Aborted : false;
        public bool IsSingleStep    => Serial != null ? Serial.Pause : false;
        public int  CommandsInQueue => Serial != null ? Serial.CommandsInQueue : 0;

        #endregion
    }
}