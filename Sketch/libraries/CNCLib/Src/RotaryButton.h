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

template <class range_t, uint8_t ACCURACY>
class CRotaryButton
{
public:

	typedef uint8_t rotarypage_t;

	enum ERotaryEvent
	{
		Nothing=0,
		RightTurn,
		LeftTurn,
		Overrun,
		Underflow,
	};

	EnumAsByte(ERotaryEvent) Tick()
	{
		return Tick(CHAL::digitalRead(_pin1), CHAL::digitalRead(_pin2));
	}

	EnumAsByte(ERotaryEvent) Tick(uint8_t pinAValue, uint8_t pinBValue)
	{
		uint8_t p = ToPos(pinAValue,pinBValue);
		if (p == _lastPos) return Nothing;

		signed char add = 0;
		if      (_lastPos == 3 && p == 0) add = 1;
		else if (_lastPos == 0 && p == 3) add = -1;
		else add = (signed char)p - (signed char)_lastPos;

		_lastPos = p;
		_pos += add;

		range_t pos = GetPos();
		if (pos > _maxpos)
		{
			if (_overrunpos)
				_pos -= (_maxpos - _minpos + 1) * ACCURACY;
			else
				_pos -= ACCURACY;

			return Overrun;
		}

		if (pos < _minpos)
		{
			if (_overrunpos)
				_pos += (_maxpos - _minpos + 1) * ACCURACY;
			else
				_pos += ACCURACY;

			return Underflow;
		}

		return add > 0 ? RightTurn : LeftTurn;

	}

	void SetMinMax(range_t minpos, range_t maxpos, bool overrun)	{ _minpos = minpos; _maxpos = maxpos; _overrunpos = overrun; }

	range_t GetMin()											{ return _minpos; }
	range_t GetMax()											{ return _maxpos; }
	bool GetOverrrunMode()										{ return _overrunpos; }

	range_t GetFullRangePos()									{ return _pos; }

	range_t GetPos()											{ return (_pos + ((_pos > 0) ? ACCURACY/2 : -(ACCURACY/2))) / ACCURACY; }
	void SetPos(range_t pos)									{ _pos = pos * ACCURACY; }

	void SetPageIdx(rotarypage_t page)							{ SetPos(page); }
	rotarypage_t GetPageIdx(rotarypage_t pages)					{ range_t rpage = GetPos()%pages; if (rpage < 0) rpage = pages+rpage; return (rotarypage_t) rpage; }

	void SetPin(pin_t pin1, pin_t pin2)
	{	
			_pin1=pin1; 
			_pin2=pin2; 
			CHAL::pinModeInputPullUp(_pin1);
			CHAL::pinModeInputPullUp(_pin2);
			_lastPos = ToPos(CHAL::digitalRead(_pin1), CHAL::digitalRead(_pin2));
	}

protected:

	static inline uint8_t ToPos(uint8_t pinAValue, uint8_t pinBValue)
	{
		if (pinAValue) return (pinBValue) ? 0 : 3;
		return (pinBValue) ? 1 : 2;
	}

	volatile range_t  _pos=0;
	range_t			_minpos=0;
	range_t			_maxpos=127 / ACCURACY;

	bool			_overrunpos = false;
	uint8_t 		_lastPos = 0;

	pin_t			_pin1		= 0;
	pin_t			_pin2		= 0;
};

////////////////////////////////////////////////////////
