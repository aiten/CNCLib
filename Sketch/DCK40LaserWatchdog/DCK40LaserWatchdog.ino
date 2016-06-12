
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

////////////////////////////////////////////////////////////

WaterFlow flow;

unsigned long blinkTime;
bool blinkOn=false;
//#define BLINK_RATE 250
#define BLINK_RATE random(5,20)
#define BLINK_PIN 13

#define WATERFLOW_PIN 2
#define WATERTEMP_PIN A0

#define WATCHDOG_PIN  12
#define WATCHDOG_ON  LOW
#define WATCHDOG_OFF HIGH
bool watchdogOn=false;

#define WATCHDOG_MINFLOW  50
#define WATCHDOG_MINTEMP  10
#define WATCHDOG_MAXTEMP  512

#define TESTMODE

////////////////////////////////////////////////////////////

void setup()
{
  pinMode(WATCHDOG_PIN, OUTPUT);
  digitalWrite(WATCHDOG_PIN,WATCHDOG_OFF);

	Serial.begin(250000);
  flow.Init(WATERFLOW_PIN);

  pinMode(BLINK_PIN, OUTPUT);
  blinkTime=millis()+BLINK_RATE;
}

////////////////////////////////////////////////////////////

bool IsWatchDogOn()
{
  // test water flow

  unsigned int avgCount = flow.AvgCount(2000);

  if (avgCount < WATCHDOG_MINFLOW)
    return false;

  // test tmperature

  float wtemp = ReadTemp();

  if (wtemp < WATCHDOG_MINTEMP || wtemp > WATCHDOG_MAXTEMP)
    return false;

    return true;
}

////////////////////////////////////////////////////////////

float ReadTemp()
{
  const unsigned char maxcount=16;
  int wtemp = 0;
  for (int i=0;i<maxcount;i++)
    wtemp += analogRead(WATERTEMP_PIN);

  return (float) wtemp  / maxcount;
}

////////////////////////////////////////////////////////////

void WatchDogOn()
{
  digitalWrite(WATCHDOG_PIN,WATCHDOG_ON);
  if (watchdogOn == false)
  {
    watchdogOn = true;
    Serial.println(F("Watchdog ON"));
  }
}

////////////////////////////////////////////////////////////

void WatchDogOff()
{
    digitalWrite(WATCHDOG_PIN,WATCHDOG_OFF);
    if (watchdogOn == true)
    {
      watchdogOn = false;
      Serial.println(F("Watchdog OFF"));
    }
}

////////////////////////////////////////////////////////////

void loop()
{
  if (blinkTime<millis())
  {
    blinkTime += BLINK_RATE;
    blinkOn = !blinkOn;
    digitalWrite(BLINK_PIN,blinkOn ? HIGH : LOW);
  }

  if (IsWatchDogOn())
  {
    WatchDogOn();
  }
  else
  {
    WatchDogOff();
  }

#ifdef TESTMODE

  static unsigned int lastAvgCount=0xffff;
  unsigned int avgCount = flow.AvgCount(2000);

  if (avgCount != lastAvgCount)
  {
    lastAvgCount = avgCount;
    Serial.println(avgCount);
  }

  static float lastwtemp=0;
  float wtemp = ReadTemp();
  if (abs(wtemp-lastwtemp) > 1)
  {
    lastwtemp = wtemp;
    Serial.println(wtemp);
  }

#endif
}

