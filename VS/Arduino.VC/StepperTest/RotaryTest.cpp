////////////////////////////////////////////////////////
/*
This file is part of CNCLib - A library for stepper motors.

Copyright (c) 2013-2015 Herbert Aitenbichler

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

#include "stdafx.h"

#include "RotaryTest.h"
#include "..\MsvcStepper\MsvcStepper.h"
#include <RotaryButton.h>


void CRotaryTest::RunTest()
{
	TestCW();
	TestCCW();

	TestOverrunCW();
	TestOverrunCCW();

	TestAccuracyCW();
}

void CRotaryTest::TestCW()
{
	CRotaryButton<signed int, 1> rotary;

	Assert(0, rotary.GetPos());
	CRotaryButton<signed int, 1>::ERotaryEvent revent;

	// nothing changed
	revent = rotary.Tick(1, 1);
	Assert(CRotaryButton<signed int, 1>::Nothing, revent);
	Assert(0, rotary.GetPos());

	// CW
	revent = rotary.Tick(0, 1);
	Assert(CRotaryButton<signed int, 1>::ERotaryEvent::RightTurn, revent);
	Assert(1, rotary.GetPos());

	revent = rotary.Tick(0, 0);
	Assert(CRotaryButton<signed int, 1>::ERotaryEvent::RightTurn, revent);
	Assert(2, rotary.GetPos());

	revent = rotary.Tick(1, 0);
	Assert(CRotaryButton<signed int, 1>::ERotaryEvent::RightTurn, revent);
	Assert(3, rotary.GetPos());

	revent = rotary.Tick(1, 1);
	Assert(CRotaryButton<signed int, 1>::ERotaryEvent::RightTurn, revent);
	Assert(4, rotary.GetPos());

	revent = rotary.Tick(0, 1);
	Assert(CRotaryButton<signed int, 1>::ERotaryEvent::RightTurn, revent);
	Assert(5, rotary.GetPos());
}

void CRotaryTest::TestCCW()
{
	CRotaryButton<signed int, 1> rotary;
	rotary.SetMinMax(-100, 100, true);


	Assert(0, rotary.GetPos());
	CRotaryButton<signed int, 1>::ERotaryEvent revent;

	// nothing changed
	revent = rotary.Tick(1, 1);
	Assert(CRotaryButton<signed int, 1>::Nothing, revent);
	Assert(0, rotary.GetPos());

	// CW
	revent = rotary.Tick(1, 0);
	Assert(CRotaryButton<signed int, 1>::ERotaryEvent::LeftTurn, revent);
	Assert(-1, rotary.GetPos());

	revent = rotary.Tick(0, 0);
	Assert(CRotaryButton<signed int, 1>::ERotaryEvent::LeftTurn, revent);
	Assert(-2, rotary.GetPos());

	revent = rotary.Tick(0, 1);
	Assert(CRotaryButton<signed int, 1>::ERotaryEvent::LeftTurn, revent);
	Assert(-3, rotary.GetPos());

	revent = rotary.Tick(1, 1);
	Assert(CRotaryButton<signed int, 1>::ERotaryEvent::LeftTurn, revent);
	Assert(-4, rotary.GetPos());

	revent = rotary.Tick(1, 0);
	Assert(CRotaryButton<signed int, 1>::ERotaryEvent::LeftTurn, revent);
	Assert(-5, rotary.GetPos());
}

uint8_t CRotaryTest::GetPinA(int pos)
{
	switch (pos % 4)
	{
		default:
		case 0: return HIGH;
		case 1: return LOW;
		case 2: return LOW;
		case 3: return HIGH;
	}
}

uint8_t CRotaryTest::GetPinB(int pos)
{
	switch (pos % 4)
	{
		default:
		case 0: return HIGH;
		case 1: return HIGH;
		case 2: return LOW;
		case 3: return LOW;
	}
}

void CRotaryTest::TestOverrunCW()
{
	CRotaryButton<signed int, 1> rotary;
	rotary.SetMinMax(0, 15, true);

	for (int i = 0; i < 257; i++)
	{
		rotary.Tick(GetPinA(i), GetPinB(i));
		Assert(i%16, rotary.GetPos());
	}
}

void CRotaryTest::TestOverrunCCW()
{
	CRotaryButton<signed int, 1> rotary;
	rotary.SetMinMax(0, 15, true);

	for (int i = 256; i >0; i--)
	{
		rotary.Tick(GetPinA(i), GetPinB(i));
		Assert(i % 16, rotary.GetPos());
	}
}

void CRotaryTest::TestAccuracyCW()
{
	const int ACCURACY = 4;
	CRotaryButton<signed int, ACCURACY> rotary;

	int i;
	for (i = 0; i <126; i++)
	{
		rotary.Tick(GetPinA(i), GetPinB(i));

		Assert(i, rotary.GetFullRangePos());
		Assert((i+ ACCURACY/2) / ACCURACY, rotary.GetPos());
	}
	Assert(CRotaryButton<signed int, 1>::ERotaryEvent::Overrun,rotary.Tick(GetPinA(i), GetPinB(i)));
	int pos = i - ACCURACY;
	Assert(pos, rotary.GetFullRangePos());
	Assert((pos + ACCURACY / 2) / ACCURACY, rotary.GetPos());
}
