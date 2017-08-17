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

#define HARDWARETYPE_PROXXONMF70	  1	// proxxon mf70, ramps 1.4, microsteps 16
#define HARDWARETYPE_MYCNC	        2	// My-CNC, rampsfd, microsteps 32
#define HARDWARETYPE_KK1000S        4 // KK1000S, due 
#define HARDWARETYPE_CUSTOM			    99 // custom

#define HARDWARETYPE HARDWARETYPE_PROXXONMF70
//#define HARDWARETYPE HARDWARETYPE_MYCNC
//#define HARDWARETYPE HARDWARETYPE_KK1000S

////////////////////////////////////////////////////////

#if HARDWARETYPE==HARDWARETYPE_PROXXONMF70

#include "ConfigurationMachine_ProxxonMF70.h"

#elif HARDWARETYPE==HARDWARETYPE_MYCNC

#include "ConfigurationMachine_MyCNC.h"

#elif HARDWARETYPE==HARDWARETYPE_KK1000S

#include "ConfigurationMachine_KK1000S.h"

#else

#endif

////////////////////////////////////////////////////////

#define X_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define Y_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define Z_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define A_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define B_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)
#define C_STEPSPERMM ((STEPSPERROTATION*MICROSTEPPING)/SCREWLEAD)

////////////////////////////////////////////////////////
