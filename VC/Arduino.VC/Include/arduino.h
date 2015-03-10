////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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

#define ISR(a) void a(void)

#define __FlashStringHelper char

//#define max(a,b) ((a)>=(b)?(a):(b))
//#define min(a,b) ((a)<=(b)?(a):(b))

#define strcpy_P(a,b) strcpy(a,b)
#define strcat_P(a,b) strcat(a,b)

#define __FlashStringHelper char
#define F(a) a
#define PROGMEM 
inline char pgm_read_byte(const char* p) { return *p; }
typedef  const char* PGM_P;


typedef unsigned char uint8_t;
typedef char int8_t;

inline void analogWrite(short, int)	{};
inline void digitalWrite(short, short)	{};
inline unsigned char digitalRead(short /*pin*/) { return LOW; };
inline void pinMode(short, short)		{};
static unsigned char PORTA;
static unsigned char PORTB;
static unsigned char PORTC;
static unsigned char PORTD;
static unsigned char PORTE;
static unsigned char PORTF;
static unsigned char PORTK;
static unsigned char PORTL;
static unsigned char DDRL;
static unsigned char DDRD;
static unsigned char DDRB;
static unsigned char TCCR0A;
static unsigned char TCCR0B;
static unsigned short TCNT0;
static unsigned char TIMSK0;
static unsigned short TIFR0;
static unsigned short OCR0B;
static unsigned char TCCR1A;
static unsigned char TCCR1B;
static unsigned short TCNT1;
static unsigned char TIMSK1;
static unsigned short TIFR1;

static unsigned char PINA;
static unsigned char PINA0;
static unsigned char PINA1;
static unsigned char PINA2;
static unsigned char PINA3;
static unsigned char PINA4;
static unsigned char PINA5;
static unsigned char PINA6;
static unsigned char PINA7;

static unsigned char PINB;
static unsigned char PINB0;
static unsigned char PINB1;
static unsigned char PINB2;
static unsigned char PINB3;
static unsigned char PINB4;
static unsigned char PINB5;
static unsigned char PINB6;
static unsigned char PINB7;

static unsigned char PINC;
static unsigned char PINC0;
static unsigned char PINC1;
static unsigned char PINC2;
static unsigned char PINC3;
static unsigned char PINC4;
static unsigned char PINC5;
static unsigned char PINC6;
static unsigned char PINC7;

static unsigned char PIND;
static unsigned char PIND0;
static unsigned char PIND1;
static unsigned char PIND2;
static unsigned char PIND3;
static unsigned char PIND4;
static unsigned char PIND5;
static unsigned char PIND6;
static unsigned char PIND7;

static unsigned char PINE;
static unsigned char PINE0;
static unsigned char PINE1;
static unsigned char PINE2;
static unsigned char PINE3;
static unsigned char PINE4;
static unsigned char PINE5;
static unsigned char PINE6;
static unsigned char PINE7;

static unsigned char PINF;
static unsigned char PINF0;
static unsigned char PINF1;
static unsigned char PINF2;
static unsigned char PINF6;
static unsigned char PINF7;

static unsigned char PINJ;
static unsigned char PINJ0;
static unsigned char PINJ1;
static unsigned char PINJ2;
static unsigned char PINJ3;
static unsigned char PINJ4;
static unsigned char PINJ5;
static unsigned char PINJ6;
static unsigned char PINJ7;

static unsigned char PINK;
static unsigned char PINK0;

static unsigned char PINL;
static unsigned char PINL1;
static unsigned char PINL3;


static unsigned char SREG;

inline unsigned long   pgm_read_dword(const void* p) { return *(unsigned long*)p; }
inline unsigned short  pgm_read_word(const void* p) { return *(unsigned short*)p; }
inline  unsigned char  pgm_read_byte(const void* p) { return *(unsigned char*)p; }
inline  const void* pgm_read_ptr(const void* p)  { return *((void **) p); }

//extern unsigned int GetTickCount();
#pragma warning(suppress: 28159)
inline unsigned long millis() { return GetTickCount(); }

//extern void Sleep(unsigned int ms);
inline void delay(int ms) { Sleep(ms); }

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
	void print(long l)				{ printf("%i", l); };
	void print(unsigned long ul)	{ printf("%u", ul); };
	void print(const char*s)		{ printf("%s", s); };

	void println()					{ printf("\n"); };
	void println(unsigned int ui)	{ printf("%u\n", ui); };
	void println(int i)				{ printf("%i\n", i); };
	void println(long l)				{ printf("%i\n", l); };
	void println(unsigned long ul)	{ printf("%u\n", ul); };
	void println(const char*s)		{ printf("%s\n", s); };

	void begin(int )				{ };
	virtual int available()	 		{
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
										char ch;
										if (_istty)
										{
											ch = (char)_getch();
											if (ch == '\r')
												ch = '\n';
										}
										else
										{
											ch = (char)_fgetchar();
										}

										_putch(ch);
										return ch;
									}

private:

	bool _istty;
	void(*_pIdle)() = NULL;

};

class CSerial : public Stream
{
};


extern CSerial Serial;

#define __attribute__

