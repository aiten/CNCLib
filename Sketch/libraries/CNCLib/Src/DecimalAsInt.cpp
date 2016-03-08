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

#ifdef _MSC_VER

#define _CRT_SECURE_NO_WARNINGS
#define _AFX_SECURE_NO_WARNINGS 

#endif

////////////////////////////////////////////////////////

#include "CNCLib.h"
#include "DecimalAsInt.h"

////////////////////////////////////////////////////////

char* CMm1000::ToString(mm1000_t pos, char*tmp, unsigned char precision, unsigned char scale)
{
	// right alligned with precision and scale  (+round to scale)
	// call the base class only here to avoid multiple inlines of a big function

	return super::ToString(pos, tmp, precision, scale);
}

////////////////////////////////////////////////////////

char* CInch100000::ToString(inch100000_t pos, char*tmp, unsigned char precision, unsigned char scale)
{
	return super::ToString(pos, tmp, precision, scale);
}

////////////////////////////////////////////////////////////

char* CSDist::ToString(sdist_t v, char*tmp, unsigned char precision)
{
	// right aligned
	tmp[precision] = 0;						// terminating 0

	unsigned char x = ToPrecisionS10(v);
	if (x == 0) x = 1;						// 0 => Precision = 0
	if (v < 0) x++;							// add -

	if (precision < x)
	{
		for (x = 0; x < precision; x++)
			tmp[x] = 'x';
	}
	else
	{
		precision = precision - x;
		for (x = 0; x < precision; x++)
			tmp[x] = ' ';
		_ltoa(v, tmp + precision, 10);
	}

	return tmp;
}
