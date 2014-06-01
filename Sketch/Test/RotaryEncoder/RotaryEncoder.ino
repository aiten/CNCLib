#include "Arduino.h"

#include "Fastio.h"
#include "RotaryButton.h"

CRotaryButton<unsigned int> button;

unsigned int lastReportedPos = 1;

void setup()
{
	Serial.begin(115200);
        pinMode(31,INPUT_PULLUP);
        pinMode(33,INPUT_PULLUP);
         button.Tick(READ(31),READ(33));
      button.Pos=0;

  OCR0B = 128;
  TIMSK0 |= (1<<OCIE0B);  

}

volatile int cnt=0;

void loop()
{
//	button.Tick(READ(31),READ(33));

        if (lastReportedPos != button.Pos)
	{
		Serial.print(cnt);
		Serial.print("Index:");
		Serial.print(button.Pos, DEC);
		Serial.println();
		lastReportedPos = button.Pos;
	}
}

ISR(TIMER0_COMPB_vect)
{
//   TIFR0 |= (1<< OCF0B);
  cnt++;
  button.Tick(READ(31),READ(33));
}


