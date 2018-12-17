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
import { Router } from '@angular/router';
import { SerialPortDefinition } from '../../models/serial.port.definition';
import { SerialServerService } from '../../services/serialserver.service';

//import { machinecontrolURL } from '../machinecontrol.routing';
const machinecontrolURL = '/machinecontrol';

@Component({
  selector: 'machinecontroloverview',
  templateUrl: './machinecontrol-overview.component.html'
})
export class MachineControlOverviewComponent {
  serialports!: SerialPortDefinition[];

  constructor(
    private serialServerService: SerialServerService,
    public router: Router) {
  }

  async ngOnInit(): Promise<void> {
    await this.reload();
  }

  async reload(): Promise<void> {
    this.serialports = await this.serialServerService.getPorts();
  }

  useport(serialport: SerialPortDefinition) {
    this.router.navigate([machinecontrolURL, serialport.Id]);
  }
}
