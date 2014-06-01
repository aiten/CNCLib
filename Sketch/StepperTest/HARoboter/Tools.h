#pragma once

////////////////////////////////////////////////////////

class TimeDiff
{
  unsigned long _time;
  unsigned long _steps;

public:

  TimeDiff();
  unsigned long Diff()
  {
    return millis()-_time;
  }
  void Print();
};

////////////////////////////////////////////////////////

class Tools
{
public:
	static void ToPoint(int x, int y, int * dx, int * dy, float rad);
	static long Dist(unsigned  x1, unsigned y1, unsigned x2, unsigned y2);
};

////////////////////////////////////////////////////////

