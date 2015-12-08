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

#include <Parser.h>

////////////////////////////////////////////////////////

class CHPGLParser : public CParser
{
private:

	typedef CParser super;

public:

	CHPGLParser(CStreamReader* reader,Stream* output) : CParser(reader,output)	{ };

	virtual void Parse() override;

	static mm1000_t HPGLToMM1000X(long xx);
	static mm1000_t HPGLToMM1000Y(long yy);

	static void Init()											{ super::Init(); _state.Init(); }

	struct SState
	{
		bool HPGLIsAbsolut;
		int HPOffsetX;
		int HPOffsetY;

		feedrate_t FeedRate;
		feedrate_t FeedRateUp;
		feedrate_t FeedRateDown;

		// Plotter

		unsigned int penUpTimeOut;

		void Init()
		{
			HPGLIsAbsolut = 1;

			FeedRateUp   = PENUP_FEEDRATE;
			FeedRateDown = PENDOWN_FEEDRATE;

			// => unit is 0,025mm(40units/mm), 6950*8(55600)steps are ca. 523,5 mm => 20940;

//			HPMul = 77;
//			HPDiv = 29;

			HPOffsetX = 0;
			HPOffsetY = 0;
/*
			penDown.max = 8000;
//			penDown.max = 4000;
			penDown.acc = 450;
			penDown.dec = 500;

			penUp.max = 25000;
			penUp.acc = 600;
			penUp.dec = 650;

			movePenUp.max = 4000;
			movePenUp.acc = 400;
			movePenUp.dec = 450;

			movePenDown.max = 4000;
			movePenDown.acc = 400;
			movePenDown.dec = 450;
*/
			penUpTimeOut = 1000;
		}
	};

	static SState _state;

private:

	void IgnoreCommand();
	void InitCommand();
	void PenMoveCommand(unsigned char cmdidx);
	
};

////////////////////////////////////////////////////////



