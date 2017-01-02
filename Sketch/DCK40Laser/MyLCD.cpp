////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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

#include <Beep.h>

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

#ifdef USE_U8G2_LIB
U8G2_ST7920_128X64_1_SW_SPI u8g(U8G2_R0, CAT(BOARDNAME,_ST7920_CLK_PIN), CAT(BOARDNAME,_ST7920_DAT_PIN), CAT(BOARDNAME,_ST7920_CS_PIN));	// SPI Com: SCK = en = 18, MOSI = rw = 16, CS = di = 17
#else
U8GLIB_ST7920_128X64_1X u8g(CAT(BOARDNAME, _ST7920_CLK_PIN), CAT(BOARDNAME, _ST7920_DAT_PIN), CAT(BOARDNAME, _ST7920_CS_PIN));	// SPI Com: SCK = en = 18, MOSI = rw = 16, CS = di = 17
#endif

U8G2& CMyLcd::GetU8G() { return u8g; }

////////////////////////////////////////////////////////////

CMyLcd Lcd;

////////////////////////////////////////////////////////////

PROGMEM const CU8GLcd::SPageDef CU8GLcd::_pagedef[] =
{
	{ &CU8GLcd::DrawLoopSplash, &CU8GLcd::ButtonPressShowMenu },
	{ &CU8GLcd::DrawLoopPosAbs, &CU8GLcd::ButtonPressShowMenu },
	{ &CU8GLcd::DrawLoopPos,	&CU8GLcd::ButtonPressShowMenu },
	{ &CU8GLcd::DrawLoopRotate2D,	&CU8GLcd::ButtonPressShowMenu },
	{ &CU8GLcd::DrawLoopRotate3D,	&CU8GLcd::ButtonPressShowMenu },
	{ &CU8GLcd::DrawLoopDebug,  &CU8GLcd::ButtonPressShowMenu },
	{ &CU8GLcd::DrawLoopSpeedOverride,  &CU8GLcd::ButtonPressSpeedOverride },
	{ &CU8GLcd::DrawLoopPreset, &CU8GLcd::ButtonPressShowMenu },
	{ &CU8GLcd::DrawLoopStartSD,&CU8GLcd::ButtonPressStartSDPage },
	{ &CU8GLcd::DrawLoopError,	&CU8GLcd::ButtonPressShowMenu },
	{ &CU8GLcd::DrawLoopCommandHis, &CU8GLcd::ButtonPressShowMenu },
	{ &CU8GLcd::DrawLoopMenu,	&CU8GLcd::ButtonPressMenuPage },
	{ NULL, NULL }
};

////////////////////////////////////////////////////////////

void CMyLcd::Init()
{
	_lcd_numaxis = LCD_NUMAXIS;

	CBeep<(CAT(BOARDNAME, _LCD_BEEPER))>::Init();
	
	super::Init();

	_rotarybutton.SetPin(ROTARY_EN1,ROTARY_EN2);
	_rotarypushbutton.SetPin(ROTARY_ENC,ROTARY_ENC_ON);

	_rotarybutton.Tick();
}

////////////////////////////////////////////////////////////

void CMyLcd::Beep(const SPlayTone* play,bool fromProgMem)
{
	if (fromProgMem)
	{
		CBeep<CAT(BOARDNAME, _LCD_BEEPER)>::PlayPGM(play);
	}
	else
	{
		CBeep<CAT(BOARDNAME, _LCD_BEEPER)>::Play(play);
	}
}

////////////////////////////////////////////////////////////

bool CMyLcd::DrawLoopDefault(EnumAsByte(EDrawLoopType) type,uintptr_t data)
{
	if (type==DrawLoopHeader)
	{
#if defined(__SAM3X8E__)
		DrawString(ToCol(4), ToRow(0), F("DC-K40 Due"));
#else
		DrawString(ToCol(4), ToRow(0), F("DC-K40 Mega"));
#endif
		return true;
	}

	return super::DrawLoopDefault(type,data);
}

////////////////////////////////////////////////////////////

