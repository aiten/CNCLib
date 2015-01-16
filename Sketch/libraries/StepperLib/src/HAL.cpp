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

#include <stdlib.h>
#include <string.h>

#include <arduino.h>
#include <ctype.h>

#include "HAL.h"

////////////////////////////////////////////////////////

static void IgnoreIrq() {}

CHAL::HALEvent CHAL::_TimerEvent0 = IgnoreIrq;
CHAL::HALEvent CHAL::_TimerEvent1 = IgnoreIrq;
CHAL::HALEvent CHAL::_TimerEvent2 = IgnoreIrq;

#if !defined(__AVR_ATmega328P__)

CHAL::HALEvent CHAL::_TimerEvent3 = IgnoreIrq;
CHAL::HALEvent CHAL::_TimerEvent4 = IgnoreIrq;
CHAL::HALEvent CHAL::_TimerEvent5 = IgnoreIrq;

#endif

