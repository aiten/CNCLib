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

#include <stdlib.h>
#include <string.h>

#include <arduino.h>
#include <ctype.h>

#include "CNCLib.h"
#include "MotionControl.h"

/////////////////////////////////////////////////////////

CMotionControl::CMotionControl()
{
	_sinXY = sin(0.523598776);
	_cosXY = cos(0.523598776);
}

/////////////////////////////////////////////////////////

void CMotionControl::TransformMachinePosition(const udist_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS])
{
	ToMm1000(src,dest);
}

/////////////////////////////////////////////////////////

void CMotionControl::TransformPosition(const mm1000_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS])
{
	memcpy(dest, src, sizeof(_current));
	dest[X_AXIS] = src[X_AXIS]*_cosXY - src[Y_AXIS]*_sinXY;
	dest[Y_AXIS] = src[X_AXIS]*_sinXY + src[Y_AXIS]*_cosXY;
}

/////////////////////////////////////////////////////////

