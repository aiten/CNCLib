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
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { ColorPickerModule } from 'ngx-color-picker';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { MaterialModule } from './material.module';
import { LoginComponent } from "./login/login.component"

import { CNCLibInfoService } from './services/CNCLib-Info.service';
import { LocalCNCLibInfoService } from './services/local-CNCLib-Info.service';
import { CNCLibLoggedinService } from './services/CNCLib-loggedin.service';
import { LocalCNCLibLoggedinService } from './services/local-CNCLib-loggedin.service';

import { SerialPortHistoryComponent } from './serialporthistory/serialporthistory.component';
import { SerialPortPendingComponent } from './serialportpending/serialportpending.component';
import { SerialCommandListComponent } from './serialcommandlist/serialcommandlist.component';
import { machineControlRoutes, machineControlComponents } from './machine-control/machine-control.routing';

import { PreviewGlobal } from "./preview/preview.global";
import { previewComponents } from './preview/preview.routing';

import { SerialServerService } from './services/serial-server.service';
import { LocalSerialServerService } from './services/local-serial-server.service';

import { BasicAuthInterceptor, ErrorInterceptor } from './_helpers';
import { MouseWheelDirective } from './_helpers/mousewheel.directive';

import { FontAwesomeModule, FaIconLibrary } from '@fortawesome/angular-fontawesome';
import { faHome, faSync, faPlug, faCalculator, faToolbox, faCogs, faEllipsisV, faArrowDown, faChevronDown, faChevronUp, faChevronLeft, faChevronRight, faDrawPolygon } from '@fortawesome/free-solid-svg-icons';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    LoginComponent,
    SerialPortHistoryComponent,
    SerialPortPendingComponent,
    SerialCommandListComponent,
    MouseWheelDirective,
    ...machineControlComponents,
    ...previewComponents,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    ColorPickerModule,
    HttpClientModule,
    FormsModule,
    FontAwesomeModule,
    ReactiveFormsModule,
    MaterialModule,
    RouterModule.forRoot([
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: 'home', component: HomeComponent },
      ...machineControlRoutes,
    ])
  ],
  providers: [
    { provide: SerialServerService, useClass: LocalSerialServerService },
    { provide: CNCLibInfoService, useClass: LocalCNCLibInfoService },
    { provide: CNCLibLoggedinService, useClass: LocalCNCLibLoggedinService },
    { provide: HTTP_INTERCEPTORS, useClass: BasicAuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    PreviewGlobal,
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(library: FaIconLibrary) {

    library.addIcons(faHome,
      faSync,
      faPlug,
      faCalculator,
      faToolbox,
      faCogs,
      faEllipsisV,
      faArrowDown,
      faChevronDown,
      faChevronUp,
      faChevronLeft,
      faChevronRight,
      faDrawPolygon);
  }
}
