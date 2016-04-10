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

#define MAXSPEED1 3000
#define MAXSPEED2 400
#define INTERVALL 200

#define XANALOGPIN  A0
#define YANALOGPIN  A1
#define PUSHPIN     8

#define BUTTON1     7
#define BUTTON2     6
#define BUTTON3     5
#define BUTTON4     4
#define BUTTON5     3
#define BUTTON6     2

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

CPushButton btn1(BUTTON1, LOW);
CPushButton btn2(BUTTON2, LOW);
CPushButton btn3(BUTTON3, LOW);
CPushButton btn4(BUTTON4, LOW);
CPushButton btn5(BUTTON5, LOW);
CPushButton btn6(BUTTON6, LOW);

const int maxanalog = 32;
const int minanalog = 0;
const int maxdist = (maxanalog - minanalog) / 2;
int neutralanalog = (maxanalog - minanalog) / 2;

int maxspeedfast = MAXSPEED1;  //mmpermin
int maxspeedslow = MAXSPEED2;  //mmpermin

int intervall = INTERVALL;      // ms

bool speedfast = false;
unsigned char laserpower = 255;

char buffer[64];
unsigned char bufferidx = 0;

unsigned long timeNext = 0;

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

void InCommand(char*b)
{
  char*col = strchr(b,'=');
  if (col!=NULL)
  {
    *col = 0;
    col++;

    int varidx = atoi(b);
    int varvalue = atoi(col);

    Serial.print(varidx);
    Serial.print(F("="));
    Serial.println(varvalue);

    switch(varidx)
    {
      case 1: maxspeedfast = varvalue; break;
      case 2: maxspeedslow = varvalue; break;
      case 3: intervall    = varvalue; break;
    }
  }
  
  //Serial.println(b);
}

////////////////////////////////////////////////////////

void loop()
{
  if (Serial.available() > 0)
  {
    char ch = buffer[bufferidx] = Serial.read();

    if (ch == '\n')
    {
      buffer[bufferidx] = 0;
      InCommand(buffer);
      bufferidx = 0;
    }
    else
    {
      bufferidx++;
      if (bufferidx >= sizeof(buffer))
      {
        Serial.println(F("buffer overrun"));
      }
    }
  }

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

  if (btn.IsOn())
  {
    speedfast  = !speedfast;
  }

  if (btn1.IsOn())
  {
    laserpower = 255;
    Serial.println(F("m107"));
  }

  if (btn2.IsOn())
  {
    laserpower = laserpower == 1 ? 255 : 1;
    Serial.print(F("m106 s"));
    Serial.println((int) laserpower);
  }

  if (btn3.IsOn())
  {
    Serial.println(F(";btn3"));
  }

  if (btn4.IsOn())
  {
    Serial.println(F(";btn4"));
  }

  if (btn5.IsOn())
  {
    Serial.println(F(";btn5"));
  }

  if (btn6.IsOn())
  {
    Serial.println(F("g92 x0y0"));
  }
}

////////////////////////////////////////////////////////

