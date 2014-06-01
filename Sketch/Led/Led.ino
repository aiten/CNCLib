#include <FastSPI_LED.h>
#include <HALed.h>

// +5   => rot
// GND  => blau
// 52   => grün
// 51   => weiss

#include "Led.h"
#include "Ball.h"
#include "Clock.h"
#include "Ticker.h"
#include "Wheel.h"
#include "Box.h"

COLOR Display[NUM_LEDS];  

void setup() 
{
  Serial.begin(9600);
  randomSeed(analogRead(0));
  
  BL.gridX = 15;
  BL.gridY = 10;
  BL.Init(NUM_LEDS,Display);
}

void loop() 
{
	BoxLoop(10000);
	WheelLoop(10000);
	BallLoop(100000,true);
	ClockLoop(9,false,1000);
        TickerLoop("Herbert Aitenbichler",ScrollFromRight,1000);
	ClockLoop(5,false,2000);
	TickerLoop("He",ScrollSingleCharFadeInRightFadeOutRight,1000);
	TickerLoop("rb",ScrollSingleCharFadeInRightFadeOutLeft,1000);
	TickerLoop("er",ScrollSingleCharFadeInRightFadeOutUp,1000);
	TickerLoop("t",ScrollSingleCharFadeInRightFadeOutDown,1000);
	ClockLoop(25,true,250);
}
