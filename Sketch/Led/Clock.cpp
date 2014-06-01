#include <Arduino.h>
#include <FastSPI_LED.h>
#include <HALed.h>

#include "Led.h"
#include "Ball.h"

///////////////////////////////////////////////////////////////

#define CHANGECOLOR
//#define CHANGECOLORBAR

///////////////////////////////////////////////////////////////

static COLOR backcolors[] = {
	BL.Color(31, 31, 31),
	BL.Color(31, 0, 0),
	BL.Color(0, 31, 0),
	BL.Color(0, 0, 31),
	BL.Color(31, 31, 0),
	BL.Color(31, 0, 31),
	BL.Color(31, 10, 0)
};

static byte backolorsidx = 0;

///////////////////////////////////////////////////////////////

static COLOR NextColor()
{
#ifdef CHANGECOLOR
	backolorsidx = (backolorsidx + 1) % (sizeof(backcolors) / sizeof(COLOR));
#endif
	return backcolors[backolorsidx];
}

///////////////////////////////////////////////////////////////

void ClockLoop(unsigned int countuntil, bool countupanddown, int delayms)
{
        backolorsidx = 0;
        
	BL.SetAll(BL.Color(0, 0, 0));

	unsigned int counter=0;
	char d=1;

	COLOR col0 = NextColor();
	COLOR col1 = NextColor();
	COLOR col2 = NextColor();
	byte shift = 2;

	COLOR bcol = BL.Color(0, 0, 0);
	COLOR bcol2 = BL.Color(0, 0, 0);

	BL.SetAll(bcol2);

	const unsigned long addtime1 = delayms;
	const unsigned long addtime2 = delayms / BL.gridX;
	unsigned long nextmilli1 = addtime1;
	unsigned long nextmilli2 = addtime2;
	unsigned long startmillis=millis();
        byte rgb=1<<2;

	while (true)
	{
		counter += d;

                if (!countupanddown && d<0)
                  return;

		if (counter == countuntil || counter == 0)
		{
                        if (counter == 0)    // count down
                          return;

			d = -d;
		}

		col0 = NextColor();

		if (counter % 10 == 0)
		{
			col1 = NextColor();
		}
		if (counter % 100 == 0)
		{
			col2 = NextColor();
		}

		unsigned int cnt = counter;
		char d0 = (cnt % 10) + '0'; cnt /= 10;
		char d1 = (cnt % 10) + '0'; cnt /= 10;
		char d2 = (cnt % 10) + '0'; cnt /= 10;


		byte charofs = BL.PrintChar(d2, 0, shift, d2 == '0' ? bcol : col2, bcol) + 1;
		charofs += BL.PrintChar(d1, charofs, shift, d2 == '0' && d1 == '0' ? bcol : col1, bcol) + 1;
		charofs += BL.PrintChar(d0, charofs, shift, col0, bcol) + 1;

		BL.Show();

		byte i = 0;

		// wait 1sek
		while ((millis()-startmillis) < nextmilli1)
		{
			if ((millis()-startmillis) > nextmilli2 && i < BL.gridX)
			{
				LEDINDEX idx = BL.Translate(i, 0);

                                if (Display[idx] == 0)
  				    Display[idx] = BL.Color((rgb&1) *(1+i), ((rgb>>1)&1) *(1+i), ((rgb>>2)&1) *(1+i));
                                else
                                {  
#ifdef CHANGECOLORBAR
                                    if (i==0)
                                    {
                                      rgb *= 2; if (rgb > 4) rgb = 1;
                                    }
#endif                                    
                                    Display[idx] = 0;
                                }
                                
				i++;
				BL.Show();
				nextmilli2 += addtime2;
			}
		}
		nextmilli2 = nextmilli1 + addtime2;
		nextmilli1 += addtime1;
	}
}
