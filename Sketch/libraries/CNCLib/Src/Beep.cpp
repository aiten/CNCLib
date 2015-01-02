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

#include <stdlib.h>
#include <string.h>

#include <arduino.h>
#include <ctype.h>

#include "Beep.h"

////////////////////////////////////////////////////////

const SPlayTone SPlayTone::PlayOK[] PROGMEM =
{
	{ ToneA6, 10 },
	{ ToneEnd }
};

const SPlayTone SPlayTone::PlayError[] PROGMEM
{
	{ ToneA6, 20 },
	{ ToneA7, 20 },
	{ ToneA6, 20 },
	{ ToneA7, 20 },
	{ ToneEnd }
};

const SPlayTone SPlayTone::PlayInfo[] PROGMEM
{
	{ ToneA5, 20 },
	{ ToneA6, 20 },
	{ ToneA7, 20 },
	{ ToneEnd }
};

