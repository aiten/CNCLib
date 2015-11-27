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
//#define ConversionToMm1000 CMotionControl::ToMm1000_1d25_3200
//#define ConversionToMachine CMotionControl::ToMachine_1d25_3200

//m6
//#define ConversionToMm1000 CMotionControl::ToMm1000_1_3200
//#define ConversionToMachine CMotionControl::ToMachine_1_3200

//float
#define STEPSPERMM 3200.0
inline mm1000_t ToMm1000_float(axis_t /* axis */, sdist_t val)               { return  (mm1000_t) (val * (1000.0/ STEPSPERMM)); }
inline sdist_t  ToMachine_float(axis_t /* axis */, mm1000_t val)             { return  (sdist_t) (val * (STEPSPERMM / 1000.0)); }
#define ConversionToMm1000 ToMm1000_float
#define ConversionToMachine ToMm1000_float

////////////////////////////////////////////////////////

#define MAXSIZE_X_AXIS 200000 
#define MAXSIZE_Y_AXIS 200000 
#define MAXSIZE_Z_AXIS 100000 
#define MAXSIZE_A_AXIS 50000 

////////////////////////////////////////////////////////

#undef ANALOGSPINDELSPEED
#define MAXSPINDLESPEED 25000		// analog 255

////////////////////////////////////////////////////////

//#define GOTOREFERENCEATBOOT

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

#define MESSAGE_MYCONTROL_CNCShield_Starting					F("CNCShield:" __DATE__ )


