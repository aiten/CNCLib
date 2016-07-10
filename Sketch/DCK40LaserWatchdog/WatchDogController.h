
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
  http://www.gnu.org/licenses/
*/

////////////////////////////////////////////////////////////

#include "WaterFlow.h"
#include "WatchDog.h"
#include "LinearLookup.h"

#define RELAY1_PIN	11
#define RELAY2_PIN	10

#define INPUT1_PIN	9
#define INPUT2_PIN	8
#define INPUT3_PIN	7

////////////////////////////////////////////////////////////

#define MINDRAW_INTERVALL 100

////////////////////////////////////////////////////////////


#define ALIVE_PIN 13
#define ALIVE_BLINK_RATE 250

#define WATCHDOG_PIN  RELAY1_PIN
#define WATCHDOG_ON LOW

#define WATERFLOW_PIN 2
#define WATCHDOG_MINFLOW  50

#define WATERTEMP_PIN A0

#define WATCHDOG_MINTEMPON  4
#define WATCHDOG_MINTEMPOFF 5
#define WATCHDOG_MAXTEMPON  35.0
#define WATCHDOG_MAXTEMPOFF 34.5

#define WATERTEMP_OVERSAMPLING 16

////////////////////////////////////////////////////////////

class WatchDogController
{
public:

	WatchDogController() {};

	void Setup();
	void Loop();

private:

	unsigned long _redrawtime = 0;
	unsigned int _secActive = 0;

	unsigned long _lastBlink = 0;
	bool  _blinkWasOn = true;
	unsigned long _lastDraw = 0;
	bool _drawLCDRequest = false;

	WaterFlow _flow;
	WatchDog _watchDog;

	float ReadTemp();

	bool IsWatchDogWaterFlowOn();
	bool IsWatchDogTempOn();

	bool IsWatchDogSW1On();
	bool IsWatchDogSW2On();
	bool IsWatchDogSW3On();

	bool IsWatchDogOn();

	void DrawLcd();

	float _currentTemp = 0.0;
	unsigned int _currentFlow = 0xffff;

	float _lastTemp = 0.0;
	unsigned int _lastFlow = 0xffff;

	bool _sw1On = true;
	bool _sw2On = true;
	bool _sw3On = true;

};

////////////////////////////////////////////////////////////

