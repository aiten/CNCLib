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

import { JoystickServerService } from "./joystick-server.service";
import { LocalJoystickServerService } from "./local-joystick-server.service";
import { CNCLibEepromConfigService } from "./CNCLib-eeprom-config.service";
import { LocalCNCLibEepromConfigService } from "./local-CNCLib-eeprom-config.service";
import { CNCLibMachineService } from "./CNCLib-machine.service";
import { LocalCNCLibMachineService } from "./local-CNCLib-machine.service";
import { CNCLibJoystickService } from "./CNCLib-joystick.service";
import { LocalCNCLibJoystickService } from "./local-CNCLib-joystick.service";
import { CNCLibLoadOptionService } from "./CNCLib-load-option.service";
import { LocalCNCLibLoadOptionService } from "./local-CNCLib-load-option.service";
import { CNCLibGCodeService } from "./CNCLib-gcode.service";
import { LocalCNCLibGCodeService } from "./local-CNCLib-gcode.service";
import { CNCLibUserFileService } from "./CNCLib-userFile.service";
import { LocalCNCLibUserFileService } from "./local-CNCLib-userFile.service";
import { CNCLibUserService } from "./CNCLib-user.service";
import { LocalCNCLibUserService } from "./local-CNCLib-user.service";
import { SerialServerService } from "./serial-server.service";
import { LocalSerialServerService } from "./local-serial-server.service";
import { CNCLibInfoService } from "./CNCLib-Info.service";
import { LocalCNCLibInfoService } from "./local-CNCLib-Info.service";
import { CNCLibLoggedinService } from "./CNCLib-loggedin.service";
import { LocalCNCLibLoggedinService } from "./local-CNCLib-loggedin.service";

export const servicesComponents =
[
];

export const servicesProvides =
[
  { provide: SerialServerService, useClass: LocalSerialServerService },
  { provide: JoystickServerService, useClass: LocalJoystickServerService },
  { provide: CNCLibInfoService, useClass: LocalCNCLibInfoService },
  { provide: CNCLibEepromConfigService, useClass: LocalCNCLibEepromConfigService },
  { provide: CNCLibMachineService, useClass: LocalCNCLibMachineService },
  { provide: CNCLibJoystickService, useClass: LocalCNCLibJoystickService },
  { provide: CNCLibLoadOptionService, useClass: LocalCNCLibLoadOptionService },
  { provide: CNCLibLoggedinService, useClass: LocalCNCLibLoggedinService },
  { provide: CNCLibGCodeService, useClass: LocalCNCLibGCodeService },
  { provide: CNCLibUserFileService, useClass: LocalCNCLibUserFileService },
  { provide: CNCLibUserService, useClass: LocalCNCLibUserService },
];
