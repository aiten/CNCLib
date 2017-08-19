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
////////////////////////////////////////////////////////

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include "Lcd.h"
#include "Control.h"

////////////////////////////////////////////////////////////

template<> CLcd* CSingleton<CLcd>::_instance = NULL;

////////////////////////////////////////////////////////////

void CLcd::Init()
{
	_nextdrawtime = Splash() + millis();
	_splash = true;

	DrawRequest(CLcd::DrawForceAll);		//first draw doesnt return fast! call at init time
}

////////////////////////////////////////////////////////////

void CLcd::Poll()
{
	DrawRequest(CLcd::DrawAll);
}

////////////////////////////////////////////////////////////

void CLcd::Invalidate()
{
	_invalidate = true;
}

////////////////////////////////////////////////////////////

void CLcd::TimerInterrupt()
{
}

////////////////////////////////////////////////////////////

void CLcd::Command(char* /* buffer */)
{
}

////////////////////////////////////////////////////////////

void CLcd::DrawRequest(EDrawType draw)
{
	if (_splash)
	{
		if (_nextdrawtime > millis()) return;
		// splash timeout;
		_splash = false;
		Draw(DrawFirst);
	}

	if (_invalidate || draw==DrawForceAll || _nextdrawtime < millis())
	{
		_invalidate = false;
		_nextdrawtime = Draw(draw) + millis();
	}
}

////////////////////////////////////////////////////////////

uint8_t CLcd::InitPostCommand(EnumAsByte(EGCodeSyntaxType) /* syntaxtype */, char* cmd)
{
	cmd[0] = 0;
	return 0;
}

////////////////////////////////////////////////////////////

bool CLcd::PostCommand(EnumAsByte(EGCodeSyntaxType) syntaxtype, const __FlashStringHelper* cmd, Stream* output)
{
	char buffer[32];

	const char* cmd1 = (const char*)cmd;
	uint8_t idx = InitPostCommand(syntaxtype, buffer);
	uint8_t idxprogmem = 0;

	for (; idx < sizeof(buffer); idx++, idxprogmem++)
	{
		buffer[idx] = pgm_read_byte(&cmd1[idxprogmem]);

		if (buffer[idx] == 0)
		{
			return PostCommand(buffer, output);
		}
	}

	return false;
}

////////////////////////////////////////////////////////////

bool CLcd::PostCommand(char* cmd, Stream* output)
{
	if (!CControl::GetInstance()->PostCommand(cmd,output))
	{
		ErrorBeep();
		return false;
	}
	return true;
}

