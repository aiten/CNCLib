////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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

#pragma once

////////////////////////////////////////////////////////

#include "Parser.h"

#define EXPRPARSER_MAXTOKENLENGTH 16

////////////////////////////////////////////////////////
//
// Expression Parser, 
//	read char from CStreamReader and calculate the result(=Answer)
//
class CExpressionParser : public CParser
{
private:

	typedef CParser super;

public:

	CExpressionParser(CStreamReader* reader, Stream* output) : super(reader,output)	{ _LeftParenthesis = '('; _RightParenthesis = ')'; };

	virtual void Parse() override;

	expr_t			Answer;

protected:

	char _LeftParenthesis;
	char _RightParenthesis;

	void GetNextToken();
	virtual void ScannNextToken();
	virtual void ReadIdent();
	virtual bool IsIdentStart(char ch)									{ return CStreamReader::IsAlpha(ch); }	// start of function or variable
	
	virtual bool EvalVariable(const char* var_name, expr_t& answer);
	virtual void AssignVariable(const char* var_name, expr_t value)		{ var_name; value; };

	enum ETokenType
	{
		UnknownSy,
		NothingSy,
		EndOfLineSy,

		AssignSy,
		LeftParenthesisSy,
		RightParenthesisSy,

		// Operator
		// Level2
		AndSy, OrSy, BitShiftLeftSy, BitShiftRightSy,
		// Level3
		EqualSy, UnEqualSy, LessSy, GreaterSy, LessEqualSy, GreaterEqualSy,
		// Level 4
		PlusSy, MinusSy,
		// Level 5
		MultiplySy, DivideSy, ModuloSy, XOrSy,
		// Level 6
		PowSy,
		// Level 7
		FactorialSy,

		IntegerSy,
		FloatSy,

		VariableSy,

		// Functions
		FirstFunctionSy,
		AbsSy = FirstFunctionSy,
		ExpSy, SignSy, SqrtSy, LogSy, Log10Sy, SinSy, CosSy, TanSy, AsinSy, AcosSy, AtanSy,
		FixSy, FupSy, RoundSy,

		FactorialFncSy, LastFunctionSy = FactorialFncSy
	};

	struct SParserState
	{
		expr_t _number;							// number if parsed integer or float or variable(content)
		
		const char* _varName;

		bool	_variableOK;					// _number = variable with content
		EnumAsByte(ETokenType) _detailtoken;

	} _state;

	EnumAsByte(ETokenType) GetTokenType()			{ return _state._detailtoken; }

	expr_t ParseLevel1();
	expr_t ParseLevel2();
	expr_t ParseLevel3();
	expr_t ParseLevel4();
	expr_t ParseLevel5();
	expr_t ParseLevel6();
	expr_t ParseLevel7();
	expr_t ParseLevel8();
	expr_t ParseLevel9();
	expr_t ParseLevel10();
	expr_t ParseNumber();

	expr_t EvalOperator(EnumAsByte(ETokenType) operatorSy, const expr_t &lhs, const expr_t &rhs);
	expr_t EvalFunction(EnumAsByte(ETokenType) operatorSy, const expr_t &value);

	expr_t Factorial(expr_t value);
	expr_t Sign(expr_t value);

	bool SaveAssign(char* buffer, char* current, char ch, unsigned char max);
};

////////////////////////////////////////////////////////