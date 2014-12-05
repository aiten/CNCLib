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
#include "MyControl.h"
#include "RotaryButton.h"
#include "GCodeParser.h"
#include "GCode3DParser.h"
#include <U8glib.h>

////////////////////////////////////////////////////////////
//
// used full graphic controller for Ramps 1.4
//
////////////////////////////////////////////////////////////

U8GLIB_ST7920_128X64_1X u8g(CAT(BOARDNAME,_ST7920_CLK_PIN), CAT(BOARDNAME,_ST7920_DAT_PIN), CAT(BOARDNAME,_ST7920_CS_PIN));	// SPI Com: SCK = en = 18, MOSI = rw = 16, CS = di = 17

////////////////////////////////////////////////////////////

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

static unsigned char ToRow(unsigned char row) { return  (row + 1)*(CharHeight); }
static unsigned char ToCol(unsigned char col) { return (col)*(CharWidth); }


////////////////////////////////////////////////////////////

CMyLcd Lcd;

////////////////////////////////////////////////////////////

PROGMEM const CMyLcd::SPageDef CMyLcd::_pagedef[] =
{
	{ &CMyLcd::DrawLoopSplash, &CMyLcd::ButtonPressShowMenu },
	{ &CMyLcd::DrawLoopDebug, &CMyLcd::ButtonPressShowMenu },
	{ &CMyLcd::DrawLoopPosAbs, &CMyLcd::ButtonPressShowMenu },
	{ &CMyLcd::DrawLoopPreset, &CMyLcd::ButtonPressShowMenu },
	{ &CMyLcd::DrawLoopStartSD, &CMyLcd::ButtonPressStartSDPage },
	{ &CMyLcd::DrawLoopPause, &CMyLcd::ButtonPressPause },
	{ &CMyLcd::DrawLoopError, &CMyLcd::ButtonPressShowMenu },
	{ &CMyLcd::DrawLoopMenu, &CMyLcd::ButtonPressMenuPage }
};

////////////////////////////////////////////////////////////

void CMyLcd::Init()
{
	CHAL::pinMode(CAT(BOARDNAME,_LCD_BEEPER), OUTPUT);
	HALFastdigitalWrite(CAT(BOARDNAME,_LCD_BEEPER), LOW);

	super::Init();

	CHAL::pinMode(ROTARY_ENC, INPUT_PULLUP);
	CHAL::pinMode(ROTARY_EN1, INPUT_PULLUP);
	CHAL::pinMode(ROTARY_EN2, INPUT_PULLUP);

	CHAL::pinMode(CAT(BOARDNAME,_LCD_KILL_PIN), INPUT_PULLUP);

	_button.Tick(HALFastdigitalRead(ROTARY_EN1), HALFastdigitalRead(ROTARY_EN2));

	SetMainMenu();
	SetDefaultPage();
}

////////////////////////////////////////////////////////////

void CMyLcd::SetDefaultPage()
{
	_currentpage = AbsPage;
	SetRotaryFocusMainPage();
}

////////////////////////////////////////////////////////////

void CMyLcd::SetMenuPage()
{
	_currentpage = MenuPage;
	SetRotaryFocusMenuPage();
}

////////////////////////////////////////////////////////////

void CMyLcd::Beep()
{
	for (int8_t i = 0; i < 10; i++)
	{
		HALFastdigitalWrite(CAT(BOARDNAME,_LCD_BEEPER), HIGH);
		delay(3);
		HALFastdigitalWrite(CAT(BOARDNAME,_LCD_BEEPER), LOW);
		delay(3);
	}
}

////////////////////////////////////////////////////////////

#ifdef _MSC_VER
CMyLcd::EPage CMyLcd::GetPage()
#else
EnumAsByte(CMyLcd::EPage) CMyLcd::GetPage()
#endif
{
	if (_rotaryFocus == RotaryMainPage)
	{
		EnumAsByte(EPage) page = (EnumAsByte(EPage))(_button.GetPageIdx(PageCount));

		if (page != _currentpage)
		{
			_currentpage = page;
			SetMainMenu();
		}
	}

	return _currentpage;
}

////////////////////////////////////////////////////////////

void CMyLcd::SetRotaryFocusMainPage()
{
	_button.SetPageIdx((rotarypos_t) _currentpage); _button.SetMinMax(0, PageCount - 1, true);
	_rotaryFocus = RotaryMainPage;
}

////////////////////////////////////////////////////////////

void CMyLcd::TimerInterrupt()
{
	super::TimerInterrupt();

	switch (_button.Tick(HALFastdigitalRead(ROTARY_EN1), HALFastdigitalRead(ROTARY_EN2)))
	{
		case CRotaryButton<rotarypos_t, ROTARY_ACCURACY>::Overrun:
			break;
		case CRotaryButton<rotarypos_t, ROTARY_ACCURACY>::Underflow:
			break;
	}
}

////////////////////////////////////////////////////////////

void CMyLcd::Idle(unsigned int idletime)
{
	super::Idle(idletime);

	if (_expectButtonOff)
	{
		if (HALFastdigitalRead(ROTARY_ENC) != ROTARY_ENC_ON)
			_expectButtonOff = false;
	}
	else if (HALFastdigitalRead(ROTARY_ENC) == ROTARY_ENC_ON)
	{
		_expectButtonOff = true;
		ButtonPress();
	}
}

////////////////////////////////////////////////////////////

#if defined(__AVR_ARCH__)

CMyLcd::ButtonFunction CMyLcd::GetButtonPress_P(const void* adr)
{
	struct ButtonFunctionWrapper
	{
		ButtonFunction fnc;
	}x;

	memcpy_P(&x, adr, sizeof(ButtonFunctionWrapper));

	return x.fnc;
}

CMyLcd::MenuButtonFunction CMyLcd::GetMenuButtonPress_P(const void* adr)
{
	struct ButtonFunctionWrapper
	{
		MenuButtonFunction fnc;
	}x;

	memcpy_P(&x, adr, sizeof(ButtonFunctionWrapper));

	return x.fnc;
}

CMyLcd::DrawFunction CMyLcd::GetDrawFunction_P(const void* adr)
{
	struct DrawFunctionWrapper
	{
		DrawFunction fnc;
	}x;

	memcpy_P(&x, adr, sizeof(DrawFunctionWrapper));

	return x.fnc;
}

#endif

////////////////////////////////////////////////////////////


void CMyLcd::ButtonPress()
{
#if defined(__AVR_ARCH__)
	ButtonFunction fnc = GetButtonPress_P(&_pagedef[GetPage()].buttonpress);
#else
	ButtonFunction fnc = _pagedef[GetPage()].buttonpress;
#endif

	if (fnc)
	{
		(*this.*fnc)();
		DrawLoop();
	}
}

////////////////////////////////////////////////////////////

unsigned long CMyLcd::Splash()
{
	DrawLoop(&CMyLcd::DrawLoopSplash);
	Beep();
	return 3000;
}

////////////////////////////////////////////////////////////

bool CMyLcd::DrawLoopSetupDefault()
{
	u8g.setFont(DEFAULTFONT);
	return true;
}

////////////////////////////////////////////////////////////

void CMyLcd::DrawLoopDefaultHead()
{
#ifdef USE_RAMPS14
#if defined(__SAM3X8E__)
	u8g.drawStr(ToCol(0), ToRow(0), F("Proxxon MF70 Ramps14S"));
#else
	u8g.drawStr(ToCol(0), ToRow(0), F("Proxxon MF70 Ramps14M"));
#endif
#else
#if defined(__SAM3X8E__)
	u8g.drawStr(ToCol(0), ToRow(0), F("Proxxon MF70 RampsFDS"));
#else
	u8g.drawStr(ToCol(0), ToRow(0), F("Proxxon MF70 RampsFDM"));
#endif
#endif
}

////////////////////////////////////////////////////////////

bool CMyLcd::DrawLoopSplash(bool setup)
{
	if (setup)	return DrawLoopSetupDefault();
	DrawLoopDefaultHead();

	u8g.drawStr(ToCol(TotalCols / 2 - 1), ToRow(2), F("by"));
	u8g.drawStr(ToCol(3), ToRow(3), F("H. Aitenbichler"));
	u8g.drawStr(ToCol(5), ToRow(5), F(__DATE__));

	return true;
}

////////////////////////////////////////////////////////////

bool CMyLcd::DrawLoopDebug(bool setup)
{
	if (setup)	return DrawLoopSetupDefault();

	u8g.drawStr(ToCol(0), ToRow(0) + HeadLineOffset, F("Debug"));

	char tmp[16];

	for (unsigned char i = 0; i < LCD_NUMAXIS; i++)
	{
		u8g.setPrintPos(ToCol(0), ToRow(i + 1) + PosLineOffset);

		udist_t pos = CStepper::GetInstance()->GetCurrentPosition(i);

		u8g.print(CSDist::ToString(pos, tmp, 6));
		u8g.print(F(" "));
		u8g.print(CMm1000::ToString(CMotionControlBase::GetInstance()->ToMm1000(i, pos), tmp, 6, 2));
		u8g.print(F(" "));

		u8g.print(CStepper::GetInstance()->IsReference(CStepper::GetInstance()->ToReferenceId(i, true)) ? '1' : '0');
		u8g.print(CStepper::GetInstance()->IsReference(CStepper::GetInstance()->ToReferenceId(i, false)) ? '1' : '0');

		u8g.print((CStepper::GetInstance()->GetLastDirection()&(1 << i)) ? '+' : '-');
	}

	u8g.setPrintPos(ToCol(20), ToRow(0 + 1) + PosLineOffset);
	u8g.print(Control.IOControl(CMyControl::Probe) ? '1' : '0');

	u8g.setPrintPos(ToCol(19), ToRow(0 + 2) + PosLineOffset);
	u8g.print(CSDist::ToString(CStepper::GetInstance()->QueuedMovements(), tmp, 2));

	return true;
}

////////////////////////////////////////////////////////////

bool CMyLcd::DrawLoopPosAbs(bool setup)
{
	if (setup)	return DrawLoopSetupDefault();

	u8g.setPrintPos(ToCol(0), ToRow(0) + HeadLineOffset); u8g.print(F("Absolut  Current"));
	char tmp[16];

	for (unsigned char i = 0; i < LCD_NUMAXIS; i++)
	{
//		udist_t cur = CStepper::GetInstance()->GetCurrentPosition(i);
		mm1000_t psall = CGCodeParser::GetAllPreset(i);

		u8g.setPrintPos(ToCol(0), ToRow(i + 1) + PosLineOffset);
		tmp[0] = 0; u8g.print(AddAxisName(tmp,i));

		u8g.print(CMm1000::ToString(CMotionControlBase::GetInstance()->GetPosition(i), tmp, 7, 2));
		u8g.print(F(" "));
		u8g.print(CMm1000::ToString(CMotionControlBase::GetInstance()->GetPosition(i) - psall, tmp, 7, 2));
/*
		u8g.print(CMm1000::ToString(CMotionControlBase::GetInstance()->ToMm1000(i, cur), tmp, 7, 2));
		u8g.print(F(" "));
		u8g.print(CMm1000::ToString(CMotionControlBase::GetInstance()->ToMm1000(i, cur) - psall, tmp, 7, 2));
*/
	}

	return true;
}
////////////////////////////////////////////////////////////

void CMyLcd::DrawLoop()
{
	if (_curretDraw && (this->*_curretDraw)(true))
	{
		u8g.firstPage();
		do
		{
			if (_curretDraw && !(this->*_curretDraw)(false))
				break;
		} while (u8g.nextPage());
	}
}

////////////////////////////////////////////////////////////

void CMyLcd::FirstDraw()
{
	DrawLoop(&CMyLcd::DrawLoopDebug);
}

////////////////////////////////////////////////////////////


void CMyLcd::Draw(EDrawType /* draw */)
{
#if defined(__AVR_ARCH__)
	DrawFunction fnc = GetDrawFunction_P(&_pagedef[GetPage()].draw);
#else
	DrawFunction fnc = _pagedef[GetPage()].draw;
#endif

	DrawLoop(fnc);
}

////////////////////////////////////////////////////////////

bool CMyLcd::SendCommand(const __FlashStringHelper* cmd)
{
	return Control.PostCommand(cmd,NULL);
}

////////////////////////////////////////////////////////////

bool CMyLcd::SendCommand(char* cmd)
{
	return Control.PostCommand(cmd,NULL);
}


////////////////////////////////////////////////////////////

void CMyLcd::ButtonPressShowMenu()
{
	SetMenuPage();
}

////////////////////////////////////////////////////////////

bool CMyLcd::DrawLoopPreset(bool setup)
{
	if (setup)	return DrawLoopSetupDefault();

	mm1000_t ps;

	const __FlashStringHelper* zeroShiftName[] PROGMEM = { F("G53"), F("G54"), F("G55"), F("G56"), F("G57"), F("G58"), F("G59") };

	u8g.setPrintPos(ToCol(0), ToRow(0) + HeadLineOffset);  u8g.print(F("Preset: ")); u8g.print(zeroShiftName[CGCodeParser::GetZeroPresetIdx()]); u8g.print(F(" G92 Height"));

	char tmp[16];

	for (unsigned char i = 0; i < LCD_NUMAXIS; i++)
	{
		u8g.setPrintPos(ToCol(0), ToRow(i + 1) + PosLineOffset);
		tmp[0] = 0; u8g.print(AddAxisName(tmp,i));
		ps = CGCodeParser::GetG54PosPreset(i);
		u8g.print(CMm1000::ToString(ps, tmp, 7, 2));

		ps = CGCodeParser::GetG92PosPreset(i);
		u8g.print(CMm1000::ToString(ps, tmp, 7, 2));

		ps = CGCodeParser::GetToolHeightPosPreset(i);
		u8g.print(CMm1000::ToString(ps, tmp, 6, 2));
	}
	return true;
}

////////////////////////////////////////////////////////////

void CMyLcd::ButtonPressStartSDPage()
{
	SendCommand(F("m21"));									// Init SD

	if (SendCommand(F("m23 proxxon.nc")))
	{
		SendCommand(F("m24"));
	}

	Beep();
}

////////////////////////////////////////////////////////////

bool CMyLcd::DrawLoopStartSD(bool setup)
{
	if (setup)	return DrawLoopSetupDefault();
	DrawLoopDefaultHead();

	u8g.drawStr(ToCol(3), ToRow(2), F("Press to start"));
	u8g.drawStr(ToCol(0), ToRow(3), F("\"proxxon.nc\" from SD"));

	return true;
}

////////////////////////////////////////////////////////////

void CMyLcd::ButtonPressPause()
{
	if (Control.IsPause())
		Control.Continue();
	else
		Control.Pause();

	Beep();
}

////////////////////////////////////////////////////////////

bool CMyLcd::DrawLoopPause(bool setup)
{
	if (setup)	return DrawLoopSetupDefault();
	DrawLoopDefaultHead();

	if (Control.IsPause())
		u8g.drawStr(ToCol(2), ToRow(2), F("Press to continue"));
	else
		u8g.drawStr(ToCol(3), ToRow(2), F("Press to pause"));

	return true;
}

////////////////////////////////////////////////////////////

bool CMyLcd::DrawLoopError(bool setup)
{
	if (setup)	return DrawLoopSetupDefault();
	DrawLoopDefaultHead();

	unsigned char errors = 0;

	if (CStepper::GetInstance()->GetError())
		u8g.drawStr(ToCol(0), ToRow(++errors + 1), CStepper::GetInstance()->GetError());
	if (Control.IsKilled())
		u8g.drawStr(ToCol(0), ToRow(++errors + 1), F("emergency stop"));

	if (errors == 0)
		u8g.drawStr(ToCol(0), ToRow(2), F("no errors"));

	return true;
}

////////////////////////////////////////////////////////////

CMyLcd::MenuButtonFunction CMyLcd::SMenuItemDef::GetButtonPress() const
{
#if defined(__AVR_ARCH__)
	return GetMenuButtonPress_P(&this->_buttonpress);
#else
	return _buttonpress;
#endif
}

////////////////////////////////////////////////////////////

unsigned char CMyLcd::GetMenuIdx()
{
	if (_rotaryFocus == RotaryMenuPage)
	{
		unsigned char menu = _button.GetPageIdx(_currentMenu->GetItemCount());
		if (menu != _currentMenuIdx)
		{
			_currentMenuIdx = menu;
		}
	}

	return _currentMenuIdx;
}

////////////////////////////////////////////////////////////

char* CMyLcd::AddAxisName(char*buffer, axis_t axis)
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

void CMyLcd::MenuButtonPressEnd(const struct SMenuItemDef*)
{
	SetMainMenu();
	SetDefaultPage();
	Beep();
}

////////////////////////////////////////////////////////////

void CMyLcd::MenuButtonPressSetMenu(const struct SMenuItemDef*def)
{
	const struct SMenuDef* newMenu = (const struct SMenuDef*) def->GetParam1();
	const struct SMenuDef* posMenu = (const struct SMenuDef*) def->GetParam2();
	SetMenu(newMenu);

	if (posMenu!=NULL)
	{
		_currentMenuIdx = newMenu->FindMenuIdx(posMenu, [] (const SMenuItemDef* def, const void*param) -> bool
		{
			return def->GetParam1() == param;
		});
	}

	SetRotaryFocusMenuPage();
	DrawLoop();
	Beep();
}

////////////////////////////////////////////////////////////

void CMyLcd::MenuButtonPressMoveBack(const struct SMenuItemDef*)
{
	const struct SMenuDef* oldMenu = _currentMenu;
	SetMainMenu();
	_currentMenuIdx = _currentMenu->FindMenuIdx(oldMenu, [] (const SMenuItemDef* def, const void*param) -> bool
	{
		return def->GetParam1() == param && def->GetButtonPress() == &CMyLcd::MenuButtonPressSetMenu;
	});

	SetRotaryFocusMenuPage();
	Beep();
}

////////////////////////////////////////////////////////////

void CMyLcd::MenuButtonPressMoveNextAxis(const struct SMenuItemDef*def)
{
	unsigned char old = _currentMenuIdx;

	axis_t axis = (axis_t) (unsigned int) _currentMenu->GetParam1();
	axis = (axis + ((int) def->GetParam1()) + LCD_NUMAXIS) % LCD_NUMAXIS;

	const SMenuDef* nextMenu = (const SMenuDef*) _mainMenuItems[axis].GetParam1();

	SetMenu(nextMenu);
	_currentMenuIdx = old;

	SetRotaryFocusMenuPage();
	DrawLoop();
	Beep();
}

////////////////////////////////////////////////////////////

void CMyLcd::MenuButtonPressMove(const struct SMenuItemDef*def)
{
	axis_t axis = (axis_t) (unsigned int) _currentMenu->GetParam1();
	unsigned char dist = (unsigned char)(unsigned int)def->GetParam1();

	if (dist == MoveHome) { MenuButtonPressHomeA(axis); return; }

	char tmp[24];

	strcpy_P(tmp, PSTR("g91 g0 "));
	AddAxisName(tmp, axis);

	switch (dist)
	{
		case MoveP10:	strcat_P(tmp, PSTR("10")); break;
		case MoveP1:	strcat_P(tmp, PSTR("1")); break;
		case MoveP01:	strcat_P(tmp, PSTR("0.1")); break;
		case MoveP001:	strcat_P(tmp, PSTR("0.01")); break;
		case MoveM001:	strcat_P(tmp, PSTR("-0.01")); break;
		case MoveM01:	strcat_P(tmp, PSTR("-0.1")); break;
		case MoveM1:	strcat_P(tmp, PSTR("-1")); break;
		case MoveM10:	strcat_P(tmp, PSTR("-10")); break;
	}

	strcat_P(tmp, PSTR(" g90"));

	SendCommand(tmp);
}

////////////////////////////////////////////////////////////

void CMyLcd::MenuButtonPressRotate(const struct SMenuItemDef*)
{
}

////////////////////////////////////////////////////////////

void CMyLcd::MenuButtonPressProbe(const struct SMenuItemDef*)
{
	if (SendCommand(F("g91 g31 Z-10 F100 g90")))
	{
		SendCommand(F("g92 Z-25"));
		SetDefaultPage();
		SendCommand(F("g91 Z3 g90"));
	}
}

////////////////////////////////////////////////////////////

void CMyLcd::MenuButtonPressHome(const struct SMenuItemDef*def)
{
	MenuButtonPressHomeA((axis_t)(unsigned int)def->GetParam1());
}

void CMyLcd::MenuButtonPressHomeA(axis_t axis)
{
	char tmp[16];

	strcpy_P(tmp, PSTR("g53 g0"));
	AddAxisName(tmp, axis);

	switch (axis)
	{
		case Z_AXIS: strcat_P(tmp, PSTR("#5163")); break;
		default: strcat_P(tmp, PSTR("0")); break;
	}
	SendCommand(tmp);
};

////////////////////////////////////////////////////////////

void CMyLcd::MenuButtonPressMoveG92(const struct SMenuItemDef*)
{
	char tmp[24];

	axis_t axis = (axis_t) (unsigned int) _currentMenu->GetParam1();

	strcpy_P(tmp, PSTR("g92 "));
	AddAxisName(tmp, axis);
	strcat_P(tmp, PSTR("0"));

	SendCommand(tmp);
	Beep();
}

////////////////////////////////////////////////////////////

void CMyLcd::MenuButtonPressSpindle(const struct SMenuItemDef*)
{
	if (Control.IOControl(CMyControl::Spindel)!=0)
		SendCommand(F("m5"));
	else
		SendCommand(F("m3"));
}

////////////////////////////////////////////////////////////

void CMyLcd::MenuButtonPressCoolant(const struct SMenuItemDef*)
{
	if (Control.IOControl(CMyControl::Coolant)!=0)
		SendCommand(F("m9"));
	else
		SendCommand(F("m7"));
}

////////////////////////////////////////////////////////////

void CMyLcd::SetRotaryFocusMenuPage()
{
	_button.SetPageIdx(_currentMenuIdx); _button.SetMinMax(0, _currentMenu->GetItemCount() - 1, false);
	_rotaryFocus = RotaryMenuPage;
}

////////////////////////////////////////////////////////////

void CMyLcd::ButtonPressMenuPage()
{
	switch (_rotaryFocus)
	{
		case RotaryMainPage:	SetRotaryFocusMenuPage(); Beep();  break;
		case RotaryMenuPage:
		{
			const struct SMenuItemDef* item = &_currentMenu->GetItems()[GetMenuIdx()];
			MenuButtonFunction fnc = item->GetButtonPress();
			if (fnc != NULL)
			{
				(this->*fnc)(item);
			}
			else
			{
				Beep(); Beep();
			}

			break;
		}
	}
}

////////////////////////////////////////////////////////////

bool CMyLcd::DrawLoopMenu(bool setup)
{
	if (setup)	return DrawLoopSetupDefault();

	u8g.setPrintPos(ToCol(0), ToRow(0) + HeadLineOffset);
	u8g.print(F("Menu: "));
	u8g.print(_currentMenuName);

	unsigned char x = 255;
	const unsigned char printFirstLine = 1;
	const unsigned char printLastLine = (TotalRows - 1);
	const unsigned char menuEntries = _currentMenu->GetItemCount();

	if (_rotaryFocus == RotaryMenuPage)
	{
		x = GetMenuIdx();

		if (x == 0)
		{
			_currentMenuOffset = 0;				// first menuitem selected => move to first line
		}
		else if (x - 1 < _currentMenuOffset)
		{
			_currentMenuOffset -= _currentMenuOffset - (x - 1);
		}

		if (menuEntries >= printLastLine)
		{
			if (x == menuEntries - 1)
			{
				_currentMenuOffset += x + printFirstLine - _currentMenuOffset - printLastLine;	// last menuitem selected => move to last line
			}
			else if (((x + 1) + printFirstLine - _currentMenuOffset) > printLastLine)
			{
				_currentMenuOffset += (x + 1) + printFirstLine - _currentMenuOffset - printLastLine;
			}
		}
	}
	unsigned char i;

	for (i = 0; i < menuEntries; i++)
	{
		unsigned char printtorow = i + printFirstLine - _currentMenuOffset;	// may overrun => no need to check for minus
		if (printtorow >= printFirstLine && printtorow <= printLastLine)
		{
			u8g.setPrintPos(ToCol(0), ToRow(printtorow) + PosLineOffset);
			if (i == x && _rotaryFocus == RotaryMenuPage)
				u8g.print(F(">"));
			else
				u8g.print(F(" "));


			u8g.print(_currentMenu->GetItems()[i].GetText());
		}
	}

	return true;
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

const CMyLcd::SMenuItemDef CMyLcd::_mainMenuItems[] PROGMEM =
{
	{ _mMoveX, &CMyLcd::MenuButtonPressSetMenu, (menuparam_t) &_moveXMenu },
#if LCD_NUMAXIS > 1
	{ _mMoveY, &CMyLcd::MenuButtonPressSetMenu, (menuparam_t) &_moveYMenu },
#if LCD_NUMAXIS > 2
	{ _mMoveZ, &CMyLcd::MenuButtonPressSetMenu, (menuparam_t) &_moveZMenu },
#if LCD_NUMAXIS > 3
	{ _mMoveA, &CMyLcd::MenuButtonPressSetMenu, (menuparam_t) &_moveAMenu },
#if LCD_NUMAXIS > 4
	{ _mMoveB, &CMyLcd::MenuButtonPressSetMenu, (menuparam_t) &_moveBMenu },
#if LCD_NUMAXIS > 5
	{ _mMoveC, &CMyLcd::MenuButtonPressSetMove, (menuparam_t) &_moveCMenu },
#endif 
#endif
#endif
#endif
#endif
	{ _mRotate, &CMyLcd::MenuButtonPressSetMenu, &_rotateMenu },
	{ _mSD, &CMyLcd::MenuButtonPressSetMenu, &_SDMenu },
	{ _mExtra, &CMyLcd::MenuButtonPressSetMenu, &_extraMenu },
	{ _mEnd, &CMyLcd::MenuButtonPressEnd },
	{ NULL, 0 }
};

////////////////////////////////////////////////////////////
// Move Menu

static const char _mNextAxis[] PROGMEM		= "Next axis";
static const char _mPrevAxis[] PROGMEM		= "Prev axis";
static const char _mP10[] PROGMEM	= "+10";
static const char _mP1[] PROGMEM	= "+1";
static const char _mP01[] PROGMEM	= "+0.1";
static const char _mP001[] PROGMEM	= "+0.01";
static const char _mM001[] PROGMEM = "-0.01";
static const char _mM01[] PROGMEM = "-0.1";
static const char _mM1[] PROGMEM	= "-1";
static const char _mM10[] PROGMEM	= "-10";
static const char _mHome[] PROGMEM	= "Home";
static const char _mG92[] PROGMEM	= "Zero Offset(G92)";

const CMyLcd::SMenuItemDef CMyLcd::_moveMenuItems[] PROGMEM =
{
	{ _mNextAxis, &CMyLcd::MenuButtonPressMoveNextAxis, (menuparam_t)1 },
	{ _mPrevAxis, &CMyLcd::MenuButtonPressMoveNextAxis, (menuparam_t)-1 },
	{ _mP10, &CMyLcd::MenuButtonPressMove, (menuparam_t)MoveP10 },
	{ _mP1, &CMyLcd::MenuButtonPressMove, (menuparam_t)MoveP1 },
	{ _mP01, &CMyLcd::MenuButtonPressMove, (menuparam_t)MoveP01 },
	{ _mP001, &CMyLcd::MenuButtonPressMove, (menuparam_t)MoveP001 },
	{ _mM001, &CMyLcd::MenuButtonPressMove, (menuparam_t)MoveM001 },
	{ _mM01, &CMyLcd::MenuButtonPressMove, (menuparam_t)MoveM01 },
	{ _mM1, &CMyLcd::MenuButtonPressMove, (menuparam_t)MoveM1 },
	{ _mM10, &CMyLcd::MenuButtonPressMove, (menuparam_t)MoveM10 },
	{ _mHome, &CMyLcd::MenuButtonPressMove, (menuparam_t)MoveHome },
	{ _mG92, &CMyLcd::MenuButtonPressMoveG92 },
	{ _mBack, &CMyLcd::MenuButtonPressMoveBack },
	{ NULL, 0 }
};

////////////////////////////////////////////////////////////
// Rotate Menu

static const char _mR0[] PROGMEM = "Rotation 0";
static const char _mRX[] PROGMEM = "Shift X";
static const char _mRY[] PROGMEM = "Shift Y";
static const char _mRZ[] PROGMEM = "Shift Z";

const CMyLcd::SMenuItemDef CMyLcd::_rotateMenuItems[] PROGMEM =
{
	{ _mR0, &CMyLcd::MenuButtonPressRotate, (menuparam_t) -1 },
	{ _mRX, &CMyLcd::MenuButtonPressRotate, (menuparam_t)X_AXIS },
	{ _mRY, &CMyLcd::MenuButtonPressRotate, (menuparam_t)Y_AXIS },
	{ _mRZ, &CMyLcd::MenuButtonPressRotate, (menuparam_t)Z_AXIS },
	{ _mBack, &CMyLcd::MenuButtonPressSetMenu, &_mainMenu, &_rotateMenu },
	{ NULL, 0 }
};

////////////////////////////////////////////////////////////
// SD Menu

static const char _mSDInit[] PROGMEM = "Init Card";

const CMyLcd::SMenuItemDef CMyLcd::_SDMenuItems[] PROGMEM =
{
	{ _mSDInit, &CMyLcd::MenuButtonPressSetCommand, (menuparam_t) _m21 },
	{ _mBack, &CMyLcd::MenuButtonPressSetMenu, &_mainMenu, &_SDMenu },
	{ NULL, 0 }
};

////////////////////////////////////////////////////////////
// Extra Menu

static const char _mSpindle[] PROGMEM	= "Spindle On/Off";
static const char _mCoolant[] PROGMEM	= "Coolant On/Off";
static const char _mHomeZ[] PROGMEM		= "Home Z";
static const char _mProbeZ[] PROGMEM	= "Probe Z";
static const char _mG92Clear[] PROGMEM  = "G92 Clear";

const CMyLcd::SMenuItemDef CMyLcd::_extraMenuItems[] PROGMEM =
{
	{ _mG92Clear, &CMyLcd::MenuButtonPressSetCommand, (menuparam_t) _g92 },
	{ _mHomeZ, &CMyLcd::MenuButtonPressHome, (menuparam_t)Z_AXIS },
	{ _mProbeZ, &CMyLcd::MenuButtonPressProbe, (menuparam_t)Z_AXIS },
	{ _mSpindle, &CMyLcd::MenuButtonPressSpindle },
	{ _mCoolant, &CMyLcd::MenuButtonPressCoolant },
	{ _mBack, &CMyLcd::MenuButtonPressSetMenu, &_mainMenu, &_extraMenu },
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

const CMyLcd::SMenuDef CMyLcd::_mainMenu PROGMEM  = { _mmMain, _mainMenuItems };
const CMyLcd::SMenuDef CMyLcd::_moveXMenu PROGMEM = { _mmMoveX, _moveMenuItems, (menuparam_t)X_AXIS };
const CMyLcd::SMenuDef CMyLcd::_moveYMenu PROGMEM = { _mmMoveY, _moveMenuItems, (menuparam_t)Y_AXIS };
const CMyLcd::SMenuDef CMyLcd::_moveZMenu PROGMEM = { _mmMoveZ, _moveMenuItems, (menuparam_t)Z_AXIS };
const CMyLcd::SMenuDef CMyLcd::_moveAMenu PROGMEM = { _mmMoveA, _moveMenuItems, (menuparam_t)A_AXIS };
const CMyLcd::SMenuDef CMyLcd::_moveBMenu PROGMEM = { _mmMoveB, _moveMenuItems, (menuparam_t)B_AXIS };
const CMyLcd::SMenuDef CMyLcd::_moveCMenu PROGMEM = { _mmMoveC, _moveMenuItems, (menuparam_t)C_AXIS };
const CMyLcd::SMenuDef CMyLcd::_rotateMenu PROGMEM = { _mmRotate, _rotateMenuItems };
const CMyLcd::SMenuDef CMyLcd::_SDMenu PROGMEM	  = { _mmSD, _SDMenuItems };
const CMyLcd::SMenuDef CMyLcd::_extraMenu PROGMEM = { _mmExtra, _extraMenuItems };

////////////////////////////////////////////////////////////
