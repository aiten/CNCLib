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
	InitMemVar();
}

////////////////////////////////////////////////////////

void CStepperRamps14::InitMemVar()
{
}

////////////////////////////////////////////////////////

void CStepperRamps14::Init()
{
	pinMode(X_STEP_PIN, OUTPUT);
	pinMode(X_DIR_PIN, OUTPUT);
	pinMode(X_ENABLE_PIN, OUTPUT);
	pinMode(X_MIN_PIN, INPUT_PULLUP);
	pinMode(X_MAX_PIN, INPUT_PULLUP);

	pinMode(Y_STEP_PIN, OUTPUT);
	pinMode(Y_DIR_PIN, OUTPUT);
	pinMode(Y_ENABLE_PIN, OUTPUT);
	pinMode(Y_MIN_PIN, INPUT_PULLUP);
	pinMode(Y_MAX_PIN, INPUT_PULLUP);

	pinMode(Z_STEP_PIN, OUTPUT);
	pinMode(Z_DIR_PIN, OUTPUT);
	pinMode(Z_ENABLE_PIN, OUTPUT);
	pinMode(Z_MIN_PIN, INPUT_PULLUP);
	pinMode(Z_MAX_PIN, INPUT_PULLUP);

	pinMode(E0_STEP_PIN, OUTPUT);
	pinMode(E0_DIR_PIN, OUTPUT);
	pinMode(E0_ENABLE_PIN, OUTPUT);

	pinMode(E1_STEP_PIN, OUTPUT);
	pinMode(E1_DIR_PIN, OUTPUT);
	pinMode(E1_ENABLE_PIN, OUTPUT);

#pragma warning( disable : 4127 )

	WRITE(X_STEP_PIN, PINON);
	WRITE(Y_STEP_PIN, PINON);
	WRITE(Z_STEP_PIN, PINON);
	WRITE(E0_STEP_PIN, PINON);
	WRITE(E1_STEP_PIN, PINON);

#pragma warning( default : 4127 )

	InitMemVar();
	super::Init();
}

////////////////////////////////////////////////////////

void CStepperRamps14::Step(const unsigned char steps[NUM_AXIS], unsigned char directionUp)
{
// The timing requirements for minimum pulse durations on the STEP pin are different for the two drivers. 
// With the DRV8825, the high and low STEP pulses must each be at least 1.9 us; 
// they can be as short as 1 us when using the A4988.

// For shorter delays use assembly language call 'nop' (no operation). Each 'nop' statement executes in one machine cycle (at 16 MHz) yielding a 62.5 ns (nanosecond) delay. 

#define NOPREQUIRED

#if defined(__SAM3X8E__) || defined(USE_A4998)
#undef NOPREQUIRED
#endif

#define SETDIR(a,dirpin)		if ((directionUp&(1<<a)) != 0) _WRITE_NC(dirpin,PINOFF); else _WRITE_NC(dirpin,PINON);
#define STEPPINOFF(steppin)		_WRITE_NC(steppin, PINOFF);
#define STEPPINON(steppin)		_WRITE_NC(steppin, PINON);

	SETDIR(X_AXIS, X_DIR_PIN);
	SETDIR(Y_AXIS, Y_DIR_PIN);
	SETDIR(Z_AXIS, Z_DIR_PIN);
	SETDIR(E0_AXIS,E0_DIR_PIN);
	SETDIR(E1_AXIS,E1_DIR_PIN);

	for (unsigned char cnt=0;;cnt++)
	{
		register bool have=false;
		if (steps[X_AXIS] > cnt)  { STEPPINOFF(X_STEP_PIN); have = true; }
		if (steps[Y_AXIS] > cnt)  { STEPPINOFF(Y_STEP_PIN); have = true; }
		if (steps[Z_AXIS] > cnt)  { STEPPINOFF(Z_STEP_PIN); have = true; }
		if (steps[E0_AXIS] > cnt) { STEPPINOFF(E0_STEP_PIN); have = true; }
		if (steps[E1_AXIS] > cnt) { STEPPINOFF(E1_STEP_PIN); have = true; }

#if defined(NOPREQUIRED)
		__asm__("nop\n\tnop\n\tnop\n\t");
		__asm__("nop\n\tnop\n\t");

		if (steps[X_AXIS] > cnt)  { STEPPINON(X_STEP_PIN); }
		if (steps[Y_AXIS] > cnt)  { STEPPINON(Y_STEP_PIN);  }
		if (steps[Z_AXIS] > cnt)  { STEPPINON(Z_STEP_PIN);  }
		if (steps[E0_AXIS] > cnt) { STEPPINON(E0_STEP_PIN); }
		if (steps[E1_AXIS] > cnt) { STEPPINON(E1_STEP_PIN); }
#else
		STEPPINON(X_STEP_PIN);
		STEPPINON(Y_STEP_PIN);
		STEPPINON(Z_STEP_PIN);
		STEPPINON(E0_STEP_PIN);
		STEPPINON(E1_STEP_PIN);
#endif

		if (!have) break;

#if defined(NOPREQUIRED)
		__asm__("nop\n\tnop\n\tnop\n\tnop\n\tnop\n\t");
		__asm__("nop\n\tnop\n\tnop\n\t");
#endif
	}

#undef SETDIR
#undef STEPPINON
#undef STEPPINOFF

}

////////////////////////////////////////////////////////

void CStepperRamps14::SetEnable(axis_t axis, unsigned char level)
{

#define SETLEVEL(pin) if (level != 0)	WRITE(pin,PINOFF);	else	WRITE(pin,PINON);
	switch (axis)
	{
#pragma warning( disable : 4127 )
		case X_AXIS:  SETLEVEL(X_ENABLE_PIN); break;
		case Y_AXIS:  SETLEVEL(Y_ENABLE_PIN); break;
		case Z_AXIS:  SETLEVEL(Z_ENABLE_PIN); break;
		case E0_AXIS: SETLEVEL(E0_ENABLE_PIN); break;
		case E1_AXIS: SETLEVEL(E1_ENABLE_PIN); break;
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
		case X_AXIS:  return READ(X_ENABLE_PIN) == PINON ? 0 : 100;
		case Y_AXIS:  return READ(Y_ENABLE_PIN) == PINON ? 0 : 100;
		case Z_AXIS:  return READ(Z_ENABLE_PIN) == PINON ? 0 : 100;
		case E0_AXIS: return READ(E0_ENABLE_PIN) == PINON ? 0 : 100;
		case E1_AXIS: return READ(E1_ENABLE_PIN) == PINON ? 0 : 100;
#pragma warning( default : 4127 )
	}
	return 0;
}

////////////////////////////////////////////////////////

bool  CStepperRamps14::IsReference(unsigned char referenceid)
{
	switch (referenceid)
	{
		case 0: return READ(X_MIN_PIN) == REF_ON;
		case 1: return READ(X_MAX_PIN) == REF_ON;
		case 2: return READ(Y_MIN_PIN) == REF_ON;
		case 3: return READ(Y_MAX_PIN) == REF_ON;
		case 4: return READ(Z_MIN_PIN) == REF_ON;
		case 5: return READ(Z_MAX_PIN) == REF_ON;
/* No reference for E0 & E1
		case 6: return READ(E0_MIN_PIN)==REF_ON;
		case 7: return READ(E0_MAX_PIN)==REF_ON;
		case 8: return READ(E1_MIN_PIN)==REF_ON;
		case 9: return READ(E1_MAX_PIN)==REF_ON;
*/
	}
	return false;
}

////////////////////////////////////////////////////////

bool  CStepperRamps14::IsAnyReference()
{
	return	(_useReference[0] && READ(X_MIN_PIN) == REF_ON) ||
		(_useReference[1] && READ(X_MAX_PIN) == REF_ON) ||
		(_useReference[2] && READ(Y_MIN_PIN) == REF_ON) ||
		(_useReference[3] && READ(Y_MAX_PIN) == REF_ON) ||
		(_useReference[4] && READ(Z_MIN_PIN) == REF_ON) ||
		(_useReference[5] && READ(Z_MAX_PIN) == REF_ON);

}

////////////////////////////////////////////////////////

#endif