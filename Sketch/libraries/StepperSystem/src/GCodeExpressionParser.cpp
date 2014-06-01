#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

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
