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
import { EepromConfig } from '../../models/eeprom-config';
import { EepromConfigInput } from '../../models/eeprom-config-input';
import { CNCLibEepromConfigService } from '../../services/CNCLib-eeprom-config.service';
import { eepromConfigURL } from '../eeprom-config-routing';
import { Router } from '@angular/router';

@Component(
  {
    selector: 'ha-eeprom-config-overview',
    templateUrl: './eeprom-config-overview.component.html',
    styleUrls: ['./eeprom-config-overview.component.css']
  })
export class EepromConfigOverviewComponent implements OnInit {
  isCalculated: boolean = false;
  isCalculating: boolean = false;

  eepromConfigInput: EepromConfigInput;
  eepromConfig: EepromConfig;

  constructor(
    private router: Router,
    private eepromConfigService: CNCLibEepromConfigService,
  ) {
    this.eepromConfigInput = new EepromConfigInput();
    this.eepromConfig = new EepromConfig();
  }

  async calculateEepromConfig() {
    this.isCalculating = true;
    console.log('calculateEepromConfig');

    this.eepromConfig = await this.eepromConfigService.calculateConfig(this.eepromConfigInput);;
    this.isCalculating = false;
    this.isCalculated = true;
  }

  ngOnInit() {
  }
}
