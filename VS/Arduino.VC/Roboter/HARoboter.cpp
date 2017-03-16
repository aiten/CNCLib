// Stepper.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <math.h>
#include "..\MsvcStepper\MsvcStepper.h"
#include "TestTools.h"
#include "..\..\..\sketch\HARoboter\Global.h"

CSerial Serial;

static void setup();
static void loop();
static void Idle();

int _tmain(int argc, _TCHAR* argv[])
{
	setup();

	while (true)
	{
		loop();
	}
}

CMyCommand Command;
CMsvcStepper MyStepper;
class CStepper& Stepper = MyStepper;
CControl Control;
SSettings Settings;
//CPlotter Plotter;

void setup() 
{     
  Serial.begin(57600);        
  Serial.println("MyStepper is starting ...");

  // only drive stepper  
  Stepper.Init();
  pinMode(13, OUTPUT);     

  Stepper.SetDefaultMaxSpeed(5000, 100 , 150);
  Stepper.SetLimitMax(0,6950);
  Stepper.SetLimitMax(1,4000);
  Stepper.SetLimitMax(2,100);

  MyStepper.InitTest();
  Serial.pIdle = Idle;
//	MyStepper._logISR = true;

}

void loop() 
{
  Control.Run();
}

void drawloop()
{

}

static void Idle()
{
	if (MyStepper.IsBusy())
		MyStepper.DoISR();
}
