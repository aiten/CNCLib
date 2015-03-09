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

template <unsigned char PIN, unsigned char ONVALUE, unsigned long STABLETIME>
class CReadPinIOTriggerControl
{
public:

	static void Init()
	{
		CHAL::pinMode(PIN, INPUT_PULLUP);
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
