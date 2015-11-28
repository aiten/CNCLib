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

#define CNCSHIELD_NUM_AXIS 4

////////////////////////////////////////////////////////

#define X_STEPSPERMM 3200.0
#define Y_STEPSPERMM 3200.0
#define Z_STEPSPERMM 3200.0
#define A_STEPSPERMM 3200.0

////////////////////////////////////////////////////////

#define X_MAXSIZE 200000				// in mm1000_t
#define Y_MAXSIZE 200000 
#define Z_MAXSIZE 100000 
#define A_MAXSIZE 50000 

////////////////////////////////////////////////////////

#define X_USEREFERENCE_MIN	
//#define X_USEREFERENCE_MAX

#define Y_USEREFERENCE_MIN	
//#define Y_USEREFERENCE_MAX

//#define Z_USEREFERENCE_MIN	
#define Z_USEREFERENCE_MAX

//#define A_USEREFERENCE_MIN	
//#define A_USEREFERENCE_MAX

#define REFMOVE_1_AXIS	Z_AXIS
#define REFMOVE_2_AXIS	Y_AXIS
#define REFMOVE_3_AXIS	X_AXIS
//#define REFMOVE_3_AXIS	A_AXIS

////////////////////////////////////////////////////////

#define GO_DEFAULT_STEPRATE		20000	// steps/sec
#define G1_DEFAULT_STEPRATE		10000	// steps/sec

////////////////////////////////////////////////////////

#undef ANALOGSPINDELSPEED
#define MAXSPINDLESPEED 25000			// analog 255

////////////////////////////////////////////////////////

#define NOGOTOREFERENCEATBOOT

////////////////////////////////////////////////////////

#define CNC_MAXSPEED 14000				// steps/sec
#define CNC_ACC  350
#define CNC_DEC  400

////////////////////////////////////////////////////////

#if defined(__AVR_ATmega328P__) || defined (_MSC_VER)
#define CNCSHIELD_PROBE_PIN     18    // AD4 (Mega => 58)
#else
#define CNCSHIELD_PROBE_PIN     58    // AD4 (Mega => 58)
#endif

#define CNCSHIELD_PROBE_ON      LOW
#define CNCSHIELD_PROBE_OFF     HIGH

////////////////////////////////////////////////////////

#include <MessageCNCLib.h>

#define MESSAGE_MYCONTROL_CNCShield_Starting					F("CNCShield:" __DATE__ )


