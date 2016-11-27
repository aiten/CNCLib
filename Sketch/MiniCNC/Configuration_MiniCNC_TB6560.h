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

#define MYNUM_AXIS 4

////////////////////////////////////////////////////////

#include <Steppers/StepperTB6560_pins.h>
#include <Steppers/StepperTB6560.h>

////////////////////////////////////////////////////////

#define CMyStepper CStepperTB6560
#define ConversionToMm1000 CMotionControl::ToMm1000_1_3200
#define ConversionToMachine CMotionControl::ToMachine_1_3200

////////////////////////////////////////////////////////

#define X_STEPSPERMM 3200.0
#define Y_STEPSPERMM 3200.0
#define Z_STEPSPERMM 3200.0
#define A_STEPSPERMM 3200.0

////////////////////////////////////////////////////////

#define X_MAXSIZE 200000        // in mm1000_t
#define Y_MAXSIZE 200000 
#define Z_MAXSIZE 100000 
#define A_MAXSIZE 50000 

////////////////////////////////////////////////////////

#define X_USEREFERENCE_MIN  
//#define X_USEREFERENCE_MAX

#define Y_USEREFERENCE_MIN  
//#define Y_USEREFERENCE_MAX

//#define Z_USEREFERENCE_MIN  
#define Z_USEREFERENCE_MAX

//#define A_USEREFERENCE_MIN  
//#define A_USEREFERENCE_MAX

#define REFMOVE_1_AXIS  Z_AXIS
#define REFMOVE_2_AXIS  Y_AXIS
#define REFMOVE_3_AXIS  X_AXIS
//#define REFMOVE_3_AXIS  A_AXIS

////////////////////////////////////////////////////////

#define GO_DEFAULT_STEPRATE    CNC_MAXSPEED  // steps/sec
#define G1_DEFAULT_STEPRATE   10000         // steps/sec
#define G1_DEFAULT_MAXSTEPRATE  CNC_MAXSPEED  // steps/sec

#define STEPRATERATE_REFMOVE  GO_DEFAULT_STEPRATE

//#define SETDIRECTION (1 << X_AXIS) + (1 << Y_AXIS)    // set bit to invert direction of each axis

////////////////////////////////////////////////////////

#define NOGOTOREFERENCEATBOOT

////////////////////////////////////////////////////////

#define CNC_MAXSPEED 14000
#define CNC_ACC  350
#define CNC_DEC  400

////////////////////////////////////////////////////////

#define CONTROLLERFAN_ONTIME	10000			// switch off controllerfan if idle for 10 Sec
#define CONTROLLERFAN_FAN_PIN	14 // AD0

#define CONTROLLERFAN_DIGITAL_ON  HIGH
#define CONTROLLERFAN_DIGITAL_OFF LOW

////////////////////////////////////////////////////////

#define SPINDEL_ENABLE_PIN	11

#define SPINDEL_DIGITAL_ON  LOW
#define SPINDEL_DIGITAL_OFF HIGH

////////////////////////////////////////////////////////

#define PROBE_PIN	12

#define PROBE_ON  LOW
#define PROBE_OFF HIGH

////////////////////////////////////////////////////////

#undef KILL_PIN

#define KILL_ON  LOW
#define KILL_OFF HIGH
