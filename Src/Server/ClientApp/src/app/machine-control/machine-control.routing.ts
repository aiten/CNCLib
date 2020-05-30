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

import { MachineControlMainComponent } from "./machine-control-main/machine-control-main.component";
import { MachineControlMoveComponent } from './machine-control-main/machine-control-move/machine-control-move.component';
import { MachineControlWorkOffsetComponent } from './machine-control-main/machine-control-workoffset/machine-control-workoffset.component';
import { MachineControlOffsetComponent } from './machine-control-main/machine-control-offset/machine-control-offset.component';
import { MachineControlRotateComponent } from './machine-control-main/machine-control-rotate/machine-control-rotate.component';
import { MachineControlPositionComponent } from './machine-control-main/machine-control-position/machine-control-position.component';
import { MachineControlSdComponent } from './machine-control-main/machine-control-sd/machine-control-sd.component';
import { MachineControlRefMoveComponent } from './machine-control-main/machine-control-refmove/machine-control-refmove.component';
import { MachineControlLaserComponent } from './machine-control-main/machine-control-laser/machine-control-laser.component';
import { MachineControlSpindleComponent } from './machine-control-main/machine-control-spindle/machine-control-spindle.component';
import { MachineControlCoolantComponent } from './machine-control-main/machine-control-coolant/machine-control-coolant.component';
import { MachineControlCustomComponent } from './machine-control-main/machine-control-custom/machine-control-custom.component';
import { MachineControlCommandComponent } from './machine-control-main/machine-control-command/machine-control-command.component';
import { MachineControlInfoComponent } from './machine-control-main/machine-control-info/machine-control-info.component';
import { MachineControlJoystickComponent } from './machine-control-main/machine-control-joystick/machine-control-joystick.component';
import { MachineControlComponent } from './machine-control.component';

import { CNCLibLoggedinService } from "../services/CNCLib-loggedin.service";

import { EepromComponent } from "./machine-control-eeprom/eeprom.component";

export const machinecontrolURL = '/machinecontrol';

export const machineControlRoutes =
[
  {
    path: 'machinecontrol',
    component: MachineControlComponent,
    canActivate: [CNCLibLoggedinService],
    children:
    [
      { path: '', component: MachineControlMainComponent },
      { path: 'eeprom', component: EepromComponent },
    ]
  }
];

export const machineControlComponents =
[
  MachineControlComponent,
  MachineControlMainComponent,
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
  MachineControlJoystickComponent,
  EepromComponent
];
