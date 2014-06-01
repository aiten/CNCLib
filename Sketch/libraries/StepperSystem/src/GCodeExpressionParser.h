#pragma once

////////////////////////////////////////////////////////

#include "ExpressionParser.h"
#include "GCodeParser.h"

////////////////////////////////////////////////////////
//
class CGCodeExpressionParser : public CExpressionParser
{
private:

	typedef CExpressionParser super;

public:

	CGCodeExpressionParser(CGCodeParser* parser) : super(parser->GetReader())	{ _gcodeparser = parser; _LeftParenthesis = '['; _RightParenthesis = ']'; };

protected:

	CGCodeParser* _gcodeparser;

	virtual void ScannNextToken();
	virtual void ReadIdent();
	virtual bool IsIdentStart(char ch)		{ return ch == '#' || super::IsIdentStart(ch); }	// start of function or variable
	virtual bool EvalVariable(const char* var_name, expr_t& answer);
};

////////////////////////////////////////////////////////