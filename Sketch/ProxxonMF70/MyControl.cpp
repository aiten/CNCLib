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
	StepperSerial.println(MESSAGE_MYCONTROL_Proxxon_Starting);

	CMotionControlBase::GetInstance()->Init();
	CMotionControlBase::GetInstance()->InitConversion(ConversionToMm1000, ConversionToMachine);

	super::Init();

	//CStepper::GetInstance()->SetBacklash(SPEEDFACTOR*5000);
	CStepper::GetInstance()->SetBacklash(X_AXIS, CMotionControlBase::GetInstance()->ToMachine(X_AXIS, 20));
	CStepper::GetInstance()->SetBacklash(Y_AXIS, CMotionControlBase::GetInstance()->ToMachine(Y_AXIS, 35));
	//CStepper::GetInstance()->SetBacklash(Z_AXIS, CMotionControl::ToMachine(Z_AXIS,20));

	//  CStepper::GetInstance()->SetMaxSpeed(20000);
	CStepper::GetInstance()->SetDefaultMaxSpeed(SPEED_MULTIPLIER_7, steprate_t(350*SPEEDFACTOR_SQT), steprate_t(350*SPEEDFACTOR_SQT));

	CGCodeParserBase::SetG0FeedRate(-feedrate_t(526518));					// feedrate_t(526518) => VMAXTOFEEDRATE(((SPEED_MULTIPLIER_4)-5))
	CGCodeParserBase::SetG1FeedRate(feedrate_t(100000));
	CGCodeParserBase::SetG1MaxFeedRate(feedrate_t(500000));

	CStepper::GetInstance()->SetLimitMax(X_AXIS, CMotionControlBase::GetInstance()->ToMachine(X_AXIS, 130000));
	CStepper::GetInstance()->SetLimitMax(Y_AXIS, CMotionControlBase::GetInstance()->ToMachine(Y_AXIS, 45000));
	CStepper::GetInstance()->SetLimitMax(Z_AXIS, CMotionControlBase::GetInstance()->ToMachine(Z_AXIS, 81000));
	CStepper::GetInstance()->SetLimitMax(A_AXIS, CMotionControlBase::GetInstance()->ToMachine(A_AXIS, 360000));		// grad
	CStepper::GetInstance()->SetLimitMax(B_AXIS, CMotionControlBase::GetInstance()->ToMachine(B_AXIS, 360000));

	CStepper::GetInstance()->SetJerkSpeed(X_AXIS, SPEEDFACTOR*1000);
	CStepper::GetInstance()->SetJerkSpeed(Y_AXIS, SPEEDFACTOR*1000);
	CStepper::GetInstance()->SetJerkSpeed(Z_AXIS, SPEEDFACTOR*1000);
	CStepper::GetInstance()->SetJerkSpeed(A_AXIS, SPEEDFACTOR*1000);
	CStepper::GetInstance()->SetJerkSpeed(B_AXIS, SPEEDFACTOR*1000);

	CStepper::GetInstance()->SetEnableTimeout(A_AXIS, 2);
	CStepper::GetInstance()->SetEnableTimeout(B_AXIS, 2);

#if NUM_AXIS > 5
	CStepper::GetInstance()->SetLimitMax(C_AXIS, CMotionControl::ToMachine(B_AXIS,360000));
	CStepper::GetInstance()->SetJerkSpeed(C_AXIS, SPEEDFACTOR*1000);

	CStepper::GetInstance()->SetEnableTimeout(C_AXIS, 2);
#endif

	for (register unsigned char i = 0; i < NUM_AXIS * 2; i++)
	{
		CStepper::GetInstance()->UseReference(i, false);
	}

	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(X_AXIS, true), true);
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Y_AXIS, true), true);
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Z_AXIS, false), true);

	CStepper::GetInstance()->SetPosition(Z_AXIS, CStepper::GetInstance()->GetLimitMax(Z_AXIS));

	_coolant.Init();
	_spindel.Init();
	_controllerfan.Init();

	_probe.Init();

// LCD KILL is shared with E! (DIR)
	_holdLcd.SetPin(CAT(BOARDNAME, _LCD_KILL_PIN), CAT(BOARDNAME, _LCD_KILL_PIN_ON));
//	_killLcd.Init();

	InitSD(SD_ENABLE_PIN);
}

////////////////////////////////////////////////////////////

void CMyControl::IOControl(unsigned char tool, unsigned short level)
{
	switch (tool)
	{
		case Spindel:			_spindel.Set(level>0);	return;
		case Coolant:			_coolant.Set(level>0); return;
		case ControllerFan:		_controllerfan.Level = (unsigned char)level;		return;
//		case Vacuum:			break;
	}
	
	super::IOControl(tool, level);
}

////////////////////////////////////////////////////////////

unsigned short CMyControl::IOControl(unsigned char tool)
{
	switch (tool)
	{
		case Probe:			{ return _probe.IsOn(); }
		case Spindel:		{ return _spindel.IsOn(); }
		case Coolant:		{ return _coolant.IsOn(); }
		case ControllerFan:	{ return _controllerfan.Level; }
//		case Vacuum:		break;
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

bool CMyControl::IsButton(EnumAsByte(EIOButtons) button)
{
	switch (button)
	{
		default:	break;
//		case KillButton:	
		case HoldButton:		return _holdLcd.IsOn();
	}

	return false;
}

////////////////////////////////////////////////////////////

void CMyControl::Poll()
{
	super::Poll();

	if (_holdLcd.IsOn())
	{
		if (IsHold())
			Resume();
		else
			Hold();
	}
}

////////////////////////////////////////////////////////////

void CMyControl::TimerInterrupt()
{
	_holdLcd.Check();
	super::TimerInterrupt();
}

////////////////////////////////////////////////////////////

void CMyControl::Initialized()
{
	super::Initialized();
	_controllerfan.Level=128;
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference(axis_t axis, steprate_t /* steprate */)
{
#if defined(__SAM3X8E__)
	if (axis == Z_AXIS)
		CStepper::GetInstance()->SetPosition(axis, CStepper::GetInstance()->GetLimitMax(axis));
	else
		CStepper::GetInstance()->SetPosition(axis, 0);
#else
	super::GoToReference(axis, CMotionControlBase::FeedRateToStepRate(axis, 300000));
#endif
}

////////////////////////////////////////////////////////////

bool CMyControl::OnStepperEvent(CStepper*stepper, EnumAsByte(CStepper::EStepperEvent) eventtype, void* addinfo)
{
	switch (eventtype)
	{
		case CStepper::OnStartEvent:
			_controllerfan.On();
			break;
		case CStepper::OnIdleEvent:
			if (millis()-stepper->IdleTime() > CONTROLLERFAN_ONTIME)
			{
				_controllerfan.Off();
			}
			break;
	}

	return super::OnStepperEvent(stepper, eventtype, addinfo);
}
