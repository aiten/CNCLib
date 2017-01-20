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

#define STEPPERTYPE 4		// CStepperCNCShield

////////////////////////////////////////////////////////

#define USBBAUDRATE 250000

////////////////////////////////////////////////////////

#define X_MAXSIZE 400000				// in mm1000_t
#define Y_MAXSIZE 380000 
#define Z_MAXSIZE 100000 
#define A_MAXSIZE 50000 

////////////////////////////////////////////////////////

#define STEPPERDIRECTION (1 << X_AXIS) + (1 << Y_AXIS)		// set bit to invert direction of each axis

#define STEPSPERROTATION	200
#define MICROSTEPPING		16

// GT2 with 15Tooth = > 30mm

#define TOOTH 15
#define TOOTHSIZE 2.0
#define SCREWLEAD (TOOTH*TOOTHSIZE)

#define X_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define Y_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define Z_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define A_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)



////////////////////////////////////////////////////////

#define CNC_MAXSPEED 27000        // steps/sec
#define CNC_ACC  350
#define CNC_DEC  400

////////////////////////////////////////////////////////
// NoReference, ReferenceToMin, ReferenceToMax

#define X_USEREFERENCE	EReverenceType::ReferenceToMin
#define Y_USEREFERENCE	EReverenceType::ReferenceToMin
#define Z_USEREFERENCE	EReverenceType::NoReference
#define A_USEREFERENCE	EReverenceType::NoReference

#define REFMOVE_1_AXIS  Y_AXIS
#define REFMOVE_2_AXIS  X_AXIS
#define REFMOVE_3_AXIS  255
#define REFMOVE_4_AXIS  255

#define MOVEAWAYFROMREF_MM1000 500

#define SPINDLE_ANALOGSPEED
#define SPINDLE_MAXSPEED	255			// analog 255

////////////////////////////////////////////////////////

#if STEPPERTYPE==4
#include "Configuration_CNCShield.h"
#endif

////////////////////////////////////////////////////////

// do not use probe
#undef PROBE_PIN

////////////////////////////////////////////////////////

#define GO_DEFAULT_STEPRATE		((steprate_t) CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, maxsteprate)))	// steps/sec
#define G1_DEFAULT_MAXSTEPRATE	((steprate_t) CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, maxsteprate)))	// steps/sec
#define G1_DEFAULT_FEEDPRATE	100000	// in mm1000 / min

#define STEPRATERATE_REFMOVE	4000

////////////////////////////////////////////////////////

#define MESSAGE_MYCONTROL_Starting					F("Laser:" __DATE__ )

