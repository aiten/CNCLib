////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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

class CPlotter
{
public:

	CPlotter();

	void Idle(unsigned int idletime);
	void Resume();

	void PenUp();
	void PenDown();

	void DelayPenUp()  { _isDelayPen = true; _isDelayPenDown = false; }
	void DelayPenDown(){ _isDelayPen = true; _isDelayPenDown = true; }
	void DelayPenNow();    // go

	void StopPen()
	{
		_isPenDown = 0;
		_isDelayPen = false;
	};

	bool IsPenDown()				{ return _isPenDown; }
	unsigned char GetPen()			{ return _pen; }
	void SetPen(unsigned char pen)	{ _pen=pen; }

protected:

	bool _isDelayPen;
	bool _isDelayPenDown;
	bool _isPenDown;
	bool _isPenDownTimeout;

	unsigned char _pen;
};

////////////////////////////////////////////////////////

extern CPlotter Plotter;