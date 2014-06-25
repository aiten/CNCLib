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

#pragma once

////////////////////////////////////////////////////////

#if  defined(__AVR_ATmega328P__) || defined(__SAM3X8E__)

#else

#define __USE_LCD__

////////////////////////////////////////////////////////

#include <LCD.h>

////////////////////////////////////////////////////////

#define MYLCD_ROWS	4
#define MYLCD_COLS	20

////////////////////////////////////////////////////////

class CMyLcd : public CLcd
{
private:

	typedef CLcd super;

public:

	virtual void Init();
	virtual void TimerInterrupt();

protected:

	virtual void Draw(EDrawType draw);
	virtual unsigned long Splash();
	virtual void FirstDraw();

private:

	void DrawPos(unsigned char col, unsigned char row, unsigned long pos);
	void DrawES(unsigned char col,  unsigned char row, bool es);

	void DrawPen(unsigned char col, unsigned char row);
};

////////////////////////////////////////////////////////

extern CMyLcd Lcd;

#endif
