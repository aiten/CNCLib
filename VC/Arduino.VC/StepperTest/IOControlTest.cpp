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

#include "IOControlTest.h"
#include "..\MsvcStepper\MsvcStepper.h"
#include <Analog8IOControlSmooth.h>
#include <Analog8IOControl.h>
#include <Analog8InvertIOControl.h>
#include <Analog8XIOControlSmooth.h>


void CIOControlTest::RunTest()
{
	TestAnalogIO();
	TestAnalogIOInvert();
	TestAnalogIOSmooth();
	TestAnalogIOSmoothFade();

	TestAnalog9IOSmooth();
	TestAnalog9IOSmoothFade();
}

void CIOControlTest::TestAnalogIO()
{
	CAnalog8IOControl<10> spindle;

	spindle.Init(0);

	Assert(false, spindle.IsOn());
	Assert(0, spindle.GetLevel());
	Assert(0, spindle.GetIOLevel());

	spindle.On(0);
	Assert(0, spindle.GetLevel());
	Assert(0, spindle.GetIOLevel());

	spindle.OnMax();
	Assert(255, spindle.GetLevel());
	Assert(255, spindle.GetIOLevel());
	Assert(true, spindle.IsOn());

	spindle.On(100);
	Assert(100, spindle.GetLevel());
	Assert(100, spindle.GetIOLevel());
	Assert(true, spindle.IsOn());

	spindle.Off();
	Assert(100, spindle.GetLevel());
	Assert(0, spindle.GetIOLevel());
	Assert(false, spindle.IsOn());

	spindle.On();
	Assert(100, spindle.GetLevel());
	Assert(100, spindle.GetIOLevel());
	Assert(true, spindle.IsOn());

	spindle.On(100);
	spindle.SetLevel(111);
	Assert(100, spindle.GetIOLevel());
	Assert(111, spindle.GetLevel());

}

void CIOControlTest::TestAnalogIOInvert()
{
	CAnalog8InvertIOControl<10> spindle;

	spindle.Init(0);

	Assert(false, spindle.IsOn());
	Assert(0, spindle.GetLevel());
	Assert(0, spindle.GetIOLevel());

	spindle.On(0);
	Assert(0, spindle.GetLevel());
	Assert(0, spindle.GetIOLevel());

	spindle.OnMax();
	Assert(255, spindle.GetLevel());
	Assert(255, spindle.GetIOLevel());
	Assert(true, spindle.IsOn());

	spindle.On(100);
	Assert(100, spindle.GetLevel());
	Assert(100, spindle.GetIOLevel());
	Assert(true, spindle.IsOn());

	spindle.Off();
	Assert(100, spindle.GetLevel());
	Assert(0, spindle.GetIOLevel());
	Assert(false, spindle.IsOn());

	spindle.On();
	Assert(100, spindle.GetLevel());
	Assert(100, spindle.GetIOLevel());
	Assert(true, spindle.IsOn());

	spindle.On(100);
	spindle.SetLevel(111);
	Assert(100, spindle.GetIOLevel());
	Assert(111, spindle.GetLevel());
}


void CIOControlTest::TestAnalogIOSmooth()
{
	CAnalog8IOControlSmooth<10> spindle;
	spindle.SetDelay(1);

	spindle.Init(0);

	Assert(false, spindle.IsOn());
	Assert(0, spindle.GetLevel());
	Assert(0, spindle.GetIOLevel());

	spindle.On(0);
	Assert(0, spindle.GetLevel());
	Assert(0, spindle.GetIOLevel());

	spindle.OnMax();				
	Assert(255, spindle.GetLevel());
	Assert(255, spindle.GetIOLevel());
	Assert(true, spindle.IsOn());

	spindle.On(100);
	Assert(100, spindle.GetLevel());
	Assert(100, spindle.GetIOLevel());
	Assert(true, spindle.IsOn());

	spindle.Off();
	Assert(100, spindle.GetLevel());
	Assert(0, spindle.GetIOLevel());
	Assert(false, spindle.IsOn());

	spindle.On();
	Assert(100, spindle.GetLevel());
	Assert(100, spindle.GetIOLevel());
	Assert(true, spindle.IsOn());

	spindle.On(100);
	spindle.SetLevel(111);
	Assert(100, spindle.GetIOLevel());
	Assert(111, spindle.GetLevel());

}

void CIOControlTest::TestAnalogIOSmoothFade()
{
	CAnalog8IOControlSmooth<10> spindle;
	spindle.SetDelay(0);

	spindle.Init(100);
	Assert(100, spindle.GetLevel());
	Assert(100, spindle.GetIOLevel());
	Assert(0, spindle.GetCurrentIOLevel());

	// delay is 0 => inc eache call to "poll"

	for (uint8_t i = 1; i < 100; i++)
	{
		spindle.Poll();
		Assert(100, spindle.GetLevel());
		Assert(100, spindle.GetIOLevel());
		Assert(i, spindle.GetCurrentIOLevel());
	}
	spindle.Poll();
	Assert(100, spindle.GetCurrentIOLevel());
	spindle.Poll();
	Assert(100, spindle.GetCurrentIOLevel());

	spindle.On(50);
	Assert(50, spindle.GetLevel());
	Assert(50, spindle.GetIOLevel());
	Assert(100, spindle.GetCurrentIOLevel());

	for (uint8_t i = 99; i > 50; i--)
	{
		spindle.Poll();
		Assert(50, spindle.GetLevel());
		Assert(50, spindle.GetIOLevel());
		Assert(i, spindle.GetCurrentIOLevel());
	}
}

void CIOControlTest::TestAnalog9IOSmooth()
{
	CAnalog8XIOControlSmooth<10,11> spindle;
	spindle.SetDelay(1);

	spindle.Init(0);

	Assert(false, spindle.IsOn());
	Assert(0, spindle.GetLevel());
	Assert(0, spindle.GetIOLevel());

	spindle.On(0);
	Assert(0, spindle.GetLevel());
	Assert(0, spindle.GetIOLevel());

	spindle.OnMax();
	Assert(255, spindle.GetLevel());
	Assert(255, spindle.GetIOLevel());
	Assert(true, spindle.IsOn());

	spindle.On(-100);
	Assert(-100, spindle.GetLevel());
	Assert(-100, spindle.GetIOLevel());
	Assert(true, spindle.IsOn());

	spindle.On(100);
	Assert(100, spindle.GetLevel());
	Assert(100, spindle.GetIOLevel());
	Assert(true, spindle.IsOn());

	spindle.Off();
	Assert(100, spindle.GetLevel());
	Assert(0, spindle.GetIOLevel());
	Assert(false, spindle.IsOn());

	spindle.On();
	Assert(100, spindle.GetLevel());
	Assert(100, spindle.GetIOLevel());
	Assert(true, spindle.IsOn());

	spindle.On(100);
	spindle.SetLevel(111);
	Assert(100, spindle.GetIOLevel());
	Assert(111, spindle.GetLevel());

}

void CIOControlTest::TestAnalog9IOSmoothFade()
{
	CAnalog8XIOControlSmooth<10,11> spindle;
	spindle.SetDelay(0);

	spindle.Init(CHAR_MAX);
	Assert(CHAR_MAX, spindle.GetLevel());
	Assert(CHAR_MAX, spindle.GetIOLevel());
	Assert(0, spindle.GetCurrentIOLevel());

	// delay is 0 => inc eache call to "poll"

	for (int i = 1; i <= CHAR_MAX; i++)
	{
		spindle.Poll();
		Assert(CHAR_MAX, spindle.GetLevel());
		Assert(CHAR_MAX, spindle.GetIOLevel());
		Assert(i, spindle.GetCurrentIOLevel());
	}

	spindle.Poll();
	Assert(CHAR_MAX, spindle.GetCurrentIOLevel());
	spindle.Poll();
	Assert(CHAR_MAX, spindle.GetCurrentIOLevel());

	spindle.On(CHAR_MIN);
	Assert(CHAR_MIN, spindle.GetLevel());
	Assert(CHAR_MIN, spindle.GetIOLevel());
	Assert(CHAR_MAX, spindle.GetCurrentIOLevel());

	for (int i = CHAR_MAX-1; i >= CHAR_MIN; i--)
	{
		spindle.Poll();
		Assert(CHAR_MIN, spindle.GetLevel());
		Assert(CHAR_MIN, spindle.GetIOLevel());
		Assert(i, spindle.GetCurrentIOLevel());
	}
}