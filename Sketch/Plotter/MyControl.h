////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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

#include <Control.h>

class CMyControl : public CControl
{
private:

	typedef CControl super;

public:

	void MyInit() { return Init(); }

protected:

	virtual void Init();
	virtual void Initialized();
	virtual bool Parse(CStreamReader* reader, Stream* output);
	virtual void Idle(unsigned int idletime);

	virtual void GoToReference();
	virtual void GoToReference(axis_t axis);

};

////////////////////////////////////////////////////////

extern CMyControl Control;
