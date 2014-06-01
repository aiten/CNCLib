#include "StepperBase.h"
#include "StepperL298N.h"

#include <arduino.h>
#include <avr/interrupt.h>
#include <avr/io.h>

////////////////////////////////////////////////////////

static unsigned char L298Nhalfstep0[8] =  { 0, 0, 0, 0 };
static unsigned char L298Nhalfstep100[8]= { 1, 3, 2, 6, 4, 12, 8, 9 };

static unsigned char L298Nfullstep0[4] =  { 0, 0, 0, 0 };
static unsigned char L298Nfullstep100[4]= { 1, 2, 4, 8 };

////////////////////////////////////////////////////////

unsigned char StepperL298N::_ports[NUM_AXIS][4] = 
{
  { 2, 3, 4, 5 },
  { 6, 7, 8, 9 },
  { -1 },
  { -1 }
};
  

void StepperL298N::Init(void)
{
  register unsigned char i,n;
  
  for (i=0;i<NUM_AXIS;i++)
  {
    for (n=0;_ports[i][n]!=0xff;n++)
    {
      pinMode(_ports[i][n],   OUTPUT);
//      StepperSerial.print(_ports[i][n]);
//      StepperSerial.println(" as output");
    }
  }
  
  StepperArduino::Init();
}

////////////////////////////////////////////////////////

void StepperL298N::Remove(void)
{
  register unsigned char i,n;
  
  for (i=0;i<NUM_AXIS;i++)
  {
    for (n=0;_ports[i][n]!=0xff;n++)
    {
      pinMode(_ports[i][n],   INPUT);
    }
  }
  StepperArduino::Remove();  
}

////////////////////////////////////////////////////////

void StepperL298N::FullStep(unsigned char* ports, unsigned char bitmask)
{
//StepperSerial.print(bitmask);StepperSerial.println("");
  digitalWrite(ports[0],bitmask&1 ? HIGH : LOW);  
  digitalWrite(ports[1],bitmask&2 ? HIGH : LOW);  
  digitalWrite(ports[2],bitmask&4 ? HIGH : LOW);  
  digitalWrite(ports[3],bitmask&8 ? HIGH : LOW);  
}

////////////////////////////////////////////////////////

void StepperL298N::HalfStep(unsigned char* ports, unsigned char bitmask)
{
//StepperSerial.print(bitmask);StepperSerial.println("");
  digitalWrite(ports[0],bitmask&1 ? HIGH : LOW);  
  digitalWrite(ports[1],bitmask&2 ? HIGH : LOW);  
  digitalWrite(ports[2],bitmask&4 ? HIGH : LOW);  
  digitalWrite(ports[3],bitmask&8 ? HIGH : LOW);  
}

////////////////////////////////////////////////////////

void   StepperL298N::SetPhase(unsigned char level, unsigned char coordinate, unsigned char stepidx)
{
    register unsigned char i;
	if (coordinate<4 && _ports[coordinate][0]!=0xff)
	{
		if (_UseFullStep)
		{
			stepidx = stepidx&0x3; 
			switch (level)
			{
				case Level20:
				case Level60:
				case Level100: FullStep(_ports[coordinate],L298Nfullstep100[stepidx]);  break;
				case Level0:   FullStep(_ports[coordinate],0);   break;
			}
		}
		else
		{
			stepidx = stepidx&0x7; 
			switch (level)
			{
				case Level20:
				case Level60:
				case Level100: HalfStep(_ports[coordinate],L298Nhalfstep100[stepidx]);  break;
				case Level0:   HalfStep(_ports[coordinate],0);  break;
			}
		}
	}
}


