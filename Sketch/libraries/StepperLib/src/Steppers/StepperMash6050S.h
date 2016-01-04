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

#include "Stepper.h"

////////////////////////////////////////////////////////

#define MASH6050S_ENDSTOPCOUNT 4

////////////////////////////////////////////////////////

class CStepperMash6050S : public CStepper
{
private:
	typedef CStepper super;
public:

	////////////////////////////////////////////////////////

	CStepperMash6050S()
	{
		_num_axis = 4;
		_lastStepDirection = 0;
	}

	////////////////////////////////////////////////////////

	virtual void Init() override
	{
		super::Init();

		_pod._idleLevel = LevelMax;		// no Idle

		CHAL::pinMode(MASH6050S_X_STEP_PIN, OUTPUT);
		CHAL::pinMode(MASH6050S_X_DIR_PIN, OUTPUT);
		CHAL::pinMode(MASH6050S_X_MIN_PIN, MASH6050S_INPUTPINMODE);

		CHAL::pinMode(MASH6050S_Y_STEP_PIN, OUTPUT);
		CHAL::pinMode(MASH6050S_Y_DIR_PIN, OUTPUT);
		CHAL::pinMode(MASH6050S_Y_MIN_PIN, MASH6050S_INPUTPINMODE);

		CHAL::pinMode(MASH6050S_Z_STEP_PIN, OUTPUT);
		CHAL::pinMode(MASH6050S_Z_DIR_PIN, OUTPUT);
		CHAL::pinMode(MASH6050S_Z_MAX_PIN, MASH6050S_INPUTPINMODE);

		CHAL::pinMode(MASH6050S_C_STEP_PIN, OUTPUT);
		CHAL::pinMode(MASH6050S_C_DIR_PIN, OUTPUT);
		CHAL::pinMode(MASH6050S_C_MIN_PIN, MASH6050S_INPUTPINMODE);

#ifdef _MSC_VER
#pragma warning( disable : 4127 )
#endif

		HALFastdigitalWrite(MASH6050S_X_STEP_PIN, MASH6050S_PIN_STEP_ON);
		HALFastdigitalWrite(MASH6050S_Y_STEP_PIN, MASH6050S_PIN_STEP_ON);
		HALFastdigitalWrite(MASH6050S_Z_STEP_PIN, MASH6050S_PIN_STEP_ON);
		HALFastdigitalWrite(MASH6050S_C_STEP_PIN, MASH6050S_PIN_STEP_ON);

		HALFastdigitalWrite(MASH6050S_X_DIR_PIN, MASH6050S_PIN_DIR_OFF);
		HALFastdigitalWrite(MASH6050S_Y_DIR_PIN, MASH6050S_PIN_DIR_OFF);
		HALFastdigitalWrite(MASH6050S_Z_DIR_PIN, MASH6050S_PIN_DIR_OFF);
		HALFastdigitalWrite(MASH6050S_C_DIR_PIN, MASH6050S_PIN_DIR_OFF);

#ifdef _MSC_VER
#pragma warning( default : 4127 )
#endif
	}

protected:

	////////////////////////////////////////////////////////

	virtual void  SetEnable(axis_t , unsigned char , bool ) override
	{
	}

	////////////////////////////////////////////////////////

	virtual unsigned char GetEnable(axis_t ) override
	{
		return LevelMax;
	}

	////////////////////////////////////////////////////////

#if defined(__SAM3X8E__)

static void Delay1() ALWAYSINLINE { CHAL::delayMicroseconds(2); }
static void Delay2() ALWAYSINLINE { CHAL::delayMicroseconds(2); }

#else //AVR

static void Delay1() ALWAYSINLINE { CHAL::delayMicroseconds(1); }
static void Delay2() ALWAYSINLINE { CHAL::delayMicroseconds(1); }

#endif

	////////////////////////////////////////////////////////

	virtual void  Step(const unsigned char steps[NUM_AXIS], axisArray_t directionUp) override
	{
		// Step:   LOW to HIGH
		// PULS must be at least 1ms
		// DIRCHANGE must be at least 5ms

		if ((directionUp&(1 << X_AXIS)) != 0) HALFastdigitalWriteNC(MASH6050S_X_DIR_PIN, MASH6050S_PIN_DIR_OFF); else HALFastdigitalWriteNC(MASH6050S_X_DIR_PIN, MASH6050S_PIN_DIR_ON);
		if ((directionUp&(1 << Y_AXIS)) != 0) HALFastdigitalWriteNC(MASH6050S_Y_DIR_PIN, MASH6050S_PIN_DIR_OFF); else HALFastdigitalWriteNC(MASH6050S_Y_DIR_PIN, MASH6050S_PIN_DIR_ON);
		if ((directionUp&(1 << Z_AXIS)) != 0) HALFastdigitalWriteNC(MASH6050S_Z_DIR_PIN, MASH6050S_PIN_DIR_OFF); else HALFastdigitalWriteNC(MASH6050S_Z_DIR_PIN, MASH6050S_PIN_DIR_ON);
		if ((directionUp&(1 << A_AXIS)) != 0) HALFastdigitalWriteNC(MASH6050S_C_DIR_PIN, MASH6050S_PIN_DIR_OFF); else HALFastdigitalWriteNC(MASH6050S_C_DIR_PIN, MASH6050S_PIN_DIR_ON);

		if (_lastStepDirection != directionUp)
		{
			CHAL::delayMicroseconds(5);
			_lastStepDirection = directionUp;
		}

		for (unsigned char cnt = 0;; cnt++)
		{
			register bool have = false;
			if (steps[X_AXIS] > cnt) { HALFastdigitalWriteNC(MASH6050S_X_STEP_PIN, MASH6050S_PIN_STEP_OFF); have = true; }
			if (steps[Y_AXIS] > cnt) { HALFastdigitalWriteNC(MASH6050S_Y_STEP_PIN, MASH6050S_PIN_STEP_OFF); have = true; }
			if (steps[Z_AXIS] > cnt) { HALFastdigitalWriteNC(MASH6050S_Z_STEP_PIN, MASH6050S_PIN_STEP_OFF); have = true; }
			if (steps[A_AXIS] > cnt) { HALFastdigitalWriteNC(MASH6050S_C_STEP_PIN, MASH6050S_PIN_STEP_OFF); have = true; }

			Delay1();

			if (steps[X_AXIS] > cnt) { HALFastdigitalWriteNC(MASH6050S_X_STEP_PIN, MASH6050S_PIN_STEP_ON); }
			if (steps[Y_AXIS] > cnt) { HALFastdigitalWriteNC(MASH6050S_Y_STEP_PIN, MASH6050S_PIN_STEP_ON); }
			if (steps[Z_AXIS] > cnt) { HALFastdigitalWriteNC(MASH6050S_Z_STEP_PIN, MASH6050S_PIN_STEP_ON); }
			if (steps[A_AXIS] > cnt) { HALFastdigitalWriteNC(MASH6050S_C_STEP_PIN, MASH6050S_PIN_STEP_ON); }

			if (!have) break;

			Delay2();
		}
	}

public:

	////////////////////////////////////////////////////////

	virtual bool IsReference(unsigned char referenceid) override
	{
		switch (referenceid)
		{
			case 0: return HALFastdigitalRead(MASH6050S_X_MIN_PIN) == MASH6050S_REF_ON;
			case 2: return HALFastdigitalRead(MASH6050S_Y_MIN_PIN) == MASH6050S_REF_ON;
			case 5: return HALFastdigitalRead(MASH6050S_Z_MAX_PIN) == MASH6050S_REF_ON;
			case 6: return HALFastdigitalRead(MASH6050S_C_MIN_PIN) == MASH6050S_REF_ON;
		}
		return false;
	}

	////////////////////////////////////////////////////////

	virtual bool IsAnyReference() override
	{
		return
			(_pod._useReference[0] && HALFastdigitalRead(MASH6050S_X_MIN_PIN) == MASH6050S_REF_ON) ||
			(_pod._useReference[2] && HALFastdigitalRead(MASH6050S_Y_MIN_PIN) == MASH6050S_REF_ON) ||
			(_pod._useReference[5] && HALFastdigitalRead(MASH6050S_Z_MAX_PIN) == MASH6050S_REF_ON) ||
			(_pod._useReference[6] && HALFastdigitalRead(MASH6050S_C_MIN_PIN) == MASH6050S_REF_ON);
	}

	////////////////////////////////////////////////////////

private:

	axisArray_t		_lastStepDirection;				// for backlash
};
