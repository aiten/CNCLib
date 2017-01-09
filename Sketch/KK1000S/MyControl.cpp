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

#include <SPI.h>
#include <SD.h>

#include <CNCLib.h>
#include <CNCLibEx.h>

#include <GCode3DParser.h>
#include <ConfigEeprom.h>

#include "MyControl.h"
#include "MyLcd.h"

////////////////////////////////////////////////////////////

CMyControl Control;
CGCodeTools GCodeTools;

CMotionControl MotionControl;
CConfigEeprom Eprom;

HardwareSerial& StepperSerial = Serial;
////////////////////////////////////////////////////////////

static const CConfigEeprom::SCNCEeprom eepromFlash PROGMEM =
{
	0x21436587,
	NUM_AXIS, MYNUM_AXIS, offsetof(CConfigEeprom::SCNCEeprom,axis), sizeof(CConfigEeprom::SCNCEeprom::SAxisDefinitions),
	0,
	CNC_MAXSPEED,
	CNC_ACC,
	CNC_DEC,
	0,// STEPRATERATE_REFMOVE,
	(1000.0 / X_STEPSPERMM),
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
	CSingleton<CConfigEeprom>::GetInstance()->Init(sizeof(CConfigEeprom::SCNCEeprom), &eepromFlash, 0x21436587);

	StepperSerial.println(MESSAGE_MYCONTROL_Starting);

	CMotionControlBase::GetInstance()->InitConversion(ConversionToMm1000, ConversionToMachine);

	super::Init();

	CStepper::GetInstance()->SetDirection((1<<X_AXIS) + (1<<Y_AXIS));

	//CStepper::GetInstance()->SetBacklash(5000);
	//CStepper::GetInstance()->SetBacklash(X_AXIS, CMotionControlBase::GetInstance()->ToMachine(X_AXIS, 20));
	//CStepper::GetInstance()->SetBacklash(Y_AXIS, CMotionControlBase::GetInstance()->ToMachine(Y_AXIS, 35));
	//CStepper::GetInstance()->SetBacklash(Z_AXIS, CMotionControl::ToMachine(Z_AXIS,20));

	CStepper::GetInstance()->SetDefaultMaxSpeed(SPEED_MULTIPLIER_7, steprate_t(350), steprate_t(350));

	CStepper::GetInstance()->SetJerkSpeed(X_AXIS, 1000);
	CStepper::GetInstance()->SetJerkSpeed(Y_AXIS, 1000);
	CStepper::GetInstance()->SetJerkSpeed(Z_AXIS, 1000);
#if NUM_AXIS > 3
	CStepper::GetInstance()->SetJerkSpeed(A_AXIS, 1000);
	CStepper::GetInstance()->SetJerkSpeed(B_AXIS, 1000);

	CStepper::GetInstance()->SetEnableTimeout(A_AXIS, 2);
	CStepper::GetInstance()->SetEnableTimeout(B_AXIS, 2);
#endif

#if NUM_AXIS > 5
	CStepper::GetInstance()->SetJerkSpeed(C_AXIS, 1000);
	CStepper::GetInstance()->SetEnableTimeout(C_AXIS, 2);
#endif

	for (uint8_t axis = 0; axis < NUM_AXIS; axis++)
	{
		EnumAsByte(EReverenceType) ref = (EReverenceType)CConfigEeprom::GetConfigU8(offsetof(CConfigEeprom::SCNCEeprom, axis[0].referenceType) + sizeof(CConfigEeprom::SCNCEeprom::SAxisDefinitions)*axis);
		if (ref != NoReference)
			CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(axis, ref == EReverenceType::ReferenceToMin), true);

		CStepper::GetInstance()->SetLimitMax(axis, CMotionControlBase::GetInstance()->ToMachine(axis, CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, axis[0].size) + sizeof(CConfigEeprom::SCNCEeprom::SAxisDefinitions)*axis)));
	}

	_controllerfan.Init(128);

	_spindel.Init();
	_probe.Init();
	_kill.Init();
	_coolant.Init();

	_probe.Init(MASH6050S_INPUTPINMODE);
	_kill.Init(MASH6050S_INPUTPINMODE);

	_holdresume.SetPin(CAT(BOARDNAME, _LCD_KILL_PIN), CAT(BOARDNAME, _LCD_KILL_PIN_ON));

	CGCodeParser::Init();
	CGCodeParserBase::InitAndSetFeedRate(-STEPRATETOFEEDRATE(GO_DEFAULT_STEPRATE), STEPRATETOFEEDRATE(G1_DEFAULT_STEPRATE), STEPRATETOFEEDRATE(G1_DEFAULT_MAXSTEPRATE));
	CStepper::GetInstance()->SetDefaultMaxSpeed(
		((steprate_t)CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, maxsteprate))),
		((steprate_t)CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, acc))),
		((steprate_t)CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, dec))));
	InitSD(SD_ENABLE_PIN);
}

////////////////////////////////////////////////////////////

void CMyControl::IOControl(uint8_t tool, unsigned short level)
{
	switch (tool)
	{
		case Spindel:		_spindel.On(ConvertSpindelSpeedToIO(level)); _spindelDir.Set(((short)level) > 0);	return;
		case Coolant:		_coolant.Set(level > 0); return;
		case ControllerFan:	_controllerfan.SetLevel((uint8_t)level); return;
	}

	super::IOControl(tool, level);
}

////////////////////////////////////////////////////////////

unsigned short CMyControl::IOControl(uint8_t tool)
{
	switch (tool)
	{
		case Spindel:		{ return _spindel.IsOn(); }
		case Probe:			{ return _probe.IsOn(); }
		case Coolant:		{ return _coolant.IsOn(); }
		case ControllerFan: { return _controllerfan.GetLevel(); }
	}

	return super::IOControl(tool);
}

////////////////////////////////////////////////////////////

void CMyControl::Kill()
{
	super::Kill();

	_spindel.Off();
	_coolant.Set(false);
}

////////////////////////////////////////////////////////////

bool CMyControl::IsKill()
{
	if (_kill.IsOn())
	{
		Lcd.Diagnostic(F("KK1000S E-Stop"));
		return true;
	}
	return false;
}

////////////////////////////////////////////////////////////

void CMyControl::TimerInterrupt()
{
	super::TimerInterrupt();
	_holdresume.Check();
	_hold.Check();
	_resume.Check();
}

////////////////////////////////////////////////////////////

void CMyControl::Initialized()
{
	super::Initialized();

	_controllerfan.SetLevel(128);
}

////////////////////////////////////////////////////////////

void CMyControl::Poll()
{
	super::Poll();

	if (IsHold())
	{
		if (_holdresume.IsOn())
		{
			Resume();
			Lcd.ClearDiagnostic();
		}
	} else if (_holdresume.IsOn())
	{
		Hold();
		Lcd.Diagnostic(F("LCD Hold"));
	}
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference()
{
	for (axis_t i = 0; i < NUM_AXIS; i++)
	{
		axis_t axis = CConfigEeprom::GetConfigU8(offsetof(CConfigEeprom::SCNCEeprom, axis[0].refmoveSequence)+sizeof(CConfigEeprom::SCNCEeprom::SAxisDefinitions)*i);
		if (axis < NUM_AXIS)
		{
			EnumAsByte(EReverenceType) referenceType = (EReverenceType)CConfigEeprom::GetConfigU8(offsetof(CConfigEeprom::SCNCEeprom, axis[0].referenceType)+sizeof(CConfigEeprom::SCNCEeprom::SAxisDefinitions)*axis);
			if (referenceType != EReverenceType::NoReference)
				GoToReference(axis,	(steprate_t) CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, refmovesteprate)),referenceType == EReverenceType::ReferenceToMin);
		}
	}
}

////////////////////////////////////////////////////////////

bool CMyControl::GoToReference(axis_t axis, steprate_t /* steprate */, bool toMinRef)
{
	if (!super::GoToReference(axis, CMotionControlBase::FeedRateToStepRate(axis,1000000), toMinRef))
		return false;

	return super::GoToReference(axis, CMotionControlBase::FeedRateToStepRate(axis, 200000), toMinRef);
}

////////////////////////////////////////////////////////////

bool CMyControl::OnEvent(EnumAsByte(EStepperControlEvent) eventtype, uintptr_t addinfo)
{
	switch (eventtype)
	{
		case OnStartEvent:
			_controllerfan.On();
			break;
		case OnIdleEvent:
			if (IsControllerFanTimeout())
			{
				_controllerfan.Off();
			}
			break;
	}

	return super::OnEvent(eventtype, addinfo);
}
