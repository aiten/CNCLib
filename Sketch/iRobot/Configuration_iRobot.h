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
  http://www.gnu.org/licenses/
*/
////////////////////////////////////////////////////////

#pragma once

////////////////////////////////////////////////////////

#define CMyStepper CStepperServo
#define ConversionToMm1000 CMotionControlBase::ToMm1000_1_1000
#define ConversionToMachine CMotionControlBase::ToMachine_1_1000

////////////////////////////////////////////////////////

#define CNC_MAXSPEED 15000
#define CNC_ACC  200
#define CNC_DEC  250

////////////////////////////////////////////////////////

#define LCD_KILL_PIN  41

#define KILL_ON  LOW
#define KILL_OFF HIGH

////////////////////////////////////////////////////////

#define LCD_GROW 64
#define LCD_GCOL 128

#define LCD_BEEPER 37

#define LCD_NUMAXIS	6

#define ROTARY_ENC           35
#define ROTARY_ENC_ON		 LOW

#define ROTARY_EN1           31
#define ROTARY_EN2           33

#define SD_ENABLE_PIN		 53

////////////////////////////////////////////////////////

#include <MessageCNCLib.h>

#define MESSAGE_MYCONTROL_iRobot_Starting					F("iRobotCNC:" __DATE__ )

