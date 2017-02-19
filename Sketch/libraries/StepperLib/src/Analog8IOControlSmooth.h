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

template <pin_t PWMPIN>
class CAnalog8IOControlSmooth
{
public:

	void Init()		// init and set default value
	{
		_nexttime = 0;
		_currentlevel = _iolevel = 0;
		Out(0);
#ifndef REDUCED_SIZE
		_level = 0;
#endif
	}

	void Init(uint8_t level)		// init and set default value
	{
		Init();
		On(level);
	}

	void On(uint8_t level)					// Set level and turn on
	{
#ifndef REDUCED_SIZE
		_level = level;
#endif
		MySetLevel(level);
	}

	void OnMax()							// turn on at max level, same as On(255)
	{
		On(255);
	}

#ifndef REDUCED_SIZE
	void On()								// turn on at specified level (see Level property)
	{
		MySetLevel(_level);
	}
#endif

	void Off()								// turn off, use On() to switch on at same value
	{
		MySetLevel(0);
	}

	bool IsOn() const
	{
		return _iolevel != 0;
	}

#ifndef REDUCED_SIZE
	void SetLevel(int16_t level)
	{
		_level = level;
	}

	uint8_t GetLevel() const
	{
		return _level;
	}
#endif

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
		unsigned long milli;
		if (_currentlevel != _iolevel && (milli=millis()) >= _nexttime)
		{
			_nexttime = milli + _delayMs;
			if (_currentlevel > _iolevel)
				_currentlevel--;
			else
				_currentlevel++;

			Out(_currentlevel);
		}
	}

	void SetDelay(uint8_t delayms)
	{
		_delayMs = delayms;
	}

private:

	static void Out(uint8_t lvl) 
	{
		CHAL::analogWrite8(PWMPIN,lvl);
	}

	unsigned long _nexttime;		// time to modify level
#ifndef REDUCED_SIZE
	uint8_t _level;					// value if "enabled", On/Off will switch between 0..level
#endif

	uint8_t _currentlevel;			// used for analogWrite
	uint8_t _iolevel;				// current level
	uint8_t	_delayMs;

	void MySetLevel(uint8_t level) NEVER_INLINE_AVR
	{
		_iolevel = level;
		if (_delayMs == 0)
		{
			_currentlevel = level;
			Out(level);
		}
	}
};

////////////////////////////////////////////////////////
