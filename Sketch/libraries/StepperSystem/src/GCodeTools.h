#pragma once

////////////////////////////////////////////////////////

#include "Configuration.h"
#include "Utilities.h"
#include "MotionControl.h"

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

	unsigned char GetToolIndex(toolnr_t tool);

	static const STools _tools[] PROGMEM;

};