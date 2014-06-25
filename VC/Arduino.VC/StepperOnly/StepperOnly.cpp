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

CSerial Serial;
CMsvcStepper Stepper;

static void setup();
static void loop();

int _tmain(int /* argc */, _TCHAR* /* argv */ [])
{
	setup();
	loop();

	Stepper.EndTest("StepperOnly.csv");

	return 0;
}

// Copy From INO


void setup() 
{     
  Serial.begin(57600);        
  Serial.println("Stepper is starting ...");

  // only drive stepper  
  Stepper.Init();
  pinMode(13, OUTPUT);     

  Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
  Stepper.SetDefaultMaxSpeed(32000, 800 , 800);

  Stepper.SetLimitMax(0,6950);
  Stepper.SetLimitMax(0,65535);
  Stepper.SetLimitMax(1,4000);
  Stepper.SetLimitMax(2,100);

//    int dist2 = Stepper.GetLimitMax(2)-Stepper.GetLimitMin(2);
//    int dist0 = Stepper.GetLimitMax(0)-Stepper.GetLimitMin(0);
//    int dist1 = Stepper.GetLimitMax(1)-Stepper.GetLimitMin(1);
//    Stepper.MoveReference(2,-min(dist2,10000),10);
//    Stepper.MoveReference(0,-min(dist0,10000),10);
//    Stepper.MoveReference(1,-min(dist1,10000),10);

    Stepper.SetWaitFinishMove(false);

   Stepper.SetJerkSpeed(0,400);
   Stepper.SetJerkSpeed(1,400);
   Stepper.SetJerkSpeed(2,400);
}

void loop() 
{

	Stepper.SetLimitMax(0,400000);
	Stepper.MoveRel(0,200000,0);
	Stepper.MoveRel(0,-200000, 0);
	return;
/*
  int count = 0;

  Stepper.CStepper::MoveRel(0,3000,30000);count+=3000;
  Stepper.CStepper::MoveRel(0,8000,6000);count+=8000;
  Stepper.CStepper::MoveRel(0,15000,9000);count+=15000;
  Stepper.CStepper::MoveRel(0,20000,15000);count+=32000;
  Stepper.CStepper::MoveRel(0,3000,1500);count+=3000;
  Stepper.CStepper::MoveRel(0,5500,6000);count+=5500;

//  Stepper.WaitBusy();
  Serial.print(F("TimerISRBusy="));Serial.println(Stepper.GetTimerISRBuys());
  delay(1000);
  Stepper.CStepper::MoveAbs(0,0,10000);
//  Stepper.WaitBusy();
  delay(1000);
*/
}


void GoToReference(axis_t axis)
{
	Stepper.MoveReference(axis, Stepper.ToReferenceId(axis, true), true, Stepper.GetDefaultVmax() / 4,0,0,0);
}

////////////////////////////////////////////////////////////

void GoToReference()
{
	GoToReference(Z_AXIS);
	GoToReference(Y_AXIS);
	GoToReference(X_AXIS);
}
