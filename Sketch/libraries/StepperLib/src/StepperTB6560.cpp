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

#include "StepperTB6560.h"
#include "StepperTB6560_Pins.h"

////////////////////////////////////////////////////////

CStepperTB6560::CStepperTB6560()
{
	InitMemVar();
}

////////////////////////////////////////////////////////

void CStepperTB6560::InitMemVar()
{
}

////////////////////////////////////////////////////////

void CStepperTB6560::Init()
{
	CHAL::pinMode(TB6560_X_STEP_PIN, OUTPUT);
	CHAL::pinMode(TB6560_X_DIR_PIN, OUTPUT);
	CHAL::pinMode(TB6560_X_ENABLE_PIN, OUTPUT);
//	CHAL::pinMode(TB6560_X_MIN_PIN, INPUT_PULLUP);
//	CHAL::pinMode(TB6560_X_MAX_PIN, INPUT_PULLUP);

	CHAL::pinMode(TB6560_Y_STEP_PIN, OUTPUT);
	CHAL::pinMode(TB6560_Y_DIR_PIN, OUTPUT);
	CHAL::pinMode(TB6560_Y_ENABLE_PIN, OUTPUT);
//	CHAL::pinMode(TB6560_Y_MIN_PIN, INPUT_PULLUP);
//	CHAL::pinMode(TB6560_Y_MAX_PIN, INPUT_PULLUP);

	CHAL::pinMode(TB6560_Z_STEP_PIN, OUTPUT);
	CHAL::pinMode(TB6560_Z_DIR_PIN, OUTPUT);
	CHAL::pinMode(TB6560_Z_ENABLE_PIN, OUTPUT);
//	CHAL::pinMode(TB6560_Z_MIN_PIN, INPUT_PULLUP);
//	CHAL::pinMode(TB6560_Z_MAX_PIN, INPUT_PULLUP);
/*
	CHAL::pinMode(TB6560_A_STEP_PIN, OUTPUT);
	CHAL::pinMode(TB6560_A_DIR_PIN, OUTPUT);
	CHAL::pinMode(TB6560_A_ENABLE_PIN, OUTPUT);

	CHAL::pinMode(TB6560_B_STEP_PIN, OUTPUT);
	CHAL::pinMode(TB6560_B_DIR_PIN, OUTPUT);
	CHAL::pinMode(TB6560_B_ENABLE_PIN, OUTPUT);
*/
#pragma warning( disable : 4127 )

	HALFastdigitalWrite(TB6560_X_STEP_PIN, TB6560_PINON);
	HALFastdigitalWrite(TB6560_Y_STEP_PIN, TB6560_PINON);
	HALFastdigitalWrite(TB6560_Z_STEP_PIN, TB6560_PINON);
//	HALFastdigitalWrite(TB6560_A_STEP_PIN, TB6560_PINON);
//	HALFastdigitalWrite(TB6560_B_STEP_PIN, TB6560_PINON);

#pragma warning( default : 4127 )

	InitMemVar();
	super::Init();
}

////////////////////////////////////////////////////////

void CStepperTB6560::Step(const unsigned char steps[NUM_AXIS], unsigned char directionUp)
{
#define SETDIR(a,dirpin)		if ((directionUp&(1<<a)) != 0) HALFastdigitalWriteNC(dirpin,TB6560_PINOFF); else HALFastdigitalWriteNC(dirpin,TB6560_PINON);

	SETDIR(X_AXIS, TB6560_X_DIR_PIN);
	SETDIR(Y_AXIS, TB6560_Y_DIR_PIN);
	SETDIR(Z_AXIS, TB6560_Z_DIR_PIN);
//	SETDIR(A_AXIS,TB6560_A_DIR_PIN);
//	SETDIR(B_AXIS,TB6560_B_DIR_PIN);

	for (unsigned char cnt=0;;cnt++)
	{
		register bool have=false;
		if (steps[X_AXIS] > cnt)  { HALFastdigitalWriteNC(TB6560_X_STEP_PIN,TB6560_PINON); have = true; }
		if (steps[Y_AXIS] > cnt)  { HALFastdigitalWriteNC(TB6560_Y_STEP_PIN,TB6560_PINON); have = true; }
		if (steps[Z_AXIS] > cnt)  { HALFastdigitalWriteNC(TB6560_Z_STEP_PIN,TB6560_PINON); have = true; }
//		if (steps[A_AXIS] > cnt) { HALFastdigitalWriteNC(TB6560_A_STEP_PIN,TB6560_PINON); have = true; }
//		if (steps[B_AXIS] > cnt) { HALFastdigitalWriteNC(TB6560_B_STEP_PIN,TB6560_PINON); have = true; }

		CHAL::delayMicroseconds(7);

		if (steps[X_AXIS] > cnt)  { HALFastdigitalWriteNC(TB6560_X_STEP_PIN,TB6560_PINOFF); }
		if (steps[Y_AXIS] > cnt)  { HALFastdigitalWriteNC(TB6560_Y_STEP_PIN,TB6560_PINOFF);  }
		if (steps[Z_AXIS] > cnt)  { HALFastdigitalWriteNC(TB6560_Z_STEP_PIN,TB6560_PINOFF);  }
//		if (steps[A_AXIS] > cnt) { HALFastdigitalWriteNC(TB6560_A_STEP_PIN,TB6560_PINOFF); }
//		if (steps[B_AXIS] > cnt) { HALFastdigitalWriteNC(TB6560_B_STEP_PIN,TB6560_PINOFF); }

		if (!have) break;

		CHAL::delayMicroseconds(7);
	}

#undef SETDIR
}

////////////////////////////////////////////////////////

void CStepperTB6560::SetEnable(axis_t axis, unsigned char level)
{

#define SETLEVEL(pin) if (level != LevelOff)	HALFastdigitalWrite(pin,TB6560_PINOFF);	else	HALFastdigitalWrite(pin,TB6560_PINON);
	switch (axis)
	{
#pragma warning( disable : 4127 )
		case X_AXIS:  SETLEVEL(TB6560_X_ENABLE_PIN); break;
		case Y_AXIS:  SETLEVEL(TB6560_Y_ENABLE_PIN); break;
		case Z_AXIS:  SETLEVEL(TB6560_Z_ENABLE_PIN); break;
//		case A_AXIS: SETLEVEL(TB6560_A_ENABLE_PIN); break;
//		case B_AXIS: SETLEVEL(TB6560_B_ENABLE_PIN); break;
#pragma warning( default : 4127 )
	}
#undef SETLEVEL

}
////////////////////////////////////////////////////////

unsigned char CStepperTB6560::GetEnable(axis_t axis)
{
	switch (axis)
	{
#pragma warning( disable : 4127 )
		case X_AXIS:  return ConvertLevel(HALFastdigitalRead(TB6560_X_ENABLE_PIN) == TB6560_PINON);
		case Y_AXIS:  return ConvertLevel(HALFastdigitalRead(TB6560_Y_ENABLE_PIN) == TB6560_PINON);
		case Z_AXIS:  return ConvertLevel(HALFastdigitalRead(TB6560_Z_ENABLE_PIN) == TB6560_PINON);
//		case A_AXIS: return ConvertLevel(HALFastdigitalRead(TB6560_A_ENABLE_PIN) == TB6560_PINON);
//		case B_AXIS: return ConvertLevel(HALFastdigitalRead(TB6560_B_ENABLE_PIN) == TB6560_PINON);
#pragma warning( default : 4127 )
	}
	return 0;
}

////////////////////////////////////////////////////////

bool  CStepperTB6560::IsReference(unsigned char referenceid)
{
	switch (referenceid)
	{
/*
		case 0: return HALFastdigitalRead(TB6560_X_MIN_PIN) == TB6560_REF_ON;
		case 1: return HALFastdigitalRead(TB6560_X_MAX_PIN) == TB6560_REF_ON;
		case 2: return HALFastdigitalRead(TB6560_Y_MIN_PIN) == TB6560_REF_ON;
		case 3: return HALFastdigitalRead(TB6560_Y_MAX_PIN) == TB6560_REF_ON;
		case 4: return HALFastdigitalRead(TB6560_Z_MIN_PIN) == TB6560_REF_ON;
		case 5: return HALFastdigitalRead(TB6560_Z_MAX_PIN) == TB6560_REF_ON;
*/
/* No reference for A & B
		case 6: return HALFastdigitalRead(TB6560_A_MIN_PIN)==TB6560_REF_ON;
		case 7: return HALFastdigitalRead(TB6560_A_MAX_PIN)==TB6560_REF_ON;
		case 8: return HALFastdigitalRead(TB6560_B_MIN_PIN)==TB6560_REF_ON;
		case 9: return HALFastdigitalRead(TB6560_B_MAX_PIN)==TB6560_REF_ON;
*/

	}
	return false;
}

////////////////////////////////////////////////////////

bool  CStepperTB6560::IsAnyReference()
{
	return false;
/*
	return	
		(_useReference[0] && HALFastdigitalRead(TB6560_X_MIN_PIN) == TB6560_REF_ON) ||
		(_useReference[1] && HALFastdigitalRead(TB6560_X_MAX_PIN) == TB6560_REF_ON) ||
		(_useReference[2] && HALFastdigitalRead(TB6560_Y_MIN_PIN) == TB6560_REF_ON) ||
		(_useReference[3] && HALFastdigitalRead(TB6560_Y_MAX_PIN) == TB6560_REF_ON) ||
		(_useReference[4] && HALFastdigitalRead(TB6560_Z_MIN_PIN) == TB6560_REF_ON) ||
		(_useReference[5] && HALFastdigitalRead(TB6560_Z_MAX_PIN) == TB6560_REF_ON);
*/
}

////////////////////////////////////////////////////////
