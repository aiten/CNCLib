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

void CHAL::digitalWrite(pin_t pin, uint8_t val)
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

unsigned char CHAL::digitalRead(pin_t pin)
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

#if defined(__AVR_ATmega1280__) || defined(__AVR_ATmega2560__)

void CHAL::analogWrite8(pin_t pin, uint8_t val)
{
	// do not care about size
	::analogWrite(pin, val);
}

#else

// care about size
// => do not calle digitalwrite

#define cbi(sfr, bit) (_SFR_BYTE(sfr) &= ~_BV(bit))
#define sbi(sfr, bit) (_SFR_BYTE(sfr) |= _BV(bit))

static void turnOffPWM(uint8_t timer)
{
	switch (timer)
	{
		case TIMER0A:  cbi(TCCR0A, COM0A1);    break;
		case TIMER0B:  cbi(TCCR0A, COM0B1);    break;
		case TIMER1A:  cbi(TCCR1A, COM1A1);    break;
		case TIMER1B:  cbi(TCCR1A, COM1B1);    break;
	}
}

void CHAL::analogWrite8(pin_t pin, uint8_t val)
{
	pinModeOutput(pin);
	if (val == 0 || val == 255)
	{
		turnOffPWM(digitalPinToTimer(pin));
		digitalWrite(pin, val == 0 ? LOW : HIGH);
	}
	else
	{
		switch (digitalPinToTimer(pin))
		{
			// XXX fix needed for atmega8
#if defined(TCCR0) && defined(COM00) && !defined(__AVR_ATmega8__)
		case TIMER0A:
			// connect pwm to pin on timer 0
			sbi(TCCR0, COM00);
			OCR0 = val; // set pwm duty
			break;
#endif

#if defined(TCCR0A) && defined(COM0A1)
		case TIMER0A:
			// connect pwm to pin on timer 0, channel A
			sbi(TCCR0A, COM0A1);
			OCR0A = val; // set pwm duty
			break;
#endif

#if defined(TCCR0A) && defined(COM0B1)
		case TIMER0B:
			// connect pwm to pin on timer 0, channel B
			sbi(TCCR0A, COM0B1);
			OCR0B = val; // set pwm duty
			break;
#endif

#if defined(TCCR1A) && defined(COM1A1)
		case TIMER1A:
			// connect pwm to pin on timer 1, channel A
			sbi(TCCR1A, COM1A1);
			OCR1A = val; // set pwm duty
			break;
#endif

#if defined(TCCR1A) && defined(COM1B1)
		case TIMER1B:
			// connect pwm to pin on timer 1, channel B
			sbi(TCCR1A, COM1B1);
			OCR1B = val; // set pwm duty
			break;
		}
	}

#endif

}

#endif		//not 2560
#endif		// AVR

