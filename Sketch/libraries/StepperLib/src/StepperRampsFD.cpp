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
	pinMode(RAMPSFD_X_STEP_PIN, OUTPUT);
	pinMode(RAMPSFD_X_DIR_PIN, OUTPUT);
	pinMode(RAMPSFD_X_ENABLE_PIN, OUTPUT);
	pinMode(RAMPSFD_X_MIN_PIN, INPUT_PULLUP);
	pinMode(RAMPSFD_X_MAX_PIN, INPUT_PULLUP);

	pinMode(RAMPSFD_Y_STEP_PIN, OUTPUT);
	pinMode(RAMPSFD_Y_DIR_PIN, OUTPUT);
	pinMode(RAMPSFD_Y_ENABLE_PIN, OUTPUT);
	pinMode(RAMPSFD_Y_MIN_PIN, INPUT_PULLUP);
	pinMode(RAMPSFD_Y_MAX_PIN, INPUT_PULLUP);

	pinMode(RAMPSFD_Z_STEP_PIN, OUTPUT);
	pinMode(RAMPSFD_Z_DIR_PIN, OUTPUT);
	pinMode(RAMPSFD_Z_ENABLE_PIN, OUTPUT);
	pinMode(RAMPSFD_Z_MIN_PIN, INPUT_PULLUP);
	pinMode(RAMPSFD_Z_MAX_PIN, INPUT_PULLUP);

	pinMode(RAMPSFD_E0_STEP_PIN, OUTPUT);
	pinMode(RAMPSFD_E0_DIR_PIN, OUTPUT);
	pinMode(RAMPSFD_E0_ENABLE_PIN, OUTPUT);
	//  pinMode(RAMPSFD_E0_MIN_PIN,	INPUT_PULLUP);         
	//  pinMode(RAMPSFD_E0_MAX_PIN,	INPUT_PULLUP);         

	pinMode(RAMPSFD_E1_STEP_PIN, OUTPUT);
	pinMode(RAMPSFD_E1_DIR_PIN, OUTPUT);
	pinMode(RAMPSFD_E1_ENABLE_PIN, OUTPUT);
	//  pinMode(RAMPSFD_E1_MIN_PIN,	INPUT_PULLUP);         
	//  pinMode(RAMPSFD_E1_MAX_PIN,	INPUT_PULLUP);         

	pinMode(RAMPSFD_E2_STEP_PIN, OUTPUT);
	pinMode(RAMPSFD_E2_DIR_PIN, OUTPUT);
	pinMode(RAMPSFD_E2_ENABLE_PIN, OUTPUT);
	//  pinMode(E2_MIN_PIN,	INPUT_PULLUP);         
	//  pinMode(E2_MAX_PIN,	INPUT_PULLUP);         

#pragma warning( disable : 4127 )

	WRITE(RAMPSFD_X_STEP_PIN, RAMPSFD_PINON);
	WRITE(RAMPSFD_Y_STEP_PIN, RAMPSFD_PINON);
	WRITE(RAMPSFD_Z_STEP_PIN, RAMPSFD_PINON);
	WRITE(RAMPSFD_E0_STEP_PIN, RAMPSFD_PINON);
	WRITE(RAMPSFD_E1_STEP_PIN, RAMPSFD_PINON);
	WRITE(RAMPSFD_E2_STEP_PIN, RAMPSFD_PINON);

#pragma warning( default : 4127 )

	// init some outputs!
	pinMode(RAMPSFD_FAN_PIN, OUTPUT);
	pinMode(RAMPSFD_HEATER_0_PIN, OUTPUT);
	pinMode(RAMPSFD_HEATER_1_PIN, OUTPUT);

	pinMode(RAMPSFD_ESTOP, INPUT_PULLUP);

	InitMemVar();
	super::Init();
}

////////////////////////////////////////////////////////

void CStepperRampsFD::Step(const unsigned char steps[NUM_AXIS], unsigned char directionUp)
{
	// The timing requirements for minimum pulse durations on the STEP pin are different for the two drivers. 
	// With the DRV8825, the high and low STEP pulses must each be at least 1.9 us; 
	// they can be as short as 1 us when using the A4988.

	// For shorter delays use assembly language call 'nop' (no operation). Each 'nop' statement executes in one machine cycle (at 16 MHz) yielding a 62.5 ns (nanosecond) delay. 

#define NOPREQUIRED

#if defined(__SAM3X8E__) || defined(USE_A4998)
#undef NOPREQUIRED
#endif

#define SETDIR(a,dirpin)		if ((directionUp&(1<<a)) != 0) _WRITE_NC(dirpin,RAMPSFD_PINOFF); else _WRITE_NC(dirpin,RAMPSFD_PINON);
#define STEPPINOFF(steppin)		_WRITE_NC(steppin, RAMPSFD_PINOFF);
#define STEPPINON(steppin)		_WRITE_NC(steppin, RAMPSFD_PINON);

	SETDIR(X_AXIS, RAMPSFD_X_DIR_PIN);
	SETDIR(Y_AXIS, RAMPSFD_Y_DIR_PIN);
	SETDIR(Z_AXIS, RAMPSFD_Z_DIR_PIN);
	SETDIR(E0_AXIS, RAMPSFD_E0_DIR_PIN);
	SETDIR(E1_AXIS, RAMPSFD_E1_DIR_PIN);
	SETDIR(E2_AXIS, RAMPSFD_E2_DIR_PIN);

	for (unsigned char cnt = 0;; cnt++)
	{
		register bool have = false;
		if (steps[X_AXIS] > cnt)  { STEPPINOFF(RAMPSFD_X_STEP_PIN); have = true; }
		if (steps[Y_AXIS] > cnt)  { STEPPINOFF(RAMPSFD_Y_STEP_PIN); have = true; }
		if (steps[Z_AXIS] > cnt)  { STEPPINOFF(RAMPSFD_Z_STEP_PIN); have = true; }
		if (steps[E0_AXIS] > cnt) { STEPPINOFF(RAMPSFD_E0_STEP_PIN); have = true; }
		if (steps[E1_AXIS] > cnt) { STEPPINOFF(RAMPSFD_E1_STEP_PIN); have = true; }
		if (steps[E2_AXIS] > cnt) { STEPPINOFF(RAMPSFD_E2_STEP_PIN); have = true; }

#if defined(NOPREQUIRED)
		__asm__("nop\n\tnop\n\tnop\n\t");
		__asm__("nop\n\tnop\n\t");

		if (steps[X_AXIS] > cnt)  { STEPPINON(RAMPSFD_X_STEP_PIN); }
		if (steps[Y_AXIS] > cnt)  { STEPPINON(RAMPSFD_Y_STEP_PIN); }
		if (steps[Z_AXIS] > cnt)  { STEPPINON(RAMPSFD_Z_STEP_PIN); }
		if (steps[E0_AXIS] > cnt) { STEPPINON(RAMPSFD_E0_STEP_PIN); }
		if (steps[E1_AXIS] > cnt) { STEPPINON(RAMPSFD_E1_STEP_PIN); }
		if (steps[E2_AXIS] > cnt) { STEPPINON(RAMPSFD_E2_STEP_PIN); }
#else
		STEPPINON(RAMPSFD_X_STEP_PIN);
		STEPPINON(RAMPSFD_Y_STEP_PIN);
		STEPPINON(RAMPSFD_Z_STEP_PIN);
		STEPPINON(RAMPSFD_E0_STEP_PIN);
		STEPPINON(RAMPSFD_E1_STEP_PIN);
		STEPPINON(RAMPSFD_E2_STEP_PIN);
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

void CStepperRampsFD::SetEnable(axis_t axis, unsigned char level)
{

#define SETLEVEL(pin) if (level != 0)	WRITE(pin,RAMPSFD_PINOFF);	else	WRITE(pin,RAMPSFD_PINON);
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
		case X_AXIS:  return READ(RAMPSFD_X_ENABLE_PIN) == RAMPSFD_PINON ? 0 : 100;
		case Y_AXIS:  return READ(RAMPSFD_Y_ENABLE_PIN) == RAMPSFD_PINON ? 0 : 100;
		case Z_AXIS:  return READ(RAMPSFD_Z_ENABLE_PIN) == RAMPSFD_PINON ? 0 : 100;
		case E0_AXIS: return READ(RAMPSFD_E0_ENABLE_PIN) == RAMPSFD_PINON ? 0 : 100;
		case E1_AXIS: return READ(RAMPSFD_E1_ENABLE_PIN) == RAMPSFD_PINON ? 0 : 100;
		case E2_AXIS: return READ(RAMPSFD_E2_ENABLE_PIN) == RAMPSFD_PINON ? 0 : 100;
#pragma warning( default : 4127 )
	}
	return 0;
}

////////////////////////////////////////////////////////

bool  CStepperRampsFD::IsReference(unsigned char referenceid)
{
	switch (referenceid)
	{
		case 0: return READ(RAMPSFD_X_MIN_PIN) == RAMPSFD_REF_ON;
		case 1: return READ(RAMPSFD_X_MAX_PIN) == RAMPSFD_REF_ON;
		case 2: return READ(RAMPSFD_Y_MIN_PIN) == RAMPSFD_REF_ON;
		case 3: return READ(RAMPSFD_Y_MAX_PIN) == RAMPSFD_REF_ON;
		case 4: return READ(RAMPSFD_Z_MIN_PIN) == RAMPSFD_REF_ON;
		case 5: return READ(RAMPSFD_Z_MAX_PIN) == RAMPSFD_REF_ON;
/* No reference for E0 & E1 & E2
		case 6: return READ(RAMPSFD_E0_MIN_PIN)==REF_ON;
		case 7: return READ(RAMPSFD_E0_MAX_PIN)==REF_ON;
		case 8: return READ(RAMPSFD_E1_MIN_PIN)==REF_ON;
		case 9: return READ(RAMPSFD_E1_MAX_PIN)==REF_ON;
		case 10:return READ(RAMPSFD_E2_MIN_PIN)==REF_ON;
		case 11:return READ(RAMPSFD_E2_MAX_PIN)==REF_ON;
		*/
	}
	return false;
}

////////////////////////////////////////////////////////

bool  CStepperRampsFD::IsAnyReference()
{
	return	
		(_useReference[0] && READ(RAMPSFD_X_MIN_PIN) == RAMPSFD_REF_ON) ||
		(_useReference[1] && READ(RAMPSFD_X_MAX_PIN) == RAMPSFD_REF_ON) ||
		(_useReference[2] && READ(RAMPSFD_Y_MIN_PIN) == RAMPSFD_REF_ON) ||
		(_useReference[3] && READ(RAMPSFD_Y_MAX_PIN) == RAMPSFD_REF_ON) ||
		(_useReference[4] && READ(RAMPSFD_Z_MIN_PIN) == RAMPSFD_REF_ON) ||
		(_useReference[5] && READ(RAMPSFD_Z_MAX_PIN) == RAMPSFD_REF_ON);
}

////////////////////////////////////////////////////////

#endif