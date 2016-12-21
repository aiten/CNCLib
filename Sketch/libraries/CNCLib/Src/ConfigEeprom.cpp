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
*/
////////////////////////////////////////////////////////

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <StepperLib.h>

#include "Parser.h"
#include "ConfigEeprom.h"

////////////////////////////////////////////////////////////

template<> CConfigEeprom* CSingleton<CConfigEeprom>::_instance = NULL;

////////////////////////////////////////////////////////////

#if defined(_MSC_VER)

static uint32_t EEPROMBASEADRUINT32_X[2048] = { 0 };
static uint32_t* EEPROMBASEADRUINT32 = EEPROMBASEADRUINT32_X;

#else

#include <alloca.h>

#define EEPROMBASEADRUINT32 (uint32_t*) NULL

#endif

////////////////////////////////////////////////////////////

void CConfigEeprom::Init(unsigned short eepromsizesize, const void* defaulteeprom, uint32_t eepromID)
{
	_eepromsizesize = eepromsizesize;
	_defaulteeprom = defaulteeprom;

	_eepromvalid = true;
	_eepromvalid = GetSlotU32(0) == eepromID;
}

////////////////////////////////////////////////////////////

uint32_t CConfigEeprom::GetSlotU32(uint8_t slot) { return GetInstance()->GetSlot32(slot); }
uint32_t CConfigEeprom::GetSlotU32(uint8_t slot, uint8_t ofs) { return GetInstance()->GetSlot32(slot+ofs); };

float CConfigEeprom::GetSlotFloat(uint8_t slot) { union { uint32_t u; float f; } v; v.u = GetInstance()->GetSlot32(slot); return v.f; }

uint8_t CConfigEeprom::GetSlotU8(uint8_t slot, uint8_t ofs) { return GetInstance()->GetSlot8(slot,ofs); };

////////////////////////////////////////////////////////////

uint32_t CConfigEeprom::GetSlot32(uint8_t slot)
{
	if (_eepromvalid)	return eeprom_read_dword(EEPROMBASEADRUINT32+slot);
	return pgm_read_dword(((uint32_t*) _defaulteeprom)+slot);
}
////////////////////////////////////////////////////////////

uint8_t CConfigEeprom::GetSlot8(uint8_t slot, uint8_t ofs)
{
	if (_eepromvalid)	return eeprom_read_byte((uint8_t*) EEPROMBASEADRUINT32 + slot*sizeof(uint32_t) + ofs);
	return pgm_read_byte((uint8_t*)_defaulteeprom + slot * sizeof(uint32_t) + ofs);
}

////////////////////////////////////////////////////////////

void CConfigEeprom::SetSlot32(uint8_t slot, uint32_t value)
{
	eeprom_write_dword(EEPROMBASEADRUINT32 + slot, value);
}

////////////////////////////////////////////////////////////

void CConfigEeprom::FlushConfig()
{
	for (uint8_t slot = 0; slot < _eepromsizesize / sizeof(uint32_t); slot++)
	{
		SetSlot32(slot, GetSlot32(slot));
	}
	_eepromvalid = true;
}

////////////////////////////////////////////////////////////

void CConfigEeprom::PrintConfig()
{
	for (uint8_t slot = 0; slot < _eepromsizesize / sizeof(uint32_t); slot++)
	{
		uint32_t val = GetSlot32(slot);
		StepperSerial.print('$'); StepperSerial.print(slot); StepperSerial.print('='); 
		StepperSerial.print(val); StepperSerial.print('('); StepperSerial.print(val, HEX); StepperSerial.println(')');
	}
}

////////////////////////////////////////////////////////////

bool CConfigEeprom::ParseConfig(CParser* parser)
{
	switch (parser->GetReader()->SkipSpaces())
	{
		case '?': 
			PrintConfig();
			parser->GetReader()->GetNextChar();
			return true;
		case '!':
			_eepromcanwrite = true;
			parser->GetReader()->GetNextChar();
			return true;
	}

	uint8_t slot = parser->GetUInt8();
	if (parser->GetReader()->SkipSpaces() != '=') 
		return false;

	parser->GetReader()->GetNextChar();
	uint32_t varvalue = parser->GetUInt32();

	if (parser->IsError() || slot < _eepromsizesize / sizeof(uint32_t) || !_eepromcanwrite)
		return false;

	if (!_eepromvalid) FlushConfig();
	SetSlot32(slot, varvalue);

	return true;
}
