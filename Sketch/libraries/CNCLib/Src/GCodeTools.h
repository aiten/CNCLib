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

#include "ConfigurationCNCLib.h"

////////////////////////////////////////////////////////

typedef unsigned short toolnr_t;
#define NOTOOLINDEX 255

////////////////////////////////////////////////////////
// 
// Tools library
//
class CGCodeTools : public CSingleton<CGCodeTools>
{

public:

	enum EToolType
	{
		EndMill,
		BullNose,
		BallNose,
		Vcutter,
		Drill,
		Lathe
	};

	struct STools
	{
		toolnr_t ToolNr;
		EnumAsByte(EToolType) ToolType;
		mm1000_t Radius;
		mm1000_t Height;
	};

	bool IsValidTool(toolnr_t tool)		{ return GetToolIndex(tool) != NOTOOLINDEX; }

	mm1000_t GetRadius(toolnr_t tool);
	mm1000_t GetHeight(toolnr_t tool);

private:

	uint8_t GetToolIndex(toolnr_t tool);

	static const STools _tools[] PROGMEM;

};