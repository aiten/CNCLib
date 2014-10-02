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

#include <Parser.h>

////////////////////////////////////////////////////////

class CHPGLParser : public CParser
{
private:

	typedef CParser super;

public:

	CHPGLParser(CStreamReader* reader,Stream* output) : CParser(reader,output)	{ };

	virtual void Parse();

	sdist_t HPGLToPlotterCordX(sdist_t xx);
	sdist_t HPGLToPlotterCordY(sdist_t yy);

	static void Init()											{ super::Init(); _state.Init(); }

	struct SState
	{
		bool HPGLIsAbsolut;
		long HPMul;
		long HPDiv;
		int HPOffsetX;
		int HPOffsetY;

		// Plotter

		unsigned int penUpPos;
		unsigned int penDownPos;
		unsigned int penUpTimeOut;

		struct SSPEED
		{
			unsigned int  max;
			unsigned int  acc;
			unsigned int  dec;
		};

		struct SSPEED penDown;
		struct SSPEED penUp;

		struct SSPEED movePenUp;
		struct SSPEED movePenDown;

		void Init()
		{
#define X_AXIS_ENDSTOP 0
#define Y_AXIS_ENDSTOP 2
#define Z_AXIS_ENDSTOP 4
#define EMERGENCY_ENDSTOP 5

			HPGLIsAbsolut = 1;

			// => unit is 0,025mm(40units/mm), 6950*8(55600)steps are ca. 523,5 mm => 20940;

			HPMul = 77;
			HPDiv = 29;

			HPOffsetX = 0;
			HPOffsetY = 0;

			penDown.max = 8000;
			penDown.acc = 500;
			penDown.dec = 500;

			penUp.max = 30000;
			penUp.acc = 1000;
			penUp.dec = 1000;

			movePenUp.max = 4000;
			movePenUp.acc = 400;
			movePenUp.dec = 450;

			movePenDown.max = 4000;
			movePenDown.acc = 400;
			movePenDown.dec = 450;

			penUpPos = 00;
			penDownPos = 30 * 8;

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



