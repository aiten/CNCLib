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

#include "stdafx.h"
#include <math.h>
#include "..\MsvcStepper\MsvcStepper.h"
#include "TestTools.h"
#include "..\..\..\sketch\Plotter\MyControl.h"
#include "..\..\..\sketch\Plotter\MyLcd.h"
#include "..\..\..\sketch\Plotter\PlotterControl.h"

CSerial Serial;

static void setup();
static void loop();
static void Idle();

int _tmain(int /* argc */, _TCHAR* /* argv*/ [])
{
	setup();

#pragma warning(suppress:4127)
	while (true)
	{
		loop();
	}
}

CMsvcStepper MyStepper;
class CStepper& Stepper = MyStepper;
CMyControl Control;
CPlotter Plotter;
CMyLcd Lcd;

void setup() 
{     
  Serial.begin(57600);        
  Serial.println("MyStepper is starting ...");

  // only drive stepper  
  Stepper.Init();
  pinMode(13, OUTPUT);     

  Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
  Stepper.SetLimitMax(0,6950*8);
  Stepper.SetLimitMax(1,4000*8);
  Stepper.SetLimitMax(2,100*8);

  MyStepper.InitTest();
  Serial.pIdle = Idle;
//	MyStepper._logISR = true;

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

/*
////////////////////////////////////////////////////////////

void GoToReference(axis_t axis)
{
	if (axis == Z_AXIS)
	{
		// goto max
		Stepper.MoveReference(axis, Stepper.ToReferenceId(axis, false), false, Stepper.GetDefaultVmax() / 2);
	}
	else
	{
		// goto min
		Stepper.MoveReference(axis, Stepper.ToReferenceId(axis, true), true, Stepper.GetDefaultVmax() / 2);
	}
}

////////////////////////////////////////////////////////////

void GoToReference()
{
	GoToReference(Z_AXIS);
	GoToReference(Y_AXIS);
	GoToReference(X_AXIS);
}

*/