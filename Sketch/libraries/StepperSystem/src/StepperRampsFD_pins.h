#pragma once

////////////////////////////////////////////////////////

#define PINOFF 0
#define PINON 1

////////////////////////////////////////////////////////

// only available on Arduino Mega / due

#define REF_ON	0
#define REF_OFF	1

#define X_STEP_PIN         63		// A9
#define X_DIR_PIN          62		// A8
#define X_ENABLE_PIN       48
#define X_MIN_PIN          22
#define X_MAX_PIN          30

#define Y_STEP_PIN         65		//A11
#define Y_DIR_PIN          64		//A10
#define Y_ENABLE_PIN       46
#define Y_MIN_PIN          24
#define Y_MAX_PIN          38

#define Z_STEP_PIN         67		//A13
#define Z_DIR_PIN          66		//A12
#define Z_ENABLE_PIN       44
#define Z_MIN_PIN          26
#define Z_MAX_PIN          34

#define E0_STEP_PIN        36
#define E0_DIR_PIN         28
#define E0_ENABLE_PIN      42

#define E1_STEP_PIN        43
#define E1_DIR_PIN         41
#define E1_ENABLE_PIN      39

#define E2_STEP_PIN        32
#define E2_DIR_PIN         47
#define E2_ENABLE_PIN      45
/*
#define SDPOWER            -1
#define SDSS               53
#define LED_PIN            13

#define FAN_PIN            9 // (Sprinter config)

#define CONTROLLERFAN_PIN  10 //Pin used for the fan to cool controller

#define PS_ON_PIN          12

#define KILL_PIN           41
#define HEATER_0_PIN       8
#define HEATER_1_PIN       9    // EXTRUDER 2 (FAN On Sprinter)

#define TEMP_0_PIN         13   // ANALOG NUMBERING
#define TEMP_1_PIN         15   // ANALOG NUMBERING
#define TEMP_2_PIN         -1   // ANALOG NUMBERING

#define HEATER_BED_PIN     -1    // NO BED
#define TEMP_BED_PIN       14   // ANALOG NUMBERING

#define SERVO0_PIN         11
#define SERVO1_PIN         6
#define SERVO2_PIN         5
#define SERVO3_PIN         4

// these pins are defined in the SD library if building with SD support  
#define MAX_SCK_PIN          52
#define MAX_MISO_PIN         50
#define MAX_MOSI_PIN         51
#define MAX6675_SS       53
*/
////////////////////////////////////////////////////////

