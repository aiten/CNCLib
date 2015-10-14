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

#define CNCSHIELD_NUM_AXIS 4

////////////////////////////////////////////////////////

//m8
#define ConversionToMm1000 CMotionControl::ToMm1000_1d25_3200
#define ConversionToMachine CMotionControl::ToMachine_1d25_3200

//m6
//#define ConversionToMm1000 CMotionControl::ToMm1000_1_3200
//#define ConversionToMachine CMotionControl::ToMachine_1_3200

////////////////////////////////////////////////////////

#define MAXSIZE_X_AXIS 130000 
#define MAXSIZE_Y_AXIS 45000 
#define MAXSIZE_Z_AXIS 81000 
#define MAXSIZE_A_AXIS 360000 

////////////////////////////////////////////////////////

#undef ANALOGSPINDELSPEED
#define MAXSPINDLESPEED 25000		// analog 255

////////////////////////////////////////////////////////

#define CNC_MAXSPEED 14000
#define CNC_ACC  350
#define CNC_DEC  400

#if defined(__AVR_ATmega328P__) || defined (_MSC_VER)
#define CNCSHIELD_PROBE_PIN     18    // AD4 (Mega => 58)
#else
#define CNCSHIELD_PROBE_PIN     58    // AD4 (Mega => 58)
#endif

#define CNCSHIELD_PROBE_ON      LOW
#define CNCSHIELD_PROBE_OFF     HIGH

////////////////////////////////////////////////////////

#include <MessageCNCLib.h>

#define MESSAGE_MYCONTROL_CNCShield_Starting					F("CNCShield:"__DATE__)

