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

#include "ConfigurationCNCLib.h"
#include "ExpressionParser.h"

////////////////////////////////////////////////////////////
//
// Expression Parser

void CExpressionParser::Parse()
{
	Answer = 0;

	GetNextToken();
	if (GetTokenType() == EndOfLineSy)
	{
		ErrorAdd(MESSAGE_EXPR_EMPTY_EXPR);
		return;
	}

	Answer = ParseLevel1();

	if (IsError()) return;

	// check for garbage at the end of the expression
	// an expression ends with a character '\0' and GetMainTokenType() = delimeter
	if (GetTokenType() != EndOfLineSy)
	{
		ErrorAdd(MESSAGE_EXPR_FORMAT);
		return;
	}
}

void CExpressionParser::GetNextToken()
{
	_state._detailtoken = NothingSy;
	if (IsError())	return;

	char ch = _reader->SkipSpaces();

	if (ch == '\0')
	{
		_state._detailtoken = EndOfLineSy;
		return;
	}

	ScannNextToken();
}

////////////////////////////////////////////////////////////
// Get next token in the current string expr.

void CExpressionParser::ScannNextToken()
{
	char ch = _reader->GetChar();
	if (IsToken(F("||"), false, false))	{ _state._detailtoken = XOrSy;			 return; }
	if (IsToken(F("<<"), false, false))	{ _state._detailtoken = BitShiftLeftSy;  return; }
	if (IsToken(F(">>"), false, false))	{ _state._detailtoken = BitShiftRightSy; return; }
	if (IsToken(F("=="), false, false))	{ _state._detailtoken = EqualSy;		 return; }
	if (IsToken(F("!="), false, false))	{ _state._detailtoken = UnEqualSy;		 return; }
	if (IsToken(F(">="), false, false))	{ _state._detailtoken = GreaterEqualSy;  return; }
	if (IsToken(F("<="), false, false))	{ _state._detailtoken = LessEqualSy;	 return; }

	if (ch == _LeftParenthesis)			{ _state._detailtoken = LeftParenthesisSy;	 _reader->GetNextChar(); return; }
	if (ch == _RightParenthesis)		{ _state._detailtoken = RightParenthesisSy;	 _reader->GetNextChar(); return; }

	switch (ch)
	{
		case '>': _state._detailtoken = GreaterSy;		_reader->GetNextChar(); return;
		case '<': _state._detailtoken = LessSy;			_reader->GetNextChar(); return;
		case '&': _state._detailtoken = AndSy;			_reader->GetNextChar(); return;
		case '|': _state._detailtoken = OrSy;			_reader->GetNextChar(); return;
		case '-': _state._detailtoken = MinusSy;		_reader->GetNextChar(); return;
		case '+': _state._detailtoken = PlusSy;			_reader->GetNextChar(); return;
		case '*': _state._detailtoken = MultiplySy;		_reader->GetNextChar(); return;
		case '/': _state._detailtoken = DivideSy;		_reader->GetNextChar(); return;
		case '%': _state._detailtoken = ModuloSy;		_reader->GetNextChar(); return;
		case '^': _state._detailtoken = PowSy;			_reader->GetNextChar(); return;
		case '!': _state._detailtoken = FactorialSy;	_reader->GetNextChar(); return;
		case '=': _state._detailtoken = AssignSy;		_reader->GetNextChar(); return;
	}

	// check for a value
	if (CStreamReader::IsDigitDot(ch))
	{
		_state._detailtoken = FloatSy;
		_state._number = GetDouble();
		return;
	}

	// check for variables or functions
	if (IsIdentStart(ch))
	{
		char*start = (char*)_reader->GetBuffer();

		ReadIdent();

		char*end = (char*)_reader->GetBuffer();
		ch = _reader->SkipSpaces();

		// temporary terminat buffer with '\00'
		CStreamReader::CSetTemporary terminate(end);

		// check if this is a variable or a function.
		// a function has a parentesis '(' open after the name

		if (ch == _LeftParenthesis)
		{
			if (TryToken(start, F("ABS"), true))		{ _state._detailtoken = AbsSy; return; }
			if (TryToken(start, F("EXP"), true))		{ _state._detailtoken = ExpSy; return; }
			if (TryToken(start, F("SIGN"), true))		{ _state._detailtoken = SignSy; return; }
			if (TryToken(start, F("SQRT"), true))		{ _state._detailtoken = SqrtSy; return; }
			if (TryToken(start, F("LOG"), true))		{ _state._detailtoken = LogSy; return; }
			if (TryToken(start, F("LOG10"), true))		{ _state._detailtoken = Log10Sy; return; }
			if (TryToken(start, F("SIN"), true))		{ _state._detailtoken = SinSy; return; }
			if (TryToken(start, F("COS"), true))		{ _state._detailtoken = CosSy; return; }
			if (TryToken(start, F("TAN"), true))		{ _state._detailtoken = TanSy; return; }
			if (TryToken(start, F("ASIN"), true))		{ _state._detailtoken = AsinSy; return; }
			if (TryToken(start, F("ACOS"), true))		{ _state._detailtoken = AcosSy; return; }
			if (TryToken(start, F("ATAN"), true))		{ _state._detailtoken = AtanSy; return; }
			if (TryToken(start, F("FACTORIAL"), true))	{ _state._detailtoken = FactorialFncSy; return; }

			if (TryToken(start, F("FIX"), true))		{ _state._detailtoken = FixSy; return; }
			if (TryToken(start, F("FUP"), true))		{ _state._detailtoken = FupSy; return; }
			if (TryToken(start, F("ROUND"), true))		{ _state._detailtoken = RoundSy; return; }

			Error(MESSAGE_EXPR_UNKNOWN_FUNCTION);
			return;
		}
		else
		{
			_state._detailtoken = VariableSy;
			_state._variableOK = EvalVariable(start, _state._number);
		}
		return;
	}

	// something unknown is found, wrong characters -> a syntax error
	_state._detailtoken = UnknownSy;
	Error(MESSAGE_EXPR_SYNTAX_ERROR);
	return;
}

////////////////////////////////////////////////////////////

void CExpressionParser::ReadIdent()
{
	char ch = _reader->GetChar();

	while (CStreamReader::IsAlpha(ch) || CStreamReader::IsDigit(ch))
	{
		ch = _reader->GetNextChar();
	}
}

////////////////////////////////////////////////////////////
// assignment of variable or function

expr_t CExpressionParser::ParseLevel1()
{
	if (GetTokenType() == VariableSy)
	{
		// copy current state
		const char* e_now = _reader->GetBuffer();
		SParserState state_now = _state;

		GetNextToken();
		if (GetTokenType() == AssignSy)
		{
			// assignment
			expr_t ans;
			GetNextToken();
			ans = ParseLevel2();

			AssignVariable(state_now._varName, ans);

			return ans;
		}
		else
		{
			if (!_state._variableOK)
			{
				// unknown variable
				ErrorAdd(MESSAGE_EXPR_UNKNOWN_VARIABLE);
				return 0;
			}

			// go back to previous token
			_reader->ResetBuffer(e_now);
			_state = state_now;
		}
	}

	return ParseLevel2();
}

////////////////////////////////////////////////////////////
// conditional operators and bitshift

expr_t CExpressionParser::ParseLevel2()
{
	expr_t ans = ParseLevel3();
	EnumAsByte(ETokenType) operatorSy = GetTokenType();

	while (operatorSy == AndSy || operatorSy == OrSy || operatorSy == BitShiftLeftSy || operatorSy == BitShiftRightSy)
	{
		GetNextToken();
		ans = EvalOperator(operatorSy, ans, ParseLevel3());
		operatorSy = GetTokenType();
	}

	return ans;
}

////////////////////////////////////////////////////////////
// conditional operators

expr_t CExpressionParser::ParseLevel3()
{
	expr_t ans = ParseLevel4();
	EnumAsByte(ETokenType) operatorSy = GetTokenType();

	while (operatorSy == EqualSy || operatorSy == UnEqualSy || operatorSy == LessSy || operatorSy == LessEqualSy || operatorSy == GreaterSy || operatorSy == GreaterEqualSy)
	{
		GetNextToken();
		ans = EvalOperator(operatorSy, ans, ParseLevel4());
		operatorSy = GetTokenType();
	}

	return ans;
}

////////////////////////////////////////////////////////////
// add or subtract

expr_t CExpressionParser::ParseLevel4()
{
	expr_t ans = ParseLevel5();
	EnumAsByte(ETokenType) operatorSy = GetTokenType();

	while (operatorSy == PlusSy || operatorSy == MinusSy)
	{
		GetNextToken();
		ans = EvalOperator(operatorSy, ans, ParseLevel5());
		operatorSy = GetTokenType();
	}

	return ans;
}

////////////////////////////////////////////////////////////
// multiply, divide, modulus, xor

expr_t CExpressionParser::ParseLevel5()
{
	expr_t ans = ParseLevel6();
	EnumAsByte(ETokenType) operatorSy = GetTokenType();

	while (operatorSy == MultiplySy || operatorSy == DivideSy || operatorSy == ModuloSy || operatorSy == XOrSy)
	{
		GetNextToken();
		ans = EvalOperator(operatorSy, ans, ParseLevel6());
		operatorSy = GetTokenType();
	}

	return ans;
}

////////////////////////////////////////////////////////////
// power

expr_t CExpressionParser::ParseLevel6()
{
	expr_t ans = ParseLevel7();
	EnumAsByte(ETokenType) operatorSy = GetTokenType();

	while (operatorSy == PowSy)
	{
		GetNextToken();
		ans = EvalOperator(operatorSy, ans, ParseLevel7());
		operatorSy = GetTokenType();
	}

	return ans;
}

////////////////////////////////////////////////////////////
// Factorial

expr_t CExpressionParser::ParseLevel7()
{
	expr_t ans = ParseLevel8();
	EnumAsByte(ETokenType) operatorSy = GetTokenType();

	while (operatorSy == FactorialSy)
	{
		GetNextToken();
		// factorial does not need a value right from the
		// operator, so zero is filled in.
		ans = EvalOperator(operatorSy, ans, 0.0);
		operatorSy = GetTokenType();
	}

	return ans;
}

////////////////////////////////////////////////////////////
// Unary minus

expr_t CExpressionParser::ParseLevel8()
{
	if (GetTokenType() == MinusSy)
	{
		GetNextToken();
		return -ParseLevel9();
	}

	return ParseLevel9();
}

////////////////////////////////////////////////////////////
// functions

expr_t CExpressionParser::ParseLevel9()
{
	if (GetTokenType() >= FirstFunctionSy && GetTokenType() <= LastFunctionSy)
	{
		EnumAsByte(ETokenType) functionSy = GetTokenType();
		GetNextToken();
		return EvalFunction(functionSy, ParseLevel10());
	}
	return ParseLevel10();
}

////////////////////////////////////////////////////////////
// parenthesized expression or value

expr_t CExpressionParser::ParseLevel10()
{
	// check if it is a parenthesized expression
	if (GetTokenType() == LeftParenthesisSy)
	{
		GetNextToken();
		expr_t ans = ParseLevel2();
		if (GetTokenType() != RightParenthesisSy)
		{
			ErrorAdd(MESSAGE_EXPR_MISSINGRPARENTHESIS);
			return 0;
		}
		GetNextToken();
		return ans;
	}

	// if not parenthesized then the expression is a value
	return ParseNumber();
}

////////////////////////////////////////////////////////////

expr_t CExpressionParser::ParseNumber()
{
	expr_t ans = 0;

	switch (GetTokenType())
	{
		case FloatSy:
		case IntegerSy:
		case VariableSy:
			// this is a number
			ans = _state._number;
			GetNextToken();
			break;
		default:
			// syntax error or unexpected end of expression
			ErrorAdd(MESSAGE_EXPR_SYNTAX_ERROR);
			return 0;
	}

	return ans;
}

////////////////////////////////////////////////////////////
// evaluate an operator for given valuess

expr_t CExpressionParser::EvalOperator(EnumAsByte(ETokenType) operatorSy, const expr_t &lhs, const expr_t &rhs)
{
	switch (operatorSy)
	{
		// level 2
		case AndSy:           return (expr_t)(static_cast<long>(lhs)& static_cast<long>(rhs));
		case OrSy:            return (expr_t)(static_cast<long>(lhs) | static_cast<long>(rhs));
		case BitShiftLeftSy:  return (expr_t)(static_cast<long>(lhs) << static_cast<long>(rhs));
		case BitShiftRightSy: return (expr_t)(static_cast<long>(lhs) >> static_cast<long>(rhs));

			// level 3
		case EqualSy:			return lhs == rhs;
		case UnEqualSy:			return lhs != rhs;
		case LessSy:			return lhs < rhs;
		case GreaterSy:			return lhs > rhs;
		case LessEqualSy:		return lhs <= rhs;
		case GreaterEqualSy:	return lhs >= rhs;

			// level 4
		case PlusSy:			return lhs + rhs;
		case MinusSy:			return lhs - rhs;

			// level 5
		case MultiplySy:		return lhs * rhs;
		case DivideSy:			return lhs / rhs;
		case ModuloSy:			return (expr_t)(static_cast<long>(lhs) % static_cast<long>(rhs));
		case XOrSy:				return (expr_t)(static_cast<long>(lhs) ^ static_cast<long>(rhs));

			// level 6
		case PowSy:				return pow(lhs, rhs);

			// level 7
		case FactorialSy:		return Factorial(lhs);
	}

	ErrorAdd(MESSAGE_EXPR_ILLEGAL_OPERATOR);
	return 0;
}

////////////////////////////////////////////////////////////
// evaluate a function

expr_t CExpressionParser::EvalFunction(EnumAsByte(ETokenType) operatorSy, const expr_t &value)
{
	switch (operatorSy)
	{

		// arithmetic
		case AbsSy:  return abs(value);
		case ExpSy:  return exp(value);
		case SignSy:  return Sign(value);
		case SqrtSy:  return sqrt(value);
		case LogSy:  return log(value);
		case Log10Sy:  return log10(value);

			// trigonometric
		case SinSy:  return sin(value);
		case CosSy:  return cos(value);
		case TanSy:  return tan(value);
		case AsinSy:  return asin(value);
		case AcosSy:  return acos(value);
		case AtanSy:  return atan(value);

			// probability
		case FactorialFncSy:  return Factorial(value);

			// cnc
		case FixSy:  return floor(value);
		case FupSy:  return ceil(value);
		case RoundSy:  return round(value);
	}

	ErrorAdd(MESSAGE_EXPR_ILLEGAL_FUNCTION);
	return 0;
}

////////////////////////////////////////////////////////////

bool CExpressionParser::EvalVariable(const char* var_name, expr_t& answer)
{
	_state._varName = var_name;

	// check for built-in variables
	if (TryToken(var_name, F("E"), true))  { answer = (expr_t) 2.7182818284590452353602874713527; return true; }
	if (TryToken(var_name, F("PI"), true)) { answer = (expr_t) 3.1415926535897932384626433832795; return true; }

	return false;
}

////////////////////////////////////////////////////////////

expr_t CExpressionParser::Factorial(expr_t value)
{
	expr_t res;
	int v = static_cast<int>(value);

	if (value != static_cast<expr_t>(v))
	{
		ErrorAdd(MESSAGE_EXPR_FRACTORIAL);
		return 0;
	}

	res = (expr_t)v;
	v--;
	while (v > 1)
	{
		res *= v;
		v--;
	}

	if (res == 0) res = 1;        // 0! is per definition 1
	return res;
}

////////////////////////////////////////////////////////////

expr_t CExpressionParser::Sign(expr_t value)
{
	if (value > 0) return 1;
	if (value < 0) return -1;
	return 0;
}
