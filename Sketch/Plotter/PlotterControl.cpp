#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <StepperSystem.h>
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
		CStepper::GetInstance()->WaitBusy();
		CStepper::GetInstance()->SetDefaultMaxSpeed(CHPGLParser::_state.movePenUp.max,Z_AXIS, CHPGLParser::_state.movePenUp.acc, CHPGLParser::_state.movePenUp.dec);
		_isPenDown = false;
		CStepper::GetInstance()->MoveAbs(Z_AXIS, CHPGLParser::_state.penUpPos);
		CStepper::GetInstance()->WaitBusy();
#ifdef __USE_LCD__
		Lcd.DrawRequest(true,CLcd::DrawAll);
#endif
		CStepper::GetInstance()->SetDefaultMaxSpeed(CHPGLParser::_state.penUp.max,X_AXIS, CHPGLParser::_state.penUp.acc, CHPGLParser::_state.penUp.dec);
		CStepper::GetInstance()->SetAccDec(Y_AXIS, CHPGLParser::_state.penUp.acc, CHPGLParser::_state.penUp.dec);
	}
}

////////////////////////////////////////////////////////////

void CPlotter::PenDown()
{
	_isDelayPen = false;
	if (!_isPenDown)
	{
		CStepper::GetInstance()->WaitBusy();
		CStepper::GetInstance()->SetDefaultMaxSpeed(CHPGLParser::_state.movePenDown.max,Z_AXIS, CHPGLParser::_state.movePenDown.acc, CHPGLParser::_state.movePenDown.dec);
		_isPenDown = true;
		CStepper::GetInstance()->MoveAbs(Z_AXIS, CHPGLParser::_state.penDownPos);
		CStepper::GetInstance()->WaitBusy();
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
