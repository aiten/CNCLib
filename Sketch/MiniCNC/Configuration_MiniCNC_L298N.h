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

#include <Steppers/StepperL298N.h>

////////////////////////////////////////////////////////

#define CMyStepper CStepperL298N
#define ConversionToMm1000 ToMm1000_L298N
#define ConversionToMachine ToMachine_L298N

////////////////////////////////////////////////////////

#define MYNUM_AXIS  4

////////////////////////////////////////////////////////

#define X_STEPSPERMM 48.0
#define Y_STEPSPERMM 48.0
#define Z_STEPSPERMM 48.0
#define A_STEPSPERMM 48.0

// 48 steps/rot
inline mm1000_t ToMm1000_L298N(axis_t /* axis */, sdist_t val) { return  RoundMulDivU32(val, 125, 6); }
inline sdist_t  ToMachine_L298N(axis_t /* axis */, mm1000_t val) { return  RoundMulDivU32(val, 6, 125); }


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

#define NOGOTOREFERENCEATBOOT

////////////////////////////////////////////////////////

#define GO_DEFAULT_STEPRATE    CNC_MAXSPEED  // steps/sec
#define G1_DEFAULT_STEPRATE   10000         // steps/sec
#define G1_DEFAULT_MAXSTEPRATE  CNC_MAXSPEED  // steps/sec

#define STEPRATERATE_REFMOVE  GO_DEFAULT_STEPRATE

//#define SETDIRECTION (1 << X_AXIS) + (1 << Y_AXIS)    // set bit to invert direction of each axis

#define CNC_MAXSPEED 375
#define CNC_ACC  65
#define CNC_DEC  75

////////////////////////////////////////////////////////

#undef CONTROLLERFAN_PIN

////////////////////////////////////////////////////////

#undef SPINDEL_PIN

////////////////////////////////////////////////////////

#undef PROBE_PIN

////////////////////////////////////////////////////////

#undef KILL_PIN

