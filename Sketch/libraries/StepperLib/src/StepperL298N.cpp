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

#include "StepperL298N.h"

////////////////////////////////////////////////////////

static unsigned char L298Nhalfstep[8] = { 1, 3, 2, 6, 4, 12, 8, 9 };
static unsigned char L298Nfullstep[4] = { 1, 2, 4, 8 };

////////////////////////////////////////////////////////

CStepperL298N::CStepperL298N()
{
	InitMemVar();
}

////////////////////////////////////////////////////////

unsigned char CStepperL298N::_pin[NUM_AXIS][4] =
{
	{ 2, 3, 4, 5 },
	{ 6, 7, 8, 9 },
	{}
};

unsigned char CStepperL298N::_pinenable[NUM_AXIS][2] =
{
	{ 10, 11 },
	{ 12, 13 },
	{}
};

////////////////////////////////////////////////////////

void CStepperL298N::Init(void)
{
	register unsigned char i, n;

	for (i = 0; i < NUM_AXIS && _pin[i][0] != 0 ; i++)
	{
		for (n = 0; n < 4; n++)
		{
			CHAL::pinMode(_pin[i][n], OUTPUT);
		}

		CHAL::pinMode(_pinenable[i][0], OUTPUT);
		CHAL::pinMode(_pinenable[i][1], OUTPUT);
	}

	super::Init();
}
////////////////////////////////////////////////////////

void CStepperL298N::InitMemVar()
{
	register unsigned char i;
	for (i = 0; i < NUM_AXIS; i++)	_stepIdx[i] = 0;

	_idleLevel = 0;
}

////////////////////////////////////////////////////////

void  CStepperL298N::Step(const unsigned char steps[NUM_AXIS], unsigned char directionUp)
{
	for (axis_t axis=0;axis < NUM_AXIS;axis++)
	{
		if (IsBitSet(directionUp,axis))
			_stepIdx[axis] += steps[axis];
		else
			_stepIdx[axis] -= steps[axis];;
		SetPhase(axis);
	}
}

////////////////////////////////////////////////////////

void CStepperL298N::SetEnable(axis_t axis, unsigned char level)
{
	if (_pinenable[axis][0] != 0)
	{
		CHAL::digitalWrite(_pinenable[axis][0],level > 0 ? HIGH : LOW);
		CHAL::digitalWrite(_pinenable[axis][1],level > 0 ? HIGH : LOW);
	}
}

////////////////////////////////////////////////////////

unsigned char CStepperL298N::GetEnable(axis_t axis)
{
	if (_pinenable[axis][0] != 0)
		return CHAL::digitalRead(_pinenable[axis][0]) == LOW ? 0 : 100;
	return 0;
}

////////////////////////////////////////////////////////

void CStepperL298N::SetPhase(axis_t axis)
{
	if (_pin[axis][0] != 0)
	{
		register unsigned char bitmask;

		if (_stepMode[axis] == FullStep)
		{
			bitmask = L298Nfullstep[_stepIdx[axis] & 0x3];
		}
		else
		{
			bitmask = L298Nhalfstep[_stepIdx[axis] & 0x7];
		}

		CHAL::digitalWrite(_pin[axis][0], (bitmask & 1) ? HIGH : LOW);
		CHAL::digitalWrite(_pin[axis][1], (bitmask & 2) ? HIGH : LOW);
		CHAL::digitalWrite(_pin[axis][2], (bitmask & 4) ? HIGH : LOW);
		CHAL::digitalWrite(_pin[axis][3], (bitmask & 8) ? HIGH : LOW);
	}
}


