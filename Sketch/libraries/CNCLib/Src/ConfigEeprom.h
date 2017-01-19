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
	static uint8_t  GetConfigU8(eepromofs_t ofs )  { return (uint8_t)GetConfigU32(ofs); };
	static uint16_t  GetConfigU16(eepromofs_t ofs) { return (uint16_t)GetConfigU32(ofs); };

	static float GetConfigFloat(eepromofs_t);

	void PrintConfig();

	bool ParseConfig(class CParser*);

	uint32_t GetConfig32(eepromofs_t ofs);

private:

	void FlushConfig();
	void SetConfig32(eepromofs_t ofs, uint32_t value);

public: 

	enum EEpromInfo1
	{
		EEPROM_INFO_SPINDLE	= (1<<0),
		EEPROM_INFO_SPINDLE_ANALOG = (1<<1),
		EEPROM_INFO_SPINDLE_DIR = (1 << 2),
		EEPROM_INFO_COOLANT	= (1<<3),
		EEPROM_INFO_PROBE   = (1<<4),

		EEPROM_INFO_SD		= (1<<11),
		EEPROM_INFO_ROTATE	= (1<<10),

		EEPROM_INFO_HOLDRESUME = (1<<12),
		EEPROM_INFO_HOLD	= (1<<13),
		EEPROM_INFO_RESUME	= (1<<14),
		EEPROM_INFO_KILL	= (1<<15)
	};

	#define EPROM_SIGNATURE		0x21436501

	struct SCNCEeprom
	{
		uint32_t  signature;

		uint8_t	  num_axis;
		uint8_t	  used_axis;
		uint8_t	  offsetAxis;
		uint8_t	  sizeofAxis;

		uint32_t  info1;
		uint32_t  info2;

		uint8_t	  stepperdirections;		// bits for each axis, see CStepper::SetDirection
		uint8_t	  dummy2;
		uint8_t	  dummy3;
		uint8_t	  dummy4;

		uint16_t  maxspindlespeed;
		uint16_t  dummy5;

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
			
			uint8_t		dummy2;
			uint8_t		dummy3;

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
