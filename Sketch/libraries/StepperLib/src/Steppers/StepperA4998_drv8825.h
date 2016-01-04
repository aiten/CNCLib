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

#if defined(USE_A4998)

static void Delay1() ALWAYSINLINE				{ } 
static void Delay2() ALWAYSINLINE				{ } 

#elif defined(__SAM3X8E__)

static void Delay1() ALWAYSINLINE				{ CHAL::delayMicroseconds(1); } 
static void Delay2() ALWAYSINLINE				{ CHAL::delayMicroseconds(1); } 

#else //AVR

static void Delay1() ALWAYSINLINE				{ CHAL::delayMicroseconds0312(); }
static void Delay2() ALWAYSINLINE				{ CHAL::delayMicroseconds0500(); }

#endif
