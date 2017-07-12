////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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

void PrintHex(uint32_t val)
{
  if (val < 16)
  {
    Serial.print((int) 0);
   }
    Serial.print(val,HEX);
    Serial.print(':');
}

void PrintEeprom()
{
  for (int i=0;i<16;i++)
  {
    uint32_t val = CHAL::eeprom_read_dword(CHAL::GetEepromBaseAdr()+i);
    PrintHex(val%256);
    PrintHex((val>>8)%256);
    PrintHex((val>>16)%256);
    PrintHex((val>>24)%256);
  }
  Serial.println();
}

void setup()
{
  Serial.begin(250000);
  Serial.println(F("Start ZeroEEPromTest"));

  CHAL::InitEeprom();

  if (!CHAL::HaveEeprom())
  {
    Serial.println(F("EEprom is NOT available"));
    return;
  }

  CHAL::InitEeprom();

  PrintEeprom();

  for (int i=0;i<16;i++)
  {
    CHAL::eeprom_write_dword(CHAL::GetEepromBaseAdr()+i,CHAL::eeprom_read_dword(CHAL::GetEepromBaseAdr()+i)+i);
  }

  PrintEeprom();

  Serial.println(F("Flushing ..."));

  CHAL::FlushEeprom();
  
  Serial.println(F("Read"));
  
  CHAL::InitEeprom();
  PrintEeprom();

  Serial.println(F("Setup done"));
}

void loop()
{
  static uint32_t nexttime=0;
  uint32_t now=millis();

  if (nexttime < now)
  {
    Serial.println(F("alive"));
    nexttime = now + 20000;
  }
}

