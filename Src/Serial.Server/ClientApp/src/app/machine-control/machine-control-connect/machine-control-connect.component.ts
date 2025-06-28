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

import { Component, Inject, Input, OnInit } from '@angular/core';
import { UntypedFormGroup, FormArray, FormControl, UntypedFormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { SerialPortDefinition } from '../../models/serial.port.definition';
import { SerialConnect } from '../../models/serial.connect';
import { SerialServerService } from '../../services/serial-server.service';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../material.module';

@Component(
  {
    selector: 'machinecontrolconnect',
    templateUrl: './machine-control-connect.component.html',
    styleUrls: ['./machine-control-connect.component.css'],
    imports: [ReactiveFormsModule, CommonModule, MaterialModule]
  })
export class MachineControlConnectComponent {
  @Input()
  entry: SerialPortDefinition = new SerialPortDefinition();

  isLoaded: boolean = false;
  connectOptions: SerialConnect = new SerialConnect();

  setupForm: UntypedFormGroup;

  constructor(
    private serialServerService: SerialServerService,
    public router: Router,
    private fb: UntypedFormBuilder
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

    await this.serialServerService.connect(this.entry.id,
      this.connectOptions.baudRate,
      this.connectOptions.dtrIsReset,
      this.connectOptions.resetOnConnect);
    window.location.reload();
  }
}
