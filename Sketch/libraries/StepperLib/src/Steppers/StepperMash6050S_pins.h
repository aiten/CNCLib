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

#pragma once

////////////////////////////////////////////////////////

#define MASH6050S_PIN_STEP_OFF 0
#define MASH6050S_PIN_STEP_ON 1

#define MASH6050S_PIN_DIR_OFF 0
#define MASH6050S_PIN_DIR_ON 1

////////////////////////////////////////////////////////

#define MASH6050S_REF_ON			0
#define MASH6050S_REF_OFF			1

#if defined(__AVR_ATmega2560__) || defined(__SAM3X8E__) || defined(__SAMD21G18A__)

////////////////////////////////////////////////////////

#if defined(__AVR_ATmega2560__)

#define MASH6050S_INPUTPINMODE		INPUT_PULLUP		

#else

#define MASH6050S_INPUTPINMODE		INPUT			// use of 4050N for level transformation

#endif

// 54 => AD0 
#define MASH6050S_X_STEP_PIN		54
#define MASH6050S_X_DIR_PIN			55
#define MASH6050S_X_MIN_PIN			3

#define MASH6050S_Y_STEP_PIN		60
#define MASH6050S_Y_DIR_PIN			61
#define MASH6050S_Y_MIN_PIN			14

#define MASH6050S_Z_STEP_PIN		46
#define MASH6050S_Z_DIR_PIN			48
#define MASH6050S_Z_MAX_PIN			19

#define MASH6050S_C_STEP_PIN		26
#define MASH6050S_C_DIR_PIN			28
#define MASH6050S_C_MIN_PIN			18

#define MASH6050S_SDPOWER			-1
#if defined(__SAM3X8E__) || defined(__SAMD21G18A__)
#define MASH6050S_SDSS_PIN			4
#else
#define MASH6050S_SDSS_PIN			53
#endif
#elif defined(__AVR_ATmega328P__) || defined (_MSC_VER)

#define MASH6050S_INPUTPINMODE		INPUT_PULLUP		

#define MASH6050S_X_STEP_PIN		14
#define MASH6050S_X_DIR_PIN			15	
#define MASH6050S_X_MIN_PIN			2	

#define MASH6050S_Y_STEP_PIN		16	
#define MASH6050S_Y_DIR_PIN			17	
#define MASH6050S_Y_MIN_PIN			3	

#define MASH6050S_Z_STEP_PIN		18	
#define MASH6050S_Z_DIR_PIN			19	
#define MASH6050S_Z_MAX_PIN			4	

#define MASH6050S_C_STEP_PIN		11	
#define MASH6050S_C_DIR_PIN			12	
#define MASH6050S_C_MIN_PIN			5	

#define MASH6050S_SDPOWER			-1
#define MASH6050S_SDSS_PIN			10		

#else



#endif

////////////////////////////////////////////////////////

#define MASH6050S_KILL_PIN	20
#define MASH6050S_KILL_PIN_ON  HIGH	// Pressed
#define MASH6050S_KILL_PIN_OFF LOW

////////////////////////////////////////////////////////

#define MASH6050S_LED_PIN			13		// D13
//#define MASH6050S_PS_ON_PIN			12		// D12

//#define MASH6050S_FET1D8_PIN		8		// D8 FET1
//#define MASH6050S_FET2D9_PIN		9		// D9 FET2
//#define MASH6050S_FET3D10_PIN		10		// D10 FET3

//#define MASH6050S_KILL_PIN			41		// D41

//#define MASH6050S_TEMP_0_PIN		67		// AD13
//#define MASH6050S_TEMP_1_PIN		68		// AD14
//#define MASH6050S_TEMP_2_PIN		69		// AD15

//#define MASH6050S_SERVO0_PIN		11		// D11
//#define MASH6050S_SERVO1_PIN		6		// D6
//#define MASH6050S_SERVO2_PIN		5		// D5
//#define MASH6050S_SERVO3_PIN		4		// D4

// these pins are defined in the SD library if building with SD support  
#define MASH6050S_MAX_SCK_PIN         52		// D52
#define MASH6050S_MAX_MISO_PIN        50		// D50
#define MASH6050S_MAX_MOSI_PIN        51		// D51
#define MASH6050S_MAX6675_SS		  53		// D53
/*
#define MASH6050S_AUX2_1				-1		// VLogic
#define MASH6050S_AUX2_2				-1		// GND
#define MASH6050S_AUX2_3				59		// AD5
#define MASH6050S_AUX2_4				63		// AD9
#define MASH6050S_AUX2_5				64		// A10
#define MASH6050S_AUX2_6				40		// D40
#define MASH6050S_AUX2_7				44		// D44
#define MASH6050S_AUX2_8				42		// D42
#define MASH6050S_AUX2_9				66		// A12
#define MASH6050S_AUX2_10				65		// A11

#define MASH6050S_AUX3_1				-1		// 5V
#define MASH6050S_AUX3_2				49		// D49
#define MASH6050S_AUX3_3				50		// MISO - D50
#define MASH6050S_AUX3_4				51		// MOSI - D51
#define MASH6050S_AUX3_5				52		// SCK  - D52
#define MASH6050S_AUX3_6				53		// SPI_CS1 - D53
#define MASH6050S_AUX3_7				-1		// GND
#define MASH6050S_AUX3_8				-1		// NC

#define MASH6050S_AUX4_1				-1		// VLOGIC
#define MASH6050S_AUX4_2				-1		// GND
#define MASH6050S_AUX4_3				32		// D32
#define MASH6050S_AUX4_4				47		// D47
#define MASH6050S_AUX4_5				45		// D45
#define MASH6050S_AUX4_6				43		// D43
#define MASH6050S_AUX4_7				41		// D41
#define MASH6050S_AUX4_8				39		// D39
#define MASH6050S_AUX4_9				37		// D37
#define MASH6050S_AUX4_10				35		// D35
#define MASH6050S_AUX4_11				33		// D33
#define MASH6050S_AUX4_12				31		// D31
#define MASH6050S_AUX4_13				29		// D29
#define MASH6050S_AUX4_14				27		// D27
#define MASH6050S_AUX4_15				25		// D25
#define MASH6050S_AUX4_16				23		// D23
#define MASH6050S_AUX4_17				17		// UART2_RX  Serial2 on pins 17 (RX) and 16 (TX)
#define MASH6050S_AUX4_18				16		// UART2_TX
// 3-8: share E1&E2
*/
////////////////////////////////////////////////////////
// LCD

#define MASH6050S_LCD_ROTARY_ENC		35  // MASH6050S_AUX4_10		// Dreh Encoder auf Ramps 1.4 - Press button
#define MASH6050S_LCD_ROTARY_EN1		33	// MASH6050S_AUX4_11		// Dreh Encoder auf Ramps 1.4
#define MASH6050S_LCD_ROTARY_EN2		31  // MASH6050S_AUX4_12		// Dreh Encoder auf Ramps 1.4

#define MASH6050S_LCD_ROTARY_ENC_ON	LOW		// Pressed
#define MASH6050S_LCD_ROTARY_ENC_OFF	HIGH

#define MASH6050S_LCD_BEEPER			37 // MASH6050S_AUX4_9		// Summer auf Ramps 1.4
#define MASH6050S_LCD_KILL_PIN			41 // MASH6050S_AUX4_7		// Stoptaste auf Ramps 1.4

#define MASH6050S_LCD_KILL_PIN_ON  LOW	// Pressed
#define MASH6050S_LCD_KILL_PIN_OFF HIGH

#define MASH6050S_LCD_PINS_RS			16 // MASH6050S_AUX4_18 
#define MASH6050S_LCD_PINS_ENABLE		17 // MASH6050S_AUX4_17
#define MASH6050S_LCD_PINS_D4			23 // MASH6050S_AUX4_16
#define MASH6050S_LCD_PINS_D5			25 // MASH6050S_AUX4_15  
#define MASH6050S_LCD_PINS_D6			27 // MASH6050S_AUX4_14
#define MASH6050S_LCD_PINS_D7			29 // MASH6050S_AUX4_13

#define MASH6050S_ST7920_CLK_PIN		MASH6050S_LCD_PINS_D4
#define MASH6050S_ST7920_DAT_PIN		MASH6050S_LCD_PINS_ENABLE
#define MASH6050S_ST7920_CS_PIN			MASH6050S_LCD_PINS_RS
