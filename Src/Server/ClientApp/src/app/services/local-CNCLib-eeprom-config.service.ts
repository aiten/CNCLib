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

import { Injectable, Inject, Pipe } from '@angular/core';
import { EepromConfigInput } from '../models/eeprom-config-input';
import { EepromConfig } from '../models/eeprom-config';
import { Http, Response, Headers, RequestOptions, } from '@angular/http';
import { HttpClient } from '@angular/common/http';
import { CNCLibEepromConfigService } from './CNCLib-eeprom-config.service';

@Injectable()
export class LocalCNCLibEepromConfigService implements CNCLibEepromConfigService {
  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') public baseUrl: string,
  ) {
  }

  calculateConfig(config: EepromConfigInput): Promise<EepromConfig> {
    console.log('LocalCNCLibEepromConfigService.calculateConfig');
    const m$ = this.http
      .get(`${this.baseUrl}api/EepromConfiguration?teeth=${config.teeth}&toothsizeInMm=${config.toothsizeinMm
        }&microsteps=${config.microsteps}&stepsPerRotation=${config.stepsPerRotation}&estimatedRotationSpeed=${
        config.estimatedRotationSpeed}&timeToAcc=${config.timeToAcc}&timeToDec=${config.timeToDec}`)
      .toPromise()
      .then((response: Response) => toEeconfig(response))
      .catch(this.handleErrorPromise);
    return m$;
  }

  private getHeaders() {
    const headers = new RequestOptions();
    headers.headers = new Headers();
    headers.headers.append('Accept', 'application/json');
    return headers;
  }

  private handleErrorPromise(error: Response | any) {
    console.error(error.message || error);
    return Promise.reject(error.message || error);
  }
}

function toEeconfig(r: any): EepromConfig {
  const eeConfig = <EepromConfig>(
    {
      maxStepRate: r.MaxStepRate,
      acc: r.Acc,
      dec: r.Dec,
      jerkSpeed: r.JerkSpeed,
      stepsPerMm1000: r.StepsPerMm1000,

      stepsPerRotation: r.StepsPerRotation,
      distancePerRotationInMm: r.DistancePerRotationInMm,
      stepsPerMm: r.StepsPerMm,

      estimatedMaxStepRate: r.EstimatedMaxStepRate,
      estimatedMaxSpeedInMmSec: r.EstimatedMaxSpeedInMmSec,
      estimatedAccelerationInMmSec2: r.EstimatedAccelerationInMmSec2,
      estimatedDecelerationInMmSec2: r.EstimatedDecelerationInMmSec2,
      estimatedAcc: r.EstimatedAcc,
      estimatedDec: r.EstimatedDec,
      estimatedJerkSpeed: r.EstimatedJerkSpeed,
    });

  console.log('Parsed toEeconfig:', eeConfig);
  return eeConfig;
}
