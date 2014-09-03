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
#include "UtilitiesStepperLib.h"

////////////////////////////////////////////////////////

#if defined(__AVR_ATmega1280__) || defined(__AVR_ATmega2560__) || defined(__AVR_ATmega328P__)

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

void CHAL::digitalWrite(uint8_t pin, uint8_t lowOrHigh)
{
	if (lowOrHigh)
	{
		switch (pin)
		{
			case 2: WRITE(2, 1); return;
			case 3: WRITE(3, 1); return;
			case 4: WRITE(4, 1); return;
			case 5: WRITE(5, 1); return;
			case 6: WRITE(6, 1); return;
			case 7: WRITE(7, 1); return;
			case 8: WRITE(8, 1); return;
			case 9: WRITE(9, 1); return;
			case 10: WRITE(10, 1); return;
			case 11: WRITE(11, 1); return;
			case 12: WRITE(12, 1); return;
			case 13: WRITE(13, 1); return;
		}
	}
	else
	{
		switch (pin)
		{
			case 2: WRITE(2, 0); return;
			case 3: WRITE(3, 0); return;
			case 4: WRITE(4, 0); return;
			case 5: WRITE(5, 0); return;
			case 6: WRITE(6, 0); return;
			case 7: WRITE(7, 0); return;
			case 8: WRITE(8, 0); return;
			case 9: WRITE(9, 0); return;
			case 10: WRITE(10, 0); return;
			case 11: WRITE(11, 0); return;
			case 12: WRITE(12, 0); return;
			case 13: WRITE(13, 0); return;
		}
	}

	::digitalWrite(pin, lowOrHigh);
}

#endif 

