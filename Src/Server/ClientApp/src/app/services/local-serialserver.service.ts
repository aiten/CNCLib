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
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { SerialServerService } from './serialserver.service';
import { SerialCommand } from "../models/serial.command";
import { SerialPortDefinition } from '../models/serial.port.definition';
import { CNCLibServerInfo } from '../models/CNCLib.Server.Info';
import { QueueSendCommand } from '../models/queue.send.command';

@Injectable()
export class LocalSerialServerService implements SerialServerService {
  constructor(
    private http: HttpClient,
    @Inject('WEBAPI_URL') public baseUrl: string,
  ) {
  }

  basicAuth: string;


  setBaseUrl(baseUrl: string, basicAuth: string) {
    this.baseUrl = baseUrl;
    this.basicAuth = basicAuth;
  }

  getHeaders(): HttpHeaders {
    return new HttpHeaders({ 'Authorization': 'Basic ' + this.basicAuth });
  }

  getInfo(): Promise<CNCLibServerInfo> {
    return this.http.get<CNCLibServerInfo>(this.baseUrl + 'api/Info', { headers: this.getHeaders() }).toPromise()
      .catch(this.handleErrorPromise);
  }

  getPorts(): Promise<SerialPortDefinition[]> {
    return this.http.get<SerialPortDefinition[]>(this.baseUrl + 'api/SerialPort', { headers: this.getHeaders() }).toPromise()
      .catch(this.handleErrorPromise);
  }

  getPort(id: number): Promise<SerialPortDefinition> {
    return this.http.get<SerialPortDefinition>(this.baseUrl + 'api/SerialPort/' + id, { headers: this.getHeaders() }).toPromise();
  }

  getPortByName(port: string): Promise<SerialPortDefinition> {
    let params = new HttpParams();
    params = params.set('portName', port);
    return this.http.get<SerialPortDefinition[]>(this.baseUrl + 'api/SerialPort?' + params.toString(), { headers: this.getHeaders() })
      .toPromise()
      .then(result => result[0]);
  }

  refresh(): Promise<SerialPortDefinition[]> {
    return this.http.post<SerialPortDefinition[]>(this.baseUrl + 'api/SerialPort/refresh', "x", { headers: this.getHeaders() }).toPromise()
      .catch(this.handleErrorPromise);
  }

  connect(serialportid: number, baudrate: number, dtrIsReset: boolean, resetonConnect: boolean): Promise<void> {

    let params = new HttpParams();
    params = params.set('baudrate', baudrate.toString());
    params = params.set('dtrIsReset', (dtrIsReset ? 'true' : 'false'));
    params = params.set('resetOnConnect', (resetonConnect ? 'true' : 'false'));

    return this.http.post<void>(this.baseUrl + 'api/SerialPort/' + serialportid + '/connect/?' + params.toString(), "x", { headers: this.getHeaders() }).toPromise()
      .catch(this.handleErrorPromise);
  }

  disconnect(serialportid: number): Promise<void> {
    return this.http.post<void>(this.baseUrl + 'api/SerialPort/' + serialportid + '/disconnect', "x", { headers: this.getHeaders() }).toPromise()
      .catch(this.handleErrorPromise);
  }

  abort(serialportid: number): Promise<void> {
    return this.http.post<void>(this.baseUrl + 'api/SerialPort/' + serialportid + '/abort', "x", { headers: this.getHeaders() }).toPromise()
      .catch(this.handleErrorPromise);
  }

  resume(serialportid: number): Promise<void> {
    return this.http.post<void>(this.baseUrl + 'api/SerialPort/' + serialportid + '/resume', "x", { headers: this.getHeaders() }).toPromise()
      .catch(this.handleErrorPromise);
  }

  getHistory(serialportid: number): Promise<SerialCommand[]> {
    return this.http.get<SerialCommand[]>(this.baseUrl + 'api/SerialPort/' + serialportid + '/history', { headers: this.getHeaders() }).toPromise()
      .catch(this.handleErrorPromise);
  }

  clearHistory(serialportid: number): Promise<void> {
    return this.http.post<void>(this.baseUrl + 'api/SerialPort/' + serialportid + '/history/clear', "x", { headers: this.getHeaders() }).toPromise()
      .catch(this.handleErrorPromise);
  }

  queueCommands(serialportid: number, command: string[], timeout: number): Promise<SerialCommand[]> {
    let cmd = new QueueSendCommand();
    cmd.commands = command;
    cmd.timeOut = timeout;

    return this.http.post<SerialCommand[]>(this.baseUrl + 'api/SerialPort/' + serialportid + '/queue', cmd, { headers: this.getHeaders() }).toPromise()
      .catch(this.handleErrorPromise);
  }

  sendCommand(serialportid: number, command: string[], timeout: number): Promise<SerialCommand[]> {
    let cmd = new QueueSendCommand();
    cmd.commands = command;
    cmd.timeOut = timeout;

    return this.http.post<SerialCommand[]>(this.baseUrl + 'api/SerialPort/' + serialportid + '/send', cmd, { headers: this.getHeaders() }).toPromise()
      .catch(this.handleErrorPromise);
  }

  sendWhileOkCommands(serialportid: number, command: string[], timeout: number): Promise<SerialCommand[]> {
    let cmd = new QueueSendCommand();
    cmd.commands = command;
    cmd.timeOut = timeout;

    return this.http.post<SerialCommand[]>(this.baseUrl + 'api/SerialPort/' + serialportid + '/sendWhileOk', cmd, { headers: this.getHeaders() })
      .toPromise()
      .catch(this.handleErrorPromise);
  }

  getParameter(serialportid: number, parameterNo): Promise<number> {

    let params = new HttpParams();
    params = params.set('paramNo', parameterNo.toString());

    return this.http.post<number>(this.baseUrl + 'api/GCode/' + serialportid + '/getParameter?' + params.toString(), "x", { headers: this.getHeaders() }).toPromise()
      .catch(this.handleErrorPromise);
  }

  getPosition(serialportid: number): Promise<number[][]> {

    return this.http.post<number[][]>(this.baseUrl + 'api/GCode/' + serialportid + '/getPosition', "x", { headers: this.getHeaders() }).toPromise()
      .catch(this.handleErrorPromise);
  }

  private handleErrorPromise(error: Response | any) {
    console.error(error.message || error);
    return Promise.reject(error.message || error);
  }
}
