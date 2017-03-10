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

#include <Steppers/StepperRamps14_pins.h>
#include <Steppers/StepperRamps14.h>
#define BOARDNAME RAMPS14

////////////////////////////////////////////////////////

#define LCD_NUMAXIS  3
#define MYNUM_AXIS  3

////////////////////////////////////////////////////////

#if defined(__SAM3X8E__)

#define ROTARY_EN1           CAT(BOARDNAME,_LCD_ROTARY_EN2)
#define ROTARY_EN2           CAT(BOARDNAME,_LCD_ROTARY_EN1)
#define SD_ENABLE_PIN     52

#else

#define ROTARY_EN1       CAT(BOARDNAME,_LCD_ROTARY_EN1)
#define ROTARY_EN2       CAT(BOARDNAME,_LCD_ROTARY_EN2)
#define SD_ENABLE_PIN    CAT(BOARDNAME,_SDSS_PIN)

#endif

////////////////////////////////////////////////////////

#define LCD_GROW 64
#define LCD_GCOL 128

#define ROTARY_ENC        CAT(BOARDNAME,_LCD_ROTARY_ENC)
#define ROTARY_ENC_ON     CAT(BOARDNAME,_LCD_ROTARY_ENC_ON)

////////////////////////////////////////////////////////

#define CMyStepper CStepperRamps14

//#define EMERGENCY_ENDSTOP 5

#define FEEDRATE_REFMOVE  CStepper::GetInstance()->GetDefaultVmax() / 4  

////////////////////////////////////////////////////////

#define CONTROLLERFAN_ANALOGSPEED
#define CONTROLLERFAN_ONTIME	10000			// switch off controllerfan if idle for 10 Sec
#define CONTROLLERFAN_FAN_PIN	RAMPS14_FET2D9_PIN

#ifdef RAMPS14_KILL_PIN
#define KILL_PIN		RAMPS14_KILL_PIN
#define KILL_PIN_ON		LOW
#endif

#ifdef RAMPS14_HOLD_PIN
#define HOLD_PIN CNCSHIELD_HOLD_PIN
#endif

#ifdef RAMPS14_RESUME_PIN
#define RESUME_PIN CNCSHIELD_RESUME_PIN
#endif

////////////////////////////////////////////////////////










