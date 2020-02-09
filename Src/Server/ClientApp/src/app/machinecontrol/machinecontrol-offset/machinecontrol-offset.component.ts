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

import { Component, Injectable, Inject, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { SerialPortDefinition } from '../../models/serial.port.definition';
import { SerialServerService } from '../../services/serialserver.service';

import { SerialServerConnection } from '../../serialServer/serialServerConnection';
import { MachineControlGlobal } from '../machinecontrol.global';


@Component({
  selector: 'machinecontroloffset',
  templateUrl: './machinecontrol-offset.component.html',
  styleUrls: ['./machinecontrol-offset.component.css'],
})
export class MachineControlOffsetComponent {

  @Input()
  offsetId!: number;

  constructor(
    public serialServerService: SerialServerService,
    public serialServer: SerialServerConnection,
    public machineControlGlobal: MachineControlGlobal
  ) {
  }

  toAxisName(axis: number): string {
    var axisNames = 'XYZABC';
    return axisNames.charAt(axis);
  }

  async setOfs(axis: number): Promise<void> {
    await this.postcommand("g92 " + this.toAxisName(axis) + this.machineControlGlobal.offsetG92[axis]);
  }

  async clearOfs(): Promise<void> {
    await this.postcommand("g92");
  }

  async probe(axis: number): Promise<void> {

    await this.sendWhileOkcommands([
      "g91 g31z-" + this.serialServer.getMachine().probeDist + " f" + this.serialServer.getMachine().probeFeed + " g90",
      "g92 z-" + this.serialServer.getMachine().probeSizeZ, " g91 g0z" + this.serialServer.getMachine().probeDistUp + " g90"
    ]);
  }

  async postcommand(command: string): Promise<void> {
    await this.serialServerService.queueCommands(this.serialServer.getSerialServerPortId(), [command], 1000);
  }

  async sendWhileOkcommands(commands: string[]): Promise<void> {
    await this.serialServerService.sendWhileOkCommands(this.serialServer.getSerialServerPortId(), commands, 10000);
  }
}
