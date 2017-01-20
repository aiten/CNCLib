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

#include <stdio.h>
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <CNCLib.h>
#include <GCodeParser.h>

#include "MyControl.h"
#include "MyLCD.h"

////////////////////////////////////////////////////////////

CMyControl Control;
CGCodeTools GCodeTools;
CMotionControl MotionControl;
CConfigEeprom Eprom;
HardwareSerial& StepperSerial = Serial;

////////////////////////////////////////////////////////////

#ifndef MYNUM_AXIS
#error Please define MYNUM_AXIS
#endif

////////////////////////////////////////////////////////////

const CConfigEeprom::SCNCEeprom CMyControl::_eepromFlash PROGMEM =
{
	EPROM_SIGNATURE,
	NUM_AXIS, MYNUM_AXIS, offsetof(CConfigEeprom::SCNCEeprom,axis), sizeof(CConfigEeprom::SCNCEeprom::SAxisDefinitions),
	GetInfo1a(),0,
	0,
	STEPPERDIRECTION,0,0,0,
	SPINDLE_MAXSPEED,0,
	CNC_MAXSPEED,
	CNC_ACC,
	CNC_DEC,
	STEPRATERATE_REFMOVE,
	MOVEAWAYFROMREF_MM1000,
	X_STEPSPERMM/1000.0,
	{
		{ X_MAXSIZE,     X_USEREFERENCE, REFMOVE_1_AXIS },
		{ Y_MAXSIZE,     Y_USEREFERENCE, REFMOVE_2_AXIS },
		{ Z_MAXSIZE,     Z_USEREFERENCE, REFMOVE_3_AXIS },
#if NUM_AXIS > 3
		{ A_MAXSIZE,     A_USEREFERENCE, REFMOVE_4_AXIS },
#endif
#if NUM_AXIS > 4
		{ B_MAXSIZE,     B_USEREFERENCE, REFMOVE_5_AXIS },
#endif
#if NUM_AXIS > 5
		{ C_MAXSIZE,     C_USEREFERENCE, REFMOVE_6_AXIS },
#endif
	}
};

////////////////////////////////////////////////////////////

void CMyControl::Init()
{
	CSingleton<CConfigEeprom>::GetInstance()->Init(sizeof(CConfigEeprom::SCNCEeprom), &_eepromFlash, EPROM_SIGNATURE);

#ifdef DISABLELEDBLINK
	DisableBlinkLed();
#endif

	StepperSerial.println(MESSAGE_MYCONTROL_Starting);

	super::Init();

	InitFromEeprom();

	_data.Init();

	//CStepper::GetInstance()->SetBacklash(5000);
	//CStepper::GetInstance()->SetBacklash(X_AXIS, CMotionControlBase::GetInstance()->ToMachine(X_AXIS, 20));
	//CStepper::GetInstance()->SetBacklash(Y_AXIS, CMotionControlBase::GetInstance()->ToMachine(Y_AXIS, 35));
	//CStepper::GetInstance()->SetBacklash(Z_AXIS, CMotionControl::ToMachine(Z_AXIS,20));

	//  CStepper::GetInstance()->SetMaxSpeed(20000);

	CStepper::GetInstance()->SetJerkSpeed(X_AXIS, SPEEDFACTOR*1000);
	CStepper::GetInstance()->SetJerkSpeed(Y_AXIS, SPEEDFACTOR*1000);
	CStepper::GetInstance()->SetJerkSpeed(Z_AXIS, SPEEDFACTOR*1000);
	CStepper::GetInstance()->SetJerkSpeed(A_AXIS, SPEEDFACTOR*1000);
	CStepper::GetInstance()->SetJerkSpeed(B_AXIS, SPEEDFACTOR*1000);

	CStepper::GetInstance()->SetEnableTimeout(A_AXIS, 2);
	CStepper::GetInstance()->SetEnableTimeout(B_AXIS, 2);

#if NUM_AXIS > 5
	CStepper::GetInstance()->SetLimitMax(C_AXIS, CMotionControl::ToMachine(C_AXIS,360000));
	CStepper::GetInstance()->SetJerkSpeed(C_AXIS, SPEEDFACTOR*1000);

	CStepper::GetInstance()->SetEnableTimeout(C_AXIS, 2);
#endif

#if NUM_AXIS < 6
// LCD KILL is shared with E1 (RampsFD) (DIR)
	_holdKillLcd.SetPin(CAT(BOARDNAME, _LCD_KILL_PIN), CAT(BOARDNAME, _LCD_KILL_PIN_ON));
#endif

	CGCodeParserDefault::InitAndSetFeedRate(-STEPRATETOFEEDRATE(GO_DEFAULT_STEPRATE), G1_DEFAULT_FEEDPRATE, STEPRATETOFEEDRATE(G1_DEFAULT_MAXSTEPRATE));

#ifdef MYUSE_LCD
	InitSD(SD_ENABLE_PIN);
#endif

}

////////////////////////////////////////////////////////////

void CMyControl::IOControl(uint8_t tool, unsigned short level)
{
	if (!_data.IOControl(tool, level))
	{
		super::IOControl(tool, level);
	}
}

////////////////////////////////////////////////////////////

unsigned short CMyControl::IOControl(uint8_t tool)
{
	switch (tool)
	{
		case SpindleCW:
		case SpindleCCW:	{ return _data._spindle.IsOn(); }
		case Probe:			{ return _data._probe.IsOn(); }
		case Coolant:		{ return _data._coolant.IsOn(); }
		case ControllerFan: { return _data._controllerfan.GetLevel(); }
	}

	return super::IOControl(tool);
}

////////////////////////////////////////////////////////////

void CMyControl::Kill()
{
	super::Kill();
	_data.Kill();
}

////////////////////////////////////////////////////////////

bool CMyControl::IsKill()
{
#if NUM_AXIS > 5
  return false;
#else
  return _holdKillLcd.IsOn();
#endif
	if (_data.IsKill())
	{
#ifdef MYUSE_LCD
		Lcd.Diagnostic(F("E-Stop"));
#endif
		return true;
	}
	return false;
}

////////////////////////////////////////////////////////////

void CMyControl::TimerInterrupt()
{
	super::TimerInterrupt();
	_data.TimerInterrupt();
#if NUM_AXIS < 6
	_holdKillLcd.Check();
#endif
}

////////////////////////////////////////////////////////////

void CMyControl::Initialized()
{
	super::Initialized();
	_data.Initialized();
}

////////////////////////////////////////////////////////////

void CMyControl::Poll()
{
	super::Poll();

	if (IsHold())
	{
		if (_data._resume.IsOn() || _data._holdresume.IsOn())
		{
			Resume();
#ifdef MYUSE_LCD
			Lcd.ClearDiagnostic();
#endif
		}
	}
	else if (_data._hold.IsOn() || _data._holdresume.IsOn())
	{
		Hold();
#ifdef MYUSE_LCD
		Lcd.Diagnostic(F("LCD Hold"));
#endif
	}
}

////////////////////////////////////////////////////////////

bool CMyControl::OnEvent(EnumAsByte(EStepperControlEvent) eventtype, uintptr_t addinfo)
{
	_data.OnEvent(eventtype, addinfo);
	return super::OnEvent(eventtype, addinfo);
}
