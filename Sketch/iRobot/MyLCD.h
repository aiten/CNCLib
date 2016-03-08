////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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

#include "Configuration_iRobot.h"

////////////////////////////////////////////////////////

#include <U8GLCD.h>
#include "MyMenu.h"

////////////////////////////////////////////////////////

class CMyLcd : public CU8GLcd
{
private:

	typedef CU8GLcd super;

public:

	virtual void Init() override;
	virtual void Beep(const SPlayTone*,bool) override;

protected:

	virtual class U8GLIB& GetU8G() override;
	virtual class CMenuBase& GetMenu() override	{ return _menu; }

	virtual bool DrawLoopDefault(EnumAsByte(EDrawLoopType) type, void *data) override;

private:

	CMyMenu _menu;

};

////////////////////////////////////////////////////////

extern CMyLcd Lcd;
