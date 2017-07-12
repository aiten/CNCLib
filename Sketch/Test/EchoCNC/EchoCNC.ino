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

char _buffer[128];
uint8_t _bufferidx;

void setup()
{
  Serial.begin(250000);
  _bufferidx = 0;
}

bool IsEndOfCommandChar(char ch)
{
  return ch == '\n' || ch == (char) - 1;
}

void Command(char* buffer)
{
  Serial.println(buffer);
  Serial.println("ok");
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
