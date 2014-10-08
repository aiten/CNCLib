////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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

#define CMyStepper CStepperSMC800
#define ConversionToMm1000 CMotionControl::ToMm1000_1_400
#define ConversionToMachine CMotionControl::ToMachine_1_400

////////////////////////////////////////////////////////

#define CNC_MAXSPEED 3000
#define CNC_ACC  150
#define CNC_DEC  250

////////////////////////////////////////////////////////

#define CONTROLLERFAN_ONTIME	10000			// switch off controllerfan if idle for 10 Sec
#define CONTROLLERFAN_FAN_PIN	14 // 10

////////////////////////////////////////////////////////

#define SPINDEL_PIN	15

#define SPINDEL_ON  LOW
#define SPINDEL_OFF HIGH

////////////////////////////////////////////////////////

#define PROBE1_PIN	16

#define PROBE_ON  LOW
#define PROBE_OFF HIGH

////////////////////////////////////////////////////////
