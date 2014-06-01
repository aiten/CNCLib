//////////////////////////////////////////

#ifdef _MSC_VER

#define _CRT_SECURE_NO_WARNINGS
#define _AFX_SECURE_NO_WARNINGS 

#endif

//////////////////////////////////////////

#include "Utilities.h"

////////////////////////////////////////////////////////

unsigned char ToPrecision10(unsigned short v)
{
	if (v >= 10000) return 5;
	if (v >= 1000) return 4;
	if (v >= 100) return 3;
	if (v >= 10) return 2;
	if (v >= 1) return 1;
	return 0;
}

unsigned char ToPrecision10(unsigned long v)
{
	if (v >= 100000000) return 9;
	if (v >= 10000000) return 8;
	if (v >= 1000000) return 7;
	if (v >= 100000) return 6;
	if (v >= 10000) return 5;
	if (v >= 1000) return 4;
	if (v >= 100) return 3;
	if (v >= 10) return 2;
	if (v >= 1) return 1;
	return 0;
}

////////////////////////////////////////////////////////

unsigned char ToPrecision2(unsigned short v)
{
	register unsigned char i = 0;
	for (; v != 0; i++)
	{
		v /= 2;
	}
	return i;
}

unsigned char ToPrecision2(unsigned long v)
{
	register unsigned char i = 0;
	for (; v != 0; i++)
	{
		v /= 2;
	}
	return i;
}

////////////////////////////////////////////////////////

unsigned long _ulsqrt_round(unsigned long val)
{
	unsigned long temp, g, b, bshft;
	g = 0;
	b = 0x8000;
	bshft = 15;
	unsigned char i;
	for (i = 0; i < 15; i++)
	{
		temp = g;
		temp = temp + (b >> 1);
		temp = temp << (bshft + 1);
		if (val >= temp)
		{
			g += b;
			val -= temp;
		}
		bshft--;
		b = b >> 1;
	}
	temp = (g << 1);
	temp = temp | 1;
	if (val >= temp)
	{
		g += b;
		val -= temp;
	}

	if (val > g)
		g++;

	return g;
}

////////////////////////////////////////////////////////

unsigned long _ulsqrt(unsigned long val)
{
	unsigned long temp, g, b, bshft;
	g = 0;
	b = 0x8000;
	bshft = 15;
	unsigned char i;
	for (i = 0; i < 15; i++)
	{
		temp = g;
		temp = temp + (b >> 1);
		temp = temp << (bshft + 1);
		if (val >= temp)
		{
			g += b;
			val -= temp;
		}
		bshft--;
		b = b >> 1;
	}
	temp = (g << 1);
	temp = temp | 1;
	if (val >= temp)
	{
		g += b;
		val -= temp;
	}

	return g;
}

////////////////////////////////////////////////////////
// right alligned with precision and scale  (TODO: +round to scale)

char* CMm1000::ToString(mm1000_t pos, char*tmp, unsigned char precision, unsigned char scale)
{
	bool isNegativ = pos < 0;
	if (isNegativ)
	{
		pos = -pos;
	}

	//round
	switch (scale)
	{
		case 0: pos += 500; break;
		case 1: pos += 50; break;
		case 2: pos += 5; break;
	}

	unsigned char x = ToPrecision10((unsigned long)pos);
	if (x < 4) x = 4;	// 0..999 => 0.000
	if (isNegativ) x++;

	if (scale == 0)
	{
		x -= 3;	// with dot
	}
	else if (scale <= 3)
	{
		// need dot
		x++;
		x -= 3 - scale;
	}
	else
	{
		x += scale - 3;
	}

	if (x > precision)
	{
		// overflow
		tmp[0] = 'X';
		tmp[1] = 0;
	}
	else
	{
		tmp[precision--] = 0;

		ldiv_t ud = ldiv(pos, 1000);

		// reuse of x
		udiv_t sud;

		if (scale > 0)
		{
			for (x = scale; x > 3; x--)
				tmp[precision--] = '0';

			sud.quot = (unsigned short)ud.rem;
			for (x = 3; x > 0; x--)
			{
				sud = udiv(sud.quot, 10);
				if (x <= scale)
					tmp[precision--] = '0' + (unsigned char)sud.rem;
			}
			tmp[precision--] = '.';
		}

		sud.quot = (unsigned short)ud.quot;
		do 
		{
			sud = udiv(sud.quot, 10);
			tmp[precision--] = '0' + (unsigned char) sud.rem;
		} 
		while (sud.quot != 0);
		
		if (isNegativ) 
			tmp[precision--] = '-';

		while (precision != ((unsigned char)-1))
			tmp[precision--] = ' ';
	}

	return tmp;
}

/*
char* CMm1000::ToString(mm1000_t pos, char*tmp, unsigned char precision, unsigned char scale)
{
	tmp[0] = tmp[1] = tmp[2] = tmp[3] = tmp[4] = ' ';
	char* t = tmp + 5;

	if (pos < 0)
	{
		pos = -pos;
		*(t++) = '-';
	}

	ldiv_t ud = ldiv(pos, 1000);

	unsigned short mm = (unsigned short)ud.quot;
	unsigned short rem = (unsigned short)ud.rem;

	unsigned char len;

	if (scale == 0)
	{
		if (rem > 500) mm++;
		_itoa(mm, t, 10);
	}
	else
	{
		_itoa(mm, t, 10);
		unsigned char len = (unsigned char)strlen(t);
		t[len++] = '.';
		unsigned char r = len;

		if (rem < 100)
			t[len++] = '0';
		if (rem < 10)
			t[len++] = '0';
		_itoa(rem, t + len, 10);

		if (scale>3)
		{
			// fill with 0
			r += 3;
			for (;scale>3;scale--)
			{
				t[r++] = '0';
			}
			t[r++] = 0;
		}
		else
		{
			t[r+3-(3-scale)] = 0;
		}

	}

	len = (unsigned char)strlen(t);
	if (precision > len)
		t = t - (precision - len);

	return t;
}
*/
////////////////////////////////////////////////////////////

char* CMm1000::ToString(mm1000_t pos, char*tmp,unsigned char scale)
{
	char* t = ToString(pos,tmp,9,scale);
	while(*t == ' ')
		t++;

	return t;
}

////////////////////////////////////////////////////////////
