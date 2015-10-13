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
#include <math.h>

#include "..\MsvcStepper\MsvcStepper.h"
#include "TestTools.h"
#include "..\..\..\sketch\CNCShield\MyControl.h"

CSerial Serial;

static void setup();
static void loop();
static void Idle();

CMsvcStepper MyStepper;
class CStepper& Stepper = MyStepper;

int _tmain(int /* argc */, _TCHAR* /* argv */ [])
{
	setup();

	while (!CGCodeParserBase::_exit)
	{
		loop();
	}

	MyStepper.EndTest();
}

void setup() 
{     
	MyStepper.DelayOptimization = false;
	MyStepper.UseSpeedSign = true;
	MyStepper.CacheSize = 50000;
	MyStepper.InitTest("CNCShield.csv");
	Serial.SetIdle(Idle);
}

void loop() 
{
  Control.Run();
}

static void Idle()
{
	if (MyStepper.IsBusy())
		MyStepper.DoISR();
}
