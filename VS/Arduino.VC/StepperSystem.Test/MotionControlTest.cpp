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

#include "CppUnitTest.h"

#include "..\MsvcStepper\MsvcStepper.h"

////////////////////////////////////////////////////////

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace StepperSystemTest
{

	TEST_CLASS(CMotionControlTest)
	{
	public:

		CMsvcStepper Stepper;

		TEST_METHOD(MotionControlTest)
		{
			Stepper.Init();
			Stepper.UseSpeedSign = true;

			for (axis_t x = 0; x < NUM_AXIS; x++)
			{
				Stepper.SetLimitMax(x, 0x100000);
			}
			Stepper.SetWaitFinishMove(false);

			FeedRateTest();
		}

		void FeedRateTest()
		{
			CMotionControlBase mc;
			mc.UnitTest();
			mc.InitConversion(
				[](axis_t, sdist_t val) { return (mm1000_t)val; },
				[](axis_t, mm1000_t val) { return (sdist_t)val; }
			);

			mm1000_t to1[NUM_AXIS] = { 1000, 0 , 0 };
			mm1000_t to2[NUM_AXIS] = { 1000, 2000 , 0 };
			mm1000_t to3[NUM_AXIS] = { 1000, 2000 , 3000 };

			Assert::AreEqual((long)1234, (long)mc.GetFeedRate(to1, 1234 * 60));

			Assert::AreEqual((long)1103, (long)mc.GetFeedRate(to2, 1234 * 60));		// 2000/sqrt(1000*1000 + 2000*2000) * 1234

			Assert::AreEqual((long)989, (long)mc.GetFeedRate(to3, 1234 * 60));
		}

		TEST_METHOD(FeedRateDistOverrunTest)
		{
			CMotionControlBase mc;
			mc.UnitTest();
			mc.InitConversion(
				[](axis_t, sdist_t val) { return (mm1000_t)val; },
				[](axis_t, mm1000_t val) { return (sdist_t)val; }
			);

			mm1000_t to1[] = { 100000, 0 , 0 };
			mm1000_t to2[] = { 100000, 20000 , 0 };
			mm1000_t to3[] = { 100000, 20000 , 30000 };

			Assert::AreEqual((long)1234, (long)mc.GetFeedRate(to1, 1234 * 60));

			Assert::AreEqual((long)1209, (long)mc.GetFeedRate(to2, 1234 * 60));

			Assert::AreEqual((long)1159, (long)mc.GetFeedRate(to3, 1234 * 60));
		}

		TEST_METHOD(FeedRateFeedRateOverrun16BitTest)
		{
			CMotionControlBase mc;
			mc.UnitTest();
			mc.InitConversion(
				[](axis_t, sdist_t val) { return (mm1000_t)val * 10; },
				[](axis_t, mm1000_t val) { return (sdist_t)val / 10; }
			);

			mm1000_t to1[] = { 100, 0 , 0 };
			mm1000_t to2[] = { 100, 200 , 0 };
			mm1000_t to3[] = { 100, 200 , 300 };

			Assert::AreEqual((long)12345, (long)mc.GetFeedRate(to1, 123456 * 60));

			Assert::AreEqual((long)11022, (long)mc.GetFeedRate(to2, 123456 * 60));

			Assert::AreEqual((long)9902, (long)mc.GetFeedRate(to3, 123456 * 60));
		}
	};
}
