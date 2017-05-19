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
  http://www.gnu.org/licenses/
*/
////////////////////////////////////////////////////////

#define _CRT_SECURE_NO_WARNINGS

////////////////////////////////////////////////////////

#if defined(_MSC_VER)

#include <stdlib.h>
#include <string.h>

#include <arduino.h>
#include <ctype.h>

#include "HAL.h"
#include "UtilitiesStepperLib.h"

////////////////////////////////////////////////////////

uint32_t CHAL::_eepromBuffer[2048] = { 0 };
char* CHAL::_eepromFileName = NULL;

////////////////////////////////////////////////////////

bool CHAL::HaveEeprom()
{
	return true;
}

////////////////////////////////////////////////////////

void CHAL::InitEeprom()
{
	if (_eepromFileName)
	{
		FILE* f = NULL;
		fopen_s(&f, _eepromFileName, "rb+");

		if (f)
		{
			fread(_eepromBuffer, 1, sizeof(_eepromBuffer), f);
			fclose(f);
		}
	}
}

void CHAL::FlushEeprom()
{
	if (_eepromFileName)
	{
		FILE* f = NULL;
		fopen_s(&f,_eepromFileName, "wb+");
		if (f)
		{
			fwrite(_eepromBuffer, sizeof(_eepromBuffer), 1, f);
			fclose(f);
		}
	}
}


////////////////////////////////////////////////////////

#endif		// _MSC_VER

