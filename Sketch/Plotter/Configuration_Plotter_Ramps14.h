////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/
////////////////////////////////////////////////////////

#pragma once

////////////////////////////////////////////////////////

#include <Steppers/StepperRamps14_pins.h>
#include <Steppers/StepperRamps14.h>

////////////////////////////////////////////////////////

#define CMyStepper CStepperRamps14

#define X_MAXSIZE	520000		//520mm
#define Y_MAXSIZE	295000		//295mm
#define Z_MAXSIZE	3000		//3mm

#define MAXSTEPRATE	25000

// 3200steps/rot, T2.5 belt pully with 12teeth
// one rottation = 3200 steps and 2.5*12 = 30mm(30000 mm1000_t) => ratio = 30000/3200 => 2400/256
// avoid overrun => 2^31 / 2400 => 894784,8533 => 0.89m
#define ConversionToMm1000  [](axis_t axis, sdist_t val) { return (mm1000_t)(RoundMulDivI32(val, 75, 8)); }
#define ConversionToMachine [](axis_t axis, mm1000_t val) { return (sdist_t)(RoundMulDivI32(val, 8, 75)); }
//#define ConversionToMm1000  [](axis_t axis, sdist_t val) { return (mm1000_t)(RoundMulDivI32(val, 2400, 256)); }
//#define ConversionToMachine [](axis_t axis, mm1000_t val) { return (sdist_t)(RoundMulDivI32(val, 256, 2400)); }
//#define ConversionToMm1000  [] (axis_t axis, sdist_t val)  { return (mm1000_t) (val*(1.0/(77.0 / 29.0 / 25.0))); },
//#define ConversionToMachine [] (axis_t axis, mm1000_t val) { return (sdist_t)  (val*(77.0 / 29.0 / 25.0)); }

#define PENUP_FEEDRATE		-STEPRATETOFEEDRATE(25000);
#define PENDOWN_FEEDRATE	STEPRATETOFEEDRATE(8000);


#define POS_Z_PENUP			0
#define POS_Z_PENDOWN		2260

#define MOVEPENUP_FEEDRATE		-STEPRATETOFEEDRATE(4000)
#define MOVEPENDOWN_FEEDRATE	-STEPRATETOFEEDRATE(3000)

#define EMERGENCY_ENDSTOP 5

////////////////////////////////////////////////////////

#define CONTROLLERFAN_ONTIME	10000			// switch off controllerfan if idle for 10 Sec
#define CONTROLLERFAN_FAN_PIN	RAMPS14_FET2D9_PIN

////////////////////////////////////////////////////////
