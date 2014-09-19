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

#include "StepperRampsFD.h"

#if defined(__AVR_ATmega2560__) || defined(_MSC_VER) || defined(__SAM3X8E__)

////////////////////////////////////////////////////////

#include "StepperRampsFD_Pins.h"

////////////////////////////////////////////////////////

// only available on Arduino Mega / due

////////////////////////////////////////////////////////

CStepperRampsFD::CStepperRampsFD()
{
	InitMemVar();
}

////////////////////////////////////////////////////////

void CStepperRampsFD::InitMemVar()
{
}

////////////////////////////////////////////////////////

void CStepperRampsFD::Init()
{
	CHAL::pinMode(RAMPSFD_X_STEP_PIN, OUTPUT);
	CHAL::pinMode(RAMPSFD_X_DIR_PIN, OUTPUT);
	CHAL::pinMode(RAMPSFD_X_ENABLE_PIN, OUTPUT);
	CHAL::pinMode(RAMPSFD_X_MIN_PIN, INPUT_PULLUP);
	CHAL::pinMode(RAMPSFD_X_MAX_PIN, INPUT_PULLUP);

	CHAL::pinMode(RAMPSFD_Y_STEP_PIN, OUTPUT);
	CHAL::pinMode(RAMPSFD_Y_DIR_PIN, OUTPUT);
	CHAL::pinMode(RAMPSFD_Y_ENABLE_PIN, OUTPUT);
	CHAL::pinMode(RAMPSFD_Y_MIN_PIN, INPUT_PULLUP);
	CHAL::pinMode(RAMPSFD_Y_MAX_PIN, INPUT_PULLUP);

	CHAL::pinMode(RAMPSFD_Z_STEP_PIN, OUTPUT);
	CHAL::pinMode(RAMPSFD_Z_DIR_PIN, OUTPUT);
	CHAL::pinMode(RAMPSFD_Z_ENABLE_PIN, OUTPUT);
	CHAL::pinMode(RAMPSFD_Z_MIN_PIN, INPUT_PULLUP);
	CHAL::pinMode(RAMPSFD_Z_MAX_PIN, INPUT_PULLUP);

	CHAL::pinMode(RAMPSFD_E0_STEP_PIN, OUTPUT);
	CHAL::pinMode(RAMPSFD_E0_DIR_PIN, OUTPUT);
	CHAL::pinMode(RAMPSFD_E0_ENABLE_PIN, OUTPUT);
	//  CHAL::pinMode(RAMPSFD_E0_MIN_PIN,	INPUT_PULLUP);         
	//  CHAL::pinMode(RAMPSFD_E0_MAX_PIN,	INPUT_PULLUP);         

	CHAL::pinMode(RAMPSFD_E1_STEP_PIN, OUTPUT);
	CHAL::pinMode(RAMPSFD_E1_DIR_PIN, OUTPUT);
	CHAL::pinMode(RAMPSFD_E1_ENABLE_PIN, OUTPUT);
	//  CHAL::pinMode(RAMPSFD_E1_MIN_PIN,	INPUT_PULLUP);         
	//  CHAL::pinMode(RAMPSFD_E1_MAX_PIN,	INPUT_PULLUP);         

	CHAL::pinMode(RAMPSFD_E2_STEP_PIN, OUTPUT);
	CHAL::pinMode(RAMPSFD_E2_DIR_PIN, OUTPUT);
	CHAL::pinMode(RAMPSFD_E2_ENABLE_PIN, OUTPUT);
	//  CHAL::pinMode(E2_MIN_PIN,	INPUT_PULLUP);         
	//  CHAL::pinMode(E2_MAX_PIN,	INPUT_PULLUP);         

#pragma warning( disable : 4127 )

	HALFastdigitalWrite(RAMPSFD_X_STEP_PIN, RAMPSFD_PINON);
	HALFastdigitalWrite(RAMPSFD_Y_STEP_PIN, RAMPSFD_PINON);
	HALFastdigitalWrite(RAMPSFD_Z_STEP_PIN, RAMPSFD_PINON);
	HALFastdigitalWrite(RAMPSFD_E0_STEP_PIN, RAMPSFD_PINON);
	HALFastdigitalWrite(RAMPSFD_E1_STEP_PIN, RAMPSFD_PINON);
	HALFastdigitalWrite(RAMPSFD_E2_STEP_PIN, RAMPSFD_PINON);

#pragma warning( default : 4127 )

	// init some outputs!

	CHAL::pinMode(RAMPSFD_ESTOP_PIN, INPUT_PULLUP);

	CHAL::pinMode(RAMPSFD_FET5D12_PIN, OUTPUT); HALFastdigitalWrite(RAMPSFD_FET5D12_PIN,0);
	CHAL::pinMode(RAMPSFD_FET6D2_PIN,  OUTPUT); HALFastdigitalWrite(RAMPSFD_FET6D2_PIN,0);
	
	InitMemVar();
	super::Init();
}

////////////////////////////////////////////////////////

void CStepperRampsFD::Step(const unsigned char steps[NUM_AXIS], axisArray_t directionUp)
{
	// The timing requirements for minimum pulse durations on the STEP pin are different for the two drivers. 
	// With the DRV8825, the high and low STEP pulses must each be at least 1.9 us; 
	// they can be as short as 1 us when using the A4988.

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

#define SETDIR(a,dirpin)		if ((directionUp&(1<<a)) != 0) HALFastdigitalWriteNC(dirpin,RAMPSFD_PINOFF); else HALFastdigitalWriteNC(dirpin,RAMPSFD_PINON);

	SETDIR(X_AXIS, RAMPSFD_X_DIR_PIN);
	SETDIR(Y_AXIS, RAMPSFD_Y_DIR_PIN);
	SETDIR(Z_AXIS, RAMPSFD_Z_DIR_PIN);
	SETDIR(E0_AXIS, RAMPSFD_E0_DIR_PIN);
	SETDIR(E1_AXIS, RAMPSFD_E1_DIR_PIN);
	SETDIR(E2_AXIS, RAMPSFD_E2_DIR_PIN);

	for (unsigned char cnt = 0;; cnt++)
	{
		register bool have = false;
		if (steps[X_AXIS] > cnt)  { HALFastdigitalWriteNC(RAMPSFD_X_STEP_PIN,  RAMPSFD_PINOFF); have = true; }
		if (steps[Y_AXIS] > cnt)  { HALFastdigitalWriteNC(RAMPSFD_Y_STEP_PIN,  RAMPSFD_PINOFF); have = true; }
		if (steps[Z_AXIS] > cnt)  { HALFastdigitalWriteNC(RAMPSFD_Z_STEP_PIN,  RAMPSFD_PINOFF); have = true; }
		if (steps[E0_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPSFD_E0_STEP_PIN, RAMPSFD_PINOFF); have = true; }
		if (steps[E1_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPSFD_E1_STEP_PIN, RAMPSFD_PINOFF); have = true; }
		if (steps[E2_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPSFD_E2_STEP_PIN, RAMPSFD_PINOFF); have = true; }

		NOPREQUIRED_1();

		if (steps[X_AXIS] > cnt)  { HALFastdigitalWriteNC(RAMPSFD_X_STEP_PIN,  RAMPSFD_PINON); }
		if (steps[Y_AXIS] > cnt)  { HALFastdigitalWriteNC(RAMPSFD_Y_STEP_PIN,  RAMPSFD_PINON); }
		if (steps[Z_AXIS] > cnt)  { HALFastdigitalWriteNC(RAMPSFD_Z_STEP_PIN,  RAMPSFD_PINON); }
		if (steps[E0_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPSFD_E0_STEP_PIN, RAMPSFD_PINON); }
		if (steps[E1_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPSFD_E1_STEP_PIN, RAMPSFD_PINON); }
		if (steps[E2_AXIS] > cnt) { HALFastdigitalWriteNC(RAMPSFD_E2_STEP_PIN, RAMPSFD_PINON); }

		if (!have) break;

		NOPREQUIRED_2();
	}

#undef SETDIR
}

////////////////////////////////////////////////////////

void CStepperRampsFD::SetEnable(axis_t axis, unsigned char level, bool /* force */)
{

#define SETLEVEL(pin) if (level != LevelOff)	HALFastdigitalWrite(pin,RAMPSFD_PINOFF);	else	HALFastdigitalWrite(pin,RAMPSFD_PINON);
	switch (axis)
	{
#pragma warning( disable : 4127 )
		case X_AXIS:  SETLEVEL(RAMPSFD_X_ENABLE_PIN); break;
		case Y_AXIS:  SETLEVEL(RAMPSFD_Y_ENABLE_PIN); break;
		case Z_AXIS:  SETLEVEL(RAMPSFD_Z_ENABLE_PIN); break;
		case E0_AXIS: SETLEVEL(RAMPSFD_E0_ENABLE_PIN); break;
		case E1_AXIS: SETLEVEL(RAMPSFD_E1_ENABLE_PIN); break;
		case E2_AXIS: SETLEVEL(RAMPSFD_E2_ENABLE_PIN); break;
#pragma warning( default : 4127 )
	}
#undef SETLEVEL

}
////////////////////////////////////////////////////////

unsigned char CStepperRampsFD::GetEnable(axis_t axis)
{
	switch (axis)
	{
#pragma warning( disable : 4127 )
		case X_AXIS:  return ConvertLevel(HALFastdigitalRead(RAMPSFD_X_ENABLE_PIN) == RAMPSFD_PINOFF);
		case Y_AXIS:  return ConvertLevel(HALFastdigitalRead(RAMPSFD_Y_ENABLE_PIN) == RAMPSFD_PINOFF);
		case Z_AXIS:  return ConvertLevel(HALFastdigitalRead(RAMPSFD_Z_ENABLE_PIN) == RAMPSFD_PINOFF);
		case E0_AXIS: return ConvertLevel(HALFastdigitalRead(RAMPSFD_E0_ENABLE_PIN) == RAMPSFD_PINOFF);
		case E1_AXIS: return ConvertLevel(HALFastdigitalRead(RAMPSFD_E1_ENABLE_PIN) == RAMPSFD_PINOFF);
		case E2_AXIS: return ConvertLevel(HALFastdigitalRead(RAMPSFD_E2_ENABLE_PIN) == RAMPSFD_PINOFF);
#pragma warning( default : 4127 )
	}
	return 0;
}

////////////////////////////////////////////////////////

bool  CStepperRampsFD::IsReference(unsigned char referenceid)
{
	switch (referenceid)
	{
		case 0: return HALFastdigitalRead(RAMPSFD_X_MIN_PIN) == RAMPSFD_REF_ON;
		case 1: return HALFastdigitalRead(RAMPSFD_X_MAX_PIN) == RAMPSFD_REF_ON;
		case 2: return HALFastdigitalRead(RAMPSFD_Y_MIN_PIN) == RAMPSFD_REF_ON;
		case 3: return HALFastdigitalRead(RAMPSFD_Y_MAX_PIN) == RAMPSFD_REF_ON;
		case 4: return HALFastdigitalRead(RAMPSFD_Z_MIN_PIN) == RAMPSFD_REF_ON;
		case 5: return HALFastdigitalRead(RAMPSFD_Z_MAX_PIN) == RAMPSFD_REF_ON;
/* No reference for E0 & E1 & E2
		case 6: return HALFastdigitalRead(RAMPSFD_E0_MIN_PIN)==REF_ON;
		case 7: return HALFastdigitalRead(RAMPSFD_E0_MAX_PIN)==REF_ON;
		case 8: return HALFastdigitalRead(RAMPSFD_E1_MIN_PIN)==REF_ON;
		case 9: return HALFastdigitalRead(RAMPSFD_E1_MAX_PIN)==REF_ON;
		case 10:return HALFastdigitalRead(RAMPSFD_E2_MIN_PIN)==REF_ON;
		case 11:return HALFastdigitalRead(RAMPSFD_E2_MAX_PIN)==REF_ON;
		*/
	}
	return false;
}

////////////////////////////////////////////////////////

bool  CStepperRampsFD::IsAnyReference()
{
	return	
		(_useReference[0] && HALFastdigitalRead(RAMPSFD_X_MIN_PIN) == RAMPSFD_REF_ON) ||
		(_useReference[1] && HALFastdigitalRead(RAMPSFD_X_MAX_PIN) == RAMPSFD_REF_ON) ||
		(_useReference[2] && HALFastdigitalRead(RAMPSFD_Y_MIN_PIN) == RAMPSFD_REF_ON) ||
		(_useReference[3] && HALFastdigitalRead(RAMPSFD_Y_MAX_PIN) == RAMPSFD_REF_ON) ||
		(_useReference[4] && HALFastdigitalRead(RAMPSFD_Z_MIN_PIN) == RAMPSFD_REF_ON) ||
		(_useReference[5] && HALFastdigitalRead(RAMPSFD_Z_MAX_PIN) == RAMPSFD_REF_ON);
}

////////////////////////////////////////////////////////

#endif