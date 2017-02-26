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
	_havePen = false;
}

////////////////////////////////////////////////////////////

void CPlotter::Init()
{
  _servo1.attach(SERVO1_PIN);
}

////////////////////////////////////////////////////////////

void CPlotter::Initialized()
{
	_servo1.write(SERVO1_CLAMPOPEN);
	PenUpNow();
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
		PenUpNow();
	}
}

////////////////////////////////////////////////////////////

void CPlotter::PenUpNow()
{
	CStepper::GetInstance()->Wait(1);
	_isPenDown = false;
	CMotionControlBase::GetInstance()->MoveAbsEx(MOVEPENUP_FEEDRATE, Z_AXIS, PLOTTER_PENUPPOS, -1);
#ifdef MYUSE_LCD
	// Lcd.DrawRequest(true,CLcd::DrawAll); => delay off movementbuffer
#endif
}


////////////////////////////////////////////////////////////

void CPlotter::PenDown()
{
	_isDelayPen = false;
	if (!_isPenDown)
	{
		_isPenDown = true;
		CMotionControlBase::GetInstance()->MoveAbsEx(MOVEPENDOWN_FEEDRATE, Z_AXIS, PLOTTER_PENDOWNPOS, -1);
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

////////////////////////////////////////////////////////////

void CPlotter::ToPenChangePos(uint8_t pen)
{
	mm1000_t ofs_y = pen*PLOTTER_PENCHANGEPOS_Y_OFS;
	CMotionControlBase::GetInstance()->MoveAbsEx(
									CHPGLParser::_state.FeedRateUp,
									X_AXIS, PLOTTER_PENCHANGEPOS_X,
									Y_AXIS, PLOTTER_PENCHANGEPOS_Y + ofs_y,
									-1);

	CMotionControlBase::GetInstance()->MoveAbsEx(MOVEPENCHANGE_FEEDRATE, Z_AXIS, PLOTTER_PENCHANGEPOS, -1);
	CStepper::GetInstance()->WaitBusy();
}

////////////////////////////////////////////////////////////

void CPlotter::OffPenChangePos(uint8_t pen)
{
	CMotionControlBase::GetInstance()->MoveAbsEx(MOVEPENCHANGE_FEEDRATE, Z_AXIS, PLOTTER_PENUPPOS, -1);
	CStepper::GetInstance()->WaitBusy();
}


////////////////////////////////////////////////////////////

bool CPlotter::SetPen(uint8_t pen) 
{ 
	if (_pen == pen && _havePen)
		return true;

	if (!PenToDepot())
		return false;

	return PenFromDepot(pen);
}

////////////////////////////////////////////////////////////

bool CPlotter::PenToDepot()
{
	if (!_havePen)
		return true;
	
	PenUp();
	CStepper::GetInstance()->WaitBusy();

	/////////////////////////////////////
	// TODO: 

	ToPenChangePos(_pen);

	_servo1.write(SERVO1_CLAMPOPEN);
	delay(SERVO1_CLAMPOPENDELAY);

	OffPenChangePos(_pen);

	////////////////////////////////////

	_havePen = false;
	return true;
}

////////////////////////////////////////////////////////////

bool CPlotter::PenFromDepot(uint8_t pen)
{

	/////////////////////////////////////
	// TODO: 

	ToPenChangePos(pen);

	_servo1.write(SERVO1_CLAMPCLOSE);
	delay(SERVO1_CLAMPCLOSEDELAY);

	OffPenChangePos(pen);

	////////////////////////////////////

	_pen = pen;
	_havePen = true;
	return true;
}









