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
// AVR 8bit
////////////////////////////////////////////////////////

#if defined(__AVR_ARCH__)

//#define pgm_read_ptr pgm_read_word
#define pgm_read_int pgm_read_word
#define pgm_read_uint pgm_read_word

#define TIMER0PRESCALE      64
#define TIMER0FREQUENCE		(F_CPU/TIMER0PRESCALE)

#define TIMER1PRESCALE      8
#define TIMER1FREQUENCE		(F_CPU/TIMER1PRESCALE)

#define TIMER1MIN			32
#define TIMER1MAX			0xffff

#define TIMER2PRESCALE      1024
#define TIMER2FREQUENCE		(F_CPU/TIMER2PRESCALE)

#ifndef __AVR_ATmega328P__

#define TIMER3PRESCALE      1024
#define TIMER3FREQUENCE		(F_CPU/TIMER3PRESCALE)

#define TIMER4PRESCALE      1024
#define TIMER4FREQUENCE		(F_CPU/TIMER4PRESCALE)

#define TIMER5PRESCALE      1024
#define TIMER5FREQUENCE		(F_CPU/TIMER5PRESCALE)

#endif

#define MAXINTERRUPTSPEED	(65535/7)	// maximal possible interrupt rate => steprate_t

#define SPEED_MULTIPLIER_1	0
#define SPEED_MULTIPLIER_2	(MAXINTERRUPTSPEED*1)
#define SPEED_MULTIPLIER_3	(MAXINTERRUPTSPEED*2)
#define SPEED_MULTIPLIER_4	(MAXINTERRUPTSPEED*3)
#define SPEED_MULTIPLIER_5	(MAXINTERRUPTSPEED*4)
#define SPEED_MULTIPLIER_6	(MAXINTERRUPTSPEED*5)
#define SPEED_MULTIPLIER_7	(MAXINTERRUPTSPEED*6)

#define TIMEROVERHEAD		(14)		// decrease Timervalue for ISR overhead before set new timer

#include <avr/interrupt.h>
#include <avr/io.h>

#include "fastio.h"

////////////////////////////////////////////////////////
// For shorter delays use assembly language call 'nop' (no operation). Each 'nop' statement executes in one machine cycle (at 16 MHz) yielding a 62.5 ns (nanosecond) delay. 

inline void CHAL::delayMicroseconds0250() {	__asm__("nop\n\tnop\n\tnop\n\tnop\n\t"); }

inline void CHAL::delayMicroseconds0312() {	__asm__("nop\n\tnop\n\tnop\n\tnop\n\tnop\n\t"); }

inline void CHAL::delayMicroseconds0375() { __asm__("nop\n\tnop\n\tnop\n\tnopn\tnop\n\tnop\n\t"); }

inline void CHAL::delayMicroseconds0438() { __asm__("nop\n\tnop\n\tnop\n\tnopn\tnop\n\tnop\n\tnop\n\t"); }

inline void CHAL::delayMicroseconds0500() {	__asm__("nop\n\tnop\n\tnop\n\tnop\n\tnop\n\tnop\n\tnop\n\tnop\n\t"); }

inline void CHAL::delayMicroseconds(unsigned int us) {	::delayMicroseconds(us); }

inline void CHAL::DisableInterrupts()	{	cli(); }
inline void CHAL::EnableInterrupts()	{	sei(); }

inline irqflags_t CHAL::GetSREG()		{ return SREG; }
inline void CHAL::SetSREG(irqflags_t a)	{ SREG=a; }

inline void  CHAL::RemoveTimer0()		{}

inline void  CHAL::InitTimer0(HALEvent evt)
{
	// shared with millis!
	_TimerEvent0 = evt;
}

////////////////////////////////////////////////////////

inline void CHAL::StartTimer0(timer_t)
{
	// shared with millis => set only interrup mask!
	TIMSK0 |= (1<<OCIE0B);  
	OCR0B = 128;
}

////////////////////////////////////////////////////////

inline void CHAL::StopTimer0()
{
	TIMSK0 &= ~(1<<OCIE0B);  
}  

////////////////////////////////////////////////////////

inline void  CHAL::RemoveTimer1() {}

inline void  CHAL::InitTimer1(HALEvent evt)
{
	_TimerEvent1 = evt;

	TCCR1A = 0x00;							// stetzt Statusregiser A Vom Timer eins auf null
	TCCR1B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TCCR1B |= (1<<CS11);					// timer laeuft mit 1/8 des CPU Takt.
}

////////////////////////////////////////////////////////

inline void CHAL::StartTimer1(timer_t timer)
{
	TCNT1  = 0 - timer;  
	TIMSK1 |= (1<<TOIE1);					// Aktiviert Interrupt beim Overflow des Timers 1
	TCCR1B |= (1<<CS11) ;					// timer laeuft mit 1/8 des CPU Takt.
	TIFR1  |= (1<<TOV1);					// clear the overflow flag
}

////////////////////////////////////////////////////////

inline void CHAL::StopTimer1()
{
	TCCR1B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TIMSK1 &= ~(1<<TOIE1);					// Deaktiviert Interrupt beim Overflow des Timers 1
	TCNT1=0;  
}  

////////////////////////////////////////////////////////

inline void  CHAL::RemoveTimer2() {}

////////////////////////////////////////////////////////

inline void  CHAL::InitTimer2(HALEvent evt)
{
	_TimerEvent2 = evt;  
	TCCR2A = 0x00;							// stetzt Statusregiser A Vom Timer eins auf null
	TCCR2B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TCCR2B |= ((1 << CS22) | (1 << CS21) | (1 << CS20));	// timer laeuft mit 1/1024 des CPU Takt.
}

////////////////////////////////////////////////////////

inline void CHAL::StartTimer2(timer_t timer)
{
	TCNT2 = (0x100 - timer);				// timer2 is 8bit
	TIMSK2 |= (1 << TOIE2);					// Aktiviert Interrupt beim Overflow des Timers 2
	TCCR2B |= ((1 << CS22) | (1 << CS21) | (1 << CS20));	// timer laeuft mit 1/1024 des CPU Takt.
	TIFR2  |= (1 << TOV2);					// clear the overflow flag
}

////////////////////////////////////////////////////////

inline void CHAL::StopTimer2()
{
	TCCR2B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TIMSK2 &= ~(1 << TOIE2);				// Deaktiviert Interrupt beim Overflow des Timers 2
	TCNT2 = 0;
}

////////////////////////////////////////////////////////

#if !defined(__AVR_ATmega328P__)

////////////////////////////////////////////////////////

inline void  CHAL::RemoveTimer3() {}

inline void  CHAL::InitTimer3(HALEvent evt)
{
	_TimerEvent3 = evt;

	TCCR3A = 0x00;							// stetzt Statusregiser A Vom Timer eins auf null
	TCCR3B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TCCR3B |= (1<<CS32) | (1<<CS30);		// timer laeuft mit 1/1024 des CPU Takt.
}

////////////////////////////////////////////////////////

inline void CHAL::StartTimer3(timer_t timer)
{
	TCNT3  = 0 - timer;  
	TIMSK3 |= (1<<TOIE3);					// Aktiviert Interrupt beim Overflow des Timers 1
	TCCR3B |= (1<<CS32) | (1<<CS30);		// timer laeuft mit 1/1024 des CPU Takt.
	TIFR3  |= (1<<TOV3);					// clear the overflow flag
}

////////////////////////////////////////////////////////

inline void CHAL::StopTimer3()
{
	TCCR3B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TIMSK3 &= ~(1<<TOIE3);					// Deaktiviert Interrupt beim Overflow des Timers 1
	TCNT3=0;  
}  

////////////////////////////////////////////////////////

inline void  CHAL::RemoveTimer4() {}

inline void  CHAL::InitTimer4(HALEvent evt)
{
	_TimerEvent4 = evt;

	TCCR4A = 0x00;							// stetzt Statusregiser A Vom Timer eins auf null
	TCCR4B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TCCR4B |= (1<<CS42) | (1<<CS40);		// timer laeuft mit 1/1024 des CPU Takt.
}

////////////////////////////////////////////////////////

inline void CHAL::StartTimer4(timer_t timer)
{
	TCNT4  = 0 - timer;  
	TIMSK4 |= (1<<TOIE4);					// Aktiviert Interrupt beim Overflow des Timers 1
	TCCR4B |= (1<<CS42) | (1<<CS40);		// timer laeuft mit 1/1024 des CPU Takt.
	TIFR4  |= (1<<TOV4);					// clear the overflow flag
}

////////////////////////////////////////////////////////

inline void CHAL::StopTimer4()
{
	TCCR4B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TIMSK4 &= ~(1<<TOIE4);					// Deaktiviert Interrupt beim Overflow des Timers 1
	TCNT4=0;  
}  

////////////////////////////////////////////////////////

inline void  CHAL::RemoveTimer5() {}

inline void  CHAL::InitTimer5(HALEvent evt)
{
	_TimerEvent5 = evt;

	TCCR5A = 0x00;							// stetzt Statusregiser A Vom Timer eins auf null
	TCCR5B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TCCR5B |= (1<<CS52) | (1<<CS50);		// timer laeuft mit 1/1024 des CPU Takt.
}

////////////////////////////////////////////////////////

inline void CHAL::StartTimer5(timer_t timer)
{
	TCNT5  = 0 - timer;  
	TIMSK5 |= (1<<TOIE5);					// Aktiviert Interrupt beim Overflow des Timers 1
	TCCR5B |= (1<<CS52) | (1<<CS50);		// timer laeuft mit 1/1024 des CPU Takt.
	TIFR5  |= (1<<TOV5);					// clear the overflow flag
}

////////////////////////////////////////////////////////

inline void CHAL::StopTimer5()
{
	TCCR5B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TIMSK5 &= ~(1<<TOIE5);					// Deaktiviert Interrupt beim Overflow des Timers 1
	TCNT5=0;  
}  

#endif

#define HALFastdigitalWrite(a,b) WRITE(a,b)
#define HALFastdigitalWriteNC(a,b) _WRITE_NC(a,b)
#define HALFastdigitalRead(a) READ(a)

inline void CHAL::pinMode(pin_t pin, uint8_t mode)
{
	::pinMode(pin, mode);
}

void CHAL::pinModeOutput(pin_t pin)
{ 
	::pinMode(pin, OUTPUT);
}

void CHAL::pinModeInputPullUp(pin_t pin)
{
	::pinMode(pin, INPUT_PULLUP);
}

void CHAL::eeprom_write_dword(uint32_t *  __p, uint32_t  	__value) 
{ 
	::eeprom_write_dword(__p, __value);
}

uint32_t CHAL::eeprom_read_dword(const uint32_t * __p) 
{ 
	return ::eeprom_read_dword(__p);
}

uint8_t CHAL::eeprom_read_byte(const uint8_t * __p) 
{ 
	return ::eeprom_read_byte(__p);
}

#endif 

