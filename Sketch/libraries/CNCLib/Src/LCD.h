////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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
////////////////////////////////////////////////////////

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
	virtual void TimerInterrupt();

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





