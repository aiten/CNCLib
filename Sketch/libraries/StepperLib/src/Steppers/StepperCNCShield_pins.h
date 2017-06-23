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

//
// https://github.com/grbl/grbl/wiki/Connecting-Grbl
//

#ifndef CNCSHIELD_NUM_AXIS
#define CNCSHIELD_NUM_AXIS 3
#endif

////////////////////////////////////////////////////////

#undef  CNCLIB_USE_A4998
#define CNCLIB_USE_DRV8825

////////////////////////////////////////////////////////

#define CNCSHIELD_SPINDLE_ENABLE_PIN_GBRL09	12
#define CNCSHIELD_SPINDLE_ENABLE_PIN_GBRL11	11
#define CNCSHIELD_Z_MAX_PIN_GBRL09	11
#define CNCSHIELD_Z_MAX_PIN_GBRL11	12
#define CNCSHIELD_Z_MIN_PIN_GBRL09	CNCSHIELD_Z_MAX_PIN_GBRL09
#define CNCSHIELD_Z_MIN_PIN_GBRL11	CNCSHIELD_Z_MAX_PIN_GBRL11

////////////////////////////////////////////////////////

#define CNCSHIELD_PIN_STEP_OFF		0
#define CNCSHIELD_PIN_STEP_ON		1

#define CNCSHIELD_PIN_DIR_OFF		0
#define CNCSHIELD_PIN_DIR_ON		1

// Enable: LOW Active
#define CNCSHIELD_PIN_ENABLE_OFF	1
#define CNCSHIELD_PIN_ENABLE_ON		0

////////////////////////////////////////////////////////

#define CNCSHIELD_ENABLE_PIN		8

#define CNCSHIELD_X_STEP_PIN		2
#define CNCSHIELD_X_DIR_PIN			5
#define CNCSHIELD_X_MIN_PIN			9
#define CNCSHIELD_X_MAX_PIN			9

#define CNCSHIELD_Y_STEP_PIN		3
#define CNCSHIELD_Y_DIR_PIN			6
#define CNCSHIELD_Y_MIN_PIN			10
#define CNCSHIELD_Y_MAX_PIN			10

#define CNCSHIELD_Z_STEP_PIN		4
#define CNCSHIELD_Z_DIR_PIN			7

#if CNCSHIELD_NUM_AXIS > 3

#define CNCSHIELD_A_STEP_PIN		12	
#define CNCSHIELD_A_DIR_PIN			13	

#define CNCSHIELD_Z_MIN_PIN			11
#define CNCSHIELD_Z_MAX_PIN			11

#else

#ifdef CNCSHIELD_GBRL09

#define CNCSHIELD_SPINDLE_ENABLE_PIN	CNCSHIELD_SPINDLE_ENABLE_PIN_GBRL09
#define CNCSHIELD_Z_MIN_PIN			CNCSHIELD_Z_MAX_PIN_GBRL09
#define CNCSHIELD_Z_MAX_PIN			CNCSHIELD_Z_MAX_PIN_GBRL09

#else

#define CNCSHIELD_SPINDLE_ENABLE_PIN	CNCSHIELD_SPINDLE_ENABLE_PIN_GBRL11
#define CNCSHIELD_Z_MIN_PIN			CNCSHIELD_Z_MIN_PIN_GBRL11
#define CNCSHIELD_Z_MAX_PIN			CNCSHIELD_Z_MAX_PIN_GBRL11

#endif

#define CNCSHIELD_SPINDLE_DIR_PIN		13

#define CNCSHIELD_SPINDLE_DIGITAL_ON		LOW
#define CNCSHIELD_SPINDLE_DIGITAL_OFF		HIGH
#define CNCSHIELD_spindle_ANALOG_MAX		255

#define CNCSHIELD_SPINDLE_DIR_CLW	LOW
#define CNCSHIELD_SPINDLE_DIR_CCLW	HIGH

#endif

// 14=>AD0
#define CNCSHIELD_ABORT_PIN			PIN_A0
#define CNCSHIELD_HOLD_PIN			PIN_A1
#define CNCSHIELD_RESUME_PIN		PIN_A2
#define CNCSHIELD_COOLANT_PIN		PIN_A3


#define CNCSHIELD_ABORT_PIN_ON		LOW
#define CNCSHIELD_ABORT_PIN_OFF		HIGH

#define CNCSHIELD_HOLD_PIN_ON		LOW
#define CNCSHIELD_HOLD_PIN_OFF		HIGH

#define CNCSHIELD_RESUME_PIN_ON		LOW
#define CNCSHIELD_RESUME_PIN_OFF	HIGH

#define CNCSHIELD_COOLANT_PIN_ON	LOW
#define CNCSHIELD_COOLANT_PIN_OFF	HIGH

#define CNCSHIELD_A4_PIN			PIN_A4
#define CNCSHIELD_A5_PIN			PIN_A5

#define CNCSHIELD_PROBE_PIN			CNCSHIELD_A4_PIN
#define CNCSHIELD_PROBE_PIN_ON		LOW
#define CNCSHIELD_PROBE_PIN_OFF		HIGH
