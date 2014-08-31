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

////////////////////////////////////////////////////////
#if defined(__SAM3X8E__)

//__attribute__((__interrupt__))
//__attribute__((nesting))
void TC8_Handler()
{
	TC_GetStatus(DUETIMER1_TC, DUETIMER1_CHANNEL);
	CHAL::_TimerEvent1();
}

void TC6_Handler()
{
	TC_GetStatus(DUETIMER3_TC, DUETIMER3_CHANNEL);
	CHAL::_TimerEvent3();
}

void CAN0_Handler()
{
	CHAL::_CAM0Event();
}

CHAL::HALEvent CHAL::_CAM0Event = IgnoreIrq;

////////////////////////////////////////////////////////
#elif defined(__AVR_ATmega1280__) || defined(__AVR_ATmega2560__) || defined(__AVR_ATmega328P__)

ISR(TIMER0_COMPB_vect)
{
	CHAL::_TimerEvent0();
}

ISR(TIMER1_OVF_vect)
{
	CHAL::_TimerEvent1();
}

ISR(TIMER2_OVF_vect)
{
	CHAL::_TimerEvent2();
}

#if !defined(__AVR_ATmega328P__)

ISR(TIMER3_OVF_vect)
{
	CHAL::_TimerEvent3();
}

ISR(TIMER4_OVF_vect)
{
	CHAL::_TimerEvent4();
}

ISR(TIMER5_OVF_vect)
{
	CHAL::_TimerEvent5();
}

#endif

////////////////////////////////////////////////////////
#elif defined(_MSC_VER)



////////////////////////////////////////////////////////
#else

ToDo;

#endif 

