#include <Arduino.h>
#include <FastSPI_LED.h>
#include <HALed.h>

#include "Led.h"
#include "Ball.h"

///////////////////////////////////////////////////////////////

#define SLEEP 25
#define RANDOMCHANGE 100
#define SNAKELENGTH 14  
#define RANDOMXY 4 

///////////////////////////////////////////////////////////////

static char NextPos(char& pos, char& d, char MAXSIZE)
{
	pos += d;

	if (pos >= MAXSIZE)
	{
		d = -1;
		pos = MAXSIZE - 2;
		return 1;
	}
	else if (pos < 0)
	{
		d = 1;
		pos = 1;
		return -1;
	}
	return 0;
}

///////////////////////////////////////////////////////////////

void BallLoop(unsigned long maxtime, bool exitIfAll)
{
	byte backcolorsidx = 1;
	COLOR backcolors[] = { BL.Color(1, 0, 0), BL.Color(0, 0, 0),
		BL.Color(0, 1, 0), BL.Color(0, 0, 0),
		BL.Color(0, 0, 1), BL.Color(0, 0, 0),
		BL.Color(1, 1, 0), BL.Color(0, 0, 0),   //gelb
		BL.Color(1, 0, 1), BL.Color(0, 0, 0),
		BL.Color(3, 1, 0), BL.Color(0, 0, 0)
	};
	COLOR backcolold = backcolors[backcolorsidx - 1];
	LEDINDEX setbackcol = 0;
	LEDINDEX changecoloridx = NUM_LEDS;

	BL.SetAll(backcolold);

	COLOR cols[SNAKELENGTH];
	LEDINDEX last[SNAKELENGTH];
	char lastidx = 0;
	char x = -1, y = -1;
	char dx = 1, dy = 1;

	char j;
	byte colrgb = 32;
	for (j = 2; j < SNAKELENGTH - 1; j++)
	{
		cols[j] = BL.Color(colrgb - 1, colrgb - 1, colrgb - 1);
		colrgb /= 2;

		if (colrgb < 2)
			colrgb = 2;
	}

	cols[0] = BL.Color(31, 0, 0);
	cols[1] = BL.Color(0, 0, 31);
	cols[SNAKELENGTH - 2] = BL.Color(0, 31, 0);
	cols[SNAKELENGTH - 1] = backcolors[backcolorsidx];

	for (j = 0; j < SNAKELENGTH; j++)
	{
		last[j] = NUM_LEDS;
	}

        unsigned long exittime = millis()+maxtime;

	while (millis() < exittime)
	{
		char lastx = x;
		char lasty = y;

		char res = NextPos(x, dx, XSIZE);
		if (res != 0)
		{
			if (dy == 0)
				dy = random(0, 2) == 0 ? 1 : -1; //-res;
			else if (random(0, RANDOMXY) == 0)
			{
				dy = 0;
			}
		}

		res = NextPos(y, dy, YSIZE);
		if (res != 0)
		{
			if (dx == 0)
			{
				dx = random(0, 2) == 0 ? 1 : -1; // -res;
				NextPos(x, dx, XSIZE);
			}
			else if (random(0, RANDOMXY) == 0)
			{
				x = lastx;
				dx = 0;
			}
		}

		last[lastidx] = BL.Translate(x, y);
		bool pause = false;

		COLOR col = Display[last[lastidx]];

		if (col == backcolold && BL.Count(backcolold) == 1)
		{
			{
				backcolold = backcolors[backcolorsidx];
				setbackcol = 1;
				backcolorsidx = (backcolorsidx + 1) % (sizeof(backcolors) / sizeof(COLOR));
				changecoloridx = last[lastidx];
				pause = true;
			}
		}

		if (changecoloridx != NUM_LEDS)
		{
			LEDINDEX idx = last[(lastidx + 1) % SNAKELENGTH];
			if (idx == changecoloridx)
			{
				cols[SNAKELENGTH - 1] = backcolors[backcolorsidx];
				changecoloridx = NUM_LEDS;
			}
		}

		for (j = SNAKELENGTH - 1; j >= 0; j--)  // last idx is 0
		{
			LEDINDEX idx = last[((SNAKELENGTH + lastidx) - j) % SNAKELENGTH];
			if (idx < NUM_LEDS)
				Display[idx] = cols[j];
		}

		lastidx = (lastidx + 1) % SNAKELENGTH;

		BL.Show();
		delay(SLEEP);

		if (pause)
                {
			delay(1000);
                        if (exitIfAll)
                          return;
                }
	}
}
