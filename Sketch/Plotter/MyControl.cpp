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

	StepperSerial.println(F("Plotter(HA) is starting ... ("__DATE__", "__TIME__")"));

#ifdef __USE_LCD__
	Lcd.Init();
#endif

	CHPGLParser::Init();

	CStepper::GetInstance()->SetLimitMax(0, 55600);  // 6950*8
	CStepper::GetInstance()->SetLimitMax(1, 32000);  // 4000*8
	CStepper::GetInstance()->SetLimitMax(2, 8000);   // 100*8

	CStepper::GetInstance()->SetJerkSpeed(0, 1000);  // 500 * 8?
	CStepper::GetInstance()->SetJerkSpeed(1, 2000);
	CStepper::GetInstance()->SetJerkSpeed(2, 1000);

	CStepper::GetInstance()->SetDefaultMaxSpeed(CHPGLParser::_state.penUp.max, CHPGLParser::_state.penUp.acc, CHPGLParser::_state.penUp.dec);

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

	GoToReference(Z_AXIS);
	GoToReference(Y_AXIS);
	GoToReference(X_AXIS);
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference(axis_t axis)
{
#define FEEDRATE_REFMOVE  CStepper::GetInstance()->GetDefaultVmax() / 4  
        bool toMin = true; // axis != Z_AXIS;
        CStepper::GetInstance()->MoveReference(axis, CStepper::GetInstance()->ToReferenceId(axis, toMin), toMin, FEEDRATE_REFMOVE);
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
