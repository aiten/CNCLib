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

#include "Configuration.h"

////////////////////////////////////////////////////////

#define TIMER0VALUE(freq)	((timer_t)((unsigned long)TIMER0FREQUENCE/(unsigned long)freq))
#define TIMER1VALUE(freq)	((timer_t)((unsigned long)TIMER1FREQUENCE/(unsigned long)freq))
#define TIMER2VALUE(freq)	((timer_t)((unsigned long)TIMER2FREQUENCE/(unsigned long)freq))
#define TIMER3VALUE(freq)	((timer_t)((unsigned long)TIMER3FREQUENCE/(unsigned long)freq))
#define TIMER4VALUE(freq)	((timer_t)((unsigned long)TIMER4FREQUENCE/(unsigned long)freq))
#define TIMER5VALUE(freq)	((timer_t)((unsigned long)TIMER5FREQUENCE/(unsigned long)freq))

////////////////////////////////////////////////////////

typedef void(*HALTimerEvent)();

// min 8 bit 
extern void HALInitTimer0(HALTimerEvent evt);
extern void HALRemoveTimer0();
extern void HALStartTimer0(timer_t timer);
extern void HALStopTimer0();

// min 16 bit
extern void HALInitTimer1(HALTimerEvent evt);
extern void HALRemoveTimer1();
extern void HALStartTimer1(timer_t timer);
extern void HALStopTimer1();

// 8 bit
extern void HALInitTimer2(HALTimerEvent evt);
extern void HALRemoveTimer2();
extern void HALStartTimer2(timer_t timer);
extern void HALStopTimer2();

extern HALTimerEvent _halTimerEvent0;
extern HALTimerEvent _halTimerEvent1;
extern HALTimerEvent _halTimerEvent2;

#if !defined( __AVR_ATmega328P__)

// min 16 bit
extern void HALInitTimer3(HALTimerEvent evt);
extern void HALRemoveTimer3();
extern void HALStartTimer3(timer_t timer);
extern void HALStopTimer3();

// min 16 bit
extern void HALInitTimer4(HALTimerEvent evt);
extern void HALRemoveTimer4();
extern void HALStartTimer4(timer_t timer);
extern void HALStopTimer4();

// min 16 bit
extern void HALInitTimer5(HALTimerEvent evt);
extern void HALRemoveTimer5();
extern void HALStartTimer5(timer_t timer);
extern void HALStopTimer5();

extern HALTimerEvent _halTimerEvent3;
extern HALTimerEvent _halTimerEvent4;
extern HALTimerEvent _halTimerEvent5;

#endif

////////////////////////////////////////////////////////
// Due 32Bit
////////////////////////////////////////////////////////

#if defined(__SAM3X8E__)

#include <arduino.h>
#include <itoa.h>

#define pgm_read_ptr pgm_read_dword

#define TIMER0FREQUENCE		TIMER3FREQUENCE
#define TIMER0PRESCALE      TIMER3PRESCALE		

// compatible to AVR => no 32 bit Timers
#if 1
#define TIMER1FREQUENCE		2000000L	
#define TIMER1PRESCALE      8			
#else
#define TIMER1FREQUENCE		(F_CPU/TIMER1PRESCALE)
#define TIMER1PRESCALE      2			
#endif

#define TIMER2FREQUENCE		(F_CPU/TIMER2PRESCALE)
#define TIMER2PRESCALE      2			

#define TIMER3FREQUENCE		(F_CPU/TIMER3PRESCALE)
#define TIMER3PRESCALE      2			

#define TIMER4FREQUENCE		(F_CPU/TIMER4PRESCALE)
#define TIMER4PRESCALE      2			

#define TIMER5FREQUENCE		(F_CPU/TIMER5PRESCALE)
#define TIMER5PRESCALE      2			

#define TIMEROVERHEAD		1			// decrease Timervalue for ISR overhead before set new timer

#define SREG_T irqflags_t

#define DisableInterrupts()				cpu_irq_disable()
#define EnableInterrupts()				cpu_irq_enable()

#define GetSREG()						cpu_irq_save()
#define SetSREG(a)						cpu_irq_restore(a)

#define READ digitalRead
#define WRITE digitalWrite
#define _WRITE_NC digitalWrite

////////////////////////////////////////////////////////

#define DUETIMER1_TC					TC2
#define DUETIMER1_CHANNEL				2
#define DUETIMER1_IRQTYPE				((IRQn_Type) ID_TC8)

#define DUETIMER3_TC					TC2
#define DUETIMER3_CHANNEL				0
#define DUETIMER3_IRQTYPE				((IRQn_Type) ID_TC6)

////////////////////////////////////////////////////////

inline void  HALRemoveTimer0() {}

inline void HALStartTimer0(timer_t delay)
{
	HALStartTimer3(delay);
}

inline void  HALInitTimer0(HALTimerEvent evt)
{
	HALInitTimer3(evt);
}

inline void  HALRemoveTimer1() {}

inline void HALStartTimer1(timer_t delay)
{
	// convert old AVR timer delay value for SAM timers
	delay *= 21;		// 2MhZ to 42MhZ

	//	delay /= 2;			// do not know why
	//	uint32_t timer_count = (delay * TIMER1_PRESCALE);

	uint32_t timer_count = delay;

	if(timer_count == 0) timer_count = 1;
	TC_SetRC(DUETIMER1_TC, DUETIMER1_CHANNEL, timer_count);
	TC_Start(DUETIMER1_TC, DUETIMER1_CHANNEL);
}

////////////////////////////////////////////////////////

inline void  HALInitTimer1(HALTimerEvent evt)
{
	_halTimerEvent1 = evt;

	pmc_enable_periph_clk(DUETIMER1_IRQTYPE );
	NVIC_SetPriority(DUETIMER1_IRQTYPE, NVIC_EncodePriority(4, 1, 0));

	TC_Configure(DUETIMER1_TC, DUETIMER1_CHANNEL, TC_CMR_WAVSEL_UP_RC | TC_CMR_WAVE | TC_CMR_TCCLKS_TIMER_CLOCK1);

	TC_SetRC(DUETIMER1_TC, DUETIMER1_CHANNEL, 100000L);
	TC_Start(DUETIMER1_TC, DUETIMER1_CHANNEL);

	DUETIMER1_TC->TC_CHANNEL[DUETIMER1_CHANNEL].TC_IER = TC_IER_CPCS;
	DUETIMER1_TC->TC_CHANNEL[DUETIMER1_CHANNEL].TC_IDR = ~TC_IER_CPCS;
	NVIC_EnableIRQ(DUETIMER1_IRQTYPE); 
}

////////////////////////////////////////////////////////

inline void HALStopTimer1()
{
	NVIC_DisableIRQ(DUETIMER1_IRQTYPE);
	TC_Stop(DUETIMER1_TC, DUETIMER1_CHANNEL);
}  

////////////////////////////////////////////////////////

inline void  HALRemoveTimer3() {}

inline void HALStartTimer3(timer_t timer_count)
{
	if (timer_count == 0) timer_count = 1;
	TC_SetRC(DUETIMER3_TC, DUETIMER3_CHANNEL, timer_count);
	TC_Start(DUETIMER3_TC, DUETIMER3_CHANNEL);
}

////////////////////////////////////////////////////////

inline void  HALInitTimer3(HALTimerEvent evt)
{
	_halTimerEvent3 = evt;

	pmc_enable_periph_clk(DUETIMER3_IRQTYPE);
	NVIC_SetPriority(DUETIMER3_IRQTYPE, NVIC_EncodePriority(4, 1, 0));

	TC_Configure(DUETIMER3_TC, DUETIMER3_CHANNEL, TC_CMR_WAVSEL_UP_RC | TC_CMR_WAVE | TC_CMR_TCCLKS_TIMER_CLOCK1);

	TC_SetRC(DUETIMER3_TC, DUETIMER3_CHANNEL, 100000L);
	TC_Start(DUETIMER3_TC, DUETIMER3_CHANNEL);

	DUETIMER3_TC->TC_CHANNEL[DUETIMER3_CHANNEL].TC_IER = TC_IER_CPCS;
	DUETIMER3_TC->TC_CHANNEL[DUETIMER3_CHANNEL].TC_IDR = ~TC_IER_CPCS;
	NVIC_EnableIRQ(DUETIMER3_IRQTYPE);
}

////////////////////////////////////////////////////////

inline void HALStopTimer3()
{
	NVIC_DisableIRQ(DUETIMER3_IRQTYPE);
	TC_Stop(DUETIMER3_TC, DUETIMER3_CHANNEL);
}

////////////////////////////////////////////////////////
// AVR 8bit
////////////////////////////////////////////////////////

#elif defined(__AVR_ATmega1280__) || defined(__AVR_ATmega2560__) || defined(__AVR_ATmega328P__)

#define pgm_read_ptr pgm_read_word

#define TIMER0PRESCALE      64
#define TIMER0FREQUENCE		(F_CPU/TIMER0PRESCALE)

#define TIMER1PRESCALE      8
#define TIMER1FREQUENCE		(F_CPU/TIMER1PRESCALE)

#define TIMER2PRESCALE      1024
#define TIMER2FREQUENCE		(F_CPU/TIMER2PRESCALE)

#ifndef defined(__AVR_ATmega328P__)

#define TIMER3PRESCALE      1024
#define TIMER3FREQUENCE		(F_CPU/TIMER3PRESCALE)

#define TIMER4PRESCALE      1024
#define TIMER4FREQUENCE		(F_CPU/TIMER4PRESCALE)

#define TIMER5PRESCALE      1024
#define TIMER5FREQUENCE		(F_CPU/TIMER5PRESCALE)

#endif

#define TIMEROVERHEAD		(14)		// decrease Timervalue for ISR overhead before set new timer

#define SREG_T unsigned char
#define irqflags_t SREG_T

#define DisableInterrupts()				cli()
#define EnableInterrupts()				sei()
#define GetSREG()						SREG
#define SetSREG(a)						SREG=a

#include <arduino.h>
#include <avr/interrupt.h>
#include <avr/io.h>

#include "fastio.h"

////////////////////////////////////////////////////////

inline void  HALRemoveTimer0() {}

inline void  HALInitTimer0(HALTimerEvent evt)
{
	// shared with millis!
	_halTimerEvent0 = evt;
}

////////////////////////////////////////////////////////

inline void HALStartTimer0(timer_t timer)
{
	// shared with millis => set only interrup mask!
	TIMSK0 |= (1<<OCIE0B);  
	OCR0B = 128;
}

////////////////////////////////////////////////////////

inline void HALStopTimer0()
{
	TIMSK0 &= ~(1<<OCIE0B);  
}  

////////////////////////////////////////////////////////

inline void  HALRemoveTimer1() {}

inline void  HALInitTimer1(HALTimerEvent evt)
{
	_halTimerEvent1 = evt;

	TCCR1A = 0x00;							// stetzt Statusregiser A Vom Timer eins auf null
	TCCR1B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TCCR1B |= (1<<CS11);					// timer laeuft mit 1/8 des CPU Takt.
}

////////////////////////////////////////////////////////

inline void HALStartTimer1(timer_t timer)
{
	TCNT1  = 0 - timer;  
	TIMSK1 |= (1<<TOIE1);					// Aktiviert Interrupt beim Overflow des Timers 1
	TCCR1B |= (1<<CS11) ;					// timer laeuft mit 1/8 des CPU Takt.
	TIFR1  |= (1<<TOV1);					// clear the overflow flag
}

////////////////////////////////////////////////////////

inline void HALStopTimer1()
{
	TCCR1B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TIMSK1 &= ~(1<<TOIE1);					// Deaktiviert Interrupt beim Overflow des Timers 1
	TCNT1=0;  
}  

////////////////////////////////////////////////////////

inline void  HALRemoveTimer2() {}

////////////////////////////////////////////////////////

inline void  HALInitTimer2(HALTimerEvent evt)
{
	_halTimerEvent2 = evt;  
	TCCR2A = 0x00;							// stetzt Statusregiser A Vom Timer eins auf null
	TCCR2B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TCCR2B |= ((1 << CS22) | (1 << CS21) | (1 << CS20));	// timer laeuft mit 1/1024 des CPU Takt.
}

////////////////////////////////////////////////////////

inline void HALStartTimer2(timer_t timer)
{
	TCNT2 = (0x100 - timer);				// timer2 is 8bit
	TIMSK2 |= (1 << TOIE2);					// Aktiviert Interrupt beim Overflow des Timers 2
	TCCR2B |= ((1 << CS22) | (1 << CS21) | (1 << CS20));	// timer laeuft mit 1/1024 des CPU Takt.
	TIFR2  |= (1 << TOV2);					// clear the overflow flag
}

////////////////////////////////////////////////////////

inline void HALStopTimer2()
{
	TCCR2B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TIMSK2 &= ~(1 << TOIE2);				// Deaktiviert Interrupt beim Overflow des Timers 2
	TCNT2 = 0;
}

////////////////////////////////////////////////////////

#if !defined(__AVR_ATmega328P__)

////////////////////////////////////////////////////////

inline void  HALRemoveTimer3() {}

inline void  HALInitTimer3(HALTimerEvent evt)
{
	_halTimerEvent3 = evt;

	TCCR3A = 0x00;							// stetzt Statusregiser A Vom Timer eins auf null
	TCCR3B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TCCR3B |= (1<<CS32) | (1<<CS30);		// timer laeuft mit 1/1024 des CPU Takt.
}

////////////////////////////////////////////////////////

inline void HALStartTimer3(timer_t timer)
{
	TCNT3  = 0 - timer;  
	TIMSK3 |= (1<<TOIE3);					// Aktiviert Interrupt beim Overflow des Timers 1
	TCCR3B |= (1<<CS32) | (1<<CS30);		// timer laeuft mit 1/1024 des CPU Takt.
	TIFR3  |= (1<<TOV3);					// clear the overflow flag
}

////////////////////////////////////////////////////////

inline void HALStopTimer3()
{
	TCCR3B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TIMSK3 &= ~(1<<TOIE3);					// Deaktiviert Interrupt beim Overflow des Timers 1
	TCNT3=0;  
}  

////////////////////////////////////////////////////////

inline void  HALRemoveTimer4() {}

inline void  HALInitTimer4(HALTimerEvent evt)
{
	_halTimerEvent4 = evt;

	TCCR4A = 0x00;							// stetzt Statusregiser A Vom Timer eins auf null
	TCCR4B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TCCR4B |= (1<<CS42) | (1<<CS40);		// timer laeuft mit 1/1024 des CPU Takt.
}

////////////////////////////////////////////////////////

inline void HALStartTimer4(timer_t timer)
{
	TCNT4  = 0 - timer;  
	TIMSK4 |= (1<<TOIE4);					// Aktiviert Interrupt beim Overflow des Timers 1
	TCCR4B |= (1<<CS42) | (1<<CS40);		// timer laeuft mit 1/1024 des CPU Takt.
	TIFR4  |= (1<<TOV4);					// clear the overflow flag
}

////////////////////////////////////////////////////////

inline void HALStopTimer4()
{
	TCCR4B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TIMSK4 &= ~(1<<TOIE4);					// Deaktiviert Interrupt beim Overflow des Timers 1
	TCNT4=0;  
}  

////////////////////////////////////////////////////////

inline void  HALRemoveTimer5() {}

inline void  HALInitTimer5(HALTimerEvent evt)
{
	_halTimerEvent5 = evt;

	TCCR5A = 0x00;							// stetzt Statusregiser A Vom Timer eins auf null
	TCCR5B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TCCR5B |= (1<<CS52) | (1<<CS50);		// timer laeuft mit 1/1024 des CPU Takt.
}

////////////////////////////////////////////////////////

inline void HALStartTimer5(timer_t timer)
{
	TCNT5  = 0 - timer;  
	TIMSK5 |= (1<<TOIE5);					// Aktiviert Interrupt beim Overflow des Timers 1
	TCCR5B |= (1<<CS52) | (1<<CS50);		// timer laeuft mit 1/1024 des CPU Takt.
	TIFR5  |= (1<<TOV5);					// clear the overflow flag
}

////////////////////////////////////////////////////////

inline void HALStopTimer5()
{
	TCCR5B = 0x00;							// stetzt Statusregiser B Vom Timer eins auf null
	TIMSK5 &= ~(1<<TOIE5);					// Deaktiviert Interrupt beim Overflow des Timers 1
	TCNT5=0;  
}  

#endif

////////////////////////////////////////////////////////
// MSC
////////////////////////////////////////////////////////

#elif defined(_MSC_VER)

#include <arduino.h>
#include <avr/interrupt.h>
#include <avr/io.h>

#define TIMER0FREQUENCE		62500L
#define TIMER1FREQUENCE		2000000L
#define TIMER2FREQUENCE		62500L
#define TIMER3FREQUENCE		62500L
#define TIMER4FREQUENCE		62500L
#define TIMER5FREQUENCE		62500L

#define TIMEROVERHEAD		(0)				// decrease Timervalue for ISR overhead before set new timer

#define SREG_T unsigned char

#define DisableInterrupts()				cli()
#define EnableInterrupts()				sei()
#define GetSREG()						SREG
#define SetSREG(a)						SREG=a

#define READ digitalRead
#define WRITE digitalWrite
#define _WRITE_NC digitalWrite

#define __asm__(a)

inline void  HALInitTimer0(HALTimerEvent evt){ _halTimerEvent1 = evt; }
inline void  HALRemoveTimer0()			{}
inline void HALStartTimer0(timer_t)		{}
inline void HALStopTimer0()				{}

inline void  HALInitTimer1(HALTimerEvent evt){ _halTimerEvent1 = evt; }
inline void  HALRemoveTimer1()			{}
inline void HALStartTimer1(timer_t)		{}
inline void HALStopTimer1()				{}

inline void  HALInitTimer2(HALTimerEvent evt){ _halTimerEvent2 = evt; }
inline void  HALRemoveTimer2()			{}
inline void HALStartTimer2(timer_t)		{}
inline void HALStopTimer2()				{}

inline void  HALInitTimer3(HALTimerEvent evt){ _halTimerEvent3 = evt; }
inline void  HALRemoveTimer3()			{}
inline void HALStartTimer3(timer_t)		{}
inline void HALStopTimer3()				{}

inline void  HALInitTimer4(HALTimerEvent evt){ _halTimerEvent4 = evt; }
inline void  HALRemoveTimer4()			{}
inline void HALStartTimer4(timer_t)		{}
inline void HALStopTimer4()				{}

inline void  HALInitTimer5(HALTimerEvent evt){ _halTimerEvent5 = evt; }
inline void  HALRemoveTimer5()			{}
inline void HALStartTimer5(timer_t)		{}
inline void HALStopTimer5()				{}


////////////////////////////////////////////////////////

#else

ToDo;

#endif 
