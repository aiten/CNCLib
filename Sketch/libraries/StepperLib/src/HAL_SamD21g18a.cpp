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

#include <stdlib.h>
#include <string.h>

#include <arduino.h>
#include <ctype.h>

#include "HAL.h"

////////////////////////////////////////////////////////

#if defined(__SAMD21G18A__)

static void IgnoreIrq() {}

void TC5_Handler()
{
	TcCount16* TC = GetTimer0Struct();

	if (TC->INTFLAG.bit.OVF == 1)                     // A overflow caused the interrupt
	{
		TC->INTFLAG.bit.OVF = 1;                     // writing a one clears the flag ovf flag
		WaitForSyncTC(TC);
	}

	CHAL::_TimerEvent0();
}


void TC4_Handler()
{
	TcCount16* TC = GetTimer1Struct();

	if (TC->INTFLAG.bit.OVF == 1)                     // A overflow caused the interrupt
	{
		TC->INTFLAG.bit.OVF = 1;                     // writing a one clears the flag ovf flag
		WaitForSyncTC(TC);
	}
	
//	StepperSerial.println("TC");
	CHAL::_TimerEvent1();
}

void I2S_Handler()
{
	CHAL::_BackgroundEvent();
}
/*
void TC3_Handler()
{
	CHAL::_BackgroundEvent();
}
*/

CHAL::HALEvent CHAL::_BackgroundEvent = IgnoreIrq;

////////////////////////////////////////////////////////

__attribute__((__aligned__(256))) \
const uint8_t CHAL::_flashStorage[EEPROM_SIZE] = { };
uint8_t CHAL::_flashBuffer[EEPROM_SIZE];

#define wait_ready()   while (NVMCTRL->INTFLAG.bit.READY == 0) {}

void CHAL::FlashWriteWords(uint32_t *flash_ptr, const uint32_t *src, uint32_t n_words)
{
	// Set automatic page write
	NVMCTRL->CTRLB.bit.MANW = 0;

	while (n_words > 0) 
	{
		uint32_t len = min(FLASH_PAGE_SIZE >> 2, n_words);
		n_words -= len;

		// Execute "PBC" Page Buffer Clear
		NVMCTRL->CTRLA.reg = NVMCTRL_CTRLA_CMDEX_KEY | NVMCTRL_CTRLA_CMD_PBC;
		wait_ready();

		while (len--)
			*flash_ptr++ = *src++;

		// Execute "WP" Write Page
		NVMCTRL->CTRLA.reg = NVMCTRL_CTRLA_CMDEX_KEY | NVMCTRL_CTRLA_CMD_WP;
		wait_ready();
	}
}

void CHAL::FlashErase(void *flash_ptr, uint32_t size)
{
	uint32_t ROW_SIZE = FLASH_PAGE_SIZE * sizeof(uint32_t);
	uint8_t *ptr = (uint8_t *)flash_ptr;
	int32_t isize = size;

	while (isize > 0)
	{
		FlashEraseRow(ptr);
		ptr += ROW_SIZE;
		isize -= ROW_SIZE;
	}
}

void CHAL::FlashEraseRow(void *flash_ptr)
{
	NVMCTRL->ADDR.reg = ((uint32_t)flash_ptr) / 2;
	NVMCTRL->CTRLA.reg = NVMCTRL_CTRLA_CMDEX_KEY | NVMCTRL_CTRLA_CMD_ER;
	wait_ready();
}

void CHAL::FlashRead(const void *flash_ptr, void *data, uint32_t size)
{
	memcpy(data, flash_ptr, size);
}


////////////////////////////////////////////////////////

#endif 

