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

//#define STEPPERTYPE 4		// CStepperCNCShield
#define STEPPERTYPE 5   // RampsFD

////////////////////////////////////////////////////////

#define USBBAUDRATE 250000

////////////////////////////////////////////////////////

#define MYNUM_AXIS 2

////////////////////////////////////////////////////////

#define X_MAXSIZE 320000				// in mm1000_t
#define Y_MAXSIZE 220000 
#define Z_MAXSIZE 100000 
#define A_MAXSIZE 1000 
#define B_MAXSIZE 1000 
#define C_MAXSIZE 1000 

#undef SETDIRECTION
//#define SETDIRECTION (1 << X_AXIS) + (1 << Y_AXIS)		// set bit to invert direction of each axis

#define STEPSPERROTATION	400
#define MICROSTEPPING		16

#define TOOTH 20
#define TOOTHSIZE 2.0
#define STEPROTATION (STEPSPERROTATION*MICROSTEPPING)

#define X_STEPSPERMM (STEPROTATION/(TOOTH*TOOTHSIZE))
#define Y_STEPSPERMM (STEPROTATION/(TOOTH*TOOTHSIZE))
#define Z_STEPSPERMM (STEPROTATION/(TOOTH*TOOTHSIZE))
#define A_STEPSPERMM (STEPROTATION/(TOOTH*TOOTHSIZE))
#define B_STEPSPERMM (STEPROTATION/(TOOTH*TOOTHSIZE))
#define C_STEPSPERMM (STEPROTATION/(TOOTH*TOOTHSIZE))

#define ConversionToMm1000 MyConvertToMm1000
#define ConversionToMachine MyConvertToMachine

////////////////////////////////////////////////////////

#define CNC_MAXSPEED 55000        // steps/sec
#define CNC_ACC  700
#define CNC_DEC  800

////////////////////////////////////////////////////////
// NoReference, ReferenceToMin, ReferenceToMax

#define X_USEREFERENCE	EReverenceType::ReferenceToMin
#define Y_USEREFERENCE	EReverenceType::ReferenceToMax
#define Z_USEREFERENCE	EReverenceType::NoReference
#define A_USEREFERENCE	EReverenceType::NoReference
#define B_USEREFERENCE	EReverenceType::NoReference
#define C_USEREFERENCE	EReverenceType::NoReference

#define REFMOVE_1_AXIS  X_AXIS
#define REFMOVE_2_AXIS  Y_AXIS
#define REFMOVE_3_AXIS  255
#define REFMOVE_4_AXIS  255
#define REFMOVE_5_AXIS  255
#define REFMOVE_6_AXIS  255

#define MOVEAWAYFROMREF_STEPS 100

////////////////////////////////////////////////////////

#if STEPPERTYPE==4

#include "Configuration_CNCShield.h"
#elif STEPPERTYPE==5
#include "Configuration_RampsFD.h"
#endif

////////////////////////////////////////////////////////

#define GO_DEFAULT_STEPRATE		((steprate_t) CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, maxsteprate)))	// steps/sec
#define G1_DEFAULT_STEPRATE		(CNC_MAXSPEED/2)			// steps/sec
#define G1_DEFAULT_MAXSTEPRATE	((steprate_t) CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, maxsteprate)))	// steps/sec

#define STEPRATERATE_REFMOVE	5000

////////////////////////////////////////////////////////

#define MESSAGE_MYCONTROL_Starting					F("DC-K40 Laser: (" __DATE__ ", " __TIME__ ")")

