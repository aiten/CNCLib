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