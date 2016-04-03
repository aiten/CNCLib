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

const int maxspeedfast = 3000;  //mmpermin
const int maxspeedslow = 400;   //mmpermin

const int intervall = 200;      // ms

#define XANALOGPIN  A0
#define YANALOGPIN  A1
#define PUSHPIN     8

////////////////////////////////////////////////////////

#define CHAL
typedef int pin_t;
#define EnumAsByte(a) unsigned char
#define pinModeInputPullUp(a) pinMode(a,INPUT_PULLUP)

#include "ReadAnalogIOControl.h"
#include "ReadPinIOControl.h"
#include "PushButton.h"

CReadAnalogIOControl<XANALOGPIN> X;
CReadAnalogIOControl<YANALOGPIN> Y;
CPushButton btn(PUSHPIN, LOW);

const int maxanalog = 32;
const int minanalog = 0;
const int maxdist = (maxanalog - minanalog) / 2;
int neutralanalog = (maxanalog - minanalog) / 2;

bool speedfast = false;

////////////////////////////////////////////////////////

void setup()
{
  X.Init(); X.SetMinMax(minanalog, maxanalog);
  Y.Init(); Y.SetMinMax(minanalog, maxanalog);

  neutralanalog = X.Read();

  Serial.begin(250000);
}

////////////////////////////////////////////////////////

unsigned long MoveTime(unsigned int distinmm1000, unsigned int mmPerMin)
{
  return (unsigned long) distinmm1000 * 60 / (unsigned long)mmPerMin;
}

////////////////////////////////////////////////////////

void SendCommand(int mm1000X, int mm1000Y, unsigned int mmPerMin)
{
  Serial.print(F("g91 g1"));
  if (mm1000X != 0)
  {
    Serial.print('X');
    Serial.print(mm1000X / 1000.0);
  }
  if (mm1000Y != 0)
  {
    Serial.print('Y');
    Serial.print(mm1000Y / 1000.0);
  }
  Serial.print(F("F"));
  Serial.print(mmPerMin);
  Serial.println(F(" g90"));
}

////////////////////////////////////////////////////////

unsigned int SpeedToDist(unsigned int mmPerMin, unsigned int ms)
{
  // return in mm1000
  return (unsigned long) mmPerMin * (unsigned long) ms / 60;
}

////////////////////////////////////////////////////////

unsigned int ToSpeed(int diffX, int diffY)
{
  // diffX, diffY 0..maxDist
  
  if (diffX == 0 && diffY == 0)
    return 0;

  // diff range 0..maxdist
  float diffF = hypot(diffX, diffY);

  int mult = (speedfast ? maxspeedfast : maxspeedslow) / maxdist;
  return (unsigned int) (diffF * mult);
}

////////////////////////////////////////////////////////

void loop()
{
  static unsigned long timeNext = 0;

  if (millis() > timeNext)
  {
    int x = X.Read();
    int y = Y.Read();
    int xdist = x - neutralanalog;
    int ydist = y - neutralanalog;
    float angle = atan2(ydist, xdist);

    int speedPerMin = ToSpeed(xdist, ydist);

    if (speedPerMin)
    {
      unsigned int mm1000 = SpeedToDist(speedPerMin, intervall);
      SendCommand((int) (cos(angle)*mm1000), (int) (-sin(angle)*mm1000), speedPerMin);
      timeNext = millis() + MoveTime(abs(mm1000), speedPerMin);
    }
  }

  bool ison = btn.IsOn();

  if (ison)
  {
    speedfast  = !speedfast;
  }
}

