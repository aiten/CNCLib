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

class CConfigEeprom : public CSingleton<CConfigEeprom>
{
private:

	unsigned short _eepromsizesize;
	const void* _defaulteeprom;

	bool _eepromvalid=false;
	bool _eepromcanwrite=false;

public:

	void Init(unsigned short eepromsizesize, const void* defaulteeprom, uint32_t eepromID);

	static uint32_t GetConfigU32(eepromofs_t);
	static uint8_t  GetConfigU8(eepromofs_t);

	static float GetConfigFloat(eepromofs_t);

	void PrintConfig();

	bool ParseConfig(class CParser*);

	uint32_t GetConfig32(eepromofs_t ofs);
	uint8_t GetConfig8(eepromofs_t ofs);

private:

	void FlushConfig();
	void SetConfig32(eepromofs_t ofs, uint32_t value);

public: 

	#define EEPROM_NUM_AXIS 4

	struct SCNCEeprom
	{
		uint32_t  signature;

		uint8_t   refmove[EEPROM_NUM_AXIS];

		uint32_t  maxsteprate;
		uint32_t  acc;
		uint32_t  dec;
		uint32_t  refmovesteprate;

		float     ScaleMm1000ToMachine;

		struct SAxisDefinitions
		{
			mm1000_t	size;

			uint8_t		referenceType;	// EReverenceType
			uint8_t		dummy1;
			uint8_t		dummy2;
			uint8_t		dummy3;

#ifndef REDUCED_SIZE

			uint32_t	maxsteprate;
			uint32_t	acc;
			uint32_t	dec;
			uint32_t	refmovesteprate;

			float		ScaleMm1000ToMachine;
#endif

		} axis[EEPROM_NUM_AXIS];
	};
};

////////////////////////////////////////////////////////
