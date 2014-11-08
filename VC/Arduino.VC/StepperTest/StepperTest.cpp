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

#include "stdafx.h"
#include <math.h>
#include "..\MsvcStepper\MsvcStepper.h"
#include "TestTools.h"

CSerial Serial;

int FromMM(double mm)
{
	return (int) (mm * 3200);
}

#pragma warning(disable: 4127)

int _tmain(int argc, _TCHAR* argv[])
{
	argc; argv;

	CMsvcStepper Stepper;

	Serial.println("StepperTest is starting ...");

	char tmp[20];

	printf("%s\n", CSDist::ToString(0, tmp, 12));
	printf("%s\n", CSDist::ToString(1, tmp, 12));
	printf("%s\n", CSDist::ToString(-1, tmp, 12));
	printf("%s\n", CSDist::ToString(10, tmp, 12));
	printf("%s\n", CSDist::ToString(-10, tmp, 12));
	printf("%s\n", CSDist::ToString(LONG_MAX, tmp, 12));
	printf("%s\n", CSDist::ToString(LONG_MIN, tmp, 12));

	printf("%s\n", CSDist::ToString(LONG_MAX, tmp, 10));
	printf("%s\n", CSDist::ToString(LONG_MIN, tmp, 11));

	printf("%s\n", CSDist::ToString(LONG_MIN, tmp, 9));
	printf("%s\n", CSDist::ToString(LONG_MIN, tmp, 10));

	printf("%s\n",CMm1000::ToString(0,tmp,9,3));
	printf("%s\n",CMm1000::ToString(1,tmp,9,3));
	printf("%s\n",CMm1000::ToString(1000,tmp,9,3));
	printf("%s\n",CMm1000::ToString(12345,tmp,9,3));
	printf("%s\n",CMm1000::ToString(-1,tmp,9,3));
	printf("%s\n",CMm1000::ToString(-1000,tmp,9,3));
	printf("%s\n",CMm1000::ToString(-12345,tmp,9,3));

	printf("%s\n",CMm1000::ToString(0,tmp,9,4));
	printf("%s\n",CMm1000::ToString(1,tmp,9,4));
	printf("%s\n",CMm1000::ToString(1000,tmp,9,4));
	printf("%s\n",CMm1000::ToString(12345,tmp,9,4));
	printf("%s\n",CMm1000::ToString(-1,tmp,9,4));
	printf("%s\n",CMm1000::ToString(-1000,tmp,9,4));
	printf("%s\n",CMm1000::ToString(-12345,tmp,9,4));


	printf("%s\n",CMm1000::ToString(0,tmp,9,2));
	printf("%s\n",CMm1000::ToString(1,tmp,9,2));
	printf("%s\n",CMm1000::ToString(1000,tmp,9,2));
	printf("%s\n",CMm1000::ToString(12345,tmp,9,2));
	printf("%s\n",CMm1000::ToString(-1,tmp,9,2));
	printf("%s\n",CMm1000::ToString(-1000,tmp,9,2));
	printf("%s\n",CMm1000::ToString(-12345,tmp,9,2));

	printf("%s\n",CMm1000::ToString(0,tmp,9,1));
	printf("%s\n",CMm1000::ToString(1,tmp,9,1));
	printf("%s\n",CMm1000::ToString(1000,tmp,9,1));
	printf("%s\n",CMm1000::ToString(12345,tmp,9,1));
	printf("%s\n",CMm1000::ToString(-1,tmp,9,1));
	printf("%s\n",CMm1000::ToString(-1000,tmp,9,1));
	printf("%s\n",CMm1000::ToString(-12345,tmp,9,1));

	printf("%s\n",CMm1000::ToString(0,tmp,9,0));
	printf("%s\n",CMm1000::ToString(1,tmp,9,0));
	printf("%s\n",CMm1000::ToString(1000,tmp,9,0));
	printf("%s\n",CMm1000::ToString(12345,tmp,9,0));
	printf("%s\n",CMm1000::ToString(-1,tmp,9,0));
	printf("%s\n",CMm1000::ToString(-1000,tmp,9,0));
	printf("%s\n",CMm1000::ToString(-12345,tmp,9,0));

	printf("%s\n", CMm1000::ToString(LONG_MAX, tmp, 11, 3));
	printf("%s\n", CMm1000::ToString(LONG_MIN, tmp, 12, 3));

	// only drive stepper  
	Stepper.Init();
	Stepper.UseSpeedSign = true;

	for (axis_t x = 0; x < NUM_AXIS; x++)
	{
		Stepper.SetLimitMax(x, 0x100000);
	}
	Stepper.SetWaitFinishMove(false);

    //int dist2 = Stepper.GetLimitMax(2)-Stepper.GetLimitMin(2);
    //int dist0 = Stepper.GetLimitMax(0)-Stepper.GetLimitMin(0);
    //int dist1 = Stepper.GetLimitMax(1)-Stepper.GetLimitMin(1);

//    Stepper.MoveReference(2,-min(dist2,10000),10);
//    Stepper.MoveReference(0,-min(dist0,10000),10);
//    Stepper.MoveReference(1,-min(dist1,10000),10);

	//for (unsigned int x=1;x<10000;x++)
	//{

	//	unsigned int timer = Stepper.GetTimer(x,Stepper.SpeedToTimer(100));
	//	unsigned int step = Stepper.GetAccSteps(timer,Stepper.SpeedToTimer(100));
	//	unsigned int timerdouble = Stepper.GetTimer(x,timer,Stepper.SpeedToTimer(100));
	//	printf("%i;%i;%i;%i;%i\n",x,timer , step, x-step,timerdouble);
	//}

	bool alltests = false;

	if (false || alltests)
	{
		// ramp up, max, down
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
		Stepper.MoveRel(0,4000,5000);
		Stepper.EndTest("TR01_Acc5000Dec.csv");
	}

	if (false || alltests)
	{
		// ramp up, max, down
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(30000, 400 , 550);
//		Stepper.MoveRel(0,65534,25000);
		Stepper.MoveRel(0,65535,0);
		Stepper.EndTest("TR01_Acc25000Dec.csv");
	}

	if (false || alltests)
	{
		// ramp up, NO max, down
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
		Stepper.MoveRel(0,50,1000);
		Stepper.EndTest("TR02_AccCutDec.csv");
	}

	if (false || alltests)
	{
		// 3 Moves: 200(1000),300(1500),200(1000)
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(5000, 80 , 150);
		Stepper.MoveRel(0,200,1000);
		Stepper.MoveRel(0,300,1500);
		Stepper.MoveRel(0,400,800);
		Stepper.EndTest("TR03_Acc1000Acc1500Dec800Dec.csv");
	}

	if (false || alltests)
	{
		// 3 Moves: 200(1000),300(1500),200(1000)
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(5000, 80 , 150);
		Stepper.MoveRel(0,200,1000);
		Stepper.MoveRel(0,100,1500);
		Stepper.MoveRel(0,400,800);
		Stepper.EndTest("TR04_Acc1000AccCutDec800.csv");
	}

	if (false || alltests)
	{
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
		Stepper.CStepper::MoveRel(0,150,1000);
		Stepper.CStepper::MoveRel(0,350,2000);
		Stepper.CStepper::MoveRel(0,450,3000);
		Stepper.CStepper::MoveRel(0,700,4000);
		Stepper.CStepper::MoveRel(0,950,5000);
		Stepper.CStepper::MoveRel(0,700,4000);
		Stepper.CStepper::MoveRel(0,450,3000);
		Stepper.CStepper::MoveRel(0,350,2000);
		Stepper.CStepper::MoveRel(0,150,1000);
		Stepper.EndTest("TR05_MergeRamp.csv");
	}

	if (false || alltests)
	{
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
		Stepper.CStepper::MoveRel(0,3000,5000);
		Stepper.CStepper::MoveRel(0,200,2000);
		Stepper.CStepper::MoveRel(0,3000,4800);
		Stepper.EndTest("TR06_Acc5000DecCutAcc4800Dec.csv");
	}

	if (false || alltests)
	{
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
		Stepper.CStepper::MoveRel(0,1000,1000);
		Stepper.CStepper::MoveRel(0,-1000,1000);
		Stepper.CStepper::MoveRel(0,1000,2000);
		Stepper.CStepper::MoveRel(0,-1000,2000);
		Stepper.EndTest("TR07_UpDown.csv");
	}

	if (false || alltests)
	{
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
		Stepper.CStepper::MoveRel(0,300,1000);
		Stepper.CStepper::MoveRel(0,200,2000);
		Stepper.CStepper::MoveRel(0,1000,3000);
		Stepper.CStepper::MoveRel(0,2500,5000);
		Stepper.CStepper::MoveRel(0,300,500);
		Stepper.CStepper::MoveRel(0,550,5000);
		Stepper.EndTest("TR08_StepUp.csv");
	}

	if (false || alltests)
	{
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
		Stepper.MSCInfo = "MoveRel(0,100,3000)";
		Stepper.CStepper::MoveRel(0,100,3000);
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


	if (false || alltests)
	{
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
		Stepper.CStepper::MoveRel(0,2500,5000);
		Stepper.CStepper::MoveRel(0,100,3000);
		Stepper.CStepper::MoveRel(0,75,2500);
		Stepper.CStepper::MoveRel(0,50,2000);
		Stepper.EndTest("TR10_BreakDown.csv");
	}

	if (false || alltests)
	{
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
		Stepper.CStepper::MoveRel(0,2500,5000);
		Stepper.CStepper::MoveRel(0,100,3000);
		Stepper.CStepper::MoveRel(0,75,2500);
		Stepper.CStepper::MoveRel(0,50,2000);
		Stepper.CStepper::MoveRel(0,100,300);
		Stepper.EndTest("TR11_BreakDownPause.csv");
	}

	if (false || alltests)
	{
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
		Stepper.CStepper::MoveRel(0,2500,5000);
		Stepper.CStepper::MoveRel(0,100,3000);
		Stepper.CStepper::MoveRel(0,75,2500);
		Stepper.CStepper::MoveRel(0,50,2000);
		Stepper.CStepper::MoveRel(0,100,300);
		Stepper.CStepper::MoveRel(0,5000,3000);
		Stepper.EndTest("TR12_BreakDownDelay.csv");
	}


	if (false || alltests)
	{
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
		Stepper.MoveRel3(3000,1000,50,5000);
		Stepper.MoveRel3(1000,1000,50,3000);
		Stepper.MoveRel3(0000,1000,50,1000);
		Stepper.MoveRel3(1000,1000,50,1000);
		Stepper.EndTest("TR13_JunctionSpeedSameDirection.csv");
	}

	if (false || alltests)
	{
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
		Stepper.MoveRel3(1000,900,50,5000);
		Stepper.MoveRel3(1000,-900,-50,5000);
		Stepper.EndTest("TR14_JunctionSpeedDifferentDirection.csv");
	}

	if (false || alltests)
	{
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
		Stepper.MoveRel3(1000,100,0,5000);
		Stepper.MoveRel3(1000,900,500,5000);
		Stepper.EndTest("TR15_JunctionYLessSpeed.csv");
	}


	if (false || alltests)
	{
		Stepper.InitTest("TR16_Circle.csv");
		Stepper.SetDefaultMaxSpeed(20000, 350 , 350);
		Stepper.SetJerkSpeed(X_AXIS, 1000);
		Stepper.SetJerkSpeed(Y_AXIS, 1000);
		Stepper.SetJerkSpeed(Z_AXIS, 1000);
		Stepper.SetJerkSpeed(A_AXIS, 1000);
		Stepper.SetJerkSpeed(B_AXIS, 1000);
		double r_mm = 40.0;
		int r = FromMM(r_mm);
		int x=r;
		int y=r;
		int n = (int) (2.0 * r_mm * M_PI * 3 + 72); // 2 * r * 3 * 3 + 72;			// 2*r*PI*3 + 72) => r must be mm;
		Polygon(Stepper,x,y,r,n,0,10000);
		Stepper.EndTest();
	}

	if (false || alltests)
	{
		Stepper.InitTest();
		  int count=0;
		  Stepper.CStepper::MoveRel(0,300,1000);count+=300;
		  Stepper.CStepper::MoveRel(0,800,2000);count+=800;
		  Stepper.CStepper::MoveRel(0,1500,3000);count+=1500;
		  Stepper.CStepper::MoveRel(0,3500,5000);count+=3500;
		  Stepper.CStepper::MoveRel(0,300,500);count+=300;
		  Stepper.CStepper::MoveRel(0,550,5000);count+=550;
		Stepper.EndTest("TR17_X.csv");
	}

	if (false || alltests)
	{
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
		Stepper.MoveRel3(4000,0,0,5000);
		Stepper.MoveRel3(100,0,0,4000);
		Stepper.EndTest("TR18_LastMoveTo0.csv");
	}

	if (false || alltests)
	{
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(3000, 100 , 150);
		Stepper.SetJerkSpeed(0,300);
		Stepper.SetJerkSpeed(1,300);
		Stepper.SetJerkSpeed(2,300);

		Stepper.MoveRel3(1000,1000,0,2000);	// max speed
		Stepper.MoveRel3(1000,850,0,2000);	// -v150 => OK
		Stepper.MoveRel3(1000,1000,0,2000);	// other direction OK TO
		Stepper.MoveRel3(1000,710,0,2000);	// -v580 => fail
		Stepper.MoveRel3(1000,1000,0,2000);	// other direction
		Stepper.EndTest("TR19_JerkSameDirection.csv");
	}

	if (false || alltests)
	{
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(3000, 100 , 150);
		Stepper.SetJerkSpeed(0,300);
		Stepper.SetJerkSpeed(1,300);
		Stepper.SetJerkSpeed(2,300);

		Stepper.MoveRel3(1000,100,0,2000);	// max speed
		Stepper.MoveRel3(1000,-50,0,2000);	// -vdiff => OK
		Stepper.MoveRel3(1000,100,0,2000);	// other direction OK TO

		Stepper.MoveRel3(1000,-150,0,2000);	// -vdiff => fail
		Stepper.MoveRel3(1000,100,0,2000);	// other direction OK TO


		Stepper.EndTest("TR20_JerkSameDifferentDirection.csv");
	}

	if (false || alltests)
	{
		// ramp up, max, down
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(100, 200, 250);
		Stepper.MoveRel(0, 10000, 0);
		Stepper.EndTest("TR21_LongSlow.csv");
	}

	if (false || alltests)
	{
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(65535,400,700);
		Stepper.MoveRelEx(16000, 0, 25000, -1);
		Stepper.MoveRelEx(32000, 0, 50000, -1);
		Stepper.MoveRelEx(47000, 0, 100000, -1);
		Stepper.MoveRelEx(65000, 0, 100000, -1);
		Stepper.EndTest("TR22_VeryFast.csv");
	}

	if (false || alltests)
	{
		Stepper.InitTest();
		Stepper.SetMaxSpeed(1,500);
		Stepper.SetMaxSpeed(2,400);
		Stepper.MoveRelEx(0, 0, 10000, 1, 9000, 2, 9500, -1);
		Stepper.EndTest("TR23_SetMaxAxixSpeed.csv");
	}

	if (false || alltests)
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
		for (udist_t x=0;x<1000;x++)
			for (udist_t y=0;y<x;y++)
			{
				Stepper.InitTest();
				Stepper.SetDefaultMaxSpeed(65535, 400, 700);
				Stepper.MoveRelEx(SPEED_MULTIPLIER_3+1, 0, x, 1, y, -1);
				//Stepper.MoveRelEx(47000, 0, x, 1, y, -1);
				Stepper.EndTest("TR24_DiffMultiplier#x.csv");
				if (Stepper.GetCurrentPosition(0)!=x ||	Stepper.GetCurrentPosition(1)!=y)
				{
					*((int *)NULL) = 1021312;	//Abort
					//MessageBox(_T("Fehler"));
				}
			}
	}

	if (false || alltests)
	{
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
		Stepper.CStepper::MoveRel(0,2500,5000);
		Stepper.CStepper::Wait(0);
		Stepper.CStepper::MoveRel(0,100,3000);
		Stepper.CStepper::MoveRel(0,75,2500);
		Stepper.CStepper::MoveRel(0,50,2000);
		Stepper.CStepper::MoveRel(0,100,300);
		Stepper.CStepper::MoveRel(0,5000,3000);
		Stepper.CStepper::Wait(100);
		Stepper.CStepper::MoveRel(0,5000,3000);
		Stepper.EndTest("TR24_Wait.csv");
	}

	if (false || alltests)
	{
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
		Stepper.CStepper::MoveRel(0,2500,1);
		Stepper.CStepper::MoveRel(0,1,2);
		Stepper.CStepper::MoveRel(0,100,3);
		Stepper.EndTest("TR25_VerySlow.csv");
	}

	if (true || alltests)
	{
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(5000, 100, 150);
		Stepper.MoveRelEx(47000, 0, 2500, 1, 1000, 2 ,500, -1);
		Stepper.MoveRelEx(47000, 0, 2500, 1, -1000, 2 ,500, -1);
		Stepper.CStepper::StopMove(75);
		Stepper.EndTest("TR25_StopMove.csv");
	}


	if (false)
	{
		Stepper.InitTest();
		Stepper.SetDefaultMaxSpeed(1500, 100 , 150);
		Stepper.SetJerkSpeed(0,800);
		Stepper.SetJerkSpeed(1,800);
		Stepper.SetJerkSpeed(2,800);

		Stepper.SetDefaultMaxSpeed(8000, 500 , 500);
		Stepper.SetLimitMax(0, 55600);  // 6950*8
		Stepper.SetLimitMax(1, 32000);  // 4000*8
		Stepper.SetLimitMax(2, 8000);   // 100*8

		Stepper.SetJerkSpeed(0, 4000);  // 500 * 8?
		Stepper.SetJerkSpeed(1, 4000);
		Stepper.SetJerkSpeed(2, 4000);

//		FILE *f = fopen("P:\\Arduino\\MyStepper.Moves\\plt\\motoguzz.plt","rt");
		FILE *f = fopen("c:\\tmp\\testc.hpgl","rt");

		bool penIsUp=true;
		int line=0;

		while (!feof(f))
		{
			char cmd[16];
			int x,y;
//			int cnt=fscanf(f,"%2s%i%i;",&cmd,&x,&y);
			int cnt=fscanf(f,"%2s %i,%i;",cmd,&x,&y);

			if (cmd[0]!=';')
				line++;

#define ToHP(a) ((a*520)/974)

			if (strcmp("PD",cmd)==0 && cnt == 3)
			{
				if (penIsUp)
				{
					Stepper.SetDefaultMaxSpeed(2000,200,250);
					penIsUp=false;
//printf("Stepper.WaitBusy();\n");
				}
				Stepper.MoveAbs3(ToHP(x),ToHP(y),0);
//printf("Stepper.MoveAbs3(%u,%u,0,1500);\n",ToHP(x),ToHP(y),0);
			}
			else if (strcmp("PU",cmd)==0 && cnt == 3)
			{
				if (!penIsUp)
				{
					Stepper.SetDefaultMaxSpeed(5000,300,350);
					penIsUp=true;
//printf("Stepper.WaitBusy();\n");
				}
				Stepper.MoveAbs3(ToHP(x),ToHP(y),0);
//printf("Stepper.MoveAbs3(%u,%u,0,5000);\n",ToHP(x),ToHP(y),0);
			}

		}
		fclose(f);
		Stepper.EndTest("TR99_File.csv");
	}

	return 0;
}
