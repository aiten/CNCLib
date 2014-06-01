#pragma once

#include "StepperArduino.h"

////////////////////////////////////////////////////////

class StepperL298N : public StepperArduino
{
public:

	virtual void Init(void);
	virtual void Remove(void);

protected:

        static unsigned char _ports[NUM_AXIS][4];

	virtual void  SetPhase(unsigned char level, unsigned char coordinate, unsigned char stepidx);

        static void FullStep(unsigned char* ports, unsigned char bitmask);
        static void HalfStep(unsigned char* ports, unsigned char bitmask);

	////////////////////////////////////////////////////////
};








