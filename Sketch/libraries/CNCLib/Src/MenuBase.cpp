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

#include "Control.h"
#include "MenuBase.h"

////////////////////////////////////////////////////////////

CMenuBase::MenuFunction CMenuBase::SMenuItemDef::GetButtonPress() const
{
#if defined(__AVR_ARCH__)

	struct ButtonFunctionWrapper
	{
		MenuFunction fnc;
	}x;

	memcpy_P(&x, &this->_buttonpress, sizeof(ButtonFunctionWrapper));

	return x.fnc;

#else

	return _buttonpress;

#endif
}

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

void CMenuBase::AdjustOffset(menupos_t firstline, menupos_t lastline)
{
	menupos_t pos = GetPosition();
	const menupos_t menuEntries = GetMenuDef()->GetItemCount();

	if (pos == 0)
	{
		SetOffset(0);				// first menuitem selected => move to first line
	}
	else if (pos - 1 < GetOffset())
	{
		SubOffset(GetOffset() - (pos - 1));
	}

	if (menuEntries >= lastline)
	{
		if (pos == menuEntries - 1)
		{
			AddOffset(pos + firstline - GetOffset() - lastline);	// last menuitem selected => move to last line
		}
		else if (((pos + 1) + firstline - GetOffset()) > lastline)
		{
			AddOffset((pos + 1) + firstline - GetOffset() - lastline);
		}
	}
}

////////////////////////////////////////////////////////////

uint8_t CMenuBase::ToPrintLine(menupos_t firstline, menupos_t lastline, menupos_t i)
{
	// return 255 if not to print

	uint8_t printtorow = i + firstline - GetOffset();	// may overrun => no need to check for minus
	if (printtorow >= firstline && printtorow <= lastline)
		return printtorow;

	return 255;
}

////////////////////////////////////////////////////////////

void CMenuBase::MenuButtonPressSetCommand(const SMenuItemDef*def)
{ 
	PostCommand((const __FlashStringHelper*)def->GetParam1()); 
}

////////////////////////////////////////////////////////////

void CMenuBase::MenuButtonPressSetMenu(const SMenuItemDef*def)
{
	const SMenuDef* newMenu = (const SMenuDef*) def->GetParam1();
	const SMenuDef* posMenu = (const SMenuDef*) def->GetParam2();
	SetMenu(newMenu);

	if (posMenu!=NULL)
	{
		// param2 != NULL => find index
		_position = newMenu->FindMenuIdx((ptr_t) def, [](const SMenuItemDef* def, ptr_t param) -> bool
		{
			return	def->GetButtonPress() == &CMenuBase::MenuButtonPressSetMenu &&			// must be setMenu
					def->GetParam1() == ((const SMenuItemDef*)param)->GetParam2();			// param1 or new menu ust be param2 of "Back from"
		});
	}

	Changed();
}

////////////////////////////////////////////////////////////

void CMenuBase::MenuButtonPressMenuBack(const SMenuItemDef* def)
{
	const SMenuDef* newMenu = (const SMenuDef*)def->GetParam1();
	const struct SMenuDef* oldMenu = GetMenuDef();
	
	SetMenu(newMenu);

	SetPosition(GetMenuDef()->FindMenuIdx((ptr_t) oldMenu, [](const SMenuItemDef* def, ptr_t oldMenu) -> bool
	{
		return def->GetParam1() == oldMenu && def->GetButtonPress() == &CMenuBase::MenuButtonPressSetMenu;
	}));

	Changed();
}

////////////////////////////////////////////////////////////

void CMenuBase::MenuButtonPressMove(const SMenuItemDef*def)
{
	axis_t axis = (axis_t)(unsigned int)GetMenuDef()->GetParam1();
	uint8_t dist = (uint8_t)(unsigned int)def->GetParam1();

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

	PostCommand(tmp);
}

////////////////////////////////////////////////////////////

void CMenuBase::MenuButtonPressRotate(const SMenuItemDef*def)
{
	uint8_t req = (uint8_t)(unsigned int)def->GetParam1();

	switch (req)
	{
		case RotateClear:		PostCommand(F("g68.10")); break;
		case RotateOffset:		PostCommand(F("g68.11")); break;
		case RotateSetYZ:		PostCommand(F("g68.13 j0k0")); break;
		case RotateSetX:		PostCommand(F("g68.14 i0")); break;
	}
}

////////////////////////////////////////////////////////////

void CMenuBase::MenuButtonPressProbe(const SMenuItemDef*)
{
	if (PostCommand(F("g91 g31 Z-10 F100 g90")))
	{
		PostCommand(F("g92 Z-25"));
		//GetLcd()->SetDefaultPage();
		PostCommand(F("g91 Z3 g90"));
	}
}

////////////////////////////////////////////////////////////

void CMenuBase::MenuButtonPressHome(const SMenuItemDef*def)
{
	MenuButtonPressHomeA((axis_t)(unsigned int)def->GetParam1());
}

void CMenuBase::MenuButtonPressHomeA(axis_t axis)
{
	char tmp[16];

	strcpy_P(tmp, PSTR("g53 g0"));
	AddAxisName(tmp, axis);

	switch (axis)
	{
		case Z_AXIS: strcat_P(tmp, PSTR("#5163")); break;
		default: strcat_P(tmp, PSTR("0")); break;
	}
	PostCommand(tmp);
};

////////////////////////////////////////////////////////////

void CMenuBase::MenuButtonPressMoveG92(const SMenuItemDef*)
{
	char tmp[24];

	axis_t axis = (axis_t)(unsigned int)GetMenuDef()->GetParam1();

	strcpy_P(tmp, PSTR("g92 "));
	AddAxisName(tmp, axis);
	strcat_P(tmp, PSTR("0"));

	PostCommand(tmp);
}

////////////////////////////////////////////////////////////

void CMenuBase::MenuButtonPressSpindle(const SMenuItemDef*)
{
	if (CControl::GetInstance()->IOControl(CControl::Spindel) != 0)
		PostCommand(F("m5"));
	else
		PostCommand(F("m3"));
}

////////////////////////////////////////////////////////////

void CMenuBase::MenuButtonPressCoolant(const SMenuItemDef*)
{
	if (CControl::GetInstance()->IOControl(CControl::Coolant) != 0)
		PostCommand(F("m9"));
	else
		PostCommand(F("m7"));
}

////////////////////////////////////////////////////////////

void CMenuBase::MenuButtonPressResurrect(const SMenuItemDef*)
{
	if (CControl::GetInstance()->IsKilled())
	{
		CControl::GetInstance()->Resurrect();
		CLcd::GetInstance()->OKBeep();
	}
	else
	{
		CLcd::GetInstance()->ErrorBeep();
	}
}

////////////////////////////////////////////////////////////

void CMenuBase::MenuButtonPressHold(const SMenuItemDef*)
{
	if (!CControl::GetInstance()->IsHold())
	{
		CControl::GetInstance()->Hold();
		CLcd::GetInstance()->OKBeep();
	}
	else
	{
		CLcd::GetInstance()->ErrorBeep();
	}
}

///////////////////////////////////////////////////////////

void CMenuBase::MenuButtonPressResume(const SMenuItemDef*)
{
	if (CControl::GetInstance()->IsHold())
	{
		CControl::GetInstance()->Resume();
		CLcd::GetInstance()->OKBeep();
	}
	else
	{
		CLcd::GetInstance()->ErrorBeep();
	}
}

////////////////////////////////////////////////////////////

char* CMenuBase::AddAxisName(char*buffer, axis_t axis)
{
	const char* axisname = NULL;
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
