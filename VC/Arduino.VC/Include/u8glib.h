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

typedef  unsigned char u8g_fntpgm_uint8_t;

class U8GLIB // : public Stream
{
public:
	void firstPage() {};
	bool nextPage() { return false; }
	void drawStr(int, int, const char*) {}
	void setFont(const u8g_fntpgm_uint8_t *font) { font; };
	void setPrintPos(int , int )	{ };

	void print(const char)			{ };

	void print(const char*)			{ };
	void println(const char*)		{ };
};

class U8GLIB_ST7920_128X64_1X : public U8GLIB
{
public:
	U8GLIB_ST7920_128X64_1X(int, int, int) {};
};

static const u8g_fntpgm_uint8_t* u8g_font_unifont;
static const u8g_fntpgm_uint8_t* u8g_font_unifontr;
static const u8g_fntpgm_uint8_t* u8g_font_6x12;
static const u8g_fntpgm_uint8_t* u8g_font_6x10;
