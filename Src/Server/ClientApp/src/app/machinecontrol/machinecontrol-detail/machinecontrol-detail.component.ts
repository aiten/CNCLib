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
  selector: 'machinecontroldetail',
  templateUrl: './machinecontrol-detail.component.html',
  styleUrls: ['./machinecontrol-detail.component.css'],
})
export class MachineControlDetailComponent {

  serialport!: SerialPortDefinition;
  isLoading: boolean = true;
  isConnected: boolean =false;

  constructor(
    private serialServerService: SerialServerService,
    private serialServer: SerialServerConnection,
    private machineControlGlobal: MachineControlGlobal 
  ) {
  }

  async ngOnInit(): Promise<void>
  {
      await this.load();
  }

  async load(): Promise<void> {

    if (this.serialServer.getMachine() != null) {
      
      console.log(this.serialServer.getSerialServerUrl());

      this.serialServerService.setBaseUrl(this.serialServer.getSerialServerUrl());
      var id = this.serialServer.getSerialServerPortId();

      this.serialport = await this.serialServerService.getPort(id);
      this.isConnected = this.serialport.IsConnected != 0;
    }
    this.isLoading = false;
  }

  async postcommand(command: string): Promise<void> {
    await this.serialServerService.queueCommands(this.serialport.Id, [command], 1000);
  }

  async sendWhileOkcommands(commands: string[]): Promise<void> {
    await this.serialServerService.sendWhileOkCommands(this.serialport.Id, commands, 10000);
  }
}
