#define _CRT_SECURE_NO_WARNINGS

////////////////////////////////////////////////////////////

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <StepperSystem.h>
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

U8GLIB_ST7920_128X64_1X u8g(ST7920_CLK_PIN, ST7920_DAT_PIN, ST7920_CS_PIN);	// SPI Com: SCK = en = 18, MOSI = rw = 16, CS = di = 17

////////////////////////////////////////////////////////////

#if defined(__SAM3X8E__)

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

PROGMEM CMyLcd::SPageDef CMyLcd::_pagedef[] =
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
	pinMode(LCD_BEEPER, OUTPUT);
	digitalWrite(LCD_BEEPER, LOW);

	super::Init();

	pinMode(ROTARY_ENC, INPUT_PULLUP);
	pinMode(ROTARY_EN1, INPUT_PULLUP);
	pinMode(ROTARY_EN2, INPUT_PULLUP);

	pinMode(LCD_KILL_PIN, INPUT_PULLUP);

	_button.Tick(READ(ROTARY_EN1), READ(ROTARY_EN2));

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
		digitalWrite(LCD_BEEPER, HIGH);
		delay(3);
		digitalWrite(LCD_BEEPER, LOW);
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

	if (READ(KILL_PIN) == LCD_KILL_PIN_ON)
	{
		Control.Kill();
	}

	switch (_button.Tick(READ(ROTARY_EN1), READ(ROTARY_EN2)))
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
		if (READ(ROTARY_ENC) != ROTARY_ENC_ON)
			_expectButtonOff = false;
	}
	else if (READ(ROTARY_ENC) == ROTARY_ENC_ON)
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
#if defined(__SAM3X8E__)
	u8g.drawStr(ToCol(1), ToRow(0), F("Proxxon MF 70 - due"));
#else
	u8g.drawStr(ToCol(0), ToRow(0), F("Proxxon MF 70 - mega"));
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

	for (unsigned char i = 0; i < NUM_AXIS; i++)
	{
		u8g.setPrintPos(ToCol(0), ToRow(i + 1) + PosLineOffset);

		udist_t pos = CStepper::GetInstance()->GetCurrentPosition(i);

		u8g.print(CSDist::ToString(pos, tmp, 6));
		u8g.print(F(" "));
		u8g.print(CMm1000::ToString(CMotionControl::ToMm1000(i, pos), tmp, 6, 2));
		u8g.print(F(" "));

		u8g.print(CStepper::GetInstance()->IsReference(CStepper::GetInstance()->ToReferenceId(i, true)) ? '1' : '0');
		u8g.print(CStepper::GetInstance()->IsReference(CStepper::GetInstance()->ToReferenceId(i, false)) ? '1' : '0');

		u8g.print((CStepper::GetInstance()->GetLastDirection()&(1 << i)) ? '+' : '-');
	}

	CProbeControl prob;
	u8g.setPrintPos(ToCol(20), ToRow(0 + 1) + PosLineOffset);
	u8g.print(prob.IsOn() ? '1' : '0');

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

	const __FlashStringHelper* axisName1[] PROGMEM = { F("X"), F("Y"), F("Z"), F("A"), F("B"), F("C") };

	for (unsigned char i = 0; i < NUM_AXIS; i++)
	{
		udist_t cur = CStepper::GetInstance()->GetCurrentPosition(i);
		mm1000_t psall = CGCodeParser::GetAllPreset(i);

		u8g.setPrintPos(ToCol(0), ToRow(i + 1) + PosLineOffset);
		u8g.print(axisName1[i]);
		u8g.print(CMm1000::ToString(CMotionControl::ToMm1000(i, cur), tmp, 7, 2));
		u8g.print(F(" "));
		u8g.print(CMm1000::ToString(CMotionControl::ToMm1000(i, cur) - psall, tmp, 7, 2));
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
	return Control.PostCommand(cmd);
}

////////////////////////////////////////////////////////////

bool CMyLcd::SendCommand(char* cmd)
{
	return Control.PostCommand(cmd);
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

	const __FlashStringHelper* axisName1[] PROGMEM = { F("X"), F("Y"), F("Z"), F("A"), F("B"), F("C") };
	char tmp[16];

	for (unsigned char i = 0; i < NUM_AXIS; i++)
	{
		u8g.setPrintPos(ToCol(0), ToRow(i + 1) + PosLineOffset);
		u8g.print(axisName1[i]);
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

const __FlashStringHelper* CMyLcd::GetMenuText(unsigned char idx)
{
	return (const __FlashStringHelper*)pgm_read_ptr(&_currentMenu[idx].text);
}

////////////////////////////////////////////////////////////

CMyLcd::MenuButtonFunction CMyLcd::GetMenuFnc(unsigned char idx)
{
#if defined(__AVR_ARCH__)
	return GetMenuButtonPress_P(&_currentMenu[idx].buttonpress);
#else
	return _currentMenu[idx].buttonpress;
#endif
}

////////////////////////////////////////////////////////////

unsigned char CMyLcd::GetMenuCount()
{
	for (unsigned char x = 0;; x++)
	{
		if (GetMenuText(x) == NULL) return x;
	}
}

////////////////////////////////////////////////////////////

unsigned char CMyLcd::GetMenuIdx()
{
	if (_rotaryFocus == RotaryMenuPage)
	{
		unsigned char menu = _button.GetPageIdx(GetMenuCount());
		if (menu != _currentMenuIdx)
		{
			_currentMenuIdx = menu;
		}
	}

	return _currentMenuIdx;
}

////////////////////////////////////////////////////////////

unsigned char CMyLcd::FindMenuIdx(CMyLcd::MenuButtonFunction f, unsigned short param, unsigned char valueIffail)
{
	for (unsigned char x = 0;GetMenuText(x) != NULL; x++)
	{
		if (GetMenuFnc(x) == f && GetMenuParam(x) == param)
			return x;
	}

	return valueIffail;
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

void CMyLcd::ButtonPressMenuEnd(unsigned short)
{
	SetMainMenu();
	SetDefaultPage();
	Beep();
}

////////////////////////////////////////////////////////////

void CMyLcd::ButtonPressMenuProbe(unsigned short)
{
	if (SendCommand(F("g91 g31 Z-10 F100 g90")))
	{
		SendCommand(F("g92 Z-25"));
		SetDefaultPage();
		SendCommand(F("g91 Z3 g90"));
	}
}

////////////////////////////////////////////////////////////

void CMyLcd::ButtonPressMenuSetSD(unsigned short)
{
	SetMenu(_SDMenu, F("SD Card"));
	SetRotaryFocusMenuPage();
	DrawLoop();
	Beep();
}

////////////////////////////////////////////////////////////

void CMyLcd::ButtonPressMenuSetMove(unsigned short axis)
{
	const __FlashStringHelper* axisName[] PROGMEM = { F("Move X"), F("Move Y"), F("Move Z"), F("Move A"), F("Move B"), F("Move C") };

	_currentMenuAxis = (axis_t)axis;
	SetMenu(_axisMenuMove, axisName[axis]);
	SetRotaryFocusMenuPage();
	DrawLoop();
	Beep();
}

////////////////////////////////////////////////////////////

void CMyLcd::ButtonPressMenuMove(unsigned short dist)
{
	if (dist == MoveHome) { ButtonPressMenuHome(_currentMenuAxis); return; }

	char tmp[24];

	strcpy_P(tmp, PSTR("g91 g0 "));
	AddAxisName(tmp, _currentMenuAxis);

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

void CMyLcd::ButtonPressMenuHome(unsigned short param)
{
	char tmp[16];
	axis_t axis = (axis_t) param;

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

void CMyLcd::ButtonPressMenuMoveG92(unsigned short)
{
	char tmp[24];

	strcpy_P(tmp, PSTR("g92 "));
	AddAxisName(tmp, _currentMenuAxis);
	strcat_P(tmp, PSTR("0"));

	SendCommand(tmp);
	Beep();
}

////////////////////////////////////////////////////////////

void CMyLcd::ButtonPressMenuMoveBack(unsigned short)
{
	SetMainMenu();
	_currentMenuIdx = FindMenuIdx(&CMyLcd::ButtonPressMenuSetMove,_currentMenuAxis,0);
	SetRotaryFocusMenuPage();
	Beep();
}

////////////////////////////////////////////////////////////

void CMyLcd::ButtonPressMenuSDBack(unsigned short)
{
	SetMainMenu();
	_currentMenuIdx = FindMenuIdx(&CMyLcd::ButtonPressMenuSetSD,0,0);
	SetRotaryFocusMenuPage();
	Beep();
}

////////////////////////////////////////////////////////////

void CMyLcd::ButtonPressMenuSpindle(unsigned short)
{
	if (Control.IOControl(CMyControl::Spindel)!=0)
		SendCommand(F("m5"));
	else
		SendCommand(F("m3"));
}

////////////////////////////////////////////////////////////

void CMyLcd::ButtonPressMenuCoolant(unsigned short)
{
	if (Control.IOControl(CMyControl::Coolant)!=0)
		SendCommand(F("m9"));
	else
		SendCommand(F("m7"));
}

////////////////////////////////////////////////////////////

void CMyLcd::SetRotaryFocusMenuPage()
{
	_button.SetPageIdx(_currentMenuIdx); _button.SetMinMax(0, GetMenuCount() - 1, false);
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
			unsigned char idx = GetMenuIdx();
			MenuButtonFunction fnc = GetMenuFnc(idx);
			if (fnc != NULL)
			{
				(this->*fnc)(GetMenuParam(idx));
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
	const unsigned char menuEntries = GetMenuCount();

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


			u8g.print(GetMenuText(i));
			/*
						if (printtorow==(TotalRows-2))
						{
						u8g.print(F(">"));
						u8g.print(_currentMenuOffset);
						}
						if (printtorow==printLastLine)
						{
						u8g.print(F(">"));
						u8g.print(_button.GetFullRangePos());
						u8g.print(F(">"));
						u8g.print(_button.GetPos());
						u8g.print(F(">"));
						u8g.print(_button.GetMin());
						u8g.print(F(">"));
						u8g.print(_button.GetMax());
						u8g.print(F(">"));
						u8g.print(x);
						}
						*/
		}
	}

	return true;
}

////////////////////////////////////////////////////////////

#define MenuText(a,b)  static const char a[] PROGMEM = b;

MenuText(_mHomeZ, "Home Z");
MenuText(_mProbeZ, "Probe Z");
MenuText(_m4, "Axis");
MenuText(_mMoveX, "Move X");
MenuText(_mMoveY, "Move Y");
MenuText(_mMoveZ, "Move Z");
MenuText(_mG92Clear, "G92 Clear");
MenuText(_mSpindle, "Spindle On/Off");
MenuText(_mCoolant, "Coolant On/Off");
MenuText(_mSD, "SD");
MenuText(_mBack, "Back");
MenuText(_mEnd, "End");

const CMyLcd::SMenuDef CMyLcd::_mainMenu[] PROGMEM =
{
	{ _mMoveX, &CMyLcd::ButtonPressMenuSetMove, X_AXIS },
	{ _mMoveY, &CMyLcd::ButtonPressMenuSetMove, Y_AXIS },
	{ _mMoveZ, &CMyLcd::ButtonPressMenuSetMove, Z_AXIS },
	{ _mHomeZ, &CMyLcd::ButtonPressMenuHome, Z_AXIS },
	{ _mProbeZ, &CMyLcd::ButtonPressMenuProbe, Z_AXIS },
	{ _mG92Clear, &CMyLcd::ButtonPressMenuG92Clear },
	{ _mSpindle, &CMyLcd::ButtonPressMenuSpindle },
	{ _mCoolant, &CMyLcd::ButtonPressMenuCoolant },
	{ _mSD, &CMyLcd::ButtonPressMenuSetSD },
	{ _mEnd, &CMyLcd::ButtonPressMenuEnd },
	{ NULL, 0 }
};

MenuText(_mP10, "+10");
MenuText(_mP1, "+1");
MenuText(_mP01, "+0.1");
MenuText(_mP001, "+0.01");
MenuText(_mM01, "-0.1");
MenuText(_mM001, "-0.01");
MenuText(_mM1, "-1");
MenuText(_mM10, "-10");
MenuText(_mHome, "Home");
MenuText(_mProbe, "Probe");
MenuText(_mG92, "G92");

const CMyLcd::SMenuDef CMyLcd::_axisMenuMove[] PROGMEM =
{
	{ _mP10, &CMyLcd::ButtonPressMenuMove, MoveP10 },
	{ _mP1, &CMyLcd::ButtonPressMenuMove, MoveP1 },
	{ _mP01, &CMyLcd::ButtonPressMenuMove, MoveP01 },
	{ _mP001, &CMyLcd::ButtonPressMenuMove, MoveP001 },
	{ _mM001, &CMyLcd::ButtonPressMenuMove, MoveM001 },
	{ _mM01, &CMyLcd::ButtonPressMenuMove, MoveM01 },
	{ _mM1, &CMyLcd::ButtonPressMenuMove, MoveM1 },
	{ _mM10, &CMyLcd::ButtonPressMenuMove, MoveM10 },
	{ _mHome, &CMyLcd::ButtonPressMenuMove, MoveHome },
	{ _mG92, &CMyLcd::ButtonPressMenuMoveG92 },
	{ _mBack, &CMyLcd::ButtonPressMenuMoveBack },
	{ NULL, 0 }
};

MenuText(_mSDInit, "Init Card");

const CMyLcd::SMenuDef CMyLcd::_SDMenu[] PROGMEM =
{
	{ _mSDInit, &CMyLcd::ButtonPressMenuSDInit },
	{ _mBack, &CMyLcd::ButtonPressMenuSDBack },
	{ NULL, 0 }
};