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

#include <Steppers/StepperCNCShield.h>

////////////////////////////////////////////////////////

#ifdef CNCSHIELD_ABORT_PIN
#define KILL_PIN		CNCSHIELD_ABORT_PIN
#define KILL_PIN_ON		CNCSHIELD_ABORT_ON
#endif

#ifdef CNCSHIELD_HOLD_PIN
#define HOLD_PIN CNCSHIELD_HOLD_PIN
#endif

#ifdef CNCSHIELD_RESUME_PIN
#define RESUME_PIN CNCSHIELD_RESUME_PIN
#endif

#ifdef CNCSHIELD_PROBE_PIN
#define PROBE_PIN		CNCSHIELD_PROBE_PIN
#define PROBE_ON		CNCSHIELD_PROBE_ON
#endif

#ifdef CNCSHIELD_COOLANT_PIN
#define COOLANT_PIN		CNCSHIELD_COOLANT_PIN
#define COOLANT_ON		CNCSHIELD_COOLANT_ON
#define COOLANT_OFF		CNCSHIELD_COOLANT_OFF
#endif

#ifdef CNCSHIELD_SPINDEL_ENABLE_PIN
#define SPINDEL_ENABLE_PIN	CNCSHIELD_SPINDEL_ENABLE_PIN
#define SPINDEL_DIGITAL_ON	CNCSHIELD_SPINDEL_DIGITAL_ON
#define SPINDEL_DIGITAL_OFF	CNCSHIELD_SPINDEL_DIGITAL_OFF
#define SPINDEL_DIR_PIN		CNCSHIELD_SPINDEL_DIR_PIN
#define SPINDEL_DIR_CLW		CNCSHIELD_SPINDEL_DIR_CLW
#define SPINDEL_DIR_CCLW	CNCSHIELD_SPINDEL_DIR_CCLW
#undef  SPINDEL_ANALOGSPEED
#define SPINDEL_MAXSPEED	25000			// analog 255
#endif

#define LASER_ENABLE_PIN  11
#define LASER_DIGITAL_ON  HIGH
#define LASER_DIGITAL_OFF LOW
#define  LASER_ANALOG

#undef USECONTROLERFAN
#ifdef USECONTROLERFAN
#define CONTROLLERFAN_ONTIME	10000			// switch off controllerfan if idle for 10 Sec
#define CONTROLLERFAN_FAN_PIN	14 // 10

#define CONTROLLERFAN_DIGITAL_ON  HIGH
#define CONTROLLERFAN_DIGITAL_OFF LOW
#undef  CONTROLLERFAN_ANALOGSPEED
#endif

////////////////////////////////////////////////////////

#define DISABLELEDBLINK

////////////////////////////////////////////////////////

// GT2 with 15Tooth => 30mm

#define TOOTH 15
#define TOOTHSIZE 2

#define X_STEPSPERMM (3200.0/(TOOTH*TOOTHSIZE))
#define Y_STEPSPERMM (3200.0/(TOOTH*TOOTHSIZE))
#define Z_STEPSPERMM (3200.0/(TOOTH*TOOTHSIZE))
#define A_STEPSPERMM (3200.0/(TOOTH*TOOTHSIZE))

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

#define X_MAXSIZE 400000				// in mm1000_t
#define Y_MAXSIZE 380000 
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

//#define REFMOVE_1_AXIS	Z_AXIS
#define REFMOVE_2_AXIS	Y_AXIS
#define REFMOVE_3_AXIS	X_AXIS
//#define REFMOVE_3_AXIS	A_AXIS

#define MOVEAWAYFROMREF_STEPS 100

////////////////////////////////////////////////////////

#define CNC_MAXSPEED 27000        // steps/sec
#define CNC_ACC  350
#define CNC_DEC  400

////////////////////////////////////////////////////////

#define GO_DEFAULT_STEPRATE		CNC_MAXSPEED	// steps/sec
#define G1_DEFAULT_STEPRATE		10000	// steps/sec
#define G1_DEFAULT_MAXSTEPRATE	CNC_MAXSPEED	// steps/sec

#define STEPRATERATE_REFMOVE	4000

#define SETDIRECTION (1 << X_AXIS) + (1 << Y_AXIS)		// set bit to invert direction of each axis

////////////////////////////////////////////////////////

#undef NOGOTOREFERENCEATBOOT

////////////////////////////////////////////////////////

#include <MessageCNCLib.h>

