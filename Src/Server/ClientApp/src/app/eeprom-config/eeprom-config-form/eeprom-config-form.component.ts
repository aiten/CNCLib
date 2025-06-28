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

import { Component, OnInit, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UntypedFormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { EepromConfigInput } from '../../models/eeprom-config-input';

import { MaterialModule } from '../../material.module';

@Component({
  selector: 'eeprom-config-form',
  templateUrl: './eeprom-config-form.component.html',
  styleUrls: ['./eeprom-config-form.component.css'],
  imports: [CommonModule,MaterialModule,ReactiveFormsModule]
})
export class EepromConfigFormComponent implements OnInit {
  @Input()
  eepromConfigInput: EepromConfigInput;
  eepromConfigForm: UntypedFormGroup;

  constructor(
    private fb: FormBuilder
  ) {
    this.eepromConfigInput = new EepromConfigInput();

    this.eepromConfigForm = fb.group(
      {
        teeth: [15, [Validators.required]],
        toothsizeinMm: [2.0, [Validators.required]],
        microsteps: [16, [Validators.required]],
        stepsPerRotation: [200, [Validators.required]],
        estimatedRotationSpeed: [7.8, [Validators.required]],
        timeToAcc: [0.2, [Validators.required]],
        timeToDec: [0.15, [Validators.required]],
      });

    this.eepromConfigForm.valueChanges.subscribe((value) => {
      Object.assign(this.eepromConfigInput, value);
    });
  }

  ngOnInit() {
  }

  isValid(): boolean {
    return this.eepromConfigForm.valid;
  }

  getFormValues() {
    this.calcEepromConfig(this.eepromConfigForm.value);
  }

  calcEepromConfig(value: any) {
    Object.assign(this.eepromConfigInput, value);
  }

  userExistsValidator = (control) => {
    /*
        return this.userService.checkUserExists(control.value)
          .map(checkResult =>
          {
            return (checkResult === false) ? { userNotFound: true } : null;
          });
    */
  }

  userExistsValidatorReused = (control) => {
    /*    
        const validator = new UserExistsValidatorDirective(this.userService);
        return validator.validate(control);
    */
  };
}
