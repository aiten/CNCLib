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
#include "PlotterControl.h"
#include "MyControl.h"
#include "MyLcd.h"
#include "HPGLParser.h"

////////////////////////////////////////////////////////////

CPlotter::CPlotter()
{
	_isPenDown = false;
	_isPenDownTimeout = false;
	_isDelayPen = false;
	_pen = 0;
}

////////////////////////////////////////////////////////////

void CPlotter::Idle(unsigned int idletime)
{
	if (!CStepper::GetInstance()->IsBusy() && (millis() - CStepper::GetInstance()->IdleTime()) > CHPGLParser::_state.penUpTimeOut)
	{
		if (_isPenDown)
		{
			PenUp();
			_isPenDownTimeout = true;
		}
	}
}

////////////////////////////////////////////////////////////

void CPlotter::Resume()
{
	if (_isPenDownTimeout)
	{
		PenDown();
	}
	_isPenDownTimeout = false;
}

////////////////////////////////////////////////////////////

void CPlotter::PenUp()
{
	_isDelayPen = false;
	if (_isPenDown)
	{
		CStepper::GetInstance()->Wait(1);
		CStepper::GetInstance()->SetDefaultMaxSpeed(CHPGLParser::_state.movePenUp.max,Z_AXIS, CHPGLParser::_state.movePenUp.acc, CHPGLParser::_state.movePenUp.dec);
		_isPenDown = false;
		CStepper::GetInstance()->MoveAbs(Z_AXIS, CHPGLParser::_state.penUpPos);
		CStepper::GetInstance()->Wait(1);
#ifdef __USE_LCD__
		Lcd.DrawRequest(true,CLcd::DrawAll);
#endif
		CStepper::GetInstance()->SetDefaultMaxSpeed(CHPGLParser::_state.penUp.max,X_AXIS, CHPGLParser::_state.penUp.acc, CHPGLParser::_state.penUp.dec);
		CStepper::GetInstance()->SetAccDec(Y_AXIS, CHPGLParser::_state.penUp.acc, CHPGLParser::_state.penUp.dec);

CStepper::GetInstance()->SetAccDec(X_AXIS, 200, 150);

	}
}

////////////////////////////////////////////////////////////

void CPlotter::PenDown()
{
	_isDelayPen = false;
	if (!_isPenDown)
	{
		CStepper::GetInstance()->Wait(1);
		CStepper::GetInstance()->SetDefaultMaxSpeed(CHPGLParser::_state.movePenDown.max,Z_AXIS, CHPGLParser::_state.movePenDown.acc, CHPGLParser::_state.movePenDown.dec);
		_isPenDown = true;
		CStepper::GetInstance()->MoveAbs(Z_AXIS, CHPGLParser::_state.penDownPos);
		CStepper::GetInstance()->Wait(1);
#ifdef __USE_LCD__
		Lcd.DrawRequest(true,CLcd::DrawAll);
#endif
		CStepper::GetInstance()->SetDefaultMaxSpeed(CHPGLParser::_state.penDown.max,X_AXIS, CHPGLParser::_state.penDown.acc, CHPGLParser::_state.penDown.dec);
		CStepper::GetInstance()->SetAccDec(Y_AXIS, CHPGLParser::_state.penDown.acc, CHPGLParser::_state.penDown.dec);
	}
}

////////////////////////////////////////////////////////////

void CPlotter::DelayPenNow()
{
	if (_isDelayPen)
	{
		_isDelayPen = false;
		if (_isDelayPenDown)
			PenDown();
		else
			PenUp();
	}
}
