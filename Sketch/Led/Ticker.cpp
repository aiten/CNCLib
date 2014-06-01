#include <Arduino.h>
#include <FastSPI_LED.h>
#include <HALed.h>

#include "Led.h"
#include "Ticker.h"

///////////////////////////////////////////////////////////////

static COLOR backcolors[] = { 
	BL.Color(31, 0, 0),
	BL.Color(0, 31, 0),
	BL.Color(0, 0, 31),
	BL.Color(31, 31, 0),
	BL.Color(31, 0, 31),
	BL.Color(31, 10, 0),
	BL.Color(31, 31, 31)
};

///////////////////////////////////////////////////////////////

static byte backolorsidx = 0;


///////////////////////////////////////////////////////////////

static COLOR NextColor()
{
	backolorsidx = (backolorsidx + 1) % (sizeof(backcolors) / sizeof(COLOR));
	return backcolors[backolorsidx];
}

///////////////////////////////////////////////////////////////

static unsigned int Gap(int delaytime)
{
	BL.ScrollLeft(0);
	BL.Show();
	delay(delaytime);

	return NextColor();
}

///////////////////////////////////////////////////////////////

void TickerLoop(char* output, ETickerType style, int delayms)
{
	BL.SetAll(BL.Color(0, 0, 0));

	byte y;
	byte srcidx;
	byte stopidx = 0xff;
	int delaytimemove = 150;
	int delaytimeon = 1000;
	int delaytimeoff = 100;
	int delaytimestop = 1000;
	unsigned int col = BL.Color(random(0, 32), random(0, 32), random(0, 32));
	byte charofs = 0;

	if (style == ScrollSingleCharFadeInRightFadeOutRight || style == ScrollSingleCharFadeInRightFadeOutLeft || 
            style == ScrollSingleCharFadeInRightFadeOutUp    || style == ScrollSingleCharFadeInRightFadeOutDown)
        {
		delaytimemove = 10;
        }

	char* ch = output;
	const byte* out;

	while (*ch != NULL)
	{
		byte charYsize = 0;
		charYsize = BL.ScrollInRight(*ch, delaytimemove, col);

		switch (style)
		{
			case ScrollFromRight:
				col = Gap(delaytimemove);
				break;

			case ScrollSingleCharFadeInRightFadeOutRight:
                        {
				byte dist = (XSIZE - charYsize) / 2;
				BL.ScrollRangeLeft(dist, delaytimemove, 0);
				delay(delaytimeon);
				BL.ScrollRangeRight(dist + charYsize, delaytimemove, 0);
				col = Gap(delaytimeoff);
				break;
                        }
			case ScrollSingleCharFadeInRightFadeOutLeft:
                        {
				byte dist = (XSIZE - charYsize) / 2;
				BL.ScrollRangeLeft(dist, delaytimemove, 0);
				delay(delaytimeon);
				BL.ScrollRangeLeft(((XSIZE - 1 + charYsize)) - (dist + charYsize), delaytimemove, 0);
				col = Gap(delaytimeoff);
				break;
                        }
			case ScrollSingleCharFadeInRightFadeOutUp:
                        {
				byte dist = (XSIZE - charYsize) / 2;
				BL.ScrollRangeLeft(dist, delaytimemove, 0);
				delay(delaytimeon);
				BL.ScrollRangeUp((YSIZE - 1), delaytimemove, 0);
				col = Gap(delaytimeoff);
				break;
                        }
			case ScrollSingleCharFadeInRightFadeOutDown:
                        {
				byte dist = (XSIZE - charYsize) / 2;
				BL.ScrollRangeLeft(dist, delaytimemove, 0);
				delay(delaytimeon);
				BL.ScrollRangeDown((YSIZE - 1), delaytimemove, 0);
				col = Gap(delaytimeoff);
                        }
		}

		ch++;
	}

	BL.ScrollRangeLeft((XSIZE - 1), delaytimemove, 0);
}
