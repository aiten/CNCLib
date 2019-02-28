////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/

import { NgModule } from '@angular/core';
import { MachineControlOverviewComponent } from './machinecontrol-overview/machinecontrol-overview.component';
import { MachineControlConnectComponent } from './machinecontrol-connect/machinecontrol-connect.component';
import { MachineControlDetailComponent } from './machinecontrol-detail/machinecontrol-detail.component';
import { MachineControlComponent } from './machinecontrol.component';
import { Routes, RouterModule } from '@angular/router';

export const machinecontrolURL = '/machinecontrol';

export const machineControlRoutes =
[
  {
    path: 'machinecontrol',
    component: MachineControlComponent,
    children:
    [
      { path: '', component: MachineControlOverviewComponent },
      { path: ':id', component: MachineControlDetailComponent },
    ]
  }
];

export const machineControlRoutingComponents =
[
  MachineControlComponent,
  MachineControlOverviewComponent,
  MachineControlDetailComponent,
  MachineControlConnectComponent
];
