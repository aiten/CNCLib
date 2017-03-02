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

#include <Parser.h>

////////////////////////////////////////////////////////

class CHPGLParser : public CParser
{
private:

	typedef CParser super;

public:

	CHPGLParser(CStreamReader* reader, Stream* output) : CParser(reader, output) { };

	virtual void Parse() override;

	static mm1000_t HPGLToMM1000X(long xx);
	static mm1000_t HPGLToMM1000Y(long yy);

	static void Init() { super::Init(); _state.Init(); }

	struct SState
	{
		bool _HPGLIsAbsolut;
		bool dummy;

		int _HPOffsetX;
		int _HPOffsetY;

		feedrate_t FeedRate;
		feedrate_t FeedRateUp;
		feedrate_t FeedRateDown;

		// Plotter

		void Init()
		{
			_HPGLIsAbsolut = true;

			FeedRateUp   = -((feedrate_t)CConfigEeprom::GetConfigU32(offsetof(CMyControl::SMyCNCEeprom, penupFeedrate)));		// always negativ
			FeedRateDown = CConfigEeprom::GetConfigU32(offsetof(CMyControl::SMyCNCEeprom, pendownFeedrate));

			_HPOffsetX = 0;
			_HPOffsetY = 0;
		}
	};

	static SState _state;

private:

	void ReadAndSkipSemicolon();

	void SelectPenCommand();

	void IgnoreCommand();
	void InitCommand();
	void PenMoveCommand(uint8_t cmdidx);

};

////////////////////////////////////////////////////////












