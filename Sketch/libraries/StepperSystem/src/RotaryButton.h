#pragma once

////////////////////////////////////////////////////////

template <class rang_t, unsigned char ACCURACY>
class CRotaryButton
{
public:

	typedef unsigned char rotarypage_t;

	CRotaryButton()
	{
		_pos = 0;
		_lastchangedA = false;
		_lastPinValue = 0;
		_lastadd = 0;
		_minpos  = 0;
		_maxpos     = 127/ACCURACY;
		_overrunpos = false;
	}

	void Tick(unsigned char pinAValue, unsigned char pinBValue)
	{
		signed char add = 0;

		if (_lastchangedA)									// ignore A and wait until B change
		{
			if (pinBValue != _lastPinValue)					// B change => A must be stable
			{
				_lastchangedA = false;
				_lastPinValue = pinAValue;

				if (pinBValue != LOW)    					// from LOW => HIGH
					add = _lastPinValue != LOW ? +1 : -1;
				else
					add = _lastPinValue == LOW ? +1 : -1;
			}
		}
		else												// ignore B and wait until A change
		{
			if (pinAValue != _lastPinValue)					// A change => B must be stable
			{
				_lastchangedA = true;
				_lastPinValue = pinBValue;
				if (pinAValue != LOW)    					// from LOW => HIGH
					add = _lastPinValue == LOW ? +1 : -1;
				else
					add = _lastPinValue != LOW ? +1 : -1;
			}
		}

		if (add != 0)										// chech for change of direction
		{
			_pos += add;
			if (add != _lastadd)
			{
				_pos += add;
				_lastadd = add;
			}

			rang_t pos = GetPos();
			if (pos > _maxpos) 
			{
				if (_overrunpos) 
					_pos -= (_maxpos - _minpos +1) * ACCURACY;
				else
					_pos -= ACCURACY;
			}
			else if (pos < _minpos) 
			{
				if (_overrunpos) 
					_pos += (_maxpos - _minpos +1) * ACCURACY;
				else
					_pos += ACCURACY;
			}
		}
	}

	void SetMinMax(rang_t minpos, rang_t maxpos, bool overrun)	{ _minpos = minpos; _maxpos = maxpos; _overrunpos = overrun; }

	rang_t GetMin()												{ return _minpos; }
	rang_t GetMax()												{ return _maxpos; }
	bool GetOverrrunMode()										{ return _overrunpos; }

	rang_t GetFullRangePos()									{ return _pos; }

	rang_t GetPos()												{ return (_pos + ((_pos > 0) ? ACCURACY/2 : -(ACCURACY/2))) / ACCURACY; }
	void SetPos(rang_t pos)										{ _pos = pos * ACCURACY; }

	void SetPageIdx(rotarypage_t page)							{ SetPos(page); }
	rotarypage_t GetPageIdx(rotarypage_t pages)					{ rang_t rpage = GetPos()%pages; if (rpage < 0) rpage = pages+rpage; return (rotarypage_t) rpage; }

protected:

	volatile rang_t  _pos;
	rang_t			_minpos;
	rang_t			_maxpos;
	bool			_overrunpos;

	bool 			_lastchangedA;
	unsigned char 	_lastPinValue;
	signed char 	_lastadd;

};

////////////////////////////////////////////////////////
