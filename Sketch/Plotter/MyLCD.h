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
