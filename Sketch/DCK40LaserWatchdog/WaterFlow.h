
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

extern class WaterFlow flow;

////////////////////////////////////////////////////////////

#define SAMPELCOUNT 16
#define SAMPELTIME  200

class WaterFlow
{
public:
	WaterFlow()
	{
	}

	void Init(uint8_t pin)
	{
		_countIdx = SAMPELCOUNT - 1;
		_countTime[_countIdx] = millis();
		Next();

		pinMode(pin, INPUT);
		attachInterrupt(digitalPinToInterrupt(pin), StaticISRPinRising, RISING);
	}

	unsigned int AvgCount(unsigned int timediff = 1000)
	{
		TestSampleTime();

		// no disable interrupts => cache

		unsigned char countIdx = _countIdx;
		unsigned long sumtime = 0;
		unsigned int  sumcount = 0;

		for (unsigned char diff = 1; diff < SAMPELCOUNT && sumtime < timediff; diff++)
		{
			unsigned char idx = PrevIndex(countIdx, diff);

			sumtime += _countTime[idx];
			sumcount += _count[idx];
		}

		return ScaleCount(sumcount, sumtime, timediff);
	}

private:

	static unsigned int ScaleCount(unsigned int count, unsigned long totaltime, unsigned long scaletotime)
	{
		return (unsigned long)count * scaletotime / totaltime;
	}

	static unsigned char NextIndex(unsigned char idx, unsigned char count)
	{
		return (unsigned char)((idx + count)) % (SAMPELCOUNT);
	}

	static unsigned char PrevIndex(unsigned char idx, unsigned char count)
	{
		return (idx >= count) ? idx - count : (SAMPELCOUNT)-(count - idx);
	}

	void TestSampleTime()
	{
		if (millis() > _countUntil)
			Next();
	}

	static void StaticISRPinRising()
	{
		flow.ISRPinRising();
	}

	void ISRPinRising()
	{
		TestSampleTime();
		_count[_countIdx]++;
	}

	void Next()
	{
		// prev
		_countTime[_countIdx] = millis() - _countTime[_countIdx];

		// set next (no cli => cache)
		uint8_t idxnext = (_countIdx + 1) % SAMPELCOUNT;
		_count[idxnext] = 0;
		_countTime[idxnext] = millis();
		_countUntil = _countTime[idxnext] + SAMPELTIME;
		_countIdx = idxnext;
	}

	unsigned long _countUntil;
	volatile unsigned char _countIdx = 0;
	volatile int _count[SAMPELCOUNT];
	unsigned long _countTime[SAMPELCOUNT];
};

////////////////////////////////////////////////////////////

