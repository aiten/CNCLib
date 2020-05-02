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

import { Component, Injectable, Inject, OnInit, OnDestroy } from '@angular/core';

import { SerialServerConnection } from '../serial-server/serial-server-connection';
import { JoystickServerConnection } from '../serial-server/joystick-server-connection';

import { MachineControlGlobal } from './machine-control.global';
import { MachineControlState } from './machine-control-state';


@Component({
  selector: 'machinecontrol',
  templateUrl: './machine-control.component.html',
  styleUrls: ['./machine-control.component.css'],
})
export class MachineControlComponent implements OnInit, OnDestroy {

  isLoading: boolean = true;

  constructor(
    public serialServer: SerialServerConnection,
    public joystickServer: JoystickServerConnection,
    public machineControlGlobal: MachineControlGlobal,
    public machineControlState: MachineControlState
  ) {
  }

  async ngOnInit(): Promise<void> {
    await this.machineControlState.load();
    this.isLoading = false;
    await this.machineControlState.loadJoystick(false);
  }

  async ngOnDestroy(): Promise<void> {
    await this.machineControlState.close();
  }

  isConnected() {
    return this.machineControlState.isConnected;
  }
}
