#pragma once

////////////////////////////////////////////////////////

#include "Stepper.h"

////////////////////////////////////////////////////////

#if defined(__AVR_ATmega2560__) || defined(_MSC_VER) || defined(__SAM3X8E__)

// only available on Arduino Mega or due

////////////////////////////////////////////////////////

#define E0_AXIS A_AXIS
#define E1_AXIS B_AXIS

#define RAMPS14_ENDSTOPCOUNT 6

#undef  USE_A4998
#define USE_DRV58825

////////////////////////////////////////////////////////

class CStepperRamps14 : public CStepper
{
private:
	typedef CStepper super;
public:

	CStepperRamps14();
	virtual void Init();

protected:

	virtual void  SetEnable(axis_t axis, unsigned char level);
	virtual unsigned char GetEnable(axis_t axis);
	virtual void  Step(const unsigned char cnt[NUM_AXIS], unsigned char directionUp);

public:

	virtual bool IsReference(unsigned char referenceid);
	virtual bool IsAnyReference();

protected:

	////////////////////////////////////////////////////////

private:

	void InitMemVar();
};

#endif