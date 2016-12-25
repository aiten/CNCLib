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

//////////////////////////////////////////

#define CAT(x, y) CAT_(x, y)
#define CAT_(x, y) x ## y

////////////////////////////////////////////////////////

#include "HAL.h"

//////////////////////////////////////////

extern uint8_t ToPrecisionU10(unsigned long);
extern uint8_t ToPrecisionU10(unsigned short);
extern uint8_t ToPrecisionS10(long v);

extern uint8_t ToPrecisionU2(unsigned long);
extern uint8_t ToPrecisionU2(unsigned short);
extern uint8_t ToPrecisionS2(long);

////////////////////////////////////////////////////////

template<class T> bool IsBitSet(T t, uint8_t bit)				{ return (t & (((T)1) << bit)) != 0; };
template<class T> bool IsBitClear(T t, uint8_t bit)			{ return (t & (((T)1) << bit)) == 0; };
template<class T> void BitSet(T& t, uint8_t bit)				{ t |= ((T)1) << bit; };
template<class T> void BitClear(T& t, uint8_t bit)			{ t &= ~(((T)1) << bit); };

////////////////////////////////////////////////////////

inline unsigned int RoundMulDivUInt(unsigned int v, unsigned int m, unsigned int d)
{
	return (unsigned int)(((unsigned long)(v)* (unsigned long)(m)+(unsigned long)(d / 2)) / d);
}

inline uint8_t RoundMulDivU8(uint8_t v, uint8_t m, uint8_t d)
{
	return (uint8_t) (((unsigned int)v * m + d / 2) / d);
}

inline unsigned long RoundMulDivU32(unsigned long v, unsigned long m, unsigned long d)
{
	return (v * m + d / 2) / d;
}

inline long RoundMulDivI32(long v, long m, long d)
{
	return (v * m + d / 2) / d;
}

inline unsigned long MulDivU32(unsigned long v, unsigned long m, unsigned long d)
{
	return (v * m) / d;
}

inline long MulDivI32(long v, long m, long d)
{
	return (v * m) / d;
}

////////////////////////////////////////////////////////

unsigned long _ulsqrt_round(unsigned long val);
unsigned long _ulsqrt(unsigned long val);

////////////////////////////////////////////////////////

#if defined(_MSC_VER) || defined(__SAM3X8E__) || defined(__SAMD21G18A__)

typedef struct _udiv_t {
	unsigned short quot;
	unsigned short rem;
} udiv_t;

inline udiv_t udiv(unsigned short __num, unsigned short __denom)
{
	div_t d = div(__num, __denom);
	udiv_t ud;
	ud.quot = (unsigned short)d.quot;
	ud.rem = (unsigned short)d.rem;
	return ud;
}

#else

typedef struct _udiv_t {
	unsigned short quot;
	unsigned short rem;
} udiv_t;

extern udiv_t udiv(unsigned short __num, unsigned short __denom) __asm__("__udivmodhi4") __ATTR_CONST__;

#endif

////////////////////////////////////////////////////////

template<typename T, uint8_t sz> void DumpArray(const __FlashStringHelper* head, const T pos[sz], bool newline)
{
	if (head != NULL)
	{
		StepperSerial.print(head);
		StepperSerial.print(F("="));
	}
	StepperSerial.print(pos[0]);
	for (uint8_t i = 1; i < sz; i++)
	{
		StepperSerial.print(F(","));
		StepperSerial.print(pos[i]);
	}
	if (newline)
		StepperSerial.println();
	else
		StepperSerial.print(F(":"));
}

////////////////////////////////////////////////////////

template<typename T> void DumpType(const __FlashStringHelper* head, T value, bool newline)
{
	if (head != NULL)
	{
		StepperSerial.print(head);
		StepperSerial.print(F("="));
	}
	StepperSerial.print(value);

	if (newline)
		StepperSerial.println();
	else
		StepperSerial.print(F(":"));
}

////////////////////////////////////////////////////////
