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
//#include "StepperTB6560_Pins.h"

////////////////////////////////////////////////////////

class CStepperTB6560 : public CStepper
{
private:
	typedef CStepper super;
public:

	CStepperTB6560()
	{
		_num_axis = 4;
	}

	////////////////////////////////////////////////////////

	virtual void Init() override
	{
		super::Init();

		CHAL::pinModeOutput(TB6560_X_STEP_PIN);
		CHAL::pinModeOutput(TB6560_X_DIR_PIN);
		CHAL::pinModeOutput(TB6560_X_ENABLE_PIN);
		//	CHAL::pinModeInputPullUp(TB6560_X_MIN_PIN);
		//	CHAL::pinModeInputPullUp(TB6560_X_MAX_PIN);

		CHAL::pinModeOutput(TB6560_Y_STEP_PIN);
		CHAL::pinModeOutput(TB6560_Y_DIR_PIN);
		CHAL::pinModeOutput(TB6560_Y_ENABLE_PIN);
		//	CHAL::pinModeInputPullUp(TB6560_Y_MIN_PIN);
		//	CHAL::pinModeInputPullUp(TB6560_Y_MAX_PIN);

		CHAL::pinModeOutput(TB6560_Z_STEP_PIN);
		CHAL::pinModeOutput(TB6560_Z_DIR_PIN);
		CHAL::pinModeOutput(TB6560_Z_ENABLE_PIN);
		//	CHAL::pinModeInputPullUp(TB6560_Z_MIN_PIN);
		//	CHAL::pinModeInputPullUp(TB6560_Z_MAX_PIN);
		/*
		CHAL::pinModeOutput(TB6560_A_STEP_PIN);
		CHAL::pinModeOutput(TB6560_A_DIR_PIN);
		CHAL::pinModeOutput(TB6560_A_ENABLE_PIN);

		CHAL::pinModeOutput(TB6560_B_STEP_PIN);
		CHAL::pinModeOutput(TB6560_B_DIR_PIN);
		CHAL::pinModeOutput(TB6560_B_ENABLE_PIN);
		*/
#ifdef _MSC_VER
#pragma warning( disable : 4127 )
#endif

		HALFastdigitalWrite(TB6560_X_STEP_PIN, TB6560_PIN_STEP_ON);
		HALFastdigitalWrite(TB6560_Y_STEP_PIN, TB6560_PIN_STEP_ON);
		HALFastdigitalWrite(TB6560_Z_STEP_PIN, TB6560_PIN_STEP_ON);
		//	HALFastdigitalWrite(TB6560_A_STEP_PIN, TB6560_PIN_STEP_ON);
		//	HALFastdigitalWrite(TB6560_B_STEP_PIN, TB6560_PIN_STEP_ON);

#ifdef _MSC_VER
#pragma warning( default : 4127 )
#endif

	}

protected:

	////////////////////////////////////////////////////////

	virtual void  SetEnable(axis_t axis, uint8_t level, bool /* force */) override
	{

#define SETLEVEL(pin) if (level != LevelOff)	HALFastdigitalWrite(pin,TB6560_PIN_ENABLE_ON);	else	HALFastdigitalWrite(pin,TB6560_PIN_ENABLE_OFF);
		switch (axis)
		{
#ifdef _MSC_VER
#pragma warning( disable : 4127 )
#endif
			case X_AXIS:  SETLEVEL(TB6560_X_ENABLE_PIN); break;
			case Y_AXIS:  SETLEVEL(TB6560_Y_ENABLE_PIN); break;
			case Z_AXIS:  SETLEVEL(TB6560_Z_ENABLE_PIN); break;
				//		case A_AXIS: SETLEVEL(TB6560_A_ENABLE_PIN); break;
				//		case B_AXIS: SETLEVEL(TB6560_B_ENABLE_PIN); break;
#ifdef _MSC_VER
#pragma warning( default : 4127 )
#endif
		}
#undef SETLEVEL

	}

	////////////////////////////////////////////////////////

	virtual uint8_t GetEnable(axis_t axis) override
	{
		switch (axis)
		{
#ifdef _MSC_VER
#pragma warning( disable : 4127 )
#endif
			case X_AXIS:  return ConvertLevel(HALFastdigitalRead(TB6560_X_ENABLE_PIN) == TB6560_PIN_ENABLE_ON);
			case Y_AXIS:  return ConvertLevel(HALFastdigitalRead(TB6560_Y_ENABLE_PIN) == TB6560_PIN_ENABLE_ON);
			case Z_AXIS:  return ConvertLevel(HALFastdigitalRead(TB6560_Z_ENABLE_PIN) == TB6560_PIN_ENABLE_ON);
				//		case A_AXIS: return ConvertLevel(HALFastdigitalRead(TB6560_A_ENABLE_PIN) == TB6560_PIN_ENABLE_ON);
				//		case B_AXIS: return ConvertLevel(HALFastdigitalRead(TB6560_B_ENABLE_PIN) == TB6560_PIN_ENABLE_ON);
#ifdef _MSC_VER
#pragma warning( default : 4127 )
#endif
		}
		return 0;
	}

	////////////////////////////////////////////////////////

	virtual void  Step(const uint8_t steps[NUM_AXIS], uint8_t directionUp) override
	{
		// Step:   LOW to HIGH

#define SETDIR(a,dirpin)		if ((directionUp&(1<<a)) != 0) HALFastdigitalWriteNC(dirpin,TB6560_PIN_DIR_OFF); else HALFastdigitalWriteNC(dirpin,TB6560_PIN_DIR_ON);

		SETDIR(X_AXIS, TB6560_X_DIR_PIN);
		SETDIR(Y_AXIS, TB6560_Y_DIR_PIN);
		SETDIR(Z_AXIS, TB6560_Z_DIR_PIN);
		//	SETDIR(A_AXIS,TB6560_A_DIR_PIN);
		//	SETDIR(B_AXIS,TB6560_B_DIR_PIN);

		for (uint8_t cnt = 0;; cnt++)
		{
			register bool have = false;
			if (steps[X_AXIS] > cnt) { HALFastdigitalWriteNC(TB6560_X_STEP_PIN, TB6560_PIN_STEP_ON); have = true; }
			if (steps[Y_AXIS] > cnt) { HALFastdigitalWriteNC(TB6560_Y_STEP_PIN, TB6560_PIN_STEP_ON); have = true; }
			if (steps[Z_AXIS] > cnt) { HALFastdigitalWriteNC(TB6560_Z_STEP_PIN, TB6560_PIN_STEP_ON); have = true; }
			//		if (steps[A_AXIS] > cnt) { HALFastdigitalWriteNC(TB6560_A_STEP_PIN,TB6560_PIN_STEP_ON); have = true; }
			//		if (steps[B_AXIS] > cnt) { HALFastdigitalWriteNC(TB6560_B_STEP_PIN,TB6560_PIN_STEP_ON); have = true; }

			CHAL::delayMicroseconds(7);

			if (steps[X_AXIS] > cnt) { HALFastdigitalWriteNC(TB6560_X_STEP_PIN, TB6560_PIN_STEP_OFF); }
			if (steps[Y_AXIS] > cnt) { HALFastdigitalWriteNC(TB6560_Y_STEP_PIN, TB6560_PIN_STEP_OFF); }
			if (steps[Z_AXIS] > cnt) { HALFastdigitalWriteNC(TB6560_Z_STEP_PIN, TB6560_PIN_STEP_OFF); }
			//		if (steps[A_AXIS] > cnt) { HALFastdigitalWriteNC(TB6560_A_STEP_PIN,TB6560_PIN_STEP_OFF); }
			//		if (steps[B_AXIS] > cnt) { HALFastdigitalWriteNC(TB6560_B_STEP_PIN,TB6560_PIN_STEP_OFF); }

			if (!have) break;

			CHAL::delayMicroseconds(7);
		}

#undef SETDIR
	}

public:

	////////////////////////////////////////////////////////

	virtual bool IsReference(uint8_t /* referenceid */) override
	{
		/*
		switch (referenceid)
		{
		case 0: return HALFastdigitalRead(TB6560_X_MIN_PIN) == TB6560_REF_ON;
		case 1: return HALFastdigitalRead(TB6560_X_MAX_PIN) == TB6560_REF_ON;
		case 2: return HALFastdigitalRead(TB6560_Y_MIN_PIN) == TB6560_REF_ON;
		case 3: return HALFastdigitalRead(TB6560_Y_MAX_PIN) == TB6560_REF_ON;
		case 4: return HALFastdigitalRead(TB6560_Z_MIN_PIN) == TB6560_REF_ON;
		case 5: return HALFastdigitalRead(TB6560_Z_MAX_PIN) == TB6560_REF_ON;
		// No reference for A & B
		//		case 6: return HALFastdigitalRead(TB6560_A_MIN_PIN)==TB6560_REF_ON;
		//		case 7: return HALFastdigitalRead(TB6560_A_MAX_PIN)==TB6560_REF_ON;
		//		case 8: return HALFastdigitalRead(TB6560_B_MIN_PIN)==TB6560_REF_ON;
		//		case 9: return HALFastdigitalRead(TB6560_B_MAX_PIN)==TB6560_REF_ON;
		}
		*/
		return false;
	}

	////////////////////////////////////////////////////////

	virtual bool IsAnyReference() override
	{
		return false;
		/*
		return
		(_referenceHitValue[0] && HALFastdigitalRead(TB6560_X_MIN_PIN) == TB6560_REF_ON) ||
		(_referenceHitValue[1] && HALFastdigitalRead(TB6560_X_MAX_PIN) == TB6560_REF_ON) ||
		(_referenceHitValue[2] && HALFastdigitalRead(TB6560_Y_MIN_PIN) == TB6560_REF_ON) ||
		(_referenceHitValue[3] && HALFastdigitalRead(TB6560_Y_MAX_PIN) == TB6560_REF_ON) ||
		(_referenceHitValue[4] && HALFastdigitalRead(TB6560_Z_MIN_PIN) == TB6560_REF_ON) ||
		(_referenceHitValue[5] && HALFastdigitalRead(TB6560_Z_MAX_PIN) == TB6560_REF_ON);
		*/
	}


    protected:

	////////////////////////////////////////////////////////

private:

};
