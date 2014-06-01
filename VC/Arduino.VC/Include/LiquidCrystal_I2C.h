#pragma once

#define POSITIVE 1

class LiquidCrystal_I2C
{
public:
	LiquidCrystal_I2C(int, int, int, int, int, int, int, int) {};

	void setBacklightPin(int, int){};
	void setBacklight(int){};
	void backlight(){};
	void clear(){};
	void begin(int, int){};
	void setCursor(int, int) {};
	void print(const char*) {};
	void print(unsigned char) {};
};
