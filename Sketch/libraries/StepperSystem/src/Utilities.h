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

//////////////////////////////////////////

#ifndef CRITICAL_SECTION_START
#define CRITICAL_SECTION_START  irqflags_t _sreg = SREG; cli();
#define CRITICAL_SECTION_END    SREG = _sreg;
#endif //CRITICAL_SECTION_START

#endif


//////////////////////////////////////////

class CMm1000
{
private: 
	mm1000_t	_value;

public:

	CMm1000()	{ _value = 0; }
	CMm1000(mm1000_t v)	{ _value = v; }

	static char* ToString(mm1000_t v, char*tmp, unsigned char precision, unsigned char scale);
	static char* ToString(mm1000_t v, char*tmp, unsigned char scale);
};

////////////////////////////////////////////////////////

extern unsigned char ToPrecision10(unsigned long);
extern unsigned char ToPrecision10(unsigned short);

extern unsigned char ToPrecision2(unsigned long);
extern unsigned char ToPrecision2(unsigned short);

////////////////////////////////////////////////////////

inline unsigned int RoundMulDivUInt(unsigned int v, unsigned int m, unsigned int d)
{
	return (unsigned int)(((unsigned long)(v)* (unsigned long)(m)+(unsigned long)(d / 2)) / d);
}
/*
inline unsigned short RoundMulDivU1632(unsigned short v, unsigned short m, unsigned short d)
{
return (unsigned short) (((unsigned long)(v) * (unsigned long)(m) + (unsigned long)(d/2)) / d);
}
*/
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

template<class T> bool IsBitSet(T t, unsigned char bit)				{ return (t & (((T)1) << bit)) != 0; };
template<class T> bool IsBitClear(T t, unsigned char bit)			{ return (t & (((T)1) << bit)) == 0; };
template<class T> void BitSet(T& t, unsigned char bit)				{ t |= ((T)1) << bit; };
template<class T> void BitClear(T& t, unsigned char bit)				{ t &= ~(((T)1) << bit); };

////////////////////////////////////////////////////////

unsigned long _ulsqrt_round(unsigned long val);
unsigned long _ulsqrt(unsigned long val);

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
