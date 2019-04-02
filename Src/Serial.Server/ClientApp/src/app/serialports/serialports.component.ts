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

import { Component, Inject, OnInit } from '@angular/core';
import { SerialPortDefinition } from '../models/serial.port.definition';
import { SerialServerService } from '../services/serialserver.service';
import { Router } from '@angular/router';
import { machinecontrolURL } from '../machinecontrol/machinecontrol.routing';

@Component({
  selector: 'serialports',
  templateUrl: './serialports.component.html'
})
export class SerialPortsComponent {
  public serialports!: SerialPortDefinition[];
  public historyportid: number = -1;

  constructor(
    private serivalServerService: SerialServerService,
    private router: Router
  ) {
  }

  async ngOnInit(): Promise<void> {
    await this.reload();
  }

  async reload(): Promise<void> {
    this.serialports = await this.serivalServerService.getPorts();
  }

  async refresh(): Promise<void> {
    this.serialports = await this.serivalServerService.refresh();
  }

  showHistory(showhistoryportid: number) {
    this.historyportid = showhistoryportid;
  }

  machineControl(showhistoryportid: number) {
    this.router.navigate([machinecontrolURL, showhistoryportid]);
  }

  async clearQueue(serialportid: number): Promise<void> {
    await this.serivalServerService.abort(serialportid);
    await this.serivalServerService.resume(serialportid);
    await this.reload();
  }

  async disconnect(serialportid: number): Promise<void> {
    await this.serivalServerService.disconnect(serialportid);
    await this.reload();
  }
}
