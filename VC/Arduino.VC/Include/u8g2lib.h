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

class U8G2 // : public Stream
{
public:
	void begin() {};
	void firstPage() {};
	bool nextPage() { return false; }
	void drawStr(int, int, const char*) {}
	void setFont(const uint8_t *font) { font; };
	void setCursor(int , int )	{ };

	void print(const char)			{ };

	void print(const char*)			{ };
	void println(const char*)		{ };
};

class U8G2_ST7920_128X64_1_SW_SPI : public U8G2
{
public:
	U8G2_ST7920_128X64_1_SW_SPI(void*, int, int, int) {};
};

static void* U8G2_R0;
static const uint8_t* u8g_font_unifont;
static const uint8_t* u8g_font_unifontr;
static const uint8_t* u8g_font_6x12;
static const uint8_t* u8g_font_6x10;
