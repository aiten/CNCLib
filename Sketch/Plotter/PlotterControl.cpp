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
	if (!CStepper::GetInstance()->IsBusy() && idletime > CHPGLParser::_state.penUpTimeOut)
	{
		if (_isPenDown)
		{
			PenUp();
			_isPenDownTimeout = true;
		}
	}
}

////////////////////////////////////////////////////////////

void CPlotter::Resume(bool /* resetpen */)
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
		CStepper::GetInstance()->SetDefaultMaxSpeed(CHPGLParser::_state.movePenUp.max);
		CStepper::GetInstance()->SetAcc(Z_AXIS, CHPGLParser::_state.movePenUp.acc); CStepper::GetInstance()->SetDec(Z_AXIS, CHPGLParser::_state.movePenUp.dec);
		_isPenDown = false;
		CStepper::GetInstance()->MoveAbs(Z_AXIS, CHPGLParser::_state.penUpPos);
		CStepper::GetInstance()->WaitBusy();
		Lcd.DrawRequest(true,CLcd::DrawAll);

		CStepper::GetInstance()->SetDefaultMaxSpeed(CHPGLParser::_state.penUp.max);
		CStepper::GetInstance()->SetAcc(X_AXIS, CHPGLParser::_state.penUp.acc); CStepper::GetInstance()->SetDec(X_AXIS, CHPGLParser::_state.penUp.dec);
		CStepper::GetInstance()->SetAcc(Y_AXIS, CHPGLParser::_state.penUp.acc); CStepper::GetInstance()->SetDec(Y_AXIS, CHPGLParser::_state.penUp.dec);
	}
}

////////////////////////////////////////////////////////////

void CPlotter::PenDown()
{
	_isDelayPen = false;
	if (!_isPenDown)
	{
		CStepper::GetInstance()->WaitBusy();
		CStepper::GetInstance()->SetDefaultMaxSpeed(CHPGLParser::_state.movePenDown.max);
		CStepper::GetInstance()->SetAcc(Z_AXIS, CHPGLParser::_state.movePenDown.acc); CStepper::GetInstance()->SetDec(Z_AXIS, CHPGLParser::_state.movePenDown.dec);
		_isPenDown = true;
		CStepper::GetInstance()->MoveAbs(Z_AXIS, CHPGLParser::_state.penDownPos);
		CStepper::GetInstance()->WaitBusy();
		Lcd.DrawRequest(true,CLcd::DrawAll);

		CStepper::GetInstance()->SetDefaultMaxSpeed(CHPGLParser::_state.penDown.max);
		CStepper::GetInstance()->SetAcc(X_AXIS, CHPGLParser::_state.penDown.acc); CStepper::GetInstance()->SetDec(X_AXIS, CHPGLParser::_state.penDown.dec);
		CStepper::GetInstance()->SetAcc(Y_AXIS, CHPGLParser::_state.penDown.acc); CStepper::GetInstance()->SetDec(Y_AXIS, CHPGLParser::_state.penDown.dec);
	}
}

////////////////////////////////////////////////////////////

void CPlotter::DelayPen()
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
