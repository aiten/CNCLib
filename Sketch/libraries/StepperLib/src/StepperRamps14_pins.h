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
#define RAMPS14_MAX_SCK_PIN          52
#define RAMPS14_MAX_MISO_PIN         50
#define RAMPS14_MAX_MOSI_PIN         51
#define RAMPS14_MAX6675_SS       53

////////////////////////////////////////////////////////
// LCD

#define RAMPS14_LCD_ROTARY_ENC		35  // Dreh Encoder auf Ramps 1.4 - Press button
#define RAMPS14_LCD_ROTARY_EN1      31  // Dreh Encoder auf Ramps 1.4
#define RAMPS14_LCD_ROTARY_EN2      33  // Dreh Encoder auf Ramps 1.4

#define RAMPS14_LCD_ROTARY_ENC_ON  LOW		// Pressed
#define RAMPS14_LCD_ROTARY_ENC_OFF HIGH

#define RAMPS14_LCD_BEEPER          37   // Summer auf Ramps 1.4
#define RAMPS14_LCD_KILL_PIN        41   // Stoptaste auf Ramps 1.4

#define RAMPS14_LCD_KILL_PIN_ON  LOW	// Pressed
#define RAMPS14_LCD_KILL_PIN_OFF HIGH

#define RAMPS14_LCD_PINS_RS 16 
#define RAMPS14_LCD_PINS_ENABLE 17
#define RAMPS14_LCD_PINS_D4 23
#define RAMPS14_LCD_PINS_D5 25 
#define RAMPS14_LCD_PINS_D6 27
#define RAMPS14_LCD_PINS_D7 29

#define RAMPS14_ST7920_CLK_PIN  RAMPS14_LCD_PINS_D4
#define RAMPS14_ST7920_DAT_PIN  RAMPS14_LCD_PINS_ENABLE
#define RAMPS14_ST7920_CS_PIN   RAMPS14_LCD_PINS_RS
