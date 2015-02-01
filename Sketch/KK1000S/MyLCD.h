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

////////////////////////////////////////////////////////

#include "Configuration_KK1000S.h"

////////////////////////////////////////////////////////

#include <U8GLCD.h>
#include "MyMenu.h"

////////////////////////////////////////////////////////

class CMyLcd : public CU8GLcd
{
private:

	typedef CU8GLcd super;

public:

	virtual void Init();
	virtual void Beep(const SPlayTone*);

protected:

	virtual class U8GLIB& GetU8G();
	virtual class CMenuBase& GetMenu()	{ return _menu; }

	virtual bool DrawLoopDefault(EnumAsByte(EDrawLoopType) type,void *data);

private:

	CMyMenu _menu;

};

////////////////////////////////////////////////////////

extern CMyLcd Lcd;
