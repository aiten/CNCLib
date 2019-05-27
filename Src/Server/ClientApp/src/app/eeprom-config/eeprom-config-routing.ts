import { NgModule } from '@angular/core';

import { EepromConfigInput } from '../models/eeprom-config-input';
import { EepromConfig } from '../models/eeprom-config';

import { EepromConfigComponent } from './eeprom-config.component';
import { EepromConfigFormComponent } from './eeprom-config-form/eeprom-config-form.component';
import { EepromConfigResultComponent } from './eeprom-config-result/eeprom-config-result.component';
import { EepromConfigOverviewComponent } from './eeprom-config-overview/eeprom-config-overview.component';

import { Routes, RouterModule } from '@angular/router';

//import { AppAboutComponent } from '../app-about/app-about.component';

export const eepromConfigURL = '/eepromconfiguration';

export const eepromConfigRoutes =
[
  {
    path: 'eepromconfig',
    component: EepromConfigComponent,
    children:
    [
      { path: '', component: EepromConfigOverviewComponent },
    ]
  }
];

export const eepromConfigRoutingComponents =
[
  EepromConfigComponent,
  EepromConfigFormComponent,
  EepromConfigOverviewComponent,
  EepromConfigResultComponent,
];
