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

#define X_MAXSIZE 320000				// in mm1000_t
#define Y_MAXSIZE 220000 
#define Z_MAXSIZE 100000 
#define A_MAXSIZE 50000 

#define X_USEREFERENCE	EReverenceType::ReferenceToMin
#define Y_USEREFERENCE	EReverenceType::ReferenceToMax
#define Z_USEREFERENCE	EReverenceType::NoReference
#define A_USEREFERENCE	EReverenceType::NoReference

#undef NOGOTOREFERENCEATBOOT

#define REFMOVE_1_AXIS  X_AXIS
#define REFMOVE_2_AXIS  Y_AXIS
#define REFMOVE_3_AXIS  255
#define REFMOVE_4_AXIS  255

////////////////////////////////////////////////////////

//#define STEPPERTYPE 4		// CStepperCNCShield
#define STEPPERTYPE 5   // RampsFD

////////////////////////////////////////////////////////

#if STEPPERTYPE==4

#include "Configuration_DCK40Laser_CNCShield.h"

#elif STEPPERTYPE==5

#include "Configuration_DCK40Laser_RampsFD.h"

#endif

////////////////////////////////////////////////////////

#define TOOTH 20
#define TOOTHSIZE 2
#define STEPROTATION 6400.0

#define X_STEPSPERMM (STEPROTATION/(TOOTH*TOOTHSIZE))
#define Y_STEPSPERMM (STEPROTATION/(TOOTH*TOOTHSIZE))
#define Z_STEPSPERMM (STEPROTATION/(TOOTH*TOOTHSIZE))
#define A_STEPSPERMM (STEPROTATION/(TOOTH*TOOTHSIZE))

////////////////////////////////////////////////////////

#define MOVEAWAYFROMREF_STEPS 100

////////////////////////////////////////////////////////

#define CNC_MAXSPEED 55000        // steps/sec
#define CNC_ACC  700
#define CNC_DEC  800

////////////////////////////////////////////////////////

#define GO_DEFAULT_STEPRATE		CNC_MAXSPEED	// steps/sec
#define G1_DEFAULT_STEPRATE		10000	// steps/sec
#define G1_DEFAULT_MAXSTEPRATE	CNC_MAXSPEED	// steps/sec

#define STEPRATERATE_REFMOVE	5000

#undef SETDIRECTION
//#define SETDIRECTION (1 << X_AXIS) + (1 << Y_AXIS)		// set bit to invert direction of each axis

////////////////////////////////////////////////////////

#define ConversionToMm1000 CNCShieldToMm1000
#define ConversionToMachine CNCShieldToMachine

////////////////////////////////////////////////////////

inline mm1000_t CNCShieldToMm1000(axis_t axis, sdist_t val)
{
	switch (axis)
	{
		default:
		case X_AXIS: return  (mm1000_t)(val * (1000.0 / X_STEPSPERMM));
		case Y_AXIS: return  (mm1000_t)(val * (1000.0 / Y_STEPSPERMM));
		case Z_AXIS: return  (mm1000_t)(val * (1000.0 / Z_STEPSPERMM));
		case A_AXIS: return  (mm1000_t)(val * (1000.0 / A_STEPSPERMM));
	}
}

inline sdist_t CNCShieldToMachine(axis_t axis, mm1000_t  val)
{
	switch (axis)
	{
		default:
		case X_AXIS: return  (sdist_t)(val * (X_STEPSPERMM / 1000.0));
		case Y_AXIS: return  (sdist_t)(val * (Y_STEPSPERMM / 1000.0));
		case Z_AXIS: return  (sdist_t)(val * (Z_STEPSPERMM / 1000.0));
		case A_AXIS: return  (sdist_t)(val * (A_STEPSPERMM / 1000.0));
	}
}

////////////////////////////////////////////////////////

#include <MessageCNCLib.h>

#define MESSAGE_MYCONTROL_Laser_Starting					F("DC-K40 Laser: (" __DATE__ ", " __TIME__ ")")

