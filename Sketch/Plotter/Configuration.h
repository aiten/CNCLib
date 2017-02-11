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

//#define STEPPERTYPE 1		// CStepperL298N
//#define STEPPERTYPE 2		// CStepperSMC800
//#define STEPPERTYPE 3		// CStepperTB6560
#define STEPPERTYPE 4		// CStepperRamps14

////////////////////////////////////////////////////////

#define USBBAUDRATE 250000

////////////////////////////////////////////////////////

#define MYNUM_AXIS 3

////////////////////////////////////////////////////////

#define X_MAXSIZE 520000		//520mm
#define Y_MAXSIZE 295000		//295mm
#define Z_MAXSIZE 21000			//21mm
#define A_MAXSIZE 360000 
#define B_MAXSIZE 360000 
#define C_MAXSIZE 360000 

////////////////////////////////////////////////////////

#define STEPPERDIRECTION 0		// set bit to invert direction of each axis

#define STEPSPERROTATION	200
#define MICROSTEPPING		16

// 3200steps/rot, T2.5 belt pully with 12teeth

#define TOOTH 12
#define TOOTHSIZE 2.5
#define SCREWLEAD (TOOTH*TOOTHSIZE)

#define X_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define Y_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define Z_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define A_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define B_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define C_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)

////////////////////////////////////////////////////////

#define CNC_MAXSPEED 25000        // steps/sec
#define CNC_ACC  400
#define CNC_DEC  450

////////////////////////////////////////////////////////
// NoReference, ReferenceToMin, ReferenceToMax

#define X_USEREFERENCE	EReverenceType::ReferenceToMin
#define Y_USEREFERENCE	EReverenceType::ReferenceToMin
#define Z_USEREFERENCE	EReverenceType::ReferenceToMin
#define A_USEREFERENCE	EReverenceType::NoReference
#define B_USEREFERENCE	EReverenceType::NoReference
#define C_USEREFERENCE	EReverenceType::NoReference

#define REFMOVE_1_AXIS  Z_AXIS
#define REFMOVE_2_AXIS  Y_AXIS
#define REFMOVE_3_AXIS  X_AXIS
#define REFMOVE_4_AXIS  255
#define REFMOVE_5_AXIS  255
#define REFMOVE_6_AXIS  255

#define X_REFERENCEHITVALUE LOW
#define Y_REFERENCEHITVALUE LOW
#define Z_REFERENCEHITVALUE LOW
#define A_REFERENCEHITVALUE LOW
#define B_REFERENCEHITVALUE LOW
#define C_REFERENCEHITVALUE LOW

#define MOVEAWAYFROMREF_MM1000 500

#define SPINDLE_ANALOGSPEED
#define SPINDLE_MAXSPEED	10000			// analog 255

////////////////////////////////////////////////////////

#if STEPPERTYPE==1
#include "Configuration_L298N.h"
#elif STEPPERTYPE==2
#include "Configuration_SMC800.h"
#elif STEPPERTYPE==3
#include "Configuration_TB6560.h"
#elif STEPPERTYPE==4

#include "Configuration_Ramps14.h"

#endif

////////////////////////////////////////////////////////

#define STEPRATERATE_REFMOVE	(CNC_MAXSPEED/4)

////////////////////////////////////////////////////////

#define MESSAGE_MYCONTROL_Starting F("Plotter(HA) is starting ... (" __DATE__ ", " __TIME__ ")")
