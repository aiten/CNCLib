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

#include "GCodeParserBase.h"
#include "GCodeTools.h"

////////////////////////////////////////////////////////

typedef unsigned short param_t;

#define NUM_PARAMETER	8
#define NUM_MAXPARAMNAMELENGTH 16

// see: http://linuxcnc.org/docs/html/gcode/overview.html#_numbered_parameters_a_id_sub_numbered_parameters_a

#define PARAMSTART_G28HOME		5161		// 5161-5169 - G28 Home for (X Y Z A B C U V W)
#define PARAMSTART_G92OFFSET	5211		// 5211-5219 - G92 offset (X Y Z A B C U V W) 
#define PARAMSTART_G54OFFSET	5221		// 5221-5230 - Coordinate System 1, G54 (X Y Z A B C U V W R) - R denotes the XY rotation angle around the Z axis 
#define PARAMSTART_CURRENTPOS	5420		// 5420-5428 - Current Position including offsets in current program units (X Y Z A B C U V W)

// extent
#define PARAMSTART_CURRENTABSPOS	6010	// Current Absolut machine position in current program units (X Y Z A B C U V W)
#define PARAMSTART_BACKLASH			6031	// Backlash in current units(e.g. mm) (X Y Z A B C U V W)
#define PARAMSTART_BACKLASH_FEEDRATE 6049	// Feedrate for backlash (0 if disabled)


#define PARAMSTART_MAX				6051	// Acc (X Y Z A B C U V W)
#define PARAMSTART_MIN				6071	// Acc (X Y Z A B C U V W)
#define PARAMSTART_ACC				6091	// Acc (X Y Z A B C U V W)
#define PARAMSTART_DEC				6111	// Dec (X Y Z A B C U V W)
#define PARAMSTART_JERK				6131	// Jerk (X Y Z A B C U V W)

#define PARAMSTART_CONTROLLERFAN	6900	// Controllerfan if not idle (0 if disabled, 255 max)
#define PARAMSTART_RAPIDMOVEFEED	6901	// RapidMove Feedrate

// g73 retraction
#define G73RETRACTION			200			// mm1000_t => 0.2mm

////////////////////////////////////////////////////////

class CGCodeParser : public CGCodeParserBase
{
private:

	typedef CGCodeParserBase super;
	friend class CGCodeExpressionParser;

public:

	CGCodeParser(CStreamReader* reader,Stream* output) : super(reader,output)		{  };

	static mm1000_t GetG54PosPreset(axis_t axis);
	static mm1000_t GetToolHeightPosPreset(axis_t axis)		{ return axis == super::_modalstate.Plane_axis_2 ? _modalstate.ToolHeigtCompensation : 0; }
	static void SetG54PosPreset(axis_t axis, mm1000_t pos)	{ _modalstate.G54Pospreset[axis] = pos; }
	static unsigned char GetZeroPresetIdx()					{ return _modalstate.ZeroPresetIdx; }
	static void SetZeroPresetIdx(unsigned char idx)			{ _modalstate.ZeroPresetIdx = idx; }

	static bool IsG53Present()								{ return _modlessstate.ZeroPresetIdx == 0; }

	static mm1000_t GetAllPreset(axis_t axis)				{ return GetG92PosPreset(axis) + GetG54PosPreset(axis) + GetToolHeightPosPreset(axis); }

	static void Init()										{ super::Init(); _modalstate.Init(); _modlessstate.Init(); }

protected:

	// overrides to exend parser

	virtual bool InitParse() override;
	virtual void CleanupParse() override;

	virtual bool GCommand(unsigned char gcode) override;	// check for GCode extension => return true if command is parsed, false to do default
	virtual bool MCommand(unsigned char mcode) override;
	virtual bool SetParamCommand(param_t pramNo);
	virtual bool Command(unsigned char ch) override;

	void ToolSelectCommand();
	void SpindleSpeedCommand();

	virtual void CommentMessage(char*) override;
	virtual mm1000_t CalcAllPreset(axis_t axis) override;

protected:

	////////////////////////////////////////////////////////
	// Modal State

	struct SModalState
	{
		enum ECutterRadiusCompensation
		{
			CutterRadiusOff = 0,
			CutterRadiusLeft,
			CutterRadiusRight
		};

		EnumAsByte(ECutterRadiusCompensation) CutterRadiusCompensation;
		bool EvenSize;

		unsigned char	ZeroPresetIdx;				// 0:g53-, 1:G54-
		bool			IsG98;						// G98 or G99	( Return To R or return to init Z) 

		toolnr_t		ToolSelected;

		mm1000_t		G8xQ;
		mm1000_t		G8xPlane2;
		mm1000_t		G8xR;
		mm1000_t		G8xP;

		mm1000_t		G54Pospreset[NUM_AXIS];
		mm1000_t		ToolHeigtCompensation;

		unsigned long	Parameter[NUM_PARAMETER];

		void Init()	
		{
			*this = SModalState();		// POD .. Plane Old Daty Type => no Constructor => init with default value = 0
			ZeroPresetIdx = 1;						// always 54
//POD		G8xQ = G8xR = G8xPlane2 = G8xP = 0;
			CutterRadiusCompensation = CutterRadiusOff;
			ToolSelected = 1;
			IsG98 = true;
//POD		for (register unsigned char i = 0; i < NUM_AXIS; i++) G54Pospreset[i] = 0;
//POD		for (register unsigned char i = 0; i < NUM_PARAMETER; i++) Parameter[i] = 0;
//POD		ToolHeigtCompensation = 0;
		}
	};

	static SModalState _modalstate;

	////////////////////////////////////////////////////////
	// Modeless State

	struct SModlessState
	{
		unsigned char	ZeroPresetIdx;				// 0:g53-, 1:G54-
		void Init()
		{
//			*this = SModlessState();		// POD .. Plane Old Daty Type => no Constructor => init with default value = 0
			ZeroPresetIdx = _modalstate.ZeroPresetIdx;
		}
	};

	static SModlessState _modlessstate;

	////////////////////////////////////////////////////////
	// Parser structure

	bool CutterRadiosIsOn()								    { if (_modalstate.CutterRadiusCompensation) { Info(MESSAGE_GCODE_G41G43AreNotAllowedWithThisCommand); return true; } else return false; }

	virtual unsigned long ParseParameter() override;
	param_t ParseParamNo();

	mm1000_t GetParamValue(param_t paramNo);
	void SetParamValue(param_t parmNo);

	mm1000_t GetParamAsPosition(mm1000_t posInMachine, axis_t axis)	{ return ToInch(CMotionControlBase::GetInstance()->ToMm1000(axis, posInMachine)); }
	mm1000_t GetParamAsMachine(mm1000_t posInmm1000, axis_t axis)		{ return FromInch(CMotionControlBase::GetInstance()->ToMachine(axis, posInmm1000)); }

	mm1000_t GetRelativePosition(mm1000_t pos, axis_t axis)				{ return pos - GetG92PosPreset(axis) - GetG54PosPreset(axis); }
	mm1000_t GetRelativePosition(axis_t axis)							{ return GetRelativePosition(CMotionControlBase::GetInstance()->GetPosition(axis), axis); }

	unsigned char GetSubCode();

private:

	void GetR81(SAxisMove& move);
	void GetP81(SAxisMove& move);
	void GetQ81(SAxisMove& move);
	void GetL81(SAxisMove& move, unsigned char& l);

	void G10Command();
	void G40Command()							{ _modalstate.CutterRadiusCompensation = SModalState::CutterRadiusOff; }
	void G41Command();		// Cutter Radius Compensation left
	void G42Command();		// Cutter Radius Compensation right
	void G43Command();		// Tool Height Compensation 
	void G49Command()							{ _modalstate.ToolHeigtCompensation = 0; }
	void G53Command();
	void G68Command();
	void G69Command();
	void G5xCommand(unsigned char idx);
	void G8xCommand(SAxisMove& move, bool useP, bool useQ, bool useMinQ);
	void G73Command();		// High-speed Peck Drilling for Shallow Holes
	void G81Command();		// Basic drilling canned cycle
	void G82Command();		// Spot Drilling Cycle
	void G83Command();		// Peck Drilling for Deeper Holes
	void G98Command()							{ _modalstate.IsG98 = true; };
	void G99Command()							{ _modalstate.IsG98 = false; };

	void M00Command();		// Compulsory stop
	void M01Command();		// Optional stop
	void M02Command();		// End of program
	void M06Command();		// Automatic tool change => not supported
	void M07Command();		// coolant on
	void M08Command();		// Coolant on (flood)
	void M09Command();		// coolant off

	/////////////////

	void SetPositionAfterG68G69()				{ CMotionControlBase::GetInstance()->SetPositionFromMachine(); }
};

////////////////////////////////////////////////////////
