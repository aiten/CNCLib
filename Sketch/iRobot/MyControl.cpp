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

#include <GCodeParserBase.h>
#include <GCodeParser.h>
#include "StepperServo.h"
#include "MyControl.h"
#include "MyMotionControl.h"
#include "MyParser.h"
#include "MyLcd.h"

////////////////////////////////////////////////////////////

CMyControl Control;
CGCodeTools GCodeTools;

CMyMotionControl MotionControl;

////////////////////////////////////////////////////////////

void CMyControl::Init()
{
	StepperSerial.println(MESSAGE_MYCONTROL_iRobot_Starting);

	CMotionControlBase::GetInstance()->Init();
	CMotionControlBase::GetInstance()->InitConversion(ConversionToMm1000, ConversionToMachine);

	super::Init();

	CStepper::GetInstance()->SetLimitMin(X_AXIS, MIN_LIMIT);  // ms
	CStepper::GetInstance()->SetLimitMin(Y_AXIS, MIN_LIMIT);
	CStepper::GetInstance()->SetLimitMin(Z_AXIS, MIN_LIMIT);
	CStepper::GetInstance()->SetLimitMin(A_AXIS, MIN_LIMIT);

	CStepper::GetInstance()->SetLimitMax(X_AXIS, MAX_LIMIT);  // ms
	CStepper::GetInstance()->SetLimitMax(Y_AXIS, MAX_LIMIT);
	CStepper::GetInstance()->SetLimitMax(Z_AXIS, MAX_LIMIT);
	CStepper::GetInstance()->SetLimitMax(A_AXIS, MAX_LIMIT);

	CGCodeParserBase::SetG0FeedRate(-STEPRATETOFEEDRATE(30000));
	CGCodeParserBase::SetG1FeedRate(STEPRATETOFEEDRATE(10000));

	CStepper::GetInstance()->SetDefaultMaxSpeed(CNC_MAXSPEED,CNC_ACC,CNC_DEC);
	_killLcd.Init();

  InitSD(SD_ENABLE_PIN);

}

////////////////////////////////////////////////////////////
/*
void CMyControl::IOControl(unsigned char tool, unsigned short level)
{
	switch (tool)
	{
	}
	
	super::IOControl(tool, level);
}

////////////////////////////////////////////////////////////

unsigned short CMyControl::IOControl(unsigned char tool)
{
	switch (tool)
	{
	}

	return super::IOControl(tool);
}
*/
////////////////////////////////////////////////////////////

void CMyControl::Kill()
{
	super::Kill();
}

////////////////////////////////////////////////////////////

bool CMyControl::IsKill()
{
	if (_killLcd.IsOn())
	{
		Lcd.Diagnostic(F("LCD E-Stop"));
		return true;
	}
	return false;
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference()
{
  CStepper::GetInstance()->SetPosition(X_AXIS, INIT_PULS1);
  CStepper::GetInstance()->SetPosition(Y_AXIS, INIT_PULS2);
  CStepper::GetInstance()->SetPosition(Z_AXIS, INIT_PULS3);
  CStepper::GetInstance()->SetPosition(A_AXIS, CENTER_LIMIT);

  ((CStepperServo*)CStepper::GetInstance())->SetServo();
}

////////////////////////////////////////////////////////////

bool CMyControl::Parse(CStreamReader* reader, Stream* output)
{
	CMyParser gcode(reader,output);
	return ParseAndPrintResult(&gcode,output);
}

////////////////////////////////////////////////////////////

bool CMyControl::OnStepperEvent(CStepper*stepper, EnumAsByte(CStepper::EStepperEvent) eventtype, void* addinfo)
{
	return super::OnStepperEvent(stepper, eventtype, addinfo);
}
