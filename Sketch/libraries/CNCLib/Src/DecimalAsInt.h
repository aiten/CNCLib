////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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

template <typename T, unsigned char SCALE, unsigned long SCALEMASK> class CDecimaAsInt
{
protected:

	T	_value;

public:

	CDecimaAsInt()		{ _value = 0; }
	CDecimaAsInt(T v)	{ _value = v; }

protected:

	// make a wrapper to the function to avoid inline => protected

	static char* ToString(long pos, char*tmp, unsigned char precision, unsigned char scale)
	{
		bool isNegativ = pos < 0;

		//round
		if (isNegativ)
		{
			switch (scale)
			{
				case 0: pos -= SCALEMASK / 2; break;
				case 1: pos -= SCALEMASK / 20; break;
				case 2: pos -= SCALEMASK / 200; break;
				case 3: pos -= SCALEMASK / 2000; break;
				case 4: pos -= SCALEMASK / 20000; break;
				case 5: pos -= SCALEMASK / 200000; break;
			}
		}
		else
		{
			switch (scale)
			{
				case 0: pos += SCALEMASK / 2; break;
				case 1: pos += SCALEMASK / 20; break;
				case 2: pos += SCALEMASK / 200; break;
				case 3: pos += SCALEMASK / 2000; break;
				case 4: pos += SCALEMASK / 20000; break;
				case 5: pos += SCALEMASK / 200000; break;
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
				tmp[precision - scale - 1] = '.';
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

				ldiv_t sud;
				sud.quot = ud.rem;

				for (x = SCALE; x > 0; x--)
				{
					sud = ldiv(sud.quot, 10);
					if (x <= scale)
						tmp[precision--] = '0' + (unsigned char)sud.rem;
				}
				tmp[precision--] = '.';
			}

			do
			{
				ud = ldiv(ud.quot, 10);
				tmp[precision--] = '0' + (unsigned char)ud.rem;
			} while (ud.quot != 0);

			if (isNegativ)
				tmp[precision--] = '-';

			while (precision != ((unsigned char)-1))
				tmp[precision--] = ' ';
		}

		return tmp;
	}

	static char *SkipSpaces(char*t) { while ((*t) == ' ') t++; return t; };

public:

	static T Convert(float v) { return (T)v; }										// do not use lrint => convert to double first

	static float DegreeToRAD(T v)	{ return (float)(v / (SCALEMASK * 180.0 / M_PI)); }
	static T FromRAD(float v)		{ return Convert((float)(v * SCALEMASK * 180 / M_PI)); }

	static float ConvertToFloat(T v) { return float(v) / SCALEMASK; }
};

//////////////////////////////////////////

class CMm1000 : public CDecimaAsInt<mm1000_t,3,1000>
{
private:

	typedef CDecimaAsInt super;

public:

	static char* ToString(mm1000_t v, char*tmp, unsigned char precision, unsigned char scale);	// right aligned
	static char* ToString(mm1000_t v, char*tmp, unsigned char scale)		{ return SkipSpaces(ToString(v, tmp, 11, scale)); };

	char*ToString(char*tmp, unsigned char precision, unsigned char scale)	{ return ToString(_value, tmp, precision, scale); } // right aligned
	char*ToString(char*tmp, unsigned char scale)							{ return ToString(_value, tmp, scale); }

};

//////////////////////////////////////////

class CInch100000 : public CDecimaAsInt<inch100000_t,5, 100000>
{
private:

	typedef CDecimaAsInt super;

private:

	inch100000_t	_value;

public:

	static char* ToString(inch100000_t v, char*tmp, unsigned char precision, unsigned char scale);	// right aligned
	static char* ToString(inch100000_t v, char*tmp, unsigned char scale)	{ return SkipSpaces(ToString(v, tmp, 11, scale)); }
	;

	char*ToString(char*tmp, unsigned char precision, unsigned char scale)	{ return ToString(_value, tmp, precision, scale); } // right aligned
	char*ToString(char*tmp, unsigned char scale)							{ return ToString(_value, tmp, scale); }
};


////////////////////////////////////////////////////////

class CSDist
{
private:
	sdist_t	_value;

public:

	CSDist()	{ _value = 0; }
	CSDist(sdist_t v)	{ _value = v; }

	static char* ToString(sdist_t v, char*tmp, unsigned char precision);		// right aligned
};

//////////////////////////////////////////

