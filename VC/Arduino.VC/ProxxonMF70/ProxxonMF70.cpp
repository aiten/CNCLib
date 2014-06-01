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

	while (!CHelpParser::_exit)
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
	  Serial.pIdle = Idle;
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
