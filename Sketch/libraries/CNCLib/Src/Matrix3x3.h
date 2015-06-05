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
class CMatrix3x3
{
private:
	
	T _v[3][3];

public:

	static void Invert(const T src[3][3], T dest[3][3])
	{
		T determinant=	src[0][0]*(src[1][1]*src[2][2]-src[2][1]*src[1][2])-
						src[0][1]*(src[1][0]*src[2][2]-src[1][2]*src[2][0])+
						src[0][2]*(src[1][0]*src[2][1]-src[1][1]*src[2][0]);
		dest[0][0]= (src[1][1]*src[2][2]-src[2][1]*src[1][2])/determinant;
		dest[0][1]=-(src[1][0]*src[2][2]-src[1][2]*src[2][0])/determinant;
		dest[0][2]= (src[1][0]*src[2][1]-src[2][0]*src[1][1])/determinant;
		dest[1][0]=-(src[0][1]*src[2][2]-src[0][2]*src[2][1])/determinant;
		dest[1][1]= (src[0][0]*src[2][2]-src[0][2]*src[2][0])/determinant;
		dest[1][2]=-(src[0][0]*src[2][1]-src[2][0]*src[0][1])/determinant;
		dest[2][0]= (src[0][1]*src[1][2]-src[0][2]*src[1][1])/determinant;
		dest[2][1]=-(src[0][0]*src[1][2]-src[1][0]*src[0][2])/determinant;
		dest[2][2]= (src[0][0]*src[1][1]-src[1][0]*src[0][1])/determinant;
/*
		T determinant=0;
		unsigned char i,j;
 
		for(i=0;i<3;i++)
			determinant = determinant + (src[0][i]*(src[1][(i+1)%3]*src[2][(i+2)%3] - src[1][(i+2)%3]*src[2][(i+1)%3]));
 
		for(i=0;i<3;i++)
		{
			for(j=0;j<3;j++)
			{
				dest [i][j] = ((src[(i+1)%3][(j+1)%3] * src[(i+2)%3][(j+2)%3]) - (src[(i+1)%3][(j+2)%3]*src[(i+2)%3][(j+1)%3]))/ determinant;
			}
		}
*/
	}
	void Invert(T dest[3][3]) const
	{
		Invert(_v,dest);
	}

	static void Mul(const T src[3][3], const T srcV[3], T dest[3])
	{
		dest[0] = src[0][0] * srcV[0] + src[0][1] * srcV[1] + src[0][2] * srcV[2];
		dest[1] = src[1][0] * srcV[0] + src[1][1] * srcV[1] + src[1][2] * srcV[2];
		dest[2] = src[2][0] * srcV[0] + src[2][1] * srcV[1] + src[2][2] * srcV[2];
	}
};

