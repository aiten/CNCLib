import { Injectable, Inject, Pipe } from '@angular/core';
import { EepromConfigInput } from '../../models/eeprom-config-input';
import { EepromConfig } from '../../models/eeprom-config';
import { Http, Response, Headers, RequestOptions, } from '@angular/http';
import { HttpClient } from '@angular/common/http';
import { EepromConfigService } from '../eeprom-config.service';
import { map } from 'rxjs/operators';

@Injectable()
export class AzureEepromConfigService implements EepromConfigService
{
  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') public baseUrl: string,
  ) {
  }

  calculateConfig(config: EepromConfigInput): Promise<EepromConfig>
  {
    console.log('AzureEepromConfigService.calculateConfig');
    const m$ = this.http
      .get(`${this.baseUrl}api/EepromConfiguration?teeth=${config.teeth}&toothsizeInMm=${config.toothsizeinMm}&microsteps=${config.microsteps}&stepsPerRotation=${config.stepsPerRotation}&estimatedRotationSpeed=${config.estimatedRotationSpeed}&timeToAcc=${config.timeToAcc}&timeToDec=${config.timeToDec}`)
      .toPromise()
      .then((response: Response) => toEeconfig(response))
      .catch(this.handleErrorPromise);
    return m$;
  }

  private getHeaders()
  {
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

function toEeconfig(r: any): EepromConfig
{
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

/*
function fromEeConfigInput(r: EepromConfigInput): any
{
  const eeConfigInput = <any>(
    {
      Teeth: r.teeth,
    });

  console.log('Parsed fromEeConfigInput:', eeConfigInput);
  return eeConfigInput;
}

*/
