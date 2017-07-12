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
const uint8_t CHAL::_flashStorage[EEPROM_SIZE] = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11,12,13,14,15,16,17,18 };
uint8_t CHAL::_flashBuffer[EEPROM_SIZE];

//PAGE_SIZE(pageSizes[NVMCTRL->PARAM.bit.PSZ]),
//PAGES(NVMCTRL->PARAM.bit.NVMP),
//MAX_FLASH(PAGE_SIZE * PAGES),
//ROW_SIZE(PAGE_SIZE * 4),

void CHAL::WriteToFlash(const volatile void *flash_ptr, const void *data, uint32_t size)
{
	uint32_t PAGE_SIZE = 8 << NVMCTRL->PARAM.bit.PSZ;
	uint32_t PAGES = NVMCTRL->PARAM.bit.NVMP;
	uint32_t ROW_SIZE = PAGE_SIZE * 4;

	// Calculate data boundaries
	uint32_t dwordcount = (size + 3) / 4;
	volatile uint8_t *dst_addr = (volatile uint8_t *)flash_ptr;
	volatile uint32_t *dst_addr32 = (volatile uint32_t *)flash_ptr;
	const uint8_t *src_addr = (uint8_t *)data;
	const uint32_t *src_addr32 = (uint32_t *)data;

	Serial.print(PAGE_SIZE); Serial.print(':');
	Serial.print(PAGES); Serial.print(':');
	Serial.print(ROW_SIZE); Serial.print(':');
	Serial.print(size); Serial.print(':');
	Serial.print(dwordcount); Serial.print(':');

	// Disable automatic page write
	NVMCTRL->CTRLB.bit.MANW = 1;

	int maxcnt = 65;

	// Do writes in pages
	while (size)
	{
		if ((maxcnt--) < 0)
		{
			Serial.print("panik");
			break;
		}

		// Execute "PBC" Page Buffer Clear
		NVMCTRL->CTRLA.reg = NVMCTRL_CTRLA_CMDEX_KEY | NVMCTRL_CTRLA_CMD_PBC;
		while (NVMCTRL->INTFLAG.bit.READY == 0) {}

		uint32_t copyByte = min(PAGE_SIZE, size);
		memcpy((void*) dst_addr, src_addr, copyByte);
		dst_addr += copyByte;
		src_addr += copyByte;
		size -= copyByte;


/*
		// Fill page buffer
		uint32_t i;
		for (i = 0; i<(PAGE_SIZE / 4) && size; i++) 
		{
//			*dst_addr = read_unaligned_uint32(src_addr);
			*dst_addr = *src_addr32;
			src_addr32++;
			dst_addr++;
			size--;
		}
*/

		// Execute "WP" Write Page
		NVMCTRL->CTRLA.reg = NVMCTRL_CTRLA_CMDEX_KEY | NVMCTRL_CTRLA_CMD_WP;
		while (NVMCTRL->INTFLAG.bit.READY == 0) {}
	}
}

void CHAL::EraseFlash(const volatile void *flash_ptr, uint32_t size)
{
	uint32_t PAGE_SIZE = 8 << NVMCTRL->PARAM.bit.PSZ;
	uint32_t PAGES = NVMCTRL->PARAM.bit.NVMP;
	uint32_t ROW_SIZE = PAGE_SIZE * 4;

	const uint8_t *ptr = (const uint8_t *)flash_ptr;
	while (size > ROW_SIZE) 
	{
		EraseFlash(ptr);
		ptr += ROW_SIZE;
		size -= ROW_SIZE;
	}
	EraseFlash(ptr);
}

void CHAL::EraseFlash(const volatile void *flash_ptr)
{
	NVMCTRL->ADDR.reg = ((uint32_t)flash_ptr) / 2;
	NVMCTRL->CTRLA.reg = NVMCTRL_CTRLA_CMDEX_KEY | NVMCTRL_CTRLA_CMD_ER;
	while (!NVMCTRL->INTFLAG.bit.READY) {}
}

void CHAL::ReadFlash(const volatile void *flash_ptr, void *data, uint32_t size)
{
	memcpy(data, (const void *)flash_ptr, size);
}


////////////////////////////////////////////////////////

#endif 

