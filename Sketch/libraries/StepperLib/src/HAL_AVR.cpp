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
/*
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
*/
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

uint8_t CHAL::digitalRead(pin_t pin)
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
// => do not calle digitalwrite (use CHAL::digitalWrite)

#define cbi(sfr, bit) (_SFR_BYTE(sfr) &= ~_BV(bit))
#define sbi(sfr, bit) (_SFR_BYTE(sfr) |= _BV(bit))

static void turnOffPWM(uint8_t timer)
{
	switch (timer)
	{
#if defined(TCCR1A) && defined(COM1A1)
	case TIMER1A:   cbi(TCCR1A, COM1A1);    break;
#endif
#if defined(TCCR1A) && defined(COM1B1)
	case TIMER1B:   cbi(TCCR1A, COM1B1);    break;
#endif
#if defined(TCCR1A) && defined(COM1C1)
	case TIMER1C:   cbi(TCCR1A, COM1C1);    break;
#endif

#if defined(TCCR2) && defined(COM21)
	case  TIMER2:   cbi(TCCR2, COM21);      break;
#endif

#if defined(TCCR0A) && defined(COM0A1)
	case  TIMER0A:  cbi(TCCR0A, COM0A1);    break;
#endif

#if defined(TCCR0A) && defined(COM0B1)
	case  TIMER0B:  cbi(TCCR0A, COM0B1);    break;
#endif
#if defined(TCCR2A) && defined(COM2A1)
	case  TIMER2A:  cbi(TCCR2A, COM2A1);    break;
#endif
#if defined(TCCR2A) && defined(COM2B1)
	case  TIMER2B:  cbi(TCCR2A, COM2B1);    break;
#endif

#if defined(TCCR3A) && defined(COM3A1)
	case  TIMER3A:  cbi(TCCR3A, COM3A1);    break;
#endif
#if defined(TCCR3A) && defined(COM3B1)
	case  TIMER3B:  cbi(TCCR3A, COM3B1);    break;
#endif
#if defined(TCCR3A) && defined(COM3C1)
	case  TIMER3C:  cbi(TCCR3A, COM3C1);    break;
#endif

#if defined(TCCR4A) && defined(COM4A1)
	case  TIMER4A:  cbi(TCCR4A, COM4A1);    break;
#endif					
#if defined(TCCR4A) && defined(COM4B1)
	case  TIMER4B:  cbi(TCCR4A, COM4B1);    break;
#endif
#if defined(TCCR4A) && defined(COM4C1)
	case  TIMER4C:  cbi(TCCR4A, COM4C1);    break;
#endif			
#if defined(TCCR4C) && defined(COM4D1)
	case TIMER4D:	cbi(TCCR4C, COM4D1);	break;
#endif			

#if defined(TCCR5A)
	case  TIMER5A:  cbi(TCCR5A, COM5A1);    break;
	case  TIMER5B:  cbi(TCCR5A, COM5B1);    break;
	case  TIMER5C:  cbi(TCCR5A, COM5C1);    break;
#endif
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
#endif

#if defined(TCCR1A) && defined(COM1C1)
		case TIMER1C:
			// connect pwm to pin on timer 1, channel B
			sbi(TCCR1A, COM1C1);
			OCR1C = val; // set pwm duty
			break;
#endif

#if defined(TCCR2) && defined(COM21)
		case TIMER2:
			// connect pwm to pin on timer 2
			sbi(TCCR2, COM21);
			OCR2 = val; // set pwm duty
			break;
#endif

#if defined(TCCR2A) && defined(COM2A1)
		case TIMER2A:
			// connect pwm to pin on timer 2, channel A
			sbi(TCCR2A, COM2A1);
			OCR2A = val; // set pwm duty
			break;
#endif

#if defined(TCCR2A) && defined(COM2B1)
		case TIMER2B:
			// connect pwm to pin on timer 2, channel B
			sbi(TCCR2A, COM2B1);
			OCR2B = val; // set pwm duty
			break;
#endif

#if defined(TCCR3A) && defined(COM3A1)
		case TIMER3A:
			// connect pwm to pin on timer 3, channel A
			sbi(TCCR3A, COM3A1);
			OCR3A = val; // set pwm duty
			break;
#endif

#if defined(TCCR3A) && defined(COM3B1)
		case TIMER3B:
			// connect pwm to pin on timer 3, channel B
			sbi(TCCR3A, COM3B1);
			OCR3B = val; // set pwm duty
			break;
#endif

#if defined(TCCR3A) && defined(COM3C1)
		case TIMER3C:
			// connect pwm to pin on timer 3, channel C
			sbi(TCCR3A, COM3C1);
			OCR3C = val; // set pwm duty
			break;
#endif

#if defined(TCCR4A)
		case TIMER4A:
			//connect pwm to pin on timer 4, channel A
			sbi(TCCR4A, COM4A1);
#if defined(COM4A0)		// only used on 32U4
			cbi(TCCR4A, COM4A0);
#endif
			OCR4A = val;	// set pwm duty
			break;
#endif

#if defined(TCCR4A) && defined(COM4B1)
		case TIMER4B:
			// connect pwm to pin on timer 4, channel B
			sbi(TCCR4A, COM4B1);
			OCR4B = val; // set pwm duty
			break;
#endif

#if defined(TCCR4A) && defined(COM4C1)
		case TIMER4C:
			// connect pwm to pin on timer 4, channel C
			sbi(TCCR4A, COM4C1);
			OCR4C = val; // set pwm duty
			break;
#endif

#if defined(TCCR4C) && defined(COM4D1)
		case TIMER4D:
			// connect pwm to pin on timer 4, channel D
			sbi(TCCR4C, COM4D1);
#if defined(COM4D0)		// only used on 32U4
			cbi(TCCR4C, COM4D0);
#endif
			OCR4D = val;	// set pwm duty
			break;
#endif


#if defined(TCCR5A) && defined(COM5A1)
		case TIMER5A:
			// connect pwm to pin on timer 5, channel A
			sbi(TCCR5A, COM5A1);
			OCR5A = val; // set pwm duty
			break;
#endif

#if defined(TCCR5A) && defined(COM5B1)
		case TIMER5B:
			// connect pwm to pin on timer 5, channel B
			sbi(TCCR5A, COM5B1);
			OCR5B = val; // set pwm duty
			break;
#endif

#if defined(TCCR5A) && defined(COM5C1)
		case TIMER5C:
			// connect pwm to pin on timer 5, channel C
			sbi(TCCR5A, COM5C1);
			OCR5C = val; // set pwm duty
			break;
#endif
		}
	}
}

#endif		//not 2560
#endif		// AVR

