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

 // 1010 -> 1000 -> 1001 -> 0001 -> 0101 -> 0100 -> 0110 -> 0010
static unsigned char _L298Nhalfstep4Pin[8] = { 10, 8, 9, 1, 5, 4, 6, 2 };

// 1010 -> 1001 -> 0101 -> 0110
 static unsigned char _L298Nfullstep4Pin[4] = { 10, 9, 5, 6 };
// static unsigned char _L298Nfullstep4Pin[4] = { 1+2, 2+4, 4+8, 8+1 };

 // 1010 -> 1001 -> 0101 -> 0110
 // aAbB => a => !a=A 
 static unsigned char _L298Nfullstep2Pin[4] = { 3, 2, 0, 1 };

////////////////////////////////////////////////////////

CStepperL298N::CStepperL298N()
{
	InitMemVar();
}

////////////////////////////////////////////////////////

pin_t CStepperL298N::_pin[NUM_AXIS][4] =
{
	{ 2, 3, 4, 5 },
	{ 6, 7, 8, 9 },
	{}
};

pin_t CStepperL298N::_pinenable[NUM_AXIS][2] =
{
	{ 0, 0 },		// 0 ... not used
	{ 0, 0 },
	{}
};

////////////////////////////////////////////////////////

void CStepperL298N::Init(void)
{
	register unsigned char i;

	for (i = 0; i < NUM_AXIS; i++)
	{
		if (IsActive(i))
		{
			CHAL::pinMode(_pin[i][0], OUTPUT);
			CHAL::pinMode(_pin[i][1], OUTPUT);
			if (Is4Pin(i))
			{
				CHAL::pinMode(_pin[i][2], OUTPUT);
				CHAL::pinMode(_pin[i][3], OUTPUT);
			}

			if (IsUseEN1(i))
			{
				CHAL::pinMode(_pinenable[i][0], OUTPUT);
				if (IsUseEN2(i)) CHAL::pinMode(_pinenable[i][1], OUTPUT);
			}
		}
	}

	super::Init();
}
////////////////////////////////////////////////////////

void CStepperL298N::InitMemVar()
{
	register unsigned char i;
	for (i = 0; i < NUM_AXIS; i++)	_stepIdx[i] = 0;
}

////////////////////////////////////////////////////////

void  CStepperL298N::Step(const unsigned char steps[NUM_AXIS], unsigned char directionUp)
{
	unsigned char mask=1;
	for (axis_t axis=0;axis < NUM_AXIS;axis++)
	{
		if (steps[axis])
		{
			if (directionUp&mask)
				_stepIdx[axis] += steps[axis];
			else
				_stepIdx[axis] -= steps[axis];
			SetPhase(axis);
		}
		mask *= 2;
	}
}

////////////////////////////////////////////////////////

void CStepperL298N::SetEnable(axis_t axis, unsigned char level)
{
	if (IsUseEN1(axis))
	{
		CHAL::digitalWrite(_pinenable[axis][0], level > LevelOff ? HIGH : LOW);
		if (IsUseEN2(axis)) CHAL::digitalWrite(_pinenable[axis][1], level > LevelOff ? HIGH : LOW);
	}
	else
	{
		// 4 PIN => set all to off

		CHAL::digitalWrite(_pin[axis][0], LOW);
		CHAL::digitalWrite(_pin[axis][1], LOW);
		CHAL::digitalWrite(_pin[axis][2], LOW);
		CHAL::digitalWrite(_pin[axis][3], LOW);
	}
}

////////////////////////////////////////////////////////

unsigned char CStepperL298N::GetEnable(axis_t axis)
{
	if (IsUseEN1(axis))
		return ConvertLevel(CHAL::digitalRead(_pinenable[axis][0]) == LOW);

	// no enable PIN => with 4 PIN test if one PIN is set

	if (Is2Pin(axis))	return LevelMax;		// 2PIN and no enable => can't be turned off

	return ConvertLevel(
		CHAL::digitalRead(_pin[axis][0]) == LOW &&
		CHAL::digitalRead(_pin[axis][1]) == LOW &&
		CHAL::digitalRead(_pin[axis][2]) == LOW &&
		CHAL::digitalRead(_pin[axis][3]) == LOW);
}

////////////////////////////////////////////////////////

void CStepperL298N::SetPhase(axis_t axis)
{
	if (IsActive(axis))
	{
		register unsigned char bitmask;

		if (Is4Pin(axis))
		{
			if (_stepMode[axis] == FullStep)
			{
				bitmask = _L298Nfullstep4Pin[_stepIdx[axis] & 0x3];
			}
			else
			{
				bitmask = _L298Nhalfstep4Pin[_stepIdx[axis] & 0x7];
			}
		}
		else
		{
			// 2 pin, only full step
			bitmask = _L298Nfullstep2Pin[_stepIdx[axis] & 0x3];
		}

		CHAL::digitalWrite(_pin[axis][0], bitmask & 1);
		CHAL::digitalWrite(_pin[axis][1], bitmask & 2);
		CHAL::digitalWrite(_pin[axis][2], bitmask & 4);
		CHAL::digitalWrite(_pin[axis][3], bitmask & 8);
	}
}


