////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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
#define ConversionToMm1000 CMotionControlBase::ToMm1000_1_1
#define ConversionToMachine CMotionControlBase::ToMachine_1_1

////////////////////////////////////////////////////////

#define CNC_MAXSPEED 3000
#define CNC_ACC  150
#define CNC_DEC  180

////////////////////////////////////////////////////////

#define KILL_PIN	-1

#define KILL_ON  LOW
#define KILL_OFF HIGH

////////////////////////////////////////////////////////

#include <MessageCNCLib.h>

#define MESSAGE_MYCONTROL_iRobot_Starting					F("iRobotCNC:"__DATE__)

