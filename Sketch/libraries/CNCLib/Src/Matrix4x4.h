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

//////////////////////////////////////////

template <class T>
class CMatrix4x4
{
private:
	
	T _v[4][4];

public:

	static void Mul(const T src[4][4], const T srcV[4], T dest[4])
	{
		dest[0] = src[0][0] * srcV[0] + src[0][1] * srcV[1] + src[0][2] * srcV[2] + src[0][3] * srcV[3];
		dest[1] = src[1][0] * srcV[0] + src[1][1] * srcV[1] + src[1][2] * srcV[2] + src[1][3] * srcV[3];
		dest[2] = src[2][0] * srcV[0] + src[2][1] * srcV[1] + src[2][2] * srcV[2] + src[2][3] * srcV[3];
		dest[3] = src[3][0] * srcV[0] + src[3][1] * srcV[1] + src[3][2] * srcV[2] + src[3][3] * srcV[3];
	}

	static void InitDenavitHartenberg(T dest[4][4], float alpha, float theta, float a, float d)
	{
		float costheta = cos(theta);
		float sintheta = sin(theta);
		float cosalpha = cos(alpha);
		float sinalpha = sin(alpha);
		
		dest[0][0] = costheta;	dest[0][1] = -sintheta*cosalpha; dest[0][2] = sintheta*sinalpha;   dest[0][3] = a*costheta;
		dest[1][0] = sintheta;	dest[1][1] = costheta*cosalpha;  dest[1][2] = -costheta*sinalpha;  dest[1][3] = a*sintheta;
		dest[2][0] = 0;			dest[2][1] = sinalpha;			 dest[2][2] = cosalpha;			   dest[2][3] = d;
		dest[3][0] = 0;			dest[3][1] = 0;					 dest[3][2] = 0;				   dest[3][3] = 1.0;
	}

	static void InitDenavitHartenbergRot(T dest[4][4], float theta)
	{
		float costheta = cos(theta);
		float sintheta = sin(theta);

		dest[0][0] = costheta;	dest[0][1] = -sintheta;			dest[0][2] = 0;		dest[0][3] = 0;
		dest[1][0] = sintheta;	dest[1][1] = costheta;			dest[1][2] = 0;		dest[1][3] = 0;
		dest[2][0] = 0;			dest[2][1] = sinalpha;			dest[2][2] = 1;		dest[2][3] = 0;
		dest[3][0] = 0;			dest[3][1] = 0;					dest[3][2] = 0;		dest[3][3] = 1;
	}

};

