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

template <pin_t PIN, uint8_t ONVALUE, uint8_t OFFVALUE>
class COnOffIOControl
{
public:

	void Init()
	{
		CHAL::pinModeOutput(PIN);
		Set(false);
	}

	void Init(uint8_t isOn)
	{
		Init();
		On(isOn);
	}

	void Set(bool val)
	{
		CHAL::digitalWrite(PIN, val ? ONVALUE : OFFVALUE);
	}

	void Off()
	{
		Set(false);
	}

	void On(uint8_t isOn)
	{
		Set(isOn !=0);
	}

	void On()
	{
		Set(true);
	}

	bool IsOn()
	{
		return CHAL::digitalRead(PIN) == ONVALUE;
	}

	void SetLevel(uint8_t isOn)
	{
		On(isOn);
	}

	uint8_t GetLevel()
	{
		return IsOn();
	}
};

////////////////////////////////////////////////////////
