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

#define POSITIVE 1

class LiquidCrystal_I2C
{
public:
	LiquidCrystal_I2C(int, int, int, int, int, int, int, int) {};
	LiquidCrystal_I2C(int, int, int, int, int, int, int, int, int, int) {};

	void setBacklightPin(int, int){};
	void setBacklight(int){};
	void backlight(){};
	void clear(){};
	void begin(int, int){};
	void setCursor(int, int) {};
	void print(const char*) {};
	void print(char) {};
	void print(uint8_t) {};
	void print(unsigned int) {};
	void print(float) {};
	void print(short) {};
	void print(unsigned long) {};
	void home() {};
};
