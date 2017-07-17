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

#define CMyStepper CStepperRampsFD

////////////////////////////////////////////////////////

#define USBBAUDRATE 115200

////////////////////////////////////////////////////////

#define MYUSE_LCD

////////////////////////////////////////////////////////

#define X_MAXSIZE 134000				// in mm1000_t
#define Y_MAXSIZE 134000 
#define Z_MAXSIZE 83000 
#define A_MAXSIZE 360000 
#define B_MAXSIZE 360000 
#define C_MAXSIZE 360000 

////////////////////////////////////////////////////////

//#define STEPPERDIRECTION 0
//#define STEPPERDIRECTION (1<<X_AXIS) + (1<<Y_AXIS)		// set bit to invert direction of each axis
#define STEPPERDIRECTION (1<<Y_AXIS)                  // set bit to invert direction of each axis

#define STEPSPERROTATION  200
#define MICROSTEPPING     32
#define SCREWLEAD         5.0

////////////////////////////////////////////////////////

#define CNC_MAXSPEED ((steprate_t)50000)        // steps/sec => 50000 => 7.8 rot /sec
#define CNC_ACC  496                            // 0.257 => time to full speed
#define CNC_DEC  565                            // 0.1975 => time to break
#define CNC_JERKSPEED 2240

////////////////////////////////////////////////////////
// NoReference, ReferenceToMin, ReferenceToMax

#define X_USEREFERENCE	EReverenceType::ReferenceToMin
#define Y_USEREFERENCE	EReverenceType::ReferenceToMax
//#define Z_USEREFERENCE	EReverenceType::ReferenceToMax
#define Z_USEREFERENCE  EReverenceType::NoReference
#define A_USEREFERENCE	EReverenceType::NoReference
#define B_USEREFERENCE	EReverenceType::NoReference
#define C_USEREFERENCE	EReverenceType::NoReference

#define REFMOVE_1_AXIS  255
#define REFMOVE_2_AXIS  X_AXIS
#define REFMOVE_3_AXIS  Y_AXIS
#define REFMOVE_4_AXIS  255
#define REFMOVE_5_AXIS  255
#define REFMOVE_6_AXIS  255

#define X_REFERENCEHITVALUE_MIN LOW
#define Y_REFERENCEHITVALUE_MIN 255
#define Z_REFERENCEHITVALUE_MIN 255
#define A_REFERENCEHITVALUE_MIN 255
#define B_REFERENCEHITVALUE_MIN 255
#define C_REFERENCEHITVALUE_MIN 255

#define X_REFERENCEHITVALUE_MAX 255
#define Y_REFERENCEHITVALUE_MAX LOW
#define Z_REFERENCEHITVALUE_MAX LOW
#define A_REFERENCEHITVALUE_MAX 255
#define B_REFERENCEHITVALUE_MAX 255
#define C_REFERENCEHITVALUE_MAX 255

#define MOVEAWAYFROMREF_MM1000 125

#undef SPINDLE_ANALOGSPEED
#define SPINDLE_MAXSPEED	255			// analog 255
#define SPINDEL_FADETIMEDELAY  0	// 8ms * 255 => 2040ms from 0 to max, 4080 from -max to +max

////////////////////////////////////////////////////////

#include "ConfigurationStepper_RampsFD.h"

////////////////////////////////////////////////////////

#define GO_DEFAULT_STEPRATE		  ((steprate_t) CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, maxsteprate)))	// steps/sec
#define G1_DEFAULT_MAXSTEPRATE	((steprate_t) CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, maxsteprate)))	// steps/sec
#define G1_DEFAULT_FEEDPRATE	  100000	// in mm1000 / min

#define STEPRATERATE_REFMOVE	  (CNC_MAXSPEED/3)
#define FEEDRATE_REFMOVE_PHASE2		200000

////////////////////////////////////////////////////////

#define CONTROLLERFAN_ONTIME	  10000			// switch off controllerfan if idle for 10 Sec
#define CONTROLLERFAN_FAN_PIN	  CAT(BOARDNAME,_FET2D9_PIN)
#define CONTROLLERFAN_ANALOGSPEED
#define CONTROLLERFAN_ANALOGSPEED_INVERT

////////////////////////////////////////////////////////

#define COOLANT_PIN	CAT(BOARDNAME,_AUX2_8)	// Ramps1.4 D42

#define COOLANT_PIN_ON  LOW
#define COOLANT_PIN_OFF HIGH

////////////////////////////////////////////////////////

#define SPINDLE_ENABLE_PIN	CAT(BOARDNAME,_AUX2_6)	// Ramps1.4 D40

#define SPINDLE_DIGITAL_ON  LOW
#define SPINDLE_DIGITAL_OFF HIGH

////////////////////////////////////////////////////////

#define PROBE_PIN	CAT(BOARDNAME,_AUX2_7)	// Ramps 1.4 D44 
#define PROBE2_PIN	CAT(BOARDNAME,_AUX2_5)	// Ramps 1.4 A10 

#define PROBE_PIN_ON  LOW
#define PROBE_PIN_OFF HIGH

#if NUM_AXIS < 6
// LCD KILL is shared with E1 (RampsFD) (DIR)
#define HOLDRESUME_PIN		CAT(BOARDNAME, _LCD_KILL_PIN)
#define HOLDRESUME_PIN_ON	CAT(BOARDNAME, _LCD_KILL_PIN_ON)
#endif

////////////////////////////////////////////////////////

#define LCD_GROW 64
#define LCD_GCOL 128

#define ROTARY_ENC           CAT(BOARDNAME,_LCD_ROTARY_ENC)
#define ROTARY_ENC_ON		 CAT(BOARDNAME,_LCD_ROTARY_ENC_ON)

//#define LCD_MENU_MOVE100

////////////////////////////////////////////////////////

#include <MessageCNCLibEx.h>

#if defined(__SAM3X8E__)
#define MESSAGE_MYCONTROL_Starting			F("My-CNC(HA) Ramps FD due is starting ... (" __DATE__ ", " __TIME__ ")")
#define MESSAGE_LCD_HEADLINE						F("My-CNC RampsFD-D")
#else
#define MESSAGE_MYCONTROL_Starting			F("My-CNC(HA) Ramps FD is starting ... (" __DATE__ ", " __TIME__ ")")
#define MESSAGE_LCD_HEADLINE						F("My-CNC RampsFD-M")
#endif

