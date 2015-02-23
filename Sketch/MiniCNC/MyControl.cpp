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
	StepperSerial.println(MESSAGE_MYCONTROL_Proxxon_Starting);

	CMotionControlBase::GetInstance()->InitConversion(ConversionToMm1000, ConversionToMachine);

	super::Init();

	//CStepper::GetInstance()->SetBacklash(SPEEDFACTOR*5000);
	//CStepper::GetInstance()->SetBacklash(X_AXIS, CMotionControl::ToMachine(X_AXIS,20));  
	//CStepper::GetInstance()->SetBacklash(Y_AXIS, CMotionControl::ToMachine(Y_AXIS,35));  
	//CStepper::GetInstance()->SetBacklash(Z_AXIS, CMotionControl::ToMachine(Z_AXIS,20));

	CStepper::GetInstance()->SetLimitMax(X_AXIS, CMotionControlBase::GetInstance()->ToMachine(X_AXIS, 130000));
	CStepper::GetInstance()->SetLimitMax(Y_AXIS, CMotionControlBase::GetInstance()->ToMachine(Y_AXIS, 45000));
	CStepper::GetInstance()->SetLimitMax(Z_AXIS, CMotionControlBase::GetInstance()->ToMachine(Z_AXIS, 81000));

#if NUM_AXIS > 3
	CStepper::GetInstance()->SetLimitMax(A_AXIS, CMotionControlBase::GetInstance()->ToMachine(A_AXIS,360000));
#endif

	//CStepper::GetInstance()->SetJerkSpeed(X_AXIS, SPEEDFACTOR*1000);
	//CStepper::GetInstance()->SetJerkSpeed(Y_AXIS, SPEEDFACTOR*1000);
	//CStepper::GetInstance()->SetJerkSpeed(Z_AXIS, SPEEDFACTOR*1000);

#if SPINDEL_PIN != -1
	_spindel.Init();
#endif
#if CONTROLLERFAN_FAN_PIN != -1
	_controllerfan.Init();
#endif

#if PROBE1_PIN != -1
	_probe.Init();
#endif
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
#if SPINDEL_PIN != -1
		case Spindel:			_spindel.Set(level>0);	return;
#endif
#if CONTROLLERFAN_FAN_PIN != -1
		case ControllerFan:		_controllerfan.Set(level>0);	return;
#endif
	}
	
	super::IOControl(tool, level);
}

////////////////////////////////////////////////////////////

unsigned short CMyControl::IOControl(unsigned char tool)
{
	switch (tool)
	{
#if PROBE1_PIN != -1
		case Probe:			{ return _probe.IsOn(); }
#endif
#if SPINDEL_PIN != -1
		case Spindel:		{ return _spindel.IsOn(); }
#endif
#if CONTROLLERFAN_FAN_PIN != -1
		case ControllerFan:	{ return _controllerfan.IsOn(); }
#endif
	}

	return super::IOControl(tool);
}

////////////////////////////////////////////////////////////

void CMyControl::Kill()
{
	super::Kill();
#if SPINDEL_PIN != -1
	_spindel.Set(false);
#endif
}

bool CMyControl::IsKill()
{
	return false;
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference()
{
	CStepper::GetInstance()->SetPosition(Z_AXIS, CStepper::GetInstance()->GetLimitMax(Z_AXIS));
//	super::GoToReference();
}

////////////////////////////////////////////////////////////

bool CMyControl::Parse(CStreamReader* reader, Stream* output)
{
	CGCodeParserBase gcode(reader,output);
	return ParseAndPrintResult(&gcode,output);
}

////////////////////////////////////////////////////////////

bool CMyControl::OnStepperEvent(CStepper*stepper, EnumAsByte(CStepper::EStepperEvent) eventtype, void* addinfo)
{
#if CONTROLLERFAN_FAN_PIN != -1
	switch (eventtype)
	{
		case CStepper::OnStartEvent:
			_controllerfan.Set(true);
			break;
		case CStepper::OnIdleEvent:
			if (millis()-stepper->IdleTime() > CONTROLLERFAN_ONTIME)
			{
				_controllerfan.Set(false);
			}
			break;
	}
#endif

	return super::OnStepperEvent(stepper, eventtype, addinfo);
}
