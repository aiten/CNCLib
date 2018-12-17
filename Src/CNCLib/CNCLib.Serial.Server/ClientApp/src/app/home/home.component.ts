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

import { Component, Inject, OnInit } from '@angular/core';
import { CNCLibServerInfo } from '../models/CNCLib.Server.Info'
import { SerialServerService } from '../services/serialserver.service';

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
    private serialServerService: SerialServerService,
  ) {
  }

  async ngOnInit(): Promise<void> {
    this.appVersionInfo = await this.serialServerService.getInfo();
    this.appVersion = this.appVersionInfo.Version;
    this.appName = this.appVersionInfo.Name;
    this.appCopyright = this.appVersionInfo.Copyright;
  }
}
