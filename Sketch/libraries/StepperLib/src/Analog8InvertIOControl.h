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

template <pin_t PIN>
class CAnalog8InvertIOControl
{
public:

	void Init(uint8_t level=0)		// init and set default value
	{
		MySetLevel(level);
		_level = level;
	}

	void On(uint8_t level)			// Set level and turn on
	{
		_level = level;
		MySetLevel(level);
	}

	void OnMax()							// turn on at max level
	{
		MySetLevel(255);
	}

	void On()								// turn on at specified level (see Level property)
	{
		MySetLevel(_level);
	}

	void Off()								// turn off, use On() to switch on at same value
	{
		MySetLevel(0);
	}

	bool IsOn() const
	{
		return _lastlevel != 0;
	}

	void SetLevel(uint8_t level)
	{
		_level = level;
	}

	uint8_t GetLevel() const
	{
		return _level;
	}

private:

	uint8_t _level;					// use like a property
	uint8_t _lastlevel;

	void MySetLevel(uint8_t level)
	{
		_lastlevel = level;
		CHAL::analogWrite8(PIN, 255 - level);
	}
};

////////////////////////////////////////////////////////
