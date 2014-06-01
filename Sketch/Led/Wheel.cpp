#include <Arduino.h>
#include <FastSPI_LED.h>
#include <HALed.h>

#include "Led.h"
#include "Wheel.h"

///////////////////////////////////////////////////////////////
//Input a value 0 to 127 to get a color value.
//The colours are a transition r - g -b - back to r

static unsigned int Wheel(byte WheelPos)
{
	byte r, g, b;
	switch (WheelPos >> 5)
	{
	case 0:
		r = 31 - WheelPos % 32;   //Red down
		g = WheelPos % 32;      // Green up
		b = 0;                  //blue off
		break;
	case 1:
		g = 31 - WheelPos % 32;  //green down
		b = WheelPos % 32;      //blue up
		r = 0;                  //red off
		break;
	case 2:
		b = 31 - WheelPos % 32;  //blue down 
		r = WheelPos % 32;      //red up
		g = 0;                  //green off
		break;
	}
	return(BL.Color(r, g, b));
}

///////////////////////////////////////////////////////////////

void WheelLoop(unsigned long maxtime)
{
	unsigned int Counter, Counter2, Counter3;
	Counter = 0;

	unsigned long exittime = millis() + maxtime;

	while (millis() < exittime)
	{
		Counter++;
		if (Counter>2000) Counter = 0;

		Counter3 = Counter * 1;
		for (Counter2 = 0; Counter2 < NUM_LEDS; Counter2++)
		{
			//      Display[Counter2+OFS_LEDS] = Wheel(Counter3%96);  //There's only 96 colors in this pallette.
			//      Counter3+=(96 / USE_LEDS);
			Display[Counter2] = Wheel(Counter3 % 96);  //There's only 96 colors in this pallette.
			Counter3 += (Counter2 % 6) == 0 ? 1 : 0;
		}
		BL.Show();
		delay(100);

	}

}

