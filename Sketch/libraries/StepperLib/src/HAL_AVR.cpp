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
#include "UtilitiesStepperLib.h"

////////////////////////////////////////////////////////

#if defined(__AVR_ARCH__)

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

void CHAL::digitalWrite(uint8_t pin, uint8_t val)
{
//	uint8_t timer = digitalPinToTimer(pin);
	uint8_t bit = digitalPinToBitMask(pin);
	uint8_t port = digitalPinToPort(pin);
	volatile uint8_t *out;

	if (port == NOT_A_PIN) return;

	// If the pin that support PWM output, we need to turn it off
	// before doing a digital write.
//	if (timer != NOT_ON_TIMER) turnOffPWM(timer);

	out = portOutputRegister(port);

	uint8_t oldSREG = SREG;
	cli();

	if (val == LOW) {
		*out &= ~bit;
	} else {
		*out |= bit;
	}

	SREG = oldSREG;
}

unsigned char CHAL::digitalRead(uint8_t pin)
{
//	uint8_t timer = digitalPinToTimer(pin);
	uint8_t bit = digitalPinToBitMask(pin);
	uint8_t port = digitalPinToPort(pin);

	if (port == NOT_A_PIN) return LOW;

	// If the pin that support PWM output, we need to turn it off
	// before getting a digital reading.
//	if (timer != NOT_ON_TIMER) turnOffPWM(timer);

	if (*portInputRegister(port) & bit) return HIGH;
	return LOW;
}

#endif 

