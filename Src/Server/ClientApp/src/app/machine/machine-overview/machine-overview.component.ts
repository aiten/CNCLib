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

import { Component, OnInit } from '@angular/core';
import { Machine } from '../../models/machine';
import { CNCLibMachineService } from '../../services/CNCLib-machine.service';
import { CNCLibLoggedinService } from '../../services/CNCLib-loggedin.service';

import { SerialServerConnection } from '../../serialServer/serialServerConnection';

import { machineURL, machineControlURL } from '../../app.global';
import { Router } from '@angular/router';


@Component(
  {
    selector: 'machine-overview',
    templateUrl: './machine-overview.component.html',
    styleUrls: ['./machine-overview.component.css'],
  })
export class MachineOverviewComponent implements OnInit {
  entries: Machine[] = [];
  errorMessage: string = '';
  isLoading: boolean = true;

  displayedColumns: string[] = ['Id', 'Description', 'SerialServer', 'ComPort', 'SizeX', 'SizeY', 'SizeZ', 'Detail', 'Connect'];

  constructor(
    private router: Router,
    private machineService: CNCLibMachineService,
    public serialServer: SerialServerConnection
  ) {
  }

  detailMachine(id: number) {
    this.router.navigate([machineURL, 'detail', String(id)]);
  }

  async connectToMachine(machine: Machine) {

    await this.serialServer.connectTo(machine);
    this.router.navigate([machineControlURL]);
  }

  canConnectTo(machine: Machine): boolean {
    return machine.serialServer.length > 0;
  }

  async newMachine() {

    var newmachineDefault = await this.machineService.getDefault();
    var newmachine = await this.machineService.addMachine(newmachineDefault);
    this.router.navigate([machineURL, 'detail', String(newmachine.id)]);
  }

  async ngOnInit() {
    this.entries = await this.machineService.getAll();
  }
}
