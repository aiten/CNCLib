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

import { Injectable, Inject } from '@angular/core';
import { Response } from '@angular/http';
import { HttpClient } from '@angular/common/http';
import { SerialServerService } from './serialserver.service';
import { SerialCommand } from "../models/serial.command";
import { SerialPortDefinition } from '../models/serial.port.definition';
import { CNCLibServerInfo } from '../models/CNCLib.Server.Info';
import { QueueSendCommand } from '../models/queue.send.command';

@Injectable()
export class LocalSerialServerService implements SerialServerService {
  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') public baseUrl: string,
  ) {
  }

  getInfo(): Promise<CNCLibServerInfo> {
    return this.http.get<CNCLibServerInfo>(this.baseUrl + 'api/Info').toPromise()
      .catch(this.handleErrorPromise);
  }

  getPorts(): Promise<SerialPortDefinition[]> {
    return this.http.get<SerialPortDefinition[]>(this.baseUrl + 'api/SerialPort').toPromise()
      .catch(this.handleErrorPromise);
  }

  getPort(id: number): Promise<SerialPortDefinition> {
    return this.http.get<SerialPortDefinition>(this.baseUrl + 'api/SerialPort/' + id).toPromise();
  }

  refresh(): Promise<SerialPortDefinition[]> {
    return this.http.post<SerialPortDefinition[]>(this.baseUrl + 'api/SerialPort/refresh', "x").toPromise()
      .catch(this.handleErrorPromise);
  }

  connect(serialportid: number, baudrate: number, dtrIsReset: boolean, resetonConnect: boolean): Promise<void> {
    return this.http
      .post<void>(this.baseUrl +
        'api/SerialPort/' +
        serialportid +
        '/connect/?baudrate=' +
        baudrate +
        '&dtrIsReset=' +
        (dtrIsReset ? 'true' : 'false') +
        '&resetOnConnect=' +
        (resetonConnect ? 'true' : 'false'),
        "x").toPromise()
      .catch(this.handleErrorPromise);
  }

  disconnect(serialportid: number): Promise<void> {
    return this.http.post<void>(this.baseUrl + 'api/SerialPort/' + serialportid + '/disconnect', "x").toPromise()
      .catch(this.handleErrorPromise);
  }

  abort(serialportid: number): Promise<void> {
    return this.http.post<void>(this.baseUrl + 'api/SerialPort/' + serialportid + '/abort', "x").toPromise()
      .catch(this.handleErrorPromise);
  }

  resume(serialportid: number): Promise<void> {
    return this.http.post<void>(this.baseUrl + 'api/SerialPort/' + serialportid + '/resume', "x").toPromise()
      .catch(this.handleErrorPromise);
  }

  getHistory(serialportid: number): Promise<SerialCommand[]> {
    return this.http.get<SerialCommand[]>(this.baseUrl + 'api/SerialPort/' + serialportid + '/history').toPromise()
      .catch(this.handleErrorPromise);
  }

  clearHistory(serialportid: number): Promise<void> {
    return this.http.post<void>(this.baseUrl + 'api/SerialPort/' + serialportid + '/history/clear', "x").toPromise()
      .catch(this.handleErrorPromise);
  }

  queueCommands(serialportid: number, command: string[], timeout: number): Promise<SerialCommand[]> {
    let cmd = new QueueSendCommand();
    cmd.Commands = command;
    cmd.TimeOut = timeout;

    return this.http.post<SerialCommand[]>(this.baseUrl + 'api/SerialPort/' + serialportid + '/queue', cmd).toPromise()
      .catch(this.handleErrorPromise);
  }

  sendWhileOkCommands(serialportid: number, command: string[], timeout: number): Promise<SerialCommand[]> {
    let cmd = new QueueSendCommand();
    cmd.Commands = command;
    cmd.TimeOut = timeout;

    return this.http.post<SerialCommand[]>(this.baseUrl + 'api/SerialPort/' + serialportid + '/sendWhileOk', cmd)
      .toPromise()
      .catch(this.handleErrorPromise);
  }

  private handleErrorPromise(error: Response | any) {
    console.error(error.message || error);
    return Promise.reject(error.message || error);
  }
}
