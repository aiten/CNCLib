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

//////////////////////////////////////////

#include "HAL.h"

//////////////////////////////////////////

#ifdef _MSC_VER

#else

#define strcpy_s strcpy
#define strncpy_s strncpy

#define _itoa(a,b,c) itoa(a,b,c)
#define _ltoa(a,b,c) ltoa(a,b,c)

#ifndef CRITICAL_SECTION_START
#define CRITICAL_SECTION_START  irqflags_t _sreg = SREG; cli();
#define CRITICAL_SECTION_END    SREG = _sreg;
#endif //CRITICAL_SECTION_START

#endif

//////////////////////////////////////////

template <class T>
class CSingleton
{
private:
	static T* _instance;

public:

	CSingleton()
	{
		_instance = (T*) this;
	}

	static T* GetInstance()	{ return _instance; }
};

//////////////////////////////////////////

template <class T>
class CRememberOld
{
private:

	T* _Value;
	T  _oldValue;

public:

	CRememberOld(T* remember, T newValue)
	{
		_Value = remember;
		_oldValue = *remember;
		*remember = newValue;
	}

	~CRememberOld()
	{
		*_Value = _oldValue;
	}
};

//////////////////////////////////////////

class CCriticalRegion
{
private:
	SREG_T _sreg;

public:

	CCriticalRegion()	{ _sreg = GetSREG(); DisableInterrupts(); }
	~CCriticalRegion()	{ SetSREG(_sreg); }
};

//////////////////////////////////////////

class CMm1000
{
private:
	mm1000_t	_value;

public:

	CMm1000()	{ _value = 0; }
	CMm1000(mm1000_t v)	{ _value = v; }

	static char* ToString(mm1000_t v, char*tmp, unsigned char precision, unsigned char scale);	// right aligned
	static char* ToString(mm1000_t v, char*tmp, unsigned char scale);

	char*ToString(char*tmp, unsigned char precision, unsigned char scale)	{ return ToString(_value, tmp, precision, scale); } // right aligned
	char*ToString(char*tmp, unsigned char scale)							{ return ToString(_value, tmp, scale); }
};

////////////////////////////////////////////////////////

class CSDist
{
private:
	sdist_t	_value;

public:

	CSDist()	{ _value = 0; }
	CSDist(sdist_t v)	{ _value = v; }

	static char* ToString(sdist_t v, char*tmp, unsigned char precision);		// right aligned
};

//////////////////////////////////////////

extern unsigned char ToPrecisionU10(unsigned long);
extern unsigned char ToPrecisionU10(unsigned short);

extern unsigned char ToPrecisionU2(unsigned long);
extern unsigned char ToPrecisionU2(unsigned short);

////////////////////////////////////////////////////////

template<class T> bool IsBitSet(T t, unsigned char bit)				{ return (t & (((T)1) << bit)) != 0; };
template<class T> bool IsBitClear(T t, unsigned char bit)			{ return (t & (((T)1) << bit)) == 0; };
template<class T> void BitSet(T& t, unsigned char bit)				{ t |= ((T)1) << bit; };
template<class T> void BitClear(T& t, unsigned char bit)			{ t &= ~(((T)1) << bit); };

////////////////////////////////////////////////////////

inline unsigned int RoundMulDivUInt(unsigned int v, unsigned int m, unsigned int d)
{
	return (unsigned int)(((unsigned long)(v)* (unsigned long)(m)+(unsigned long)(d / 2)) / d);
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

#if defined(_MSC_VER) || defined(__SAM3X8E__)

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

template<typename T, unsigned char sz> void DumpArray(const __FlashStringHelper* head, const T pos[sz], bool newline)
{
	if (head != NULL)
	{
		StepperSerial.print(head);
		StepperSerial.print(F("="));
	}
	StepperSerial.print(pos[0]);
	for (unsigned char i = 1; i < sz; i++)
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
