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

#define _CRT_SECURE_NO_WARNINGS

////////////////////////////////////////////////////////////

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <CNCLib.h>
#include <CNCLibEx.h>

#include "U8GLCD.h"
#include <Beep.h>

#include "RotaryButton.h"
#include "GCodeParser.h"
#include "GCode3DParser.h"
#include <U8glib.h>

////////////////////////////////////////////////////////////
//
// used full graphic controller for Ramps 1.4
//
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

void CU8GLcd::Init()
{
	CBeep<(CAT(BOARDNAME, _LCD_BEEPER))>::Init();

	super::Init();

	CHAL::pinMode(ROTARY_ENC, INPUT_PULLUP);
	CHAL::pinMode(ROTARY_EN1, INPUT_PULLUP);
	CHAL::pinMode(ROTARY_EN2, INPUT_PULLUP);

	CHAL::pinMode(CAT(BOARDNAME,_LCD_KILL_PIN), INPUT_PULLUP);

	_button.Tick(HALFastdigitalRead(ROTARY_EN1), HALFastdigitalRead(ROTARY_EN2));

	GetMenu().SetMainMenu();
	SetDefaultPage();
}

////////////////////////////////////////////////////////////

void CU8GLcd::SetDefaultPage()
{
	_currentpage = AbsPage;
	SetRotaryFocusMainPage();
}

////////////////////////////////////////////////////////////

void CU8GLcd::SetMenuPage()
{
	_currentpage = MenuPage;
	SetRotaryFocusMenuPage();
}

////////////////////////////////////////////////////////////

void CU8GLcd::Beep(const SPlayTone* play)
{
	CBeep<CAT(BOARDNAME, _LCD_BEEPER)>::Play(play);
        // CBeep<CAT(BOARDNAME, _LCD_BEEPER)>::Beep(ToneA4,16);
}

////////////////////////////////////////////////////////////
/*
#ifdef _MSC_VER
CU8GLcd::EPage CU8GLcd::GetPage()
#else
EnumAsByte(CU8GLcd::EPage) CU8GLcd::GetPage()
#endif
{
	if (_rotaryFocus == RotaryMainPage)
	{
		EnumAsByte(EPage) page = (EnumAsByte(EPage))(_button.GetPageIdx(PageCount));

		if (page != _currentpage)
		{
			_currentpage = page;
			_menu.SetMainMenu();
		}
	}

	return _currentpage;
}
*/
////////////////////////////////////////////////////////////

void CU8GLcd::SetRotaryFocusMainPage()
{
	_button.SetPageIdx((rotarypos_t) _currentpage); _button.SetMinMax(0, PageCount - 1, true);
	_rotaryFocus = RotaryMainPage;
}

////////////////////////////////////////////////////////////

void CU8GLcd::TimerInterrupt()
{
	super::TimerInterrupt();

	switch (_button.Tick(HALFastdigitalRead(ROTARY_EN1), HALFastdigitalRead(ROTARY_EN2)))
	{
		case CRotaryButton<rotarypos_t, ROTARY_ACCURACY>::Overrun:
			break;
		case CRotaryButton<rotarypos_t, ROTARY_ACCURACY>::Underflow:
			break;
	}
	if (_expectButtonOff)
	{
		if (HALFastdigitalRead(ROTARY_ENC) != ROTARY_ENC_ON)
			_expectButtonOff = false;
	}
}

////////////////////////////////////////////////////////////

void CU8GLcd::Poll()
{
	super::Poll();

	if (!_expectButtonOff && HALFastdigitalRead(ROTARY_ENC) == ROTARY_ENC_ON)
	{
		_expectButtonOff = true;
		ButtonPress();
	}
}

////////////////////////////////////////////////////////////

void CU8GLcd::Command(char* buffer)
{
	super::Command(buffer);

	if (*buffer)
	{
		for (unsigned char commandlenght = 0; *buffer && commandlenght < MAXCHARPERLINE; commandlenght++)
		{
			QueueCommandHistory(*(buffer++));
		}
		QueueCommandHistory(0);
	}
}

////////////////////////////////////////////////////////////

void CU8GLcd::QueueCommandHistory(char ch)
{
	if (_commandHis.IsFull())
	{
		// dequeue last command
		do
		{
			_commandHis.Dequeue();
		} 
		while (!_commandHis.IsEmpty() && _commandHis.Head() != 0);
	}
	_commandHis.Enqueue(ch);

}


////////////////////////////////////////////////////////////

#if defined(__AVR_ARCH__)

CU8GLcd::ButtonFunction CMyLcd::GetButtonPress_P(const void* adr)
{
	struct ButtonFunctionWrapper
	{
		ButtonFunction fnc;
	}x;

	memcpy_P(&x, adr, sizeof(ButtonFunctionWrapper));

	return x.fnc;
}

CU8GLcd::DrawFunction CMyLcd::GetDrawFunction_P(const void* adr)
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


void CU8GLcd::ButtonPress()
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

unsigned long CU8GLcd::Splash()
{
	DrawLoop(&CU8GLcd::DrawLoopSplash);
	OKBeep();
	return 3000;
}

////////////////////////////////////////////////////////////

bool CU8GLcd::DrawLoopSetupDefault()
{
	GetU8G().setFont(DEFAULTFONT);
	return true;
}

////////////////////////////////////////////////////////////

void CU8GLcd::DrawLoopDefaultHead()
{
#ifdef USE_RAMPS14
#if defined(__SAM3X8E__)
	GetU8G().drawStr(ToCol(0), ToRow(0), F("Proxxon MF70 Ramps14S"));
#else
	GetU8G().drawStr(ToCol(0), ToRow(0), F("Proxxon MF70 Ramps14M"));
#endif
#else
#if defined(__SAM3X8E__)
	GetU8G().drawStr(ToCol(0), ToRow(0), F("Proxxon MF70 RampsFDS"));
#else
	GetU8G().drawStr(ToCol(0), ToRow(0), F("Proxxon MF70 RampsFDM"));
#endif
#endif
}

////////////////////////////////////////////////////////////

bool CU8GLcd::DrawLoopSplash(bool setup)
{
	if (setup)	return DrawLoopSetupDefault();
	DrawLoopDefaultHead();

	GetU8G().drawStr(ToCol(TotalCols / 2 - 1), ToRow(2), F("by"));
	GetU8G().drawStr(ToCol(3), ToRow(3), F("H. Aitenbichler"));
	GetU8G().drawStr(ToCol(5), ToRow(5), F(__DATE__));

	return true;
}

////////////////////////////////////////////////////////////

bool CU8GLcd::DrawLoopDebug(bool setup)
{
	if (setup)	return DrawLoopSetupDefault();

	GetU8G().drawStr(ToCol(0), ToRow(0) + HeadLineOffset, F("Debug"));

	char tmp[16];

	for (unsigned char i = 0; i < LCD_NUMAXIS; i++)
	{
		GetU8G().setPrintPos(ToCol(0), ToRow(i + 1) + PosLineOffset);

		udist_t pos = CStepper::GetInstance()->GetCurrentPosition(i);

		GetU8G().print(CSDist::ToString(pos, tmp, 6));
		GetU8G().print(F(" "));
		GetU8G().print(CMm1000::ToString(CMotionControlBase::GetInstance()->ToMm1000(i, pos), tmp, 6, 2));
		GetU8G().print(F(" "));

		GetU8G().print(CStepper::GetInstance()->IsReference(CStepper::GetInstance()->ToReferenceId(i, true)) ? '1' : '0');
		GetU8G().print(CStepper::GetInstance()->IsReference(CStepper::GetInstance()->ToReferenceId(i, false)) ? '1' : '0');

		GetU8G().print((CStepper::GetInstance()->GetLastDirection()&(1 << i)) ? '+' : '-');
	}

	GetU8G().setPrintPos(ToCol(20), ToRow(0 + 1) + PosLineOffset);
	GetU8G().print(CControl::GetInstance()->IOControl(CControl::Probe) ? '1' : '0');

	GetU8G().setPrintPos(ToCol(19), ToRow(0 + 2) + PosLineOffset);
	GetU8G().print(CSDist::ToString(CStepper::GetInstance()->QueuedMovements(), tmp, 2));

	return true;
}

////////////////////////////////////////////////////////////

bool CU8GLcd::DrawLoopPosAbs(bool setup)
{
	if (setup)	return DrawLoopSetupDefault();

	GetU8G().setPrintPos(ToCol(0), ToRow(0) + HeadLineOffset); GetU8G().print(F("Absolut  Current"));
	char tmp[16];

	for (unsigned char i = 0; i < LCD_NUMAXIS; i++)
	{
//		udist_t cur = CStepper::GetInstance()->GetCurrentPosition(i);
		mm1000_t psall = CGCodeParser::GetAllPreset(i);

		GetU8G().setPrintPos(ToCol(0), ToRow(i + 1) + PosLineOffset);
		tmp[0] = 0; GetU8G().print(AddAxisName(tmp,i));

		GetU8G().print(CMm1000::ToString(CMotionControlBase::GetInstance()->GetPosition(i), tmp, 7, 2));
		GetU8G().print(F(" "));
		GetU8G().print(CMm1000::ToString(CMotionControlBase::GetInstance()->GetPosition(i) - psall, tmp, 7, 2));
/*
		GetU8G().print(CMm1000::ToString(CMotionControlBase::GetInstance()->ToMm1000(i, cur), tmp, 7, 2));
		GetU8G().print(F(" "));
		GetU8G().print(CMm1000::ToString(CMotionControlBase::GetInstance()->ToMm1000(i, cur) - psall, tmp, 7, 2));
*/
	}

	return true;
}
////////////////////////////////////////////////////////////

void CU8GLcd::DrawLoop()
{
	if (_curretDraw && (this->*_curretDraw)(true))
	{
		GetU8G().firstPage();
		do
		{
			if (_curretDraw && !(this->*_curretDraw)(false))
				break;
		} while (GetU8G().nextPage());
	}
}

////////////////////////////////////////////////////////////

void CU8GLcd::FirstDraw()
{
	DrawLoop(&CU8GLcd::DrawLoopDebug);
}

////////////////////////////////////////////////////////////


unsigned long CU8GLcd::Draw(EDrawType /* draw */)
{
#if defined(__AVR_ARCH__)
	DrawFunction fnc = GetDrawFunction_P(&_pagedef[GetPage()].draw);
#else
	DrawFunction fnc = _pagedef[GetPage()].draw;
#endif

	DrawLoop(fnc);

	return 333;
}

////////////////////////////////////////////////////////////

void CU8GLcd::ButtonPressShowMenu()
{
	SetMenuPage();
}

////////////////////////////////////////////////////////////

bool CU8GLcd::DrawLoopPreset(bool setup)
{
	if (setup)	return DrawLoopSetupDefault();

	mm1000_t ps;

	const __FlashStringHelper* zeroShiftName[] PROGMEM = { F("G53"), F("G54"), F("G55"), F("G56"), F("G57"), F("G58"), F("G59") };

	GetU8G().setPrintPos(ToCol(0), ToRow(0) + HeadLineOffset);  GetU8G().print(F("Preset: ")); GetU8G().print(zeroShiftName[CGCodeParser::GetZeroPresetIdx()]); GetU8G().print(F(" G92 Height"));

	char tmp[16];

	for (unsigned char i = 0; i < LCD_NUMAXIS; i++)
	{
		GetU8G().setPrintPos(ToCol(0), ToRow(i + 1) + PosLineOffset);
		tmp[0] = 0; GetU8G().print(AddAxisName(tmp,i));
		ps = CGCodeParser::GetG54PosPreset(i);
		GetU8G().print(CMm1000::ToString(ps, tmp, 7, 2));

		ps = CGCodeParser::GetG92PosPreset(i);
		GetU8G().print(CMm1000::ToString(ps, tmp, 7, 2));

		ps = CGCodeParser::GetToolHeightPosPreset(i);
		GetU8G().print(CMm1000::ToString(ps, tmp, 6, 2));
	}
	return true;
}

////////////////////////////////////////////////////////////

void CU8GLcd::ButtonPressStartSDPage()
{
	PostCommand(F("m21"));									// Init SD

	if (PostCommand(F("m23 proxxon.nc")))
	{
		PostCommand(F("m24"));
	}

	OKBeep();
}

////////////////////////////////////////////////////////////

bool CU8GLcd::DrawLoopStartSD(bool setup)
{
	if (setup)	return DrawLoopSetupDefault();
	DrawLoopDefaultHead();

	char tmp[16];

	if (CGCode3DParser::GetExecutingFile())
  	    GetU8G().drawStr(ToCol(3), ToRow(2), F("Press to start"));
	GetU8G().setPrintPos(ToCol(0), ToRow(3) + PosLineOffset); GetU8G().print(F("File: ")); GetU8G().print(CGCode3DParser::GetExecutingFileName());
	GetU8G().setPrintPos(ToCol(0), ToRow(4) + PosLineOffset); GetU8G().print(F("At:   ")); GetU8G().print(CSDist::ToString(CGCode3DParser::GetExecutingFilePosition(), tmp, 8));
	GetU8G().setPrintPos(ToCol(0), ToRow(5) + PosLineOffset); GetU8G().print(F("Line: ")); GetU8G().print(CSDist::ToString(CGCode3DParser::GetExecutingFileLine(), tmp, 8));

	return true;
}

////////////////////////////////////////////////////////////

void CU8GLcd::ButtonPressPause()
{
	if (CControl::GetInstance()->IsPause())
		CControl::GetInstance()->Continue();
	else
		CControl::GetInstance()->Pause();

	OKBeep();
}

////////////////////////////////////////////////////////////

bool CU8GLcd::DrawLoopPause(bool setup)
{
	if (setup)	return DrawLoopSetupDefault();
	DrawLoopDefaultHead();

	if (CControl::GetInstance()->IsPause())
		GetU8G().drawStr(ToCol(2), ToRow(2), F("Press to continue"));
	else
		GetU8G().drawStr(ToCol(3), ToRow(2), F("Press to pause"));

	return true;
}

////////////////////////////////////////////////////////////

bool CU8GLcd::DrawLoopError(bool setup)
{
	if (setup)	return DrawLoopSetupDefault();
	DrawLoopDefaultHead();

	unsigned char errors = 0;

	if (CStepper::GetInstance()->GetError())
		GetU8G().drawStr(ToCol(0), ToRow(++errors + 1), CStepper::GetInstance()->GetError());
	if (CControl::GetInstance()->IsKilled())
		GetU8G().drawStr(ToCol(0), ToRow(++errors + 1), F("emergency stop"));

	if (errors == 0)
		GetU8G().drawStr(ToCol(0), ToRow(2), F("no errors"));

	return true;
}

////////////////////////////////////////////////////////////

bool CU8GLcd::DrawLoopCommandHis(bool setup)
{
	if (setup)	return DrawLoopSetupDefault();
	DrawLoopDefaultHead();

	char tmp[MAXCHARPERLINE+1];
	unsigned char commandpos = _commandHis.T2HInit();	// idx of \0 of last command

	for (unsigned char i = 0; i < LCD_NUMAXIS; i++)
	{
		GetU8G().setPrintPos(ToCol(0), ToRow(LCD_NUMAXIS - i) + PosLineOffset);

		unsigned char idx = MAXCHARPERLINE;
		tmp[idx] = 0;

		if (_commandHis.T2HTest(commandpos))
		{
			for (commandpos = _commandHis.T2HInc(commandpos); _commandHis.T2HTest(commandpos) && _commandHis.Buffer[commandpos] != 0;commandpos = _commandHis.T2HInc(commandpos))
			{
				tmp[--idx] = _commandHis.Buffer[commandpos];
			}
			GetU8G().print(&tmp[idx]);
		}
	}

	return true;
}

////////////////////////////////////////////////////////////

unsigned char CU8GLcd::GetMenuIdx()
{
	if (_rotaryFocus == RotaryMenuPage)
	{
		unsigned char menu = _button.GetPageIdx(GetMenu().GetMenuDef()->GetItemCount());
		if (menu != GetMenu().GetPosition())
		{
			GetMenu().SetPosition(menu);
		}
	}

	return GetMenu().GetPosition();
}

////////////////////////////////////////////////////////////

void CU8GLcd::SetRotaryFocusMenuPage()
{
	_button.SetPageIdx(GetMenu().GetPosition()); _button.SetMinMax(0, GetMenu().GetMenuDef()->GetItemCount() - 1, false);
	_rotaryFocus = RotaryMenuPage;
}

////////////////////////////////////////////////////////////

void CU8GLcd::ButtonPressMenuPage()
{
	switch (_rotaryFocus)
	{
		case RotaryMainPage:	SetRotaryFocusMenuPage(); OKBeep();  break;
		case RotaryMenuPage:
		{
			if (!GetMenu().Select())
			{
				ErrorBeep();
			}

			break;
		}
	}
}

////////////////////////////////////////////////////////////

bool CU8GLcd::DrawLoopMenu(bool setup)
{
	if (setup)	return DrawLoopSetupDefault();

	GetU8G().setPrintPos(ToCol(0), ToRow(0) + HeadLineOffset);
	GetU8G().print(F("Menu: "));
	GetU8G().print(GetMenu().GetMenuDef()->GetText());

	unsigned char x = 255;
	const unsigned char printFirstLine = 1;
	const unsigned char printLastLine = (TotalRows - 1);
	const unsigned char menuEntries = GetMenu().GetMenuDef()->GetItemCount();

	if (_rotaryFocus == RotaryMenuPage)
	{
		x = GetMenuIdx();													// get and set menupositions
		GetMenu().AdjustOffset(printFirstLine, printLastLine);
	}

	unsigned char i;

	for (i = 0; i < menuEntries; i++)
	{
		unsigned char printtorow = GetMenu().ToPrintLine(printFirstLine, printLastLine, i);
		if (printtorow != 255)
		{
			GetU8G().setPrintPos(ToCol(0), ToRow(printtorow) + PosLineOffset);
			if (i == x && _rotaryFocus == RotaryMenuPage)
				GetU8G().print(F(">"));
			else
				GetU8G().print(F(" "));


			GetU8G().print(GetMenu().GetMenuDef()->GetItems()[i].GetText());
		}
	}

	return true;
}

////////////////////////////////////////////////////////////
