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

#include "StepperSMC800.h"

////////////////////////////////////////////////////////

#if defined(__AVR_ATmega1280__) || defined(__AVR_ATmega2560__)

#define SMC800_REFININ 40
#define SMC800_STROBEPIN 41

#elif defined(__AVR_ATmega328P__) || defined (_MSC_VER)

#define SMC800_REFININ 11
#define SMC800_STROBEPIN 10
// use SMC800 Byte 2-9

#elif defined(__SAM3X8E__)

#define SMC800_REFININ 11
#define SMC800_STROBEPIN 10

#else
ToDo;
#endif

////////////////////////////////////////////////////////

enum EStepperBaseAxis
{
	StepperX = 0,
	StepperY = 64,
	StepperZ = 128
};

////////////////////////////////////////////////////////

static const unsigned char sbm800halfstep0[8] PROGMEM = { 0x3F, 0x3F, 0x1F, 0x1F, 0x1B, 0x1B, 0x3B, 0x3B };
static const unsigned char sbm800halfstep20[8] PROGMEM = { 0x37, 0x36, 0x1E, 0x16, 0x13, 0x12, 0x3A, 0x32 };
static const unsigned char sbm800halfstep60[8] PROGMEM = { 0x2F, 0x2D, 0x1D, 0x0D, 0x0B, 0x09, 0x39, 0x29 };
static const unsigned char sbm800halfstep100[8] PROGMEM = { 0x27, 0x2D, 0x1C, 0x0D, 0x03, 0x09, 0x38, 0x29 };

static const unsigned char sbm800fullstep0[4] PROGMEM = { 0x3F, 0x3B, 0x1B, 0x1F };
static const unsigned char sbm800fullstep20[4] PROGMEM = { 0x36, 0x32, 0x12, 0x16 };
static const unsigned char sbm800fullstep60[4] PROGMEM = { 0x2D, 0x29, 0x09, 0x0D };
static const unsigned char sbm800fullstep100[4] PROGMEM = { 0x24, 0x20, 0x00, 0x04 };

static const unsigned char stepperadd[SMC800_NUM_AXIS] PROGMEM = { StepperX, StepperY, StepperZ };

////////////////////////////////////////////////////////

CStepperSMC800::CStepperSMC800()
{
	InitMemVar();
}

////////////////////////////////////////////////////////

void CStepperSMC800::OutSMC800Cmd(const unsigned char val)
{

#if defined(__AVR_ATmega1280__) || defined(__AVR_ATmega2560__)

	PORTL = val;

#elif defined(__SAM3X8E__)

#pragma message ("TODO: due")

#elif defined(__AVR_ATmega328P__)

	PORTD = (PORTD & 3) + (val << 2);
	PORTB = (PORTB & 0b11111100) + (val >> 6);

#elif defined(_MSC_VER)
	val;
#else
	ToDo
#endif

	digitalWrite(SMC800_STROBEPIN, 0);
	digitalWrite(SMC800_STROBEPIN, 1);
}

////////////////////////////////////////////////////////

void CStepperSMC800::InitMemVar()
{
	register unsigned char i;
	for (i = 0; i < SMC800_NUM_AXIS; i++)	_stepIdx[i] = 0;
	for (i = 0; i < SMC800_NUM_AXIS; i++)	_level[i] = Level0;

	_idleLevel = Level20;
}

////////////////////////////////////////////////////////

void CStepperSMC800::Init()
{
#if defined(__AVR_ATmega1280__) || defined(__AVR_ATmega2560__)

	DDRL = 0xff;

#elif defined(__SAM3X8E__)

#pragma message ("TODO: due")

#elif defined(__AVR_ATmega328P__)

	DDRD = (DDRD & 3) + 0b11111100;
	DDRB = (DDRB & 0b11111100) + 3;

#elif defined(_MSC_VER)

#else
	ToDo
#endif

	pinMode(SMC800_STROBEPIN, OUTPUT);
	digitalWrite(SMC800_STROBEPIN, 1);

	pinMode(SMC800_REFININ, INPUT_PULLUP);

	super::Init();
	InitMemVar();
}

////////////////////////////////////////////////////////

void CStepperSMC800::Remove()
{
#if defined(__AVR_ATmega1280__) || defined(__AVR_ATmega2560__)

	DDRL = 0x0;

#elif defined(__SAM3X8E__)

#pragma message ("TODO: due")

#elif defined(__AVR_ATmega328P__)

	DDRD = DDRD & 3;
	DDRB = DDRB & 0b11111100;

#elif defined(_MSC_VER)

#else
	ToDo
#endif

	pinMode(SMC800_REFININ, INPUT);
	pinMode(SMC800_STROBEPIN, INPUT);

	super::Remove();
}

////////////////////////////////////////////////////////

void  CStepperSMC800::Step(const unsigned char steps[NUM_AXIS], unsigned char directionUp)
{
	for (axis_t axis=0;axis < SMC800_NUM_AXIS;axis++)
	{
		if (IsBitSet(directionUp,axis))
			_stepIdx[axis] += steps[axis];
		else
			_stepIdx[axis] -= steps[axis];;
		SetPhase(axis);
	}
}

////////////////////////////////////////////////////////

void CStepperSMC800::SetEnable(axis_t axis, unsigned char level)
{
	if (axis<SMC800_NUM_AXIS)
	{
		if (level > 60)      _level[axis] = Level100;
		else if (level > 20) _level[axis] = Level60;
		else if (level > 0)  _level[axis] = Level20;
		else				 _level[axis] = Level0;
		SetPhase(axis);
	}
}

////////////////////////////////////////////////////////

unsigned char CStepperSMC800::GetEnable(axis_t axis)
{
	if (axis >= SMC800_NUM_AXIS) return 0;
	return _level[axis];
}

////////////////////////////////////////////////////////

void CStepperSMC800::SetPhase(axis_t axis)
{
	if (axis < SMC800_NUM_AXIS)
	{
		register unsigned char addIO = pgm_read_byte(&stepperadd[axis]);
		register unsigned char stepidx = _stepIdx[axis];

		if (_stepMode[axis] == FullStep)
		{
			stepidx = stepidx & 0x3;
			switch (_level[axis])
			{
				default:
				case Level100:   OutSMC800Cmd(pgm_read_byte(&sbm800fullstep100[stepidx]) + addIO);      break;
				case Level60:    OutSMC800Cmd(pgm_read_byte(&sbm800fullstep60[stepidx]) + addIO);       break;
				case Level20:    OutSMC800Cmd(pgm_read_byte(&sbm800fullstep20[stepidx]) + addIO);       break;
				case Level0:     OutSMC800Cmd(pgm_read_byte(&sbm800fullstep0[stepidx]) + addIO);        break;
			}
		}
		else
		{
			stepidx = stepidx & 0x7;
			switch (_level[axis])
			{
				default:
				case Level100:    OutSMC800Cmd(pgm_read_byte(&sbm800halfstep100[stepidx]) + addIO);    break;
				case Level60:     OutSMC800Cmd(pgm_read_byte(&sbm800halfstep60[stepidx]) + addIO);     break;
				case Level20:     OutSMC800Cmd(pgm_read_byte(&sbm800halfstep20[stepidx]) + addIO);     break;
				case Level0:      OutSMC800Cmd(pgm_read_byte(&sbm800halfstep0[stepidx]) + addIO);      break;
			}
		}
	}
}

////////////////////////////////////////////////////////

bool  CStepperSMC800::IsReference(unsigned char /*referenceid*/)
{
	return digitalRead(SMC800_REFININ) == HIGH;
}

////////////////////////////////////////////////////////

void CStepperSMC800::MoveAwayFromReference(axis_t /* axis */, sdist_t dist, steprate_t vMax)
{
	MoveRelEx(vMax, X_AXIS, min(dist, (sdist_t)GetLimitMax(X_AXIS) / 2), 
					Y_AXIS, min(dist, (sdist_t)GetLimitMax(Y_AXIS) / 2), 
					Z_AXIS, min(dist, (sdist_t)GetLimitMax(Z_AXIS) / 2), 
					-1);
}
