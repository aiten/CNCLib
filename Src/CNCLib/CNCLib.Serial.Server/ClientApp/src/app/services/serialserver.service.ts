////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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

import { SerialCommand } from "../models/serial.command";
import { SerialPortDefinition } from '../models/serial.port.definition';
import { CNCLibServerInfo } from '../models/CNCLib.Server.Info'
//import { Observable } from 'rxjs/Observable';

export abstract class SerialServerService {
  public abstract getInfo(): Promise<CNCLibServerInfo>;

  public abstract getPorts(): Promise<SerialPortDefinition[]>;

  public abstract getPort(id: number): Promise<SerialPortDefinition>;

  public abstract refresh(): Promise<SerialPortDefinition[]>;

  public abstract connect(serialportid: number, baudrate: number, dtrIsReset: boolean, resetonConnect: boolean):
    Promise<void>;

  public abstract disconnect(serialportid: number): Promise<void>;

  public abstract abort(serialportid: number): Promise<void>;

  public abstract resume(serialportid: number): Promise<void>;

  public abstract getHistory(serialportid: number): Promise<SerialCommand[]>;

  public abstract clearHistory(serialportid: number): Promise<void>;

  public abstract queueCommands(serialportid: number, command: string[], timeout: number): Promise<SerialCommand[]>;

  public abstract sendWhileOkCommands(serialportid: number, command: string[], timeout: number):
    Promise<SerialCommand[]>;
}
