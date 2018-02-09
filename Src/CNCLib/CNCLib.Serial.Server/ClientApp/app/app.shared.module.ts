////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { SerialPortsComponent } from './components/serialports/serialports.component';
import { SerialPortHistoryComponent } from './components/serialporthistory/serialporthistory.component';
import { machineControlRoutes, machineControlRoutingComponents } from './components/machinecontrol/machinecontrol.routing';

import { SerialServerService } from './services/serialserver.service';
import { LocalSerialServerService } from './services/local.serialserver.service';

@NgModule(
{
    declarations:
    [
        AppComponent,
        NavMenuComponent,
        SerialPortsComponent,
        SerialPortHistoryComponent,
        ...machineControlRoutingComponents,
        HomeComponent
    ],
    imports:
    [
        CommonModule,
        HttpModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule.forRoot(
        [
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'serialports', component: SerialPortsComponent },
            ...machineControlRoutes,

            { path: '**', redirectTo: 'home' }
        ])
    ],
    providers:
    [
        { provide: SerialServerService, useClass: LocalSerialServerService },
    ],
})

export class AppModuleShared
{
}
