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

import { SerialCommand } from "../models/serial.command";
import { SerialPortDefinition } from '../models/serial.port.definition';
import { CNCLibServerInfo } from '../models/CNCLib.Server.Info'

export abstract class SerialServerService {

  public abstract setBaseUrl(baseUrl: string);

  public abstract getInfo(): Promise<CNCLibServerInfo>;

  public abstract getPorts(): Promise<SerialPortDefinition[]>;

  public abstract getPort(id: number): Promise<SerialPortDefinition>;

  public abstract getPortByName(port: string): Promise<SerialPortDefinition>;

  public abstract refresh(): Promise<SerialPortDefinition[]>;

  public abstract connect(serialportid: number, baudrate: number, dtrIsReset: boolean, resetonConnect: boolean): Promise<void>;

  public abstract disconnect(serialportid: number): Promise<void>;

  public abstract abort(serialportid: number): Promise<void>;

  public abstract resume(serialportid: number): Promise<void>;

  public abstract getHistory(serialportid: number): Promise<SerialCommand[]>;

  public abstract clearHistory(serialportid: number): Promise<void>;

  public abstract queueCommands(serialportid: number, command: string[], timeout: number): Promise<SerialCommand[]>;

  public abstract sendWhileOkCommands(serialportid: number, command: string[], timeout: number): Promise<SerialCommand[]>;
}
