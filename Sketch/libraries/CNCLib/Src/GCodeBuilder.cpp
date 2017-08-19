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
*/
////////////////////////////////////////////////////////

#define _CRT_SECURE_NO_WARNINGS

////////////////////////////////////////////////////////

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include "CNCLib.h"
#include "GCodeBuilder.h"

////////////////////////////////////////////////////////////

CGCodeBuilder& CGCodeBuilder::Add(const __FlashStringHelper* cmd)
{
	strcat_P(_commandstring, (const char*) cmd);
	return *this;
}

////////////////////////////////////////////////////////////

CGCodeBuilder& CGCodeBuilder::Add(char* cmd)
{
	strcat(_commandstring, cmd);
	return *this;
}

////////////////////////////////////////////////////////////

CGCodeBuilder& CGCodeBuilder::Add(mm1000_t mm1000)
{
	char tmp[16];
	Add(CMm1000::ToString(mm1000, tmp, 3));
	return *this;
}

////////////////////////////////////////////////////////////

CGCodeBuilder& CGCodeBuilder::AddAxisName(axis_t axis)
{
	uint8_t len = (uint8_t)strlen(_commandstring);
	_commandstring[len] = AxisToChar(axis);
	_commandstring[len + 1] = 0;
	return *this;
}

////////////////////////////////////////////////////////////

char CGCodeBuilder::AxisToChar(axis_t axis)
{
	return pgm_read_byte(PSTR("XYZABCUVW") + axis);
}

////////////////////////////////////////////////////////////

CGCodeBuilder& CGCodeBuilder::InitCommand()
{
	_commandstring[0] = 0;
	return *this;
}
