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
#include "LinearLookup.h"

////////////////////////////////////////////////////////////

#define RELAY1_PIN	11
#define RELAY2_PIN	10

#define INPUT1_PIN	9
#define INPUT2_PIN	8
#define INPUT3_PIN	7

////////////////////////////////////////////////////////////

WaterFlow flow;
WatchDog watchDog;

#define ALIVE_PIN 13
//#define ALIVE_BLINK_RATE random(5,20)
#define ALIVE_BLINK_RATE 250

#define WATCHDOG_PIN  RELAY1_PIN
#define WATCHDOG_ON LOW

#define WATERFLOW_PIN 2
#define WATCHDOG_MINFLOW  50

#define WATERTEMP_PIN A0

#define WATCHDOG_MINTEMPON  4
#define WATCHDOG_MINTEMPOFF 5
#define WATCHDOG_MAXTEMPON  35.0
#define WATCHDOG_MAXTEMPOFF 34.5

#define WATERTEMP_OVERSAMPLING 16

CLinearLookup<float, float>::SLookupTable linear10k[] =
{
  {1, 430},
  {54, 137},
  {107, 107},
  {160, 91},
  {213, 80},
  {266, 71},
  {319, 64},
  {372, 57},
  {425, 51},
  {478, 46},
  {531, 41},
  {584, 35},
  {637, 30},
  {690, 25},
  {743, 20},
  {796, 14},
  {849, 7},
  {902, 0},
  {955, -11},
  {1008, -35}
};
CLinearLookup<float, float> temp10k(linear10k, sizeof(linear10k) / sizeof(CLinearLookup<float, float>::SLookupTable));

////////////////////////////////////////////////////////////

void setup()
{
	Serial.begin(250000);

  pinMode(ALIVE_PIN,OUTPUT);

	flow.Init(WATERFLOW_PIN);
	watchDog.Init(WATCHDOG_PIN, WATCHDOG_ON);
}

////////////////////////////////////////////////////////////

unsigned long lastBlink=0;
bool  blinkWasOn=true;

////////////////////////////////////////////////////////////

float ReadTemp()
{
	const unsigned char maxcount = WATERTEMP_OVERSAMPLING;
	int wtemp = 0;
	for (int i = 0; i < maxcount; i++)
		wtemp += analogRead(WATERTEMP_PIN);

  return temp10k.Lookup((float)wtemp / maxcount);
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

void TestWatchDogLoop();

////////////////////////////////////////////////////////////

void loop()
{
	watchDog.OnOff(IsWatchDogOn());

  if (millis()> lastBlink)
  {
    lastBlink = millis() + ALIVE_BLINK_RATE;
    blinkWasOn = !blinkWasOn;
    digitalWrite(ALIVE_PIN, blinkWasOn ? HIGH : LOW);
  }

	TestWatchDogLoop();
}

void TestWatchDogLoop()
{
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

