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

#define MOVEAWAYFROMREF_STEPS 100

////////////////////////////////////////////////////////

#define CNC_MAXSPEED 27000        // steps/sec
#define CNC_ACC  350
#define CNC_DEC  400

////////////////////////////////////////////////////////

#define CMyStepper CStepperCNCShield
#define ConversionToMm1000 LaserToMm1000
#define ConversionToMachine LaserToMachine

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

//#undef CNCSHIELD_SPINDEL_ENABLE_PIN
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

#include <MessageCNCLib.h>

