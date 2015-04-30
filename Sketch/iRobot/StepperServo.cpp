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

#include "StepperServo.h"
//#include "StepperServo_Pins.h"

////////////////////////////////////////////////////////

CStepperServo::CStepperServo()
{
}

////////////////////////////////////////////////////////

void CStepperServo::Init()
{	
	super::Init();

	_pod._idleLevel = LevelMax;		// no Idle

	_servo[0].attach(5);
	_servo[1].attach(6);
	_servo[2].attach(7);
	_servo[3].attach(8);
}

////////////////////////////////////////////////////////

void CStepperServo::Step(const unsigned char steps[NUM_AXIS], unsigned char directionUp)
{
	SetServo();
}

////////////////////////////////////////////////////////

void CStepperServo::SetServo()
{
	for (axis_t i = 0; i<NUM_AXIS; i++)
	{
		udist_t pos = MIN_PULSE_WIDTH+GetCurrentPosition(i);
		if (pos != _lastPos[i])
		{
			//_servo[i].writeMicroseconds(pos);
			_servo[i].write(pos);
			_lastPos[i] = pos;
		}
	} 
}

////////////////////////////////////////////////////////

void CStepperServo::SetEnable(axis_t axis, unsigned char level, bool /* force */)
{
}
////////////////////////////////////////////////////////

unsigned char CStepperServo::GetEnable(axis_t axis)
{
	return LevelMax;
}

////////////////////////////////////////////////////////

bool  CStepperServo::IsReference(unsigned char /* referenceid */)
{
	return false;
}

////////////////////////////////////////////////////////

bool  CStepperServo::IsAnyReference()
{
	return false;
}

////////////////////////////////////////////////////////
