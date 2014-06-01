#pragma once

////////////////////////////////////////////////////////

#include "Stepper.h"

////////////////////////////////////////////////////////

class CLcd
{
public:

	CLcd()														{ _nextdrawtime = 0; _splash = false; _lcd = this; }

	static CLcd* GetInstance()									{ return _lcd; }

	enum EDrawType
	{
		DrawAll,
		DrawStepperPos
	};

	virtual void Init();
	virtual void DrawRequest(bool forcedraw, EDrawType draw);

	virtual void Idle(unsigned int idletime);
	virtual void TimerInterrupt() = 0;

protected:

	virtual void FirstDraw() = 0;								// e.g. clear screen - called after splash timeout
	virtual void Draw(EDrawType draw) = 0;

	virtual unsigned long Splash() = 0;							// return time to display

	bool IsSplash()												{ return _splash; };

protected:

	static CLcd* _lcd;

private:

	unsigned long _nextdrawtime;

	bool _splash;

};

////////////////////////////////////////////////////////





