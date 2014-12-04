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

#include "Configuration_ProxxonMF70.h"

////////////////////////////////////////////////////////

#include <LCD.h>
#include <RotaryButton.h>

#define ROTARY_ACCURACY	4

////////////////////////////////////////////////////////

class CMyLcd : public CLcd
{
private:

	typedef CLcd super;

public:

	CMyLcd()												{ _curretDraw = NULL; _expectButtonOff = false; _rotaryFocus = RotaryMainPage; }

	virtual void Init();
	virtual void Idle(unsigned int idletime);
	virtual void TimerInterrupt();

protected:

	typedef signed char rotarypos_t;

	enum EPage
	{
		SplashPage = 0,
		DebugPage = 1,
		AbsPage = 2,
		PresetPage = 3,
		StartSDPage = 4,
		PausePage = 5,
		ErrorPage = 6,
		MenuPage = 7,
		PageCount
	};

	enum ERotaryFocus
	{
		RotaryMainPage,
		RotaryMenuPage
	};

	EnumAsByte(EPage) GetPage();

	virtual void Draw(EDrawType draw);
	virtual unsigned long Splash();
	virtual void FirstDraw();

private:

	void SetRotaryFocusMainPage();
	void SetRotaryFocusMenuPage();

	struct SMenuItemDef;
	struct SMenuDef;

	typedef bool(CMyLcd::*DrawFunction)(bool setup);
	typedef void(CMyLcd::*ButtonFunction)();
	typedef void(CMyLcd::*MenuButtonFunction)(const struct SMenuItemDef*);

	void DrawLoop(DrawFunction drawfnc)						{ _curretDraw = drawfnc; DrawLoop(); }
	void DrawLoop();

	struct SPageDef
	{
		DrawFunction draw;
		ButtonFunction buttonpress;
	};

	typedef const void* menuparam_t;

	struct SMenuItemDef
	{
	public:
		const char* _text;
		MenuButtonFunction _buttonpress;
		menuparam_t _param1;
		menuparam_t _param2;
	public:
		const __FlashStringHelper* GetText() const			{ return (const __FlashStringHelper*)pgm_read_ptr(&this->_text); }
		MenuButtonFunction GetButtonPress()const;
		menuparam_t GetParam1()	const						{ return (menuparam_t)pgm_read_word(&this->_param1); }
		menuparam_t GetParam2()	const						{ return (menuparam_t)pgm_read_word(&this->_param2); }
	};

	struct SMenuDef
	{
	public:
		const char* _text;
//		menuparam_t _param1;
//		menuparam_t _param2;
		const struct SMenuItemDef* _items;

		unsigned char GetItemCount() const 
		{
			const struct SMenuItemDef* items = GetItems();
			for (unsigned char x = 0;; x++)
			{
				if (items[x].GetText() == NULL) return x;
			}
		}

		unsigned char FindMenuIdx(CMyLcd::MenuButtonFunction f, unsigned short param, unsigned char valueIffail) const
		{
			const struct SMenuItemDef* items = GetItems();
			for (unsigned char x = 0; items[x].GetText() != NULL; x++)
			{
				if (items[x].GetButtonPress() == f && items[x].GetParam1() == (menuparam_t)param)
					return x;
			}

			return valueIffail;
		}

	public:
		const __FlashStringHelper* GetText() const		{ return (const __FlashStringHelper*)pgm_read_ptr(&this->_text); }
		const struct SMenuItemDef* GetItems() const		{ return (const struct SMenuItemDef*)pgm_read_word(&this->_items); }
//		menuparam_t GetMenuParam1()	const				{ return (menuparam_t)pgm_read_word(&this->param1); }
//		menuparam_t GetMenuParam2()	const				{ return (menuparam_t)pgm_read_word(&this->param2); }
	};


	EnumAsByte(ERotaryFocus) _rotaryFocus;
	EnumAsByte(EPage)		_currentpage;
	unsigned char			_currentMenuIdx;
	unsigned char			_currentMenuOffset;
	axis_t					_currentMenuAxis;

	const SMenuDef*				_currentMenu;
	const __FlashStringHelper*	_currentMenuName;

	bool _expectButtonOff;

	DrawFunction _curretDraw;

	CRotaryButton<rotarypos_t, ROTARY_ACCURACY> _button;

	static const SPageDef _pagedef[];

	void SetDefaultPage();
	void SetMenuPage();

	void Beep();
	static bool SendCommand(const __FlashStringHelper* cmd);
	static bool SendCommand(char* cmd);


	void ButtonPress();

	void ButtonPressStartSDPage();
	void ButtonPressPause();
	void ButtonPressMenuPage();
	void ButtonPressShowMenu();

	bool DrawLoopSplash(bool setup);
	bool DrawLoopDebug(bool setup);	
	bool DrawLoopPosAbs(bool setup);
	bool DrawLoopPreset(bool setup);
	bool DrawLoopStartSD(bool setup);
	bool DrawLoopPause(bool setup);	
	bool DrawLoopError(bool setup);
	bool DrawLoopMenu(bool setup);

	bool DrawLoopSetupDefault();
	void DrawLoopDefaultHead();

	// Menu Page

	unsigned char GetMenuIdx();
	unsigned char GetMenuCount();

	char* AddAxisName(char*buffer, axis_t axis);

	void MenuButtonPressG92Clear(const struct SMenuItemDef*)				{ SendCommand(F("g92")); Beep(); }
	void MenuButtonPressEnd(const struct SMenuItemDef*);

	void MenuButtonPressHomeA(axis_t axis);
	void MenuButtonPressHome(const struct SMenuItemDef*);
	void MenuButtonPressProbe(const struct SMenuItemDef*);
	void MenuButtonPressSpindle(const struct SMenuItemDef*);
	void MenuButtonPressCoolant(const struct SMenuItemDef*);

	void MenuButtonPressMoveNextAxis(const struct SMenuItemDef*);
	void MenuButtonPressMoveG92(const struct SMenuItemDef*);
	void MenuButtonPressMove(const struct SMenuItemDef*);
	void MenuButtonPressMoveBack(const struct SMenuItemDef*);

	void MenuButtonPressRotate(const struct SMenuItemDef*);
	void MenuButtonPressRotateBack(const struct SMenuItemDef*);

	void MenuButtonPressSDInit(const struct SMenuItemDef*)				{ SendCommand(F("m21")); Beep(); }
	void MenuButtonPressSDBack(const struct SMenuItemDef*);

	void MenuButtonPressSetMenu(const struct SMenuItemDef*);

	void MenuButtonPressExtraBack(const struct SMenuItemDef*);

	void MenuButtonPressSetMoveA(axis_t axis);
	void MenuButtonPressSetMove(const struct SMenuItemDef*);
	void MenuButtonPressSetRotate(const struct SMenuItemDef*);
	void MenuButtonPressSetSD(const struct SMenuItemDef*);
	void MenuButtonPressSetExtra(const struct SMenuItemDef*);

	enum EMoveType
	{
		MoveP10,
		MoveP1,
		MoveP01,
		MoveP001,
		MoveM10,
		MoveM1,
		MoveM01,
		MoveM001,
		MoveHome
	};

	void SetMenu(const SMenuDef* pMenu,const __FlashStringHelper* name)				{ _currentMenu = pMenu; _currentMenuName = name; _currentMenuIdx = 0; _currentMenuOffset = 0; };
	void SetMenu(const SMenuDef* pMenu)												{ SetMenu(pMenu, pMenu->GetText()); };
	void SetMainMenu()																{ SetMenu(&_mainMenu); }

	static const SMenuDef _mainMenu PROGMEM;
	static const SMenuDef _moveMenu PROGMEM;
	static const SMenuDef _rotateMenu PROGMEM;
	static const SMenuDef _SDMenu PROGMEM;
	static const SMenuDef _extraMenu PROGMEM;

	static const SMenuItemDef _mainMenuItems[] PROGMEM;
	static const SMenuItemDef _moveMenuItems[] PROGMEM;
	static const SMenuItemDef _rotateMenuItems[] PROGMEM;
	static const SMenuItemDef _SDMenuItems[] PROGMEM;
	static const SMenuItemDef _extraMenuItems[] PROGMEM;

#if defined(__AVR_ARCH__)

        static ButtonFunction GetButtonPress_P(const void* adr);
        static MenuButtonFunction GetMenuButtonPress_P(const void* adr);
        static DrawFunction GetDrawFunction_P(const void* adr);

#endif
};

////////////////////////////////////////////////////////

extern CMyLcd Lcd;
