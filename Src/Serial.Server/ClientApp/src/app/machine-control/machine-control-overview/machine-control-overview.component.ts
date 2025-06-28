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
import { Router } from '@angular/router';
import { SerialPortDefinition } from '../../models/serial.port.definition';
import { SerialServerService } from '../../services/serial-server.service';
import { CommonModule } from '@angular/common';
import { SerialCommandListComponent } from '../../serialcommandlist/serialcommandlist.component';
import { MaterialModule } from '../../material.module';

//import { machinecontrolURL } from '../machinecontrol.routing';
const machinecontrolURL = '/machinecontrol';

@Component({
  selector: 'machinecontroloverview',
  templateUrl: './machine-control-overview.component.html',
  styleUrls: ['./machine-control-overview.component.css'],
  imports: [CommonModule, SerialCommandListComponent,MaterialModule]
})
export class MachineControlOverviewComponent {
  serialports!: SerialPortDefinition[];
  public historyportid: number = -1;
  public pendingportid: number = -1;

  displayedColumns: string[] = ['id', 'portName', 'isConnected', 'isAborted', 'isSingleStep', 'commandsInQueue', 'task'];

  constructor(
    private serialServerService: SerialServerService,
    public router: Router) {
  }

  async ngOnInit(): Promise<void> {
    await this.reload();
  }

  useport(serialport: SerialPortDefinition) {
    this.router.navigate([machinecontrolURL, serialport.id]);
  }

  async reload(): Promise<void> {
    this.serialports = await this.serialServerService.getPorts();
  }

  async refresh(): Promise<void> {
    this.serialports = await this.serialServerService.refresh();
  }

  showHistory(serialPortId: number) {
    this.historyportid = serialPortId;
    this.pendingportid = -1;
  }

  showPending(serialPortId: number) {
    this.historyportid = -1;
    this.pendingportid = serialPortId;
  }

  machineControl(serialPortId: number) {
    this.router.navigate([machinecontrolURL, serialPortId]);
  }

  async clearQueue(serialPortId: number): Promise<void> {
    await this.serialServerService.abort(serialPortId);
    await this.serialServerService.resume(serialPortId);
    await this.reload();
  }

  async disconnect(serialPortId: number): Promise<void> {
    await this.serialServerService.disconnect(serialPortId);
    await this.reload();
  }
}
