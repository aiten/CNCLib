
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/

#include <Adafruit_TiCoServo.h>
#define Servo Adafruit_TiCoServo

#define NUM_AXIS 4
#define DEFAULT_PULSE_WIDTH 1500
  
Servo _servo[NUM_AXIS];

////////////////////////////////////////////////////////////

unsigned int pos=1300;
#define STEPPERRANGE 1800							// MAX_PULSE_WIDTH - MIN_PULSE_WIDTH
#define CENTERPOSOPPSET ((2000-STEPPERRANGE)/2)		

void setup()
{
  Serial.begin(115200);
	_servo[0].attach(5);                    // do not change, see Adafruit_TiCoServo for available pins
	_servo[1].attach(6);
	_servo[2].attach(7);
	_servo[3].attach(8);

	for (unsigned char i = 0; i<NUM_AXIS; i++)
	{
  	  _servo[i].write(pos);
	} 
}

////////////////////////////////////////////////////////////

//int diff = 500;
int diff = 0;

void loop()
{
  _servo[0].write(pos);
   delay(5000);

   pos += diff;
   if (pos > 1700 || pos < 1000) 
   {
     diff = -diff;
     pos += diff;
     pos += diff;
   }
}

////////////////////////////////////////////////////////////
  
