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

#include <SPI.h>
#include <SD.h>

#include <StepperSystem.h>

#include "GCode3DParser.h"
#include "MyControl.h"
#include "MyLcd.h"

////////////////////////////////////////////////////////////

CMyControl Control;
CGCodeTools GCodeTools;

////////////////////////////////////////////////////////////

void CMyControl::Init()
{
	StepperSerial.println(MESSAGE_MYCONTROL_Proxxon_Starting);

	super::Init();

	//CStepper::GetInstance()->SetWaitFinishMove(false); = > default changed
	
	//CStepper::GetInstance()->SetBacklash(5000);
	CStepper::GetInstance()->SetBacklash(X_AXIS, CMotionControl::ToMachine(X_AXIS,20));  
	CStepper::GetInstance()->SetBacklash(Y_AXIS, CMotionControl::ToMachine(Y_AXIS,35));  
	//CStepper::GetInstance()->SetBacklash(Z_AXIS, CMotionControl::ToMachine(Y_AXIS,20));

	//  CStepper::GetInstance()->SetMaxSpeed(20000);
	CStepper::GetInstance()->SetDefaultMaxSpeed(SPEED_MULTIPLIER_7, 350, 350);

	CStepper::GetInstance()->SetLimitMax(X_AXIS, CMotionControl::ToMachine(X_AXIS,130000));
	CStepper::GetInstance()->SetLimitMax(Y_AXIS, CMotionControl::ToMachine(Y_AXIS,45000));
	CStepper::GetInstance()->SetLimitMax(Z_AXIS, CMotionControl::ToMachine(Z_AXIS,81000));
	CStepper::GetInstance()->SetLimitMax(A_AXIS, CMotionControl::ToMachine(A_AXIS,360000));		// grad
	CStepper::GetInstance()->SetLimitMax(B_AXIS, CMotionControl::ToMachine(B_AXIS,360000));

	CStepper::GetInstance()->SetJerkSpeed(X_AXIS, 1000);
	CStepper::GetInstance()->SetJerkSpeed(Y_AXIS, 1000);
	CStepper::GetInstance()->SetJerkSpeed(Z_AXIS, 1000);
	CStepper::GetInstance()->SetJerkSpeed(A_AXIS, 1000);
	CStepper::GetInstance()->SetJerkSpeed(B_AXIS, 1000);

#if NUM_AXIS > 5
	CStepper::GetInstance()->SetLimitMax(C_AXIS, CMotionControl::ToMachine(B_AXIS,360000));
	CStepper::GetInstance()->SetJerkSpeed(C_AXIS, 1000);
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

	CProbeControl::Init();
	CGCode3DParser::Init();

	StepperSerial.print(MESSAGE_MYCONTROL_InitializingSDCard);

	ClearPrintFromSD ();

	pinMode(10, OUTPUT);
	digitalWrite(10, HIGH);
	pinMode(53, OUTPUT);

	if (!SD.begin(53))
	{
		StepperSerial.println(MESSAGE_MYCONTROL_initializationFailed);
	}
    else
    {
    	StepperSerial.println(MESSAGE_MYCONTROL_initializationDone);
    }
}

////////////////////////////////////////////////////////////

void CMyControl::IOControl(unsigned char tool, unsigned short level)
{
	switch (tool)
	{
		case Spindel:			_spindel.On(level);	return;
		case Coolant:			_coolant.On(level); return;
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
		case Coolant:		{ return _coolant.IsOn(); }
		case ControllerFan:	{ return _controllerfan.Level; }
	}

	return super::IOControl(tool);
}

////////////////////////////////////////////////////////////

void CMyControl::Kill()
{
	super::Kill();
	_spindel.On(0);
	_coolant.On(0);
}

////////////////////////////////////////////////////////////

void CMyControl::Initialized()
{
	super::Initialized();

	GoToReference();

	CGCode3DParser::GetExecutingFile() = SD.open("startup.nc", FILE_READ);

	if (CGCode3DParser::GetExecutingFile())
	{
		StepperSerial.println(MESSAGE_MYCONTROL_ExecutingStartupNc);
		StartPrintFromSD();
	}
	else
	{
		StepperSerial.println(MESSAGE_MYCONTROL_NoStartupNcFoundOnSD);
	}
	
	_controllerfan.Level=128;
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference()
{
	super::GoToReference();

	GoToReference(Z_AXIS);
	GoToReference(Y_AXIS);
	GoToReference(X_AXIS);
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference(axis_t axis)
{
#if defined(__SAM3X8E__)
	if (axis == Z_AXIS)
		CStepper::GetInstance()->SetPosition(axis, CStepper::GetInstance()->GetLimitMax(axis));
	else
		CStepper::GetInstance()->SetPosition(axis, 0);
#else
	if (axis == Z_AXIS)
	{
		// goto max
		CStepper::GetInstance()->MoveReference(axis, CStepper::GetInstance()->ToReferenceId(axis, false), false, FEEDRATE_REFMOVE);
	}
	else
	{
		// goto min
		CStepper::GetInstance()->MoveReference(axis, CStepper::GetInstance()->ToReferenceId(axis, true), true, FEEDRATE_REFMOVE);
	}
#endif
}

////////////////////////////////////////////////////////////

bool CMyControl::Parse()
{
	CGCode3DParser gcode(&_reader);
	return ParseAndPrintResult(&gcode);
}

////////////////////////////////////////////////////////////

void CMyControl::ReadAndExecuteCommand()
{
	super::ReadAndExecuteCommand();

	File file = CGCode3DParser::GetExecutingFile();
	if (PrintFromSDRunnding() && file)
	{
		if (IsKilled())
		{
			ClearPrintFromSD();
			file.close();
		}
		else
		{
			CGCode3DParser::SetExecutingFilePosition(file.position());

			FileReadAndExecuteCommand(&file);			// one line!!!

			if (file.available() == 0)
			{
				ClearPrintFromSD();
				file.close();
				StepperSerial.println(MESSAGE_MYCONTROL_ExecutingStartupNcDone);
			}
		}
	}
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
