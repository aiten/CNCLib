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

template <pin_t PIN>
class CReadAnalogIOControl
{
public:

	void Init()		// init and set default value
	{
    CHAL::pinMode(PIN,INPUT);
	}

  int Read()
  {
    return constrain(
              map(CHAL::analogRead(PIN), _minRead, _maxRead, _min, _max), 
              _min, 
              _max);
  }

  void SetMinMax(int min, int max)
  {
    _min = min;
    _max = max;
  }

private:

  int _min = 0;
  int _max = 256;

  int _minRead = 0;         // minimum sensor value
  int _maxRead = 1023;      // maximum sensor value

};

////////////////////////////////////////////////////////
