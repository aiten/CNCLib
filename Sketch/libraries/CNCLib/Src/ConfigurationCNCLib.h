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
*/
////////////////////////////////////////////////////////

#pragma once

////////////////////////////////////////////////////////

#include <StepperLib.h>
#include "MessageCNCLib.h"

#ifdef REDUCED_SIZE

#undef _USE_LCD

#else

#define _USE_LCD

#endif

////////////////////////////////////////////////////////

typedef float expr_t;			// type for expression parser

typedef long mm1000_t;			// 1/1000 mm
typedef long feedrate_t;		// mm_1000 / min

typedef long inch100000_t;		// 1/100000 inch

#define NUM_AXISXYZ			3			// 3dimensions

#define SCALE_FEEDRATE	3
#define SCALE_MM		3
#define SCALE_INCH		5

////////////////////////////////////////////////////////
//
// Control

#define SERIALBUFFERSIZE	128			// even size 

#define TIMEOUTCALLIDEL		333			// time in ms after move completet to call Idle
#define TIMEOUTCALLPOLL		500			// time in ms to call Poll() next if not idle => ASSERT( TIMEOUTCALLPOLL > TIMEOUTCALLIDEL)

#define IDLETIMER0VALUE     TIMER0VALUE(1000)		// AVR dont care ... Timer 0 shared with milli	

#define BLINK_LED			13
#define TIMEOUTBLINK		1000		// blink of led 13

////////////////////////////////////////////////////////
