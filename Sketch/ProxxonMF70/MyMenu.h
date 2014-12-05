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

#include "MenuBase.h"

////////////////////////////////////////////////////////

class CMyLcd;

class CMyMenu : public CMenuBase
{
private:

	typedef CMenuBase super;

public:

	static CMyLcd* GetLcd() { return ((CMyLcd*) CLcd::GetInstance()); }

	void SetMainMenu()																{ SetMenu(&_mainMenu); }

protected:

	virtual void Changed();
	virtual void Beep();

	void MenuButtonPressEnd(const SMenuItemDef*);

	void MenuButtonPressHomeA(axis_t axis);
	void MenuButtonPressHome(const SMenuItemDef*);
	void MenuButtonPressProbe(const SMenuItemDef*);
	void MenuButtonPressSpindle(const SMenuItemDef*);
	void MenuButtonPressCoolant(const SMenuItemDef*);

	void MenuButtonPressMoveNextAxis(const SMenuItemDef*);
	void MenuButtonPressMoveG92(const SMenuItemDef*);

	void MenuButtonPressMove(const SMenuItemDef*);
	void MenuButtonPressMoveBack(const SMenuItemDef*);

	void MenuButtonPressRotate(const SMenuItemDef*);

	void MenuButtonPressSetCommand(const SMenuItemDef*def)		{ SendCommand((const __FlashStringHelper*) def->GetParam1()); }

	bool SendCommand(const __FlashStringHelper*);
	bool SendCommand(char*);

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

	static const SMenuDef _mainMenu PROGMEM;
	static const SMenuDef _moveXMenu PROGMEM;
	static const SMenuDef _moveYMenu PROGMEM;
	static const SMenuDef _moveZMenu PROGMEM;
	static const SMenuDef _moveAMenu PROGMEM;
	static const SMenuDef _moveBMenu PROGMEM;
	static const SMenuDef _moveCMenu PROGMEM;
	static const SMenuDef _rotateMenu PROGMEM;
	static const SMenuDef _SDMenu PROGMEM;
	static const SMenuDef _extraMenu PROGMEM;

	static const SMenuItemDef _mainMenuItems[] PROGMEM;
	static const SMenuItemDef _moveMenuItems[] PROGMEM;
	static const SMenuItemDef _rotateMenuItems[] PROGMEM;
	static const SMenuItemDef _SDMenuItems[] PROGMEM;
	static const SMenuItemDef _extraMenuItems[] PROGMEM;

};

