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
#include "MyControl.h"
#include "MyLcd.h"

////////////////////////////////////////////////////////////

CMyControl Control;
CGCodeTools GCodeTools;

CMotionControl MotionControl;

////////////////////////////////////////////////////////////

void CMyControl::Init()
{
	StepperSerial.println(MESSAGE_MYCONTROL_Starting);

	CMotionControlBase::GetInstance()->Init();
	CMotionControlBase::GetInstance()->InitConversion(ConversionToMm1000, ConversionToMachine);

	super::Init();

	CStepper::GetInstance()->SetDirection((1<<X_AXIS) + (1<<Y_AXIS));

	//CStepper::GetInstance()->SetBacklash(5000);
	//CStepper::GetInstance()->SetBacklash(X_AXIS, CMotionControlBase::GetInstance()->ToMachine(X_AXIS, 20));
	//CStepper::GetInstance()->SetBacklash(Y_AXIS, CMotionControlBase::GetInstance()->ToMachine(Y_AXIS, 35));
	//CStepper::GetInstance()->SetBacklash(Z_AXIS, CMotionControl::ToMachine(Z_AXIS,20));

	CStepper::GetInstance()->SetDefaultMaxSpeed(SPEED_MULTIPLIER_7, steprate_t(350), steprate_t(350));

	CStepper::GetInstance()->SetLimitMax(X_AXIS, CMotionControlBase::GetInstance()->ToMachine(X_AXIS, 800000));
	CStepper::GetInstance()->SetLimitMax(Y_AXIS, CMotionControlBase::GetInstance()->ToMachine(Y_AXIS, 500000));
	CStepper::GetInstance()->SetLimitMax(Z_AXIS, CMotionControlBase::GetInstance()->ToMachine(Z_AXIS, 100000));
//MSCV has only 3 axis!
#if NUM_AXIS > 3
	CStepper::GetInstance()->SetLimitMax(A_AXIS, CMotionControlBase::GetInstance()->ToMachine(A_AXIS, 360000));		// grad
	CStepper::GetInstance()->SetLimitMax(B_AXIS, CMotionControlBase::GetInstance()->ToMachine(B_AXIS, 360000));
#endif

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
	CStepper::GetInstance()->SetLimitMax(C_AXIS, CMotionControl::ToMachine(B_AXIS,360000));
	CStepper::GetInstance()->SetJerkSpeed(C_AXIS, 1000);

	CStepper::GetInstance()->SetEnableTimeout(C_AXIS, 2);
#endif

	CGCodeParserBase::SetG0FeedRate(-STEPRATETOFEEDRATE(30000));
	CGCodeParserBase::SetG1FeedRate(feedrate_t(100000));
	CGCodeParserBase::SetG1MaxFeedRate(STEPRATETOFEEDRATE(30000));

	for (register uint8_t i = 0; i < NUM_AXIS * 2; i++)
	{
		CStepper::GetInstance()->UseReference(i, false);
	}

	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(X_AXIS, true), true);
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Y_AXIS, true), true);
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Z_AXIS, false), true);

	CStepper::GetInstance()->SetPosition(Z_AXIS, CStepper::GetInstance()->GetLimitMax(Z_AXIS));

	_coolant.Init();
	_spindel.Init();
	_controllerfan.Init(255);

	_probe.Init(MASH6050S_INPUTPINMODE);
	_kill.Init(MASH6050S_INPUTPINMODE);

	_killLcd.SetPin(CAT(BOARDNAME, _LCD_KILL_PIN), CAT(BOARDNAME, _LCD_KILL_PIN_ON));

	InitSD(SD_ENABLE_PIN);
}

////////////////////////////////////////////////////////////

void CMyControl::IOControl(uint8_t tool, unsigned short level)
{
	switch (tool)
	{
		case Spindel:			_spindel.Set(level>0);	return;
		case Coolant:			_coolant.Set(level>0); return;
		case ControllerFan:		_controllerfan.Level = (uint8_t)level;		return;
		case Vacuum:			break;
	}
	
	super::IOControl(tool, level);
}

////////////////////////////////////////////////////////////

unsigned short CMyControl::IOControl(uint8_t tool)
{
	switch (tool)
	{
		case Probe:			{ return _probe.IsOn(); }
		case Spindel:		{ return _spindel.IsOn(); }
		case Coolant:		{ return _coolant.IsOn(); }
		case ControllerFan:	{ return _controllerfan.Level; }
		case Vacuum:		break;
	}

	return super::IOControl(tool);
}

////////////////////////////////////////////////////////////

void CMyControl::Kill()
{
	super::Kill();
	_spindel.Set(false);
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

void CMyControl::Poll()
{
	super::Poll();

	if (IsHold())
	{
		if (_killLcd.IsOn())
		{
			Resume();
      Lcd.ClearDiagnostic();
		}
	} else if (_killLcd.IsOn())
	{
		Hold();
		Lcd.Diagnostic(F("LCD Hold"));
	}
}

////////////////////////////////////////////////////////////

void CMyControl::TimerInterrupt()
{
	super::TimerInterrupt();
	_killLcd.Check();
}

////////////////////////////////////////////////////////////

void CMyControl::Initialized()
{
	super::Initialized();
	_controllerfan.Level=128;
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference()
{
	GoToReference(Z_AXIS, 0, false);
	GoToReference(Y_AXIS, 0, true);
	GoToReference(X_AXIS, 0, true);
}

////////////////////////////////////////////////////////////

bool CMyControl::GoToReference(axis_t axis, steprate_t /* steprate */, bool toMinRef)
{
	if (!super::GoToReference(axis, CMotionControlBase::FeedRateToStepRate(axis,1000000), toMinRef))
		return false;

	return super::GoToReference(axis, CMotionControlBase::FeedRateToStepRate(axis, 200000), toMinRef);
}

////////////////////////////////////////////////////////////

bool CMyControl::OnEvent(EnumAsByte(CStepper::EStepperEvent) eventtype, uintptr_t addinfo)
{
	switch (eventtype)
	{
		case CStepper::OnStartEvent:
			_controllerfan.On();
			break;
		case CStepper::OnIdleEvent:
			if (millis()- CStepper::GetInstance()->IdleTime() > CONTROLLERFAN_ONTIME)
			{
				_controllerfan.Off();
			}
			break;
	}

	return super::OnEvent(eventtype, addinfo);
}
