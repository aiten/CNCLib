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
unsigned int irq_count = 0;

void HandleInterrupt()
{
  irq_count++;

  CHAL::StartTimer1(48000);
}

void setup()
{
  Serial.begin(250000);
  Serial.println(F("Start HAL Timer Test"));


  CHAL::InitTimer1(HandleInterrupt);

  CHAL::StartTimer1(65530);

  {
    CCriticalRegion crit;
    // do not wait until finished
    CHAL::StartTimer1(50000);

    

    
  }
  Serial.println(F("Setup done"));
}

void loop()
{
  static unsigned int myirq_count = 0;
  static long starttime = millis();

  // dummy
  delay(250);

  static int dotcount = 0;

  if (myirq_count != irq_count)
  {
    if (dotcount > 0)
      Serial.println();
    Serial.print("Timer=");
    myirq_count = irq_count;
    Serial.print(myirq_count);
    Serial.print("(");
    Serial.print((millis() - starttime) / 1000.0 / myirq_count, 6);
    Serial.println(")");
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

