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

#include <CNCLib.h>

#include <GCodeParserBase.h>
#include <GCodeParser.h>
#include "MyControl.h"

////////////////////////////////////////////////////////////

CMyControl Control;
CMotionControl MotionControl;
#define CMyParser CGCodeParser

////////////////////////////////////////////////////////////

#ifndef MYNUM_AXIS
#error Please define MYNUM_AXIS
#endif

////////////////////////////////////////////////////////////

void CMyControl::Init()
{
	DisableBlinkLed();

	StepperSerial.println(MESSAGE_MYCONTROL_Laser_Starting);

	CMotionControlBase::GetInstance()->Init();
	CMotionControlBase::GetInstance()->InitConversion(ConversionToMm1000, ConversionToMachine);

	super::Init();

	CStepper::GetInstance()->SetLimitMax(X_AXIS, CMotionControlBase::GetInstance()->ToMachine(X_AXIS, X_MAXSIZE));
	CStepper::GetInstance()->SetLimitMax(Y_AXIS, CMotionControlBase::GetInstance()->ToMachine(Y_AXIS, Y_MAXSIZE));
	CStepper::GetInstance()->SetLimitMax(Z_AXIS, CMotionControlBase::GetInstance()->ToMachine(Z_AXIS, Z_MAXSIZE));

	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(X_AXIS, true), true);
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Y_AXIS, false), true);

	_laserPWM.Init();
	_laserOnOff.Init();

	_laserWater.Init();
	_laserVacuum.Init();
	
	_kill.Init();

	_hold.SetPin(HOLD_PIN);
	_resume.SetPin(RESUME_PIN);

	CMyParser::Init();

	CGCodeParserBase::SetG0FeedRate(-STEPRATETOFEEDRATE(GO_DEFAULT_STEPRATE));
	CGCodeParserBase::SetG1FeedRate(STEPRATETOFEEDRATE(G1_DEFAULT_STEPRATE));
	CGCodeParserBase::SetG1MaxFeedRate(STEPRATETOFEEDRATE(G1_DEFAULT_MAXSTEPRATE));

	CStepper::GetInstance()->SetDefaultMaxSpeed(CNC_MAXSPEED, CNC_ACC, CNC_DEC);
}

////////////////////////////////////////////////////////////

void CMyControl::IOControl(unsigned char tool, unsigned short level)
{
	switch (tool)
	{
		case Laser:

			if (level != 0)
			{
				_laserPWM.On((unsigned char)level);
				_laserOnOff.On();
			}
			else
			{
				_laserOnOff.Off();
			}
			return;

		case Vacuum:  _laserVacuum.Set(level > 0); return;
		// case Coolant: _laserWater.Set(level > 0); return; do not allow water turn off

	}

	super::IOControl(tool, level);
}

////////////////////////////////////////////////////////////

unsigned short CMyControl::IOControl(unsigned char tool)
{
	switch (tool)
	{
		case Laser: { return _laserPWM.IsOn(); }
		case Coolant: { return _laserWater.IsOn(); }
		case Vacuum: { return _laserVacuum.IsOn(); }
	}

	return super::IOControl(tool);
}

////////////////////////////////////////////////////////////

void CMyControl::Kill()
{
	super::Kill();

	_laserOnOff.Off();
}

////////////////////////////////////////////////////////////

bool CMyControl::IsKill()
{
	return _kill.IsOn();
}

////////////////////////////////////////////////////////////

void CMyControl::TimerInterrupt()
{
	super::TimerInterrupt();

	_hold.Check();
	_resume.Check();
}

////////////////////////////////////////////////////////////

void CMyControl::Initialized()
{
	super::Initialized();
}

////////////////////////////////////////////////////////////

void CMyControl::Poll()
{
	super::Poll();

	if (IsHold())
	{
		if (_resume.IsOn())
		{
			Resume();
		}
	}
	else if (_hold.IsOn())
	{
		Hold();
	}
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference()
{
#ifdef NOGOTOREFERENCEATBOOT

#pragma message ("for test purpose only, not gotoReference at boot")

	CStepper::GetInstance()->SetPosition(Z_AXIS, CStepper::GetInstance()->GetLimitMax(Z_AXIS));

	// force linking to see size used in sketch
	if (IsHold())
		super::GoToReference(X_AXIS, CMotionControlBase::FeedRateToStepRate(X_AXIS, 300000), true);

#else

	GoToReference(Y_AXIS, 0, CStepper::GetInstance()->IsUseReference(Y_AXIS, true));
	GoToReference(X_AXIS, 0, CStepper::GetInstance()->IsUseReference(X_AXIS, true));

#endif
}

////////////////////////////////////////////////////////////

bool CMyControl::GoToReference(axis_t axis, steprate_t /* steprate */, bool toMinRef)
{
	return CStepper::GetInstance()->MoveReference(axis, CStepper::GetInstance()->ToReferenceId(axis, toMinRef), toMinRef, STEPRATERATE_REFMOVE, 0, MOVEAWAYFROMREF_STEPS);
}

////////////////////////////////////////////////////////////

bool CMyControl::Parse(CStreamReader* reader, Stream* output)
{
	CMyParser gcode(reader, output);
	return ParseAndPrintResult(&gcode, output);
}

////////////////////////////////////////////////////////////

bool CMyControl::OnStepperEvent(CStepper*stepper, EnumAsByte(CStepper::EStepperEvent) eventtype, void* addinfo)
{
	switch (eventtype)
	{
		case CStepper::OnStartEvent:
			_laserWater.On();
			_laserVacuum.On();
			break;
		case CStepper::OnIdleEvent:
			if (millis() - stepper->IdleTime() > LASERWATER_ONTIME)
			{
				_laserWater.Off();
			}
			if (millis() - stepper->IdleTime() > LASERVACUUM__ONTIME)
			{
				_laserVacuum.Off();
			}
			break;
	}

	return super::OnStepperEvent(stepper, eventtype, addinfo);
}
