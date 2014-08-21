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

#define RAMPSFD_PINOFF 0
#define RAMPSFD_PINON 1

////////////////////////////////////////////////////////

// only available on Arduino Mega / due

#define RAMPSFD_REF_ON	0
#define RAMPSFD_REF_OFF	1

#define RAMPSFD_X_STEP_PIN         63		// A9
#define RAMPSFD_X_DIR_PIN          62		// A8
#define RAMPSFD_X_ENABLE_PIN       48
#define RAMPSFD_X_MIN_PIN          22
#define RAMPSFD_X_MAX_PIN          30

#define RAMPSFD_Y_STEP_PIN         65		//A11
#define RAMPSFD_Y_DIR_PIN          64		//A10
#define RAMPSFD_Y_ENABLE_PIN       46
#define RAMPSFD_Y_MIN_PIN          24
#define RAMPSFD_Y_MAX_PIN          38

#define RAMPSFD_Z_STEP_PIN         67		//A13
#define RAMPSFD_Z_DIR_PIN          66		//A12
#define RAMPSFD_Z_ENABLE_PIN       44
#define RAMPSFD_Z_MIN_PIN          26
#define RAMPSFD_Z_MAX_PIN          34

#define RAMPSFD_E0_STEP_PIN        36
#define RAMPSFD_E0_DIR_PIN         28
#define RAMPSFD_E0_ENABLE_PIN      42

#define RAMPSFD_E1_STEP_PIN        43
#define RAMPSFD_E1_DIR_PIN         41
#define RAMPSFD_E1_ENABLE_PIN      39

#define RAMPSFD_E2_STEP_PIN        32
#define RAMPSFD_E2_DIR_PIN         47
#define RAMPSFD_E2_ENABLE_PIN      45

#define RAMPSFD_SDPOWER            -1
#define RAMPSFD_SDSS               4
#define RAMPSFD_LED_PIN            13

#define RAMPSFD_FAN_PIN            9 // (Sprinter config)

#define RAMPSFD_CONTROLLERFAN_PIN  10 //Pin used for the fan to cool controller

#define RAMPSFD_PS_ON_PIN          12

#define RAMPSFD_KILL_PIN           41
#define RAMPSFD_HEATER_0_PIN       8
#define RAMPSFD_HEATER_1_PIN       9    // EXTRUDER 2 (FAN On Sprinter)

#define RAMPSFD_TEMP_0_PIN         13   // ANALOG NUMBERING
#define RAMPSFD_TEMP_1_PIN         15   // ANALOG NUMBERING
#define RAMPSFD_TEMP_2_PIN         -1   // ANALOG NUMBERING

#define RAMPSFD_HEATER_BED_PIN     -1    // NO BED
#define RAMPSFD_TEMP_BED_PIN       14   // ANALOG NUMBERING

#define RAMPSFD_SERVO0_PIN         11
#define RAMPSFD_SERVO1_PIN         6
#define RAMPSFD_SERVO2_PIN         5
#define RAMPSFD_SERVO3_PIN         4

#define RAMPSFD_ESTOP		       40

////////////////////////////////////////////////////////
// LCD

#define RAMPSFD_LCD_ROTARY_ENC		35  // Dreh Encoder auf Ramps 1.4 - Press button
#define RAMPSFD_LCD_ROTARY_EN1      31  // Dreh Encoder auf Ramps 1.4
#define RAMPSFD_LCD_ROTARY_EN2      33  // Dreh Encoder auf Ramps 1.4

#define RAMPSFD_LCD_ROTARY_ENC_ON  LOW		// Pressed
#define RAMPSFD_LCD_ROTARY_ENC_OFF HIGH

#define RAMPSFD_LCD_BEEPER          37   // Summer auf Ramps 1.4
#define RAMPSFD_LCD_KILL_PIN        41   // Stoptaste auf Ramps 1.4

#define RAMPSFD_LCD_KILL_PIN_ON  LOW	// Pressed
#define RAMPSFD_LCD_KILL_PIN_OFF HIGH

#define RAMPSFD_LCD_PINS_RS 16 
#define RAMPSFD_LCD_PINS_ENABLE 17
#define RAMPSFD_LCD_PINS_D4 23
#define RAMPSFD_LCD_PINS_D5 25 
#define RAMPSFD_LCD_PINS_D6 27
#define RAMPSFD_LCD_PINS_D7 29

#define RAMPSFD_ST7920_CLK_PIN  RAMPSFD_LCD_PINS_D4
#define RAMPSFD_ST7920_DAT_PIN  RAMPSFD_LCD_PINS_ENABLE
#define RAMPSFD_ST7920_CS_PIN   RAMPSFD_LCD_PINS_RS
