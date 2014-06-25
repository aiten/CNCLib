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
