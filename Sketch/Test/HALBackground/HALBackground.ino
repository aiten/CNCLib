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

unsigned int background_count = 0;

void HandleBackGround()
{
  Serial.println(F("in bkg"));

  background_count++;

  for (uint32_t i=0; i < 10000000;i++)
  {
    // wait
  }
  Serial.println(F("out bkg"));
}

void setup()
{
  Serial.begin(250000);
  Serial.println(F("Start Background on SAM"));

  CHAL::InitBackground(HandleBackGround);

  Serial.println(F("Setup done"));
}

void loop()
{
  static unsigned int mybackground_count = 0;
  static long starttime = millis();

  // dummy
  delay(250);
  CHAL::BackgroundRequest();

  static int dotcount = 0;

  if (mybackground_count != background_count)
  {
    if (dotcount > 0)
      Serial.println();
    Serial.print("Timer=");
    mybackground_count = background_count;
    Serial.print(mybackground_count);
    Serial.print("(");
    Serial.print((millis() - starttime) / 1000.0 / mybackground_count, 6);
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

