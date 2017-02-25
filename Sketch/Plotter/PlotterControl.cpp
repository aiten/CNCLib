////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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

void CPlotter::Idle(unsigned int /* idletime */)
{
	if (!CStepper::GetInstance()->IsBusy() && (millis() - CStepper::GetInstance()->IdleTime()) > CHPGLParser::_state._penUpTimeOut)
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
		_isPenDown = false;
		CMotionControlBase::GetInstance()->MoveAbsEx(MOVEPENUP_FEEDRATE, Z_AXIS, CStepper::GetInstance()->GetLimitMin(Z_AXIS), -1);
#ifdef MYUSE_LCD
		// Lcd.DrawRequest(true,CLcd::DrawAll); => delay off movementbuffer
#endif

	}
}

////////////////////////////////////////////////////////////

void CPlotter::PenDown()
{
	_isDelayPen = false;
	if (!_isPenDown)
	{
		_isPenDown = true;
		CMotionControlBase::GetInstance()->MoveAbsEx(MOVEPENDOWN_FEEDRATE, Z_AXIS, CStepper::GetInstance()->GetLimitMax(Z_AXIS), -1);
		CStepper::GetInstance()->Wait(1);
#ifdef MYUSE_LCD
		// Lcd.DrawRequest(true,CLcd::DrawAll); => delay off movementbuffer
		Lcd.DrawRequest(CLcd::DrawForceAll);
#endif
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
