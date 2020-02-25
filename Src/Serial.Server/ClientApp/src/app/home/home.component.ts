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

import { Component, Inject, OnInit } from '@angular/core';
import { CNCLibServerInfo } from '../models/CNCLib.Server.Info'
import { CNCLibInfoService } from '../services/CNCLib-Info.service';

import { CNCLibLoggedinService } from '../services/CNCLib-loggedin.service';

@Component({
  selector: 'home',
  templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
  appName: string = '';
  appVersion: string = '';
  appCopyright: string = '';
  appVersionInfo: CNCLibServerInfo = new CNCLibServerInfo();

  constructor(
    private cncLibInfoService: CNCLibInfoService,
    public cncLibloggedinService: CNCLibLoggedinService,
    @Inject('WEBAPI_URL') public baseUrl: string
  ) {
  }

  async ngOnInit(): Promise<void> {
    this.appVersionInfo = await this.cncLibInfoService.getInfo();
    this.appVersion = this.appVersionInfo.version;
    this.appName = this.appVersionInfo.name;
    this.appCopyright = this.appVersionInfo.copyright;
  }
}
