////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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

#include <StepperLib.h>
#include <Beep.h>

////////////////////////////////////////////////////////

class CControl;

////////////////////////////////////////////////////////

class CLcd : public CSingleton<CLcd>
{
	friend CControl;

public:

	CLcd()														{ _nextdrawtime = 0; _splash = false; }

	enum EDrawType
	{
		DrawForceAll,
		DrawAll,
		DrawStepperPos
	};

	virtual void Init();
	virtual void DrawRequest(EDrawType draw);

	////////////////////////////////////////////////////////////

protected:

	virtual void Poll();
	virtual void TimerInterrupt();

	virtual void Command(char* cmd);


	virtual void FirstDraw() = 0;								// e.g. clear screen - called after splash timeout
	virtual void Draw(EDrawType draw) = 0;

	virtual unsigned long Splash() = 0;							// return time to display

	bool IsSplash()												{ return _splash; };

public:

	virtual void Beep(const SPlayTone*)=0;
	void OKBeep()												{ Beep(SPlayTone::PlayOK); }
	void ErrorBeep()											{ Beep(SPlayTone::PlayError); }

	bool PostCommand(const __FlashStringHelper* cmd, Stream* output=NULL);
	bool PostCommand(char* cmd, Stream* output=NULL);

private:

	unsigned long _nextdrawtime;

	bool _splash;
};

////////////////////////////////////////////////////////





