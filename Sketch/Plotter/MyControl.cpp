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
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <StepperLib.h>
#include <CNCLib.h>
#include "MyControl.h"
#include "HPGLParser.h"
#include "PlotterControl.h"

////////////////////////////////////////////////////////////

void CMyControl::Init()
{
	super::Init();

	StepperSerial.println(F("Plotter(HA) is starting ... (" __DATE__ ", " __TIME__ ")"));

#ifdef __USE_LCD__
	Lcd.Init();
#endif

	CHPGLParser::Init();

	CStepper::GetInstance()->SetLimitMax(0, CHPGLParser::HPGLToPlotterCordY(20800));
	CStepper::GetInstance()->SetLimitMax(1, CHPGLParser::HPGLToPlotterCordY(11800));
	CStepper::GetInstance()->SetLimitMax(2, 8000);   // 100*8

	CStepper::GetInstance()->SetJerkSpeed(0, 1000);  // 500 * 8?
	CStepper::GetInstance()->SetJerkSpeed(1, 2000);
	CStepper::GetInstance()->SetJerkSpeed(2, 1000);

	CStepper::GetInstance()->SetDefaultMaxSpeed(CHPGLParser::_state.penUp.max, CHPGLParser::_state.penUp.acc, CHPGLParser::_state.penUp.dec);

	_controllerfan.Init(255);
	_kill.Init();

#if defined(__AVR_ARCH__) || defined(__SAM3X8E__)
	for (register unsigned char i=0;i<NUM_AXIS*2;i++)
	{
		CStepper::GetInstance()->UseReference(i,false);  
	}
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(X_AXIS, true),true);  
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Y_AXIS, true),true);  
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Z_AXIS, true),true);  
	CStepper::GetInstance()->UseReference(EMERGENCY_ENDSTOP,true);    // not stop
#endif    
}

////////////////////////////////////////////////////////////

void CMyControl::Initialized()
{
	super::Initialized();

//	CStepper::GetInstance()->Wait(1);
//	CStepper::GetInstance()->MoveAbs(Z_AXIS, 200);

	GoToReference();
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference()
{
	super::GoToReference();

	GoToReference(Z_AXIS, 0);
	GoToReference(Y_AXIS, 0);
	GoToReference(X_AXIS, 0);
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference(axis_t axis, steprate_t /* steprate */)
{
#define FEEDRATE_REFMOVE  CStepper::GetInstance()->GetDefaultVmax() / 4  
        bool toMin = true; // axis != Z_AXIS;
        CStepper::GetInstance()->MoveReference(axis, CStepper::GetInstance()->ToReferenceId(axis, toMin), toMin, FEEDRATE_REFMOVE);
}

////////////////////////////////////////////////////////////

bool CMyControl::IsKill()
{
	return _kill.IsOn();
}

////////////////////////////////////////////////////////////

void CMyControl::IOControl(unsigned char tool, unsigned short level)
{
	switch (tool)
	{
#if CONTROLLERFAN_FAN_PIN != -1
		case ControllerFan:		_controllerfan.Level = (unsigned char)level;		return;
#endif
	}
	
	super::IOControl(tool, level);
}

////////////////////////////////////////////////////////////

unsigned short CMyControl::IOControl(unsigned char tool)
{
	switch (tool)
	{
		case ControllerFan:	{ return _controllerfan.Level; }
	}

	return super::IOControl(tool);
}

////////////////////////////////////////////////////////////

bool CMyControl::OnStepperEvent(CStepper*stepper, EnumAsByte(CStepper::EStepperEvent) eventtype, void* addinfo)
{
#if CONTROLLERFAN_FAN_PIN != -1
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
	CHPGLParser hpgl(reader,output);
	return ParseAndPrintResult(&hpgl,output);
}

////////////////////////////////////////////////////////////
