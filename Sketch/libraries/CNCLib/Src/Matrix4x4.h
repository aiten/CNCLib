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

#define MATRIX4X4SIZEX	4
#define MATRIX4X4SIZEY	4


template <class T>
class CMatrix4x4
{
private:
	
	T _v[MATRIX4X4SIZEX][MATRIX4X4SIZEY];

public:

	CMatrix4x4<T>(const CMatrix4x4<T>& src)
	{
		memcpy(_v, src._v, sizeof(_v));
	}
	CMatrix4x4<T>()
	{
	}
	CMatrix4x4<T>(const T dest[MATRIX4X4SIZEX][MATRIX4X4SIZEY])
	{
		memcpy(_v, dest, sizeof(_v));
	}

	static void Zero(T dest[MATRIX4X4SIZEX][MATRIX4X4SIZEY])
	{
		memset(dest, 0, sizeof(T)*MATRIX4X4SIZEX*MATRIX4X4SIZEY);
	}
	void Zero()
	{
		Zero(_v);
	}

	static bool Compare(const T src1[MATRIX4X4SIZEX][MATRIX4X4SIZEY], const T src2[MATRIX4X4SIZEX][MATRIX4X4SIZEY])
	{
		for (unsigned char i = 0; i < MATRIX4X4SIZEX; i++)
			for (unsigned char k = 0; k < MATRIX4X4SIZEY; k++)
				if (src1[i][k] != src2[i][k])
					return false;
		return true;
	}

	bool operator==(const CMatrix4x4<T>& cmp) const
	{
		return Compare(this->_v, cmp._v);
	}

	bool operator!=(const CMatrix4x4<T>& cmp) const
	{
		return !Compare(this->_v, cmp._v);
	}

	static bool IsEqual(const T f1, const T f2, const T epsilon)
	{
		return fabs(f1 - f2) <= epsilon;
	}

	static bool Compare(const T src1[MATRIX4X4SIZEX][MATRIX4X4SIZEY], const T src2[MATRIX4X4SIZEX][MATRIX4X4SIZEY], const T epsilon)
	{
		for (unsigned char i = 0; i < MATRIX4X4SIZEX; i++)
			for (unsigned char k = 0; k < MATRIX4X4SIZEY; k++)
				if (!IsEqual(src1[i][k], src2[i][k], epsilon))
					return false;
		return true;
	}

	bool IsEqual(const CMatrix4x4<T>& cmp, T epsilon) const
	{
		return Compare(this->_v, cmp._v, epsilon);
	}

	static void Mul(const T src[MATRIX4X4SIZEX][MATRIX4X4SIZEY], const T srcV[MATRIX4X4SIZEX], T dest[MATRIX4X4SIZEX])
	{
		dest[0] = src[0][0] * srcV[0] + src[0][1] * srcV[1] + src[0][2] * srcV[2] + src[0][3] * srcV[3];
		dest[1] = src[1][0] * srcV[0] + src[1][1] * srcV[1] + src[1][2] * srcV[2] + src[1][3] * srcV[3];
		dest[2] = src[2][0] * srcV[0] + src[2][1] * srcV[1] + src[2][2] * srcV[2] + src[2][3] * srcV[3];
		dest[3] = src[3][0] * srcV[0] + src[3][1] * srcV[1] + src[3][2] * srcV[2] + src[3][3] * srcV[3];
	}

	void Mul(const T srcV[MATRIX4X4SIZEX], T destV[MATRIX4X4SIZEX]) const
	{
		Mul(_v, srcV, destV);
	}

	static void Mul(const T src1[MATRIX4X4SIZEX][MATRIX4X4SIZEY], const T src2[MATRIX4X4SIZEX][MATRIX4X4SIZEY], T dest[MATRIX4X4SIZEX][MATRIX4X4SIZEY])
	{
		Zero(dest);
		for (unsigned char i = 0; i < MATRIX4X4SIZEX; i++)
			for (unsigned char k = 0; k < MATRIX4X4SIZEY; k++)
				for (unsigned char j = 0; j < MATRIX4X4SIZEX; j++)		// col/rows of src1/src2
				{
					dest[i][k] = dest[i][k] + src1[i][j] * src2[j][k];
				}
	}

	CMatrix4x4<T>& operator*=(const CMatrix4x4<T>& rhs)
	{
		CMatrix4x4<T> src2(*this);
		Mul(rhs._v, src2._v, _v);
		return *this;
	}

	T Get(unsigned char x, unsigned char y) const
	{
		return _v[x][y];
	}

	void Set(unsigned char x, unsigned char y, T value)
	{
		_v[x][y] = value;
	}

	friend CMatrix4x4<T> operator*(const CMatrix4x4<T>& lhs, const CMatrix4x4<T>& rhs)
	{
		CMatrix4x4<T> dest;
		Mul(lhs._v, rhs._v, dest._v);
		return dest;
	}

	static void InitDenavitHartenbergNOP(T dest[4][4])
	{
		Zero(dest);

		dest[0][0] = 1.0;
		dest[1][1] = 1.0;
		dest[2][2] = 1.0;
		dest[3][3] = 1.0;
	}

	CMatrix4x4<T>& InitDenavitHartenbergNOP()
	{
		InitDenavitHartenbergNOP(_v);
		return *this;
	}

	// rot1*trans2*trans3*rot4

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

	CMatrix4x4<T>& InitDenavitHartenberg(float alpha, float theta, float a, float d)
	{
		InitDenavitHartenberg(_v, alpha, theta, a, d);
		return *this;
	}

	static void InitDenavitHartenbergInverse(T dest[4][4], float alpha, float theta, float a, float d)
	{
		float costheta = cos(theta);
		float sintheta = sin(theta);
		float cosalpha = cos(alpha);
		float sinalpha = sin(alpha);

		dest[0][0] = costheta;				dest[0][1] = sintheta;			 dest[0][2] = 0;		dest[0][3] = -a;
		dest[1][0] = -sintheta*cosalpha;	dest[1][1] = costheta*cosalpha;  dest[1][2] = sinalpha; dest[1][3] = -d*sinalpha;
		dest[2][0] = sintheta*sinalpha;		dest[2][1] = -costheta*sinalpha; dest[2][2] = cosalpha;	dest[2][3] = -d*cosalpha;
		dest[3][0] = 0;						dest[3][1] = 0;					 dest[3][2] = 0;		dest[3][3] = 1.0;
	}

	CMatrix4x4<T>& InitDenavitHartenbergInverse(float alpha, float theta, float a, float d)
	{
		InitDenavitHartenbergInverse(_v, alpha, theta, a, d);
		return *this;
	}

	// einer Rotation \theta_n(Gelenkwinkel) um die z_{ n - 1 }-Achse, damit die x_{ n - 1 }-Achse parallel zu der x_n - Achse liegt
	
	static void InitDenavitHartenberg1Rot(T dest[4][4], float theta)
	{
		float costheta = cos(theta);
		float sintheta = sin(theta);

		dest[0][0] = costheta;	dest[0][1] = -sintheta;			dest[0][2] = 0;		dest[0][3] = 0;
		dest[1][0] = sintheta;	dest[1][1] = costheta;			dest[1][2] = 0;		dest[1][3] = 0;
		dest[2][0] = 0;			dest[2][1] = 0;					dest[2][2] = 1;		dest[2][3] = 0;
		dest[3][0] = 0;			dest[3][1] = 0;					dest[3][2] = 0;		dest[3][3] = 1;
	}

	CMatrix4x4<T>& InitDenavitHartenberg1Rot(float theta)
	{
		InitDenavitHartenberg1Rot(_v, theta);
		return *this;
	}

	// einer Translation d_n(Gelenkabstand) entlang der z_{ n - 1 }-Achse bis zu dem Punkt, wo sich z_{ n - 1 } und x_n schneiden

	static void InitDenavitHartenberg2Trans(T dest[4][4], float d)
	{
		dest[0][0] = 1;			dest[0][1] = 0;				dest[0][2] = 0;		dest[0][3] = 0;
		dest[1][0] = 0;			dest[1][1] = 1;				dest[1][2] = 0;		dest[1][3] = 0;
		dest[2][0] = 0;			dest[2][1] = 0;				dest[2][2] = 1;		dest[2][3] = d;
		dest[3][0] = 0;			dest[3][1] = 0;				dest[3][2] = 0;		dest[3][3] = 1;
	}

	CMatrix4x4<T>& InitDenavitHartenberg2Trans(float d)
	{
		InitDenavitHartenberg2Trans(_v, d);
		return *this;
	}

	// einer Translation a_n(Armelementlänge) entlang der x_n - Achse, um die Ursprünge der Koordinatensysteme in Deckung zu bringen

	static void InitDenavitHartenberg3Trans(T dest[4][4], float a)
	{
		dest[0][0] = 1;			dest[0][1] = 0;				dest[0][2] = 0;		dest[0][3] = a;
		dest[1][0] = 0;			dest[1][1] = 1;				dest[1][2] = 0;		dest[1][3] = 0;
		dest[2][0] = 0;			dest[2][1] = 0;				dest[2][2] = 1;		dest[2][3] = 0;
		dest[3][0] = 0;			dest[3][1] = 0;				dest[3][2] = 0;		dest[3][3] = 1;
	}

	CMatrix4x4<T>& InitDenavitHartenberg3Trans(float a)
	{
		InitDenavitHartenberg3Trans(_v, a);
		return *this;
	}

	// einer Rotation \alpha_n(Verwindung) um die x_n - Achse, um die z_{ n - 1 }-Achse in die z_n - Achse zu überführen

	static void InitDenavitHartenberg4Rot(T dest[4][4], float alpha)
	{
		float cosalpha = cos(alpha);
		float sinalpha = sin(alpha);

		dest[0][0] = 1;			dest[0][1] = 0;				dest[0][2] = 0;				dest[0][3] = 0;
		dest[1][0] = 0;			dest[1][1] = cosalpha;		dest[1][2] = -sinalpha;		dest[1][3] = 0;
		dest[2][0] = 0;			dest[2][1] = sinalpha;		dest[2][2] = cosalpha;		dest[2][3] = 0;
		dest[3][0] = 0;			dest[3][1] = 0;				dest[3][2] = 0;				dest[3][3] = 1;
	}

	CMatrix4x4<T>& InitDenavitHartenberg4Rot(float a)
	{
		InitDenavitHartenberg4Rot(_v, a);
		return *this;
	}
};

