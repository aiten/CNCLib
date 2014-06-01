#pragma once

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
	virtual void Idle(unsigned int idletime);
	virtual void TimerInterrupt();

	virtual unsigned char TextModeCols()					{ return MYLCD_COLS; }
	virtual unsigned char TextModeRows()					{ return MYLCD_ROWS; }

	virtual void TextModeClear();
	virtual void TextModeDraw(unsigned char col, unsigned char row, const __FlashStringHelper* s);
	virtual void TextModeDraw(unsigned char col, unsigned char row, char* s);

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