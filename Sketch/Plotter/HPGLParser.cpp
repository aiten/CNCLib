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

#include <CNCLib.h>
#include <HelpParser.h>

#include "HPGLParser.h"
#include "PlotterControl.h"
#include "MyControl.h"

////////////////////////////////////////////////////////////

enum EPenMove
{
	PD,
	PU,
	PA,
	PR
};

////////////////////////////////////////////////////////////

CHPGLParser::SState CHPGLParser::_state;

////////////////////////////////////////////////////////////

sdist_t CHPGLParser::HPGLToPlotterCordX(sdist_t xx)
{
	return RoundMulDivI32(xx + _state.HPOffsetX, _state.HPMul, _state.HPDiv);
}

////////////////////////////////////////////////////////////

sdist_t CHPGLParser::HPGLToPlotterCordY(sdist_t yy)
{
	return RoundMulDivI32(yy + _state.HPOffsetY, _state.HPMul, _state.HPDiv);
}

////////////////////////////////////////////////////////////

void CHPGLParser::Parse()
{
	if (IsToken(F("SP"), false, false) || IsToken(F("LT"), false, false))	{ IgnoreCommand();		return; }
	if (IsToken(F("IN"), false, false))										{ InitCommand();		return; }
	if (IsToken(F("PD"), false, false))										{ PenMoveCommand(PD);	return; }
	if (IsToken(F("PU"), false, false))										{ PenMoveCommand(PU);	return; }
	if (IsToken(F("PA"), false, false))										{ PenMoveCommand(PA);	return; }
	if (IsToken(F("PR"), false, false))										{ PenMoveCommand(PR);	return; }

	// command escape to "own" extension

	CHelpParser mycommand(_reader);
	mycommand.ParseCommand();

	if (mycommand.IsError()) Error(mycommand.GetError());
	_OkMessage = mycommand.GetOkMessage();
}

////////////////////////////////////////////////////////////

void CHPGLParser::IgnoreCommand()
{
	_reader->MoveToEnd();
}

////////////////////////////////////////////////////////////

void CHPGLParser::InitCommand()
{
	Control.MyInit();
}

////////////////////////////////////////////////////////////

void CHPGLParser::PenMoveCommand(unsigned char cmdidx)
{
	Plotter.Resume();

	switch (cmdidx)
	{
		case PU:	Plotter.DelayPenUp();		break;
		case PD:	Plotter.PenDown();			break;
		case PA:	_state.HPGLIsAbsolut = 1;	break;
		case PR:	_state.HPGLIsAbsolut = 0;	break;
	}

	if (IsToken(F("PD"), false, false))											{ PenMoveCommand(PD);	return; }
	if (IsToken(F("PU"), false, false))											{ PenMoveCommand(PU);	return; }
	if (IsToken(F("PA"), false, false))											{ PenMoveCommand(PA);	return; }
	if (IsToken(F("PR"), false, false))											{ PenMoveCommand(PR);	return; }

	while (IsInt(_reader->GetChar()))
	{
		sdist_t x = GetSDist();

		//all blank or colon

		if (IsInt(_reader->SkipSpaces()))
		{
			// only blank as seperator
		}
		else if (_reader->GetChar() == ',' && IsInt(_reader->GetNextCharSkipScaces()))
		{
			// Colon
		}
		else
		{
		ERROR_MISSINGARGUMENT:
			Error(F("Missing or invalid parameter"));
			return;
		}

		sdist_t y = GetSDist();

		if (_reader->IsError())	goto ERROR_MISSINGARGUMENT;

		x = HPGLToPlotterCordX(x);
		y = HPGLToPlotterCordY(y);

		if (_state.HPGLIsAbsolut)
		{
			if (x != (sdist_t)CStepper::GetInstance()->GetPosition(X_AXIS) || y != (sdist_t)CStepper::GetInstance()->GetPosition(Y_AXIS))
			{
				Plotter.DelayPenNow();
				CStepper::GetInstance()->MoveAbsEx(0, X_AXIS, x, Y_AXIS, y, -1);
			}
		}
		else
		{
			if (x != 0 || y != 0)
			{
				Plotter.DelayPenNow();
				CStepper::GetInstance()->MoveRelEx(0,X_AXIS,x, Y_AXIS, y, -1);
			}
		}
		if (_reader->SkipSpaces() != ',')
			break;

		_reader->GetNextCharSkipScaces();
	}
}

////////////////////////////////////////////////////////////
