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

class CPushButton
{
public:

	CPushButton()
	{
	}

	void SetPin(unsigned char pin, unsigned char onValue)		
	{ 
		_pin=pin; 
		_onvalue = onValue; 
		CHAL::pinMode(_pin, INPUT_PULLUP);
	}

	bool CheckOn()
	{
		unsigned char value = CHAL::digitalRead(_pin);

		if (_expectButtonOff)
		{
			if (value != _onvalue)
				_expectButtonOff = false;
			return false;
		}

		return value == _onvalue;
	}

	bool IsOn()
	{
		// check and set state 
		// button must be released and pressed to get "true" again.

		if (CheckOn())
		{
			_expectButtonOff = true;
			return true;
		}
		return false;
	}

protected:

	unsigned char	_pin = 0;
	unsigned char	_onvalue = 0;
	bool			_expectButtonOff = false;
};

////////////////////////////////////////////////////////
