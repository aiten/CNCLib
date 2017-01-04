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


class CConfigEeprom : public CSingleton<CConfigEeprom>
{
private:

	unsigned short _eepromsizesize;
	const void* _defaulteeprom;

	bool _eepromvalid=false;
	bool _eepromcanwrite=false;

public:

	void Init(unsigned short eepromsizesize, const void* defaulteeprom, uint32_t eepromID);

	static uint32_t GetSlotU32(uint8_t slot);
	static uint32_t GetSlotU32(uint8_t slot, uint8_t ofs);
	static uint8_t  GetSlotU8(uint8_t slot, uint8_t ofs);

	static float GetSlotFloat(uint8_t slot);

	void PrintConfig();

	bool ParseConfig(class CParser*);

	uint32_t GetSlot32(uint8_t slot);
	uint8_t GetSlot8(uint8_t slot, uint8_t ofs);

private:

	void FlushConfig();
	void SetSlot32(uint8_t slot, uint32_t value);

public: 

	#define EEPROM_NUM_AXIS 4

	enum EConfigSlot
	{
		Signature = 0,
		MaxSize = 1,
		ScaleMmToMachine = MaxSize + EEPROM_NUM_AXIS,
		ReferenceType = ScaleMmToMachine + 1,
		RefMove,

		MaxStepRate,
		Acc,
		Dec,
		RefMoveStepRate
	};

	struct SCNCEeprom
	{
		uint32_t  signature;
		mm1000_t  maxsize[EEPROM_NUM_AXIS];

		float     ScaleMm1000ToMachine;

		uint8_t	  referenceType[EEPROM_NUM_AXIS];
		uint8_t   refmove[EEPROM_NUM_AXIS];

		uint32_t  maxsteprate;
		uint32_t  acc;
		uint32_t  dec;
		uint32_t  refmovesteprate;
	};
};

////////////////////////////////////////////////////////
