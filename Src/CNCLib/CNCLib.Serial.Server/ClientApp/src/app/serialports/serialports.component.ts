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
import { SerialPortDefinition } from '../models/serial.port.definition';
import { SerialServerService } from '../services/serialserver.service';
import { Router } from '@angular/router';
import { machinecontrolURL } from '../machinecontrol/machinecontrol.routing';

@Component({
  selector: 'serialports',
  templateUrl: './serialports.component.html'
})
export class SerialPortsComponent {
  public serialports!:
  SerialPortDefinition
  [];
  public historyportid: number = -1;

  constructor(
    private serivalServerService: SerialServerService,
    private router: Router
  ) {
  }

  async ngOnInit(): Promise<void> {
    await this.reload();
  }

  async reload(): Promise<void> {
    this.serialports = await this.serivalServerService.getPorts();
  }

  async refresh(): Promise<void> {
    this.serialports = await this.serivalServerService.refresh();
  }

  showHistory(showhistoryportid: number) {
    this.historyportid = showhistoryportid;
  }

  machineControl(showhistoryportid: number) {
    this.router.navigate([machinecontrolURL, showhistoryportid]);
  }

  async clearQueue(serialportid: number): Promise<void> {
    await this.serivalServerService.abort(serialportid);
    await this.serivalServerService.resume(serialportid);
    await this.reload();
  }

  async disconnect(serialportid: number): Promise<void> {
    await this.serivalServerService.disconnect(serialportid);
    await this.reload();
  }
}
