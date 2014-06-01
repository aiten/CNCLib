#include <Arduino.h>
#include <FastSPI_LED.h>
#include <HALed.h>

#include "Led.h"
#include "Box.h"

///////////////////////////////////////////////////////////////

void BoxLoop(unsigned long maxtime)
{
	unsigned long exittime = millis() + maxtime;

	while (millis() < exittime)
	{
		BL.SetAll(0);
		for (byte x = 0; x < BL.gridY / 2; x++)
		{
			BL.Box(x, x, BL.gridX - 1 - x, BL.gridY - 1 - x, BL.Color(random(0, 32), random(0, 32), random(0, 32)));
			BL.Show();
			delay(200);
		}
	}

}

