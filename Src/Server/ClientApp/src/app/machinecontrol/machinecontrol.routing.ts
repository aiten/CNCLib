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
import { MachineControlDetailComponent } from './machinecontrol-detail/machinecontrol-detail.component';
import { MachineControlMoveComponent } from './machinecontrol-move/machinecontrol-move.component';
import { MachineControlWorkOffsetComponent } from './machinecontrol-workoffset/machinecontrol-workoffset.component';
import { MachineControlOffsetComponent } from './machinecontrol-offset/machinecontrol-offset.component';
import { MachineControlRotateComponent } from './machinecontrol-rotate/machinecontrol-rotate.component';
import { MachineControlPositionComponent } from './machinecontrol-position/machinecontrol-position.component';
import { MachineControlSdComponent } from './machinecontrol-sd/machinecontrol-sd.component';
import { MachineControlRefMoveComponent } from './machinecontrol-refmove/machinecontrol-refmove.component';
import { MachineControlLaserComponent } from './machinecontrol-laser/machinecontrol-laser.component';
import { MachineControlSpindleComponent } from './machinecontrol-spindle/machinecontrol-spindle.component';
import { MachineControlCoolantComponent } from './machinecontrol-coolant/machinecontrol-coolant.component';
import { MachineControlCustomComponent } from './machinecontrol-custom/machinecontrol-custom.component';
import { MachineControlCommandComponent } from './machinecontrol-command/machinecontrol-command.component';
import { MachineControlInfoComponent } from './machinecontrol-info/machinecontrol-info.component';
import { MachineControlComponent } from './machinecontrol.component';
import { Routes, RouterModule } from '@angular/router';
import { MachineControlGlobal } from './machinecontrol.global';
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
      { path: '', component: MachineControlDetailComponent },
    ]
  }
];

export const machineControlComponents =
[
  MachineControlComponent,
  MachineControlDetailComponent,
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
];
