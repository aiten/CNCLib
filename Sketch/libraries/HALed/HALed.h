#pragma once

typedef byte LEDINDEX;
typedef byte COORDINATE;
typedef unsigned int COLOR;
#define NOLEDIDX -1

class HALed 
{ 
public:

	/////////////////////////////////////////////////////////////////////////////
 
	void Init(LEDINDEX numleds, COLOR display[]);
  
	COLOR* _display;  
	LEDINDEX _leds;

	COORDINATE gridY;
	COORDINATE gridX;

	static const COLOR NoColor;
	static const COLOR OffColor;
   
	/////////////////////////////////////////////////////////////////////////////

	void Show();  

	void SetRange(LEDINDEX startLED, LEDINDEX endLED, COLOR color );
	void SetAll(COLOR color );
	LEDINDEX Count(COLOR col);

	static COLOR Wheel(byte WheelPos);
	static COLOR Color(byte r, byte g, byte b);
	static void ColorToRGB(COLOR col, byte& r, byte& g, byte& b);

	void SetPixel(LEDINDEX LED, COLOR color);
	void SetPixel(COORDINATE x, COORDINATE y, COLOR color);
	
	// x/y system

	LEDINDEX Translate(COORDINATE x, COORDINATE y);
	LEDINDEX Translate(COORDINATE x, COORDINATE y,COORDINATE x0, COORDINATE y0,COORDINATE x1, COORDINATE y1);
	
	void Box(COORDINATE x0, COORDINATE y0, COORDINATE x1, COORDINATE y1, COLOR color);
	void Line(COORDINATE x0,  COORDINATE y0, COORDINATE x1, COORDINATE y1, COLOR color);
	void Fill(COORDINATE x0, COORDINATE y0, COORDINATE x1, COORDINATE y1, COLOR color);

	void Scroll(COORDINATE x0, COORDINATE y0, COORDINATE x1, COORDINATE y1, char dx, char dy, COLOR fillcol=NoColor); 

	void ScrollLeft (COORDINATE x0, COORDINATE y0, COORDINATE x1, COORDINATE y1, COLOR fillcol=NoColor);
	void ScrollRight(COORDINATE x0, COORDINATE y0, COORDINATE x1, COORDINATE y1, COLOR fillcol=NoColor);
	void ScrollDown (COORDINATE x0, COORDINATE y0, COORDINATE x1, COORDINATE y1, COLOR fillcol=NoColor);
	void ScrollUp   (COORDINATE x0, COORDINATE y0, COORDINATE x1, COORDINATE y1, COLOR fillcol=NoColor);
 
	void Scroll(char dx, char dy, COLOR fillcol=NoColor) { Scroll(0,0,gridX-1,gridY-1, dx, dy, fillcol); }; 
	void ScrollLeft (COLOR fillcol=NoColor)		{ ScrollLeft (0,0,gridX-1,gridY-1, fillcol); };
	void ScrollRight(COLOR fillcol=NoColor)		{ ScrollRight(0,0,gridX-1,gridY-1, fillcol); };
	void ScrollDown (COLOR fillcol=NoColor)		{ ScrollDown (0,0,gridX-1,gridY-1, fillcol); };
	void ScrollUp   (COLOR fillcol=NoColor)		{ ScrollUp   (0,0,gridX-1,gridY-1, fillcol); };

	void FadeOut(COORDINATE x0, COORDINATE y0, COORDINATE x1, COORDINATE y1, unsigned int delaytime); 

	void ScrollRange(byte d, COORDINATE x0, COORDINATE y0, COORDINATE x1, COORDINATE y1, char dx, char dy, unsigned int delaytime, unsigned int col=NoColor); 

	void ScrollRangeLeft (byte d, unsigned int delaytime, unsigned int col=NoColor)		{ ScrollRange(d,0,0,gridX-1,gridY-1, -1,0,delaytime, col); };
	void ScrollRangeRight(byte d, unsigned int delaytime, unsigned int col=NoColor)		{ ScrollRange(d,0,0,gridX-1,gridY-1,  1,0,delaytime, col); };
	void ScrollRangeUp   (byte d, unsigned int delaytime, unsigned int col=NoColor)		{ ScrollRange(d,0,0,gridX-1,gridY-1, 0, 1,delaytime, col); };
	void ScrollRangeDown (byte d, unsigned int delaytime, unsigned int col=NoColor)		{ ScrollRange(d,0,0,gridX-1,gridY-1, 0,-1,delaytime, col); };
	
	/// font
	
	static const byte* ToAsciiArray(char ch);
	static const byte* ToAsciiArray(char ch, const byte*& outend, byte& charXsize, char& shift);

	byte ScrollInRight(char ch, unsigned int delay, COLOR col, COLOR fillcol=OffColor); 
	byte PrintChar(char ch, COORDINATE x, COORDINATE y, COLOR col, COLOR fillcol=OffColor);
	byte PrintChar(char ch, COORDINATE x0, COORDINATE y0,COORDINATE x1, COORDINATE y1, COLOR col, COLOR fillcol=OffColor);	//center in area

private:

	static const byte __0[];
	static const byte __1[];
	static const byte __2[];
	static const byte __3[];
	static const byte __4[];
	static const byte __5[];
	static const byte __6[];
	static const byte __7[];
	static const byte __8[];
	static const byte __9[];

	static const byte __Blank[];
	static const byte __Dot[];
	static const byte __Comma[];
	static const byte __Minus[];
	static const byte __Plus[];
	static const byte __Mult[];
	static const byte __Div[];
	static const byte __Exp[];
	static const byte __A[];
	static const byte __B[];
	static const byte __C[];
	static const byte __D[];
	static const byte __E[];
	static const byte __F[];
	static const byte __G[];
	static const byte __H[];
	static const byte __I[];
	static const byte __J[];
	static const byte __K[];
	static const byte __L[];
	static const byte __M[];
	static const byte __N[];
	static const byte __O[];
	static const byte __P[];
	static const byte __Q[];
	static const byte __R[];
	static const byte __S[];
	static const byte __T[];
	static const byte __U[];
	static const byte __V[];
	static const byte __W[];
	static const byte __X[];
	static const byte __Y[];
	static const byte __Z[];
	static const byte __a[];
	static const byte __b[];
	static const byte __c[];
	static const byte __d[];
	static const byte __e[];
	static const byte __f[];
	static const byte __g[];
	static const byte __h[];
	static const byte __i[];
	static const byte __j[];
	static const byte __k[];
	static const byte __l[];
	static const byte __m[];
	static const byte __n[];
	static const byte __o[];
	static const byte __p[];
	static const byte __q[];
	static const byte __r[];
	static const byte __s[];
	static const byte __t[];
	static const byte __u[];
	static const byte __v[];
	static const byte __w[];
	static const byte __x[];
	static const byte __y[];
	static const byte __z[];
	
	static const byte* __ABC[];
	static const byte* __abc[];
	static const byte* __0123[];
	
};

extern HALed BL;

