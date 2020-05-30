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

import { EepromConfigComponent } from './eeprom-config.component';
import { EepromConfigFormComponent } from './eeprom-config-form/eeprom-config-form.component';
import { EepromConfigResultComponent } from './eeprom-config-result/eeprom-config-result.component';
import { EepromConfigOverviewComponent } from './eeprom-config-overview/eeprom-config-overview.component';

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

export const eepromConfigComponents =
[
  EepromConfigComponent,
  EepromConfigFormComponent,
  EepromConfigOverviewComponent,
  EepromConfigResultComponent,
];
