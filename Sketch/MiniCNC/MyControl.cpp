////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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

////////////////////////////////////////////////////////////

CMyControl Control;

////////////////////////////////////////////////////////////

void CMyControl::Init()
{
	StepperSerial.println(MESSAGE_MYCONTROL_Proxxon_Starting);

	CMotionControl::InitConversion(ConversionToMm1000,ConversionToMachine);

	super::Init();

	//CStepper::GetInstance()->SetBacklash(SPEEDFACTOR*5000);
	//CStepper::GetInstance()->SetBacklash(X_AXIS, CMotionControl::ToMachine(X_AXIS,20));  
	//CStepper::GetInstance()->SetBacklash(Y_AXIS, CMotionControl::ToMachine(Y_AXIS,35));  
	//CStepper::GetInstance()->SetBacklash(Z_AXIS, CMotionControl::ToMachine(Z_AXIS,20));

	//  CStepper::GetInstance()->SetMaxSpeed(20000);
	//CStepper::GetInstance()->SetDefaultMaxSpeed(SPEED_MULTIPLIER_7, steprate_t(350*SPEEDFACTOR_SQT), steprate_t(350*SPEEDFACTOR_SQT));

	CStepper::GetInstance()->SetLimitMax(X_AXIS, CMotionControl::ToMachine(X_AXIS,130000));
	CStepper::GetInstance()->SetLimitMax(Y_AXIS, CMotionControl::ToMachine(Y_AXIS,45000));
	CStepper::GetInstance()->SetLimitMax(Z_AXIS, CMotionControl::ToMachine(Z_AXIS,81000));

	//CStepper::GetInstance()->SetJerkSpeed(X_AXIS, SPEEDFACTOR*1000);
	//CStepper::GetInstance()->SetJerkSpeed(Y_AXIS, SPEEDFACTOR*1000);
	//CStepper::GetInstance()->SetJerkSpeed(Z_AXIS, SPEEDFACTOR*1000);

	CStepper::GetInstance()->SetPosition(Z_AXIS, CStepper::GetInstance()->GetLimitMax(Z_AXIS));

	_spindel.Init();
	_controllerfan.Init();

	CProbeControl::Init();
	CGCodeParser::Init();
}

////////////////////////////////////////////////////////////

void CMyControl::IOControl(unsigned char tool, unsigned short level)
{
	switch (tool)
	{
		case Spindel:			_spindel.On(level);	return;
		case ControllerFan:		_controllerfan.Level = (unsigned char)level;		return;
	}
	
	super::IOControl(tool, level);
}

////////////////////////////////////////////////////////////

unsigned short CMyControl::IOControl(unsigned char tool)
{
	switch (tool)
	{
		case Probe:			{ CProbeControl probe;	return probe.IsOn(); }
		case Spindel:		{ return _spindel.IsOn(); }
		case ControllerFan:	{ return _controllerfan.Level; }
	}

	return super::IOControl(tool);
}

////////////////////////////////////////////////////////////

void CMyControl::Kill()
{
	super::Kill();
	_spindel.On(0);
}

////////////////////////////////////////////////////////////

void CMyControl::Initialized()
{
	super::Initialized();

	GoToReference();

	_controllerfan.Level=128;
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference()
{
	super::GoToReference();
return;
	GoToReference(Z_AXIS);
	GoToReference(Y_AXIS);
	GoToReference(X_AXIS);
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference(axis_t axis)
{
	// goto min/max
	CStepper::GetInstance()->MoveReference(axis, CStepper::GetInstance()->ToReferenceId(axis, axis == Z_AXIS), axis == Z_AXIS, STEPRATE_REFMOVE);
}

////////////////////////////////////////////////////////////

bool CMyControl::Parse(CStreamReader* reader, Stream* output)
{
	CGCodeParser gcode(reader,output);
	return ParseAndPrintResult(&gcode,output);
}

////////////////////////////////////////////////////////////

bool CMyControl::OnStepperEvent(CStepper*stepper, EnumAsByte(CStepper::EStepperEvent) eventtype, unsigned char addinfo)
{
	switch (eventtype)
	{
		case CStepper::OnStartEvent:
			_controllerfan.On();
			break;
		case CStepper::OnIdleEvent:
			if (millis()-CStepper::GetInstance()->IdleTime() > CONTROLLERFAN_ONTIME)
			{
				_controllerfan.Off();
			}
			break;
	}

	return super::OnStepperEvent(stepper, eventtype, addinfo);
}
