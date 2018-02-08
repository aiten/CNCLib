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

import { Component, Injectable, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ActivatedRoute, Params } from '@angular/router';
import { SerialPortDefinition } from '../../../models/serial.port.definition';
import { SerialServerService } from '../../../services/serialserver.service';

@Component({ 
    selector: 'machinecontroldetail',
    templateUrl: './machinecontrol-detail.component.html',
    styleUrls: [ './machinecontrol-detail.component.css' ]
})
export class MachineControlDetailComponent
{
    serialport!: SerialPortDefinition;
    isLoading: boolean = true;

    constructor(
        private serivalServerService: SerialServerService,
        private route: ActivatedRoute
    ) 
    {
    }

    async ngOnInit() : Promise<void>
    {
        this.route.params.subscribe(params =>
        {
            let serialId = params['id'];
            this.load(serialId);
        });
    }

    async load(id: number): Promise<void> 
    {
        this.serialport = await this.serivalServerService.getPort(id);
        this.isLoading = false;
    }

    async postcommand(command: string): Promise<void>
    {
        await this.serivalServerService.queueCommands(this.serialport.Id, [command], 1000 );
    }
}
