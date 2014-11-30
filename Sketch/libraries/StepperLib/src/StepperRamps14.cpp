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

#include "StepperRamps14.h"

#if defined(__AVR_ATmega2560__) || defined(_MSC_VER) || defined(__SAM3X8E__)

////////////////////////////////////////////////////////

#include "StepperRamps14_Pins.h"
////////////////////////////////////////////////////////

// only available on Arduino Mega / due

////////////////////////////////////////////////////////

CStepperRamps14::CStepperRamps14()
{
	_num_axis = 5;
}

////////////////////////////////////////////////////////

void CStepperRamps14::Init()
{
	super::Init();

	CHAL::pinMode(RAMPS14_X_STEP_PIN, OUTPUT);
	CHAL::pinMode(RAMPS14_X_DIR_PIN, OUTPUT);
	CHAL::pinMode(RAMPS14_X_ENABLE_PIN, OUTPUT);
	CHAL::pinMode(RAMPS14_X_MIN_PIN, INPUT_PULLUP);
	CHAL::pinMode(RAMPS14_X_MAX_PIN, INPUT_PULLUP);

	CHAL::pinMode(RAMPS14_Y_STEP_PIN, OUTPUT);
	CHAL::pinMode(RAMPS14_Y_DIR_PIN, OUTPUT);
	CHAL::pinMode(RAMPS14_Y_ENABLE_PIN, OUTPUT);
	CHAL::pinMode(RAMPS14_Y_MIN_PIN, INPUT_PULLUP);
	CHAL::pinMode(RAMPS14_Y_MAX_PIN, INPUT_PULLUP);

	CHAL::pinMode(RAMPS14_Z_STEP_PIN, OUTPUT);
	CHAL::pinMode(RAMPS14_Z_DIR_PIN, OUTPUT);
	CHAL::pinMode(RAMPS14_Z_ENABLE_PIN, OUTPUT);
	CHAL::pinMode(RAMPS14_Z_MIN_PIN, INPUT_PULLUP);
	CHAL::pinMode(RAMPS14_Z_MAX_PIN, INPUT_PULLUP);

	CHAL::pinMode(RAMPS14_E0_STEP_PIN, OUTPUT);
	CHAL::pinMode(RAMPS14_E0_DIR_PIN, OUTPUT);
	CHAL::pinMode(RAMPS14_E0_ENABLE_PIN, OUTPUT);

	CHAL::pinMode(RAMPS14_E1_STEP_PIN, OUTPUT);
	CHAL::pinMode(RAMPS14_E1_DIR_PIN, OUTPUT);
	CHAL::pinMode(RAMPS14_E1_ENABLE_PIN, OUTPUT);

#pragma warning( disable : 4127 )

	HALFastdigitalWrite(RAMPS14_X_STEP_PIN, RAMPS14_PIN_STEP_ON);
	HALFastdigitalWrite(RAMPS14_Y_STEP_PIN, RAMPS14_PIN_STEP_ON);
	HALFastdigitalWrite(RAMPS14_Z_STEP_PIN, RAMPS14_PIN_STEP_ON);
	HALFastdigitalWrite(RAMPS14_E0_STEP_PIN, RAMPS14_PIN_STEP_ON);
	HALFastdigitalWrite(RAMPS14_E1_STEP_PIN, RAMPS14_PIN_STEP_ON);

#pragma warning( default : 4127 )
}

////////////////////////////////////////////////////////

void CStepperRamps14::Step(const unsigned char steps[NUM_AXIS], axisArray_t directionUp)
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

#define SETDIR(a,dirpin)		if ((directionUp&(1<<a)) != 0) HALFastdigitalWriteNC(dirpin,RAMPS14_PIN_DIR_OFF); else HALFastdigitalWriteNC(dirpin,RAMPS14_PIN_DIR_ON);

	SETDIR(X_AXIS, RAMPS14_X_DIR_PIN);
	SETDIR(Y_AXIS, RAMPS14_Y_DIR_PIN);
	SETDIR(Z_AXIS, RAMPS14_Z_DIR_PIN);
	SETDIR(E0_AXIS,RAMPS14_E0_DIR_PIN);
	SETDIR(E1_AXIS,RAMPS14_E1_DIR_PIN);

	for (unsigned char cnt=0;;cnt++)
	{
		register bool have=false;
		if (steps[X_AXIS] > cnt)  { HALFastdigitalWriteNC(RAMPS14_X_STEP_PIN, RAMPS14_PIN_STEP_OFF); have = true; }
		if (steps[Y_AXIS] > cnt)  { HALFastdigitalWriteNC(RAMPS14_Y_STEP_PIN, RAMPS14_PIN_STEP_OFF); have = true; }
		if (steps[Z_AXIS] > cnt)  { HALFastdigitalWriteNC(RAMPS14_Z_STEP_PIN, RAMPS14_PIN_STEP_OFF); have = true; }
		if (steps[E0_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPS14_E0_STEP_PIN, RAMPS14_PIN_STEP_OFF); have = true; }
		if (steps[E1_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPS14_E1_STEP_PIN, RAMPS14_PIN_STEP_OFF); have = true; }

		NOPREQUIRED_1();

		if (steps[X_AXIS] > cnt)  { HALFastdigitalWriteNC(RAMPS14_X_STEP_PIN, RAMPS14_PIN_STEP_ON); }
		if (steps[Y_AXIS] > cnt)  { HALFastdigitalWriteNC(RAMPS14_Y_STEP_PIN, RAMPS14_PIN_STEP_ON); }
		if (steps[Z_AXIS] > cnt)  { HALFastdigitalWriteNC(RAMPS14_Z_STEP_PIN, RAMPS14_PIN_STEP_ON); }
		if (steps[E0_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPS14_E0_STEP_PIN, RAMPS14_PIN_STEP_ON); }
		if (steps[E1_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPS14_E1_STEP_PIN, RAMPS14_PIN_STEP_ON); }

		if (!have) break;

		NOPREQUIRED_2();
	}

#undef SETDIR
}

////////////////////////////////////////////////////////

void CStepperRamps14::SetEnable(axis_t axis, unsigned char level, bool /* force */)
{
#define SETLEVEL(pin) if (level != LevelOff)	HALFastdigitalWrite(pin,RAMPS14_PIN_ENABLE_ON);	else	HALFastdigitalWrite(pin,RAMPS14_PIN_ENABLE_OFF);
	switch (axis)
	{
#pragma warning( disable : 4127 )
		case X_AXIS:  SETLEVEL(RAMPS14_X_ENABLE_PIN); break;
		case Y_AXIS:  SETLEVEL(RAMPS14_Y_ENABLE_PIN); break;
		case Z_AXIS:  SETLEVEL(RAMPS14_Z_ENABLE_PIN); break;
		case E0_AXIS: SETLEVEL(RAMPS14_E0_ENABLE_PIN); break;
		case E1_AXIS: SETLEVEL(RAMPS14_E1_ENABLE_PIN); break;
#pragma warning( default : 4127 )
	}
#undef SETLEVEL

}
////////////////////////////////////////////////////////

unsigned char CStepperRamps14::GetEnable(axis_t axis)
{
	switch (axis)
	{
#pragma warning( disable : 4127 )
		case X_AXIS:  return ConvertLevel(HALFastdigitalRead(RAMPS14_X_ENABLE_PIN) == RAMPS14_PIN_ENABLE_ON);
		case Y_AXIS:  return ConvertLevel(HALFastdigitalRead(RAMPS14_Y_ENABLE_PIN) == RAMPS14_PIN_ENABLE_ON);
		case Z_AXIS:  return ConvertLevel(HALFastdigitalRead(RAMPS14_Z_ENABLE_PIN) == RAMPS14_PIN_ENABLE_ON);
		case E0_AXIS: return ConvertLevel(HALFastdigitalRead(RAMPS14_E0_ENABLE_PIN) == RAMPS14_PIN_ENABLE_ON);
		case E1_AXIS: return ConvertLevel(HALFastdigitalRead(RAMPS14_E1_ENABLE_PIN) == RAMPS14_PIN_ENABLE_ON);
#pragma warning( default : 4127 )
	}
	return LevelOff;
}

////////////////////////////////////////////////////////

bool  CStepperRamps14::IsReference(unsigned char referenceid)
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

bool  CStepperRamps14::IsAnyReference()
{
	return	
		(_pod._useReference[0] && HALFastdigitalRead(RAMPS14_X_MIN_PIN) == RAMPS14_REF_ON) ||
		(_pod._useReference[1] && HALFastdigitalRead(RAMPS14_X_MAX_PIN) == RAMPS14_REF_ON) ||
		(_pod._useReference[2] && HALFastdigitalRead(RAMPS14_Y_MIN_PIN) == RAMPS14_REF_ON) ||
		(_pod._useReference[3] && HALFastdigitalRead(RAMPS14_Y_MAX_PIN) == RAMPS14_REF_ON) ||
		(_pod._useReference[4] && HALFastdigitalRead(RAMPS14_Z_MIN_PIN) == RAMPS14_REF_ON) ||
		(_pod._useReference[5] && HALFastdigitalRead(RAMPS14_Z_MAX_PIN) == RAMPS14_REF_ON);

}

////////////////////////////////////////////////////////

#endif