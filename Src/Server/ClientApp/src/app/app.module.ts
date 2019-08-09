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

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import 'hammerjs';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { MaterialModule } from './material.module';

import { EepromConfigComponent } from "./eeprom-config/eeprom-config.component";

import { CNCLibInfoService as CNCLibService } from './services/CNCLib-Info.service';
import { LocalCNCLibInfoService } from './services/local-CNCLib-Info.service';

import { CNCLibEepromConfigService } from './services/CNCLib-eeprom-config.service';
import { LocalCNCLibEepromConfigService } from './services/local-CNCLib-eeprom-config.service';

import { CNCLibMachineService } from './services/CNCLib-machine.service';
import { LocalCNCLibMachineService } from './services/local-CNCLib-machine.service';

import { CNCLibLoadOptionService } from './services/CNCLib-load-option.service';
import { LocalCNCLibLoadOptionService } from './services/local-CNCLib-load-option.service';

import { eepromConfigRoutes, eepromConfigRoutingComponents } from './eeprom-config/eeprom-config-routing';
import { machineRoutes, machineRoutingComponents } from './machine/machine-routing';

import { ModalComponent } from './modal/modal.component';
import { MessageBoxComponent } from './modal/message-box/message-box.component';

import { gcodeRoutes, gcodeRoutingComponents } from './gcode/gcode-routing';

import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faHome } from '@fortawesome/free-solid-svg-icons';
import { faSync } from '@fortawesome/free-solid-svg-icons';
import { faPlug } from '@fortawesome/free-solid-svg-icons';
import { faCalculator } from '@fortawesome/free-solid-svg-icons';
import { faToolbox } from '@fortawesome/free-solid-svg-icons';
import { faCogs } from '@fortawesome/free-solid-svg-icons';

library.add(faHome, faSync, faPlug, faCalculator, faToolbox, faCogs);

@
NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    ModalComponent,
    MessageBoxComponent,
    ...machineRoutingComponents,
    ...eepromConfigRoutingComponents,
    ...gcodeRoutingComponents
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    FontAwesomeModule,
    ReactiveFormsModule,
    MaterialModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'home', component: HomeComponent },
      ...eepromConfigRoutes,
      ...machineRoutes,
      ...gcodeRoutes,
    ]),
  ],
  providers: [
    { provide: CNCLibService, useClass: LocalCNCLibInfoService },
    { provide: CNCLibEepromConfigService, useClass: LocalCNCLibEepromConfigService },
    { provide: CNCLibMachineService, useClass: LocalCNCLibMachineService },
    { provide: CNCLibLoadOptionService, useClass: LocalCNCLibLoadOptionService },
  ],
  bootstrap: [AppComponent],
  entryComponents: [ModalComponent, MessageBoxComponent]
})
export class AppModule {
}
