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
#define CMyParser CGCode3DParser
#define CMyControlBase CControl3D

////////////////////////////////////////////////////////

#define BOARDNAME RAMPSFD
#define SD_ENABLE_PIN     CAT(BOARDNAME,_SDSS_PIN)

////////////////////////////////////////////////////////

#include <Steppers/StepperRampsFD_pins.h>

// change some pin definition here:

#undef RAMPSFD_REF_ON 
#undef RAMPSFD_REF_OFF

#define RAMPSFD_REF_ON  1
#define RAMPSFD_REF_OFF 0

#include <Steppers/StepperRampsFD.h>

////////////////////////////////////////////////////////

#define HOLDRESUME_PIN		CAT(BOARDNAME, _LCD_KILL_PIN)
#define HOLDRESUME_PIN_ON	CAT(BOARDNAME, _LCD_KILL_PIN_ON)

#define KILL_PIN		RAMPSFD_ESTOP_PIN
#define KILL_PIN_ON		LOW

#define LASER_PWM_PIN  RAMPSFD_SERVO1_PIN

#define LASER_ENABLE_PIN  RAMPSFD_SERVO2_PIN
#define LASER_ENABLE_PIN_ON  LOW
#define LASER_ENABLE_PIN_OFF HIGH

#define LASERWATER_PIN	RAMPSFD_SERVO3_PIN
#define LASERWATER_PIN_ON  LOW
#define LASERWATER_PIN_OFF HIGH
#define LASERWATER_ONTIME	120000 // 120000			// switch off if idle for 12000 => 2 min Sec
//#define LASERWATER_ONTIME  1000 // 1200000     // switch off if idle for 1200 => 20 min Sec

#define LASERVACUUM_PIN	RAMPSFD_SERVO4_PIN
#define LASERVACUUM_PIN_ON  LOW
#define LASERVACUUM_PIN_OFF HIGH
#define LASERVACUUM_ONTIME	60000			// switch off if idle for ?? Sec

////////////////////////////////////////////////////////

#define LCD_GROW 64
#define LCD_GCOL 128

#define ROTARY_ENC           CAT(BOARDNAME,_LCD_ROTARY_ENC)
#define ROTARY_ENC_ON    CAT(BOARDNAME,_LCD_ROTARY_ENC_ON)

#define ROTARY_EN1           CAT(BOARDNAME,_LCD_ROTARY_EN1)
#define ROTARY_EN2           CAT(BOARDNAME,_LCD_ROTARY_EN2)
#define SD_ENABLE_PIN    CAT(BOARDNAME,_SDSS_PIN)

////////////////////////////////////////////////////////


