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

class CPlotter
{
public:

	CPlotter();

	void Init();
	void Initialized();

	void Idle(unsigned int idletime);
	void Resume();

	void PenDown();
	void PenUp();
	void PenUpNow();

	void DelayPenUp() { _isDelayPen = true; _isDelayPenDown = false; }
	void DelayPenDown() { _isDelayPen = true; _isDelayPenDown = true; }
	void DelayPenNow();    // go

	void StopPen()
	{
		_isPenDown = 0;
		_isDelayPen = false;
	};

	bool IsPenDown() { return _isPenDown; }
	uint8_t GetPen() { return _pen; }
	bool SetPen(uint8_t pen);


private:

	bool _isDelayPen;
	bool _isDelayPenDown;
	bool _isPenDown;
	bool _isPenDownTimeout;

	uint8_t _pen;
	bool _havePen;

	bool MoveToPenPosition(feedrate_t feedrate, mm1000_t pos);

	bool PenToDepot();
	bool PenFromDepot(uint8_t pen);
	bool ToPenChangePos(uint8_t pen);
	bool OffPenChangePos(uint8_t pen);

	mm1000_t ConvertConfigPos(mm1000_t pos, axis_t axis);
};

////////////////////////////////////////////////////////

extern CPlotter Plotter;










