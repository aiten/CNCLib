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
	for (register unsigned char i=0;i<3;i++) _rotateEnabled[i] = false;
	ClearOffset();
}

/////////////////////////////////////////////////////////

void CMotionControl::SetRotate(axis_t axis, double rad)
{
	_rotateEnabled[axis] = rad!=0.0;

	if (_rotateEnabled[axis])
	{
		_rotate[axis]._sin = sin(rad);
		_rotate[axis]._cos = cos(rad);
	}
}

/////////////////////////////////////////////////////////

void CMotionControl::TransformMachinePosition(const udist_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS])
{
	ToMm1000(src,dest);

	// todo:=> rotate and offset!
}

/////////////////////////////////////////////////////////

inline void CMotionControl::Rotate(const CMotionControl::SRotate&rotate, mm1000_t& x, mm1000_t& y, mm1000_t ofsx, mm1000_t ofsy)
{
	float fx = x-ofsx;
	float fy = y-ofsy;
	x = ((mm1000_t) fx*rotate._cos - fy*rotate._sin)+ofsx;
	y =	((mm1000_t) fx*rotate._sin + fy*rotate._cos)+ofsy;
}

/////////////////////////////////////////////////////////

void CMotionControl::TransformPosition(const mm1000_t src[NUM_AXIS], mm1000_t dest[NUM_AXIS])
{
	memcpy(dest, src, sizeof(_current));

	if (_rotateEnabled[Z_AXIS])
	{
		Rotate(_rotate[Z_AXIS],dest[X_AXIS],dest[Y_AXIS],_rotateOffset[X_AXIS],_rotateOffset[Y_AXIS]);
	}

	if (_rotateEnabled[Y_AXIS])
	{
		Rotate(_rotate[Y_AXIS],dest[Z_AXIS],dest[X_AXIS],_rotateOffset[Z_AXIS],_rotateOffset[X_AXIS]);
	}

	if (_rotateEnabled[X_AXIS])
	{
		Rotate(_rotate[X_AXIS],dest[Y_AXIS],dest[Z_AXIS],_rotateOffset[Y_AXIS],_rotateOffset[Z_AXIS]);
	}
}

/////////////////////////////////////////////////////////

