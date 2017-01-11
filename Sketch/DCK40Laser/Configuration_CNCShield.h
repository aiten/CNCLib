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

#undef MYUSE_LCD

////////////////////////////////////////////////////////

#define CMyStepper CStepperCNCShield
#define CMyParser CGCodeParser
#define CMyControlBase CControl

////////////////////////////////////////////////////////

#define MYNUM_AXIS 3
#define CNCSHIELD_NUM_AXIS MYNUM_AXIS
//#define CNCSHIELD_GBRL09

#include <Steppers/StepperCNCShield_pins.h>

// change some pin definition here:

#undef CNCSHIELD_REF_ON
#undef CNCSHIELD_REF_OFF

#define CNCSHIELD_REF_ON      1
#define CNCSHIELD_REF_OFF     0

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

#if defined(__AVR_ATmega1280__) || defined(__AVR_ATmega2560__)
// 11 use timer 1 => 44 timer5 for pwm (https://oscarliang.com/arduino-timer-and-interrupt-tutorial/) 
#define LASER_PWM_PIN  44
#else
#define DISABLELEDBLINK
#define LASER_PWM_PIN  11
#endif

#define LASER_ENABLE_PIN  CNCSHIELD_SPINDEL_ENABLE_PIN
#define LASER_ENABLE_ON  LOW
#define LASER_ENABLE_OFF HIGH

#define LASERWATER_PIN	CNCSHIELD_A5_PIN
#define LASERWATER_ON  LOW
#define LASERWATER_OFF HIGH
#define LASERWATER_ONTIME	120000 // 120000			// switch off if idle for 12000 => 2 min Sec
//#define LASERWATER_ONTIME  1000 // 1200000     // switch off if idle for 1200 => 20 min Sec

#define LASERVACUUM_PIN	CNCSHIELD_A4_PIN
#define LASERVACUUM_ON  LOW
#define LASERVACUUM_OFF HIGH
#define LASERVACUUM__ONTIME	9000			// switch off if idle for ?? Sec

#define LASERWATCHDOG_PIN		CNCSHIELD_SPINDEL_DIR_PIN
#define LASERWATCHDOG_ON		LOW
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

