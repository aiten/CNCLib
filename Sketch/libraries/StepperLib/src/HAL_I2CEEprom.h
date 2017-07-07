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

#include <Wire.h>

////////////////////////////////////////////////////////

class CHAL_I2C_EEprom24C256
{
public:

	static void Init()
	{
		Wire.begin();
	}

	#define I2C24C256ADR 0x50

	static byte i2c_eeprom_read_byte(unsigned int eeaddress)
	{
		return i2c_eeprom_read_byte(I2C24C256ADR, eeaddress);
	}

	static uint32_t i2c_eeprom_read_dword(unsigned int eeaddress)
	{
		uint32_t data;
		i2c_eeprom_read_buffer(I2C24C256ADR, eeaddress, (byte*) &data, sizeof(data));
		return data;
	}

	static void i2c_eeprom_write_dword(unsigned int eeaddress, uint32_t value)
	{
		i2c_eeprom_write_page(I2C24C256ADR, eeaddress, (byte*)&value, sizeof(value));
	}

	////////////////////////////////
	// see: http://playground.arduino.cc/Code/I2CEEPROM

	static void i2c_eeprom_write_byte(int deviceaddress, unsigned int eeaddress, byte data) 
	{
		int rdata = data;
		Wire.beginTransmission(deviceaddress);
		Wire.write((int)(eeaddress >> 8)); // MSB
		Wire.write((int)(eeaddress & 0xFF)); // LSB
		Wire.write(rdata);
		Wire.endTransmission();
	}

	// WARNING: address is a page address, 6-bit end will wrap around
	// also, data can be maximum of about 30 bytes, because the Wire library has a buffer of 32 bytes
	static void i2c_eeprom_write_page(int deviceaddress, unsigned int eeaddresspage, byte* data, byte length)
	{
		Wire.beginTransmission(deviceaddress);
		Wire.write((int)(eeaddresspage >> 8)); // MSB
		Wire.write((int)(eeaddresspage & 0xFF)); // LSB
		byte c;
		for (c = 0; c < length; c++)
			Wire.write(data[c]);
		Wire.endTransmission();
	}

	static byte i2c_eeprom_read_byte(int deviceaddress, unsigned int eeaddress)
	{
		byte rdata = 0xFF;
		Wire.beginTransmission(deviceaddress);
		Wire.write((int)(eeaddress >> 8)); // MSB
		Wire.write((int)(eeaddress & 0xFF)); // LSB
		Wire.endTransmission();
		Wire.requestFrom(deviceaddress, 1);
		if (Wire.available()) rdata = Wire.read();
		return rdata;
	}

	// maybe let's not read more than 30 or 32 bytes at a time!
	static void i2c_eeprom_read_buffer(int deviceaddress, unsigned int eeaddress, byte *buffer, int length)
	{
		Wire.beginTransmission(deviceaddress);
		Wire.write((int)(eeaddress >> 8)); // MSB
		Wire.write((int)(eeaddress & 0xFF)); // LSB
		Wire.endTransmission();
		Wire.requestFrom(deviceaddress, length);
		int c = 0;
		for (c = 0; c < length; c++)
			if (Wire.available()) buffer[c] = Wire.read();
	}
};