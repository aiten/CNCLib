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

#pragma once

////////////////////////////////////////////////////////

template <unsigned char PIN>
class CAnalog8InvertIOControl
{
public:

	unsigned char Level;					// use like a property

	void Init(unsigned char level=0)		// init and set default value
	{
		SetLevel(level);
		Level = level;
	}

	void On(unsigned char level)			// Set level and turn on
	{
		Level = level;
		SetLevel(level);
	}

	void OnMax()							// turn on at max level
	{
		OnLevel(255);
	}

	void On()								// turn on at specified level (see Level property)
	{
		SetLevel(Level);
	}

	void Off()								// turn off, use On() to switch on at same value
	{
		SetLevel(0);
	}

	bool IsOn()
	{
		return _lastlevel != 0;
	}

private:

	unsigned char _lastlevel;

	void SetLevel(unsigned char level)
	{
		_lastlevel = level;
		CHAL::analogWrite(PIN, 255 - level);
	}
};

////////////////////////////////////////////////////////
