////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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

#include <GCode3DParser.h>

////////////////////////////////////////////////////////
//
// GCode Parser for 3d printer extensions
//
class CMyParser : public CGCode3DParser
{
private:

	typedef CGCode3DParser super;

public:

	CMyParser(CStreamReader* reader,Stream* output) : super(reader,output)		{  }

protected:

	// overrides to exend parser

//	virtual bool InitParse() override;						// begin parsing of a command (override for prechecks)
//	virtual bool GCommand(gcode_t gcode) override;
	virtual bool MCommand(mcode_t mcode) override;
//	virtual bool Command(char ch) override;

private:

	////////////////////////////////////////////////////////

	void M117Command();
	void M118Command();

	static void PrintInfo();

	bool GetAxisAbs(SAxisMove& move);
};

////////////////////////////////////////////////////////

