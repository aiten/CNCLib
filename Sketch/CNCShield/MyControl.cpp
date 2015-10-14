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

#include <CNCLib.h>

#include <GCodeParserBase.h>
#include "MyControl.h"

////////////////////////////////////////////////////////////

CMyControl Control;
CMotionControlBase MotionControl;

////////////////////////////////////////////////////////////

void CMyControl::Init()
{
	DisableBlinkLed();

	StepperSerial.println(MESSAGE_MYCONTROL_CNCShield_Starting);

	CMotionControlBase::GetInstance()->Init();
	CMotionControlBase::GetInstance()->InitConversion(ConversionToMm1000, ConversionToMachine);

	super::Init();

	//CStepper::GetInstance()->SetBacklash(SPEEDFACTOR*5000);
	//CStepper::GetInstance()->SetBacklash(X_AXIS, CMotionControl::ToMachine(X_AXIS,20));  
	//CStepper::GetInstance()->SetBacklash(Y_AXIS, CMotionControl::ToMachine(Y_AXIS,35));  
	//CStepper::GetInstance()->SetBacklash(Z_AXIS, CMotionControl::ToMachine(Z_AXIS,20));

	CStepper::GetInstance()->SetLimitMax(X_AXIS, CMotionControlBase::GetInstance()->ToMachine(X_AXIS, MAXSIZE_X_AXIS));
	CStepper::GetInstance()->SetLimitMax(Y_AXIS, CMotionControlBase::GetInstance()->ToMachine(Y_AXIS, MAXSIZE_Y_AXIS));
	CStepper::GetInstance()->SetLimitMax(Z_AXIS, CMotionControlBase::GetInstance()->ToMachine(Z_AXIS, MAXSIZE_Z_AXIS));

#if CNCSHIELD_NUM_AXIS > 3
	CStepper::GetInstance()->SetLimitMax(A_AXIS, CMotionControlBase::GetInstance()->ToMachine(A_AXIS,MAXSIZE_A_AXIS));
#endif

	//CStepper::GetInstance()->SetJerkSpeed(X_AXIS, SPEEDFACTOR*1000);
	//CStepper::GetInstance()->SetJerkSpeed(Y_AXIS, SPEEDFACTOR*1000);
	//CStepper::GetInstance()->SetJerkSpeed(Z_AXIS, SPEEDFACTOR*1000);

	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(X_AXIS, true), true);
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Y_AXIS, true), true);
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Z_AXIS, false), true);

#ifdef CNCSHIELD_SPINDEL_ENABLE_PIN
	_spindel.Init();
#endif

#ifdef CNCSHIELD_PROBE_PIN
	_probe.Init();
#endif

	_kill.Init();
	_coolant.Init();

	_hold.SetPin(CNCSHIELD_HOLD_PIN);
	_resume.SetPin(CNCSHIELD_RESUME_PIN);

	CGCodeParserBase::Init();

	CGCodeParserBase::SetG0FeedRate(-STEPRATETOFEEDRATE(20000));
	CGCodeParserBase::SetG1FeedRate(STEPRATETOFEEDRATE(10000));

	CStepper::GetInstance()->SetDefaultMaxSpeed(CNC_MAXSPEED,CNC_ACC,CNC_DEC);
}

////////////////////////////////////////////////////////////

void CMyControl::IOControl(unsigned char tool, unsigned short level)
{
	switch (tool)
	{
#ifdef CNCSHIELD_SPINDEL_ENABLE_PIN
		case Spindel:			
			if (level != 0)
			{
#ifdef ANALOGSPINDELSPEED
				_spindel.On(MulDivU32(abs(level),255, MAXSPINDLESPEED));
#else        
        _spindel.On();
#endif
				_spindelDir.Set(((short)level)>0);
			}
			else
			{
			  _spindel.Off();
			}
			return;
#endif
	    case Coolant:     _coolant.Set(level>0); return;
	}
	
	super::IOControl(tool, level);
}

////////////////////////////////////////////////////////////

unsigned short CMyControl::IOControl(unsigned char tool)
{
	switch (tool)
	{
#ifdef CNCSHIELD_PROBE_PIN
		case Probe:			{ return _probe.IsOn(); }
#endif
#ifdef CNCSHIELD_SPINDEL_ENABLE_PIN
		case Spindel:		{ return _spindel.IsOn(); }
#endif
    case Coolant:   { return _coolant.IsOn(); }
	}

	return super::IOControl(tool);
}

////////////////////////////////////////////////////////////

void CMyControl::Kill()
{
	super::Kill();
#ifdef CNCSHIELD_SPINDEL_ENABLE_PIN
	_spindel.Off();
#endif
	_coolant.Set(false);
}

////////////////////////////////////////////////////////////

void CMyControl::TimerInterrupt()
{
	super::TimerInterrupt();
	_hold.Check();
	_resume.Check();
}

////////////////////////////////////////////////////////////

bool CMyControl::IsKill()
{
	return _kill.IsOn();
}

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

void CMyControl::GoToReference(axis_t axis, steprate_t /* steprate */)
{
	//  super::GoToReference(axis, CMotionControlBase::FeedRateToStepRate(axis, 300000));

	CStepper::GetInstance()->SetPosition(Z_AXIS, CStepper::GetInstance()->GetLimitMax(Z_AXIS));

	// force linking to see size used in sketch
	if (IsHold())
		super::GoToReference();
}

////////////////////////////////////////////////////////////

bool CMyControl::Parse(CStreamReader* reader, Stream* output)
{
	CGCodeParserBase gcode(reader,output);
	return ParseAndPrintResult(&gcode,output);
}

////////////////////////////////////////////////////////////
/*
bool CMyControl::OnStepperEvent(CStepper*stepper, EnumAsByte(CStepper::EStepperEvent) eventtype, void* addinfo)
{
	return super::OnStepperEvent(stepper, eventtype, addinfo);
}
*/
