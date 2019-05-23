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
import { Router } from '@angular/router';
import { ActivatedRoute, Params } from '@angular/router';
import { SerialPortDefinition } from '../../models/serial.port.definition';
import { SerialServerService } from '../../services/serialserver.service';

@Component({
  selector: 'machinecontroldetail',
  templateUrl: './machinecontrol-detail.component.html',
  styleUrls: ['./machinecontrol-detail.component.css']
})
export class MachineControlDetailComponent {
  serialport!: SerialPortDefinition;
  isLoading: boolean = true;

  constructor(
    private serialServerService: SerialServerService,
    private route: ActivatedRoute
  ) {
  }

  probeSize: number = 25.0;
  retractDist: number = 3.0;
  probeDist: number = 10.0;

  async ngOnInit(): Promise<void> {
    this.route.params.subscribe(params => {
      let serialId = params['id'];
      this.load(serialId);
    });
  }

  async load(id: number): Promise<void> {
    this.serialport = await this.serialServerService.getPort(id);
    this.isLoading = false;
  }

  async postcommand(command: string): Promise<void> {
    await this.serialServerService.queueCommands(this.serialport.Id, [command], 1000);
  }

  async sendWhileOkcommands(commands: string[]): Promise<void> {
    await this.serialServerService.sendWhileOkCommands(this.serialport.Id, commands, 10000);
  }
}
