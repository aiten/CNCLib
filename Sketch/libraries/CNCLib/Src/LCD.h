////////////////////////////////////////////////////////
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
////////////////////////////////////////////////////////

#pragma once

////////////////////////////////////////////////////////

#include <StepperLib.h>
#include "Beep.h"

////////////////////////////////////////////////////////

class CControl;

////////////////////////////////////////////////////////

class CLcd : public CSingleton<CLcd>
{
	friend CControl;

public:

	CLcd()														{  }

	enum EDrawType
	{
		DrawFirst,			// draw after splash
		DrawForceAll,		// draw now
		DrawAll				// draw with timeout
	};

	enum ESyntaxType
	{
		GCodeBasic=0,
		GCode=1
	};

	virtual void Init();
	virtual void DrawRequest(EDrawType draw);

	void Invalidate();		// draw with next timeout

	////////////////////////////////////////////////////////////

	static void InvalidateLcd()
	{
#ifdef _USE_LCD
	
		if (CLcd::GetInstance())
			CLcd::GetInstance()->Invalidate();
#endif
	}

protected:

	virtual void Poll();
	virtual void TimerInterrupt();

	virtual void Command(char* cmd);

	virtual unsigned long Draw(EDrawType draw) = 0;				// return => timeout for next draw

	virtual unsigned long Splash() = 0;							// return time to display

	bool IsSplash() const 										{ return _splash; };

public:

	virtual void Beep(const SPlayTone*, bool fromProgmem)=0;
	void OKBeep()												{ Beep(SPlayTone::PlayOK,true); }
	void ErrorBeep()											{ Beep(SPlayTone::PlayError,true); }

	bool PostCommand(uint8_t syntaxtype, const __FlashStringHelper* cmd, Stream* output=NULL);
	bool PostCommand(char* cmd, Stream* output=NULL);

	virtual uint8_t InitPostCommand(uint8_t syntaxtype, char* cmd);

private:

	unsigned long _nextdrawtime=0;

	bool _splash=false;
	bool _invalidate=false;

public:

	void Diagnostic(const __FlashStringHelper * diag)			{ _diagnostics = diag; }

	bool IsDiagnostic()											{ return _diagnostics != NULL; };
	const __FlashStringHelper * GetDiagnostic()					{ return _diagnostics; }
	void ClearDiagnostic()										{ _diagnostics = NULL; }

private:

	const __FlashStringHelper * _diagnostics=NULL;


};

////////////////////////////////////////////////////////





