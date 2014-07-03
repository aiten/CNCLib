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

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include "ConfigurationCNCLib.h"
#include "Control.h"
#include "HelpParser.h"
#include "Stepper.h"

////////////////////////////////////////////////////////////

#ifdef _MSC_VER

bool CHelpParser::_exit = false;

#endif

////////////////////////////////////////////////////////////

void CHelpParser::Parse()
{
	unsigned char i;

	_reader->SkipSpaces();

#ifdef _MSC_VER
	if (IsToken(F("X"), true, false)) { _exit = true; return; }
#endif

	if (IsToken(F("s"), true, false)) { SetSpeed(); return; }

	if (IsToken(F("r1"), true, false)) { MoveRel(X_AXIS); return; }
	if (IsToken(F("r2"), true, false)) { MoveRel(Y_AXIS); return; }
	if (IsToken(F("r3"), true, false)) { MoveRel(Z_AXIS); return; }
#if NUM_AXIS > 3
	if (IsToken(F("r4"), true, false)) { MoveRel(A_AXIS); return; }
	if (IsToken(F("r5"), true, false)) { MoveRel(B_AXIS); return; }
	if (IsToken(F("r6"), true, false)) { MoveRel(C_AXIS); return; }
#endif
	if (IsToken(F("r"), true, false))  { MoveRel(); return; }

	if (IsToken(F("a1"), true, false)) { MoveAbs(X_AXIS); return; }
	if (IsToken(F("a2"), true, false)) { MoveAbs(Y_AXIS); return; }
	if (IsToken(F("a3"), true, false)) { MoveAbs(Z_AXIS); return; }
#if NUM_AXIS > 3
	if (IsToken(F("a4"), true, false)) { MoveAbs(A_AXIS); return; }
	if (IsToken(F("a5"), true, false)) { MoveAbs(B_AXIS); return; }
	if (IsToken(F("a6"), true, false)) { MoveAbs(C_AXIS); return; }
#endif
	if (IsToken(F("a"), true, false))  { MoveAbs(); return; }

#ifndef REDUCED_SIZE

	if (IsToken(F("p1"), true, false)) { SetPosition(X_AXIS); return; }
	if (IsToken(F("p2"), true, false)) { SetPosition(Y_AXIS); return; }
	if (IsToken(F("p3"), true, false)) { SetPosition(Z_AXIS); return; }
#if NUM_AXIS > 3
	if (IsToken(F("p4"), true, false)) { SetPosition(A_AXIS); return; }
	if (IsToken(F("p5"), true, false)) { SetPosition(B_AXIS); return; }
	if (IsToken(F("p6"), true, false)) { SetPosition(C_AXIS); return; }
#endif
#endif
	/*
		if (IsToken(F("ix"), true, false)) { MyGoToReference(X_AXIS); return; }
		if (IsToken(F("iy"), true, false)) { MyGoToReference(Y_AXIS); return; }
		if (IsToken(F("iz"), true, false)) { MyGoToReference(Z_AXIS); return; }
		if (IsToken(F("ia"), true, false)) { MyGoToReference(A_AXIS); return; }
		if (IsToken(F("ib"), true, false)) { MyGoToReference(B_AXIS); return; }
		if (IsToken(F("ic"), true, false)) { MyGoToReference(C_AXIS); return; }
		if (IsToken(F("i!"), true, false)) { if (CheckEOC())	::GoToReference();	return; }
		*/
	if (IsToken(F("i!"), true, false)) { if (CheckEOC())	CControl::GetInstance()->GoToReference();	return; }
	if (IsToken(F("!"), true, false))	{ if (CheckEOC()) { CStepper::GetInstance()->AbortMove(); } return; }
	if (IsToken(F("?"), true, false))	{ if (CheckEOC()) { CStepper::GetInstance()->Dump(CStepper::DumpAll); }	return; }

#ifndef REDUCED_SIZE
	if (IsToken(F("-"), true, false))
	{
		if (CheckEOC())
		{
			for (i = 0; i < NUM_AXIS - 1; i++)
			{
				StepperSerial.print(CStepper::GetInstance()->GetCurrentPosition(i)); StepperSerial.print(F(":"));
			}
			StepperSerial.print(CStepper::GetInstance()->GetCurrentPosition(NUM_AXIS - 1)); StepperSerial.println();
		}
		return;
	}
	if (IsToken(F("w"), true, false))  { if (CheckEOC())	{ CStepper::GetInstance()->WaitBusy(); } return; }
#endif

	Error(F("Illegal command"));
}

////////////////////////////////////////////////////////////

bool CHelpParser::SetSpeed()
{
	steprate_t max = 4500;
	steprate_t acc = 400;
	steprate_t dec = 400;

	if (IsUInt(_reader->GetChar()))
	{
		max = (steprate_t)GetUInt32();
		if (IsUInt(_reader->SkipSpaces()))
		{
			acc = (steprate_t)GetUInt32();
			if (IsUInt(_reader->SkipSpaces()))
			{
				dec = (steprate_t)GetUInt32();
			}
		}
	}

	if (CheckEOC())
	{
		CStepper::GetInstance()->SetDefaultMaxSpeed(max, acc, dec);
	}

	return true;
}

////////////////////////////////////////////////////////////

bool CHelpParser::MoveAbs()
{
	udist_t pos[NUM_AXIS] = { 0 };

	for (unsigned char axis = 0; axis < NUM_AXIS; axis++)
	{
		if (IsUInt(_reader->SkipSpaces()))
		{
			pos[axis] = GetUInt32();
		}
		else
		{
			break;
		}
	}

	if (CheckEOC())
	{
		CStepper::GetInstance()->MoveAbs(pos);
	}

	return true;
}

////////////////////////////////////////////////////////////

bool CHelpParser::MoveAbs(axis_t axis)
{
	if (axis > NUM_AXIS)
		return true;

	udist_t dist = 0;
	char ch = _reader->SkipSpaces();

	if (IsUInt(ch))
	{
		dist = GetUInt32();
	}

	if (CheckEOC())
		CStepper::GetInstance()->MoveAbs(axis, dist, 0);

	return true;
}

////////////////////////////////////////////////////////////

bool CHelpParser::MoveRel()
{
	sdist_t pos[NUM_AXIS] = { 0 };

	for (unsigned char axis = 0; axis < NUM_AXIS; axis++)
	{
		if (IsInt(_reader->SkipSpaces()))
		{
			pos[axis] = GetInt32();
		}
		else
		{
			break;
		}
	}

	if (CheckEOC())
	{
		CStepper::GetInstance()->MoveRel(pos);
	}

	return true;
}

////////////////////////////////////////////////////////////

bool CHelpParser::MoveRel(axis_t axis)
{
	if (axis > NUM_AXIS)
		return true;

	sdist_t dist = 0;
	char ch = _reader->SkipSpaces();

	if (IsInt(ch))
	{
		dist = GetInt32();
	}

	if (CheckEOC())
		CStepper::GetInstance()->MoveRel(axis, dist, 0);

	return true;
}

////////////////////////////////////////////////////////////

bool CHelpParser::SetPosition(axis_t axis)
{
	if (axis > NUM_AXIS)
		return true;

	udist_t dist = 0;
	char ch = _reader->SkipSpaces();

	if (IsUInt(ch))
	{
		dist = GetUInt32();
	}

	if (CheckEOC())
		CStepper::GetInstance()->SetPosition(axis, dist);

	return true;
}

////////////////////////////////////////////////////////////

bool CHelpParser::MyGoToReference(axis_t axis)
{
	if (axis > NUM_AXIS)
		return true;

	if (CheckEOC())
		CControl::GetInstance()->GoToReference(axis);

	return true;
}

////////////////////////////////////////////////////////////

bool CHelpParser::CheckEOC()
{
	if (_reader->IsError())
		return false;

	char ch = _reader->SkipSpaces();
	if (!_reader->IsEOC(ch))
	{
		Error(F("Illegal characters in command"));
		return false;
	}

	if (ch != 0)
	{
		_reader->GetNextChar();
		_reader->SkipSpaces();
	}

	return true;
}
