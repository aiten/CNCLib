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
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <StepperLib.h>
#include <CNCLib.h>
#include "MyControl.h"
#include "HPGLParser.h"
#include "PlotterControl.h"

CMotionControl MotionControl;

////////////////////////////////////////////////////////////

void CMyControl::Init()
{
  CMotionControlBase::GetInstance()->Init();
  CMotionControlBase::GetInstance()->InitConversion(ConversionToMm1000, ConversionToMachine);

#ifdef __USE_LCD__
	Lcd.Init();
#endif

	StepperSerial.println(F("Plotter(HA) is starting ... (" __DATE__ ", " __TIME__ ")"));

	super::Init();

	CHPGLParser::Init();

	CStepper::GetInstance()->SetLimitMax(X_AXIS, CMotionControlBase::GetInstance()->ToMachine(X_AXIS, X_MAXSIZE));
	CStepper::GetInstance()->SetLimitMax(Y_AXIS, CMotionControlBase::GetInstance()->ToMachine(Y_AXIS, Y_MAXSIZE));
	CStepper::GetInstance()->SetLimitMax(Z_AXIS, CMotionControlBase::GetInstance()->ToMachine(Z_AXIS, Z_MAXSIZE));

	CStepper::GetInstance()->SetJerkSpeed(0, 1000);  // 500 * 8?
	CStepper::GetInstance()->SetJerkSpeed(1, 2000);
	CStepper::GetInstance()->SetJerkSpeed(2, 1000);

	CStepper::GetInstance()->SetDefaultMaxSpeed(MAXSTEPRATE, 400, 450);
  
	_controllerfan.Init(255);
	_kill.Init();

#if defined(__AVR_ARCH__) || defined(__SAM3X8E__)
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(X_AXIS, true), true);
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Y_AXIS, true), true);
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Z_AXIS, true), true);
	CStepper::GetInstance()->UseReference(EMERGENCY_ENDSTOP, true);    // not stop
#endif    
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference()
{
	GoToReference(Z_AXIS, 0, true);
	GoToReference(Y_AXIS, 0, true);
	GoToReference(X_AXIS, 0, true);
}

////////////////////////////////////////////////////////////

bool CMyControl::GoToReference(axis_t axis, steprate_t /* steprate */, bool toMinRef)
{
	return super::GoToReference(axis, FEEDRATE_REFMOVE, toMinRef);
}

////////////////////////////////////////////////////////////

bool CMyControl::IsKill()
{
	return _kill.IsOn();
}

////////////////////////////////////////////////////////////

void CMyControl::IOControl(uint8_t tool, unsigned short level)
{
	switch (tool)
	{
#ifdef CONTROLLERFAN_FAN_PIN
		case ControllerFan:		_controllerfan.Level = (uint8_t)level;		return;
#endif
	}

	super::IOControl(tool, level);
}

////////////////////////////////////////////////////////////

unsigned short CMyControl::IOControl(uint8_t tool)
{
	switch (tool)
	{
#ifdef CONTROLLERFAN_FAN_PIN
		case ControllerFan: { return _controllerfan.Level; }
#endif
	}

	return super::IOControl(tool);
}

////////////////////////////////////////////////////////////

bool CMyControl::OnStepperEvent(CStepper*stepper, EnumAsByte(CStepper::EStepperEvent) eventtype, void* addinfo)
{
#ifdef CONTROLLERFAN_FAN_PIN
	switch (eventtype)
	{
		case CStepper::OnStartEvent:
			_controllerfan.On();
			break;
		case CStepper::OnIdleEvent:
			if (millis() - stepper->IdleTime() > CONTROLLERFAN_ONTIME)
			{
				_controllerfan.Off();
			}
			break;
	}
#endif

	return super::OnStepperEvent(stepper, eventtype, addinfo);
}

////////////////////////////////////////////////////////////

void CMyControl::Idle(unsigned int idletime)
{
	Plotter.Idle(idletime);
	super::Idle(idletime);
}

////////////////////////////////////////////////////////////

bool CMyControl::Parse(CStreamReader* reader, Stream* output)
{
	CHPGLParser hpgl(reader, output);
	return ParseAndPrintResult(&hpgl, output);
}

////////////////////////////////////////////////////////////
