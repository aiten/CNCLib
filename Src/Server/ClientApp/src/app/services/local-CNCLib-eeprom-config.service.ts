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

import { Injectable, Inject } from '@angular/core';
import { EepromConfigInput } from '../models/eeprom-config-input';
import { EepromConfig } from '../models/eeprom-config';
import { HttpClient } from '@angular/common/http';
import { CNCLibEepromConfigService } from './CNCLib-eeprom-config.service';

@Injectable()
export class LocalCNCLibEepromConfigService implements CNCLibEepromConfigService {
  constructor(
    private http: HttpClient,
    @Inject('WEBAPI_URL') public baseUrl: string,
  ) {
  }

  calculateConfig(config: EepromConfigInput): Promise<EepromConfig> {
    const m = this.http
      .get(`${this.baseUrl}api/EepromConfiguration?teeth=${config.teeth}&toothsizeInMm=${config.toothsizeinMm
        }&microsteps=${config.microsteps}&stepsPerRotation=${config.stepsPerRotation}&estimatedRotationSpeed=${
        config.estimatedRotationSpeed}&timeToAcc=${config.timeToAcc}&timeToDec=${config.timeToDec}`)
      .toPromise()
      .then((response: Response) => toEeconfig(response))
      .catch(this.handleErrorPromise);
    return m;
  }

  private handleErrorPromise(error: Response | any) {
    console.error(error.message || error);
    return Promise.reject(error.message || error);
  }
}

function toEeconfig(r: any): EepromConfig {
  const eeConfig = <EepromConfig>(
    {
      maxStepRate: r.maxStepRate,
      acc: r.acc,
      dec: r.dec,
      jerkSpeed: r.jerkSpeed,
      stepsPerMm1000: r.stepsPerMm1000,

      stepsPerRotation: r.stepsPerRotation,
      distancePerRotationInMm: r.distancePerRotationInMm,
      distancePerStepInMm: r.distancePerStepInMm,
      stepsPerMm: r.stepsPerMm,

      estimatedMaxStepRate: r.estimatedMaxStepRate,
      estimatedMaxSpeedInMmSec: r.estimatedMaxSpeedInMmSec,
      estimatedMaxSpeedInMmMin: r.estimatedMaxSpeedInMmMin,
      estimatedAccelerationInMmSec2: r.estimatedAccelerationInMmSec2,
      estimatedDecelerationInMmSec2: r.estimatedDecelerationInMmSec2,
      estimatedAccelerationDistToMaxSpeedInMm: r.estimatedAccelerationDistToMaxSpeedInMm,
      estimatedDecelerationDistFromMaxSpeedInMm: r.estimatedDecelerationDistFromMaxSpeedInMm,
      estimatedAcc: r.estimatedAcc,
      estimatedDec: r.estimatedDec,
      estimatedJerkSpeed: r.estimatedJerkSpeed,
    });

  return eeConfig;
}
