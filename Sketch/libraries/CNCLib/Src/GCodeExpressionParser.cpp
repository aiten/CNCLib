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
*/
////////////////////////////////////////////////////////

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include "CNCLib.h"
#include "MotionControl.h"

#include "GCodeExpressionParser.h"

////////////////////////////////////////////////////////////
//
// Expression Parser

void CGCodeExpressionParser::ReadIdent()
{
	// read variable name of gcode : #1 or #<_x>

	char ch = _reader->GetChar();
	if (ch == '#')
	{
		// start of GCODE variable => format #1 or #<_x>
		_reader->GetNextChar();
		_state._number = _gcodeparser->ParseParamNo();

		if (_gcodeparser->IsError())
		{
			Error(_gcodeparser->GetError());
			return;
		}
	}
	else
	{
		super::ReadIdent();
	}
}

////////////////////////////////////////////////////////////

void CGCodeExpressionParser::ScannNextToken()
{
	char ch = _reader->GetChar();
	while (ch)
	{
		if (ch == ';' || ch == '(')		// comment
		{
			ch = _gcodeparser->SkipSpacesOrComment();
		}
		else
		{
			break;
		}
	}
	if (ch == '\0')
	{
		_state._detailtoken = EndOfLineSy;
		return;
	}
	super::ScannNextToken();
}

////////////////////////////////////////////////////////////

bool CGCodeExpressionParser::EvalVariable(const char* var_name, expr_t& answer)
{
	if (var_name[0] == '#')
	{
		// assigned in ReadIdent
		answer = CMotionControl::ToDouble(_gcodeparser->GetParamValue((param_t) _state._number));
		return true;
	}
	return super::EvalVariable(var_name, answer);
}
