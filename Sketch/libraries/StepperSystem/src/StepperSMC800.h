#pragma once

////////////////////////////////////////////////////////

#include "Stepper.h"

////////////////////////////////////////////////////////

#define SMC800_NUM_AXIS	3

////////////////////////////////////////////////////////

class CStepperSMC800 : public CStepper
{
private:
	typedef CStepper super;
public:

	CStepperSMC800();
	virtual void Init();
	virtual void Remove();

protected:

	enum ELevel
	{
		Level0 = 0,
		Level20 = 20,
		Level60 = 60,
		Level100 = 100
	};

public:

	virtual bool IsAnyReference()							{ return IsReference(0); };
	virtual bool IsReference(unsigned char referenceid);

protected:

	virtual void  Step(const unsigned char steps[NUM_AXIS], bool directionUp[NUM_AXIS]);
	virtual void  SetEnable(axis_t axis, unsigned char level);
	virtual unsigned char GetEnable(axis_t axis);

	virtual bool  MoveAwayFromReference(axis_t axis, unsigned char referenceid, sdist_t diff, steprate_t vMax);

	virtual void OnStart();

	////////////////////////////////////////////////////////

private:

	void InitMemVar();

	unsigned char _stepIdx[SMC800_NUM_AXIS];
	unsigned char _level[SMC800_NUM_AXIS];

	void   SetPhase(axis_t axis);
	static void OutSMC800Cmd(const unsigned char val);
};
