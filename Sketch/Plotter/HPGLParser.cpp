#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <StepperSystem.h>
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
	mycommand.Parse();

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
	Plotter.Resume(false);

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
				Plotter.DelayPen();
				CStepper::GetInstance()->MoveAbs3(x, y, CStepper::GetInstance()->GetPosition(Z_AXIS));
			}
		}
		else
		{
			if (x != 0 || y != 0)
			{
				Plotter.DelayPen();
				CStepper::GetInstance()->MoveRel3(x, y, 0);
			}
		}
		if (_reader->SkipSpaces() != ',')
			break;

		_reader->GetNextCharSkipScaces();
	}
}

////////////////////////////////////////////////////////////
