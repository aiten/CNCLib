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

template <pin_t PIN, uint16_t delayMs>
class CAnalog8IOControlSmooth
{
public:

	CAnalog8IOControlSmooth()
	{
		_currentlevel=0;
		_iolevel=0;
	}

	void Init(uint8_t level=0)		// init and set default value
	{
		MySetLevel(level);
		_level = level;
	}

	void On(uint8_t level)					// Set level and turn on
	{
		_level = level;
		MySetLevel(level);
	}

	void OnMax()							// turn on at max level, same as On(255)
	{
		On(255);
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
		return _iolevel != 0;
	}

	void SetLevel(uint8_t level)
	{
		_level = level;
	}

	uint8_t GetLevel() const
	{
		return _level;
	}

	uint8_t GetIOLevel() const
	{
		return _iolevel;
	}

	uint8_t GetCurrentIOLevel() const
	{
		return _currentlevel;
	}

	void Poll()
	{
		if (_currentlevel != _iolevel && millis() >= _nexttime)
		{
			_nexttime = millis() + delayMs;
			if (_currentlevel > _iolevel)
				_currentlevel--;
			else
				_currentlevel++;
			CHAL::analogWrite8(PIN, _currentlevel);
		}
	}

private:

	unsigned long _nexttime;				// time to modify level
	uint8_t _level;					// value if "enabled", On/Off will switch between 0..level
	uint8_t _currentlevel;			// used for analogWrite
	uint8_t _iolevel;				// current level

	void MySetLevel(uint8_t level)
	{
		_iolevel = level;
		_nexttime = 0;
	}
};

////////////////////////////////////////////////////////
