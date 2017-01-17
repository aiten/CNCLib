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

#define MYUSE_LCD

////////////////////////////////////////////////////////

#define HARDWARETYPE 1			// ramps 1.4, microsteps 16
//#define HARDWARETYPE 2			// rampsfd, microsteps 32


#if HARDWARETYPE==1

#define USE_RAMPS14

#define SPEEDFACTOR 1
#define SPEEDFACTOR_SQT 1

#elif HARDWARETYPE==2

#define USE_RAMPSFD

#define SPEEDFACTOR 2
#define SPEEDFACTOR_SQT 1.41421356237309504880

#else

#endif

////////////////////////////////////////////////////////

#define USBBAUDRATE 115200

////////////////////////////////////////////////////////

#define X_MAXSIZE 130000				// in mm1000_t
#define Y_MAXSIZE 45000 
#define Z_MAXSIZE 81000 
#define A_MAXSIZE 360000 
#define B_MAXSIZE 360000 
#define C_MAXSIZE 360000 

////////////////////////////////////////////////////////

//#define SETDIRECTION (1<<X_AXIS) + (1<<Y_AXIS)		// set bit to invert direction of each axis

////////////////////////////////////////////////////////

#define STEPSPERROTATION	200
#define MICROSTEPPING		(16*SPEEDFACTOR)
#define SCREWLEAD			1.0

#define X_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define Y_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define Z_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define A_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define B_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define C_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)

////////////////////////////////////////////////////////

#define CNC_MAXSPEED (28000*SPEEDFACTOR)        // steps/sec
#define CNC_ACC  (350*SPEEDFACTOR_SQT)
#define CNC_DEC  (400*SPEEDFACTOR_SQT)

////////////////////////////////////////////////////////
// NoReference, ReferenceToMin, ReferenceToMax

#define X_USEREFERENCE	EReverenceType::ReferenceToMin
#define Y_USEREFERENCE	EReverenceType::ReferenceToMin
#define Z_USEREFERENCE	EReverenceType::ReferenceToMax
#define A_USEREFERENCE	EReverenceType::NoReference
#define B_USEREFERENCE  EReverenceType::NoReference
#define C_USEREFERENCE  EReverenceType::NoReference

#define REFMOVE_1_AXIS  Z_AXIS
#define REFMOVE_2_AXIS  Y_AXIS
#define REFMOVE_3_AXIS  X_AXIS
#define REFMOVE_4_AXIS  255
#define REFMOVE_5_AXIS  255
#define REFMOVE_6_AXIS  255

#define MOVEAWAYFROMREF_MM1000 125

#undef SPINDEL_ANALOGSPEED
#define SPINDEL_MAXSPEED	255			// analog 255

////////////////////////////////////////////////////////

#if defined(USE_RAMPS14)
#include "Configuration_Ramps14.h"
#elif  defined(USE_RAMPSFD)
#include "Configuration_RampsFD.h"
#endif

////////////////////////////////////////////////////////

#define GO_DEFAULT_STEPRATE		((steprate_t) CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, maxsteprate)))	// steps/sec
#define G1_DEFAULT_MAXSTEPRATE	((steprate_t) CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, maxsteprate)))	// steps/sec
#define G1_DEFAULT_FEEDPRATE	100000	// in mm1000 / min

#define STEPRATERATE_REFMOVE	(CNC_MAXSPEED/4)

////////////////////////////////////////////////////////

#define CONTROLLERFAN_ONTIME	10000			// switch off controllerfan if idle for 10 Sec
#define CONTROLLERFAN_FAN_PIN	CAT(BOARDNAME,_FET2D9_PIN)
#define CONTROLLERFAN_ANALOGSPEED

////////////////////////////////////////////////////////

#define COOLANT_PIN	CAT(BOARDNAME,_AUX2_8)	// Ramps1.4 D42

#define COOLANT_ON  LOW
#define COOLANT_OFF HIGH

////////////////////////////////////////////////////////

#define SPINDEL_ENABLE_PIN	CAT(BOARDNAME,_AUX2_6)	// Ramps1.4 D40

#define SPINDEL_DIGITAL_ON  LOW
#define SPINDEL_DIGITAL_OFF HIGH

////////////////////////////////////////////////////////

#define PROBE_PIN	CAT(BOARDNAME,_AUX2_7)	// Ramps 1.4 D44 
#define PROBE2_PIN	CAT(BOARDNAME,_AUX2_5)	// Ramps 1.4 A10 

#define PROBE_ON  LOW
#define PROBE_OFF HIGH

////////////////////////////////////////////////////////

#define LCD_GROW 64
#define LCD_GCOL 128

#define ROTARY_ENC           CAT(BOARDNAME,_LCD_ROTARY_ENC)
#define ROTARY_ENC_ON		 CAT(BOARDNAME,_LCD_ROTARY_ENC_ON)

////////////////////////////////////////////////////////

#include <MessageCNCLibEx.h>

#if defined(__SAM3X8E__)
#if defined(USE_RAMPS14)
#define MESSAGE_MYCONTROL_Starting					F("Proxxon MF 70(HA) Ramps 1.4 due is starting ... (" __DATE__ ", " __TIME__ ")")
#elif defined(USE_RAMPSFD)
#define MESSAGE_MYCONTROL_Starting					F("Proxxon MF 70(HA) Ramps FD due is starting ... (" __DATE__ ", " __TIME__ ")")
#endif
#elif defined(__AVR_ATmega1280__) || defined(__AVR_ATmega2560__)
#define MESSAGE_MYCONTROL_Starting					F("Proxxon MF 70(HA) Ramps 1.4 mega is starting ... (" __DATE__ ", " __TIME__ ")")
#else
#define MESSAGE_MYCONTROL_Starting					F("Proxxon MF 70(HA) Ramps 1.4 is starting ... (" __DATE__ ", " __TIME__ ")")
#endif

