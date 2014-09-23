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

class U8GLIB_ST7920_128X64_1X // : public Stream
{
public:
	U8GLIB_ST7920_128X64_1X(int, int, int) {};
	void firstPage() {};
	bool nextPage() { return false; }
	void drawStr(int, int, const char*) {}
	void setFont(int) {};
	void setPrintPos(int , int )	{ };

	void print(const char)			{ };

	void print(const char*)			{ };
	void println(const char*)		{ };
};

static int u8g_font_unifont;
static int u8g_font_unifontr;
static int u8g_font_6x12;
static int u8g_font_6x10;
