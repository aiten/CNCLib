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
*/
////////////////////////////////////////////////////////

#pragma once

//#define StepperSerial SerialUSB
#define StepperSerial Serial

////////////////////////////////////////////////////////

//#define _NO_LONG_MESSAGE
//#define _NO_DUMP

////////////////////////////////////////////////////////

#define X_AXIS 0
#define Y_AXIS 1
#define Z_AXIS 2
#define A_AXIS 3	// rotary around X
#define B_AXIS 4	// rotary around Y
#define C_AXIS 5	// rotary around Z
#define U_AXIS 6	// Relative axis parallel to U
#define V_AXIS 7	// Relative axis parallel to V
#define W_AXIS 8	// Relative axis parallel to W

////////////////////////////////////////////////////////

typedef unsigned char axis_t;	// type for "axis"

typedef signed   long sdist_t;	// tpye of stepper coord system (signed)
typedef unsigned long udist_t;	// tpye of stepper coord system (unsigned)

////////////////////////////////////////////////////////
//
// Stepper

#define REFERENCESTABLETIME	2			// time in ms for reference must not change (in Reference move) => signal bounce

#define IDLETIMER1VALUE		TIMER1VALUE(10)			// Idle timer value (stepper timer not moving)
#define TIMEOUTSETIDLE		1000					// set level after 1000ms

#define MAXSPEED			(65535)					// see range for mdist_t
#define MAXINTERRUPTSPEED	(65535/7)				// maximal possible interrupt rate
#define TIMER1VALUEMAXSPEED	TIMER1VALUE(MAXSPEED)

#define SPEED_MULTIPLIER_1			0
#define SPEED_MULTIPLIER_2			(MAXINTERRUPTSPEED*1)
#define SPEED_MULTIPLIER_3			(MAXINTERRUPTSPEED*2)
#define SPEED_MULTIPLIER_4			(MAXINTERRUPTSPEED*3)
#define SPEED_MULTIPLIER_5			(MAXINTERRUPTSPEED*4)
#define SPEED_MULTIPLIER_6			(MAXINTERRUPTSPEED*5)
#define SPEED_MULTIPLIER_7			(MAXINTERRUPTSPEED*6)

////////////////////////////////////////////////////////

#if defined(__AVR_ATmega1280__) || defined(__AVR_ATmega2560__)

// usual with Ramps1.4

#undef use32bit
#define use16bit

#define NUM_AXIS			5

#define STEPBUFFERSIZE		128		// size 2^x but not 256
#define MOVEMENTBUFFERSIZE	64

////////////////////////////////////////////////////////

#elif defined(__AVR_ATmega328P__)

// usual with SMC800

#undef use32bit
#define use16bit

#define STEPBUFFERSIZE		16		// size 2^x but not 256
#define MOVEMENTBUFFERSIZE	8

#undef NUM_AXIS
#define NUM_AXIS 4

#define REDUCED_SIZE
#define _NO_LONG_MESSAGE
#define _NO_DUMP

////////////////////////////////////////////////////////

#elif defined(__SAM3X8E__)

// usual with Ramps FD

#define use32bit
#undef use16bit

#define NUM_AXIS			6

#define STEPBUFFERSIZE		128		// size 2^x but not 256
#define MOVEMENTBUFFERSIZE	64

////////////////////////////////////////////////////////

#elif defined (_MSC_VER)

// test environment only

typedef unsigned long long uint64_t;

#undef use32bit
#define use16bit

//#undef use16bit
//#define use32bit

#define STEPBUFFERSIZE		16
#define MOVEMENTBUFFERSIZE	32

//#undef NUM_AXIS
#define NUM_AXIS 5

#undef REFERENCESTABLETIME
#define REFERENCESTABLETIME	0

#define MOVEMENTINFOSIZE	128

////////////////////////////////////////////////////////

#else
ToDo;
#endif

////////////////////////////////////////////////////////
// Global types and configuration
////////////////////////////////////////////////////////

#if defined(use16bit)

#define MAXSTEPSPERMOVE		0xffff			// split in moves
#define MAXACCDECSTEPS		(0x10000/4 -10)	// max stepps for acc and dec ramp ( otherwise overrun)

typedef unsigned short timer_t;			// timer tpye (16bit)
typedef unsigned short mdist_t;			// tpye for one movement (16bit)
typedef unsigned short steprate_t;		// tpye for speed (Hz), Steps/sec

#define mudiv	udiv
#define mudiv_t	udiv_t

#elif defined(use32bit)

#define MAXSTEPSPERMOVE		0xffffffff	// split in Moves
#define MAXACCDECSTEPS		0x1000000

typedef unsigned long timer_t;			// timer tpye (32bit)
typedef unsigned long mdist_t;			// tpye for one movement (32bit)
typedef unsigned long steprate_t;		// tpye for speed (Hz), Steps/sec

#define mudiv	ldiv
#define mudiv_t	ldiv_t

#endif

/////////////////////////////////////////////////////////////////////////////////////////////////

#ifdef _MSC_VER

#define EnumAsByte(a) a
#define debugvirtula virtual
#define stepperstatic 
#define stepperstatic_avr 

#else

#if defined(__AVR_ARCH__)
#define stepperstatic_avr static
#else
#define stepperstatic_avr 
#endif


#define stepperstatic static
#define stepperstatic_
#define EnumAsByte(a) unsigned char			// use a 8 bit enum (and not 16, see compiler output)
#define debugvirtula						// only used in msvc for debugging - not used on AVR controller 

#endif

/////////////////////////////////////////////////////////////////////////////////////////////////

typedef unsigned char axisArray_t;			// on bit per axis

#if NUM_AXIS > 3
typedef unsigned long DirCount_t;			// 4 bit for eache axis (0..7) count, 8 dirup, see DirCountAll_t
#define DirCountBytes 4
#else
typedef unsigned short DirCount_t;			// 4 bit for eache axis (0..7) count, 8 dirup 
#define DirCountBytes 2
#endif

#if NUM_AXIS > 7
#error "NUM_AXIS must be < 8"				// because of last dirCount_t used for info 
#endif

struct DirCountStepByte_t
{
	unsigned char count1 : 3;
	unsigned char dirUp1 : 1;

	unsigned char count2 : 3;
	unsigned char dirUp2 : 1;
};

struct DirCountInfoByte_t
{
	unsigned char count1 : 3;
	unsigned char dirUp1 : 1;

	unsigned char nocount : 1;		// do not count step (e.g. move for backlash
	unsigned char unused1 : 1;
	unsigned char unused2 : 1;
	unsigned char unused3 : 1;
};

struct DirCountByte_t
{
	DirCountStepByte_t byte[DirCountBytes - 1];
	DirCountInfoByte_t byteInfo;
};

/*
union DirCount_t
{
	DirCountByte_t		byte;
	DirCountAll_t		all;
};
*/

////////////////////////////////////////////////////////

#include "MessageStepperLib.h"

////////////////////////////////////////////////////////
