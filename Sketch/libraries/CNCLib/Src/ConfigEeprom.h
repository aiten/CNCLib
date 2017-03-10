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
*/
////////////////////////////////////////////////////////

#pragma once

////////////////////////////////////////////////////////

#include <StepperLib.h>
#include "ConfigurationCNCLib.h"

////////////////////////////////////////////////////////

#ifdef REDUCED_SIZE

typedef uint8_t eepromofs_t;

#else

typedef uint16_t eepromofs_t;

#endif

enum EReverenceType
{
	NoReference,
	ReferenceToMin,
	ReferenceToMax
};

class CConfigEeprom : public CSingleton<CConfigEeprom>
{
private:

	unsigned short _eepromsizesize;
	const void* _defaulteeprom;

	bool _eepromvalid=false;
	bool _eepromcanwrite=false;

public:

	CConfigEeprom() {};

	CConfigEeprom(unsigned short eepromsizesize, const void* defaulteeprom, uint32_t eepromID)
	{
		Init(eepromsizesize, defaulteeprom, eepromID);
	}

	void Init(unsigned short eepromsizesize, const void* defaulteeprom, uint32_t eepromID);

	static uint32_t GetConfigU32(eepromofs_t);
#if defined(__AVR_ARCH__)
	static uint8_t  GetConfigU8(eepromofs_t ofs) { return (uint8_t)GetConfigU32(ofs); };
	static uint16_t  GetConfigU16(eepromofs_t ofs) { return (uint16_t)GetConfigU32(ofs); };
#else
	static uint8_t  GetConfigU8(eepromofs_t ofs)
	{
		// must be dword alligned
		eepromofs_t diff = ofs % 4;
		uint32_t val = GetConfigU32(ofs - diff);
		return (val >> (diff * 8)) & 0xff;
	};
	static uint16_t  GetConfigU16(eepromofs_t ofs)
	{
		// must be dword alligned
		// must be in this 32bit value (diff can only be 0,1,2 and not 3)
		eepromofs_t diff = ofs % 4;
		uint32_t val = GetConfigU32(ofs-diff);
		return (val >> (diff * 8)) & 0xffff;
	}
#endif

	static float GetConfigFloat(eepromofs_t);

	void PrintConfig();

	bool ParseConfig(class CParser*);

	uint32_t GetConfig32(eepromofs_t ofs);

private:

	void FlushConfig();
	void SetConfig32(eepromofs_t ofs, uint32_t value);

public: 

	enum ECommandSyntax
	{
		GCodeBasic = 0,
		GCode = 1,
		HPGL = 7,           // max 3 bit
	};

	enum EEpromInfo1
	{
		NONE = 0,
		HAVE_SPINDLE	= (1<<0),
		HAVE_SPINDLE_ANALOG = (1<<1),
		HAVE_SPINDLE_DIR = (1 << 2),
		HAVE_COOLANT	= (1<<3),
		HAVE_PROBE   = (1<<4),

		IS_LASER	 = (1<<5),

		COMMANDSYNTAXBIT0 = (1 << 6),
		COMMANDSYNTAXBIT1 = (1 << 7),
		COMMANDSYNTAXBIT2 = (1 << 8),

		HAVE_EEPROM = (1<<9),
		HAVE_SD		= (1<<10),
		CAN_ROTATE	= (1<<11),

		HAVE_HOLDRESUME = (1<<12),
		HAVE_HOLD	= (1<<13),
		HAVE_RESUME	= (1<<14),
		HAVE_KILL	= (1<<15)
	};

	#define COMMANDSYNTAX_BIT	6
	#define COMMANDSYNTAX_LEN	2
	#define COMMANDSYNTAX_MASK  (CConfigEeprom::COMMANDSYNTAXBIT0+CConfigEeprom::COMMANDSYNTAXBIT1+CConfigEeprom::COMMANDSYNTAXBIT2)
	#define COMMANDSYNTAX_VALUE(a)	(((a)*(1<<COMMANDSYNTAX_BIT))&COMMANDSYNTAX_MASK)
	#define COMMANDSYNTAX_CLEAR(a)	((a)&~COMMANDSYNTAX_MASK)

	#define EPROM_SIGNATURE		0x21436501

	struct SCNCEeprom
	{
		uint32_t  signature;

		uint8_t	  num_axis;
		uint8_t	  used_axis;
		uint8_t	  offsetAxis;
		uint8_t	  sizeofAxis;

		uint16_t  info1a;
		uint16_t  info1b;
		uint32_t  info2;

		uint8_t	  stepperdirections;		// bits for each axis, see CStepper::SetDirection
		uint8_t	  dummy2;
		uint8_t	  dummy3;
		uint8_t	  spindlefadetime;

		uint16_t  maxspindlespeed;
		uint16_t  jerkspeed;

		uint32_t  maxsteprate;
		uint16_t  acc;
		uint16_t  dec;
		uint32_t  refmovesteprate;
		uint32_t  moveAwayFromRefernece;

		float     StepsPerMm1000;

		struct SAxisDefinitions
		{
			mm1000_t	size;

			uint8_t		referenceType;		// EReverenceType
			uint8_t		refmoveSequence;
			
			uint8_t		referenceValue_min;
			uint8_t		referenceValue_max;

#ifndef REDUCED_SIZE

			uint32_t	maxsteprate;
			uint32_t	acc;
			uint32_t	dec;
			uint32_t	refmovesteprate;

			float		ScaleMm1000ToMachine;
#endif

		} axis[NUM_AXIS];
	};
};

////////////////////////////////////////////////////////
