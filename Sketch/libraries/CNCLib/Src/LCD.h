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

#include <StepperLib.h>
#include <Beep.h>

////////////////////////////////////////////////////////

class CLcd : public CSingleton<CLcd>
{
public:

	CLcd()														{ _nextdrawtime = 0; _splash = false; }

	enum EDrawType
	{
		DrawAll,
		DrawStepperPos
	};

	virtual void Init();
	virtual void DrawRequest(bool forcedraw, EDrawType draw);

	virtual void Idle(unsigned int idletime);
	virtual void TimerInterrupt();

	////////////////////////////////////////////////////////////

protected:

	virtual void FirstDraw() = 0;								// e.g. clear screen - called after splash timeout
	virtual void Draw(EDrawType draw) = 0;

	virtual unsigned long Splash() = 0;							// return time to display

	bool IsSplash()												{ return _splash; };

public:

	virtual void Beep(ETone freq, unsigned char durationin100Sec)=0;
	void OKBeep()												{ Beep(ToneA4, 10); }
	void ErrorBeep()											{ for (unsigned char i = 0; i < 4; i++) { Beep(ToneA4, 5); delay(50); }; }

	bool PostCommand(const __FlashStringHelper* cmd, Stream* output=NULL);
	bool PostCommand(char* cmd, Stream* output=NULL);

private:

	unsigned long _nextdrawtime;

	bool _splash;
};

////////////////////////////////////////////////////////





