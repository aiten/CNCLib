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


////////////////////////////////////////////////////////////

class WatchDog
{
private:

	bool _isOn = false;
	uint8_t _onValue;
	uint8_t _offValue;
	uint8_t _pin;

public:

	void Init(uint8_t pin, uint8_t onvalue)
	{
		_pin = pin;
		_onValue = onvalue;
		_offValue = onvalue == LOW ? HIGH : LOW;

		pinMode(_pin, OUTPUT);
		digitalWrite(_pin, _offValue);
	}

	bool OnOff(bool on)
	{
		return on ? On() : Off();
	}

	bool On()
	{
		digitalWrite(_pin, _onValue);
		if (_isOn == false)
		{
			_isOn = true;
			return true;
		}
		return false;
	}

	////////////////////////////////////////////////////////////

	bool IsOn()
	{
		return _isOn;
	}

	////////////////////////////////////////////////////////////

	bool Off()
	{
		digitalWrite(_pin, _offValue);
		if (_isOn == true)
		{
			_isOn = false;
			return true;
		}
		return false;
	}
};

