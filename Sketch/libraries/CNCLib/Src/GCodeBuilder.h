////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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

#include "CNCLibTypes.h"

////////////////////////////////////////////////////////

#define GCODEBUILDER_MAXCOMANDLENGHT	24

////////////////////////////////////////////////////////

class CGCodeBuilder
{
public:

	CGCodeBuilder()
	{
		InitCommand();
	}

	////////////////////////////////////////////////////////////

	CGCodeBuilder& Add(const __FlashStringHelper* cmd);
	CGCodeBuilder& Add(char* cmd);
	CGCodeBuilder& AddAxisName(axis_t axis);
	CGCodeBuilder& Add(mm1000_t mm1000);
	static char AxisToChar(axis_t axis);
	CGCodeBuilder& InitCommand();

	char* GetCommand()
	{
		return _commandstring;
	}

private:

	char _commandstring[GCODEBUILDER_MAXCOMANDLENGHT];
};

////////////////////////////////////////////////////////
