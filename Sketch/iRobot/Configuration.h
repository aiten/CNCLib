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

#define CMyStepper CStepperServo
#define ConversionToMm1000 CMotionControlBase::ToMm1000_1_1000
#define ConversionToMachine CMotionControlBase::ToMachine_1_1000

////////////////////////////////////////////////////////

#define USBBAUDRATE 115200

////////////////////////////////////////////////////////

#define MYNUM_AXIS 6
#define LCD_NUMAXIS  MYNUM_AXIS
#define MYUSE_LCD

////////////////////////////////////////////////////////

#define X_MAXSIZE 300000				// in mm1000_t
#define Y_MAXSIZE 300000 
#define Z_MAXSIZE 300000 
#define A_MAXSIZE 1000 
#define B_MAXSIZE 1000 
#define C_MAXSIZE 1000 

#define STEPPERDIRECTION 0		// set bit to invert direction of each axis

#define STEPSPERROTATION	1
#define MICROSTEPPING		1
#define SCREWLEAD			1.0

#define X_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define Y_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define Z_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define A_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define B_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define C_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)

////////////////////////////////////////////////////////

#define CNC_MAXSPEED 50000
#define CNC_ACC  350
#define CNC_DEC  400

////////////////////////////////////////////////////////
// NoReference, ReferenceToMin, ReferenceToMax

#define X_USEREFERENCE  EReverenceType::NoReference
#define Y_USEREFERENCE  EReverenceType::NoReference
#define Z_USEREFERENCE  EReverenceType::NoReference
#define A_USEREFERENCE  EReverenceType::NoReference
#define B_USEREFERENCE  EReverenceType::NoReference
#define C_USEREFERENCE  EReverenceType::NoReference

#define REFMOVE_1_AXIS  255
#define REFMOVE_2_AXIS  255
#define REFMOVE_3_AXIS  255
#define REFMOVE_4_AXIS  255
#define REFMOVE_5_AXIS  255
#define REFMOVE_6_AXIS  255

#define X_REFERENCEHITVALUE_MIN LOW
#define Y_REFERENCEHITVALUE_MIN LOW
#define Z_REFERENCEHITVALUE_MIN LOW
#define A_REFERENCEHITVALUE_MIN LOW
#define B_REFERENCEHITVALUE_MIN LOW
#define C_REFERENCEHITVALUE_MIN LOW

#define X_REFERENCEHITVALUE_MAX LOW
#define Y_REFERENCEHITVALUE_MAX LOW
#define Z_REFERENCEHITVALUE_MAX LOW
#define A_REFERENCEHITVALUE_MAX LOW
#define B_REFERENCEHITVALUE_MAX LOW
#define C_REFERENCEHITVALUE_MAX LOW

#define MOVEAWAYFROMREF_MM1000 250

#undef SPINDLE_ANALOGSPEED
#define SPINDLE_MAXSPEED	255			// analog 255

////////////////////////////////////////////////////////

#define LCD_KILL_PIN  41

#define KILL_ON  LOW
#define KILL_OFF HIGH

#define GO_DEFAULT_STEPRATE		((steprate_t) CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, maxsteprate)))	// steps/sec
#define G1_DEFAULT_MAXSTEPRATE	((steprate_t) CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, maxsteprate)))	// steps/sec
#define G1_DEFAULT_FEEDPRATE	100000	// in mm1000 / min

#define STEPRATERATE_REFMOVE	(CNC_MAXSPEED/4)

////////////////////////////////////////////////////////

#define LCD_GROW 64
#define LCD_GCOL 128

#define LCD_BEEPER 37

#define ROTARY_ENC           35
#define ROTARY_ENC_ON		 LOW

#define ROTARY_EN1           31
#define ROTARY_EN2           33

#define SD_ENABLE_PIN		 53

////////////////////////////////////////////////////////

#include <MessageCNCLib.h>

#define MESSAGE_MYCONTROL_Starting					F("iRobotCNC:" __DATE__ )

