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

#define _CRT_SECURE_NO_WARNINGS

////////////////////////////////////////////////////////////

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <CNCLib.h>

#include "MyLcd.h"
#include "MyMenu.h"
#include "MyControl.h"
#include <MenuBaseText.h>

////////////////////////////////////////////////////////////

void CMyMenu::Changed()
{
	GetLcd()->MenuChanged();
}

////////////////////////////////////////////////////////////

void CMyMenu::MenuButtonPressEnd(const SMenuItemDef*)
{
	SetMainMenu();
	GetLcd()->SetDefaultPage();
	OKBeep();
}

////////////////////////////////////////////////////////////

void CMyMenu::MenuButtonPressMoveNextAxis(const SMenuItemDef*def)
{
	uint8_t old = GetPosition();

	axis_t axis = (axis_t) (unsigned int) GetMenuDef()->GetParam1();
	axis = (axis + ((int) def->GetParam1()) + LCD_NUMAXIS) % LCD_NUMAXIS;

	const SMenuDef* nextMenu = (const SMenuDef*) _mainMenuItems[axis].GetParam1();

	SetMenu(nextMenu);
	SetPosition(old);

	Changed();
}

////////////////////////////////////////////////////////////

void CMyMenu::MenuButtonPressFuerElise(const SMenuItemDef* /* def */)
{
	CLcd::GetInstance()->Beep(SPlayTone::PlayInfo,true);
}

////////////////////////////////////////////////////////////
// Main Menu

#ifndef _MSC_VER
#pragma GCC diagnostic ignored "-Wmissing-field-initializers"
#endif

const CMyMenu::SMenuItemDef CMyMenu::_mainMenuItems[] PROGMEM =
{
	{ _mMoveX, &CMenuBase::MenuButtonPressSetMenu, (menuparam_t) &_moveXMenu },
#if LCD_NUMAXIS > 1
	{ _mMoveY, &CMenuBase::MenuButtonPressSetMenu, (menuparam_t) &_moveYMenu },
#if LCD_NUMAXIS > 2
	{ _mMoveZ, &CMenuBase::MenuButtonPressSetMenu, (menuparam_t) &_moveZMenu },
/*
#if LCD_NUMAXIS > 3
	{ _mMoveA, &CMenuBase::MenuButtonPressSetMenu, (menuparam_t) &_moveAMenu },
#if LCD_NUMAXIS > 4
	{ _mMoveB, &CMenuBase::MenuButtonPressSetMenu, (menuparam_t) &_moveBMenu },
#if LCD_NUMAXIS > 5
	{ _mMoveC, &CMenuBase::MenuButtonPressSetMove, (menuparam_t) &_moveCMenu },
#endif 
#endif
#endif
*/
#endif
#endif
	{ _mRotate, &CMenuBase::MenuButtonPressSetMenu, (menuparam_t) &_rotateMenu },
	{ _mSD, &CMenuBase::MenuButtonPressSetMenu, (menuparam_t) &_SDMenu },
	{ _mExtra, &CMenuBase::MenuButtonPressSetMenu, (menuparam_t) &_extraMenu },
	{ _mEnd, (MenuFunction) &CMyMenu::MenuButtonPressEnd },
	{ NULL, 0 }
};

////////////////////////////////////////////////////////////
// Move Menu

const CMyMenu::SMenuItemDef CMyMenu::_moveMenuItems[] PROGMEM =
{
	{ _mNextAxis, (MenuFunction) &CMyMenu::MenuButtonPressMoveNextAxis, (menuparam_t)1 },
	{ _mPrevAxis, (MenuFunction) &CMyMenu::MenuButtonPressMoveNextAxis, (menuparam_t)-1 },
	{ _mP100, 	&CMenuBase::MenuButtonPressMove, (menuparam_t)MoveP100 },
	{ _mP10,	&CMenuBase::MenuButtonPressMove, (menuparam_t)MoveP10 },
	{ _mP1,		&CMenuBase::MenuButtonPressMove, (menuparam_t)MoveP1 },
	{ _mP01,	&CMenuBase::MenuButtonPressMove, (menuparam_t)MoveP01 },
	{ _mP001,	&CMenuBase::MenuButtonPressMove, (menuparam_t)MoveP001 },
	{ _mM001,	&CMenuBase::MenuButtonPressMove, (menuparam_t)MoveM001 },
	{ _mM01,	&CMenuBase::MenuButtonPressMove, (menuparam_t)MoveM01 },
	{ _mM1,		&CMenuBase::MenuButtonPressMove, (menuparam_t)MoveM1 },
	{ _mM10,	&CMenuBase::MenuButtonPressMove, (menuparam_t)MoveM10 },
	{ _mM100, 	&CMenuBase::MenuButtonPressMove, (menuparam_t)MoveM100 },
	{ _mHome,	&CMenuBase::MenuButtonPressMove, (menuparam_t)MoveHome },
	{ _mG92,	&CMenuBase::MenuButtonPressMoveG92 },
	{ _mBack,   &CMenuBase::MenuButtonPressMenuBack, (menuparam_t) &_mainMenu },
	{ NULL, 0 }
};

////////////////////////////////////////////////////////////
// Rotate Menu

const CMyMenu::SMenuItemDef CMyMenu::_rotateMenuItems[] PROGMEM =
{
	{ _mRClr, &CMenuBase::MenuButtonPressRotate, (menuparam_t)RotateClear },
	{ _mR0,   &CMenuBase::MenuButtonPressRotate, (menuparam_t)RotateOffset },
	{ _mRYZ,  &CMenuBase::MenuButtonPressRotate, (menuparam_t)RotateSetYZ },
	{ _mRX,   &CMenuBase::MenuButtonPressRotate, (menuparam_t)RotateSetX },
	{ _mBack, &CMenuBase::MenuButtonPressMenuBack, (menuparam_t) &_mainMenu },
	{ NULL, 0 }
};

////////////////////////////////////////////////////////////
// SD Menu

const CMyMenu::SMenuItemDef CMyMenu::_SDMenuItems[] PROGMEM =
{
	{ _mSDInit, &CMenuBase::MenuButtonPressSetCommand, (menuparam_t) _m21 },
	{ _mBack, &CMenuBase::MenuButtonPressMenuBack, (menuparam_t) &_mainMenu },
	{ NULL, 0 }
};

////////////////////////////////////////////////////////////
// Extra Menu

const CMyMenu::SMenuItemDef CMyMenu::_extraMenuItems[] PROGMEM =
{
	{ _mG92Clear,&CMenuBase::MenuButtonPressSetCommand, (menuparam_t) _g92 },
	{ _mHomeZ,   &CMenuBase::MenuButtonPressHome, (menuparam_t)Z_AXIS },
	{ _mProbeZ,	 &CMenuBase::MenuButtonPressProbe, (menuparam_t)Z_AXIS },
	{ _mSpindle, &CMenuBase::MenuButtonPressSpindle },
	{ _mCoolant, &CMenuBase::MenuButtonPressCoolant },
	{ _mFuerElise, (MenuFunction) &CMyMenu::MenuButtonPressFuerElise },
	{ _mResurrect, (MenuFunction) &CMyMenu::MenuButtonPressResurrect },
	{ _mHold,	(MenuFunction)&CMyMenu::MenuButtonPressHold },
	{ _mResume,	(MenuFunction)&CMyMenu::MenuButtonPressResume },
	{ _mBack,	 &CMenuBase::MenuButtonPressMenuBack, (menuparam_t) &_mainMenu },
	{ NULL, 0 }
};

////////////////////////////////////////////////////////////

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
