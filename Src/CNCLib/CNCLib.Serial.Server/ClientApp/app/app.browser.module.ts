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
import { BrowserModule } from '@angular/platform-browser';
import { AppModuleShared } from './app.shared.module';
import { AppComponent } from './components/app/app.component';
import { HttpClientModule } from '@angular/common/http';

@NgModule(
    {
    bootstrap: [ AppComponent ],
    imports:
    [
        BrowserModule,
        HttpClientModule,
        AppModuleShared
    ],
    providers:
    [
        { provide: 'BASE_URL', useFactory: getBaseUrl }
    ]
})
export class AppModule 
{
}

export function getBaseUrl() 
{
    //return 'http://10.1.1.20:5000/';    // CORS must be enabled!!!
    return document.getElementsByTagName('base')[0].href;
}
