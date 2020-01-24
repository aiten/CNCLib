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
  selector: 'machinecontrolworkoffset',
  templateUrl: './machinecontrol-workoffset.component.html',
  styleUrls: ['./machinecontrol-workoffset.component.css'],
})
export class MachineControlWorkOffsetComponent {

  @Input()
  offsetId!: number;

  constructor(
    private serialServerService: SerialServerService,
    private serialServer: SerialServerConnection,
    public machineControlGlobal: MachineControlGlobal 
  ) {
  }

  async selectOfs(): Promise<void> {
    await this.postcommand("g" + (54 + this.offsetId).toString())
  }

  async getOfs(): Promise<void> {
  }

  async setOfs(): Promise<void> {

    var cmd = "g10 l2 p" + this.offsetId;

    if (this.machineControlGlobal.workOfsX[this.offsetId].toString().length > 0) {
      cmd = cmd + " x" + this.machineControlGlobal.workOfsX[this.offsetId];
    }

    if (this.machineControlGlobal.workOfsY[this.offsetId].toString().length > 0) {
      cmd = cmd + " y" + this.machineControlGlobal.workOfsY[this.offsetId];
    }

    if (this.machineControlGlobal.workOfsZ[this.offsetId].toString().length > 0) {
      cmd = cmd + " z" + this.machineControlGlobal.workOfsZ[this.offsetId];
    }

    await this.postcommand(cmd);
  }

  async relOfs(): Promise<void> {
  }

  async postcommand(command: string): Promise<void> {
    await this.serialServerService.queueCommands(this.serialServer.getSerialServerPortId(), [command], 1000);
  }

  async sendWhileOkcommands(commands: string[]): Promise<void> {
    await this.serialServerService.sendWhileOkCommands(this.serialServer.getSerialServerPortId(), commands, 10000);
  }
}
