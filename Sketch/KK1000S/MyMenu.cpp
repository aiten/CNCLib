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

#define _CRT_SECURE_NO_WARNINGS

////////////////////////////////////////////////////////////

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <CNCLib.h>
#include <CNCLibEx.h>

#include "MyLcd.h"
#include "MyMenu.h"
#include "MyControl.h"
#include "RotaryButton.h"
#include "GCodeParser.h"
#include "GCode3DParser.h"

////////////////////////////////////////////////////////////

void CMyMenu::Changed()
{
	GetLcd()->MenuChanged();
}

////////////////////////////////////////////////////////////

void CMyMenu::Beep()
{
	GetLcd()->Beep();
}

////////////////////////////////////////////////////////////

void CMyMenu::MenuButtonPressEnd(const SMenuItemDef*)
{
	SetMainMenu();
	GetLcd()->SetDefaultPage();
	GetLcd()->Beep();
}

////////////////////////////////////////////////////////////

void CMyMenu::MenuButtonPressMoveBack(const SMenuItemDef*)
{
	const struct SMenuDef* oldMenu = GetMenuDef();
	SetMainMenu();
	SetPosition(GetMenuDef()->FindMenuIdx(oldMenu, [] (const SMenuItemDef* def, const void*param) -> bool
	{
		return def->GetParam1() == param && def->GetButtonPress() == &CMyMenu::MenuButtonPressSetMenu;
	}));

	Changed();
}

////////////////////////////////////////////////////////////

void CMyMenu::MenuButtonPressMoveNextAxis(const SMenuItemDef*def)
{
	unsigned char old = GetPosition();

	axis_t axis = (axis_t) (unsigned int) GetMenuDef()->GetParam1();
	axis = (axis + ((int) def->GetParam1()) + LCD_NUMAXIS) % LCD_NUMAXIS;

	const SMenuDef* nextMenu = (const SMenuDef*) _mainMenuItems[axis].GetParam1();

	SetMenu(nextMenu);
	SetPosition(old);

	Changed();
}

////////////////////////////////////////////////////////////

void CMyMenu::MenuButtonPressMove(const SMenuItemDef*def)
{
	axis_t axis = (axis_t) (unsigned int) GetMenuDef()->GetParam1();
	unsigned char dist = (unsigned char)(unsigned int)def->GetParam1();

	if (dist == MoveHome) { MenuButtonPressHomeA(axis); return; }

	char tmp[24];

	strcpy_P(tmp, PSTR("g91 g0 "));
	AddAxisName(tmp, axis);

	switch (dist)
	{
		case MoveP100:	strcat_P(tmp, PSTR("100")); break;
		case MoveP10:	strcat_P(tmp, PSTR("10")); break;
		case MoveP1:	strcat_P(tmp, PSTR("1")); break;
		case MoveP01:	strcat_P(tmp, PSTR("0.1")); break;
		case MoveP001:	strcat_P(tmp, PSTR("0.01")); break;
		case MoveM001:	strcat_P(tmp, PSTR("-0.01")); break;
		case MoveM01:	strcat_P(tmp, PSTR("-0.1")); break;
		case MoveM1:	strcat_P(tmp, PSTR("-1")); break;
		case MoveM10:	strcat_P(tmp, PSTR("-10")); break;
		case MoveM100:  strcat_P(tmp, PSTR("-100")); break;
	}

	strcat_P(tmp, PSTR(" g90"));

	CControl::GetInstance()->PostCommand(tmp);
}

////////////////////////////////////////////////////////////

void CMyMenu::MenuButtonPressRotate(const SMenuItemDef*)
{
}

////////////////////////////////////////////////////////////

void CMyMenu::MenuButtonPressProbe(const SMenuItemDef*)
{
	if (CControl::GetInstance()->PostCommand(F("g91 g31 Z-10 F100 g90")))
	{
		CControl::GetInstance()->PostCommand(F("g92 Z-25"));
		GetLcd()->SetDefaultPage();
		CControl::GetInstance()->PostCommand(F("g91 Z3 g90"));
	}
}

////////////////////////////////////////////////////////////

void CMyMenu::MenuButtonPressHome(const SMenuItemDef*def)
{
	MenuButtonPressHomeA((axis_t)(unsigned int)def->GetParam1());
}

void CMyMenu::MenuButtonPressHomeA(axis_t axis)
{
	char tmp[16];

	strcpy_P(tmp, PSTR("g53 g0"));
	AddAxisName(tmp, axis);

	switch (axis)
	{
		case Z_AXIS: strcat_P(tmp, PSTR("#5163")); break;
		default: strcat_P(tmp, PSTR("0")); break;
	}
	CControl::GetInstance()->PostCommand(tmp);
};

////////////////////////////////////////////////////////////

void CMyMenu::MenuButtonPressMoveG92(const SMenuItemDef*)
{
	char tmp[24];

	axis_t axis = (axis_t) (unsigned int) GetMenuDef()->GetParam1();

	strcpy_P(tmp, PSTR("g92 "));
	AddAxisName(tmp, axis);
	strcat_P(tmp, PSTR("0"));

	CControl::GetInstance()->PostCommand(tmp);
	GetLcd()->Beep();
}

////////////////////////////////////////////////////////////

void CMyMenu::MenuButtonPressSpindle(const SMenuItemDef*)
{
	if (Control.IOControl(CMyControl::Spindel)!=0)
		CControl::GetInstance()->PostCommand(F("m5"));
	else
		CControl::GetInstance()->PostCommand(F("m3"));
}

////////////////////////////////////////////////////////////

void CMyMenu::MenuButtonPressCoolant(const SMenuItemDef*)
{
	if (Control.IOControl(CMyControl::Coolant)!=0)
		CControl::GetInstance()->PostCommand(F("m9"));
	else
		CControl::GetInstance()->PostCommand(F("m7"));
}

////////////////////////////////////////////////////////////

static const char _g92[] PROGMEM		= "g92";
static const char _m21[] PROGMEM		= "m21";

////////////////////////////////////////////////////////////
// Main Menu


static const char _mMoveX[] PROGMEM		= "Move X             >";
static const char _mMoveY[] PROGMEM		= "Move Y             >";
static const char _mMoveZ[] PROGMEM		= "Move Z             >";
static const char _mMoveA[] PROGMEM		= "Move A             >";
static const char _mMoveB[] PROGMEM		= "Move B             >";
static const char _mMoveC[] PROGMEM		= "Move C             >";
static const char _mRotate[] PROGMEM	= "Rotate             >";
static const char _mSD[] PROGMEM	    = "SD                 >";
static const char _mExtra[] PROGMEM		= "Extra              >";
static const char _mBack[] PROGMEM		= "Back               <";
static const char _mEnd[] PROGMEM		= "End";

const CMyMenu::SMenuItemDef CMyMenu::_mainMenuItems[] PROGMEM =
{
	{ _mMoveX, &CMenuBase::MenuButtonPressSetMenu, (menuparam_t) &_moveXMenu },
#if LCD_NUMAXIS > 1
	{ _mMoveY, &CMenuBase::MenuButtonPressSetMenu, (menuparam_t) &_moveYMenu },
#if LCD_NUMAXIS > 2
	{ _mMoveZ, &CMenuBase::MenuButtonPressSetMenu, (menuparam_t) &_moveZMenu },
#if LCD_NUMAXIS > 3
	{ _mMoveA, &CMenuBase::MenuButtonPressSetMenu, (menuparam_t) &_moveAMenu },
#if LCD_NUMAXIS > 4
	{ _mMoveB, &CMenuBase::MenuButtonPressSetMenu, (menuparam_t) &_moveBMenu },
#if LCD_NUMAXIS > 5
	{ _mMoveC, &CMenuBase::MenuButtonPressSetMove, (menuparam_t) &_moveCMenu },
#endif 
#endif
#endif
#endif
#endif
	{ _mRotate, &CMenuBase::MenuButtonPressSetMenu, &_rotateMenu },
	{ _mSD, &CMenuBase::MenuButtonPressSetMenu, &_SDMenu },
	{ _mExtra, &CMenuBase::MenuButtonPressSetMenu, &_extraMenu },
	{ _mEnd, (MenuFunction) &CMyMenu::MenuButtonPressEnd },
	{ NULL, 0 }
};

////////////////////////////////////////////////////////////
// Move Menu

static const char _mNextAxis[] PROGMEM		= "Next axis";
static const char _mPrevAxis[] PROGMEM		= "Prev axis";
static const char _mP100[] PROGMEM	= "+100";
static const char _mP10[] PROGMEM	= "+10";
static const char _mP1[] PROGMEM	= "+1";
static const char _mP01[] PROGMEM	= "+0.1";
static const char _mP001[] PROGMEM	= "+0.01";
static const char _mM001[] PROGMEM = "-0.01";
static const char _mM01[] PROGMEM = "-0.1";
static const char _mM1[] PROGMEM	= "-1";
static const char _mM10[] PROGMEM	= "-10";
static const char _mM100[] PROGMEM	= "-100";
static const char _mHome[] PROGMEM	= "Home";
static const char _mG92[] PROGMEM	= "Zero Offset(G92)";

const CMyMenu::SMenuItemDef CMyMenu::_moveMenuItems[] PROGMEM =
{
	{ _mNextAxis, (MenuFunction) &CMyMenu::MenuButtonPressMoveNextAxis, (menuparam_t)1 },
	{ _mPrevAxis, (MenuFunction) &CMyMenu::MenuButtonPressMoveNextAxis, (menuparam_t)-1 },
	{ _mP100, (MenuFunction) &CMyMenu::MenuButtonPressMove, (menuparam_t)MoveP100 },
	{ _mP10, (MenuFunction) &CMyMenu::MenuButtonPressMove, (menuparam_t)MoveP10 },
	{ _mP1, (MenuFunction) &CMyMenu::MenuButtonPressMove, (menuparam_t)MoveP1 },
	{ _mP01, (MenuFunction) &CMyMenu::MenuButtonPressMove, (menuparam_t)MoveP01 },
	{ _mP001, (MenuFunction) &CMyMenu::MenuButtonPressMove, (menuparam_t)MoveP001 },
	{ _mM001, (MenuFunction) &CMyMenu::MenuButtonPressMove, (menuparam_t)MoveM001 },
	{ _mM01, (MenuFunction) &CMyMenu::MenuButtonPressMove, (menuparam_t)MoveM01 },
	{ _mM1, (MenuFunction) &CMyMenu::MenuButtonPressMove, (menuparam_t)MoveM1 },
	{ _mM10, (MenuFunction) &CMyMenu::MenuButtonPressMove, (menuparam_t)MoveM10 },
	{ _mM100, (MenuFunction) &CMyMenu::MenuButtonPressMove, (menuparam_t)MoveM100 },
	{ _mHome, (MenuFunction) &CMyMenu::MenuButtonPressMove, (menuparam_t)MoveHome },
	{ _mG92, (MenuFunction) &CMyMenu::MenuButtonPressMoveG92 },
	{ _mBack, (MenuFunction) &CMyMenu::MenuButtonPressMoveBack },
	{ NULL, 0 }
};

////////////////////////////////////////////////////////////
// Rotate Menu

static const char _mR0[] PROGMEM = "Rotation 0";
static const char _mRX[] PROGMEM = "Shift X";
static const char _mRY[] PROGMEM = "Shift Y";
static const char _mRZ[] PROGMEM = "Shift Z";

const CMyMenu::SMenuItemDef CMyMenu::_rotateMenuItems[] PROGMEM =
{
	{ _mR0, (MenuFunction)  &CMyMenu::MenuButtonPressRotate, (menuparam_t) -1 },
	{ _mRX, (MenuFunction) &CMyMenu::MenuButtonPressRotate, (menuparam_t)X_AXIS },
	{ _mRY, (MenuFunction) &CMyMenu::MenuButtonPressRotate, (menuparam_t)Y_AXIS },
	{ _mRZ, (MenuFunction) &CMyMenu::MenuButtonPressRotate, (menuparam_t)Z_AXIS },
	{ _mBack, &CMenuBase::MenuButtonPressSetMenu, &_mainMenu, &_rotateMenu },
	{ NULL, 0 }
};

////////////////////////////////////////////////////////////
// SD Menu

static const char _mSDInit[] PROGMEM = "Init Card";

const CMyMenu::SMenuItemDef CMyMenu::_SDMenuItems[] PROGMEM =
{
	{ _mSDInit, &CMenuBase::MenuButtonPressSetCommand, (menuparam_t) _m21 },
	{ _mBack, &CMenuBase::MenuButtonPressSetMenu, &_mainMenu, &_SDMenu },
	{ NULL, 0 }
};

////////////////////////////////////////////////////////////
// Extra Menu

static const char _mSpindle[] PROGMEM	= "Spindle On/Off";
static const char _mCoolant[] PROGMEM	= "Coolant On/Off";
static const char _mHomeZ[] PROGMEM		= "Home Z";
static const char _mProbeZ[] PROGMEM	= "Probe Z";
static const char _mG92Clear[] PROGMEM  = "G92 Clear";

const CMyMenu::SMenuItemDef CMyMenu::_extraMenuItems[] PROGMEM =
{
	{ _mG92Clear, &CMenuBase::MenuButtonPressSetCommand, (menuparam_t) _g92 },
	{ _mHomeZ, (MenuFunction)&CMyMenu::MenuButtonPressHome, (menuparam_t)Z_AXIS },
	{ _mProbeZ,(MenuFunction) &CMyMenu::MenuButtonPressProbe, (menuparam_t)Z_AXIS },
	{ _mSpindle, (MenuFunction)&CMyMenu::MenuButtonPressSpindle },
	{ _mCoolant, (MenuFunction)&CMyMenu::MenuButtonPressCoolant },
	{ _mBack, &CMenuBase::MenuButtonPressSetMenu, &_mainMenu, &_extraMenu },
	{ NULL, 0 }
};

////////////////////////////////////////////////////////////

static const char _mmMain[] PROGMEM	 = "Main";
static const char _mmMoveX[] PROGMEM = "Move X";
static const char _mmMoveY[] PROGMEM = "Move Y";
static const char _mmMoveZ[] PROGMEM = "Move Z";
static const char _mmMoveA[] PROGMEM = "Move A";
static const char _mmMoveB[] PROGMEM = "Move B";
static const char _mmMoveC[] PROGMEM = "Move C";
static const char _mmRotate[] PROGMEM = "Rotate";
static const char _mmSD[] PROGMEM		= "SD Card";
static const char _mmExtra[] PROGMEM	= "Extra";

const CMyMenu::SMenuDef CMyMenu::_mainMenu PROGMEM  = { _mmMain, _mainMenuItems };
const CMyMenu::SMenuDef CMyMenu::_moveXMenu PROGMEM = { _mmMoveX, _moveMenuItems, (menuparam_t)X_AXIS };
const CMyMenu::SMenuDef CMyMenu::_moveYMenu PROGMEM = { _mmMoveY, _moveMenuItems, (menuparam_t)Y_AXIS };
const CMyMenu::SMenuDef CMyMenu::_moveZMenu PROGMEM = { _mmMoveZ, _moveMenuItems, (menuparam_t)Z_AXIS };
const CMyMenu::SMenuDef CMyMenu::_moveAMenu PROGMEM = { _mmMoveA, _moveMenuItems, (menuparam_t)A_AXIS };
const CMyMenu::SMenuDef CMyMenu::_moveBMenu PROGMEM = { _mmMoveB, _moveMenuItems, (menuparam_t)B_AXIS };
const CMyMenu::SMenuDef CMyMenu::_moveCMenu PROGMEM = { _mmMoveC, _moveMenuItems, (menuparam_t)C_AXIS };
const CMyMenu::SMenuDef CMyMenu::_rotateMenu PROGMEM = { _mmRotate, _rotateMenuItems };
const CMyMenu::SMenuDef CMyMenu::_SDMenu PROGMEM	  = { _mmSD, _SDMenuItems };
const CMyMenu::SMenuDef CMyMenu::_extraMenu PROGMEM = { _mmExtra, _extraMenuItems };

////////////////////////////////////////////////////////////
