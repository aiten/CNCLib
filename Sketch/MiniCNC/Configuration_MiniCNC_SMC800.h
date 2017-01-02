////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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

#define CNC_MAXSPEED 3000
#define CNC_ACC  150
#define CNC_DEC  180

//#define SETDIRECTION (1 << X_AXIS) + (1 << Y_AXIS)    // set bit to invert direction of each axis

////////////////////////////////////////////////////////

#define MYNUM_AXIS 3

////////////////////////////////////////////////////////

#include <Steppers/StepperSMC800.h>

////////////////////////////////////////////////////////

#define CMyStepper CStepperSMC800
#define ConversionToMm1000 CMotionControlBase::ToMm1000_1_400
#define ConversionToMachine CMotionControlBase::ToMachine_1_400
//#define ConversionToMm1000 MiniCNCToMm1000
//#define ConversionToMachine MiniCNCToMachine

#define X_STEPSPERMM 400.0
#define Y_STEPSPERMM 400.0
#define Z_STEPSPERMM 400.0
#define A_STEPSPERMM 400.0

////////////////////////////////////////////////////////

#define CONTROLLERFAN_ONTIME	10000			// switch off controllerfan if idle for 10 Sec
//#define CONTROLLERFAN_FAN_PIN	-1 //14 // 10
#undef CONTROLLERFAN_FAN_PIN

#define CONTROLLERFAN_DIGITAL_ON  HIGH
#define CONTROLLERFAN_DIGITAL_OFF LOW

////////////////////////////////////////////////////////

#define SPINDEL_ENABLE_PIN	15

#define SPINDEL_DIGITAL_ON  LOW
#define SPINDEL_DIGITAL_OFF HIGH

////////////////////////////////////////////////////////

#define PROBE_PIN	16

#define PROBE_ON  LOW
#define PROBE_OFF HIGH

////////////////////////////////////////////////////////

#undef KILL_PIN	
