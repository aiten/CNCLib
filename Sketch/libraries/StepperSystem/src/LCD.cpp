////////////////////////////////////////////////////////////

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include "Lcd.h"

////////////////////////////////////////////////////////////

CLcd* CLcd::_lcd = NULL;

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
