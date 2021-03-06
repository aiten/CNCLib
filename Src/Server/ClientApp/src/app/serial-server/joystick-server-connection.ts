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
import { JoystickServerService } from '../services/joystick-server.service';
import { MachineControlGlobal } from '../machine-control/machine-control.global';


@Injectable()
export class JoystickServerConnection {

  constructor(
    private joystickServerService: JoystickServerService,
    public machineControlGlobal: MachineControlGlobal,
  ) {
  }

  private serialServerUrl: string;
  private serialServerPortId: number;
  private serialServerAuth: string;

  getSerialServerUrl(): string {
    return this.serialServerUrl;
  }

  getSerialServerPortId(): number {
    return this.serialServerPortId;
  }

  getSerialServerAuth(): string {
    return this.serialServerAuth;
  }

  async getInfoX(serialServer: string, username: string, password: string, comPort: string): Promise<string> {

    var uri = serialServer + '/';
    var auth = window.btoa(username + ':' + password);

    this.joystickServerService.setBaseUrl(uri, auth);

    var port = await this.joystickServerService.getPortByName(comPort);

    if (!port.isConnected) {
      await this.joystickServerService.connect(port.id);
    }

    this.serialServerUrl = uri;
    this.serialServerPortId = port.id;
    this.serialServerAuth = auth;

    return this.serialServerUrl;
  }
};
