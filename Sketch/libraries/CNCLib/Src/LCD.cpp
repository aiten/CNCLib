////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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

	DrawRequest(true, CLcd::DrawAll);		//first draw doesnt return fast! call at init time
}

////////////////////////////////////////////////////////////

void CLcd::Idle(unsigned int /* idletime */)
{
	DrawRequest(true, CLcd::DrawAll);
}

////////////////////////////////////////////////////////////

void CLcd::TimerInterrupt()
{
}

////////////////////////////////////////////////////////////

void CLcd::DrawRequest(bool forcedraw, EDrawType draw)
{
	if (_splash)
	{
		if (_nextdrawtime > millis()) return;
		// splash timeout;
		_splash = false;
		FirstDraw();
	}

	if (forcedraw || _nextdrawtime < millis())
	{
		Draw(draw);
		_nextdrawtime = millis() + 333;
	}
}

////////////////////////////////////////////////////////////

bool CLcd::PostCommand(const __FlashStringHelper* cmd, Stream* output)
{
	if (!CControl::GetInstance()->PostCommand(cmd,output))
	{
		ErrorBeep();
		return false;
	}
	return true;
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

