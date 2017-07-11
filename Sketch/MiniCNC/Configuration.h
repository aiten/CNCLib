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

#define HARDWARETYPE_CNC3325         1 // CNC-3325, microsteps 32, DRV8825, CNCShield V3.51, zero
#define HARDWARETYPE_CNCShieldTest  10 // All "Test" must be configured with eeprom
#define HARDWARETYPE_L298Test	      11 // 
#define HARDWARETYPE_SMCTest	      12 // 
#define HARDWARETYPE_TB6560Test     13 // 
#define HARDWARETYPE_CUSTOM	        99 // custom

#define HARDWARETYPE HARDWARETYPE_CNC3325

////////////////////////////////////////////////////////

#define GO_DEFAULT_STEPRATE      ((steprate_t) CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, maxsteprate)))  // steps/sec
#define G1_DEFAULT_MAXSTEPRATE  ((steprate_t) CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, maxsteprate)))  // steps/sec
#define G1_DEFAULT_FEEDPRATE    100000  // in mm1000 / min

////////////////////////////////////////////////////////

#if HARDWARETYPE==HARDWARETYPE_CNC3325
#include "ConfigurationMachine_CNC3325.h"
#elif HARDWARETYPE==HARDWARETYPE_CNCShieldTest
#include "ConfigurationMachine_CNCShieldTest.h"
#elif HARDWARETYPE==HARDWARETYPE_L298Test
#include "ConfigurationMachine_L298NTest.h"
#elif HARDWARETYPE==HARDWARETYPE_SMCTest
#include "ConfigurationMachine_SMC800Test.h"
#elif HARDWARETYPE==HARDWARETYPE_TB6560Test
#include "ConfigurationMachine_TB6560Test.h"
#elif HARDWARETYPE==HARDWARETYPE_CUSTOM
#include "ConfigurationMachine_Custom.h"
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

#define DISABLELEDBLINK

////////////////////////////////////////////////////////

