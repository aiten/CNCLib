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
#include "StepperTest.h"
#include "TestTools.h"


CSerial Serial;

int FromMM(double mm)
{
	return (int)(mm * 3200);
}

void CStepperTest::RunTest()
{
	Serial.println("StepperTest is starting ...");

	// only drive stepper  
	Stepper.Init();
	Stepper.UseSpeedSign = true;

	for (axis_t x = 0; x < NUM_AXIS; x++)
	{
		Stepper.SetLimitMax(x, 0x100000);
	}
	Stepper.SetWaitFinishMove(false);

	bool alltests = false;

	if (false || alltests) 		TestAcc5000Dec();
	if (false || alltests) 		TestAcc25000Dec();
	if (false || alltests) 		TestAccCutDec();
	if (false || alltests) 		TestAcc1000Acc1500Dec800Dec();
	if (false || alltests) 		TestAcc1000AccCutDec800();
	if (false || alltests) 		TestMergeRamp();
	if (false || alltests) 		TestAcc5000DecCutAcc4800Dec();
	if (false || alltests) 		TestUpDown();
	if (false || alltests) 		TestStepUp();
	if (false || alltests) 		TestSpeedUp();
	if (false || alltests) 		TestBreakDown();
	if (false || alltests) 		TestBreakDownPause();
	if (false || alltests) 		TestBreakDownDelay();
	if (false || alltests) 		TestJunctionSpeedSameDirection();
	if (false || alltests) 		TestJunctionSpeedDifferentDirection();
	if (false || alltests) 		TestJunctionYLessSpeed();
	if (false || alltests) 		TestCircle();
	if (false || alltests) 		TestX();
	if (false || alltests) 		TestLastMoveTo0();
	if (false || alltests) 		TestJerkSameDirection();
	if (false || alltests) 		TestJerkSameDifferentDirection();
	if (false || alltests) 		TestLongSlow();
	if (false || alltests) 		TestVeryFast();
	if (false || alltests) 		TestSetMaxAxixSpeed();
	if (false || alltests) 		TestDiffMultiplier();
	if (false || alltests) 		TestWait();
	if (false || alltests) 		TestVerySlow();
	if (false || alltests) 		TestStopMove();
	if (false || alltests) 		TestWaitHold();
	if (true  || alltests)		TestPause();

	if (false || alltests)		TestFile();


}

void CStepperTest::TestAcc5000Dec()
{
	// ramp up, max, down
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 100, 150);
	Stepper.MoveRel(0, 4000, 5000);
	Stepper.EndTest("TR01_Acc5000Dec.csv");
}

void CStepperTest::TestAcc25000Dec()
{
	// ramp up, max, down
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(30000, 400, 550);
	//		Stepper.MoveRel(0,65534,25000);
	Stepper.MoveRel(0, 65535, 0);
	Stepper.EndTest("TR01_Acc25000Dec.csv");
}

void CStepperTest::TestAccCutDec()
{
	// ramp up, NO max, down
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 100, 150);
	Stepper.MoveRel(0, 50, 1000);
	Stepper.EndTest("TR02_AccCutDec.csv");
}

void CStepperTest::TestAcc1000Acc1500Dec800Dec()
{
	// 3 Moves: 200(1000),300(1500),200(1000)
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 80, 150);
	Stepper.MoveRel(0, 200, 1000);
	Stepper.MoveRel(0, 300, 1500);
	Stepper.MoveRel(0, 400, 800);
	Stepper.EndTest("TR03_Acc1000Acc1500Dec800Dec.csv");
}

void CStepperTest::TestAcc1000AccCutDec800()
{
	// 3 Moves: 200(1000),300(1500),200(1000)
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 80, 150);
	Stepper.MoveRel(0, 200, 1000);
	Stepper.MoveRel(0, 100, 1500);
	Stepper.MoveRel(0, 400, 800);
	Stepper.EndTest("TR04_Acc1000AccCutDec800.csv");
}

void CStepperTest::TestMergeRamp()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 100, 150);
	Stepper.CStepper::MoveRel(0, 150, 1000);
	Stepper.CStepper::MoveRel(0, 350, 2000);
	Stepper.CStepper::MoveRel(0, 450, 3000);
	Stepper.CStepper::MoveRel(0, 700, 4000);
	Stepper.CStepper::MoveRel(0, 950, 5000);
	Stepper.CStepper::MoveRel(0, 700, 4000);
	Stepper.CStepper::MoveRel(0, 450, 3000);
	Stepper.CStepper::MoveRel(0, 350, 2000);
	Stepper.CStepper::MoveRel(0, 150, 1000);
	Stepper.EndTest("TR05_MergeRamp.csv");
}

void CStepperTest::TestAcc5000DecCutAcc4800Dec()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 100, 150);
	Stepper.CStepper::MoveRel(0, 3000, 5000);
	Stepper.CStepper::MoveRel(0, 200, 2000);
	Stepper.CStepper::MoveRel(0, 3000, 4800);
	Stepper.EndTest("TR06_Acc5000DecCutAcc4800Dec.csv");
}

void CStepperTest::TestUpDown()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 100, 150);
	Stepper.CStepper::MoveRel(0, 1000, 1000);
	Stepper.CStepper::MoveRel(0, -1000, 1000);
	Stepper.CStepper::MoveRel(0, 1000, 2000);
	Stepper.CStepper::MoveRel(0, -1000, 2000);
	Stepper.EndTest("TR07_UpDown.csv");
}

void CStepperTest::TestStepUp()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 100, 150);
	Stepper.CStepper::MoveRel(0, 300, 1000);
	Stepper.CStepper::MoveRel(0, 200, 2000);
	Stepper.CStepper::MoveRel(0, 1000, 3000);
	Stepper.CStepper::MoveRel(0, 2500, 5000);
	Stepper.CStepper::MoveRel(0, 300, 500);
	Stepper.CStepper::MoveRel(0, 550, 5000);
	Stepper.EndTest("TR08_StepUp.csv");
}

void CStepperTest::TestSpeedUp()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 100, 150);
	Stepper.MSCInfo = "MoveRel(0,100,3000)";
	Stepper.CStepper::MoveRel(0, 100, 3000);
	Stepper.MSCInfo = "MoveRel(0,100,3000)";
	Stepper.CStepper::MoveRel(0, 100, 3000);
	Stepper.MSCInfo = "MoveRel(0,100,3000)";
	Stepper.CStepper::MoveRel(0, 100, 3000);
	Stepper.MSCInfo = "MoveRel(0,100,3000)";
	Stepper.CStepper::MoveRel(0, 100, 3000);
	Stepper.MSCInfo = "MoveRel(0,100,3000)";
	Stepper.CStepper::MoveRel(0, 100, 3000);
	Stepper.MSCInfo = "MoveRel(0,5000,3000)";
	Stepper.CStepper::MoveRel(0, 5000, 3000);
	Stepper.EndTest("TR09_SpeedUp.csv");
}


void CStepperTest::TestBreakDown()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 100, 150);
	Stepper.CStepper::MoveRel(0, 2500, 5000);
	Stepper.CStepper::MoveRel(0, 100, 3000);
	Stepper.CStepper::MoveRel(0, 75, 2500);
	Stepper.CStepper::MoveRel(0, 50, 2000);
	Stepper.EndTest("TR10_BreakDown.csv");
}

void CStepperTest::TestBreakDownPause()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 100, 150);
	Stepper.CStepper::MoveRel(0, 2500, 5000);
	Stepper.CStepper::MoveRel(0, 100, 3000);
	Stepper.CStepper::MoveRel(0, 75, 2500);
	Stepper.CStepper::MoveRel(0, 50, 2000);
	Stepper.CStepper::MoveRel(0, 100, 300);
	Stepper.EndTest("TR11_BreakDownPause.csv");
}

void CStepperTest::TestBreakDownDelay()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 100, 150);
	Stepper.CStepper::MoveRel(0, 2500, 5000);
	Stepper.CStepper::MoveRel(0, 100, 3000);
	Stepper.CStepper::MoveRel(0, 75, 2500);
	Stepper.CStepper::MoveRel(0, 50, 2000);
	Stepper.CStepper::MoveRel(0, 100, 300);
	Stepper.CStepper::MoveRel(0, 5000, 3000);
	Stepper.EndTest("TR12_BreakDownDelay.csv");
}


void CStepperTest::TestJunctionSpeedSameDirection()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 100, 150);
	Stepper.MoveRel3(3000, 1000, 50, 5000);
	Stepper.MoveRel3(1000, 1000, 50, 3000);
	Stepper.MoveRel3(0000, 1000, 50, 1000);
	Stepper.MoveRel3(1000, 1000, 50, 1000);
	Stepper.EndTest("TR13_JunctionSpeedSameDirection.csv");
}

void CStepperTest::TestJunctionSpeedDifferentDirection()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 100, 150);
	Stepper.MoveRel3(1000, 900, 50, 5000);
	Stepper.MoveRel3(1000, -900, -50, 5000);
	Stepper.EndTest("TR14_JunctionSpeedDifferentDirection.csv");
}

void CStepperTest::TestJunctionYLessSpeed()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 100, 150);
	Stepper.MoveRel3(1000, 100, 0, 5000);
	Stepper.MoveRel3(1000, 900, 500, 5000);
	Stepper.EndTest("TR15_JunctionYLessSpeed.csv");
}


void CStepperTest::TestCircle()
{
	Stepper.InitTest("TR16_Circle.csv");
	Stepper.SetDefaultMaxSpeed(20000, 350, 350);
	Stepper.SetJerkSpeed(X_AXIS, 1000);
	Stepper.SetJerkSpeed(Y_AXIS, 1000);
	Stepper.SetJerkSpeed(Z_AXIS, 1000);
	Stepper.SetJerkSpeed(A_AXIS, 1000);
	Stepper.SetJerkSpeed(B_AXIS, 1000);
	double r_mm = 40.0;
	int r = FromMM(r_mm);
	int x = r;
	int y = r;
	int n = (int)(2.0 * r_mm * M_PI * 3 + 72); // 2 * r * 3 * 3 + 72;			// 2*r*PI*3 + 72) => r must be mm;
	Polygon(Stepper, x, y, r, n, 0, 10000);
	Stepper.EndTest();
}

void CStepperTest::TestX()
{
	Stepper.InitTest();
	int count = 0;
	Stepper.CStepper::MoveRel(0, 300, 1000); count += 300;
	Stepper.CStepper::MoveRel(0, 800, 2000); count += 800;
	Stepper.CStepper::MoveRel(0, 1500, 3000); count += 1500;
	Stepper.CStepper::MoveRel(0, 3500, 5000); count += 3500;
	Stepper.CStepper::MoveRel(0, 300, 500); count += 300;
	Stepper.CStepper::MoveRel(0, 550, 5000); count += 550;
	Stepper.EndTest("TR17_X.csv");
}

void CStepperTest::TestLastMoveTo0()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 100, 150);
	Stepper.MoveRel3(4000, 0, 0, 5000);
	Stepper.MoveRel3(100, 0, 0, 4000);
	Stepper.EndTest("TR18_LastMoveTo0.csv");
}

void CStepperTest::TestJerkSameDirection()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(3000, 100, 150);
	Stepper.SetJerkSpeed(0, 300);
	Stepper.SetJerkSpeed(1, 300);
	Stepper.SetJerkSpeed(2, 300);

	Stepper.MoveRel3(1000, 1000, 0, 2000);	// max speed
	Stepper.MoveRel3(1000, 850, 0, 2000);	// -v150 => OK
	Stepper.MoveRel3(1000, 1000, 0, 2000);	// other direction OK TO
	Stepper.MoveRel3(1000, 710, 0, 2000);	// -v580 => fail
	Stepper.MoveRel3(1000, 1000, 0, 2000);	// other direction
	Stepper.EndTest("TR19_JerkSameDirection.csv");
}

void CStepperTest::TestJerkSameDifferentDirection()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(3000, 100, 150);
	Stepper.SetJerkSpeed(0, 300);
	Stepper.SetJerkSpeed(1, 300);
	Stepper.SetJerkSpeed(2, 300);

	Stepper.MoveRel3(1000, 100, 0, 2000);	// max speed
	Stepper.MoveRel3(1000, -50, 0, 2000);	// -vdiff => OK
	Stepper.MoveRel3(1000, 100, 0, 2000);	// other direction OK TO

	Stepper.MoveRel3(1000, -150, 0, 2000);	// -vdiff => fail
	Stepper.MoveRel3(1000, 100, 0, 2000);	// other direction OK TO


	Stepper.EndTest("TR20_JerkSameDifferentDirection.csv");
}

void CStepperTest::TestLongSlow()
{
	// ramp up, max, down
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(100, 200, 250);
	Stepper.MoveRel(0, 10000, 0);
	Stepper.EndTest("TR21_LongSlow.csv");
}

void CStepperTest::TestVeryFast()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(65535, 400, 700);
	Stepper.MoveRelEx(16000, 0, 25000, -1);
	Stepper.MoveRelEx(32000, 0, 50000, -1);
	Stepper.MoveRelEx(47000, 0, 100000, -1);
	Stepper.MoveRelEx(65000, 0, 100000, -1);
	Stepper.EndTest("TR22_VeryFast.csv");
}

void CStepperTest::TestSetMaxAxixSpeed()
{
	Stepper.InitTest();
	Stepper.SetMaxSpeed(1, 500);
	Stepper.SetMaxSpeed(2, 400);
	Stepper.MoveRelEx(0, 0, 10000, 1, 9000, 2, 9500, -1);
	Stepper.EndTest("TR23_SetMaxAxixSpeed.csv");
}

void CStepperTest::TestDiffMultiplier()
{
	/*
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(65535, 400, 700);
	Stepper.MoveRelEx(47000, 0, 65535, 1, 50000, 2 ,0, 3, 0, 4, 0,  -1);
	Stepper.MoveRelEx(47000, 0, 65535, 1, 50000, 2 ,32000, 3, 22000, 4, 13000,  -1);
	Stepper.MoveRelEx(47000, 0, 65530, 1, 50002, 2, 32000, 3, 22001, 4, 13001, -1);
	Stepper.EndTest("TR24_DiffMultiplier#1.csv");


	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(65535, 400, 700);
	Stepper.MoveRelEx(47000, 0, 2, -1);
	Stepper.EndTest("TR24_DiffMultiplier#2.csv");
	*/
	for (udist_t x = 0; x<1000; x++)
		for (udist_t y = 0; y<x; y++)
		{
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(65535, 400, 700);
			Stepper.MoveRelEx(SPEED_MULTIPLIER_3 + 1, 0, x, 1, y, -1);
			//Stepper.MoveRelEx(47000, 0, x, 1, y, -1);
			Stepper.EndTest("TR24_DiffMultiplier#x.csv");
			if (Stepper.GetCurrentPosition(0) != x || Stepper.GetCurrentPosition(1) != y)
			{
				*((int *)NULL) = 1021312;	//Abort
											//MessageBox(_T("Fehler"));
			}
		}
}

void CStepperTest::TestWait()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 100, 150);
	Stepper.CStepper::MoveRel(0, 2500, 5000);
	Stepper.CStepper::Wait(0);
	Stepper.CStepper::MoveRel(0, 100, 3000);
	Stepper.CStepper::MoveRel(0, 75, 2500);
	Stepper.CStepper::MoveRel(0, 50, 2000);
	Stepper.CStepper::MoveRel(0, 100, 300);
	Stepper.CStepper::MoveRel(0, 5000, 3000);
	Stepper.CStepper::Wait(100);
	Stepper.CStepper::MoveRel(0, 5000, 3000);
	Stepper.EndTest("TR24_Wait.csv");
}

void CStepperTest::TestVerySlow()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 100, 150);
	Stepper.CStepper::MoveRel(0, 2500, 1);
	Stepper.CStepper::MoveRel(0, 1, 2);
	Stepper.CStepper::MoveRel(0, 100, 3);
	Stepper.EndTest("TR25_VerySlow.csv");
}

void CStepperTest::TestStopMove()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 100, 150);
	Stepper.MoveRelEx(47000, 0, 2500, 1, 1000, 2, 500, -1);
	Stepper.MoveRelEx(47000, 0, 2500, 1, -1000, 2, 500, -1);
	Stepper.CStepper::StopMove(75);
	Stepper.EndTest("TR25_StopMove.csv");
}

void CStepperTest::TestWaitHold()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 100, 150);
	Stepper.CStepper::MoveRel(0, 2500, 5000);
	Stepper.CStepper::SetWaitConditional(true);
	Stepper.CStepper::WaitConditional(0);				// timeout 0 => no wait
	Stepper.CStepper::MoveRel(0, 100, 3000);
	Stepper.CStepper::WaitConditional(100);
	Stepper.CStepper::MoveRel(0, 5000, 3000);
	Stepper.EndTest("TR25_WaitHold.csv");
}

void CStepperTest::TestPause()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(5000, 100, 150);

	Stepper.CStepper::MoveRel(0, 2500, 5000);
	Stepper.CStepper::MoveRel(0, -100, 3000);
	Stepper.CStepper::MoveRel(0, 5000, 3000);
	Stepper.OptimizeMovementQueue(true);			// calc ramp
	Stepper.CStepper::PauseMove();
	Assert(4, Stepper.GetMovementCount());
	Assert(true, Stepper.GetMovement(0).mv.IsActiveMove());  Assert(2500, Stepper.GetMovement(0).mv.GetSteps());
	Assert(true, Stepper.GetMovement(1).mv.IsActiveWait());
	Assert(true, Stepper.GetMovement(2).mv.IsActiveMove());	 Assert(100, Stepper.GetMovement(0).mv.GetSteps());
	Assert(true, Stepper.GetMovement(3).mv.IsActiveMove());  Assert(5000, Stepper.GetMovement(0).mv.GetSteps());
	Stepper.EndTest("TR26_TestPause#A.csv");

	Stepper.InitTest();
	Stepper.CStepper::MoveRel(0, 2500, 5000);
	Stepper.CStepper::MoveRel(0, 1000, 3000);
	Stepper.CStepper::MoveRel(0, -1500, 3000);
	Stepper.OptimizeMovementQueue(true);			// calc ramp
	Stepper.CStepper::PauseMove();
	Assert(4, Stepper.GetMovementCount());
	Assert(true, Stepper.GetMovement(0).mv.IsActiveMove()); Assert(2500, Stepper.GetMovement(0).mv.GetSteps());
	Assert(true, Stepper.GetMovement(1).mv.IsActiveMove()); Assert(1000, Stepper.GetMovement(0).mv.GetSteps());
	Assert(true, Stepper.GetMovement(2).mv.IsActiveWait());
	Assert(true, Stepper.GetMovement(3).mv.IsActiveMove()); Assert(1500, Stepper.GetMovement(0).mv.GetSteps());
	Stepper.EndTest("TR26_TestPausea#B.csv");


}

void CStepperTest::TestFile()
{
	Stepper.InitTest();
	Stepper.SetDefaultMaxSpeed(1500, 100, 150);
	Stepper.SetJerkSpeed(0, 800);
	Stepper.SetJerkSpeed(1, 800);
	Stepper.SetJerkSpeed(2, 800);

	Stepper.SetDefaultMaxSpeed(8000, 500, 500);
	Stepper.SetLimitMax(0, 55600);  // 6950*8
	Stepper.SetLimitMax(1, 32000);  // 4000*8
	Stepper.SetLimitMax(2, 8000);   // 100*8

	Stepper.SetJerkSpeed(0, 4000);  // 500 * 8?
	Stepper.SetJerkSpeed(1, 4000);
	Stepper.SetJerkSpeed(2, 4000);

	//		FILE *f = fopen("P:\\Arduino\\MyStepper.Moves\\plt\\motoguzz.plt","rt");
	FILE *f = fopen("c:\\tmp\\testc.hpgl", "rt");

	bool penIsUp = true;
	int line = 0;

	while (!feof(f))
	{
		char cmd[16];
		int x, y;
		//			int cnt=fscanf(f,"%2s%i%i;",&cmd,&x,&y);
		int cnt = fscanf(f, "%2s %i,%i;", cmd, &x, &y);

		if (cmd[0] != ';')
			line++;

#define ToHP(a) ((a*520)/974)

		if (strcmp("PD", cmd) == 0 && cnt == 3)
		{
			if (penIsUp)
			{
				Stepper.SetDefaultMaxSpeed(2000, 200, 250);
				penIsUp = false;
				//printf("Stepper.WaitBusy();\n");
			}
			Stepper.MoveAbs3(ToHP(x), ToHP(y), 0);
			//printf("Stepper.MoveAbs3(%u,%u,0,1500);\n",ToHP(x),ToHP(y),0);
		}
		else if (strcmp("PU", cmd) == 0 && cnt == 3)
		{
			if (!penIsUp)
			{
				Stepper.SetDefaultMaxSpeed(5000, 300, 350);
				penIsUp = true;
				//printf("Stepper.WaitBusy();\n");
			}
			Stepper.MoveAbs3(ToHP(x), ToHP(y), 0);
			//printf("Stepper.MoveAbs3(%u,%u,0,5000);\n",ToHP(x),ToHP(y),0);
		}

	}
	fclose(f);
	Stepper.EndTest("TR99_File.csv");
}
