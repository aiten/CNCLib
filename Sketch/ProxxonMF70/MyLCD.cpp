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
	{ CMyLcd::MyDrawLoopSplash, CMyLcd::MyButtonPressNOP },
	{ CMyLcd::MyDrawLoopDebug, CMyLcd::MyButtonPressNOP },
	{ CMyLcd::MyDrawLoopPosAbs, CMyLcd::MyButtonPressNOP },
	{ CMyLcd::MyDrawLoopPreset, CMyLcd::MyButtonPressPresetPage },
	{ CMyLcd::MyDrawLoopStartSD, CMyLcd::MyButtonPressStartSDPage },
	{ CMyLcd::MyDrawLoopPause, CMyLcd::MyButtonPressPause },
	{ CMyLcd::MyDrawLoopError, CMyLcd::MyButtonPressNOP },
	{ CMyLcd::MyDrawLoopMenu, CMyLcd::MyButtonPressMenuPage }
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

	_currentpage = AbsPage;
	SetRotaryFocusMainPage();
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

EnumAsByte(CMyLcd::EPage) CMyLcd::GetPage()
{
	if (_rotaryFocus == MainPage)
	{
		EnumAsByte(EPage) page = (EnumAsByte(EPage))(_button.GetPageIdx(PageCount));

		if (page != _currentpage)
		{
			_currentpage = page;
			_currentMenu = 0;
			_currentMenuOffset = 0;
		}
	}

	return _currentpage;
}

////////////////////////////////////////////////////////////

void CMyLcd::SetRotaryFocusMainPage()
{
	_button.SetPageIdx(_currentpage); _button.SetMinMax(0, PageCount - 1, true);
	_rotaryFocus = MainPage;
}

////////////////////////////////////////////////////////////

void CMyLcd::TimerInterrupt()
{
	super::TimerInterrupt();

	if (READ(KILL_PIN) == LCD_KILL_PIN_ON)
	{
		Control.Kill();
	}

	_button.Tick(READ(ROTARY_EN1), READ(ROTARY_EN2));
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

void CMyLcd::ButtonPress()
{
	ButtonFunction fnc = (ButtonFunction)pgm_read_ptr(&_pagedef[GetPage()].buttonpress);

	if (fnc)
	{
		fnc(this);
		DrawLoop();
	}
}

////////////////////////////////////////////////////////////

unsigned long CMyLcd::Splash()
{
	DrawLoop(MyDrawLoopSplash);
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

void CMyLcd::ButtonPressPresetPage()
{
	// set current position to AbsPosPreset

	mm1000_t current[NUM_AXIS];
	CMotionControl::GetPositions(current);

	for (register unsigned char i = 0; i < NUM_AXIS; i++)
	{
		CGCodeParser::SetG54PosPreset(i, current[i]);
	}
	CGCodeParser::SetZeroPresetIdx(1);

	Beep();
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
	char tmp[16]; strcpy_P(tmp, (PGM_P)F("auto0.g"));;  //avoid string in memory
	CGCode3DParser::GetExecutingFile() = SD.open(tmp);
	if (CGCode3DParser::GetExecutingFile() && CGCode3DParser::GetExecutingFile().seek(0))
		Control.StartPrintFromSD();

	Beep();
}

////////////////////////////////////////////////////////////

bool CMyLcd::DrawLoopStartSD(bool setup)
{
	if (setup)	return DrawLoopSetupDefault();
	DrawLoopDefaultHead();

	u8g.drawStr(ToCol(3), ToRow(2), F("Press to start"));
	u8g.drawStr(ToCol(3), ToRow(3), F("auto0.g from SD"));

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

unsigned char CMyLcd::GetMenuCount()
{
	return 21;		// see const __FlashStringHelper* menuName[] PROGMEM in CMyLcd::DrawLoopMenu
}

////////////////////////////////////////////////////////////

unsigned char CMyLcd::GetMenuIdx()
{
	if (_rotaryFocus == MenuPage)
	{
		unsigned char menu = _button.GetPageIdx(GetMenuCount());
		if (menu != _currentMenu)
		{
			_currentMenu = menu;
		}
	}

	return _currentMenu;
}

////////////////////////////////////////////////////////////

void CMyLcd::SetRotaryFocusMenuPage()
{
	_button.SetPageIdx(_currentMenu); _button.SetMinMax(0, GetMenuCount()-1, false); 
	_rotaryFocus = MenuPage;
}

////////////////////////////////////////////////////////////

void CMyLcd::ButtonPressMenuPage()
{
	switch (_rotaryFocus)
	{
		case MainPage:	SetRotaryFocusMenuPage(); Beep();  break;
		case MenuPage:	
		{
			unsigned char x = GetMenuIdx();
			if (x==GetMenuCount()-1)		// last is always back
			{
				SetRotaryFocusMainPage(); 
				Beep();  
			}
			else
			{
				Beep();  
				Beep();  
			}
			break;
		}
	}
}

////////////////////////////////////////////////////////////

bool CMyLcd::DrawLoopMenu(bool setup)
{
	const __FlashStringHelper* menuName[] PROGMEM = 
	{ 
		F("Move X"), 
		F("Move Y"), 
		F("Move Z"), 
		F("Probe Z"), 
		F("M5"), 
		F("M6"),
		F("M7"),
		F("M8"),
		F("M9"),
		F("M10"),
		F("M11"),
		F("M12"),
		F("M13"),
		F("M14"),
		F("M15"),
		F("M16"),
		F("M17"),
		F("M18"),
		F("M19"),
		F("M20"),
		F("back")		// see GetMenuCount()
	};

	if (setup)	return DrawLoopSetupDefault();

	u8g.drawStr(ToCol(0), ToRow(0) + HeadLineOffset, F("Menu"));

	unsigned char x = 255;
	const unsigned char printFirstLine = 1;
	const unsigned char printLastLine  = (TotalRows - 1);
	const unsigned char menuEntries = GetMenuCount();

	if (_rotaryFocus == MenuPage)
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

		if (x == menuEntries - 1)
		{
			_currentMenuOffset += x + printFirstLine - _currentMenuOffset - printLastLine;	// last menuitem selected => move to last line
		}
		else if (((x + 1) + printFirstLine - _currentMenuOffset) > printLastLine)
		{
			_currentMenuOffset += (x + 1) + printFirstLine - _currentMenuOffset - printLastLine;
		}
	}
	unsigned char i;

	for (i = 0; i < menuEntries; i++)
	{
		unsigned char printtorow = i+printFirstLine-_currentMenuOffset;	// may overrun => no need to check for minus
		if (printtorow >= printFirstLine && printtorow <= printLastLine)
		{
			u8g.setPrintPos(ToCol(0), ToRow(printtorow) + PosLineOffset);
			if (i == x && _rotaryFocus == MenuPage)
				u8g.print(F(">"));
			else
				u8g.print(F(" "));

			u8g.print(menuName[i]);
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

void CMyLcd::DrawLoop()
{
	if (_curretDraw && _curretDraw(this, true))
	{
		u8g.firstPage();
		do
		{
			if (_curretDraw && !_curretDraw(this, false))
				break;
		} while (u8g.nextPage());
	}
}

////////////////////////////////////////////////////////////

void CMyLcd::FirstDraw()
{
	DrawLoop(MyDrawLoopDebug);
}

////////////////////////////////////////////////////////////


void CMyLcd::Draw(EDrawType /* draw */)
{
	DrawFunction fnc = (DrawFunction)pgm_read_ptr(&_pagedef[GetPage()].draw);
	DrawLoop(fnc);
}

////////////////////////////////////////////////////////////
