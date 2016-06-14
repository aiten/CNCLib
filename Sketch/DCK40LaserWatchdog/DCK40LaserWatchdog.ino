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

#include "WaterFlow.h"
#include "WatchDog.h"

////////////////////////////////////////////////////////////

WaterFlow flow;
WatchDog watchDog;


#define WATCHDOG_PIN  13
#define WATCHDOG_ON LOW

#define WATERFLOW_PIN 2
#define WATERTEMP_PIN A0

#define WATCHDOG_MINFLOW  50
#define WATCHDOG_MINTEMPON  10
#define WATCHDOG_MINTEMPOFF 14
#define WATCHDOG_MAXTEMPON  512
#define WATCHDOG_MAXTEMPOFF 505

#define TESTMODE

////////////////////////////////////////////////////////////

void setup()
{

	Serial.begin(250000);

	flow.Init(WATERFLOW_PIN);
  watchDog.Init(WATCHDOG_PIN,WATCHDOG_ON);

#ifdef TESTMODE
  TestWatchDogSetup();
#endif
}

////////////////////////////////////////////////////////////

bool IsWatchDogWaterFlowOn()
{
	unsigned int avgCount = flow.AvgCount(2000);

	return avgCount > WATCHDOG_MINFLOW;
}

////////////////////////////////////////////////////////////

bool IsWatchDogTempOn()
{
	float wtemp = ReadTemp();
	static bool tempOn = false;

	if (tempOn)
		tempOn = wtemp > WATCHDOG_MINTEMPON && wtemp < WATCHDOG_MAXTEMPON;
	else
		tempOn = wtemp > WATCHDOG_MINTEMPOFF && wtemp < WATCHDOG_MAXTEMPOFF;

	return tempOn;
}

////////////////////////////////////////////////////////////

bool IsWatchDogOn()
{
	// first test all 
	// and ask all watchdogs

	bool isWaterFlowOn = IsWatchDogWaterFlowOn();
	bool isWaterTempOn = IsWatchDogTempOn();

	// now test

	return isWaterFlowOn && isWaterTempOn;
}

////////////////////////////////////////////////////////////

float ReadTemp()
{
	const unsigned char maxcount = 16;
	int wtemp = 0;
	for (int i = 0; i < maxcount; i++)
		wtemp += analogRead(WATERTEMP_PIN);

	return (float)wtemp / maxcount;
}

////////////////////////////////////////////////////////////

void loop()
{
  watchDog.OnOff(IsWatchDogOn());


#ifdef TESTMODE
  TestWatchDogLoop();
#endif
}


#ifdef TESTMODE

unsigned long blinkTime;
bool blinkOn = false;
//#define BLINK_RATE 250
#define BLINK_RATE random(5,20)
#define BLINK_PIN 12

void TestWatchDogSetup()
{ 
  pinMode(BLINK_PIN, OUTPUT);
  blinkTime = millis() + BLINK_RATE;
}

void TestWatchDogLoop()
{ 
  if (blinkTime < millis())
  {
    blinkTime += BLINK_RATE;
    blinkOn = !blinkOn;
    digitalWrite(BLINK_PIN, blinkOn ? HIGH : LOW);
  }

	static unsigned int lastAvgCount = 0xffff;
	unsigned int avgCount = flow.AvgCount(2000);

	if (avgCount != lastAvgCount)
	{
		lastAvgCount = avgCount;
		Serial.println(avgCount);
	}

	static float lastwtemp = 0;
	float wtemp = ReadTemp();
	if (abs(wtemp - lastwtemp) > 1)
	{
		lastwtemp = wtemp;
		Serial.println(wtemp);
	}
}
#endif

