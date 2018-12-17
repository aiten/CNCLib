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

import { Component, Inject, Input, OnInit } from '@angular/core';
import { FormGroup, FormArray, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { SerialPortDefinition } from '../../models/serial.port.definition';
import { SerialConnect } from '../../models/serial.connect';
import { SerialServerService } from '../../services/serialserver.service';

@Component(
  {
    selector: 'machinecontrolconnect',
    templateUrl: './machinecontrol-connect.component.html'
  })
export class MachineControlConnectComponent {
  @Input()
  entry: SerialPortDefinition = new SerialPortDefinition();

  isLoaded: boolean = false;
  connectOptions: SerialConnect = new SerialConnect();

  setupForm: FormGroup;

  constructor(
    private serialServerService: SerialServerService,
    public router: Router,
    private fb: FormBuilder
  ) {
    this.setupForm = fb.group(
      {
        baudRate: [115200],
        dtrIsReset: true,
        resetOnConnect: false
      });

    this.setupForm.valueChanges.subscribe((value) => {
      if (this.isLoaded)
        Object.assign(this.connectOptions, value);
    });

  }

  ngOnInit() {
    this.isLoaded = true;
  }

  async save(value: any): Promise<void> {
    Object.assign(this.connectOptions, value);
    console.log(value);
    console.log(this.connectOptions);

    console.log('save:' + this.entry.Id);

    await this.serialServerService.connect(this.entry.Id,
      this.connectOptions.baudRate,
      this.connectOptions.dtrIsReset,
      this.connectOptions.resetOnConnect);
    window.location.reload();
  }
}
