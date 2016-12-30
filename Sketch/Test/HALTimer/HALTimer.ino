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

#include <StepperLib.h>

////////////////////////////////////////////////////////
unsigned int irq_countTimer0 = 0;
unsigned int irq_countTimer1 = 0;

void HandleInterruptTimer0()
{
  irq_countTimer0++;
}
void HandleInterruptTimer1()
{
  irq_countTimer1++;

  CHAL::StartTimer1OneShot(48000);
}

void setup()
{
  Serial.begin(250000);
  Serial.println(F("Start HAL Timer Test"));

  CHAL::InitTimer0(HandleInterruptTimer0);
  CHAL::InitTimer1OneShot(HandleInterruptTimer1);

  CHAL::StartTimer0(10000);
  CHAL::StartTimer1OneShot(65530);

  {
    CCriticalRegion crit;
    // do not wait until finished
    CHAL::StartTimer1OneShot(50000);
  }
  Serial.println(F("Setup done"));
}

void loop()
{
  static unsigned int myirq_countTimer0 = 0;
  static unsigned int myirq_countTimer1 = 0;
  static long starttime0 = millis();
  static long starttime1 = millis();

  // dummy
  delay(333);

  static int dotcount = 0;

  if (myirq_countTimer0 != irq_countTimer0 || myirq_countTimer1 != irq_countTimer1)
  {
    if (dotcount > 0)
      Serial.println();
    if (myirq_countTimer0 != irq_countTimer0)
    {
      Serial.print("Timer0=");
      myirq_countTimer0 = irq_countTimer0;
      Serial.print(myirq_countTimer0);
      Serial.print("(");
      Serial.print((millis() - starttime0) / 1000.0 / myirq_countTimer0, 6);
      Serial.println(")");
    }
    if (myirq_countTimer1 != irq_countTimer1)
    {
      Serial.print("Timer1=");
      myirq_countTimer1 = irq_countTimer1;
      Serial.print(myirq_countTimer1);
      Serial.print("(");
      Serial.print((millis() - starttime1) / 1000.0 / myirq_countTimer1, 6);
      Serial.println(")");
    }
  }
  else
  {
    dotcount++;
    Serial.print('+');
    if (dotcount > 40)
    {
      dotcount = 0;
      Serial.println();
    }
  }
}

