////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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

#include <U8glib.h>
#include <LCD.h>
#include <RotaryButton.h>
#include <PushButton.h>
#include <Beep.h>

#define ROTARY_ACCURACY	4

#define LCD_GROW 64
#define LCD_GCOL 128

////////////////////////////////////////////////////////

class CU8GLcd : public CLcd
{
private:

	typedef CLcd super;

public:

	CU8GLcd();

	virtual void Init() override;

	enum EDrawLoopType
	{
		DrawLoopSetup,
		DrawLoopHeader,
		DrawLoopDraw,
		DrawLoopQueryTimerout
	};

	typedef bool(CU8GLcd::*DrawFunction)(EnumAsByte(EDrawLoopType) type, intptr_t data);
	typedef void(CU8GLcd::*ButtonFunction)();

protected:

	virtual void Poll() override;
	virtual void TimerInterrupt() override;
	virtual void Command(char* cmd) override;

	////////////////////////////////////////////////////////

protected:

	virtual class U8GLIB& GetU8G() = 0;
	virtual class CMenuBase& GetMenu() = 0;

	uint8_t GetPageCount();

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

	uint8_t GetMenuIdx();

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

	uint8_t GetPage();

	virtual unsigned long Draw(EDrawType draw) override;
	virtual unsigned long Splash() override;

	void QueueCommandHistory(char ch);

	unsigned long DrawLoop(DrawFunction drawfnc)						{ _curretDraw = drawfnc; return DrawLoop(); }
	unsigned long DrawLoop();

	virtual bool DrawLoopDefault(EnumAsByte(EDrawLoopType) type,intptr_t data);

	void SetMenuPage();

	void ButtonPress();

	void SetRotaryFocusMainPage();
	void SetRotaryFocusMenuPage();

	void ButtonPressStartSDPage();
	void ButtonPressMenuPage();
	void ButtonPressShowMenu();
	void ButtonPressSpeedOverride();

	bool DrawLoopSplash(EnumAsByte(EDrawLoopType) type,intptr_t data);
	bool DrawLoopDebug(EnumAsByte(EDrawLoopType) type,intptr_t data);	
	bool DrawLoopPosAbs(EnumAsByte(EDrawLoopType) type,intptr_t data);
	bool DrawLoopPos(EnumAsByte(EDrawLoopType) type, intptr_t data);
	bool DrawLoopRotate2D(EnumAsByte(EDrawLoopType) type, intptr_t data);
	bool DrawLoopRotate3D(EnumAsByte(EDrawLoopType) type, intptr_t data);
	bool DrawLoopSpeedOverride(EnumAsByte(EDrawLoopType) type, intptr_t data);
	bool DrawLoopPreset(EnumAsByte(EDrawLoopType) type, intptr_t data);
	bool DrawLoopStartSD(EnumAsByte(EDrawLoopType) type,intptr_t data);
	bool DrawLoopError(EnumAsByte(EDrawLoopType) type,intptr_t data);
    bool DrawLoopCommandHis(EnumAsByte(EDrawLoopType) type,intptr_t data);
	bool DrawLoopMenu(EnumAsByte(EDrawLoopType) type,intptr_t data);

private:

	DrawFunction				_curretDraw=NULL;

	EnumAsByte(ERotaryFocus)	_rotaryFocus=RotaryMainPage;

	uint8_t						_currentpage;

	CRingBufferQueue<char, 128> _commandHis;

protected:

	CRotaryButton<rotarypos_t, ROTARY_ACCURACY> _rotarybutton;
	CPushButton									_rotarypushbutton;

	uint8_t						_lcd_numaxis = NUM_AXIS;
	uint8_t						_charHeight = 10;
	uint8_t						_charWidth = 6;

	const u8g_fntpgm_uint8_t*	_font = u8g_font_6x10;


	uint8_t ToRow(uint8_t row) { return  (row + 1)*(_charHeight); }
	uint8_t ToCol(uint8_t col) { return (col)*(_charWidth); }

	uint8_t TotalRows() { return LCD_GROW / _charHeight; }
	uint8_t TotalCols() { return LCD_GCOL / _charWidth; }

	static char* DrawPos(axis_t axis, mm1000_t pos, char *tmp, uint8_t precision);		// draw mm100 or inch

#if defined(__AVR_ARCH__)

    static ButtonFunction GetButtonPress(const void* adr);
    static DrawFunction GetDrawFunction(const void* adr);

#else
	static ButtonFunction GetButtonPress(const ButtonFunction* adr)	{ return *adr; }
    static DrawFunction GetDrawFunction(const DrawFunction* adr)	{ return *adr; };

#endif

};

////////////////////////////////////////////////////////
