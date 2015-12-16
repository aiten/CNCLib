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

////////////////////////////////////////////////////////

#undef  RAMPSFD_USE_A4998
#define RAMPSFD_USE_DRV8825

////////////////////////////////////////////////////////

#define RAMPSFD_PIN_STEP_OFF 0
#define RAMPSFD_PIN_STEP_ON 1

#define RAMPSFD_PIN_DIR_OFF 0
#define RAMPSFD_PIN_DIR_ON 1

// Enable: LOW Active
#define RAMPSFD_PIN_ENABLE_OFF 1
#define RAMPSFD_PIN_ENABLE_ON 0

////////////////////////////////////////////////////////

// only available on Arduino Mega / due

#define RAMPSFD_REF_ON	0
#define RAMPSFD_REF_OFF	1

// 63 => AD9
#define RAMPSFD_X_STEP_PIN			63
#define RAMPSFD_X_DIR_PIN			62
#define RAMPSFD_X_ENABLE_PIN		48
#define RAMPSFD_X_MIN_PIN			22
#define RAMPSFD_X_MAX_PIN			30

#define RAMPSFD_Y_STEP_PIN			65
#define RAMPSFD_Y_DIR_PIN			64
#define RAMPSFD_Y_ENABLE_PIN		46
#define RAMPSFD_Y_MIN_PIN			24
#define RAMPSFD_Y_MAX_PIN			38

#define RAMPSFD_Z_STEP_PIN			67
#define RAMPSFD_Z_DIR_PIN			66
#define RAMPSFD_Z_ENABLE_PIN		44
#define RAMPSFD_Z_MIN_PIN			26
#define RAMPSFD_Z_MAX_PIN			34

#define RAMPSFD_E0_STEP_PIN			36
#define RAMPSFD_E0_DIR_PIN			28
#define RAMPSFD_E0_ENABLE_PIN		42

#define RAMPSFD_E1_STEP_PIN			43
#define RAMPSFD_E1_DIR_PIN			41
#define RAMPSFD_E1_ENABLE_PIN		39

#define RAMPSFD_E2_STEP_PIN			32
#define RAMPSFD_E2_DIR_PIN			47
#define RAMPSFD_E2_ENABLE_PIN		45

#define RAMPSFD_SDPOWER_PIN			-1
#define RAMPSFD_SDSS_PIN			4	
#define RAMPSFD_LED_PIN				13	

#define RAMPSFD_UART3_TX_PIN		14	
#define RAMPSFD_UART3_RX_PIN		15	
#define RAMPSFD_UART2_TX_PIN		16	
#define RAMPSFD_UART2_RX_PIN		17	
#define RAMPSFD_UART1_TX_PIN		18	
#define RAMPSFD_UART1_RX_PIN		19	
#define RAMPSFD_SDA_PIN				20	
#define RAMPSFD_SLC_PIN				21	

// D8 FET1
// D9 FET2
// D10 FET3
// D11 FET4
// D12 FET5 - active hi
// D2 FET6 - active hi
#define RAMPSFD_FET1D8_PIN			8	
#define RAMPSFD_FET2D9_PIN			9	
#define RAMPSFD_FET3D10_PIN			10	
#define RAMPSFD_FET4D11_PIN			11	
#define RAMPSFD_FET5D12_PIN			12	
#define RAMPSFD_FET6D2_PIN			2	

#define RAMPSFD_PS_ON_PIN			53		// D53

#define RAMPSFD_ESTOP_PIN			40		// D40

#define RAMPSFD_TEMP_0_PIN			54		// AD0
#define RAMPSFD_TEMP_1_PIN			55		// AD1
#define RAMPSFD_TEMP_2_PIN			56		// AD2
#define RAMPSFD_TEMP_3_PIN			57		// AD3

#define RAMPSFD_SERVO1_PIN			7		// D7
#define RAMPSFD_SERVO2_PIN			6		// D6
#define RAMPSFD_SERVO3_PIN			5		// D5
#define RAMPSFD_SERVO4_PIN			3		// D3

#define RAMPSFD_AUX2_1				-1		// VLogic
#define RAMPSFD_AUX2_2				-1		// GND
#define RAMPSFD_AUX2_3				58		// AD4
#define RAMPSFD_AUX2_4				59		// AD5
#define RAMPSFD_AUX2_5				60		// AD6
#define RAMPSFD_AUX2_6				61		// AD7
#define RAMPSFD_AUX2_7				68		// AD14 - CANRX
#define RAMPSFD_AUX2_8				69		// AD15	- CANTX
#define RAMPSFD_AUX2_9				-1		// UART3_TX
#define RAMPSFD_AUX2_10				-1		// UART3_RX

#define RAMPSFD_AUX3_1				-1		// 5V
#define RAMPSFD_AUX3_2				49		// D49
#define RAMPSFD_AUX3_3				4		// MISO
#define RAMPSFD_AUX3_4				5		// MOSI
#define RAMPSFD_AUX3_5				6		// SCK
#define RAMPSFD_AUX3_6				7		// SPI_CS1
#define RAMPSFD_AUX3_7				-1		// GND
#define RAMPSFD_AUX3_8				-1		// NC
#define RAMPSFD_AUX3_9				50		// D50 - MISO
#define RAMPSFD_AUX3_10				51		// D51 - MOSI
#define RAMPSFD_AUX3_11				52		// D52 - SCK
#define RAMPSFD_AUX3_12				-1		// CLOGIC

#define RAMPSFD_AUX4_1				-1		// VLOGIC
#define RAMPSFD_AUX4_2				-1		// GND
#define RAMPSFD_AUX4_3				32		// D32
#define RAMPSFD_AUX4_4				47		// D47
#define RAMPSFD_AUX4_5				45		// D45
#define RAMPSFD_AUX4_6				43		// D43
#define RAMPSFD_AUX4_7				41		// D41
#define RAMPSFD_AUX4_8				39		// D39
#define RAMPSFD_AUX4_9				37		// D37
#define RAMPSFD_AUX4_10				35		// D35
#define RAMPSFD_AUX4_11				33		// D33
#define RAMPSFD_AUX4_12				31		// D31
#define RAMPSFD_AUX4_13				29		// D29
#define RAMPSFD_AUX4_14				27		// D27
#define RAMPSFD_AUX4_15				25		// D25
#define RAMPSFD_AUX4_16				23		// D23
#define RAMPSFD_AUX4_17				RAMPSFD_UART2_RX_PIN
#define RAMPSFD_AUX4_18				RAMPSFD_UART2_TX_PIN
// 3-8: share E1&E2

////////////////////////////////////////////////////////
// LCD

#define RAMPSFD_LCD_ROTARY_ENC		RAMPSFD_AUX4_10		// Dreh Encoder auf Ramps FD - Press button
#define RAMPSFD_LCD_ROTARY_EN1      RAMPSFD_AUX4_12		// Dreh Encoder auf Ramps FD
#define RAMPSFD_LCD_ROTARY_EN2      RAMPSFD_AUX4_11		// Dreh Encoder auf Ramps FD

#define RAMPSFD_LCD_ROTARY_ENC_ON	LOW					// Pressed
#define RAMPSFD_LCD_ROTARY_ENC_OFF	HIGH

#define RAMPSFD_LCD_BEEPER          RAMPSFD_AUX4_9		// Summer auf Ramps FD
#define RAMPSFD_LCD_KILL_PIN        RAMPSFD_AUX4_7		// Stoptaste auf Ramps FD (LCD) => shared with E1
//#define RAMPSFD_LCD_KILL_PIN        RAMPSFD_ESTOP_PIN	// hard link

#define RAMPSFD_LCD_KILL_PIN_ON		LOW	// Pressed
#define RAMPSFD_LCD_KILL_PIN_OFF	HIGH

#define RAMPSFD_LCD_PINS_RS			RAMPSFD_AUX4_18
#define RAMPSFD_LCD_PINS_ENABLE		RAMPSFD_AUX4_17
#define RAMPSFD_LCD_PINS_D4			RAMPSFD_AUX4_16
#define RAMPSFD_LCD_PINS_D5			RAMPSFD_AUX4_15 
#define RAMPSFD_LCD_PINS_D6			RAMPSFD_AUX4_14
#define RAMPSFD_LCD_PINS_D7			RAMPSFD_AUX4_13

#define RAMPSFD_ST7920_CLK_PIN		RAMPSFD_LCD_PINS_D4
#define RAMPSFD_ST7920_DAT_PIN		RAMPSFD_LCD_PINS_ENABLE
#define RAMPSFD_ST7920_CS_PIN		RAMPSFD_LCD_PINS_RS
