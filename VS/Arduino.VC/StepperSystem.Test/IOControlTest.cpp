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

#include "..\MsvcStepper\MsvcStepper.h"
#include <Analog8IOControlSmooth.h>
#include <Analog8IOControl.h>
#include <Analog8InvertIOControl.h>
#include <Analog8XIOControlSmooth.h>

#include "CppUnitTest.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace StepperSystemTest
{

	TEST_CLASS(IOControlTest)
	{
	public:

		TEST_METHOD(TestAnalogIO)
		{
			CAnalog8IOControl<10> spindle;

			spindle.Init(0);

			Assert::AreEqual(false, spindle.IsOn());
			Assert::AreEqual((uint8_t) 0, spindle.GetLevel());
			Assert::AreEqual((uint8_t) 0, spindle.GetIOLevel());

			spindle.On(0);
			Assert::AreEqual((uint8_t)0, spindle.GetLevel());
			Assert::AreEqual((uint8_t)0, spindle.GetIOLevel());

			spindle.OnMax();
			Assert::AreEqual((uint8_t)255, spindle.GetLevel());
			Assert::AreEqual((uint8_t)255, spindle.GetIOLevel());
			Assert::AreEqual(true, spindle.IsOn());

			spindle.On(100);
			Assert::AreEqual((uint8_t)100, spindle.GetLevel());
			Assert::AreEqual((uint8_t)100, spindle.GetIOLevel());
			Assert::AreEqual(true, spindle.IsOn());

			spindle.Off();
			Assert::AreEqual((uint8_t)100, spindle.GetLevel());
			Assert::AreEqual((uint8_t)0, spindle.GetIOLevel());
			Assert::AreEqual(false, spindle.IsOn());

			spindle.On();
			Assert::AreEqual((uint8_t)100, spindle.GetLevel());
			Assert::AreEqual((uint8_t)100, spindle.GetIOLevel());
			Assert::AreEqual(true, spindle.IsOn());

			spindle.On(100);
			spindle.SetLevel(111);
			Assert::AreEqual((uint8_t)100, spindle.GetIOLevel());
			Assert::AreEqual((uint8_t)111, spindle.GetLevel());
		}

		TEST_METHOD(TestAnalogIOInvert)
		{
			CAnalog8InvertIOControl<10> spindle;

			spindle.Init(0);

			Assert::AreEqual(false, spindle.IsOn());
			Assert::AreEqual((uint8_t)0, spindle.GetLevel());
			Assert::AreEqual((uint8_t)0, spindle.GetIOLevel());

			spindle.On(0);
			Assert::AreEqual((uint8_t)0, spindle.GetLevel());
			Assert::AreEqual((uint8_t)0, spindle.GetIOLevel());

			spindle.OnMax();
			Assert::AreEqual((uint8_t)255, spindle.GetLevel());
			Assert::AreEqual((uint8_t)255, spindle.GetIOLevel());
			Assert::AreEqual(true, spindle.IsOn());

			spindle.On(100);
			Assert::AreEqual((uint8_t)100, spindle.GetLevel());
			Assert::AreEqual((uint8_t)100, spindle.GetIOLevel());
			Assert::AreEqual(true, spindle.IsOn());

			spindle.Off();
			Assert::AreEqual((uint8_t)100, spindle.GetLevel());
			Assert::AreEqual((uint8_t)0, spindle.GetIOLevel());
			Assert::AreEqual(false, spindle.IsOn());

			spindle.On();
			Assert::AreEqual((uint8_t)100, spindle.GetLevel());
			Assert::AreEqual((uint8_t)100, spindle.GetIOLevel());
			Assert::AreEqual(true, spindle.IsOn());

			spindle.On(100);
			spindle.SetLevel(111);
			Assert::AreEqual((uint8_t)100, spindle.GetIOLevel());
			Assert::AreEqual((uint8_t)111, spindle.GetLevel());
		}
		TEST_METHOD(TestAnalogIOSmooth)
		{

			CAnalog8IOControlSmooth<10> spindle;
			spindle.SetDelay(1);

			spindle.Init(0);

			Assert::AreEqual(false, spindle.IsOn());
			Assert::AreEqual((uint8_t)0, spindle.GetLevel());
			Assert::AreEqual((uint8_t)0, spindle.GetIOLevel());

			spindle.On(0);
			Assert::AreEqual((uint8_t)0, spindle.GetLevel());
			Assert::AreEqual((uint8_t)0, spindle.GetIOLevel());

			spindle.OnMax();
			Assert::AreEqual((uint8_t)255, spindle.GetLevel());
			Assert::AreEqual((uint8_t)255, spindle.GetIOLevel());
			Assert::AreEqual(true, spindle.IsOn());

			spindle.On(100);
			Assert::AreEqual((uint8_t)100, spindle.GetLevel());
			Assert::AreEqual((uint8_t)100, spindle.GetIOLevel());
			Assert::AreEqual(true, spindle.IsOn());

			spindle.Off();
			Assert::AreEqual((uint8_t)100, spindle.GetLevel());
			Assert::AreEqual((uint8_t)0, spindle.GetIOLevel());
			Assert::AreEqual(false, spindle.IsOn());

			spindle.On();
			Assert::AreEqual((uint8_t)100, spindle.GetLevel());
			Assert::AreEqual((uint8_t)100, spindle.GetIOLevel());
			Assert::AreEqual(true, spindle.IsOn());

			spindle.On(100);
			spindle.SetLevel(111);
			Assert::AreEqual((uint8_t)100, spindle.GetIOLevel());
			Assert::AreEqual((uint8_t)111, spindle.GetLevel());

		}
		TEST_METHOD(TestAnalogIOSmoothFade)
		{
			CAnalog8IOControlSmooth<10> spindle;
			spindle.SetDelay(0);

			spindle.Init(100);
			Assert::AreEqual((uint8_t)100, spindle.GetLevel());
			Assert::AreEqual((uint8_t)100, spindle.GetIOLevel());
			Assert::AreEqual((uint8_t)0, spindle.GetCurrentIOLevel());

			// delay is 0 => inc eache call to "poll"

			for (uint8_t i = 1; i < 100; i++)
			{
				spindle.Poll();
				Assert::AreEqual((uint8_t)100, spindle.GetLevel());
				Assert::AreEqual((uint8_t)100, spindle.GetIOLevel());
				Assert::AreEqual(i, spindle.GetCurrentIOLevel());
			}
			spindle.Poll();
			Assert::AreEqual((uint8_t)100, spindle.GetCurrentIOLevel());
			spindle.Poll();
			Assert::AreEqual((uint8_t)100, spindle.GetCurrentIOLevel());

			spindle.On(50);
			Assert::AreEqual((uint8_t)50, spindle.GetLevel());
			Assert::AreEqual((uint8_t)50, spindle.GetIOLevel());
			Assert::AreEqual((uint8_t)100, spindle.GetCurrentIOLevel());

			for (uint8_t i = 99; i > 50; i--)
			{
				spindle.Poll();
				Assert::AreEqual((uint8_t)50, spindle.GetLevel());
				Assert::AreEqual((uint8_t)50, spindle.GetIOLevel());
				Assert::AreEqual(i, spindle.GetCurrentIOLevel());
			}
		}

		TEST_METHOD(TestAnalog9IOSmooth)
		{
			CAnalog8XIOControlSmooth<10, 11> spindle;
			spindle.SetDelay(1);

			spindle.Init(0);

			Assert::AreEqual(false, spindle.IsOn());
			Assert::AreEqual((int16_t)0, spindle.GetLevel());
			Assert::AreEqual((int16_t)0, spindle.GetIOLevel());

			spindle.On(0);
			Assert::AreEqual((int16_t)0, spindle.GetLevel());
			Assert::AreEqual((int16_t)0, spindle.GetIOLevel());

			spindle.OnMax();
			Assert::AreEqual((int16_t)255, spindle.GetLevel());
			Assert::AreEqual((int16_t)255, spindle.GetIOLevel());
			Assert::AreEqual(true, spindle.IsOn());

			spindle.On(-100);
			Assert::AreEqual((int16_t)-100, spindle.GetLevel());
			Assert::AreEqual((int16_t)-100, spindle.GetIOLevel());
			Assert::AreEqual(true, spindle.IsOn());

			spindle.On(100);
			Assert::AreEqual((int16_t)100, spindle.GetLevel());
			Assert::AreEqual((int16_t)100, spindle.GetIOLevel());
			Assert::AreEqual(true, spindle.IsOn());

			spindle.Off();
			Assert::AreEqual((int16_t)100, spindle.GetLevel());
			Assert::AreEqual((int16_t)0, spindle.GetIOLevel());
			Assert::AreEqual(false, spindle.IsOn());

			spindle.On();
			Assert::AreEqual((int16_t)100, spindle.GetLevel());
			Assert::AreEqual((int16_t)100, spindle.GetIOLevel());
			Assert::AreEqual(true, spindle.IsOn());

			spindle.On(100);
			spindle.SetLevel(111);
			Assert::AreEqual((int16_t)100, spindle.GetIOLevel());
			Assert::AreEqual((int16_t)111, spindle.GetLevel());

		}
		TEST_METHOD(TestAnalog9IOSmoothFade)
		{
			CAnalog8XIOControlSmooth<10, 11> spindle;
			spindle.SetDelay(0);

			spindle.Init(CHAR_MAX);
			Assert::AreEqual((int16_t)CHAR_MAX, spindle.GetLevel());
			Assert::AreEqual((int16_t)CHAR_MAX, spindle.GetIOLevel());
			Assert::AreEqual((int16_t)0, spindle.GetCurrentIOLevel());

			// delay is 0 => inc eache call to "poll"

			for (int16_t i = 1; i <= CHAR_MAX; i++)
			{
				spindle.Poll();
				Assert::AreEqual((int16_t)CHAR_MAX, spindle.GetLevel());
				Assert::AreEqual((int16_t)CHAR_MAX, spindle.GetIOLevel());
				Assert::AreEqual(i, spindle.GetCurrentIOLevel());
			}

			spindle.Poll();
			Assert::AreEqual((int16_t)CHAR_MAX, spindle.GetCurrentIOLevel());
			spindle.Poll();
			Assert::AreEqual((int16_t)CHAR_MAX, spindle.GetCurrentIOLevel());

			spindle.On(CHAR_MIN);
			Assert::AreEqual((int16_t)CHAR_MIN, spindle.GetLevel());
			Assert::AreEqual((int16_t)CHAR_MIN, spindle.GetIOLevel());
			Assert::AreEqual((int16_t)CHAR_MAX, spindle.GetCurrentIOLevel());

			for (int16_t i = CHAR_MAX - 1; i >= CHAR_MIN; i--)
			{
				spindle.Poll();
				Assert::AreEqual((int16_t)CHAR_MIN, spindle.GetLevel());
				Assert::AreEqual((int16_t)CHAR_MIN, spindle.GetIOLevel());
				Assert::AreEqual(i, spindle.GetCurrentIOLevel());
			}
		}
	};
}