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

#pragma once

////////////////////////////////////////////////////////

#include "Stepper.h"

////////////////////////////////////////////////////////

#define CNCSHIELD_ENDSTOPCOUNT 3

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

		CHAL::pinModeOutput(CNCSHIELD_ENABLE_PIN);

		CHAL::pinModeOutput(CNCSHIELD_X_STEP_PIN);
		CHAL::pinModeOutput(CNCSHIELD_X_DIR_PIN);
		CHAL::pinModeInputPullUp(CNCSHIELD_X_MIN_PIN);

		CHAL::pinModeOutput(CNCSHIELD_Y_STEP_PIN);
		CHAL::pinModeOutput(CNCSHIELD_Y_DIR_PIN);
		CHAL::pinModeInputPullUp(CNCSHIELD_Y_MIN_PIN);

		CHAL::pinModeOutput(CNCSHIELD_Z_STEP_PIN);
		CHAL::pinModeOutput(CNCSHIELD_Z_DIR_PIN);
		CHAL::pinModeInputPullUp(CNCSHIELD_Z_MIN_PIN);

		HALFastdigitalWrite(CNCSHIELD_X_STEP_PIN, CNCSHIELD_PIN_STEP_ON);
		HALFastdigitalWrite(CNCSHIELD_Y_STEP_PIN, CNCSHIELD_PIN_STEP_ON);
		HALFastdigitalWrite(CNCSHIELD_Z_STEP_PIN, CNCSHIELD_PIN_STEP_ON);

#if CNCSHIELD_NUM_AXIS > 3

		CHAL::pinModeOutput(CNCSHIELD_A_STEP_PIN);
		CHAL::pinModeOutput(CNCSHIELD_A_DIR_PIN);

		HALFastdigitalWrite(CNCSHIELD_A_STEP_PIN, CNCSHIELD_PIN_STEP_ON);
#endif
	}

protected:

	////////////////////////////////////////////////////////

	virtual void  SetEnable(axis_t /* axis */, uint8_t level, bool /* force */) override
	{
		if (level != LevelOff)	HALFastdigitalWrite(CNCSHIELD_ENABLE_PIN,CNCSHIELD_PIN_ENABLE_ON);	else	HALFastdigitalWrite(CNCSHIELD_ENABLE_PIN,CNCSHIELD_PIN_ENABLE_OFF);
	}

	////////////////////////////////////////////////////////

	virtual uint8_t GetEnable(axis_t /* axis */) override
	{
		return ConvertLevel(HALFastdigitalRead(CNCSHIELD_ENABLE_PIN) == CNCSHIELD_PIN_ENABLE_ON);
	}

	////////////////////////////////////////////////////////

#if defined(CNCLIB_USE_A4998)
#define USE_A4998
#else
#undef USE_A4998
#endif
#include "StepperA4998_DRV8825.h"

	////////////////////////////////////////////////////////

	virtual void  Step(const uint8_t steps[NUM_AXIS], axisArray_t directionUp) override
	{
		// The timing requirements for minimum pulse durations on the STEP pin are different for the two drivers. 
		// With the DRV8825, the high and low STEP pulses must each be at least 1.9 us; 
		// they can be as short as 1 us when using the A4988.

		// Step:   LOW to HIGH

		if ((directionUp&(1 << X_AXIS)) != 0) HALFastdigitalWriteNC(CNCSHIELD_X_DIR_PIN, CNCSHIELD_PIN_DIR_OFF); else HALFastdigitalWriteNC(CNCSHIELD_X_DIR_PIN, CNCSHIELD_PIN_DIR_ON);
		if ((directionUp&(1 << Y_AXIS)) != 0) HALFastdigitalWriteNC(CNCSHIELD_Y_DIR_PIN, CNCSHIELD_PIN_DIR_OFF); else HALFastdigitalWriteNC(CNCSHIELD_Y_DIR_PIN, CNCSHIELD_PIN_DIR_ON);
		if ((directionUp&(1 << Z_AXIS)) != 0) HALFastdigitalWriteNC(CNCSHIELD_Z_DIR_PIN, CNCSHIELD_PIN_DIR_OFF); else HALFastdigitalWriteNC(CNCSHIELD_Z_DIR_PIN, CNCSHIELD_PIN_DIR_ON);

#if CNCSHIELD_NUM_AXIS > 3
		if ((directionUp&(1 << A_AXIS)) != 0) HALFastdigitalWriteNC(CNCSHIELD_A_DIR_PIN, CNCSHIELD_PIN_DIR_OFF); else HALFastdigitalWriteNC(CNCSHIELD_A_DIR_PIN, CNCSHIELD_PIN_DIR_ON);
#endif

		for (uint8_t cnt = 0;; cnt++)
		{
			register bool have = false;
			if (steps[X_AXIS] > cnt) { HALFastdigitalWriteNC(CNCSHIELD_X_STEP_PIN, CNCSHIELD_PIN_STEP_OFF); have = true; }
			if (steps[Y_AXIS] > cnt) { HALFastdigitalWriteNC(CNCSHIELD_Y_STEP_PIN, CNCSHIELD_PIN_STEP_OFF); have = true; }
			if (steps[Z_AXIS] > cnt) { HALFastdigitalWriteNC(CNCSHIELD_Z_STEP_PIN, CNCSHIELD_PIN_STEP_OFF); have = true; }
#if CNCSHIELD_NUM_AXIS > 3
			if (steps[A_AXIS] > cnt) { HALFastdigitalWriteNC(CNCSHIELD_A_STEP_PIN, CNCSHIELD_PIN_STEP_OFF); have = true; }
#endif

			Delay1(CNCSHIELD_NUM_AXIS);

			if (steps[X_AXIS] > cnt) { HALFastdigitalWriteNC(CNCSHIELD_X_STEP_PIN, CNCSHIELD_PIN_STEP_ON); }
			if (steps[Y_AXIS] > cnt) { HALFastdigitalWriteNC(CNCSHIELD_Y_STEP_PIN, CNCSHIELD_PIN_STEP_ON); }
			if (steps[Z_AXIS] > cnt) { HALFastdigitalWriteNC(CNCSHIELD_Z_STEP_PIN, CNCSHIELD_PIN_STEP_ON); }
#if CNCSHIELD_NUM_AXIS > 3
			if (steps[A_AXIS] > cnt) { HALFastdigitalWriteNC(CNCSHIELD_A_STEP_PIN, CNCSHIELD_PIN_STEP_ON); }
#endif

			if (!have) break;

			Delay2();
		}
	}

public:

	////////////////////////////////////////////////////////

	virtual bool IsReference(uint8_t referenceid) override
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
