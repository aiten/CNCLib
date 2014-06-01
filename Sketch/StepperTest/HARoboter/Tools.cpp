#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include "Settings.h"
#include <HAStepper.h>

#include "Global.h"
#include "Tools.h"

////////////////////////////////////////////////////////////

TimeDiff::TimeDiff()
{
  _time = millis();    
  _steps = Stepper.GetTotalSteps();
}

 void TimeDiff::Print()
{
	unsigned long steps = Stepper.GetTotalSteps()-_steps;
	unsigned long diff = Diff();
	StepperSerial.print(steps);
	StepperSerial.print(F("/"));
	StepperSerial.print(diff);
	StepperSerial.print(F("="));
	//    double d=sqrt(steps*1000/(double) diff);
	StepperSerial.print(steps*1000/diff);
	//    StepperSerial.print(d);
};
  
////////////////////////////////////////////////////////////

void Tools::ToPoint(int x, int y, int * dx, int * dy, float rad)
{
	float mysin=sin(rad);
	float mycos=cos(rad);

	*dx = (int) ((float)x * mycos + (float)y * mysin);
	*dy = (int) ((float)y * mycos - (float)x * mysin);
}

////////////////////////////////////////////////////////////

long Tools::Dist(unsigned  x1, unsigned y1, unsigned x2, unsigned y2)
{
	long dx = (x1 > x2) ? x1-x2 : x2-x1;
	long dy = (y1 > y2) ? y1-y2 : y2-y1;
	return dx*dx+dy*dy;
//	return sqrt(dx*dx + dy*dy);
}




