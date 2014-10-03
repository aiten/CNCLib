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

#include <CNCLib.h>
#include <CNCLibEx.h>

#include <GCode3DParser.h>
#include "MyControl.h"
#include "MyLcd.h"

////////////////////////////////////////////////////////////

CMyControl Control;
CGCodeTools GCodeTools;

////////////////////////////////////////////////////////////

void CMyControl::Init()
{
	StepperSerial.println(MESSAGE_MYCONTROL_Proxxon_Starting);

	CMotionControl::InitConversion(ConversionToMm1000,ConversionToMachine);

	super::Init();

	//CStepper::GetInstance()->SetBacklash(SPEEDFACTOR*5000);
	CStepper::GetInstance()->SetBacklash(X_AXIS, CMotionControl::ToMachine(X_AXIS,20));  
	CStepper::GetInstance()->SetBacklash(Y_AXIS, CMotionControl::ToMachine(Y_AXIS,35));  
	//CStepper::GetInstance()->SetBacklash(Z_AXIS, CMotionControl::ToMachine(Z_AXIS,20));

	//  CStepper::GetInstance()->SetMaxSpeed(20000);
	CStepper::GetInstance()->SetDefaultMaxSpeed(SPEED_MULTIPLIER_7, steprate_t(350*SPEEDFACTOR_SQT), steprate_t(350*SPEEDFACTOR_SQT));

	CStepper::GetInstance()->SetLimitMax(X_AXIS, CMotionControl::ToMachine(X_AXIS,130000));
	CStepper::GetInstance()->SetLimitMax(Y_AXIS, CMotionControl::ToMachine(Y_AXIS,45000));
	CStepper::GetInstance()->SetLimitMax(Z_AXIS, CMotionControl::ToMachine(Z_AXIS,81000));
	CStepper::GetInstance()->SetLimitMax(A_AXIS, CMotionControl::ToMachine(A_AXIS,360000));		// grad
	CStepper::GetInstance()->SetLimitMax(B_AXIS, CMotionControl::ToMachine(B_AXIS,360000));

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
	CGCode3DParser::Init();

	StepperSerial.print(MESSAGE_MYCONTROL_InitializingSDCard);

	ClearPrintFromSD ();

	CHAL::pinMode(SD_ENABLE_PIN, OUTPUT);
	CHAL::digitalWrite(SD_ENABLE_PIN, HIGH);

	if (!SD.begin(SD_ENABLE_PIN))
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
		case Probe:			{ return _probe.IsOn(); }
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
	bool toMax = axis == Z_AXIS;
	CStepper::GetInstance()->MoveReference(axis, CStepper::GetInstance()->ToReferenceId(axis, toMax), toMax, STEPRATE_REFMOVE);
#endif
}

////////////////////////////////////////////////////////////

bool CMyControl::Parse(CStreamReader* reader, Stream* output)
{
	CGCode3DParser gcode(reader,output);
	return ParseAndPrintResult(&gcode,output);
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

			FileReadAndExecuteCommand(&file,NULL);			// one line!!! Output goes to NULL

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

bool CMyControl::OnStepperEvent(CStepper*stepper, EnumAsByte(CStepper::EStepperEvent) eventtype, void* addinfo)
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
