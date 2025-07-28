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

import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

import { Router } from '@angular/router';
import { MaterialModule } from '../../../material.module';

import { MachineControlState } from "../../machine-control-state";
import { MachineControlGlobal } from '../../machine-control.global';
import { SerialServerConnection } from '../../../serial-server/serial-server-connection';

@Component({
  selector: 'machinecontrolmenu',
  templateUrl: './machine-control-menu.component.html',
  styleUrls: ['./machine-control-menu.component.css'],
  imports: [CommonModule, MaterialModule]
})
export class MachineControlMenuComponent {

  constructor(
    private router: Router
  ) {
  }

  @Input()
  public machineControlGlobal: MachineControlGlobal
  @Input()
  public serialServer: SerialServerConnection 
  @Input()
    machineControlState: MachineControlState;
  
  async emergencyStop() {
    await this.serialServer.abort();
    await this.serialServer.resume();
    await this.machineControlState.postcommand("!");    // send to machine
    await this.machineControlState.postcommand("!!!");  // resurrect machine
    await this.machineControlState.postcommand("m5");   // stop spindle / laser
  }
}
