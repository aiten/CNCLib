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

#include <Steppers/StepperMash6050S_pins.h>
#include <Steppers/StepperMash6050S.h>
#define BOARDNAME MASH6050S 

////////////////////////////////////////////////////////

//#define CONTROLLERFAN_FAN_PIN	CAT(BOARDNAME,_FET2D9_PIN)
#define CONTROLLERFAN_ONTIME	10000			// switch off controllerfan if idle for 10 Sec

////////////////////////////////////////////////////////

//#define COOLANT_PIN	CAT(BOARDNAME,_AUX2_8)	// Ramps1.4 D42
#define COOLANT_PIN	42

#define COOLANT_ON  HIGH
#define COOLANT_OFF LOW

////////////////////////////////////////////////////////

//#define SPINDEL_PIN	CAT(BOARDNAME,_AUX2_6)	// Ramps1.4 D40
#define SPINDEL_ENABLE_PIN 40

#define SPINDEL_DIGITAL_ON  HIGH
#define SPINDEL_DIGITAL_OFF LOW

////////////////////////////////////////////////////////

#define PROBE_PIN	CAT(BOARDNAME,_C_MIN_PIN)	// Ref of C 
//#define PROBE1_PIN	CAT(BOARDNAME,_AUX2_7)	// Ramps 1.4 D44 
//#define PROBE2_PIN	CAT(BOARDNAME,_AUX2_5)	// Ramps 1.4 A10 

#define PROBE_ON  LOW
#define PROBE_OFF HIGH

#define KILL_PIN MASH6050S_KILL_PIN
#define KILL_PIN_ON MASH6050S_KILL_PIN_ON
#define KILL_PIN_OFF MASH6050S_KILL_PIN_OFF

////////////////////////////////////////////////////////

#define LCD_GROW 64
#define LCD_GCOL 128

#define LCD_NUMAXIS	3

#define ROTARY_ENC           CAT(BOARDNAME,_LCD_ROTARY_ENC)
#define ROTARY_ENC_ON		 CAT(BOARDNAME,_LCD_ROTARY_ENC_ON)

#define ROTARY_EN1           CAT(BOARDNAME,_LCD_ROTARY_EN1)
#define ROTARY_EN2           CAT(BOARDNAME,_LCD_ROTARY_EN2)
#define SD_ENABLE_PIN		 CAT(BOARDNAME,_SDSS_PIN)

////////////////////////////////////////////////////////
