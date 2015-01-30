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

#include "Configuration_ProxxonMF70.h"

////////////////////////////////////////////////////////

#include <LCD.h>
#include <RotaryButton.h>
#include <Beep.h>

#include "MyMenu.h"

#define ROTARY_ACCURACY	4

#define MAXCHARPERLINE  21

////////////////////////////////////////////////////////

class CMyLcd : public CLcd
{
private:

	typedef CLcd super;

public:

	CMyLcd()												{ _curretDraw = NULL; _expectButtonOff = false; _rotaryFocus = RotaryMainPage; }

	virtual void Init();

	typedef bool(CMyLcd::*DrawFunction)(bool setup);
	typedef void(CMyLcd::*ButtonFunction)();

protected:

	virtual void Poll();
	virtual void TimerInterrupt();
	virtual void Command(char* cmd);

	////////////////////////////////////////////////////////

public:
	// for menu

	void SetDefaultPage();

	virtual void Beep(const SPlayTone*);

	void MenuChanged()
	{
		SetRotaryFocusMenuPage();
		DrawLoop();
		OKBeep();
	}

	////////////////////////////////////////////////////////

protected:

	struct SPageDef
	{
		DrawFunction draw;
		ButtonFunction buttonpress;
	};

	typedef signed char rotarypos_t;

	enum EPage
	{
		SplashPage = 0,
		DebugPage = 1,
		AbsPage = 2,
		PresetPage = 3,
		SDPage = 4,
		PausePage = 5,
		ErrorPage = 6,
		CommandHisPage = 7,
		MenuPage = 8,
		PageCount
	};

	enum ERotaryFocus
	{
		RotaryMainPage,
		RotaryMenuPage
	};

	EnumAsByte(EPage) GetPage();

	virtual unsigned long Draw(EDrawType draw);
	virtual unsigned long Splash();
	virtual void FirstDraw();

	void QueueCommandHistory(char ch);

	DrawFunction _curretDraw;

	bool _expectButtonOff;
	EnumAsByte(ERotaryFocus) _rotaryFocus;

	EnumAsByte(EPage)		_currentpage;


	CRotaryButton<rotarypos_t, ROTARY_ACCURACY> _button;
	CRingBufferQueue<char, 128> _commandHis;


	static const SPageDef _pagedef[];

	void SetMenuPage();

	void ButtonPress();

	void SetRotaryFocusMainPage();
	void SetRotaryFocusMenuPage();

	CMyMenu _menu;

	void ButtonPressStartSDPage();
	void ButtonPressPause();
	void ButtonPressMenuPage();
	void ButtonPressShowMenu();

	void DrawLoop(DrawFunction drawfnc)						{ _curretDraw = drawfnc; DrawLoop(); }
	void DrawLoop();

	bool DrawLoopSplash(bool setup);
	bool DrawLoopDebug(bool setup);	
	bool DrawLoopPosAbs(bool setup);
	bool DrawLoopPreset(bool setup);
	bool DrawLoopStartSD(bool setup);
	bool DrawLoopPause(bool setup);	
	bool DrawLoopError(bool setup);
        bool DrawLoopCommandHis(bool setup);
	bool DrawLoopMenu(bool setup);

	bool DrawLoopSetupDefault();
	void DrawLoopDefaultHead();

	// Menu Page

	unsigned char GetMenuIdx();


public:

#if defined(__AVR_ARCH__)

        static ButtonFunction GetButtonPress_P(const void* adr);
        static DrawFunction GetDrawFunction_P(const void* adr);

#endif
};

////////////////////////////////////////////////////////

extern CMyLcd Lcd;
