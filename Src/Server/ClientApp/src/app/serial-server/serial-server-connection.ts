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

// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

import { Injectable } from '@angular/core';
import { SerialServerService } from '../services/serial-server.service';
import { MachineControlGlobal } from '../machine-control/machine-control.global';
import { PreviewGlobal } from '../preview/preview.global';

import { Machine } from '../models/machine';
import { SerialPortHistoryPreviewGlobal } from "../serialporthistory/models/serialporthistory.global";

@Injectable()
export class SerialServerConnection {

  constructor(
    private serialServerService: SerialServerService,
    public machineControlGlobal: MachineControlGlobal,
    public serialPortHistoryPreviewGlobal: SerialPortHistoryPreviewGlobal,
    public previewGlobal: PreviewGlobal
  ) {
  }

  private machine: Machine;

  private serialServerUrl: string;
  private serialServerPortId: number;
  private serialServerAuth: string;

  getMachine(): Machine {
    return this.machine;
  }

  getSerialServerUrl(): string {
    return this.serialServerUrl;
  }

  getSerialServerPortId(): number {
    return this.serialServerPortId;
  }

  getSerialServerAuth(): string {
    return this.serialServerAuth;
  }

  isConnected(machine: Machine) {
    if (this.machine != undefined && this.machine.id == machine.id) {
      return true;
    }
    return false;
  }

  disconnectFrom() {
    this.machine = undefined;
  }

  getSerialServerService(): SerialServerService {
    return this.serialServerService;
  }

  async connectAndRead(machine: Machine): Promise<string> {

    var url = await this.getInfoX(machine.serialServer,
      machine.serialServerUser,
      machine.serialServerPassword,
      machine.comPort,
      machine.baudRate,
      machine.dtrIsReset);

    return url;
  }

  async connectTo(machine: Machine): Promise<string> {

    var url = await this.getInfoX(machine.serialServer,
      machine.serialServerUser,
      machine.serialServerPassword,
      machine.comPort,
      machine.baudRate,
      machine.dtrIsReset);

    console.log(machine);
    this.machine = machine;

    this.machineControlGlobal.setFromMachine(machine);
    this.previewGlobal.setFromMachine(machine);

    this.serialPortHistoryPreviewGlobal.previewOpt = await this.serialServerService.getDefault(this.serialServerPortId);
    this.serialPortHistoryPreviewGlobal.setFromMachine(machine);

    return url;
  }

  async getInfoX(serialServer: string, username: string, password: string, comPort: string, baudrate: number, dtrIsReset: boolean): Promise<string> {

    var uri = serialServer + '/';
    var auth = window.btoa(username + ':' + password);

    this.serialServerService.setBaseUrl(uri, auth);

    var port = await this.serialServerService.getPortByName(comPort);

    if (!port.isConnected) {
      await this.serialServerService.connect(port.id, baudrate, dtrIsReset, false);
    }

    this.serialServerUrl = uri;
    this.serialServerPortId = port.id;
    this.serialServerAuth = auth;

    return this.serialServerUrl;
  }
};
