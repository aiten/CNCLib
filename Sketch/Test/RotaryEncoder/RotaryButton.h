 #pragma once

////////////////////////////////////////////////////////

template <class rang>
class CRotaryButton
{
public:

	CRotaryButton()
	{
		Pos = 0;
		_lastchangedA = false;
		_lastPinValue = 0;
                _lastadd=0;
	}

	void Tick(unsigned char pinAValue, unsigned char pinBValue)
	{
		unsigned char pinvalue;
		signed char add = 0;

		if (_lastchangedA)									// ignore A and wait until B change
		{						
  			if (pinBValue != _lastPinValue)	// B change => A must be stable
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
			if (pinAValue != _lastPinValue)		// A change => B must be stable
			{
				_lastchangedA = true;
				_lastPinValue = pinBValue;
				if (pinAValue != LOW)    						// from LOW => HIGH
					add = _lastPinValue == LOW ? +1 : -1;
				else
					add = _lastPinValue != LOW ? +1 : -1;
			}
		}

		if (add != 0)										// chech for chaned of direction
		{
			Pos += add;
			if (add != _lastadd)
			{
				Pos += add;
				_lastadd = add;
			}
		}
	}

	volatile rang   Pos;
	
protected:

	bool 		_lastchangedA;
	unsigned char 	_lastPinValue;
	signed char 	_lastadd;

};

////////////////////////////////////////////////////////

