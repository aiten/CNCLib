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

#include "ConfigurationMachine_Default.h"

////////////////////////////////////////////////////////

#define STEPPERDIRECTION 0
//#define STEPPERDIRECTION (1 << X_AXIS) + (1 << Y_AXIS)    // set bit to invert direction of each axis

// 48 steps/rot

#define STEPSPERROTATION  48
#define MICROSTEPPING   1
#define SCREWLEAD     1.0

////////////////////////////////////////////////////////

#define CNC_MAXSPEED 375
#define CNC_ACC  61
#define CNC_DEC  65
#define CNC_JERKSPEED 10

////////////////////////////////////////////////////////

#include "ConfigurationStepper_L298N.h"

////////////////////////////////////////////////////////

#undef CONTROLLERFAN_PIN

////////////////////////////////////////////////////////
// PWM Spindel Pin

#define SPINDLE_ENABLE_PIN  11
#define SPINDLE_DIGITAL_ON  LOW
#define SPINDLE_DIGITAL_OFF HIGH
//#define SPINDLE_DIR_PIN  12
//#define SPINDLE_FADE

////////////////////////////////////////////////////////

#undef PROBE_PIN

////////////////////////////////////////////////////////

#undef KILL_PIN

////////////////////////////////////////////////////////

#define MESSAGE_MYCONTROL_Starting          F("L298N:" __DATE__ )

