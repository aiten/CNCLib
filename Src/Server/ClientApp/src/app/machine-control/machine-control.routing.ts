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

import { NgModule } from '@angular/core';
import { MachineControlMoveComponent } from './machine-control-move/machine-control-move.component';
import { MachineControlWorkOffsetComponent } from './machine-control-workoffset/machine-control-workoffset.component';
import { MachineControlOffsetComponent } from './machine-control-offset/machine-control-offset.component';
import { MachineControlRotateComponent } from './machine-control-rotate/machine-control-rotate.component';
import { MachineControlPositionComponent } from './machine-control-position/machine-control-position.component';
import { MachineControlSdComponent } from './machine-control-sd/machine-control-sd.component';
import { MachineControlRefMoveComponent } from './machine-control-refmove/machine-control-refmove.component';
import { MachineControlLaserComponent } from './machine-control-laser/machine-control-laser.component';
import { MachineControlSpindleComponent } from './machine-control-spindle/machine-control-spindle.component';
import { MachineControlCoolantComponent } from './machine-control-coolant/machine-control-coolant.component';
import { MachineControlCustomComponent } from './machine-control-custom/machine-control-custom.component';
import { MachineControlCommandComponent } from './machine-control-command/machine-control-command.component';
import { MachineControlInfoComponent } from './machine-control-info/machine-control-info.component';
import { MachineControlJoystickComponent } from './machine-control-joystick/machine-control-joystick.component';
import { MachineControlComponent } from './machine-control.component';
import { MachineControlState } from './machine-control-state';
import { Routes, RouterModule } from '@angular/router';
import { MachineControlGlobal } from './machine-control.global';
import { CNCLibLoggedinService } from "../services/CNCLib-loggedin.service";

export const machinecontrolURL = '/machinecontrol';

export const machineControlRoutes =
[
  {
    path: 'machinecontrol',
    component: MachineControlComponent,
    canActivate: [CNCLibLoggedinService],
    children:
    [
    ]
  }
];

export const machineControlComponents =
[
  MachineControlComponent,
  MachineControlMoveComponent,
  MachineControlOffsetComponent,
  MachineControlWorkOffsetComponent,
  MachineControlRotateComponent,
  MachineControlPositionComponent,
  MachineControlSdComponent,
  MachineControlRefMoveComponent,
  MachineControlLaserComponent,
  MachineControlSpindleComponent,
  MachineControlCoolantComponent,
  MachineControlCustomComponent,
  MachineControlCommandComponent,
  MachineControlInfoComponent,
  MachineControlJoystickComponent
];
