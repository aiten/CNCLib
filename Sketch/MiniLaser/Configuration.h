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

#define X_MAXSIZE 36000				// in mm1000_t
#define Y_MAXSIZE 36000 
#define Z_MAXSIZE 10000 
#define A_MAXSIZE 50000 
// 3 mm/rot
// 20 steps/rot
// * 16 => 1/16 step

#define X_STEPSPERMM (20.0/3*16)
#define Y_STEPSPERMM (20.0/3*16)
#define Z_STEPSPERMM (20.0/3*16)
#define A_STEPSPERMM (20.0/3*16)

////////////////////////////////////////////////////////

#define CNC_MAXSPEED 20000 // 27000        // steps/sec
#define CNC_ACC  500
#define CNC_DEC  550

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

#define MOVEAWAYFROMREF_MM1000 250

#define SPINDEL_ANALOGSPEED
#define SPINDEL_MAXSPEED	255			// analog 255

////////////////////////////////////////////////////////

#if STEPPERTYPE==4
#include "Configuration_CNCShield.h"
#endif

////////////////////////////////////////////////////////

#define GO_DEFAULT_STEPRATE		((steprate_t) CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, maxsteprate)))	// steps/sec
#define G1_DEFAULT_MAXSTEPRATE	((steprate_t) CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, maxsteprate)))	// steps/sec
#define G1_DEFAULT_FEEDPRATE	100000	// in mm1000 / min

#define STEPRATERATE_REFMOVE	(CNC_MAXSPEED/4)

////////////////////////////////////////////////////////

#define MESSAGE_MYCONTROL_Starting					F("MiniL:" __DATE__ )

