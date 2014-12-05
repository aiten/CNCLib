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

#include "Control.h"
#include "MenuBase.h"

////////////////////////////////////////////////////////////


CMenuBase::MenuFunction CMenuBase::SMenuItemDef::GetButtonPress() const
{
#if defined(__AVR_ARCH__)
	return GetMenuButtonPress_P(&this->_buttonpress);
#else
	return _buttonpress;
#endif
}

////////////////////////////////////////////////////////////
#if defined(__AVR_ARCH__)


CMenuBase::MenuFunction CMenuBase::GetMenuButtonPress_P(const void* adr)
{
	struct ButtonFunctionWrapper
	{
		MenuFunction fnc;
	}x;

	memcpy_P(&x, adr, sizeof(ButtonFunctionWrapper));

	return x.fnc;
}

#endif

////////////////////////////////////////////////////////////

bool CMenuBase::Select()
{
	const SMenuItemDef* item = &GetMenuDef()->GetItems()[GetPosition()];
	MenuFunction fnc = item->GetButtonPress();
	if (fnc != NULL)
	{
		(this->*fnc)(item);
		return true;
	}

	return false;
}

////////////////////////////////////////////////////////////

void CMenuBase::AdjustPositionAndOffset(menupos_t firstline, menupos_t lastline)
{
	menupos_t x = GetPosition();
	const menupos_t menuEntries = GetMenuDef()->GetItemCount();

	if (x == 0)
	{
		SetOffset(0);				// first menuitem selected => move to first line
	}
	else if (x - 1 < GetOffset())
	{
		SubOffset(GetOffset() - (x - 1));
	}

	if (menuEntries >= lastline)
	{
		if (x == menuEntries - 1)
		{
			AddOffset(x + firstline - GetOffset() - lastline);	// last menuitem selected => move to last line
		}
		else if (((x + 1) + firstline - GetOffset()) > lastline)
		{
			AddOffset((x + 1) + firstline - GetOffset() - lastline);
		}
	}
}

////////////////////////////////////////////////////////////

unsigned char CMenuBase::ToPrintLine(menupos_t firstline, menupos_t lastline, menupos_t i)
{
	// return 255 if not to print

	unsigned char printtorow = i + firstline - GetOffset();	// may overrun => no need to check for minus
	if (printtorow >= firstline && printtorow <= lastline)
		return printtorow;

	return 255;
}

////////////////////////////////////////////////////////////

bool CMenuBase::SendCommand(const __FlashStringHelper* cmd)
{
	return CControl::GetInstance()->PostCommand(cmd,NULL);
}

////////////////////////////////////////////////////////////

bool CMenuBase::SendCommand(char* cmd)
{
	return CControl::GetInstance()->PostCommand(cmd,NULL);
}

////////////////////////////////////////////////////////////

void CMenuBase::MenuButtonPressSetMenu(const SMenuItemDef*def)
{
	const SMenuDef* newMenu = (const SMenuDef*) def->GetParam1();
	const SMenuDef* posMenu = (const SMenuDef*) def->GetParam2();
	SetMenu(newMenu);

	if (posMenu!=NULL)
	{
		_position = newMenu->FindMenuIdx(posMenu, [] (const SMenuItemDef* def, const void*param) -> bool
		{
			return def->GetParam1() == param;
		});
	}

	Changed();
}

////////////////////////////////////////////////////////////
