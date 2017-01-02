////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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

// Clock should be AVR Compatible 2Mhz => 48Mhz/8(prescaler)/3(genclk)
#define TIMERCLOCKDIV		3	
#define TIMER0_CLKGEN		5
#define TIMER1_CLKGEN		5

#define TIMERBASEFREQUENCE	(F_CPU/TIMER1_CLKGEN)


#define TIMER0FREQUENCE		(TIMERBASEFREQUENCE/TIMER0PRESCALE)
#define TIMER0PRESCALE      1024			

// compatible to AVR => no 32 bit Timers
#if 1
#define TIMER1FREQUENCE		2000000L	
//#define TIMER1PRESCALE      (8*3)
#else
#define TIMER1FREQUENCE		(TIMERBASEFREQUENCE/TIMER1PRESCALE)
#define TIMER1PRESCALE      2			
#endif

#define TIMER1MIN			4
#define TIMER1MAX			0xffffffffl

#define MAXINTERRUPTSPEED	(65535/7)		// maximal possible interrupt rate => steprate_t

#define SPEED_MULTIPLIER_1	0
#define SPEED_MULTIPLIER_2	(MAXINTERRUPTSPEED*1)
#define SPEED_MULTIPLIER_3	(MAXINTERRUPTSPEED*2)
#define SPEED_MULTIPLIER_4	(MAXINTERRUPTSPEED*3)
#define SPEED_MULTIPLIER_5	(MAXINTERRUPTSPEED*4)
#define SPEED_MULTIPLIER_6	(MAXINTERRUPTSPEED*5)
#define SPEED_MULTIPLIER_7	(MAXINTERRUPTSPEED*6)

#define TIMEROVERHEAD		1				// decrease Timervalue for ISR overhead before set new timer

inline void CHAL::DisableInterrupts()		{ noInterrupts(); }
inline void CHAL::EnableInterrupts()		{ interrupts(); }

#ifndef interruptsStatus
#define interruptsStatus() __interruptsStatus()
static inline unsigned char __interruptsStatus(void) __attribute__((always_inline, unused));
static inline unsigned char __interruptsStatus(void)
{
	// See http://infocenter.arm.com/help/index.jsp?topic=/com.arm.doc.dui0497a/CHDBIBGJ.html
	return (__get_PRIMASK() ? 0 : 1);
}
#endif

inline irqflags_t CHAL::GetSREG()			{ return interruptsStatus(); }
inline void CHAL::SetSREG(irqflags_t a)		{ if (a != GetSREG()) { if (a) EnableInterrupts(); else DisableInterrupts(); } }

// TODO
// use CAN as backgroundworker thread
#define NVIC_EncodePriority(a,b,c) 0
#define IRQTYPE I2S_IRQn

inline void CHAL::BackgroundRequest()			{ NVIC_SetPendingIRQ(IRQTYPE); }
inline void CHAL::InitBackground(HALEvent evt)	{ NVIC_EnableIRQ(IRQTYPE);  NVIC_SetPriority(IRQTYPE, NVIC_EncodePriority(4, 7, 0)); _BackgroundEvent = evt; }

#define HALFastdigitalRead(a)	CHAL::digitalRead(a)
#define HALFastdigitalWrite(a,b) CHAL::digitalWrite(a,b)
#define HALFastdigitalWriteNC(a,b) CHAL::digitalWrite(a,b)

inline uint8_t CHAL::digitalRead(pin_t pin)
{
	return ::digitalRead(pin);
}

inline void CHAL::digitalWrite(pin_t pin, uint8_t val)
{
	::digitalWrite(pin,val);
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

// SAMD21 eeprom => ignore

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

inline void WaitForSyncGCLK()
{
	while (GCLK->STATUS.bit.SYNCBUSY == 1);
}

inline void InitGClk(int clkgen,int dest,int clockdiv)
{
	// Clock should be AVR Compatible 2Mhz => 48Mhz/24

	REG_GCLK_GENCTRL =    // GCLK_GENCTRL_DIVSEL |
		GCLK_GENCTRL_IDC |
		GCLK_GENCTRL_GENEN |
		GCLK_GENCTRL_SRC_DFLL48M |
		GCLK_CLKCTRL_GEN_GCLK1 |
		GCLK_GENCTRL_ID(clkgen);
	WaitForSyncGCLK();

	if (clockdiv > 1)
	{
		REG_GCLK_GENDIV = GCLK_GENDIV_DIV(clockdiv) |	// Divide the 48MHz clock source by divisor x: 48MHz/x=xxMHz
			GCLK_GENDIV_ID(clkgen);						// Select Generic Clock (GCLK) 
		WaitForSyncGCLK();
	}

	// Enable clock for TC
	REG_GCLK_CLKCTRL = (uint16_t)(GCLK_CLKCTRL_CLKEN |
		GCLK_CLKCTRL_GEN(clkgen) |
		GCLK_CLKCTRL_GEN_GCLK1 |
		GCLK_CLKCTRL_ID(dest));
	WaitForSyncGCLK();
}

////////////////////////////////////////////////////////

inline void WaitForSyncTC(TcCount16* TC)
{
	while (TC->STATUS.bit.SYNCBUSY == 1);
}

////////////////////////////////////////////////////////

inline TcCount16* GetTimer0Struct() { return (TcCount16*)TC5; }

inline void  CHAL::RemoveTimer0() {}

inline void CHAL::StartTimer0(timer_t delay)
{
	// do not use 32bit

	uint16_t timer_count = (uint16_t)delay;
	if (timer_count == 0) timer_count = 1;

	TcCount16* TC = GetTimer0Struct();

	TC->CTRLBSET.bit.CMD = TC_CTRLBCLR_CMD_RETRIGGER_Val;
	TC->CC[0].reg = timer_count;

	TC->CTRLA.reg |= TC_CTRLA_ENABLE;

	// dont care about wait (we are in ISR)
	// WaitForSyncTC(TC);
}

inline void  CHAL::InitTimer0(HALEvent evt)
{
	_TimerEvent0 = evt;

	InitGClk(TIMER0_CLKGEN, GCM_TC4_TC5, TIMERCLOCKDIV);

	TcCount16* TC = GetTimer0Struct();

	TC->CTRLA.reg &= ~TC_CTRLA_ENABLE;			// Disable
	WaitForSyncTC(TC);

	TC->CTRLA.reg = TC_CTRLA_MODE_COUNT16 |		// Set Timer counter Mode to 32 bits
		TC_CTRLA_WAVEGEN_MFRQ |					// use TOP
		TC_CTRLA_PRESCALER_DIV1024;				// Set perscaler
	WaitForSyncTC(TC);

	TC->CC[0].reg = 100;
	WaitForSyncTC(TC);

	// Interrupts
	TC->INTENSET.reg = 0;                     // disable all interrupts
	TC->INTENSET.bit.OVF = 1;                 // enable overfollow

	NVIC_DisableIRQ(TC5_IRQn);				  // Configure interrupt request
	NVIC_ClearPendingIRQ(TC5_IRQn);
	NVIC_SetPriority(TC5_IRQn, 0);
	NVIC_EnableIRQ(TC5_IRQn);
}

inline void CHAL::StopTimer0()
{
	//NVIC_DisableIRQ(ZEROTIMER3_IRQTYPE);

	//TODO...
}

////////////////////////////////////////////////////////

inline TcCount16* GetTimer1Struct() { return (TcCount16*)TC4; }

inline void  CHAL::RemoveTimer1() {}

inline void CHAL::StartTimer1OneShot(timer_t delay)
{
	// do not use 32bit => 2Mhz Timer as AVR

	uint16_t timer_count = (uint16_t) delay;
	if(timer_count == 0) timer_count = 1;

	TcCount16* TC = GetTimer1Struct();

	TC->CTRLBSET.bit.CMD = TC_CTRLBCLR_CMD_RETRIGGER_Val;
	TC->CC[0].reg = timer_count;
	
	TC->CTRLA.reg |= TC_CTRLA_ENABLE;

	// dont care about wait (we are in ISR)
	WaitForSyncTC(TC);

//	if (timer_count != 64516)
//	{
//		StepperSerial.print("TCS:");
//		StepperSerial.println(timer_count);
//	}
}

////////////////////////////////////////////////////////

inline void  CHAL::InitTimer1OneShot(HALEvent evt)
{
	InitGClk(TIMER1_CLKGEN, GCM_TC4_TC5, TIMERCLOCKDIV);

	_TimerEvent1 = evt;

	TcCount16* TC = GetTimer1Struct();

	TC->CTRLA.reg &= ~TC_CTRLA_ENABLE;			// Disable
	WaitForSyncTC(TC);

	TC->CTRLA.reg = TC_CTRLA_MODE_COUNT16 |		// Set Timer counter Mode to 32 bits
		TC_CTRLA_WAVEGEN_MFRQ |					// use TOP
		TC_CTRLA_PRESCALER_DIV8;				// Set perscaler
	WaitForSyncTC(TC);

	TC->CTRLBSET.bit.ONESHOT = 1;
	WaitForSyncTC(TC);

	TC->CC[0].reg = 100;
	WaitForSyncTC(TC);

	TC->CTRLBSET.bit.CMD = TC_CTRLBCLR_CMD_STOP_Val;
	WaitForSyncTC(TC);

		// Interrupts
	TC->INTENSET.reg = 0;                     // disable all interrupts
	TC->INTENSET.bit.OVF = 1;                 // enable overfollow

	NVIC_DisableIRQ(TC4_IRQn);				  // Configure interrupt request
	NVIC_ClearPendingIRQ(TC4_IRQn);
	NVIC_SetPriority(TC4_IRQn, 0);
	NVIC_EnableIRQ(TC4_IRQn);

//	// Enable TC
//	TC->CTRLA.reg |= TC_CTRLA_ENABLE;
//	WaitForSyncTC(TC);
}

////////////////////////////////////////////////////////

inline void CHAL::StopTimer1()
{
	NVIC_DisableIRQ(TC4_IRQn);

	TcCount16* TC = (TcCount16*)TC4;

	TC->CTRLA.reg &= ~TC_CTRLA_ENABLE;			// Disable
	WaitForSyncTC(TC);
}

////////////////////////////////////////////////////////

#endif 

