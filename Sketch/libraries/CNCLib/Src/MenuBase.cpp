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

char* CMenuBase::AddAxisName(char*buffer, axis_t axis)
{
	const char* axisname=NULL;
	switch (axis)
	{
		case X_AXIS:	axisname = PSTR("X"); break;
		case Y_AXIS:	axisname = PSTR("Y"); break;
		case Z_AXIS:	axisname = PSTR("Z"); break;
		case A_AXIS:	axisname = PSTR("A"); break;
		case B_AXIS:	axisname = PSTR("B"); break;
		case C_AXIS:	axisname = PSTR("C"); break;
	}
	if (axisname)
		strcat_P(buffer, axisname);
	
	return buffer;
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
