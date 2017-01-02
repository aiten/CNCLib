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

template <pin_t PIN, uint8_t ONVALUE, unsigned long STABLETIME>
class CReadPinIOTriggerControl
{
public:

	static void Init(uint8_t inputmode = INPUT_PULLUP)
	{
		CHAL::pinMode(PIN, inputmode);
	}

	bool IsOn()
	{
		if (CHAL::digitalRead(PIN) == ONVALUE)
		{
			unsigned long now = millis();

			if (_timeOn == 0)	// first on
			{
				_timeOn = now;
			}
			
			return (now - _timeOn >= STABLETIME);
		}

		_timeOn = 0;
		return false;
	}
private:

	unsigned long _timeOn = 0;
};

////////////////////////////////////////////////////////
