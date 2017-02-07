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

class CDummyIOControl
{
public:

	void Init(uint8_t = 0)			// digital and analog
	{
	}

	void Set(bool)
	{
	}

	void Off()
	{
	}

	void On(uint8_t  = 0)			// same signature as CAnalog8IOControl
	{
	}

	void SetLevel(uint8_t)	
	{
	}

	uint8_t GetLevel()
	{
		return 0;
	}

	bool IsOn() const				// default is off, e.g. for "kill"
	{
		return false;
	}

	void SetPin(pin_t)
	{
	}

	void Check()
	{
	}

	void Poll()
	{
	}
};

////////////////////////////////////////////////////////
