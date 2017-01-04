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

#define X_MAXSIZE 36000				// in mm1000_t
#define Y_MAXSIZE 36000 
#define Z_MAXSIZE 10000 
#define A_MAXSIZE 50000 

////////////////////////////////////////////////////////
// NoReference, ReferenceToMin, ReferenceToMax

#define X_USEREFERENCE	EReverenceType::NoReference
#define Y_USEREFERENCE	EReverenceType::NoReference
#define Z_USEREFERENCE	EReverenceType::NoReference
#define A_USEREFERENCE	EReverenceType::NoReference

#define REFMOVE_1_AXIS	255
#define REFMOVE_2_AXIS	255
#define REFMOVE_3_AXIS	255
#define REFMOVE_4_AXIS	255

////////////////////////////////////////////////////////

#define STEPPERTYPE 4		// CStepperCNCShield

////////////////////////////////////////////////////////

#if STEPPERTYPE==4
#include "Configuration_Laser_CNCShield.h"
#endif

////////////////////////////////////////////////////////

#define GO_DEFAULT_STEPRATE		((steprate_t) CConfigEeprom::GetSlotU32(CConfigEeprom::MaxStepRate))	// steps/sec
#define G1_DEFAULT_STEPRATE		10000			// steps/sec
#define G1_DEFAULT_MAXSTEPRATE	((steprate_t) CConfigEeprom::GetSlotU32(CConfigEeprom::MaxStepRate))	// steps/sec

#define STEPRATERATE_REFMOVE	CNC_MAXSPEED // GO_DEFAULT_STEPRATE

////////////////////////////////////////////////////////

extern float scaleToMm;
extern float scaleToMachine;

// 3 mm/rot
// 20 steps/rot
// * 16 => 1/16 step

#define X_STEPSPERMM (20.0/3*16)
#define Y_STEPSPERMM (20.0/3*16)
#define Z_STEPSPERMM (20.0/3*16)
#define A_STEPSPERMM (20.0/3*16)

inline mm1000_t LaserToMm1000(axis_t axis, sdist_t val)
{
	switch (axis)
	{
		default:
		case X_AXIS: return  (mm1000_t)(val * scaleToMm);
		case Y_AXIS: return  (mm1000_t)(val * scaleToMm);
		case Z_AXIS: return  (mm1000_t)(val * scaleToMm);
		case A_AXIS: return  (mm1000_t)(val * scaleToMm);
	}
}

inline sdist_t LaserToMachine(axis_t axis, mm1000_t  val)
{
	switch (axis)
	{
		default:
		case X_AXIS: return  (sdist_t)(val * scaleToMachine);
		case Y_AXIS: return  (sdist_t)(val * scaleToMachine);
		case Z_AXIS: return  (sdist_t)(val * scaleToMachine);
		case A_AXIS: return  (sdist_t)(val * scaleToMachine);
	}
}

////////////////////////////////////////////////////////

#include <MessageCNCLib.h>

#define MESSAGE_MYCONTROL_Starting					F("MiniL:" __DATE__ )

