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

#include <arduino.h>
#include "ConfigurationStepperLib.h"

////////////////////////////////////////////////////////

#define TIMER0VALUE(freq)	((timer_t)((unsigned long)TIMER0FREQUENCE/(unsigned long)freq))
#define TIMER1VALUE(freq)	((timer_t)((unsigned long)TIMER1FREQUENCE/(unsigned long)freq))
#define TIMER2VALUE(freq)	((timer_t)((unsigned long)TIMER2FREQUENCE/(unsigned long)freq))
#define TIMER3VALUE(freq)	((timer_t)((unsigned long)TIMER3FREQUENCE/(unsigned long)freq))
#define TIMER4VALUE(freq)	((timer_t)((unsigned long)TIMER4FREQUENCE/(unsigned long)freq))
#define TIMER5VALUE(freq)	((timer_t)((unsigned long)TIMER5FREQUENCE/(unsigned long)freq))

////////////////////////////////////////////////////////

#if defined(__SAM3X8E__)

#else

#define irqflags_t unsigned char

#endif


////////////////////////////////////////////////////////

class CHAL
{
public:

	typedef void(*TimerEvent)();

	// min 8 bit 
	static void InitTimer0(TimerEvent evt);
	static void RemoveTimer0();
	static void StartTimer0(timer_t timer);
	static void StopTimer0();

	// min 16 bit
	static void InitTimer1(TimerEvent evt);
	static void RemoveTimer1();
	static void StartTimer1(timer_t timer);
	static void StopTimer1();
	static void NestedTimer1();

	// 8 bit
	static void InitTimer2(TimerEvent evt);
	static void RemoveTimer2();
	static void StartTimer2(timer_t timer);
	static void StopTimer2();

	static TimerEvent _TimerEvent0;
	static TimerEvent _TimerEvent1;
	static TimerEvent _TimerEvent2;

#if !defined( __AVR_ATmega328P__)

	// min 16 bit
	static void InitTimer3(TimerEvent evt);
	static void RemoveTimer3();
	static void StartTimer3(timer_t timer);
	static void StopTimer3();

	// min 16 bit
	static void InitTimer4(TimerEvent evt);
	static void RemoveTimer4();
	static void StartTimer4(timer_t timer);
	static void StopTimer4();

	// min 16 bit
	static void InitTimer5(TimerEvent evt);
	static void RemoveTimer5();
	static void StartTimer5(timer_t timer);
	static void StopTimer5();

	static TimerEvent _TimerEvent3;
	static TimerEvent _TimerEvent4;
	static TimerEvent _TimerEvent5;

#endif

	static inline void DisableInterrupts();
	static inline void EnableInterrupts();

	static inline irqflags_t GetSREG();
	static inline void SetSREG(irqflags_t);

	static inline void pinMode(unsigned char pin, unsigned char mode);

	static void digitalWrite(unsigned char pin, unsigned char lowOrHigh);
	static void digitalWriteNC(unsigned char pin, unsigned char lowOrHigh);		// no disableIRQ
	static unsigned char digitalRead(unsigned char pin);

};

//////////////////////////////////////////

class CCriticalRegion
{
private:
	irqflags_t _sreg;

public:

	inline CCriticalRegion() :_sreg(CHAL::GetSREG()) {  CHAL::DisableInterrupts(); };
	inline ~CCriticalRegion()	{ CHAL::SetSREG(_sreg); }
};

////////////////////////////////////////////////////////
// Due 32Bit
////////////////////////////////////////////////////////

#if defined(__SAM3X8E__)

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

inline void CHAL::DisableInterrupts()		{	cpu_irq_disable(); }
inline void CHAL::EnableInterrupts()		{	cpu_irq_enable(); }

inline irqflags_t CHAL::GetSREG()			{ return cpu_irq_save(); }
inline void CHAL::SetSREG(irqflags_t a)		{ cpu_irq_restore(a); }

#define HALFastdigitalRead(a)	CHAL::digitalRead(a)
#define HALFastdigitalWrite(a,b) CHAL::digitalWrite(a,b)
#define HALFastdigitalWriteNC(a,b) CHAL::digitalWriteNC(a,b)
/*
inline void digitalWriteDirect(int pin, boolean val){
  if(val) g_APinDescription[pin].pPort -> PIO_SODR = g_APinDescription[pin].ulPin;
  else    g_APinDescription[pin].pPort -> PIO_CODR = g_APinDescription[pin].ulPin;
}

inline int digitalReadDirect(int pin){
  return !!(g_APinDescription[pin].pPort -> PIO_PDSR & g_APinDescription[pin].ulPin);
}
*/
inline unsigned char CHAL::digitalRead(uint8_t pin)
{
  return (g_APinDescription[pin].pPort -> PIO_PDSR & g_APinDescription[pin].ulPin) ? HIGH : LOW;
//	return ::digitalReadDirect(pin);
}

inline void CHAL::digitalWrite(uint8_t pin, uint8_t val)
{
  if(val) g_APinDescription[pin].pPort -> PIO_SODR = g_APinDescription[pin].ulPin;
  else    g_APinDescription[pin].pPort -> PIO_CODR = g_APinDescription[pin].ulPin;
  //	digitalWriteDirect(pin,lowOrHigh);
}

inline void CHAL::digitalWriteNC(uint8_t pin, uint8_t val)
{
  if(val) g_APinDescription[pin].pPort -> PIO_SODR = g_APinDescription[pin].ulPin;
  else    g_APinDescription[pin].pPort -> PIO_CODR = g_APinDescription[pin].ulPin;
//	digitalWriteDirect(pin,lowOrHigh);
}

inline void CHAL::pinMode(unsigned char pin, unsigned char mode)			
{ 
	::pinMode(pin,mode); 
}

////////////////////////////////////////////////////////

#define DUETIMER1_TC					TC2
#define DUETIMER1_CHANNEL				2
#define DUETIMER1_IRQTYPE				((IRQn_Type) ID_TC8)

#define DUETIMER3_TC					TC2
#define DUETIMER3_CHANNEL				0
#define DUETIMER3_IRQTYPE				((IRQn_Type) ID_TC6)

////////////////////////////////////////////////////////

inline void  CHAL::RemoveTimer0() {}

inline void CHAL::StartTimer0(timer_t delay)
{
	StartTimer3(delay);
}

inline void  CHAL::InitTimer0(TimerEvent evt)
{
	InitTimer3(evt);
}

inline void  CHAL::RemoveTimer1() {}

inline void CHAL::StartTimer1(timer_t delay)
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

inline void CHAL::NestedTimer1()
{
	// reenable IRQ during ISR
	EnableInterrupts();
}

////////////////////////////////////////////////////////

inline void  CHAL::InitTimer1(TimerEvent evt)
{
	_TimerEvent1 = evt;

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

inline void CHAL::StopTimer1()
{
	NVIC_DisableIRQ(DUETIMER1_IRQTYPE);
	TC_Stop(DUETIMER1_TC, DUETIMER1_CHANNEL);
}  

////////////////////////////////////////////////////////

inline void  CHAL::RemoveTimer3() {}

inline void CHAL::StartTimer3(timer_t timer_count)
{
	if (timer_count == 0) timer_count = 1;
	TC_SetRC(DUETIMER3_TC, DUETIMER3_CHANNEL, timer_count);
	TC_Start(DUETIMER3_TC, DUETIMER3_CHANNEL);
}

////////////////////////////////////////////////////////

inline void  CHAL::InitTimer3(TimerEvent evt)
{
	_TimerEvent3 = evt;

	pmc_enable_periph_clk(DUETIMER3_IRQTYPE);
	NVIC_SetPriority(DUETIMER3_IRQTYPE, NVIC_EncodePriority(4, 3, 0));

	TC_Configure(DUETIMER3_TC, DUETIMER3_CHANNEL, TC_CMR_WAVSEL_UP_RC | TC_CMR_WAVE | TC_CMR_TCCLKS_TIMER_CLOCK1);

	TC_SetRC(DUETIMER3_TC, DUETIMER3_CHANNEL, 100000L);
	TC_Start(DUETIMER3_TC, DUETIMER3_CHANNEL);

	DUETIMER3_TC->TC_CHANNEL[DUETIMER3_CHANNEL].TC_IER = TC_IER_CPCS;
	DUETIMER3_TC->TC_CHANNEL[DUETIMER3_CHANNEL].TC_IDR = ~TC_IER_CPCS;
	NVIC_EnableIRQ(DUETIMER3_IRQTYPE);
}

////////////////////////////////////////////////////////

inline void CHAL::StopTimer3()
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

#include <avr/interrupt.h>
#include <avr/io.h>

#include "fastio.h"

////////////////////////////////////////////////////////

inline void CHAL::DisableInterrupts()	{	cli(); }
inline void CHAL::EnableInterrupts()	{	sei(); }

inline irqflags_t CHAL::GetSREG()		{ return SREG; }
inline void CHAL::SetSREG(irqflags_t a)	{ SREG=a; }

inline void  CHAL::RemoveTimer0() {}

inline void  CHAL::InitTimer0(TimerEvent evt)
{
	// shared with millis!
	_TimerEvent0 = evt;
}

////////////////////////////////////////////////////////

inline void CHAL::StartTimer0(timer_t timer)
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

inline void  CHAL::InitTimer1(TimerEvent evt)
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

inline void CHAL::NestedTimer1()
{
	// reenable IRQ during ISR
	EnableInterrupts();
}

////////////////////////////////////////////////////////

inline void  CHAL::RemoveTimer2() {}

////////////////////////////////////////////////////////

inline void  CHAL::InitTimer2(TimerEvent evt)
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

inline void  CHAL::InitTimer3(TimerEvent evt)
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

inline void  CHAL::InitTimer4(TimerEvent evt)
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

inline void  CHAL::InitTimer5(TimerEvent evt)
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

inline void CHAL::digitalWrite(uint8_t pin, uint8_t lowOrHigh)
{
	::digitalWrite(pin,lowOrHigh);
}

inline void CHAL::digitalWriteNC(uint8_t pin, uint8_t lowOrHigh)
{
	::digitalWrite(pin,lowOrHigh);
}

inline unsigned char CHAL::digitalRead(uint8_t pin)
{
	return ::digitalRead(pin);
}

inline void CHAL::pinMode(unsigned char pin, unsigned char mode)			
{ 
	::pinMode(pin,mode); 
}


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

inline void CHAL::DisableInterrupts()	{	cli(); }
inline void CHAL::EnableInterrupts()	{	sei(); }

inline irqflags_t CHAL::GetSREG()				{ return SREG; }
inline void CHAL::SetSREG(irqflags_t a)			{ SREG=a; }

#define __asm__(a)

inline void CHAL::InitTimer0(TimerEvent evt){ _TimerEvent0 = evt; }
inline void CHAL::RemoveTimer0()			{}
inline void CHAL::StartTimer0(timer_t)		{}
inline void CHAL::StopTimer0()				{}

inline void CHAL::InitTimer1(TimerEvent evt){ _TimerEvent1 = evt; }
inline void CHAL::RemoveTimer1()			{}
inline void CHAL::StartTimer1(timer_t)		{}
inline void CHAL::StopTimer1()				{}
inline void CHAL::NestedTimer1()			{}

inline void CHAL::InitTimer2(TimerEvent evt){ _TimerEvent2 = evt; }
inline void CHAL::RemoveTimer2()			{}
inline void CHAL::StartTimer2(timer_t)		{}
inline void CHAL::StopTimer2()				{}

inline void CHAL::InitTimer3(TimerEvent evt){ _TimerEvent3 = evt; }
inline void CHAL::RemoveTimer3()			{}
inline void CHAL::StartTimer3(timer_t)		{}
inline void CHAL::StopTimer3()				{}

inline void CHAL::InitTimer4(TimerEvent evt){ _TimerEvent4 = evt; }
inline void CHAL::RemoveTimer4()			{}
inline void CHAL::StartTimer4(timer_t)		{}
inline void CHAL::StopTimer4()				{}

inline void CHAL::InitTimer5(TimerEvent evt){ _TimerEvent5 = evt; }
inline void CHAL::RemoveTimer5()			{}
inline void CHAL::StartTimer5(timer_t)		{}
inline void CHAL::StopTimer5()				{}

#define HALFastdigitalRead(a) CHAL::digitalRead(a)
#define HALFastdigitalWrite(a,b) CHAL::digitalWrite(a,b)
#define HALFastdigitalWriteNC(a,b) CHAL::digitalWriteNC(a,b)

inline void CHAL::digitalWrite(uint8_t pin, uint8_t lowOrHigh)
{
	::digitalWrite(pin,lowOrHigh);
}

inline void CHAL::digitalWriteNC(uint8_t pin, uint8_t lowOrHigh)
{
	::digitalWrite(pin,lowOrHigh);
}

inline unsigned char CHAL::digitalRead(uint8_t pin)
{
	return ::digitalRead(pin);
}

inline void CHAL::pinMode(unsigned char pin, unsigned char mode)			
{ 
	::pinMode(pin,mode); 
}

////////////////////////////////////////////////////////

#else

ToDo;

#endif 

