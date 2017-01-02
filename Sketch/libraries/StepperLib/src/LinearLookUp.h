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

//////////////////////////////////////////

template<class TInput, class TOutput>
class CLinearLookup
{
public:
	struct SLookupTable
	{
		TInput	input;
		TOutput	output;
	};

	typedef uint8_t index_t;

	TInput GetInput(index_t i) const
	{
#if defined(__AVR_ARCH__)

    if (TInput(1)/2!=0)
      return (TInput) pgm_read_float(&_pTable[i].input);
		if (sizeof(TInput) == 4)
			return (TInput) pgm_read_dword(&_pTable[i].input);
		if (sizeof(TInput) == 2)
			return (TInput) pgm_read_word(&_pTable[i].input);
		return (TInput)pgm_read_byte(&_pTable[i].input);
#else
		return _pTable[i].input;
#endif
	}

	TOutput GetOutput(index_t i) const
	{
#if defined(__AVR_ARCH__)
    if (TOutput(1)/2!=0)
  		return (TOutput) pgm_read_float(&_pTable[i].output);
		if (sizeof(TOutput) == 4)
			return (TOutput)pgm_read_dword(&_pTable[i].output);
		if (sizeof(TOutput) == 2)
			return (TOutput)pgm_read_word(&_pTable[i].output);
		return (TOutput)pgm_read_byte(&_pTable[i].output);
#else
		return _pTable[i].output;
#endif
	}

	TOutput LinearInterpolation(TInput input, index_t i) const
	{
		TInput   distinput = input - GetInput(i);
		TInput   diffinput = GetInput(i+1) - GetInput(i);
		TOutput  diffoutput = GetOutput(i+1) - GetOutput(i);

		//return pTable[i].output + ( distinput / diffinput  ) * diffoutput; => OK if TInput is float
		return GetOutput(i) + (distinput * diffoutput) / diffinput;
	}

	TOutput Lookup(TInput input) const
	{
		// table must be sorted!!!!
		// binary serach

		index_t c;
		index_t left = 0;
		index_t right = _tabelSize - 1;

		if (_tabelSize == 0)	return TOutput(0);

		while (true)
		{
			c = left + ((right - left) / 2);

			TInput val = GetInput(c);

			if (val == input)
			{
				// no linear approximation (we found the exact value)
				return GetOutput(c);
			}
			else
			{
				if (val > input)
				{
					if (c == 0)
					{
						// no approximation => input < first table entry 
						return GetOutput(c);
					}

					right = c - 1;
					if (left > right)
					{
						// linear approximation between c-1 and c
						return LinearInterpolation(input, c - 1);
					}
				}
				else
				{
					if (c == (_tabelSize - 1))
					{
						// no approximation => input > last table entry 
						return GetOutput(c);
					}

					left = c + 1;
					if (left > right)
					{
						// linear approximation between c and c+1
						return LinearInterpolation(input, c);
					}
				}
			}
		}
	}

	CLinearLookup(const SLookupTable* pTable, index_t tabelSize)
	{
		_tabelSize = tabelSize;
		_pTable = pTable;
	}

private:

	const SLookupTable* _pTable;
	uint8_t _tabelSize;

};
