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

#include "Stepper.h"

////////////////////////////////////////////////////////

#if defined(__AVR_ATmega2560__) || defined(_MSC_VER) || defined(__SAM3X8E__)

// only available on Arduino Mega or due

////////////////////////////////////////////////////////

#define E0_AXIS A_AXIS
#define E1_AXIS B_AXIS

#define RAMPS14_ENDSTOPCOUNT 6

////////////////////////////////////////////////////////

class CStepperRamps14 : public CStepper
{
private:
	typedef CStepper super;
public:

	////////////////////////////////////////////////////////

	CStepperRamps14()
	{
		_num_axis = 5;
	}

	////////////////////////////////////////////////////////

	virtual void Init() override
	{
		super::Init();

		CHAL::pinModeOutput(RAMPS14_X_STEP_PIN);
		CHAL::pinModeOutput(RAMPS14_X_DIR_PIN);
		CHAL::pinModeOutput(RAMPS14_X_ENABLE_PIN);
		CHAL::pinModeInputPullUp(RAMPS14_X_MIN_PIN);
		CHAL::pinModeInputPullUp(RAMPS14_X_MAX_PIN);

		CHAL::pinModeOutput(RAMPS14_Y_STEP_PIN);
		CHAL::pinModeOutput(RAMPS14_Y_DIR_PIN);
		CHAL::pinModeOutput(RAMPS14_Y_ENABLE_PIN);
		CHAL::pinModeInputPullUp(RAMPS14_Y_MIN_PIN);
		CHAL::pinModeInputPullUp(RAMPS14_Y_MAX_PIN);

		CHAL::pinModeOutput(RAMPS14_Z_STEP_PIN);
		CHAL::pinModeOutput(RAMPS14_Z_DIR_PIN);
		CHAL::pinModeOutput(RAMPS14_Z_ENABLE_PIN);
		CHAL::pinModeInputPullUp(RAMPS14_Z_MIN_PIN);
		CHAL::pinModeInputPullUp(RAMPS14_Z_MAX_PIN);

		CHAL::pinModeOutput(RAMPS14_E0_STEP_PIN);
		CHAL::pinModeOutput(RAMPS14_E0_DIR_PIN);
		CHAL::pinModeOutput(RAMPS14_E0_ENABLE_PIN);

		CHAL::pinModeOutput(RAMPS14_E1_STEP_PIN);
		CHAL::pinModeOutput(RAMPS14_E1_DIR_PIN);
		CHAL::pinModeOutput(RAMPS14_E1_ENABLE_PIN);

#ifdef _MSC_VER
#pragma warning( disable : 4127 )
#endif

		HALFastdigitalWrite(RAMPS14_X_STEP_PIN, RAMPS14_PIN_STEP_ON);
		HALFastdigitalWrite(RAMPS14_Y_STEP_PIN, RAMPS14_PIN_STEP_ON);
		HALFastdigitalWrite(RAMPS14_Z_STEP_PIN, RAMPS14_PIN_STEP_ON);
		HALFastdigitalWrite(RAMPS14_E0_STEP_PIN, RAMPS14_PIN_STEP_ON);
		HALFastdigitalWrite(RAMPS14_E1_STEP_PIN, RAMPS14_PIN_STEP_ON);

#ifdef _MSC_VER
#pragma warning( default : 4127 )
#endif
	}

	////////////////////////////////////////////////////////

protected:

	virtual void  SetEnable(axis_t axis, uint8_t level, bool /* force */) override
	{
		switch (axis)
		{
#ifdef _MSC_VER
#pragma warning( disable : 4127 )
#endif
			case X_AXIS:  if (level != LevelOff)	HALFastdigitalWrite(RAMPS14_X_ENABLE_PIN, RAMPS14_PIN_ENABLE_ON);	else	HALFastdigitalWrite(RAMPS14_X_ENABLE_PIN, RAMPS14_PIN_ENABLE_OFF); break;
			case Y_AXIS:  if (level != LevelOff)	HALFastdigitalWrite(RAMPS14_Y_ENABLE_PIN, RAMPS14_PIN_ENABLE_ON);	else	HALFastdigitalWrite(RAMPS14_Y_ENABLE_PIN, RAMPS14_PIN_ENABLE_OFF); break;
			case Z_AXIS:  if (level != LevelOff)	HALFastdigitalWrite(RAMPS14_Z_ENABLE_PIN, RAMPS14_PIN_ENABLE_ON);	else	HALFastdigitalWrite(RAMPS14_Z_ENABLE_PIN, RAMPS14_PIN_ENABLE_OFF); break;
			case E0_AXIS: if (level != LevelOff)	HALFastdigitalWrite(RAMPS14_E0_ENABLE_PIN, RAMPS14_PIN_ENABLE_ON);	else	HALFastdigitalWrite(RAMPS14_E0_ENABLE_PIN, RAMPS14_PIN_ENABLE_OFF); break;
			case E1_AXIS: if (level != LevelOff)	HALFastdigitalWrite(RAMPS14_E1_ENABLE_PIN, RAMPS14_PIN_ENABLE_ON);	else	HALFastdigitalWrite(RAMPS14_E1_ENABLE_PIN, RAMPS14_PIN_ENABLE_OFF); break;
#ifdef _MSC_VER
#pragma warning( default : 4127 )
#endif
		}
	}

	////////////////////////////////////////////////////////

	virtual uint8_t GetEnable(axis_t axis) override
	{
		switch (axis)
		{
#ifdef _MSC_VER
#pragma warning( disable : 4127 )
#endif
			case X_AXIS:  return ConvertLevel(HALFastdigitalRead(RAMPS14_X_ENABLE_PIN) == RAMPS14_PIN_ENABLE_ON);
			case Y_AXIS:  return ConvertLevel(HALFastdigitalRead(RAMPS14_Y_ENABLE_PIN) == RAMPS14_PIN_ENABLE_ON);
			case Z_AXIS:  return ConvertLevel(HALFastdigitalRead(RAMPS14_Z_ENABLE_PIN) == RAMPS14_PIN_ENABLE_ON);
			case E0_AXIS: return ConvertLevel(HALFastdigitalRead(RAMPS14_E0_ENABLE_PIN) == RAMPS14_PIN_ENABLE_ON);
			case E1_AXIS: return ConvertLevel(HALFastdigitalRead(RAMPS14_E1_ENABLE_PIN) == RAMPS14_PIN_ENABLE_ON);
#ifdef _MSC_VER
#pragma warning( default : 4127 )
#endif
		}
		return LevelOff;
	}

	////////////////////////////////////////////////////////

	#if defined(RAMPS14_USE_A4998)
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

		if ((directionUp&(1 << X_AXIS)) != 0)  HALFastdigitalWriteNC(RAMPS14_X_DIR_PIN,  RAMPS14_PIN_DIR_OFF); else HALFastdigitalWriteNC(RAMPS14_X_DIR_PIN,  RAMPS14_PIN_DIR_ON);
		if ((directionUp&(1 << Y_AXIS)) != 0)  HALFastdigitalWriteNC(RAMPS14_Y_DIR_PIN,  RAMPS14_PIN_DIR_OFF); else HALFastdigitalWriteNC(RAMPS14_Y_DIR_PIN,  RAMPS14_PIN_DIR_ON);
		if ((directionUp&(1 << Z_AXIS)) != 0)  HALFastdigitalWriteNC(RAMPS14_Z_DIR_PIN,  RAMPS14_PIN_DIR_OFF); else HALFastdigitalWriteNC(RAMPS14_Z_DIR_PIN,  RAMPS14_PIN_DIR_ON);
		if ((directionUp&(1 << E0_AXIS)) != 0) HALFastdigitalWriteNC(RAMPS14_E0_DIR_PIN, RAMPS14_PIN_DIR_OFF); else HALFastdigitalWriteNC(RAMPS14_E0_DIR_PIN, RAMPS14_PIN_DIR_ON);
		if ((directionUp&(1 << E1_AXIS)) != 0) HALFastdigitalWriteNC(RAMPS14_E1_DIR_PIN, RAMPS14_PIN_DIR_OFF); else HALFastdigitalWriteNC(RAMPS14_E1_DIR_PIN, RAMPS14_PIN_DIR_ON);

		for (uint8_t cnt = 0;; cnt++)
		{
			register bool have = false;
			if (steps[X_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPS14_X_STEP_PIN, RAMPS14_PIN_STEP_OFF); have = true; }
			if (steps[Y_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPS14_Y_STEP_PIN, RAMPS14_PIN_STEP_OFF); have = true; }
			if (steps[Z_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPS14_Z_STEP_PIN, RAMPS14_PIN_STEP_OFF); have = true; }
			if (steps[E0_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPS14_E0_STEP_PIN, RAMPS14_PIN_STEP_OFF); have = true; }
			if (steps[E1_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPS14_E1_STEP_PIN, RAMPS14_PIN_STEP_OFF); have = true; }

			Delay1();

			if (steps[X_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPS14_X_STEP_PIN, RAMPS14_PIN_STEP_ON); }
			if (steps[Y_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPS14_Y_STEP_PIN, RAMPS14_PIN_STEP_ON); }
			if (steps[Z_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPS14_Z_STEP_PIN, RAMPS14_PIN_STEP_ON); }
			if (steps[E0_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPS14_E0_STEP_PIN, RAMPS14_PIN_STEP_ON); }
			if (steps[E1_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPS14_E1_STEP_PIN, RAMPS14_PIN_STEP_ON); }

			if (!have) break;

			Delay2();
		}
	}

public:

	////////////////////////////////////////////////////////

	virtual bool IsReference(uint8_t referenceid) override
	{
		switch (referenceid)
		{
			case 0: return HALFastdigitalRead(RAMPS14_X_MIN_PIN) == RAMPS14_REF_ON;
			case 1: return HALFastdigitalRead(RAMPS14_X_MAX_PIN) == RAMPS14_REF_ON;
			case 2: return HALFastdigitalRead(RAMPS14_Y_MIN_PIN) == RAMPS14_REF_ON;
			case 3: return HALFastdigitalRead(RAMPS14_Y_MAX_PIN) == RAMPS14_REF_ON;
			case 4: return HALFastdigitalRead(RAMPS14_Z_MIN_PIN) == RAMPS14_REF_ON;
			case 5: return HALFastdigitalRead(RAMPS14_Z_MAX_PIN) == RAMPS14_REF_ON;
				/* No reference for E0 & E1
				case 6: return HALFastdigitalRead(RAMPS14_E0_MIN_PIN)==RAMPS14_REF_ON;
				case 7: return HALFastdigitalRead(RAMPS14_E0_MAX_PIN)==RAMPS14_REF_ON;
				case 8: return HALFastdigitalRead(RAMPS14_E1_MIN_PIN)==RAMPS14_REF_ON;
				case 9: return HALFastdigitalRead(RAMPS14_E1_MAX_PIN)==RAMPS14_REF_ON;
				*/
		}
		return false;
	}

	////////////////////////////////////////////////////////

	virtual bool IsAnyReference() override
	{
		return
			(_pod._useReference[0] && HALFastdigitalRead(RAMPS14_X_MIN_PIN) == RAMPS14_REF_ON) ||
			(_pod._useReference[1] && HALFastdigitalRead(RAMPS14_X_MAX_PIN) == RAMPS14_REF_ON) ||
			(_pod._useReference[2] && HALFastdigitalRead(RAMPS14_Y_MIN_PIN) == RAMPS14_REF_ON) ||
			(_pod._useReference[3] && HALFastdigitalRead(RAMPS14_Y_MAX_PIN) == RAMPS14_REF_ON) ||
			(_pod._useReference[4] && HALFastdigitalRead(RAMPS14_Z_MIN_PIN) == RAMPS14_REF_ON) ||
			(_pod._useReference[5] && HALFastdigitalRead(RAMPS14_Z_MAX_PIN) == RAMPS14_REF_ON);
	}
};

#endif