#pragma once

////////////////////////////////////////////////////////

#include "Configuration.h"
#include "Parser.h"

////////////////////////////////////////////////////////
// Parser for NOT G-Code and HTML 
// use this parser for testing

class CHelpParser : public CParser
{
public:

	CHelpParser(CStreamReader* reader) : CParser(reader){}

protected:

	virtual void Parse();

	bool MoveRel();
	bool MoveRel(axis_t axis);
	bool MoveAbs();
	bool MoveAbs(axis_t axis);
	bool SetPosition(axis_t axis);
	bool MyGoToReference(axis_t axis);
	bool SetSpeed();

	bool CheckEOC();

#ifdef _MSC_VER
public:
	static bool _exit;
#endif

};

////////////////////////////////////////////////////////
