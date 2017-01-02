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

class CStepperTest
{
public:

  struct SMove
  {
    sdist_t dist;
    steprate_t rate;
  };

private:

  struct SMove* _moves;
  steprate_t _maxspeed;
  udist_t _maxdist;

  steprate_t _movespeed;
  udist_t _movedist;

  sdist_t _count[NUM_AXIS];

public:

  CStepperTest(struct SMove* moves, steprate_t maxspeed, udist_t maxdist)
  {
    _moves = moves;
    _maxspeed = maxspeed;
    _maxdist = maxdist;
    
    _movespeed = 0;
    _movedist = 0;
    
    for(unsigned char i=0;i<NUM_AXIS;i++) _count[i] = 0;

    for(unsigned char i=0;_moves[i].rate;i++)
    {
      _movedist += _moves[i].dist;
      if (_moves[i].dist && _movespeed < _moves[i].rate)   _movespeed = _moves[i].rate;
    }
  }

  void TestAxis(axis_t axis)
  {
    for (register unsigned char i=0;_moves[i].rate != 0; i++)
    {
      if (_moves[i].dist)
      {
        sdist_t    dist = RoundMulDivI32(_moves[i].dist,_maxdist,_movedist);
        steprate_t rate = RoundMulDivU32(_moves[i].rate,_maxspeed,_movespeed);
        CStepper::GetInstance()->MoveRel(axis, dist, rate); _count[axis] -= dist;
      }
      else
      {
        CStepper::GetInstance()->Wait((unsigned int) _moves[i].rate);
      }
    }
  }

  void Home(steprate_t speed=0)
  {
    CStepper::GetInstance()->MoveRel(_count,speed ? speed : _maxspeed);

    for(unsigned char i=0;i<NUM_AXIS;i++) _count[i] = 0;
  }
};
