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

#include <StepperLib.h>

////////////////////////////////////////////////////////

#define FreqToTone(a) ((unsigned int) ((a*10)+0.5))

enum ETone
{
//	ToneA7 = FreqToTone(3520),
	ToneD7 = FreqToTone(2349.32),
	ToneA6 = FreqToTone(1760),
	ToneD6 = FreqToTone(1175.66),
	ToneA5 = FreqToTone(880),
	ToneD5 = FreqToTone(554.365),
	ToneA4 = FreqToTone(440),
	ToneD4 = FreqToTone(294.665),
	ToneA3 = FreqToTone(220),
	ToneD3 = FreqToTone(146.832),
	ToneA2 = FreqToTone(110),
	ToneD2 = FreqToTone(73.4162),
	ToneA1 = FreqToTone(55)
};

struct SPlayTone
{
	enum ETone Tone;					// 0 => end
	unsigned char Durationin100Sec;
};

////////////////////////////////////////////////////////

template <unsigned char PIN>
class CBeep
{
public:

	CBeep(enum ETone freq = ToneA4)
	{
		_durationin100Sec = 10;
		_freq = freq;
	}

	static void Init()
	{
		CHAL::pinMode(PIN, OUTPUT);
		CHAL::digitalWrite(PIN, LOW);
	}

	static void Beep(ETone freq, unsigned char durationin100Sec)
	{
		unsigned long endmillis = millis() + durationin100Sec * 10l;
		unsigned int tonePause = 10 * 1000000l / freq / 2;

		do
		{
			CHAL::digitalWrite(PIN, HIGH);
			CHAL::delayMicroseconds(tonePause);
			CHAL::digitalWrite(PIN, LOW);
			CHAL::delayMicroseconds(tonePause);
		} while (millis() < endmillis);
	}

	static void Play(const SPlayTone* list)
	{
		ETone freq = (ETone)pgm_read_ptr(&list->Tone);
		while (freq != 0)
		{
			Beep(freq, pgm_read_byte(&list->Durationin100Sec));
			freq = (ETone)pgm_read_ptr(&list->Tone);
		}
	}

	void Beep(unsigned char durationin100Sec = 0)
	{
		Beep(_freq, durationin100Sec ?  durationin100Sec : _durationin100Sec);
	}

private:

	unsigned char _durationin100Sec;
	ETone		  _freq;

};

////////////////////////////////////////////////////////





