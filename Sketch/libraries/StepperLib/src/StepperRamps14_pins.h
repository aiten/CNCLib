////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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

#define RAMPS14_PINOFF 0
#define RAMPS14_PINON 1

////////////////////////////////////////////////////////

// only available on Arduino Mega / due

#define RAMPS14_REF_ON	0
#define RAMPS14_REF_OFF	1

#define RAMPS14_X_STEP_PIN         54
#define RAMPS14_X_DIR_PIN          55
#define RAMPS14_X_ENABLE_PIN       38
#define RAMPS14_X_MIN_PIN           3
#define RAMPS14_X_MAX_PIN           2

#define RAMPS14_Y_STEP_PIN         60
#define RAMPS14_Y_DIR_PIN          61
#define RAMPS14_Y_ENABLE_PIN       56
#define RAMPS14_Y_MIN_PIN          14
#define RAMPS14_Y_MAX_PIN          15

#define RAMPS14_Z_STEP_PIN         46
#define RAMPS14_Z_DIR_PIN          48
#define RAMPS14_Z_ENABLE_PIN       62
#define RAMPS14_Z_MIN_PIN          18
#define RAMPS14_Z_MAX_PIN          19

#define RAMPS14_Z2_STEP_PIN        36
#define RAMPS14_Z2_DIR_PIN         34
#define RAMPS14_Z2_ENABLE_PIN      30

#define RAMPS14_E0_STEP_PIN        26
#define RAMPS14_E0_DIR_PIN         28
#define RAMPS14_E0_ENABLE_PIN      24

#define RAMPS14_E1_STEP_PIN        36
#define RAMPS14_E1_DIR_PIN         34
#define RAMPS14_E1_ENABLE_PIN      30

#define RAMPS14_SDPOWER            -1
#define RAMPS14_SDSS               53
#define RAMPS14_LED_PIN            13

#define RAMPS14_FAN_PIN            9 // (Sprinter config)

#define RAMPS14_CONTROLLERFAN_PIN  10 //Pin used for the fan to cool controller

#define RAMPS14_PS_ON_PIN          12

#define RAMPS14_KILL_PIN           41
#define RAMPS14_HEATER_0_PIN       8
#define RAMPS14_HEATER_1_PIN       9    // EXTRUDER 2 (FAN On Sprinter)

#define RAMPS14_TEMP_0_PIN         13   // ANALOG NUMBERING
#define RAMPS14_TEMP_1_PIN         15   // ANALOG NUMBERING
#define RAMPS14_TEMP_2_PIN         -1   // ANALOG NUMBERING

#define RAMPS14_HEATER_BED_PIN     -1    // NO BED
#define RAMPS14_TEMP_BED_PIN       14   // ANALOG NUMBERING

#define RAMPS14_SERVO0_PIN         11
#define RAMPS14_SERVO1_PIN         6
#define RAMPS14_SERVO2_PIN         5
#define RAMPS14_SERVO3_PIN         4

// these pins are defined in the SD library if building with SD support  
#define RAMPS14_MAX_SCK_PIN         52
#define RAMPS14_MAX_MISO_PIN        50
#define RAMPS14_MAX_MOSI_PIN        51
#define RAMPS14_MAX6675_SS			53

#define RAMPS14_AUX2_1				-1	// VLogic
#define RAMPS14_AUX2_2				-1	// GND
#define RAMPS14_AUX2_3				59	// A5
#define RAMPS14_AUX2_4				63	// A9
#define RAMPS14_AUX2_5				64	// A10
#define RAMPS14_AUX2_6				40	// D40
#define RAMPS14_AUX2_7				44	// D44
#define RAMPS14_AUX2_8				42	// D42
#define RAMPS14_AUX2_9				66	// A12
#define RAMPS14_AUX2_10				65	// A11

#define RAMPS14_AUX3_1				-1	// 5V
#define RAMPS14_AUX3_2				49	// D49
#define RAMPS14_AUX3_3				50	// MISO - D50
#define RAMPS14_AUX3_4				51	// MOSI - D51
#define RAMPS14_AUX3_5				52	// SCK  - D52
#define RAMPS14_AUX3_6				53	// SPI_CS1 - D53
#define RAMPS14_AUX3_7				-1	// GND
#define RAMPS14_AUX3_8				-1	// NC

#define RAMPS14_AUX4_1				-1	// VLOGIC
#define RAMPS14_AUX4_2				-1	// GND
#define RAMPS14_AUX4_3				32	// D32
#define RAMPS14_AUX4_4				47	// D47
#define RAMPS14_AUX4_5				45	// D45
#define RAMPS14_AUX4_6				43	// D43
#define RAMPS14_AUX4_7				41	// D41
#define RAMPS14_AUX4_8				39	// D39
#define RAMPS14_AUX4_9				37	// D37
#define RAMPS14_AUX4_10				35	// D35
#define RAMPS14_AUX4_11				33	// D33
#define RAMPS14_AUX4_12				31	// D31
#define RAMPS14_AUX4_13				29	// D29
#define RAMPS14_AUX4_14				27	// D27
#define RAMPS14_AUX4_15				25	// D25
#define RAMPS14_AUX4_16				23	// D23
#define RAMPS14_AUX4_17				17	// UART2_RX  Serial2 on pins 17 (RX) and 16 (TX)
#define RAMPS14_AUX4_18				16	// UART2_TX
// 3-8: share E1&E2

////////////////////////////////////////////////////////
// LCD

#define RAMPS14_LCD_ROTARY_ENC		RAMPS14_AUX4_10		// Dreh Encoder auf Ramps 1.4 - Press button
#define RAMPS14_LCD_ROTARY_EN1      RAMPS14_AUX4_12		// Dreh Encoder auf Ramps 1.4
#define RAMPS14_LCD_ROTARY_EN2      RAMPS14_AUX4_11		// Dreh Encoder auf Ramps 1.4

#define RAMPS14_LCD_ROTARY_ENC_ON	LOW		// Pressed
#define RAMPS14_LCD_ROTARY_ENC_OFF	HIGH

#define RAMPS14_LCD_BEEPER          RAMPS14_AUX4_9		// Summer auf Ramps 1.4
#define RAMPS14_LCD_KILL_PIN        RAMPS14_AUX4_7		// Stoptaste auf Ramps 1.4

#define RAMPS14_LCD_KILL_PIN_ON  LOW	// Pressed
#define RAMPS14_LCD_KILL_PIN_OFF HIGH

#define RAMPS14_LCD_PINS_RS			RAMPS14_AUX4_18 
#define RAMPS14_LCD_PINS_ENABLE		RAMPS14_AUX4_17
#define RAMPS14_LCD_PINS_D4			RAMPS14_AUX4_16
#define RAMPS14_LCD_PINS_D5			RAMPS14_AUX4_15  
#define RAMPS14_LCD_PINS_D6			RAMPS14_AUX4_14
#define RAMPS14_LCD_PINS_D7			RAMPS14_AUX4_13

#define RAMPS14_ST7920_CLK_PIN		RAMPS14_LCD_PINS_D4
#define RAMPS14_ST7920_DAT_PIN		RAMPS14_LCD_PINS_ENABLE
#define RAMPS14_ST7920_CS_PIN		RAMPS14_LCD_PINS_RS
