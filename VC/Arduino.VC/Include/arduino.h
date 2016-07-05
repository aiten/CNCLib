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

#pragma comment (lib, "StepperSystem.lib")

#include <stdio.h>
#include <ctype.h>
#define _USE_MATH_DEFINES
#include <math.h>
#include <conio.h>
#include <io.h>
#include <windows.h>
#include "trace.h"
#include <assert.h>

#include <functional>

#define OUTPUT 1
#define INPUT_PULLUP 1
#define INPUT 2
#define CS12 1
#define CS11 1
#define TOIE1 1
#define LOW 0
#define HIGH 1

#define TOIE0 0
#define OCIE0A 1
#define OCIE0B 2
#define OCF0B   2
#define OCF0A   1
#define TOV0    0

#define TOIE1 1
#define TOV1 0

#define CHANGE 1
#define FALLING 2
#define RISING 3

#define INTERNAL 3
#define DEFAULT 1
#define EXTERNAL 0

#define NOT_A_PIN 0
#define NOT_A_PORT 0

#define NOT_AN_INTERRUPT -1


#define ISR(a) void a(void)

#define __FlashStringHelper char

//#define max(a,b) ((a)>=(b)?(a):(b))
//#define min(a,b) ((a)<=(b)?(a):(b))

#define strcpy_P(a,b) strcpy(a,b)
#define strcat_P(a,b) strcat(a,b)
#define strcmp_P(a,b) strcmp(a,b)
#define strcasecmp_P(a,b) _stricmp(a,b)

#define __FlashStringHelper char
#define F(a) a
#define PROGMEM 
inline char pgm_read_byte(const char* p) { return *p; }
typedef  const char* PGM_P;


inline void attachInterrupt(uint8_t, void(*)(void), int /* mode */) {};
inline void detachInterrupt(uint8_t) {};

inline uint8_t digitalPinToInterrupt(uint8_t p) { return ((p) == 2 ? 0 : ((p) == 3 ? 1 : NOT_AN_INTERRUPT)); }

typedef unsigned char	uint8_t;
typedef signed char		int8_t;

inline void analogWrite(short, int)	{};
inline int analogRead(short) { return 0; };
inline void digitalWrite(short, short)	{};
inline uint8_t digitalRead(short /*pin*/) { return LOW; };
inline void pinMode(short, short)		{};

static uint8_t A0 = 0;


static uint8_t PORTA;
static uint8_t PORTB;
static uint8_t PORTC;
static uint8_t PORTD;
static uint8_t PORTE;
static uint8_t PORTF;
static uint8_t PORTK;
static uint8_t PORTL;
static uint8_t DDRL;
static uint8_t DDRD;
static uint8_t DDRB;
static uint8_t TCCR0A;
static uint8_t TCCR0B;
static unsigned short TCNT0;
static uint8_t TIMSK0;
static unsigned short TIFR0;
static unsigned short OCR0B;
static uint8_t TCCR1A;
static uint8_t TCCR1B;
static unsigned short TCNT1;
static uint8_t TIMSK1;
static unsigned short TIFR1;

static uint8_t PINA;
static uint8_t PINA0;
static uint8_t PINA1;
static uint8_t PINA2;
static uint8_t PINA3;
static uint8_t PINA4;
static uint8_t PINA5;
static uint8_t PINA6;
static uint8_t PINA7;

static uint8_t PINB;
static uint8_t PINB0;
static uint8_t PINB1;
static uint8_t PINB2;
static uint8_t PINB3;
static uint8_t PINB4;
static uint8_t PINB5;
static uint8_t PINB6;
static uint8_t PINB7;

static uint8_t PINC;
static uint8_t PINC0;
static uint8_t PINC1;
static uint8_t PINC2;
static uint8_t PINC3;
static uint8_t PINC4;
static uint8_t PINC5;
static uint8_t PINC6;
static uint8_t PINC7;

static uint8_t PIND;
static uint8_t PIND0;
static uint8_t PIND1;
static uint8_t PIND2;
static uint8_t PIND3;
static uint8_t PIND4;
static uint8_t PIND5;
static uint8_t PIND6;
static uint8_t PIND7;

static uint8_t PINE;
static uint8_t PINE0;
static uint8_t PINE1;
static uint8_t PINE2;
static uint8_t PINE3;
static uint8_t PINE4;
static uint8_t PINE5;
static uint8_t PINE6;
static uint8_t PINE7;

static uint8_t PINF;
static uint8_t PINF0;
static uint8_t PINF1;
static uint8_t PINF2;
static uint8_t PINF6;
static uint8_t PINF7;

static uint8_t PINJ;
static uint8_t PINJ0;
static uint8_t PINJ1;
static uint8_t PINJ2;
static uint8_t PINJ3;
static uint8_t PINJ4;
static uint8_t PINJ5;
static uint8_t PINJ6;
static uint8_t PINJ7;

static uint8_t PINK;
static uint8_t PINK0;

static uint8_t PINL;
static uint8_t PINL1;
static uint8_t PINL3;


static uint8_t SREG;

inline unsigned long   pgm_read_dword(const void* p) { return *(unsigned long*)p; }
inline unsigned short  pgm_read_word(const void* p) { return *(unsigned short*)p; }
inline  uint8_t  pgm_read_byte(const void* p) { return *(uint8_t*)p; }
inline  const void* pgm_read_ptr(const void* p)  { return *((void **) p); }

//extern unsigned int GetTickCount();
#pragma warning(suppress: 28159)
inline unsigned long millis() { return GetTickCount(); }

//extern void Sleep(unsigned int ms);
inline void delay(unsigned long ms) { Sleep(ms); }

#define STDIO 0

class Stream
{
public:
	Stream()
	{
		_istty = _isatty(STDIO)!=0;
	}

	void SetIdle(void(*pIdle)())	{ _pIdle = pIdle;  }

	void print(char c)				{ printf("%c", c); };
	void print(unsigned int ui)		{ printf("%u", ui); };
	void print(int i)				{ printf("%i", i); };
	void print(long l)				{ printf("%li", l); };
	void print(unsigned long ul)	{ printf("%lu", ul); };
	void print(const char*s)		{ printf("%s", s); };
	void print(float f)				{ printf("%f", f); };

	void println()					{ printf("\n"); };
	void println(unsigned int ui)	{ printf("%u\n", ui); };
	void println(int i)				{ printf("%i\n", i); };
	void println(long l)			{ printf("%li\n", l); };
	void println(unsigned long ul)	{ printf("%lu\n", ul); };
	void println(const char*s)		{ printf("%s\n", s); };
	void println(float f)			{ printf("%f\n", f); };

	void begin(int )				{ };
	virtual int available()	 		{
										if  (_last)
											return 1;

										if (!_istty)
										{
											if (feof(stdin) != 0)
											{
												_istty = true;
												return 0;
											}
										}
										if (!_istty || _kbhit())
											return 1; 
		
										if (_pIdle) _pIdle(); 
										return 0; 
									}
	virtual char read()				{
										char ch=_last;
										if (ch)
										{
											_last = 0;
										}
										else
										{
											if (_istty)
											{
												ch = (char)_getch();
												if (ch == '\r')
													_last = '\n';
											}
											else
											{
												ch = (char)_fgetchar();
											}
										}

										_putch(ch);
										return ch;
									}

private:

	void(*_pIdle)() = NULL;
	char _last=0;
	bool _istty;

};

class CSerial : public Stream
{
};


extern CSerial Serial;

#define __attribute__

