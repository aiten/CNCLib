import { Component, OnInit } from '@angular/core';
import { EepromConfig } from '../../models/eeprom-config';
import { EepromConfigInput } from '../../models/eeprom-config-input';
import { EepromConfigService } from '../../services/eeprom-config.service';
import { eepromConfigURL } from '../eeprom-config-routing';
import { Router } from '@angular/router';

@Component(
  {
    selector: 'ha-eeprom-config-overview',
    templateUrl: './eeprom-config-overview.component.html',
    styleUrls: ['./eeprom-config-overview.component.css']
  })
export class EepromConfigOverviewComponent implements OnInit
{
  isCalculated: boolean = false;
  isCalculating: boolean = false;

  eepromConfigInput: EepromConfigInput;
  eepromConfig: EepromConfig;

  constructor(
    private router: Router,
    private eepromConfigService: EepromConfigService,
  )
  {
    this.eepromConfigInput = new EepromConfigInput();
    this.eepromConfig = new EepromConfig();
  }

  async calculateEepromConfig()
  {
    this.isCalculating = true;
    console.log('calculateEepromConfig');

    this.eepromConfig = await this.eepromConfigService.calculateConfig(this.eepromConfigInput);;
    this.isCalculating = false;        
    this.isCalculated = true;
  }

  ngOnInit()
  {
  }
}

