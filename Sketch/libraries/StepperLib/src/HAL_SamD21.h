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
// Due 32Bit
////////////////////////////////////////////////////////

#if defined(__SAMD21G18A__)

#include <itoa.h>

#undef pgm_read_ptr
inline  const void* pgm_read_ptr(const void* p) { return *((void **)p); }
inline  int pgm_read_int(const void* p) { return * ((const int*) p); }

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

#define TIMER1MIN			4
#define TIMER1MAX			0xffffffffl

#define TIMER2FREQUENCE		(F_CPU/TIMER2PRESCALE)
#define TIMER2PRESCALE      2			

#define TIMER3FREQUENCE		(F_CPU/TIMER3PRESCALE)
#define TIMER3PRESCALE      2			

#define TIMER4FREQUENCE		(F_CPU/TIMER4PRESCALE)
#define TIMER4PRESCALE      2			

#define TIMER5FREQUENCE		(F_CPU/TIMER5PRESCALE)
#define TIMER5PRESCALE      2			

#define MAXINTERRUPTSPEED	(65535/7)		// maximal possible interrupt rate => steprate_t

#define SPEED_MULTIPLIER_1	0
#define SPEED_MULTIPLIER_2	(MAXINTERRUPTSPEED*1)
#define SPEED_MULTIPLIER_3	(MAXINTERRUPTSPEED*2)
#define SPEED_MULTIPLIER_4	(MAXINTERRUPTSPEED*3)
#define SPEED_MULTIPLIER_5	(MAXINTERRUPTSPEED*4)
#define SPEED_MULTIPLIER_6	(MAXINTERRUPTSPEED*5)
#define SPEED_MULTIPLIER_7	(MAXINTERRUPTSPEED*6)

#define TIMEROVERHEAD		1				// decrease Timervalue for ISR overhead before set new timer

inline void CHAL::DisableInterrupts()		{ __disable_irq(); }
inline void CHAL::EnableInterrupts()		{ __enable_irq(); }

inline irqflags_t CHAL::GetSREG()			{ return cpu_irq_save(); }
inline void CHAL::SetSREG(irqflags_t a)		{ cpu_irq_restore(a); }

// TODO
// use CAN as backgroundworker thread
#define NVIC_EncodePriority(a,b,c) 0
#define IRQTYPE I2S_IRQn

inline void CHAL::BackgroundRequest()			{ NVIC_SetPendingIRQ(IRQTYPE); }
inline void CHAL::InitBackground(HALEvent evt)	{ NVIC_EnableIRQ(IRQTYPE);  NVIC_SetPriority(IRQTYPE, NVIC_EncodePriority(4, 7, 0)); _BackgroundEvent = evt; }

#define HALFastdigitalRead(a)	CHAL::digitalRead(a)
#define HALFastdigitalWrite(a,b) CHAL::digitalWrite(a,b)
#define HALFastdigitalWriteNC(a,b) CHAL::digitalWrite(a,b)
/*
inline void digitalWriteDirect(int pin, boolean val){
  if(val) g_APinDescription[pin].pPort -> PIO_SODR = g_APinDescription[pin].ulPin;
  else    g_APinDescription[pin].pPort -> PIO_CODR = g_APinDescription[pin].ulPin;
}

inline int digitalReadDirect(int pin){
  return !!(g_APinDescription[pin].pPort -> PIO_PDSR & g_APinDescription[pin].ulPin);
}
*/
inline uint8_t CHAL::digitalRead(pin_t pin)
{
  return (g_APinDescription[pin].pPort -> PIO_PDSR & g_APinDescription[pin].ulPin) ? HIGH : LOW;
//	return ::digitalReadDirect(pin);
}

inline void CHAL::digitalWrite(pin_t pin, uint8_t val)
{
  if(val) g_APinDescription[pin].pPort -> PIO_SODR = g_APinDescription[pin].ulPin;
  else    g_APinDescription[pin].pPort -> PIO_CODR = g_APinDescription[pin].ulPin;
  //	digitalWriteDirect(pin,lowOrHigh);
}

inline void CHAL::pinMode(pin_t pin, uint8_t mode)
{ 
	::pinMode(pin,mode); 
}

inline void CHAL::analogWrite8(pin_t pin, uint8_t val)
{
	::analogWrite(pin, val);
}

inline void CHAL::pinModeOutput(pin_t pin)
{
	::pinMode(pin, OUTPUT);
}

inline void CHAL::pinModeInputPullUp(pin_t pin)
{
	::pinMode(pin, INPUT_PULLUP);
}

// SAM3x has no eeprom => ignore

inline void CHAL::eeprom_write_dword(uint32_t *, uint32_t)
{
}

inline uint32_t CHAL::eeprom_read_dword(const uint32_t *)
{
	return 0;
}

inline uint8_t CHAL::eeprom_read_byte(const uint8_t *)
{
	return 0;
}
////////////////////////////////////////////////////////

inline void CHAL::delayMicroseconds(unsigned int usec)
{
	::delayMicroseconds(usec);
}

inline void CHAL::delayMicroseconds0500()
{
	// uint32_t n = usec * (VARIANT_MCK / 3000000);
	uint32_t n = 1 * (VARIANT_MCK / 3000000) / 2;
	asm volatile(
		"L_%=_delayMicroseconds:"       "\n\t"
		"subs   %0, #1"                 "\n\t"
		"bne    L_%=_delayMicroseconds" "\n"
		: "+r" (n) :
		);
}

inline void CHAL::delayMicroseconds0250()
{
	// uint32_t n = usec * (VARIANT_MCK / 3000000);
	uint32_t n = 1 * (VARIANT_MCK / 3000000) / 4;
	asm volatile(
		"L_%=_delayMicroseconds:"       "\n\t"
		"subs   %0, #1"                 "\n\t"
		"bne    L_%=_delayMicroseconds" "\n"
		: "+r" (n) :
		);
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

inline void  CHAL::InitTimer0(HALEvent evt)
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

////////////////////////////////////////////////////////

inline void  CHAL::InitTimer1(HALEvent evt)
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

inline void  CHAL::InitTimer3(HALEvent evt)
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

#endif 

