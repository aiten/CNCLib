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

   Serial.println(F("Setup done"));
   Serial.println(F("(i)nit, (r)ead, (s)et, (w)rite, (x)reset"));
}

static char _buffer[128];
static uint8_t _bufferidx=0;

bool IsEndOfCommandChar(char ch)
{
  return ch == '\r' || ch == '\n' || ch == (char) - 1;
}

void CommandRead()
{
  Serial.println("Eeprom: ");
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

void CommandInit()
{
    CHAL::InitEeprom();
    Serial.println("init done");
}

void CommandSet()
{
  for (int i=0;i<16;i++)
  {
    CHAL::eeprom_write_dword(CHAL::GetEepromBaseAdr()+i,CHAL::eeprom_read_dword(CHAL::GetEepromBaseAdr()+i)+i);
  }
  CommandRead();
}

void CommandWrite()
{
    CHAL::FlushEeprom();
}

void Command(char* buffer)
{
  if (buffer[0])
  {
    if (strcmp(buffer,"read") == 0 || strcmp(buffer,"r") == 0)
    {
      CommandRead();
    }
    else if ((strcmp(buffer,"init") == 0 || strcmp(buffer,"i") == 0))
    {
      CommandInit();
    }
    else if ((strcmp(buffer,"set") == 0 || strcmp(buffer,"s") == 0))
    {
      CommandSet();
    }
    else if ((strcmp(buffer,"write") == 0 || strcmp(buffer,"w") == 0))
    {
      CommandWrite();
    }
    else if ((strcmp(buffer,"reset") == 0 || strcmp(buffer,"x") == 0))
    {
      NVIC_SystemReset();
    }
    else
    {
        Serial.println(buffer);
        Serial.println("?");
    }
  }
}

void loop()
{
  if (Serial.available() > 0)
  {
    while (Serial.available() > 0)
    {
      char ch = _buffer[_bufferidx] = Serial.read();

      if (IsEndOfCommandChar(ch))
      {
        _buffer[_bufferidx] = 0;      // remove from buffer
        Command(_buffer);
        _bufferidx = 0;

        return;
      }

      _bufferidx++;
      if (_bufferidx >= sizeof(_buffer))
      {
        Serial.println("MESSAGE_CONTROL_FLUSHBUFFER");
        _bufferidx = 0;
      }
    }
  }
}

