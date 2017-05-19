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
#include <RotaryButton.h>
#include "TestTools.h"

#include "CppUnitTest.h"

////////////////////////////////////////////////////////

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace StepperSystemTest
{
	TEST_CLASS(CStepperTest)
	{
	public:

		CMsvcStepper Stepper;

		bool overrideTestOK = false;

		int FromMM(double mm)
		{
			return (int)(mm * 3200);
		}

		char TestResultOKDir[_MAX_PATH] = { 0 };
		char TestResultDir[_MAX_PATH] = { 0 };

		char TestResultOKFile[_MAX_PATH] = { 0 };
		char TestResultFile[_MAX_PATH] = { 0 };

		char* AddFileName(char*dest, const char* start, const char*filename)
		{
			strcpy(dest, start);
			strcat(dest, "Test_");
			strcat(dest, filename);

			return dest;
		}

		char* GetResultFileName(const char*filename) { return AddFileName(TestResultFile, TestResultDir, filename); }
		char* GetResultOkFileName(const char*filename) { return AddFileName(TestResultOKFile, TestResultOKDir, filename); };

		void CreateTestFile(const char* filename)
		{
			Stepper.EndTest(GetResultFileName(filename));
		}

		void Init(char*exename)
		{
			::GetTempPathA(_MAX_PATH, TestResultDir);

			strcpy_s(TestResultOKDir, exename);
			*(strrchr(TestResultOKDir, '\\') + 0) = 0;
			*(strrchr(TestResultOKDir, '\\') + 1) = 0;
			strcat_s(TestResultOKDir, "TestResult\\");
		}

		void AssertFile(const char* filename)
		{
			const char* pathname_src = GetResultFileName(filename);
			const char* pathname_dest = GetResultOkFileName(filename);

			if (overrideTestOK)		// create Test result file as OK
			{
				Stepper.EndTest(pathname_dest);
				return;
			}

			Stepper.EndTest(pathname_src);

			FILE* fsrc;
			FILE* fdest;

			fsrc = fopen(pathname_src, "rt");
			fdest = fopen(pathname_dest, "rt");

			Assert::IsTrue(fsrc != NULL);
			Assert::IsTrue(fdest != NULL);

			char lines[512];
			char lined[512];

			char* src = fgets(lines, sizeof(lines), fsrc);

			while (src)
			{
				char* dest = fgets(lined, sizeof(lined), fdest);
				Assert::IsTrue(dest != NULL);
				if (strcmp(src, dest) != 0)
				{
					Assert::Fail();
				}

				//Assert::AreSame(src, dest);

				src = fgets(lines, sizeof(lines), fsrc);
			}

			fclose(fsrc);
			fclose(fdest);

		}

		void AssertMove(mdist_t steps, CMsvcStepper::SMovementX mv)
		{
			Assert::IsTrue(mv.mv.IsActiveMove());
			Assert::AreEqual((long) steps, (long) mv.mv.GetSteps());
		}

		void AssertWait(mdist_t steps, CMsvcStepper::SMovementX mv)
		{
			Assert::IsTrue(mv.mv.IsActiveWait());
			Assert::AreEqual((long)steps, (long)mv.mv.GetSteps());
		}

		CStepperTest()
		{
			InitializeTestObject();
		}

		void InitializeTestObject()
		{
			char modulefile[_MAX_PATH];
			auto hInst = GetModuleHandle(L"StepperSystem.Test.dll");
			GetModuleFileNameA(hInst, modulefile, sizeof(modulefile));
			Init(modulefile);

			Serial.println("StepperTest is starting ...");

			// only drive stepper  
			Stepper.Init();
			Stepper.UseSpeedSign = true;

			for (axis_t x = 0; x < NUM_AXIS; x++)
			{
				Stepper.SetLimitMax(x, 0x100000);
			}
			Stepper.SetWaitFinishMove(false);
		}

		TEST_METHOD(StepperTest)
		{
/*
			TestAcc5000Dec();
			TestAcc25000Dec();
			TestAccCutDec();
			TestAcc1000Acc1500Dec800Dec();
			TestAcc1000AccCutDec800();
			TestMergeRamp();
			TestAcc5000DecCutAcc4800Dec();
			TestUpDown();
			TestStepUp();
			TestSpeedUp();
			TestBreakDown();
			TestBreakDownPause();
			TestBreakDownDelay();
			TestJunctionSpeedSameDirection();
			TestJunctionSpeedDifferentDirection();
			TestJunctionYLessSpeed();
			TestCircle();
			TestX();
			TestLastMoveTo0();
			TestJerkSameDirection();
			TestJerkSameDifferentDirection();
			TestLongSlow();
			TestVeryFast();
			TestSetMaxAxixSpeed();
			TestWait();
			TestVerySlow();
			TestStopMove();
			TestWaitHold();
			TestPause1();
			TestPause2();
			TestPause3();
			TestPause4();

			TestIo();
*/
/*
			// very long running!!!!
			TestDiffMultiplierAbs();
			TestDiffMultiplierLoop();

			TestFile();
*/
		}

		TEST_METHOD(StepperAcc5000Dec)
		{
			// ramp up, max, down
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(5000, 100, 150);
			Stepper.MoveRel(0, 4000, 5000);
			AssertFile("Acc5000Dec.csv");
		}

		TEST_METHOD(StepperAcc25000Dec)
		{
			// ramp up, max, down
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(30000, 400, 550);
			//		Stepper.MoveRel(0,65534,25000);
			Stepper.MoveRel(0, 65535, 0);
			AssertFile("Acc25000Dec.csv");
		}

		TEST_METHOD(StepperAccCutDec)
		{
			// ramp up, NO max, down
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(5000, 100, 150);
			Stepper.MoveRel(0, 50, 1000);
			AssertFile("AccCutDec.csv");
		}

		TEST_METHOD(StepperAcc1000Acc1500Dec800Dec)
		{
			// 3 Moves: 200(1000),300(1500),200(1000)
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(5000, 80, 150);
			Stepper.MoveRel(0, 200, 1000);
			Stepper.MoveRel(0, 300, 1500);
			Stepper.MoveRel(0, 400, 800);
			AssertFile("Acc1000Acc1500Dec800Dec.csv");
		}

		TEST_METHOD(StepperAcc1000AccCutDec800)
		{
			// 3 Moves: 200(1000),300(1500),200(1000)
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(5000, 80, 150);
			Stepper.MoveRel(0, 200, 1000);
			Stepper.MoveRel(0, 100, 1500);
			Stepper.MoveRel(0, 400, 800);
			AssertFile("Acc1000AccCutDec800.csv");
		}

		TEST_METHOD(StepperMergeRamp)
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
			AssertFile("MergeRamp.csv");
		}

		TEST_METHOD(StepperAcc5000DecCutAcc4800Dec)
		{
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(5000, 100, 150);
			Stepper.CStepper::MoveRel(0, 3000, 5000);
			Stepper.CStepper::MoveRel(0, 200, 2000);
			Stepper.CStepper::MoveRel(0, 3000, 4800);
			AssertFile("Acc5000DecCutAcc4800Dec.csv");
		}

		TEST_METHOD(StepperUpDown)
		{
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(5000, 100, 150);
			Stepper.CStepper::MoveRel(0, 1000, 1000);
			Stepper.CStepper::MoveRel(0, -1000, 1000);
			Stepper.CStepper::MoveRel(0, 1000, 2000);
			Stepper.CStepper::MoveRel(0, -1000, 2000);
			AssertFile("UpDown.csv");
		}

		TEST_METHOD(StepperStepUp)
		{
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(5000, 100, 150);
			Stepper.CStepper::MoveRel(0, 300, 1000);
			Stepper.CStepper::MoveRel(0, 200, 2000);
			Stepper.CStepper::MoveRel(0, 1000, 3000);
			Stepper.CStepper::MoveRel(0, 2500, 5000);
			Stepper.CStepper::MoveRel(0, 300, 500);
			Stepper.CStepper::MoveRel(0, 550, 5000);
			AssertFile("StepUp.csv");
		}

		TEST_METHOD(StepperSpeedUp)
		{
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(5000, 100, 150);
			Stepper.MSCInfo = "1MoveRel(0,100,3000)";
			Stepper.CStepper::MoveRel(0, 100, 3000);
			Stepper.MSCInfo = "2MoveRel(0,100,3000)";
			Stepper.CStepper::MoveRel(0, 100, 3000);
			Stepper.MSCInfo = "3MoveRel(0,100,3000)";
			Stepper.CStepper::MoveRel(0, 100, 3000);
			Stepper.MSCInfo = "4MoveRel(0,100,3000)";
			Stepper.CStepper::MoveRel(0, 100, 3000);
			Stepper.MSCInfo = "5MoveRel(0,100,3000)";
			Stepper.CStepper::MoveRel(0, 100, 3000);
			Stepper.MSCInfo = "6MoveRel(0,5000,3000)";
			Stepper.CStepper::MoveRel(0, 5000, 3000);
			AssertFile("SpeedUp.csv");
		}

		TEST_METHOD(StepperBreakDown)
		{
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(5000, 100, 150);
			Stepper.CStepper::MoveRel(0, 2500, 5000);
			Stepper.CStepper::MoveRel(0, 100, 3000);
			Stepper.CStepper::MoveRel(0, 75, 2500);
			Stepper.CStepper::MoveRel(0, 50, 2000);
			AssertFile("BreakDown.csv");
		}

		TEST_METHOD(StepperBreakDownPause)
		{
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(5000, 100, 150);
			Stepper.CStepper::MoveRel(0, 2500, 5000);
			Stepper.CStepper::MoveRel(0, 100, 3000);
			Stepper.CStepper::MoveRel(0, 75, 2500);
			Stepper.CStepper::MoveRel(0, 50, 2000);
			Stepper.CStepper::MoveRel(0, 100, 300);
			AssertFile("BreakDownPause.csv");
		}

		TEST_METHOD(StepperBreakDownDelay)
		{
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(5000, 100, 150);
			Stepper.CStepper::MoveRel(0, 2500, 5000);
			Stepper.CStepper::MoveRel(0, 100, 3000);
			Stepper.CStepper::MoveRel(0, 75, 2500);
			Stepper.CStepper::MoveRel(0, 50, 2000);
			Stepper.CStepper::MoveRel(0, 100, 300);
			Stepper.CStepper::MoveRel(0, 5000, 3000);
			AssertFile("BreakDownDelay.csv");
		}

		TEST_METHOD(StepperJunctionSpeedSameDirection)
		{
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(5000, 100, 150);
			Stepper.MoveRel3(3000, 1000, 50, 5000);
			Stepper.MoveRel3(1000, 1000, 50, 3000);
			Stepper.MoveRel3(0000, 1000, 50, 1000);
			Stepper.MoveRel3(1000, 1000, 50, 1000);
			AssertFile("JunctionSpeedSameDirection.csv");
		}

		TEST_METHOD(StepperJunctionSpeedDifferentDirection)
		{
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(5000, 100, 150);
			Stepper.MoveRel3(1000, 900, 50, 5000);
			Stepper.MoveRel3(1000, -900, -50, 5000);
			AssertFile("JunctionSpeedDifferentDirection.csv");
		}

		TEST_METHOD(StepperJunctionYLessSpeed)
		{
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(5000, 100, 150);
			Stepper.MoveRel3(1000, 100, 0, 5000);
			Stepper.MoveRel3(1000, 900, 500, 5000);
			AssertFile("JunctionYLessSpeed.csv");
		}

		TEST_METHOD(StepperCircle)
		{
			Stepper.InitTest(GetResultFileName("Circle.csv"));
			Stepper.SetDefaultMaxSpeed(20000, 350, 350);
			Stepper.SetJerkSpeed(X_AXIS, 1000);
			Stepper.SetJerkSpeed(Y_AXIS, 1000);
			Stepper.SetJerkSpeed(Z_AXIS, 1000);
			Stepper.SetJerkSpeed(A_AXIS, 1000);
			Stepper.SetJerkSpeed(B_AXIS, 1000);
			double r_mm = 40.0;
			mdist_t r = (mdist_t)FromMM(r_mm);
			mdist_t x = r;
			mdist_t y = r;
			int n = (int)(2.0 * r_mm * M_PI * 3 + 72); // 2 * r * 3 * 3 + 72;			// 2*r*PI*3 + 72) => r must be mm;
			Polygon(Stepper, x, y, r, n, 0, 10000);
			Stepper.EndTest();
		}

		TEST_METHOD(StepperX)
		{
			Stepper.InitTest();
			int count = 0;
			Stepper.CStepper::MoveRel(0, 300, 1000); count += 300;
			Stepper.CStepper::MoveRel(0, 800, 2000); count += 800;
			Stepper.CStepper::MoveRel(0, 1500, 3000); count += 1500;
			Stepper.CStepper::MoveRel(0, 3500, 5000); count += 3500;
			Stepper.CStepper::MoveRel(0, 300, 500); count += 300;
			Stepper.CStepper::MoveRel(0, 550, 5000); count += 550;
			CreateTestFile("X.csv");
		}

		TEST_METHOD(StepperLastMoveTo0)
		{
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(5000, 100, 150);
			Stepper.MoveRel3(4000, 0, 0, 5000);
			Stepper.MoveRel3(100, 0, 0, 4000);
			AssertFile("LastMoveTo0.csv");
		}

		TEST_METHOD(StepperJerkSameDirection)
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
			AssertFile("JerkSameDirection.csv");
		}

		TEST_METHOD(StepperJerkSameDifferentDirection)
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


			AssertFile("JerkSameDifferentDirection.csv");
		}

		TEST_METHOD(StepperLongSlow)
		{
			// ramp up, max, down
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(100, 200, 250);
			Stepper.MoveRel(0, 10000, 0);
			AssertFile("LongSlow.csv");
		}

		TEST_METHOD(StepperVeryFast)
		{
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(65535, 400, 700);
			Stepper.MoveRelEx(16000, 0, 25000, -1);
			Stepper.MoveRelEx(32000, 0, 50000, -1);
			Stepper.MoveRelEx(47000, 0, 100000, -1);
			Stepper.MoveRelEx(65000, 0, 100000, -1);
			CreateTestFile("VeryFast.csv");
		}

		TEST_METHOD(StepperSetMaxAxixSpeed)
		{
			Stepper.InitTest();
			Stepper.SetMaxSpeed(1, 500);
			Stepper.SetMaxSpeed(2, 400);
			Stepper.MoveRelEx(0, 0, 100, 1, 90, 2, 95, -1);
			AssertFile("SetMaxAxixSpeed.csv");
		}

		TEST_METHOD(StepperDiffMultiplierAbs)
		{
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(65535, 400, 700);
			Stepper.MoveRelEx(47000, 0, 65535, 1, 50000, 2, 0, 3, 0, 4, 0, -1);
			Stepper.MoveRelEx(47000, 0, 65535, 1, 50000, 2, 32000, 3, 22000, 4, 13000, -1);
			Stepper.MoveRelEx(47000, 0, 65530, 1, 50002, 2, 32000, 3, 22001, 4, 13001, -1);
			CreateTestFile("DiffMultiplierAbs1.csv");

			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(65535, 400, 700);
			Stepper.MoveRelEx(47000, 0, 2, -1);
			CreateTestFile("DiffMultiplierAbs2.csv");
		}

		TEST_METHOD(StepperDiffMultiplierLoop)
		{
			for (udist_t x = 0; x < 300; x++)
				for (udist_t y = 0; y < x; y++)
				{
					Stepper.InitTest();
					Stepper.SetDefaultMaxSpeed(65535, 400, 700);
					Stepper.MoveRelEx(SPEED_MULTIPLIER_3 + 1, 0, x, 1, y, -1);
					//Stepper.MoveRelEx(47000, 0, x, 1, y, -1);
					CreateTestFile("DiffMultiplierLoop.csv");
					Assert::AreEqual(x, Stepper.GetCurrentPosition(0));
					Assert::AreEqual(y, Stepper.GetCurrentPosition(1));
				}
		}

		TEST_METHOD(StepperWait)
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
			CreateTestFile("Wait.csv");
		}

		TEST_METHOD(StepperVerySlow)
		{
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(5000, 100, 150);
			Stepper.CStepper::MoveRel(0, 2500, 1);
			Stepper.CStepper::MoveRel(0, 1, 2);
			Stepper.CStepper::MoveRel(0, 100, 3);
			CreateTestFile("VerySlow.csv");
		}

		TEST_METHOD(StepperStopMove)
		{
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(5000, 100, 150);
			Stepper.MoveRelEx(47000, 0, 2500, 1, 1000, 2, 500, -1);
			Stepper.MoveRelEx(47000, 0, 2500, 1, -1000, 2, 500, -1);
			Stepper.CStepper::StopMove(75);
			CreateTestFile("StopMove.csv");
		}

		TEST_METHOD(StepperWaitHold)
		{
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(5000, 100, 150);
			Stepper.CStepper::MoveRel(0, 2500, 5000);
			Stepper.CStepper::SetWaitConditional(true);
			Stepper.CStepper::WaitConditional(0);				// timeout 0 => no wait
			Stepper.CStepper::MoveRel(0, 100, 3000);
			Stepper.CStepper::WaitConditional(100);
			Stepper.CStepper::MoveRel(0, 5000, 3000);
			CreateTestFile("WaitHold.csv");
		}

		TEST_METHOD(StepperPause1)
		{
			Stepper.InitTest();

			Stepper.CStepper::MoveRel(0, 2500, 5000);
			Stepper.CStepper::MoveRel(0, -100, 3000);		// pause here
			Stepper.CStepper::MoveRel(0, 5000, 3000);
			Stepper.OptimizeMovementQueue(true);			// calc ramp
			Stepper.CStepper::PauseMove();

			Assert::AreEqual((uint8_t) 4, Stepper.GetMovementCount());
			AssertMove(2500, Stepper.GetMovement(0));
			AssertWait(65535, Stepper.GetMovement(1));
			AssertMove(100, Stepper.GetMovement(2));
			AssertMove(5000, Stepper.GetMovement(3));

			CreateTestFile("TestPause1.csv");
		}

		TEST_METHOD(StepperPause2)
		{
			Stepper.InitTest();

			Stepper.CStepper::MoveRel(0, 2500, 5000);
			Stepper.CStepper::MoveRel(0, 1000, 3000);
			Stepper.CStepper::MoveRel(0, -1500, 3000);		// pause should be created here

			Stepper.OptimizeMovementQueue(true);			// calc ramp
			Stepper.CStepper::PauseMove();

			Assert::AreEqual((uint8_t) 4, Stepper.GetMovementCount());
			AssertMove(2500, Stepper.GetMovement(0));
			AssertMove(1000, Stepper.GetMovement(1));
			AssertWait(65535, Stepper.GetMovement(2));
			AssertMove(1500, Stepper.GetMovement(3));

			CreateTestFile("TestPausea2.csv");
		}

		TEST_METHOD(StepperPause3)
		{
			Stepper.InitTest();

			Stepper.MoveRel3(10000, 10000, 10000, 5000);
			Stepper.MoveRel3(10000, -1000, 10000, 5000);		// jerk break => pause here
			Stepper.MoveRel3(10000, 10000, 1000, 5000);

			Stepper.OptimizeMovementQueue(true);			// calc ramp
			Stepper.CStepper::PauseMove();

			WriteStepperTestMovement();

			Assert::AreEqual((uint8_t)4, Stepper.GetMovementCount());
			AssertMove(10000, Stepper.GetMovement(0));
			AssertWait(65535, Stepper.GetMovement(1));
			AssertMove(10000, Stepper.GetMovement(2));
			AssertMove(10000, Stepper.GetMovement(3));

			CreateTestFile("TestPausea3.csv");
		}

		TEST_METHOD(StepperPause4)
		{
			Stepper.InitTest();

			Stepper.MoveRel3(10000, 10000, 10000, 5000);
			Stepper.MoveRel3(10000, 9000, 10000, 5000);
			Stepper.MoveRel3(10000, -9000, 10000, 5000);		// jerk break => pause here

			Stepper.OptimizeMovementQueue(true);			// calc ramp
			Stepper.CStepper::PauseMove();

			Assert::AreEqual((uint8_t)4, Stepper.GetMovementCount());
			AssertMove(10000, Stepper.GetMovement(0));
			AssertMove(10000, Stepper.GetMovement(1));
			AssertWait(65535, Stepper.GetMovement(2));
			AssertMove(10000, Stepper.GetMovement(3));

			CreateTestFile("TestPausea4.csv");
		}

		TEST_METHOD(StepperIo)
		{
			//Like Merge but with Io
			Stepper.InitTest();
			Stepper.SetDefaultMaxSpeed(5000, 100, 150);
			Stepper.CStepper::IoControl(0, 1);
			Stepper.CStepper::IoControl(0, 1);
			Stepper.CStepper::MoveRel(0, 150, 1000);
			Stepper.CStepper::IoControl(0, 1);
			Stepper.CStepper::IoControl(0, 1);
			Stepper.CStepper::MoveRel(0, 350, 2000);
			Stepper.CStepper::IoControl(0, 1);
			Stepper.CStepper::IoControl(0, 1);
			Stepper.CStepper::MoveRel(0, 450, 3000);
			Stepper.CStepper::IoControl(0, 1);
			Stepper.CStepper::IoControl(0, 1);
			Stepper.CStepper::MoveRel(0, 700, 4000);
			Stepper.CStepper::IoControl(0, 1);
			Stepper.CStepper::IoControl(0, 1);
			Stepper.CStepper::MoveRel(0, 950, 5000);
			Stepper.CStepper::IoControl(0, 1);
			Stepper.CStepper::IoControl(0, 1);
			Stepper.CStepper::MoveRel(0, 700, 4000);
			Stepper.CStepper::IoControl(0, 1);
			Stepper.CStepper::IoControl(0, 1);
			Stepper.CStepper::MoveRel(0, 450, 3000);
			Stepper.CStepper::IoControl(0, 1);
			Stepper.CStepper::IoControl(0, 1);
			Stepper.CStepper::MoveRel(0, 350, 2000);
			Stepper.CStepper::IoControl(0, 1);
			Stepper.CStepper::IoControl(0, 1);
			Stepper.CStepper::MoveRel(0, 150, 1000);
			Stepper.CStepper::IoControl(0, 1);
			Stepper.CStepper::IoControl(0, 1);
			AssertFile("MergeRampWithIo.csv");
		}

		void TestFile()
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
			CreateTestFile("TR99_File.csv");
		}

		void WriteStepperTestMovement()
		{
			FILE*f;
			fopen_s(&f, "c:\\tmp\\test.txt", "wt");

			fprintf(f, "\tAssert(%i, Stepper.GetMovementCount());\n", (int)Stepper.GetMovementCount());
			for (uint8_t i = 0; i < Stepper.GetMovementCount(); i++)
			{
				CMsvcStepper::SMovementX mv = Stepper.GetMovement(i);
				if (mv.mv.IsActiveMove())
				{
					fprintf(f, "\tAssertMove(%i, Stepper.GetMovement(%i));\n", (int)mv.mv.GetSteps(), (int)i);
				}
				else
				{
					fprintf(f, "\tAssertWait(%i, Stepper.GetMovement(%i));\n", (int)mv.mv.GetSteps(), (int)i);
				}
			}
			/*
				Assert(true, Stepper.GetMovement(0).mv.IsActiveMove());  Assert(2500, Stepper.GetMovement(0).mv.GetSteps());
				Assert(true, Stepper.GetMovement(1).mv.IsActiveWait());
				Assert(true, Stepper.GetMovement(2).mv.IsActiveMove());	 Assert(100, Stepper.GetMovement(0).mv.GetSteps());
				Assert(true, Stepper.GetMovement(3).mv.IsActiveMove());  Assert(5000, Stepper.GetMovement(0).mv.GetSteps());
			*/

			fclose(f);
		}

	};
}