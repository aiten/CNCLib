////////////////////////////////////////////////////////
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

	typedef unsigned char index_t;

	static TOutput LinearInterpolation(SLookupTable* pTable, TInput input, index_t i)
	{
		TInput   distinput   = input - pTable[i].input;
		TInput   diffinput   = pTable[i+1].input  - pTable[i].input;
		TOutput  diffoutput  = pTable[i+1].output - pTable[i].output;

		//return pTable[i].output + ( distinput / diffinput  ) * diffoutput; => OK if TInput is float
		return pTable[i].output + ( distinput * diffoutput )  / diffinput;
	}

	static TOutput Lookup(SLookupTable* pTable, index_t tabelSize, TInput input)
	{
		// table must be sorted!!!!
		// binary serach

		index_t c;
		index_t left = 0;
		index_t right = tabelSize - 1;

		if (tabelSize==0)	return TOutput(0);

		while (true)   
		{
			c = left + ((right - left) / 2);
 
			if (pTable[c].input == input)	
			{
				// no linear approximation (we found the exact value)
				return pTable[c].output;
			}
			else
			{
				if (pTable[c].input > input)
				{
					if (c == 0)
					{
						// no approximation => input < first table entry 
						return pTable[c].output;
					}

					right = c - 1;
					if (left > right)
					{
						// linear approximation between c-1 and c
						return LinearInterpolation(pTable, input, c - 1);
					}
				}
				else 
				{
					if (c == (tabelSize - 1))
					{
						// no approximation => input > last table entry 
						return pTable[c].output;
					}

					left = c + 1;
					if (left > right)
					{
						// linear approximation between c and c+1
						return LinearInterpolation(pTable, input, c);
					}
				}
			}
		}
	}

	CLinearLookup(SLookupTable* pTable, index_t tabelSize) : _tabelSize(tabelSize), _pTable(pTable)
	{
	}

	TOutput Lookup(TInput input)
	{
		return Lookup(_pTable,_tabelSize,input);
	}

private:

	SLookupTable* _pTable;
	unsigned char _tabelSize;

};
