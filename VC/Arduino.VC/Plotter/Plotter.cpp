// Plotter.cpp : Defines the entry point for the console application.
//

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