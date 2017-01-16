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

#include <Steppers/StepperRamps14_pins.h>
#include <Steppers/StepperRamps14.h>
#define BOARDNAME RAMPS14

#define SPEEDFACTOR 1
#define SPEEDFACTOR_SQT 1

////////////////////////////////////////////////////////

#define LCD_NUMAXIS	5
#define MYNUM_AXIS	5

////////////////////////////////////////////////////////


#if defined(__SAM3X8E__)

#define ROTARY_EN1           CAT(BOARDNAME,_LCD_ROTARY_EN2)
#define ROTARY_EN2           CAT(BOARDNAME,_LCD_ROTARY_EN1)
#define SD_ENABLE_PIN	 	 52

#else

#define ROTARY_EN1           CAT(BOARDNAME,_LCD_ROTARY_EN1)
#define ROTARY_EN2           CAT(BOARDNAME,_LCD_ROTARY_EN2)
#define SD_ENABLE_PIN		 CAT(BOARDNAME,_SDSS_PIN)

#endif

////////////////////////////////////////////////////////
