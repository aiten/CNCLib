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

#ifdef _MSC_VER

#define _CRT_SECURE_NO_WARNINGS
#define _AFX_SECURE_NO_WARNINGS 

#endif

////////////////////////////////////////////////////////

#include "CNCLib.h"
#include "UtilitiesCNCLib.h"

////////////////////////////////////////////////////////
// right alligned with precision and scale  (+round to scale)

char* CMm1000::ToString(mm1000_t pos, char*tmp, unsigned char precision, unsigned char scale)
{
	bool isNegativ = pos < 0;

#define SCALE 3
#define SCALEMASK 1000

	//round
	if (isNegativ)
	{
		switch (scale)
		{
			case 0: pos -= 500; break;
			case 1: pos -= 50; break;
			case 2: pos -= 5; break;
		}
	}
	else
	{
		switch (scale)
		{
			case 0: pos += 500; break;
			case 1: pos += 50; break;
			case 2: pos += 5; break;
		}
	}

	unsigned char x = ToPrecisionS10(pos);
	if (x < (SCALE + 1)) x = (SCALE + 1);	// 0..999 => 0.000
	if (isNegativ) x++;

	if (scale == 0)
	{
		x -= SCALE;	// with dot
	}
	else if (scale <= SCALE)
	{
		// need dot
		x++;
		x -= SCALE - scale;
	}
	else
	{
		x += scale - SCALE;
	}

	if (x > precision)
	{
		// overflow
		for (x = 0; x < precision; x++)
			tmp[x] = 'x';
		if (scale > 0)
			tmp[precision-scale-1] = '.';
		if (isNegativ) 
			tmp[0] = '-';
		tmp[x] = 0;
	}
	else
	{
		tmp[precision--] = 0;

		ldiv_t ud = ldiv(pos, SCALEMASK);

		if (ud.quot < 0) ud.quot = -ud.quot;
		if (ud.rem < 0)  ud.rem = -ud.rem;

		if (scale > 0)
		{
			for (x = scale; x > SCALE; x--)
				tmp[precision--] = '0';

			udiv_t sud;
			sud.quot = (unsigned short)ud.rem;

			for (x = SCALE; x > 0; x--)
			{
				sud = udiv(sud.quot, 10);
				if (x <= scale)
					tmp[precision--] = '0' + (unsigned char)sud.rem;
			}
			tmp[precision--] = '.';
		}

		do 
		{
			ud = ldiv(ud.quot, 10);
			tmp[precision--] = '0' + (unsigned char) ud.rem;
		} 
		while (ud.quot != 0);
		
		if (isNegativ) 
			tmp[precision--] = '-';

		while (precision != ((unsigned char)-1))
			tmp[precision--] = ' ';
	}

	return tmp;
}

////////////////////////////////////////////////////////////

char* CMm1000::ToString(mm1000_t pos, char*tmp,unsigned char scale)
{
	char* t = ToString(pos,tmp,11,scale);
	while(*t == ' ')
		t++;

	return t;
}

////////////////////////////////////////////////////////////
// right aligned

char* CSDist::ToString(sdist_t v, char*tmp, unsigned char precision)
{
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

////////////////////////////////////////////////////////////

char* AddAxisName(char*buffer, axis_t axis)
{
	const char* axisname=NULL;
	switch (axis)
	{
		case X_AXIS:	axisname = PSTR("X"); break;
		case Y_AXIS:	axisname = PSTR("Y"); break;
		case Z_AXIS:	axisname = PSTR("Z"); break;
		case A_AXIS:	axisname = PSTR("A"); break;
		case B_AXIS:	axisname = PSTR("B"); break;
		case C_AXIS:	axisname = PSTR("C"); break;
	}
	if (axisname)
		strcat_P(buffer, axisname);
	
	return buffer;
}
