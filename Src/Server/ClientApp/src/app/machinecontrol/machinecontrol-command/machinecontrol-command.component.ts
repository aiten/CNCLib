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

import { Component, Injectable, Inject, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { SerialPortDefinition } from '../../models/serial.port.definition';
import { SerialServerService } from '../../services/serialserver.service';

import { SerialServerConnection } from '../../serialServer/serialServerConnection';
import { MachineControlGlobal } from '../machinecontrol.global';


@Component({
  selector: 'machinecontrolcommand',
  templateUrl: './machinecontrol-command.component.html',
  styleUrls: ['./machinecontrol-command.component.css'],
})
export class MachineControlCommandComponent {

  constructor(
    private serialServerService: SerialServerService,
    public serialServer: SerialServerConnection,
    public machineControlGlobal: MachineControlGlobal
  ) {
  }

  async sendcommand(command: string): Promise<void> {
    await this.postcommand(command);
    if (this.machineControlGlobal.lastCommands.find(elem => elem === command) == undefined) {
      this.machineControlGlobal.lastCommands.unshift(command);
      if (this.machineControlGlobal.lastCommands.length > 10) {
        this.machineControlGlobal.lastCommands = this.machineControlGlobal.lastCommands.slice(0, 10);
      }
    }
  }

  async postcommand(command: string): Promise<void> {
    await this.serialServerService.queueCommands(this.serialServer.getSerialServerPortId(), [command], 1000);
  }

  async sendWhileOkcommands(commands: string[]): Promise<void> {
    await this.serialServerService.sendWhileOkCommands(this.serialServer.getSerialServerPortId(), commands, 10000);
  }
}
