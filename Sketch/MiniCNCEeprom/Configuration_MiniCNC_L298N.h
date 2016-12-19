  ////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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

#define MOVEAWAYFROMREF_STEPS 10

////////////////////////////////////////////////////////

#define CNC_MAXSPEED 375
#define CNC_ACC  65
#define CNC_DEC  75

//#define SETDIRECTION (1 << X_AXIS) + (1 << Y_AXIS)    // set bit to invert direction of each axis

////////////////////////////////////////////////////////

#define CMyStepper CStepperL298N

#define ConversionToMm1000 MiniCNCToMm1000
#define ConversionToMachine MiniCNCToMachine
//#define ConversionToMm1000 MiniCNCToMm1000
//#define ConversionToMachine MiniCNCToMachine

// 48 steps/rot
//inline mm1000_t ToMm1000_L298N(axis_t /* axis */, sdist_t val) { return  RoundMulDivU32(val, 125, 6); }
//inline sdist_t  ToMachine_L298N(axis_t /* axis */, mm1000_t val) { return  RoundMulDivU32(val, 6, 125); }

#define X_STEPSPERMM 48.0
#define Y_STEPSPERMM 48.0
#define Z_STEPSPERMM 48.0
#define A_STEPSPERMM 48.0

////////////////////////////////////////////////////////

#include <Steppers/StepperL298N.h>

////////////////////////////////////////////////////////

#define MYNUM_AXIS  4

////////////////////////////////////////////////////////

#undef CONTROLLERFAN_PIN

////////////////////////////////////////////////////////

#undef SPINDEL_PIN

////////////////////////////////////////////////////////

#undef PROBE_PIN

////////////////////////////////////////////////////////

#undef KILL_PIN

