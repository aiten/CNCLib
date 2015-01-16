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
  http://www.gnu.org/licenses/
*/
////////////////////////////////////////////////////////

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include "GCodeTools.h"

////////////////////////////////////////////////////////

template<> CGCodeTools* CSingleton<CGCodeTools>::_instance=NULL;

////////////////////////////////////////////////////////


const CGCodeTools::STools CGCodeTools::_tools[] PROGMEM =
{
	{ 1,  EndMill,  100, 20000 },
	{ 2,  EndMill,  200, 20000 },
	{ 3,  EndMill,  300, 20000 },
	{ 4,  EndMill,  400, 20000 },
	{ 5,  EndMill,  500, 20000 },
	{ 6,  EndMill,  600, 20000 },
	{ 7,  EndMill,  700, 20000 },
	{ 8,  EndMill,  800, 20000 },
	{ 9,  EndMill,  900, 20000 },
	{ 10, EndMill, 1000, 20000 },
	{ 11, EndMill, 1100, 20000 },
	{ 12, EndMill, 1200, 20000 },
	{ 13, EndMill, 1300, 20000 },
	{ 14, EndMill, 1400, 20000 },
	{ 15, EndMill, 1500, 20000 },
	{ 16, EndMill, 1600, 20000 },
	{ 17, EndMill, 1700, 20000 },
	{ 18, EndMill, 1800, 20000 },
	{ 19, EndMill, 1900, 20000 },
	{ 20, EndMill, 2000, 20000 },
	{ 21, EndMill, 2100, 20000 },
	{ 22, EndMill, 2200, 20000 },
	{ 23, EndMill, 2300, 20000 },
	{ 24, EndMill, 2400, 20000 },
	{ 25, EndMill, 2500, 20000 },
	{ 26, EndMill, 2600, 20000 },
	{ 27, EndMill, 2700, 20000 },
	{ 28, EndMill, 2800, 20000 },
	{ 29, EndMill, 2900, 20000 },
	{ 30, EndMill, 3000, 20000 },
	{ 0xffff }
};

////////////////////////////////////////////////////////

unsigned char CGCodeTools::GetToolIndex(toolnr_t tool)
{
	for (unsigned char i = 0;; i++)
	{
		toolnr_t t = pgm_read_word(&_tools[i].ToolNr);
		if (t == 0xffff) break;

		if (t == tool) return i;
	}

	return NOTOOLINDEX;
}

////////////////////////////////////////////////////////

mm1000_t CGCodeTools::GetRadius(toolnr_t tool)
{
	unsigned char idx = GetToolIndex(tool);
	if (idx == NOTOOLINDEX) return 0;

	return pgm_read_dword(&_tools[idx].Radius);
}

////////////////////////////////////////////////////////

mm1000_t CGCodeTools::GetHeight(toolnr_t tool)
{
	unsigned char idx = GetToolIndex(tool);
	if (idx == NOTOOLINDEX) return 0;

	return pgm_read_dword(&_tools[idx].Height);
}
