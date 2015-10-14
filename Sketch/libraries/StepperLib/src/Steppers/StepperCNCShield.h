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
#include "StepperCNCShield_pins.h"

////////////////////////////////////////////////////////

#define CNCSHIELD_ENDSTOPCOUNT 3

#undef  USE_A4998
#define USE_DRV8825

////////////////////////////////////////////////////////

class CStepperCNCShield : public CStepper
{
private:
	typedef CStepper super;
public:

	CStepperCNCShield()
	{
		_num_axis = CNCSHIELD_NUM_AXIS;
	}

	////////////////////////////////////////////////////////

	virtual void Init() override
	{
		super::Init();

		CHAL::pinMode(CNCSHIELD_ENABLE_PIN, OUTPUT);

		CHAL::pinMode(CNCSHIELD_X_STEP_PIN, OUTPUT);
		CHAL::pinMode(CNCSHIELD_X_DIR_PIN, OUTPUT);
		CHAL::pinMode(CNCSHIELD_X_MIN_PIN, INPUT_PULLUP);

		CHAL::pinMode(CNCSHIELD_Y_STEP_PIN, OUTPUT);
		CHAL::pinMode(CNCSHIELD_Y_DIR_PIN, OUTPUT);
		CHAL::pinMode(CNCSHIELD_Y_MIN_PIN, INPUT_PULLUP);

		CHAL::pinMode(CNCSHIELD_Z_STEP_PIN, OUTPUT);
		CHAL::pinMode(CNCSHIELD_Z_DIR_PIN, OUTPUT);
		CHAL::pinMode(CNCSHIELD_Z_MAX_PIN, INPUT_PULLUP);

		HALFastdigitalWrite(CNCSHIELD_X_STEP_PIN, CNCSHIELD_PIN_STEP_ON);
		HALFastdigitalWrite(CNCSHIELD_Y_STEP_PIN, CNCSHIELD_PIN_STEP_ON);
		HALFastdigitalWrite(CNCSHIELD_Z_STEP_PIN, CNCSHIELD_PIN_STEP_ON);

#if CNCSHIELD_NUM_AXIS > 3

		CHAL::pinMode(CNCSHIELD_A_STEP_PIN, OUTPUT);
		CHAL::pinMode(CNCSHIELD_A_DIR_PIN, OUTPUT);

		HALFastdigitalWrite(CNCSHIELD_A_STEP_PIN, CNCSHIELD_PIN_STEP_ON);
#endif
	}

protected:

	////////////////////////////////////////////////////////

	virtual void  SetEnable(axis_t /* axis */, unsigned char level, bool /* force */) override
	{

#define SETLEVEL(pin) if (level != LevelOff)	HALFastdigitalWrite(pin,CNCSHIELD_PIN_ENABLE_ON);	else	HALFastdigitalWrite(pin,CNCSHIELD_PIN_ENABLE_OFF);

	SETLEVEL(CNCSHIELD_ENABLE_PIN);

#undef SETLEVEL

	}

	////////////////////////////////////////////////////////

	virtual unsigned char GetEnable(axis_t /* axis */) override
	{
		return ConvertLevel(HALFastdigitalRead(CNCSHIELD_ENABLE_PIN) == CNCSHIELD_PIN_ENABLE_ON);
	}

	////////////////////////////////////////////////////////

	virtual void  Step(const unsigned char steps[NUM_AXIS], axisArray_t directionUp) override
	{
		// The timing requirements for minimum pulse durations on the STEP pin are different for the two drivers. 
		// With the DRV8825, the high and low STEP pulses must each be at least 1.9 us; 
		// they can be as short as 1 us when using the A4988.

		// Step:   LOW to HIGH

#if defined(USE_A4998)

#define NOPREQUIRED_1()
#define NOPREQUIRED_2()

#elif defined(__SAM3X8E__)

#define NOPREQUIRED_1()	CHAL::delayMicroseconds(1);
#define NOPREQUIRED_2()	CHAL::delayMicroseconds(1);

#else //AVR

#define NOPREQUIRED_1()	CHAL::delayMicroseconds0312();
#define NOPREQUIRED_2()	CHAL::delayMicroseconds0500();

#endif

#define SETDIR(a,dirpin)		if ((directionUp&(1<<a)) != 0) HALFastdigitalWriteNC(dirpin,CNCSHIELD_PIN_DIR_OFF); else HALFastdigitalWriteNC(dirpin,CNCSHIELD_PIN_DIR_ON);

		SETDIR(X_AXIS, CNCSHIELD_X_DIR_PIN);
		SETDIR(Y_AXIS, CNCSHIELD_Y_DIR_PIN);
		SETDIR(Z_AXIS, CNCSHIELD_Z_DIR_PIN);
#if CNCSHIELD_NUM_AXIS > 3
		SETDIR(A_AXIS, CNCSHIELD_A_DIR_PIN);
#endif

		for (unsigned char cnt = 0;; cnt++)
		{
			register bool have = false;
			if (steps[X_AXIS] > cnt) { HALFastdigitalWriteNC(CNCSHIELD_X_STEP_PIN, CNCSHIELD_PIN_STEP_OFF); have = true; }
			if (steps[Y_AXIS] > cnt) { HALFastdigitalWriteNC(CNCSHIELD_Y_STEP_PIN, CNCSHIELD_PIN_STEP_OFF); have = true; }
			if (steps[Z_AXIS] > cnt) { HALFastdigitalWriteNC(CNCSHIELD_Z_STEP_PIN, CNCSHIELD_PIN_STEP_OFF); have = true; }
#if CNCSHIELD_NUM_AXIS > 3
			if (steps[A_AXIS] > cnt) { HALFastdigitalWriteNC(CNCSHIELD_A_STEP_PIN, CNCSHIELD_PIN_STEP_OFF); have = true; }
#endif

			NOPREQUIRED_1();

			if (steps[X_AXIS] > cnt) { HALFastdigitalWriteNC(CNCSHIELD_X_STEP_PIN, CNCSHIELD_PIN_STEP_ON); }
			if (steps[Y_AXIS] > cnt) { HALFastdigitalWriteNC(CNCSHIELD_Y_STEP_PIN, CNCSHIELD_PIN_STEP_ON); }
			if (steps[Z_AXIS] > cnt) { HALFastdigitalWriteNC(CNCSHIELD_Z_STEP_PIN, CNCSHIELD_PIN_STEP_ON); }
#if CNCSHIELD_NUM_AXIS > 3
			if (steps[A_AXIS] > cnt) { HALFastdigitalWriteNC(CNCSHIELD_A_STEP_PIN, CNCSHIELD_PIN_STEP_ON); }
#endif

			if (!have) break;

			NOPREQUIRED_2();
		}

#undef SETDIR
#undef NOPREQUIRED_1
#undef NOPREQUIRED_2
	}

public:

	////////////////////////////////////////////////////////

	virtual bool IsReference(unsigned char referenceid) override
	{
		// min and max is the same pin
		switch (referenceid)
		{
			case 0:
			case 1: return HALFastdigitalRead(CNCSHIELD_X_MIN_PIN) == CNCSHIELD_REF_ON;
			case 2:
			case 3: return HALFastdigitalRead(CNCSHIELD_Y_MIN_PIN) == CNCSHIELD_REF_ON;
			case 4:
			case 5: return HALFastdigitalRead(CNCSHIELD_Z_MIN_PIN) == CNCSHIELD_REF_ON;
		}
		return false;
	}

	////////////////////////////////////////////////////////

	virtual bool IsAnyReference() override
	{
		// min and max is the same pin
		return
			((_pod._useReference[0] || _pod._useReference[1]) && HALFastdigitalRead(CNCSHIELD_X_MIN_PIN) == CNCSHIELD_REF_ON) ||
			((_pod._useReference[2] || _pod._useReference[3]) && HALFastdigitalRead(CNCSHIELD_Y_MIN_PIN) == CNCSHIELD_REF_ON) ||
			((_pod._useReference[4] || _pod._useReference[5]) && HALFastdigitalRead(CNCSHIELD_Z_MIN_PIN) == CNCSHIELD_REF_ON);
	}

protected:

	////////////////////////////////////////////////////////

private:
};
