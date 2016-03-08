////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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
#include "..\..\..\sketch\ProxxonMF70\MyControl.h"
#include "..\..\..\sketch\ProxxonMF70\MyLcd.h"

#include <SPI.h>
#include <SD.h>

CSerial Serial;
SDClass SD;

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
	MyStepper.CacheSize = 100000;
	MyStepper.InitTest("ProxxonMF70.csv");
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
