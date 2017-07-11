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

#define USBBAUDRATE 250000

////////////////////////////////////////////////////////

#define MYNUM_AXIS 3

////////////////////////////////////////////////////////

#include "ConfigurationMachine_Default.h"

////////////////////////////////////////////////////////

#define STEPPERDIRECTION 0
//#define STEPPERDIRECTION (1 << X_AXIS) + (1 << Y_AXIS)    // set bit to invert direction of each axis

#define STEPSPERROTATION  200
#define MICROSTEPPING   16
#define SCREWLEAD     4.0

////////////////////////////////////////////////////////

#define CNC_MAXSPEED 20000        // steps/sec = 6.25 rot/sec
#define CNC_ACC  350              // 0.184 sec to full speed
#define CNC_DEC  400              // 0.141 sec to break
#define CNC_JERKSPEED 1000

////////////////////////////////////////////////////////

#include "ConfigurationStepper_CNCShield.h"

////////////////////////////////////////////////////////

#define MESSAGE_MYCONTROL_Starting          F("CNCShield:" __DATE__ )

