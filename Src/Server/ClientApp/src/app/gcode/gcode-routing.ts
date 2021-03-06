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

import { GcodeComponent } from './gcode.component';
import { GcodeOverviewComponent } from './gcode-overview/gcode-overview.component';
import { GcodeDetailComponent } from './gcode-detail/gcode-detail.component';
import { GcodeRunComponent } from './gcode-run/gcode-run.component';
import { GcodeRunInputComponent } from './gcode-run-input/gcode-run-input.component';
import { CNCLibLoggedinService } from "../services/CNCLib-loggedin.service";

export const gcodeRoutes =
[
  {
    path: 'gcode',
    component: GcodeComponent,
    canActivate: [CNCLibLoggedinService],
    children:
    [
      { path: '', component: GcodeOverviewComponent },
//      { path: 'detail/:id/edit', component: MachineFormComponent },
      { path: 'detail/:id', component: GcodeDetailComponent },
      { path: 'run', component: GcodeRunComponent },
      { path: 'run/:id', component: GcodeRunComponent }
    ]
  }
];

export const gcodeComponents =
[
  GcodeComponent,
  GcodeOverviewComponent,
  GcodeDetailComponent,
  GcodeRunComponent,
  GcodeRunInputComponent,
];
