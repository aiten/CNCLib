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

//#define FreqToTone(a) ((unsigned int) ((a*10.0)+0.5))
#define FreqToTone(a) ((unsigned int) ((1000000/2/a)+0.5))

enum ETone
{
	ToneB8	= FreqToTone(7902),
	ToneBb8 = FreqToTone(7459),	
	ToneA8	= FreqToTone(7040),
	ToneAb8 = FreqToTone(6645),
	ToneG8	= FreqToTone(6272),
	ToneGb8 = FreqToTone(5920),
	ToneF8	= FreqToTone(5588),
	ToneE8	= FreqToTone(5274),
	ToneEb8 = FreqToTone(4978),
	ToneD8	= FreqToTone(4699),
	ToneDb8 = FreqToTone(4435),
	ToneC8	= FreqToTone(4186.01),
	ToneB7	= FreqToTone(3951.07),
	ToneBb7 = FreqToTone(3729.31),
	ToneA7	= FreqToTone(3520),
	ToneAb7 = FreqToTone(3322.44),
	ToneG7	= FreqToTone(3135.96),
	ToneGb7 = FreqToTone(2959.96),
	ToneF7	= FreqToTone(2793.83),
	ToneE7	= FreqToTone(2637.02),
	ToneEb7 = FreqToTone(2489.02),
	ToneD7	= FreqToTone(2349.32),
	ToneDb7 = FreqToTone(2217.46),
	ToneC7	= FreqToTone(2093),
	ToneB6	= FreqToTone(1975.53),
	ToneBb6 = FreqToTone(1864.66),
	ToneA6	= FreqToTone(1760),
	ToneAb6 = FreqToTone(1661.22),
	ToneG6	= FreqToTone(1567.98),
	ToneGb6 = FreqToTone(1479.98),
	ToneF6	= FreqToTone(1396.91),
	ToneE6	= FreqToTone(1318.51),
	ToneEb6 = FreqToTone(1244.51),
	ToneD6	= FreqToTone(1174.66),
	ToneDb6 = FreqToTone(1108.73),
	ToneC6	= FreqToTone(1046.5),
	ToneB5	= FreqToTone(987.767),
	ToneBb5 = FreqToTone(932.328),
	ToneA5	= FreqToTone(880),
	ToneAb5 = FreqToTone(830.609),
	ToneG5	= FreqToTone(783.991),
	ToneGb5 = FreqToTone(739.989),
	ToneF5	= FreqToTone(698.456),
	ToneE5	= FreqToTone(659.255),
	ToneEb5 = FreqToTone(622.254),
	ToneD5	= FreqToTone(587.33),
	ToneDb5 = FreqToTone(554.365),
	ToneC5	= FreqToTone(523.251),
	ToneB4	= FreqToTone(493.883),
	ToneBb4 = FreqToTone(466.164),
	ToneA4	= FreqToTone(440),
	ToneAb4 = FreqToTone(415.305),
	ToneG4	= FreqToTone(391.995),
	ToneGb4 = FreqToTone(369.994),
	ToneF4	= FreqToTone(349.228),
	ToneE4	= FreqToTone(329.628),
	ToneEb4	= FreqToTone(311.127),
	ToneD4	= FreqToTone(293.665),
	ToneDb4 = FreqToTone(277.183),
	ToneC4	= FreqToTone(261.626),
	ToneB3	= FreqToTone(246.942),
	ToneBb3	= FreqToTone(233.082),
	ToneA3	= FreqToTone(220),
	ToneAb3 = FreqToTone(207.652),
	ToneG3	= FreqToTone(195.998),
	ToneGb3 = FreqToTone(184.997),
	ToneF3	= FreqToTone(174.614),
	ToneE3	= FreqToTone(164.814),
	ToneEb3 = FreqToTone(155.563),
	ToneD3	= FreqToTone(146.832),
	ToneDb3 = FreqToTone(138.591),
	ToneC3	= FreqToTone(130.813),
	ToneB2	= FreqToTone(123.471),
	ToneBb2 = FreqToTone(116.541),
	ToneA2	= FreqToTone(110),
	ToneAb2 = FreqToTone(103.826),
	ToneG2	= FreqToTone(97.9989),
	ToneGb2 = FreqToTone(92.4986),
	ToneF2 = FreqToTone(87.3071),
	ToneE2 = FreqToTone(82.4069),
	ToneEb2 = FreqToTone(77.7817),
	ToneD2	= FreqToTone(73.4162),
	ToneDb2 = FreqToTone(69.2957),
	ToneC2	= FreqToTone(65.4064),
	ToneB1	= FreqToTone(61.7354),
	ToneBb1 = FreqToTone(58.2705),
	ToneA1	= FreqToTone(55),
	ToneAb1 = FreqToTone(51.9131),
	ToneG1	= FreqToTone(48.9994),
	ToneGb1 = FreqToTone(46.2493),
	ToneF1	= FreqToTone(43.6535),
	ToneE1	= FreqToTone(41.2034),
	ToneEb1 = FreqToTone(38.8909),
	ToneD1	= FreqToTone(36.7081),
	ToneDb1 = FreqToTone(34.6478),
	ToneC1	= FreqToTone(32.7032),
	ToneB0	= FreqToTone(30.8677),
	ToneBb0 = FreqToTone(29.1352),
	ToneA0	= FreqToTone(27.5),

	ToneNo	= 1,
	ToneEnd	= 0
};

struct SPlayTone
{
	enum ETone Tone;					// 0 => end
	unsigned char Durationin100Sec;

	static const SPlayTone PlayOK[] PROGMEM;
	static const SPlayTone PlayError[] PROGMEM;
	static const SPlayTone PlayInfo[] PROGMEM;
};

////////////////////////////////////////////////////////

template <unsigned char PIN>
class CBeep
{
public:

	static void Init()
	{
		CHAL::pinMode(PIN, OUTPUT);
		CHAL::digitalWrite(PIN, LOW);
	}

	static void Beep(ETone freq, unsigned char durationin100Sec)
	{
		unsigned long endmillis = millis() + durationin100Sec * 10l;
//		unsigned int tonePause = ( 10l * 1000000l / 2l) / (unsigned int) freq;
		unsigned int tonePause = (unsigned int)freq;

		do
		{
			if (freq!=ToneNo)
			{
				CHAL::digitalWrite(PIN, HIGH);
				CHAL::delayMicroseconds(tonePause);
				CHAL::digitalWrite(PIN, LOW);
				CHAL::delayMicroseconds(tonePause);
			}
		} while (millis() < endmillis);
	}

	static void Play(const SPlayTone* list)
	{
		ETone freq = (ETone)pgm_read_int(&list->Tone);
		while (freq != 0)
		{
			Beep(freq, pgm_read_byte(&list->Durationin100Sec));
			list++;
			freq = (ETone)pgm_read_int(&list->Tone);
		}
	}

private:

};

////////////////////////////////////////////////////////





