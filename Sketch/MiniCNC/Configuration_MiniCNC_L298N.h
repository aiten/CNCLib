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

#include <Steppers/StepperL298N.h>

////////////////////////////////////////////////////////

#define CMyStepper CStepperL298N
#define ConversionToMm1000 ToMm1000_L298N
#define ConversionToMachine ToMachine_L298N

#define CNC_MAXSPEED 375
#define CNC_ACC  65
#define CNC_DEC  75

// 48 steps/rot
inline mm1000_t ToMm1000_L298N(axis_t /* axis */, sdist_t val)				{ return  RoundMulDivU32(val, 125, 6); }
inline sdist_t  ToMachine_L298N(axis_t /* axis */, mm1000_t val)			{ return  RoundMulDivU32(val, 6, 125); }

////////////////////////////////////////////////////////

#define CONTROLLERFAN_ON  LOW
#define CONTROLLERFAN_OFF HIGH

////////////////////////////////////////////////////////

#define SPINDEL_PIN	-1

#define SPINDEL_ON  LOW
#define SPINDEL_OFF HIGH

////////////////////////////////////////////////////////

#define PROBE1_PIN	-1

#define PROBE_ON  LOW
#define CONTROLLERFAN_ONTIME	10000			// switch off controllerfan if idle for 10 Sec
#define CONTROLLERFAN_FAN_PIN	13 // 10

#define PROBE_OFF HIGH

////////////////////////////////////////////////////////

#define KILL_PIN	-1

#define KILL_ON  LOW
#define KILL_OFF HIGH
