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

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';

import { SerialPortsComponent } from './serialports/serialports.component';
import { SerialPortHistoryComponent } from './serialporthistory/serialporthistory.component';
import { machineControlRoutes, machineControlRoutingComponents } from './machinecontrol/machinecontrol.routing';

import { SerialServerService } from './services/serialserver.service';
import { LocalSerialServerService } from './services/local.serialserver.service';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    SerialPortsComponent,
    SerialPortHistoryComponent,
    ...machineControlRoutingComponents,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      	{ path: '', component: HomeComponent, pathMatch: 'full' },
        { path: 'home', component: HomeComponent },
        { path: 'serialports', component: SerialPortsComponent },
        ...machineControlRoutes,
    ])
  ],
  providers: [
        { provide: SerialServerService, useClass: LocalSerialServerService },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
