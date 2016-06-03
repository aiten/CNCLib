////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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

#define CMyStepper CStepperCNCShield
#define ConversionToMm1000 CNCShieldToMm1000
#define ConversionToMachine CNCShieldToMachine

////////////////////////////////////////////////////////

#define MYNUM_AXIS 3
#define CNCSHIELD_NUM_AXIS MYNUM_AXIS

#include <Steppers/StepperCNCShield_pins.h>

// change some pin definition here:

// use ZMinRef for analog laser PWM
#undef CNCSHIELD_Z_MIN_PIN
#undef CNCSHIELD_Z_MAX_PIN
#define CNCSHIELD_Z_MIN_PIN 10

#undef CNCSHIELD_REF_ON
#undef CNCSHIELD_REF_OFF

#define CNCSHIELD_REF_ON      1
#define CNCSHIELD_REF_OFF     0

#include <Steppers/StepperCNCShield.h>

////////////////////////////////////////////////////////

#define KILL_PIN		CNCSHIELD_ABORT_PIN
#define KILL_PIN_ON		CNCSHIELD_ABORT_ON

#define HOLD_PIN CNCSHIELD_HOLD_PIN
#define RESUME_PIN CNCSHIELD_RESUME_PIN

#define LASER_PWM_PIN  11
#define LASER_ENABLE_PIN  CNCSHIELD_SPINDEL_ENABLE_PIN
#define LASER_ENABLE_ON  LOW
#define LASER_ENABLE_OFF HIGH

#define LASERWATER_PIN	CNCSHIELD_A4_PIN
#define LASERWATER_ON  HIGH
#define LASERWATER_OFF LOW
#define LASERWATER_ONTIME	10000			// switch off if idle for 10 Sec

#define LASERVACUUM_PIN	CNCSHIELD_A5_PIN
#define LASERVACUUM_ON  HIGH
#define LASERVACUUM_OFF LOW

////////////////////////////////////////////////////////

#define TOOTH 20
#define TOOTHSIZE 2
#define STEPROTATION 6400.0

#define X_STEPSPERMM (STEPROTATION/(TOOTH*TOOTHSIZE))
#define Y_STEPSPERMM (STEPROTATION/(TOOTH*TOOTHSIZE))
#define Z_STEPSPERMM (STEPROTATION/(TOOTH*TOOTHSIZE))
#define A_STEPSPERMM (STEPROTATION/(TOOTH*TOOTHSIZE))

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

#define X_MAXSIZE 320000				// in mm1000_t
#define Y_MAXSIZE 220000 
#define Z_MAXSIZE 100000 
#define A_MAXSIZE 50000 

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

#undef NOGOTOREFERENCEATBOOT
//#define NOGOTOREFERENCEATBOOT

////////////////////////////////////////////////////////

#include <MessageCNCLib.h>

