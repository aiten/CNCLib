////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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

	_servo[0].attach(MG995_SERVO1_PIN);            // do not change, see Adafruit_TiCoServo for available pins
	_servo[1].attach(MG995_SERVO2_PIN);
	_servo[2].attach(MG995_SERVO3_PIN);
	_servo[3].attach(MG995_SERVO4_PIN);
}

////////////////////////////////////////////////////////

void CStepperServo::Step(const uint8_t /* steps */[NUM_AXIS], uint8_t /* directionUp */)
{
	SetServo();
}

////////////////////////////////////////////////////////

void CStepperServo::SetServo()
{
	for (axis_t i = 0; i<NUM_AXIS; i++)
	{
		udist_t pos;
		if (i==Y_AXIS)
		{
			pos = MAX_LIMIT+MIN_LIMIT-GetCurrentPosition(i);
		}
		else
		{
			pos = GetCurrentPosition(i);
		}
		if (pos != _lastPos[i])
		{
			_servo[i].write(pos);
			_lastPos[i] = pos;
		}
	} 
}

////////////////////////////////////////////////////////

void CStepperServo::SetEnable(axis_t /* axis */, uint8_t /* level */, bool /* force */)
{
}

////////////////////////////////////////////////////////

uint8_t CStepperServo::GetEnable(axis_t /* axis */)
{
	return LevelMax;
}

////////////////////////////////////////////////////////

uint8_t CStepperServo::GetReferenceValue(uint8_t /* referenceid */)
{
	return false;
}

////////////////////////////////////////////////////////

bool  CStepperServo::IsAnyReference()
{
	return false;
}

////////////////////////////////////////////////////////
