////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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

#define STEPPERTYPE 1		// CStepperL298N
//#define STEPPERTYPE 2		// CStepperSMC800
//#define STEPPERTYPE 3		// CStepperTB6560

////////////////////////////////////////////////////////

#if STEPPERTYPE==1			// CStepperL298N

#define CMyStepper CStepperL298N
#define ConversionToMm1000 CMotionControl::ToMm1000_L298N
#define ConversionToMachine CMotionControl::ToMachine_L298N

// 50 steps/rot
inline mm1000_t ToMm1000_L298N(axis_t /* axis */, sdist_t val)				{ return  RoundMulDivU32(val, 80, 4); }
inline sdist_t  ToMachine_L298N(axis_t /* axis */, mm1000_t val)			{ return  MulDivU32(val, 4, 80); }

////////////////////////////////////////////////////////

#define CONTROLLERFAN_ONTIME	10000			// switch off controllerfan if idle for 10 Sec
#define CONTROLLERFAN_FAN_PIN	13 // 10

////////////////////////////////////////////////////////

#define SPINDEL_PIN	-1

#define SPINDEL_ON  LOW
#define SPINDEL_OFF HIGH

////////////////////////////////////////////////////////

#define PROBE1_PIN	-1

#define PROBE_ON  LOW
#define PROBE_OFF HIGH

////////////////////////////////////////////////////////

#elif STEPPERTYPE==2

#define CMyStepper CStepperSMC800
#define ConversionToMm1000 CMotionControl::ToMm1000_1_3200
#define ConversionToMachine CMotionControl::ToMachine_1_3200

////////////////////////////////////////////////////////

#define CONTROLLERFAN_ONTIME	10000			// switch off controllerfan if idle for 10 Sec
#define CONTROLLERFAN_FAN_PIN	13 // 10

////////////////////////////////////////////////////////

#define SPINDEL_PIN	11

#define SPINDEL_ON  LOW
#define SPINDEL_OFF HIGH

////////////////////////////////////////////////////////

#define PROBE1_PIN	12

#define PROBE_ON  LOW
#define PROBE_OFF HIGH

////////////////////////////////////////////////////////

#elif STEPPERTYPE==3

#define CMyStepper CStepperTB6560
#define ConversionToMm1000 CMotionControl::ToMm1000_1_3200
#define ConversionToMachine CMotionControl::ToMachine_1_3200

////////////////////////////////////////////////////////

#define CONTROLLERFAN_ONTIME	10000			// switch off controllerfan if idle for 10 Sec
#define CONTROLLERFAN_FAN_PIN	13 // 10

////////////////////////////////////////////////////////

#define SPINDEL_PIN	11

#define SPINDEL_ON  LOW
#define SPINDEL_OFF HIGH

////////////////////////////////////////////////////////

#define PROBE1_PIN	12

#define PROBE_ON  LOW
#define PROBE_OFF HIGH

////////////////////////////////////////////////////////

#endif

#include <MessageCNCLib.h>

#define MESSAGE_MYCONTROL_Proxxon_Starting					F("MiniCNC:"__DATE__)

