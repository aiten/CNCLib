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

#ifndef CNCSHIELD_NUM_AXIS
#define CNCSHIELD_NUM_AXIS 3
#endif

////////////////////////////////////////////////////////

#undef  CNCLIB_USE_A4998
#define CNCLIB_USE_DRV8825

////////////////////////////////////////////////////////

#define CNCSHIELD_PIN_STEP_OFF		0
#define CNCSHIELD_PIN_STEP_ON		1

#define CNCSHIELD_PIN_DIR_OFF		0
#define CNCSHIELD_PIN_DIR_ON		1

// Enable: LOW Active
#define CNCSHIELD_PIN_ENABLE_OFF	1
#define CNCSHIELD_PIN_ENABLE_ON		0

////////////////////////////////////////////////////////

#define CNCSHIELD_REF_ON			0
#define CNCSHIELD_REF_OFF			1

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
#define CNCSHIELD_Z_MIN_PIN			11
#define CNCSHIELD_Z_MAX_PIN			11

#if CNCSHIELD_NUM_AXIS > 3

#define CNCSHIELD_A_STEP_PIN		12	
#define CNCSHIELD_A_DIR_PIN			13	

#else

#define CNCSHIELD_SPINDEL_DIR_PIN		13
#define CNCSHIELD_SPINDEL_ENABLE_PIN	12

#define CNCSHIELD_SPINDEL_DIGITAL_ON		LOW
#define CNCSHIELD_SPINDEL_DIGITAL_OFF		HIGH
#define CNCSHIELD_SPINDEL_ANALOG_MAX		255

#define CNCSHIELD_SPINDEL_DIR_CLW	LOW
#define CNCSHIELD_SPINDEL_DIR_CCLW	HIGH

#endif

#if defined(__AVR_ATmega328P__) || defined (_MSC_VER)  || defined(__SAMD21G18A__)

// 14=>AD0
#define CNCSHIELD_ABORT_PIN			14
#define CNCSHIELD_HOLD_PIN			15
#define CNCSHIELD_RESUME_PIN		16
#define CNCSHIELD_COOLANT_PIN		17

#elif defined(__AVR_ATmega2560__)

// 54=>AD0
#define CNCSHIELD_ABORT_PIN			54
#define CNCSHIELD_HOLD_PIN			55
#define CNCSHIELD_RESUME_PIN		56
#define CNCSHIELD_COOLANT_PIN		57

#endif

#define CNCSHIELD_ABORT_ON			LOW
#define CNCSHIELD_ABORT_OFF			HIGH

#define CNCSHIELD_HOLD_ON			LOW
#define CNCSHIELD_HOLD_OFF			HIGH

#define CNCSHIELD_RESUME_ON			LOW
#define CNCSHIELD_RESUME_OFF		HIGH

#define CNCSHIELD_COOLANT_ON		LOW
#define CNCSHIELD_COOLANT_OFF		HIGH

#if defined(__AVR_ATmega328P__) || defined (_MSC_VER) || defined(__SAMD21G18A__)
#define CNCSHIELD_A4_PIN     18    // AD4
#define CNCSHIELD_A5_PIN     19    // AD5
#else
#define CNCSHIELD_A4_PIN     58    // AD4 (Mega)
#define CNCSHIELD_A5_PIN     59    // AD5 (Mega)
#endif

#define CNCSHIELD_PROBE_PIN     CNCSHIELD_A4_PIN
#define CNCSHIELD_PROBE_ON      LOW
#define CNCSHIELD_PROBE_OFF     HIGH
