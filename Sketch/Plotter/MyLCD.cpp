#define _CRT_SECURE_NO_WARNINGS

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <StepperSystem.h>
#include <LiquidCrystal_I2C.h>

#include "MyLCD.h"
#include "PlotterControl.h"
#include "HPGLParser.h"

////////////////////////////////////////////////////////////

#define I2C_ADDR    0x27  // Define I2C Address where the PCF8574A is
#define BACKLIGHT_PIN     3
#define En_pin  2
#define Rw_pin  1
#define Rs_pin  0
#define D4_pin  4
#define D5_pin  5
#define D6_pin  6
#define D7_pin  7

////////////////////////////////////////////////////////////

LiquidCrystal_I2C	lcd(I2C_ADDR, En_pin, Rw_pin, Rs_pin, D4_pin, D5_pin, D6_pin, D7_pin);

////////////////////////////////////////////////////////////

void CMyLcd::Init()
{
	lcd.setBacklightPin(BACKLIGHT_PIN, POSITIVE);
	lcd.setBacklight(HIGH);

	lcd.backlight();			// finish with backlight on  

	lcd.begin(MYLCD_COLS, MYLCD_ROWS);			// initialize the lcd for 20 chars 4 lines, turn on backlight

	super::Init();
}

////////////////////////////////////////////////////////////

void CMyLcd::TimerInterrupt()
{
	super::TimerInterrupt();
/*
	if (READ(KILL_PIN)==0)
	{
		CStepper::GetInstance()->AbortMove();
	}

	button.Tick(READ(BTN_EN1),READ(BTN_EN2));
*/
}

////////////////////////////////////////////////////////////

void CMyLcd::Idle(unsigned int idletime)
{
	super::Idle(idletime);
}

////////////////////////////////////////////////////////////

void CMyLcd::TextModeDraw(unsigned char col, unsigned char row, const __FlashStringHelper* s)
{
	lcd.setCursor(col, row); lcd.print(s);
}
void CMyLcd::TextModeDraw(unsigned char col, unsigned char row, char* s)
{
	lcd.setCursor(col, row); lcd.print(s);
}

////////////////////////////////////////////////////////////

void CMyLcd::FirstDraw()
{
	lcd.clear();
//	lcd.setCursor(0, 0); lcd.print(F("123456789001234567890"));
	lcd.setCursor(0, 0); lcd.print(F("X:xxx.xx"));
	lcd.setCursor(0, 1); lcd.print(F("Y:xxx.xx"));
	lcd.setCursor(0, 2); lcd.print(F("Pen:x"));
	//	lcd.setCursor(0, 2); lcd.print(F("Herbert Aitenbichler"));
	//	lcd.setCursor(0, 3); lcd.print(F("Herbert Aitenbichler"));
}

////////////////////////////////////////////////////////////

unsigned long CMyLcd::Splash()
{
	lcd.clear();
	lcd.setCursor(6, 0); //Start at character 4 on line 0
	lcd.print(F("Hello"));

	lcd.setCursor(0, 2);
	lcd.print(F("Herbert Aitenbichler"));
	lcd.setCursor(0, 3);
	lcd.print(F(__DATE__","__TIME__));

	delay(100);

	return 5000l;
}

////////////////////////////////////////////////////////////

void CMyLcd::TextModeClear()
{
	lcd.clear();
}

////////////////////////////////////////////////////////////

void CMyLcd::Draw(EDrawType /* draw */)
{
	DrawPos(2, 0, CStepper::GetInstance()->GetCurrentPosition(X_AXIS));
	DrawPos(2, 1, CStepper::GetInstance()->GetCurrentPosition(Y_AXIS));
	DrawES(17, 0, CStepper::GetInstance()->IsReference(CStepper::GetInstance()->ToReferenceId(X_AXIS, true)));
	DrawES(18, 0, CStepper::GetInstance()->IsReference(CStepper::GetInstance()->ToReferenceId(Y_AXIS, true)));
	DrawES(19, 0, CStepper::GetInstance()->IsReference(CStepper::GetInstance()->ToReferenceId(Z_AXIS, true)));
	DrawPen(4, 2);
}

////////////////////////////////////////////////////////////

void CMyLcd::DrawPos(unsigned char col, unsigned char row, unsigned long pos)
{
	lcd.setCursor(col, row);

	char tmp[16];

	// draw in mm

	unsigned long hpglunits = RoundMulDivI32(pos, CHPGLParser::_state.HPDiv, CHPGLParser::_state.HPMul);

	unsigned short mm = (unsigned short) (hpglunits / 40);
	if (mm >= 1000)
		mm = 999;
	unsigned short mm100 = (hpglunits % 40) * 5 / 2;
	_itoa(mm, tmp, 10);
	int len = strlen(tmp);
	tmp[len++] = '.';
	if (mm100 < 10)
		tmp[len++] = '0';
	_itoa(mm100, tmp + len, 10);

	if (mm < 10)
		lcd.print(F("  "));
	else if (mm < 100)
		lcd.print(F(" "));
	lcd.print(tmp);
}

////////////////////////////////////////////////////////////

void CMyLcd::DrawES(unsigned char col, unsigned char row, bool es)
{
	lcd.setCursor(col, row);
	lcd.print(es ? F("1") : F("0"));
}

////////////////////////////////////////////////////////////

void CMyLcd::DrawPen(unsigned char col, unsigned char row)
{
	lcd.setCursor(col, row);
	lcd.print(Plotter.GetPen());
	lcd.print(Plotter.IsPenDown() ? F(" down") : F(" up  ") );
}
