////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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

#include "Configuration_ProxxonMF70.h"

////////////////////////////////////////////////////////

class CControllerFanControl
{
public:

	unsigned char Level;    // use like a property

	void Init()
	{
		On(255);
		Level=255;
	}

	void On()
	{
		On(Level);
	}

	void Off()
	{
		On(0);
	}

private:

	void On(unsigned char level)
	{
		analogWrite(CONTROLLERFAN_FAN_PIN, level);
	}

};

////////////////////////////////////////////////////////
