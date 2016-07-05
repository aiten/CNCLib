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
{ 0, 705.051087182432 }, // 0
{ 1, 351.963930354562 }, // 0.286164794080624
{ 4, 239.299124763951 }, // 0.282141433631067
{ 10, 184.540907353204 }, // 0.275657534277222
{ 25, 139.965638569205 }, // 0.259500094580318
{ 50, 111.091101597831 }, // 0.233539623708403
{ 76, 95.1209269091311 }, // 0.207926035800247
{ 114, 80.4055700858972 }, // 0.173013904708565
{ 138, 73.6444920311915 }, // 0.152604858560589
{ 166, 67.1643791931888 }, // 0.130837577431401
{ 197, 61.1715179927255 }, // 0.10859582641703
{ 231, 55.5723098278272 }, // 0.0867249796485391
{ 267, 50.4196611531001 }, // 0.0662745260662323
{ 304, 45.7190237690924 }, // 0.05
{ 342, 41.3501275584711 }, // 0.05
{ 386, 36.7163169579019 }, // 0.05
{ 440, 31.4705353903448 }, // 0.05
{ 511, 25.0440038955068 }, // 0
{ 694, 9.09811297347608 }, // 0.05
{ 756, 3.28337003919228 }, // 0.0598368401509873
{ 813, -2.63889795857875 }, // 0.0930565729615026
{ 863, -8.64589732076234 }, // 0.128354120173919
{ 905, -14.7371105639454 }, // 0.162667112120678
{ 938, -20.7451829044707 }, // 0.193280281711455
{ 964, -26.9201033647565 }, // 0.218997257895142
{ 996, -38.8159061570892 }, // 0.254689684321797
{ 1017, -58.256730491718 }, // 0.279140539878
{ 1020, -66.0977536764764 }, // 0.283144899766378
{ 1023, -273.15 }, // 0
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

