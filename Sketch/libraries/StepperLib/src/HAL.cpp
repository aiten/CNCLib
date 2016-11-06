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

#include <stdlib.h>
#include <string.h>

#include <arduino.h>
#include <ctype.h>

#include "HAL.h"

////////////////////////////////////////////////////////

unsigned short CHAL::analogRead(pin_t pin)
{
	return (unsigned short) ::analogRead(pin);
}

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

#ifdef _MSC_VER

std::function<uint8_t(short)> mydigitalRead=NULL;

#endif