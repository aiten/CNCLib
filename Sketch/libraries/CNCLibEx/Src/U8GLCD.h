////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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

#include <LCD.h>
#include <RotaryButton.h>
#include <PushButton.h>
#include <Beep.h>

#define ROTARY_ACCURACY	4

#define LCD_GROW 64
#define LCD_GCOL 128

//#define LCD_NUMAXIS	NUM_AXIS

////////////////////////////////////////////////////////

#if LCD_NUMAXIS > 5

#define DEFAULTFONT u8g_font_6x10
#define CharHeight  9		// char height
#define CharAHeight 7		// char A height
#define CharWidth   6

#define HeadLineOffset (-2)
#define PosLineOffset  (0)

#else

#define DEFAULTFONT u8g_font_6x12
#define CharHeight  10		// char height
#define CharAHeight 7		// char A height
#define CharWidth   6

#define HeadLineOffset (-2)
#define PosLineOffset  (1)

#endif

#define TotalRows (LCD_GROW / CharHeight)
#define TotalCols (LCD_GCOL / CharWidth)

////////////////////////////////////////////////////////

class CU8GLcd : public CLcd
{
private:

	typedef CLcd super;

public:

	CU8GLcd()												{  }

	virtual void Init() override;

	enum EDrawLoopType
	{
		DrawLoopSetup,
		DrawLoopHeader,
		DrawLoopDraw,
		DrawLoopQueryTimerout
	};

	typedef bool(CU8GLcd::*DrawFunction)(EnumAsByte(EDrawLoopType) type,void *data);
	typedef void(CU8GLcd::*ButtonFunction)();

protected:

	virtual void Poll() override;
	virtual void TimerInterrupt() override;
	virtual void Command(char* cmd) override;

	////////////////////////////////////////////////////////

protected:

	virtual class U8GLIB& GetU8G() = 0;
	virtual class CMenuBase& GetMenu() = 0;

	unsigned char GetPageCount();

public:

	////////////////////////////////////////////////////////
	// for menu

	void SetDefaultPage();

	void MenuChanged()
	{
		SetRotaryFocusMenuPage();
		DrawLoop();
		OKBeep();
	}

	unsigned char GetMenuIdx();

	////////////////////////////////////////////////////////

protected:

	struct SPageDef
	{
		DrawFunction draw;
		ButtonFunction buttonpress;
	};

	static const SPageDef _pagedef[];

	typedef signed int rotarypos_t;

	enum ERotaryFocus
	{
		RotaryMainPage,
		RotaryMenuPage,
		RotarySlider
	};

	unsigned char GetPage();

	virtual unsigned long Draw(EDrawType draw) override;
	virtual unsigned long Splash() override;

	void QueueCommandHistory(char ch);

	unsigned long DrawLoop(DrawFunction drawfnc)						{ _curretDraw = drawfnc; return DrawLoop(); }
	unsigned long DrawLoop();

	virtual bool DrawLoopDefault(EnumAsByte(EDrawLoopType) type,void *data);

	void SetMenuPage();

	void ButtonPress();

	void SetRotaryFocusMainPage();
	void SetRotaryFocusMenuPage();

	void ButtonPressStartSDPage();
	void ButtonPressPause();
	void ButtonPressMenuPage();
	void ButtonPressShowMenu();
	void ButtonPressSpeedOverride();

	bool DrawLoopSplash(EnumAsByte(EDrawLoopType) type,void *data);
	bool DrawLoopDebug(EnumAsByte(EDrawLoopType) type,void *data);	
	bool DrawLoopPosAbs(EnumAsByte(EDrawLoopType) type,void *data);
	bool DrawLoopPos(EnumAsByte(EDrawLoopType) type, void *data);
	bool DrawLoopSpeedOverride(EnumAsByte(EDrawLoopType) type, void *data);
	bool DrawLoopPreset(EnumAsByte(EDrawLoopType) type, void *data);
	bool DrawLoopStartSD(EnumAsByte(EDrawLoopType) type,void *data);
	bool DrawLoopPause(EnumAsByte(EDrawLoopType) type,void *data);	
	bool DrawLoopError(EnumAsByte(EDrawLoopType) type,void *data);
    bool DrawLoopCommandHis(EnumAsByte(EDrawLoopType) type,void *data);
	bool DrawLoopMenu(EnumAsByte(EDrawLoopType) type,void *data);

private:

	DrawFunction				_curretDraw=NULL;

	EnumAsByte(ERotaryFocus)	_rotaryFocus=RotaryMainPage;

	unsigned char				_currentpage;

	CRingBufferQueue<char, 128> _commandHis;

protected:

	CRotaryButton<rotarypos_t, ROTARY_ACCURACY> _rotarybutton;
	CPushButton									_rotarypushbutton;

	unsigned char				_lcd_numaxis = NUM_AXIS;

	static unsigned char ToRow(unsigned char row) { return  (row + 1)*(CharHeight); }
	static unsigned char ToCol(unsigned char col) { return (col)*(CharWidth); }

#if defined(__AVR_ARCH__)

    static ButtonFunction GetButtonPress(const void* adr);
    static DrawFunction GetDrawFunction(const void* adr);

#else
	static ButtonFunction GetButtonPress(const ButtonFunction* adr)	{ return *adr; }
    static DrawFunction GetDrawFunction(const DrawFunction* adr)	{ return *adr; };

#endif

};

////////////////////////////////////////////////////////
