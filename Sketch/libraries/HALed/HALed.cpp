/******************************************************************************
 * Includes
 ******************************************************************************/

#include <Arduino.h>
#include <FastSPI_LED.h>
#include "HALed.h"      

/******************************************************************************/

HALed BL;

/******************************************************************************/

void HALed::Init(LEDINDEX leds, COLOR* display)
{
	gridX = 15;
	gridY = 10;

	_leds = leds;
	_display=display;
	SetAll(0);

	// setup/run the fast spi library
	FastSPI_LED.setLeds(leds);

	FastSPI_LED.setChipset(CFastSPI_LED::SPI_LPD6803);
	FastSPI_LED.setCPUPercentage(50); 
	FastSPI_LED.init();
	FastSPI_LED.start();

	BL.Show();
}

/******************************************************************************/

void HALed::Show()
{
	// copy data from display into the rgb library's output - we need to expand it back out since 
	// the rgb library expects values from 0-255 (because it's more generically focused).
	unsigned char *pData = FastSPI_LED.getRGBData();
	for(LEDINDEX i=0; i < _leds; i++) 
	{ 
		byte r = (_display[i] & 0x1F) * 8;
		byte g = ((_display[i] >> 10) & 0x1F) * 8;
		byte b = ((_display[i] >> 5) & 0x1F) * 8;

		*pData++ = r;
		*pData++ = g;
		*pData++ = b;
	}
	FastSPI_LED.setDirty();
}

/******************************************************************************/

unsigned int HALed::Color(byte r, byte g, byte b)           //Take the lowest 5 bits of each value and append them end to end
{
	//Take the lowest 5 bits of each value and append them end to end
	return( ((unsigned int)g & 0x1F )<<10 | ((unsigned int)b & 0x1F)<<5 | (unsigned int)r & 0x1F);
}

/******************************************************************************/

void HALed::ColorToRGB(COLOR col, byte& r, byte& g, byte& b)
{
	r = (col & 0x1F);
	g = ((col >> 10) & 0x1F);
	b = ((col >> 5) & 0x1F);
}

/******************************************************************************/

void HALed::SetPixel(LEDINDEX led, COLOR color)
{
    _display[led]=color;  
}

/******************************************************************************/

void HALed::SetPixel(COORDINATE x, COORDINATE y, COLOR color)
{
	LEDINDEX led = Translate(x,y);
	if (led!=NOLEDIDX)
	    _display[led]=color;  
}

/******************************************************************************/

LEDINDEX HALed::Count(COLOR color)
{
	LEDINDEX cnt=0;
	for(LEDINDEX i=0; i < _leds; i++) 
	{ 
		if (_display[i] == color)
			cnt++;
	}
	return cnt;
}

/******************************************************************************/
 
COLOR HALed::Wheel(byte WheelPos)
{
	byte r,g,b;
	switch(WheelPos >> 5)
	{
		case 0:
			r=31- WheelPos % 32;   //Red down
			g=WheelPos % 32;      // Green up
			b=0;                  //blue off
			break; 
		case 1:
			g=31- WheelPos % 32;  //green down
			b=WheelPos % 32;      //blue up
			r=0;                  //red off
			break; 
		case 2:
			b=31- WheelPos % 32;  //blue down 
			r=WheelPos % 32;      //red up
			g=0;                  //green off
			break; 
	}
	return(BL.Color(r,g,b));
}

/******************************************************************************/

void HALed::SetAll(COLOR color )
{
	SetRange(0, _leds-1,color );
}

/******************************************************************************/

void HALed::SetRange(LEDINDEX startLED, LEDINDEX endLED, COLOR color )
{
	// set a linear range of LEDs. The color value must be created with the Color function (15 bit rgb)
	while(startLED <= endLED)
		SetPixel(startLED++, color);
}

/******************************************************************************/

//Translate x and y to a LED index number in an array.
//Assume LEDS are layed out in a zig zag manner eg for a 3x3:
//0 5 6
//1 4 7
//2 3 8

LEDINDEX HALed::Translate(COORDINATE x, COORDINATE y)
{
	x=gridX-x-1;
//	y=gridY-y-1;

	if (x >= gridX || y >= gridY)
		return NOLEDIDX;

	if(x%2)
	{
		return(((x+1) * gridY)- 1 - y);
	}
	else
	{
		return((x * gridY) + y);
	}
}

/******************************************************************************/

LEDINDEX HALed::Translate(COORDINATE x, COORDINATE y,COORDINATE x0, COORDINATE y0,COORDINATE x1, COORDINATE y1)
{
	if (x < x0 || x > x1 || y < y0 || y > y1 )
		return NOLEDIDX;

	return Translate (x,y);
}

/******************************************************************************/

////Swap the values of two variables, for use when drawing lines.
void Swap(int * a, int * b)
{
  byte temp;
  temp=*b;
  *b=*a;
  *a=temp;
}

// Draw a line in defined color between two points
// Using Bresenham's line algorithm, optimized for no floating point.

void HALed::Line(COORDINATE bx0,  COORDINATE by0, COORDINATE bx1, COORDINATE by1, COLOR color)
{
	int x0=bx0;  int y0=by0; int x1=bx1; int y1=by1;
     boolean steep;
     steep= abs(y1 - y0) > abs(x1 - x0);
     if (steep)
    {
         Swap(&x0, &y0);
         Swap(&x1, &y1);
    }
     if (x0 > x1)
    {
         Swap(&x0, &x1);
         Swap(&y0, &y1);
    }
     int deltax = x1 - x0;
     int deltay = abs(y1 - y0);
     int error = 0;
     int ystep;
     int y = y0;
     int x;
     if (y0 < y1) 
       ystep = 1; 
     else 
       ystep = -1;
     for (x=x0; x<=x1; x++) // from x0 to x1
       {
         if (steep)
          SetPixel((Translate(y,x)),color);
         else 
           SetPixel((Translate(x,y)),color);
         error = error + deltay;
         if (2 * error >= deltax)
           {
           y = y + ystep;
           error = error - deltax;
           }
       }
}

/******************************************************************************/

void HALed::Box(COORDINATE x0, COORDINATE y0, COORDINATE x1, COORDINATE y1, COLOR color)
{
  Line(x0,y0,x1,y0,color);
  Line(x1,y0,x1,y1,color);
  Line(x1,y1,x0,y1,color);
  Line(x0,y1,x0,y0,color);  
}

/******************************************************************************/

void HALed::Fill(COORDINATE x0, COORDINATE y0, COORDINATE x1, COORDINATE y1, COLOR color)
{
	COORDINATE x,y;
	for(x=x0;x<=x1;x++)
	{
		for(y=y0;y<=y1;y++)
		{
			_display[Translate(x,y)] = color;
		}
	}
}

/******************************************************************************/
/* => should be linear */
/*
void HALed::FadeOut(COORDINATE x0, COORDINATE y0, COORDINATE x1, COORDINATE y1, unsigned int delaytime)
{
	COORDINATE x,y;
	for (char maxrange = 31;maxrange > 0;maxrange -= 1)
	{
		for(x=x0;x<=x1;x++)
		{
			for(y=y0;y<=y1;y++)
			{
				COORDINATE led = Translate(x,y);
				byte r,g,b;
				ColorToRGB(_display[led],r,g,b);
				if (r>=1) r -= 1;
				if (g>=1) g -= 1;
				if (b>=1) b -= 1;
				_display[Translate(x,y)] = Color(r,g,b);
			}
		}
		Show();
		delay(delaytime);
	}
}
*/
/******************************************************************************/

void HALed::ScrollLeft(COORDINATE x0, COORDINATE y0, COORDINATE x1, COORDINATE y1, COLOR fillcol)
{
	COORDINATE x,y;  
	for(y=y0;y<=y1;y++)
	{
		for(x=x0+1;x<=x1;x++)
		{
			_display[Translate(x-1,y)] = _display[Translate(x,y)];
		}
		if (fillcol!=NoColor)
			_display[Translate(gridX-1,y)] = fillcol;
	}
}

/******************************************************************************/

void HALed::ScrollRight(COORDINATE x0, COORDINATE y0, COORDINATE x1, COORDINATE y1, COLOR fillcol)
{
  signed char x,y;  
  for(y=y0;y<=y1;y++)
  {
    for(x=x1-1;x>=x0;x--)
    {
      _display[Translate(x+1,y)] = _display[Translate(x,y)];
    }
    if (fillcol!=NoColor)
      _display[Translate(0,y)] = fillcol;
  }
}

/******************************************************************************/

void HALed::ScrollDown(COORDINATE x0, COORDINATE y0, COORDINATE x1, COORDINATE y1, COLOR fillcol)
{
  COORDINATE x,y;  
  for(x=x0;x<=x1;x++)
  {
    for(y=y0+1;y<=y1;y++)
    {
      _display[Translate(x,y-1)] = _display[Translate(x,y)];
    }
    if (fillcol!=NoColor)
		_display[Translate(x,gridY-1)] = fillcol;
  }
}

/******************************************************************************/

void HALed::ScrollUp(COORDINATE x0, COORDINATE y0, COORDINATE x1, COORDINATE y1, COLOR fillcol)
{
  signed char x,y;  
  for(x=x0;x<=x1;x++)
  {
    for(y=y1-1;y>=y0;y--)
    {
      _display[Translate(x,y+1)] = _display[Translate(x,y)];
    }
    if (fillcol!=NoColor)
		_display[Translate(x,0)] = fillcol;
  }
}

/******************************************************************************/

void HALed::Scroll(COORDINATE x0, COORDINATE y0, COORDINATE x1, COORDINATE y1, char dx, char dy, COLOR fillcol)
{
	if (dx > 0)
		ScrollRight(x0, y0, x1, y1,fillcol);
	else if (dx < 0)
		ScrollLeft(x0, y0, x1, y1,fillcol);

	if (dy > 0)
		ScrollUp(x0, y0, x1, y1,fillcol);
	else if (dy < 0)
		ScrollDown(x0, y0, x1, y1,fillcol);
}

/******************************************************************************/

void HALed::ScrollRange(byte d, COORDINATE x0, COORDINATE y0, COORDINATE x1, COORDINATE y1, char dx, char dy, unsigned int delaytime, COLOR col)
{
	for (byte b=0;b<d;b++)
	{
		Scroll(x0, y0, x1, y1, dx, dy, col);
		Show();
		delay(delaytime);
	}
}

/******************************************************************************/

byte HALed::PrintChar(char ch, COORDINATE x, COORDINATE y, COLOR col, COLOR fillcol)
{
	const byte* outend;
	byte charXsize;
	char shift;
	const byte* out = ToAsciiArray(ch,outend,charXsize,shift);
shift=0;

	for (byte dx=0;((*out) & 0x80) == 0;out++,dx++)
	{
		byte mask=1;
		for (COORDINATE dy=6;dy!=0xff;dy--)
		{
		  LEDINDEX idx = Translate(x+dx,y+dy+shift);
		  if (idx != NOLEDIDX)
			_display[idx] = (*out & mask)!=0 ? col : fillcol;
		  mask *= 2;
		}
	}
	return charXsize;
}

/******************************************************************************/

byte HALed::PrintChar(char ch, COORDINATE x0, COORDINATE y0, COORDINATE x1, COORDINATE y1, COLOR col, COLOR fillcol)
{
	Fill(x0,y0,x1,y1,fillcol);

	const byte* outend;
	byte charXsize;
	char shift;
	const byte* out = ToAsciiArray(ch,outend,charXsize,shift);

	signed char ddx=(((((int)x1-(int)x0)-(int) charXsize))+1)/2;
	signed char ddy=(((((int)y1-(int)y0)-(int) 7))+1)/2;

	for (byte dx=0;((*out) & 0x80) == 0;out++,dx++)
	{
		byte mask=1;
		for (COORDINATE y=6;y!=0xff;y--)
		{
		  LEDINDEX idx = Translate(x0+dx+ddx,y+y0+shift+ddy,x0,y0,x1,y1);
		  if (idx != NOLEDIDX && (*out & mask)!=0)
			_display[idx] = col;
		  mask *= 2;
		}
	}
	return charXsize;
}

/******************************************************************************/

const byte* HALed::ToAsciiArray(char ch, const byte*& outend, byte& charXsize, char& shift)
{
	const byte* out = ToAsciiArray(ch);
	outend=out;
	charXsize=0;
	shift=0;

	for (charXsize=0;((*outend) & 0x80) == 0;outend++,charXsize++)
	{
	}
	shift = 0 - (*outend&7);

	return out;
}

/******************************************************************************/

const byte* HALed::ToAsciiArray(char ch)
{
  switch (ch)
  {
	case '-':  return __Minus;
	case '+':  return __Plus;
	case '*':  return __Mult;
	case '.':  return __Dot;
	case '!':  return __Exp;
	case '/':  return __Div;
	case ',':  return __Comma;
//	case '?':
  }

  if (ch >= 'A' && ch<= 'Z')
  {
	return __ABC[ch-'A'];
  }
  else if (ch >= 'a' && ch<= 'z')
  {
	return __abc[ch-'a'];
  }
  else if (ch >= '0' && ch<= '9')
  {
	return __0123[ch-'0'];
  }

  return __Blank;
}

/******************************************************************************/

byte HALed::ScrollInRight(char ch, unsigned int delaytime, COLOR col, COLOR fillcol)
{
	const byte* outend;
	byte charXsize;
	char shift;
	const byte* out = ToAsciiArray(ch,outend,charXsize,shift);
	shift += 2;

	for (;((*out) & 0x80) == 0;out++)
	{
		byte mask=1;
		ScrollLeft(0);
		//        for (y=YSIZE-1;y!=0xff;y--)
		for (COORDINATE y=6;y!=0xff;y--)
		{
		  _display[Translate(gridX-1,y+shift)] = (*out & mask)!=0 ? col : fillcol;
		  mask *= 2;
		}
		Show();
		delay(delaytime);
	}
	return charXsize;
}

/******************************************************************************/

const COLOR HALed::NoColor  = 0xffff;
const COLOR HALed::OffColor = 0;

/******************************************************************************/

const byte HALed::__Blank[] =
{
  0b00000000, 
  0b10000000 
};

const byte HALed::__Dot[] =
{
  0b01000000,
  0b10000000 
};

const byte HALed::__Comma[] =
{
  0b01000000,
  0b00110000,
  0b10000001 
};

const byte HALed::__Minus[] =
{
  0b00001000,
  0b00001000,
  0b00001000,
  0b10000000 
};

const byte HALed::__Plus[] =
{
  0b00001000,
  0b00011100,
  0b00001000,
  0b10000000 
};

const byte HALed::__Mult[] =
{
  0b00101010,
  0b00011100,
  0b00011100,
  0b00101010,
  0b10000000 
};

const byte HALed::__Div[] =
{
  0b00100000,
  0b00010000,
  0b00001000,
  0b00000100,
  0b00000010,
  0b10000000 
};

const byte HALed::__Exp[] =
{
  0b01011111,
  0b10000000 
};


const byte HALed::__A[] =
{
  0b01111100,
  0b00010010,
  0b00010001,
  0b00010010,
  0b01111100,
  0b10000000 
};
const byte HALed::__B[] =
{
  0b01111111,
  0b01001001,
  0b01001001,
  0b00110110,
  0b10000000 
};
const byte HALed::__C[] =
{
  0b00111110,
  0b01000001,
  0b01000001,
  0b00100010,
  0b10000000 
};
const byte HALed::__D[] =
{
  0b01111111,
  0b01000001,
  0b01000001,
  0b00111110,
  0b10000000 
};
const byte HALed::__E[] =
{
  0b01111111,
  0b01001001,
  0b01001001,
  0b01001001,
  0b10000000 
};
const byte HALed::__F[] =
{
  0b01111111,
  0b00001001,
  0b00001001,
  0b00001001,
  0b10000000 
};
const byte HALed::__G[] =
{
  0b00111110,
  0b01000001,
  0b01001001,
  0b00111010,
  0b10000000 
};
const byte HALed::__H[] =
{
  0b01111111,
  0b00001000,
  0b00001000,
  0b01111111,
  0b10000000 
};
const byte HALed::__I[] =
{
  0b01000001,
  0b01111111,
  0b01000001,
  0b10000000 
};
const byte HALed::__J[] =
{
  0b00100000,
  0b01000000,
  0b01000000,
  0b00111111,
  0b10000000 
};
const byte HALed::__K[] =
{
  0b01111111,
  0b00010100,
  0b00100010,
  0b01000001,
  0b10000000 
};
const byte HALed::__L[] =
{
  0b01111111,
  0b01000000, 
  0b01000000, 
  0b01000000, 
  0b10000000 
};
const byte HALed::__M[] =
{
  0b01111111,
  0b00000010,
  0b00000100,
  0b00000010,
  0b01111111,
  0b10000000 
};
const byte HALed::__N[] =
{
  0b01111111,
  0b00000010,
  0b00001000,
  0b00100000,
  0b01111111,
  0b10000000 
};
const byte HALed::__O[] =
{
  0b00111110,
  0b01000001,
  0b01000001,
  0b00111110,
  0b10000000 
};
const byte HALed::__P[] =
{
  0b01111111,
  0b00001001,
  0b00001001,
  0b00000110,
  0b10000000 
};
const byte HALed::__Q[] =
{
  0b00111110,
  0b01000001,
  0b01000001,
  0b00111110,
  0b01000000,
  0b10000000 
};
const byte HALed::__R[] =
{
  0b01111111,
  0b00011001,
  0b00101001,
  0b01000110,
  0b10000000 
};
const byte HALed::__S[] =
{
  0b01000110,
  0b01001001,
  0b01001001,
  0b00110001,
  0b10000000 
};
const byte HALed::__T[] =
{
  0b00000001,
  0b00000001,
  0b01111111,
  0b00000001,
  0b00000001,
  0b10000000 
};
const byte HALed::__U[] =
{
  0b00111111,
  0b01000000,
  0b01000000,
  0b00111111,
  0b10000000 
};
const byte HALed::__V[] =
{
  0b00011111,
  0b00100000,
  0b01000000,
  0b00100000,
  0b00011111,
  0b10000000 
};
const byte HALed::__W[] =
{
  0b00011111,
  0b00100000,
  0b01000000,
  0b00110000,
  0b01000000,
  0b00100000,
  0b00011111,
  0b10000000 
};
const byte HALed::__X[] =
{
  0b01100011,
  0b00010100,
  0b00001000,
  0b00010100,
  0b01100011,
  0b10000000 
};
const byte HALed::__Y[] =
{
  0b00000011,
  0b00000100,
  0b01111000,
  0b00000100,
  0b00000011,
  0b10000000 
};
const byte HALed::__Z[] =
{
  0b01000001,
  0b01010001,
  0b01000101,
  0b01000001,
  0b10000000 
};

const byte HALed::__a[] =
{
  0b00100000,
  0b01010100,
  0b01010100,
  0b01111000,
  0b10000000 
};

const byte HALed::__b[] =
{
  0b01111111,
  0b01000100,
  0b01000100,
  0b00111000,
  0b10000000 
};

const byte HALed::__c[] =
{
  0b00111000,
  0b01000100,
  0b01000100,
  0b10000000 
};

const byte HALed::__d[] =
{
  0b00111000,
  0b01000100,
  0b01000100,
  0b01111111,
  0b10000000 
};

const byte HALed::__e[] =
{
  0b00111000,
  0b01010100,
  0b01010100,
  0b00011000,
  0b10000000 
};

const byte HALed::__f[] =
{
  0b00000100,
  0b01111110,
  0b00000101,
  0b10000000 
};

const byte HALed::__g[] =
{
  0b00001100,
  0b01010010,
  0b01010010,
  0b00111110,
  0b10000001 
};

const byte HALed::__h[] =
{
  0b01111111,
  0b00000100,
  0b00000100,
  0b01111000,
  0b10000000 
};

const byte HALed::__i[] =
{
  0b01111101,
  0b10000000 
};

const byte HALed::__j[] =
{
  0b01000000,
  0b01000000,
  0b00111101,
  0b10000000 
};

const byte HALed::__k[] =
{
  0b01111111,
  0b00010000,
  0b00101000,
  0b01000100,
  0b10000000 
};

const byte HALed::__l[] =
{
  0b00111111,
  0b01000000,
  0b01000000,
  0b10000000 
};

const byte HALed::__m[] =
{
  0b01111000,
  0b00000100,
  0b01111000,
  0b00000100,
  0b01111000,
  0b10000000 
};

const byte HALed::__n[] =
{
  0b01111100,
  0b00000100,
  0b00000100,
  0b01111000,
  0b10000000 
};

const byte HALed::__o[] =
{
  0b00111000,
  0b01000100,
  0b01000100,
  0b00111000,
  0b10000000 
};

const byte HALed::__p[] =
{
  0b01111100,
  0b00100100,
  0b00100100,
  0b00011000,
  0b10000000 
};

const byte HALed::__q[] =
{
  0b00011000,
  0b00100100,
  0b00100100,
  0b01111100,
  0b10000000 
};

const byte HALed::__r[] =
{
  0b01111100,
  0b00000100,
  0b00001000,
  0b10000000 
};

const byte HALed::__s[] =
{
  0b00001000,
  0b01010100,
  0b01010100,
  0b00100000,
  0b10000000 
};

const byte HALed::__t[] =
{
  0b00000100,
  0b00111111,
  0b01000100,
  0b10000000 
};

const byte HALed::__u[] =
{
  0b00111100,
  0b01000000,
  0b01000000,
  0b00111100,
  0b10000000 
};

const byte HALed::__v[] =
{
  0b00011100,
  0b00100000,
  0b01000000,
  0b00100000,
  0b00011100,
  0b10000000 
};

const byte HALed::__w[] =
{
  0b01111100,
  0b00100000,
  0b00010000,
  0b00100000,
  0b01111100,
  0b10000000 
};

const byte HALed::__x[] =
{
  0b01000100,
  0b00101000,
  0b00010000,
  0b00101000,
  0b01000100,
  0b10000000 
};

const byte HALed::__y[] =
{
  0b00001100,
  0b01010000,
  0b01010000,
  0b00111100,
  0b10000000 
};

const byte HALed::__z[] =
{
  0b01100100,
  0b01010100,
  0b01010100,
  0b01001100,
  0b10000000 
};

const byte HALed::__0[] =
{
  0b00111110,
  0b01010001,
  0b01000101,
  0b00111110,
  0b10000000 
};
const byte HALed::__1[] =
{
  0b01000010,
  0b01000001,
  0b01111111,
  0b01000000,
  0b10000000 
};

const byte HALed::__2[] =
{
  0b01100010,
  0b01010001,
  0b01001001,
  0b01000110,
  0b10000000 
};
const byte HALed::__3[] =
{
  0b01000001,
  0b01001001,
  0b01001001,
  0b00110110,
  0b10000000 
};
const byte HALed::__4[] =
{
  0b00011111,
  0b00010000,
  0b01111100,
  0b00010000,
  0b10000000 
};
const byte HALed::__5[] =
{
  0b01001111,
  0b01001001,
  0b01001001,
  0b00110001,
  0b10000000 
};
const byte HALed::__6[] =
{
  0b00111110,
  0b01001001,
  0b01001001,
  0b00110000,
  0b10000000 
};
const byte HALed::__7[] =
{
  0b01110001,
  0b00001001,
  0b00000101,
  0b00000011,
  0b10000000 
};
const byte HALed::__8[] =
{
  0b00110110,
  0b01001001,
  0b01001001,
  0b00110110,
  0b10000000 
};
const byte HALed::__9[] =
{
  0b00000110,
  0b01001001,
  0b01001001,
  0b00111110,
  0b10000000 
};

const byte* HALed::__ABC[] =
{
    __A,__B,__C,__D,__E,__F,__G,__H,__I,__J,__K,__L,__M,__N,__O,__P,__Q, __R,__S,__T,__U,__V,__W,__X,__Y,__Z
};  
const byte* HALed::__abc[] =
{
    __a,__b,__c,__d,__e,__f,__g,__h,__i,__j,__k,__l,__m,__n,__o,__p,__q, __r,__s,__t,__u,__v,__w,__x,__y,__z
};  
const byte* HALed::__0123[] =
{
    __0,__1,__2,__3,__4,__5,__6,__7,__8,__9
};  
