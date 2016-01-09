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

#pragma once

#include "TestClass.h"
#include "..\MsvcStepper\MsvcStepper.h"

class CStepperTest : public CTestClass
{

public:

	virtual void RunTest() override;

private:

	CMsvcStepper Stepper;

	void WriteStepperTestMovement();
	void AssertFile(const char* filename);
	void CreateTestFile(const char* filename);

	void AssertMove(mdist_t steps, CMsvcStepper::SMovementX mv)
	{
		Assert(true, mv.mv.IsActiveMove());
		Assert(steps, mv.mv.GetSteps());
	}

	void AssertWait(mdist_t steps, CMsvcStepper::SMovementX mv)
	{
		Assert(true, mv.mv.IsActiveWait());
		Assert(steps, mv.mv.GetSteps());
	}

	void TestAcc5000Dec();
	void TestAcc25000Dec();
	void TestAccCutDec();
	void TestAcc1000Acc1500Dec800Dec();
	void TestAcc1000AccCutDec800();
	void TestMergeRamp();
	void TestAcc5000DecCutAcc4800Dec();
	void TestUpDown();
	void TestStepUp();
	void TestSpeedUp();
	void TestBreakDown();
	void TestBreakDownPause();
	void TestBreakDownDelay();
	void TestJunctionSpeedSameDirection();
	void TestJunctionSpeedDifferentDirection();
	void TestJunctionYLessSpeed();
	void TestCircle();
	void TestX();
	void TestLastMoveTo0();
	void TestJerkSameDirection();
	void TestJerkSameDifferentDirection();
	void TestLongSlow();
	void TestVeryFast();
	void TestSetMaxAxixSpeed();
	void TestDiffMultiplierAbs();
	void TestDiffMultiplierLoop();
	void TestWait();
	void TestVerySlow();
	void TestStopMove();
	void TestWaitHold();
	void TestPause1();
	void TestPause2();
	void TestPause3();
	void TestPause4();
	
	void TestIo();

	void TestFile();
};

