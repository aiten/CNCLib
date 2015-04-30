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
#include <GCodeParser.h>
#include <GCode3DParser.h>
#include "StepperServo.h"
#include "MyControl.h"

////////////////////////////////////////////////////////////

CMyControl Control;

CMotionControlBase MotionControl;

////////////////////////////////////////////////////////////

void CMyControl::Init()
{
	StepperSerial.println(MESSAGE_MYCONTROL_iRobot_Starting);

	CMotionControlBase::GetInstance()->InitConversion(ConversionToMm1000, ConversionToMachine);

	super::Init();

	CStepper::GetInstance()->SetLimitMax(X_AXIS, MAX_PULSE_WIDTH-MIN_PULSE_WIDTH);  // ms
	CStepper::GetInstance()->SetLimitMax(Y_AXIS, MAX_PULSE_WIDTH-MIN_PULSE_WIDTH);
	CStepper::GetInstance()->SetLimitMax(Z_AXIS, MAX_PULSE_WIDTH-MIN_PULSE_WIDTH);
	CStepper::GetInstance()->SetLimitMax(A_AXIS, MAX_PULSE_WIDTH-MIN_PULSE_WIDTH);


	//CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(X_AXIS, true), true);
	//CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Y_AXIS, true), true);
	//CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Z_AXIS, false), true);
	//CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(A_AXIS, false), true);

#if KILL_PIN != -1
	_kill.Init();
#endif

	CGCodeParserBase::Init();

	CGCodeParserBase::SetG0FeedRate(-STEPRATETOFEEDRATE(30000));
	CGCodeParserBase::SetG1FeedRate(STEPRATETOFEEDRATE(10000));

	CStepper::GetInstance()->SetDefaultMaxSpeed(CNC_MAXSPEED,CNC_ACC,CNC_DEC);
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
/*
void CMyControl::Kill()
{
	super::Kill();
}
*/
bool CMyControl::IsKill()
{
#if KILL_PIN != -1
	return _kill.IsOn();
#else
	return false;
#endif
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference()
{
  CStepper::GetInstance()->SetPosition(X_AXIS, CStepper::GetInstance()->GetLimitMax(X_AXIS)/2);
  CStepper::GetInstance()->SetPosition(Y_AXIS, CStepper::GetInstance()->GetLimitMax(Y_AXIS)/2);
  CStepper::GetInstance()->SetPosition(Z_AXIS, CStepper::GetInstance()->GetLimitMax(Z_AXIS)/2);
  CStepper::GetInstance()->SetPosition(A_AXIS, CStepper::GetInstance()->GetLimitMax(A_AXIS)/2);

((CStepperServo*)CStepper::GetInstance())->SetServo();

	// force linking to see size used in sketch
//	if (_controllerfan.IsOn())
//		super::GoToReference();
}

////////////////////////////////////////////////////////////

bool CMyControl::Parse(CStreamReader* reader, Stream* output)
{
	CGCode3DParser gcode(reader,output);
	return ParseAndPrintResult(&gcode,output);
}

////////////////////////////////////////////////////////////

bool CMyControl::OnStepperEvent(CStepper*stepper, EnumAsByte(CStepper::EStepperEvent) eventtype, void* addinfo)
{
	return super::OnStepperEvent(stepper, eventtype, addinfo);
}
