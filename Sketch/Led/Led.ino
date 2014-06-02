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

#include "KeyPad16.h"


COLOR Display[NUM_LEDS];

CKeyPad16 keypad;

void setup()
{
  Serial.begin(9600);
  randomSeed(analogRead(0));

  keypad.Init();

  BL.gridX = 15;
  BL.gridY = 10;
  BL.Init(NUM_LEDS, Display);
}

void KeyPadLoop(unsigned long maxtime);

void loop()
{
  TickerLoop("Press Key", ScrollFromRight, 1000);
  KeyPadLoop(10000);
  //return;
  BoxLoop(10000);
  WheelLoop(10000);
  BallLoop(100000, true);
  ClockLoop(9, false, 1000);
  TickerLoop("Herbert Aitenbichler", ScrollFromRight, 1000);
  ClockLoop(5, false, 2000);
  TickerLoop("He", ScrollSingleCharFadeInRightFadeOutRight, 1000);
  TickerLoop("rb", ScrollSingleCharFadeInRightFadeOutLeft, 1000);
  TickerLoop("er", ScrollSingleCharFadeInRightFadeOutUp, 1000);
  TickerLoop("t", ScrollSingleCharFadeInRightFadeOutDown, 1000);
  ClockLoop(25, true, 250);
}

void KeyPadLoop(unsigned long maxtime)
{
  unsigned int Counter, Counter2, Counter3;
  Counter = 0;

  unsigned long exittime = millis() + maxtime;

  BL.SetAll(BL.Color(0, 0, 0));

  while (millis() < exittime)
  {
    unsigned int keys = keypad.GetAllKeys();
    static unsigned int oldkeys = 0;

    if (keys != oldkeys)
    {

      BL.SetAll(BL.Color(0, 0, 0));

      oldkeys = keys;

      bool have = false;
      for (unsigned char key = 0 ; key < 16; key++)
      {
        if ((keys % 2) == 1)
        {
          if (have)
          {
//            Serial.print(F("-"));
          }
          else
          {
            have = true;
          }
          BL.SetPixel(key, BL.Color(31, 31, 31));
//          Serial.print((unsigned char )key + 1);
        }
        keys /= 2;
      }
/* 
      if (have)
        Serial.println();
      else
        Serial.println(F("no"));
*/        
    }
    BL.Show();
  }
}

