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

#include "Parser.h"
#include "GCodeTools.h"

////////////////////////////////////////////////////////

typedef unsigned short param_t;

#define NUM_PARAMETER	8
#define NUM_MAXPARAMNAMELENGTH 16

// see: http://linuxcnc.org/docs/html/gcode/overview.html#_numbered_parameters_a_id_sub_numbered_parameters_a

#define PARAMSTART_G28HOME 5161			// 5161-5169 - G28 Home for (X Y Z A B C U V W)
#define PARAMSTART_G92OFFSET 5211		// 5211-5219 - G92 offset (X Y Z A B C U V W) 
#define PARAMSTART_G54OFFSET 5221		// 5221-5230 - Coordinate System 1, G54 (X Y Z A B C U V W R) - R denotes the XY rotation angle around the Z axis 
#define PARAMSTART_CURRENTPOS 5420		// 5420-5428 - Current Position including offsets in current program units (X Y Z A B C U V W)

// extent
#define PARAMSTART_CURRENTABSPOS 5430	// Current Absolut maschine position in current program units (X Y Z A B C U V W)
#define PARAMSTART_BACKLASH 5450			// Backlash in current units(e.g. mm) (X Y Z A B C U V W)
#define PARAMSTART_BACKLASH_FEEDRATE 5469	// Feedrate for backlash (0 if disabled)

#define PARAMSTART_CONTROLLERFAN 5470		// Controllerfan if not idle (0 if disabled, 255 max)


// g73 retraction
#define G73RETRACTION	200				// mm1000_t => 0.2mm

#define FEEDRATE_MIN_ALLOWED	VMAXTOFEEDRATE(100)
//#define FEEDRATE_MAX_ALLOWED	VMAXTOFEEDRATE(40000)
//#define FEEDRATE_MAX_G0			VMAXTOFEEDRATE(35000)
#define FEEDRATE_MAX_ALLOWED	VMAXTOFEEDRATE(26667)

#define FEEDRATE_MAX_G0			VMAXTOFEEDRATE(((SPEED_MULTIPLIER_4)-5))
//#define FEEDRATE_MAX_G0			VMAXTOFEEDRATE(30000)

#define FEEDRATE_DEFAULT_G0	-FEEDRATE_MAX_G0
#define FEEDRATE_DEFAULT_G1	VMAXTOFEEDRATE(5000)

#define FEEDRATE_REFMOVE	15000                  // in Steps/sec

////////////////////////////////////////////////////////

class CGCodeParser : public CParser
{
private:

	typedef CParser super;
	friend class CGCodeExpressionParser;

public:

	CGCodeParser(CStreamReader* reader,Stream* output) : super(reader,output)		{  };

	static void SetG0FeedRate(feedrate_t feedrate)			{ _modalstate.G0FeedRate = feedrate; }
	static void SetG1FeedRate(feedrate_t feedrate)			{ _modalstate.G1FeedRate = feedrate; }
	static mm1000_t GetG54PosPreset(axis_t axis);
	static mm1000_t GetG92PosPreset(axis_t axis)			{ return IsG53Present() ? 0 : _modalstate.G92Pospreset[axis]; }
	static mm1000_t GetToolHeightPosPreset(axis_t axis)		{ return IsG53Present() ? 0 : (axis == _modalstate.Plane_axis_2 ? _modalstate.ToolHeigtCompensation : 0); }
	static void SetG54PosPreset(axis_t axis, mm1000_t pos)	{ _modalstate.G54Pospreset[axis] = pos; }
	static unsigned char GetZeroPresetIdx()					{ return _modalstate.ZeroPresetIdx; }
	static void SetZeroPresetIdx(unsigned char idx)			{ _modalstate.ZeroPresetIdx = idx; }

	static bool IsG53Present()								{ return _modlessstate.ZeroPresetIdx == 0; }

	static mm1000_t GetAllPreset(axis_t axis)				{ return GetG92PosPreset(axis) + GetG54PosPreset(axis) + GetToolHeightPosPreset(axis); }

	static void Init()										{ super::Init(); _modalstate.Init(); _modlessstate.Init(); }

protected:

	// overrides to exend parser

	virtual void Parse();
	virtual bool InitParse();
	virtual void CleanupParse();

	virtual bool GCommand(unsigned char gcode);		// check for GCode extension => return true if command is parsed, false to do default
	virtual bool MCommand(unsigned char mcode);
	virtual bool SetParamCommand(param_t pramNo);
	virtual bool Command(unsigned char ch);

	virtual bool ParseLineNumber(bool setlinenumber);	// line number is ignored! => ret is error
	virtual char SkipSpacesOrComment();

	void ToolSelectCommand();
	void SpindleSpeedCommand();

protected:

	////////////////////////////////////////////////////////
	// Modal State

	struct SModalState
	{
		long			Linenumber;

		enum ELastCommand
		{
			LastG00 = 0,
			LastG01,
			LastG02,
			LastG03,
			LastG73,
			LastG81,
			LastG82,
			LastG83,
		};

		enum ECutterRadiusCompensation
		{
			CutterRadiusOff = 0,
			CutterRadiusLeft,
			CutterRadiusRight
		};

		EnumAsByte(ELastCommand) LastCommand;
		bool			IsAbsolut;

		unsigned char	Plane_axis_0;				// x
		unsigned char	Plane_axis_1;				// y 
		unsigned char	Plane_axis_2;				// z

		EnumAsByte(ECutterRadiusCompensation) CutterRadiusCompensation;

		unsigned char	ZeroPresetIdx;				// 0:g53-, 1:G54-
		bool			IsG98;						// G98 or G99	( Return To R or return to init Z) 

		bool			UnitisMm;				// g20,g21
		bool			FeedRatePerUnit;		//feedrate per Unit(mm,inch) per min, or per revolution /g94/95

		bool			ConstantVelocity;		// G61 G64
		bool			DummyEvenSize;

		feedrate_t		G0FeedRate;
		feedrate_t		G1FeedRate;

		short			SpindleSpeed;			// > 0 CW, < 0 CCW
		toolnr_t		ToolSelected;

		mm1000_t		G8xQ;
		mm1000_t		G8xPlane2;
		mm1000_t		G8xR;
		mm1000_t		G8xP;

		mm1000_t		G54Pospreset[NUM_AXIS];
		mm1000_t		G92Pospreset[NUM_AXIS];

		mm1000_t		ToolHeigtCompensation;

		unsigned long	Parameter[NUM_PARAMETER];

		void Init()	
		{
			ZeroPresetIdx = 1;						// always 54
			Linenumber = 0;
			G8xQ = G8xR = G8xPlane2 = G8xP = 0;
			LastCommand = LastG00;
			CutterRadiusCompensation = CutterRadiusOff;
			SpindleSpeed = 1000;
			ToolSelected = 1;
			IsG98 = true;
			UnitisMm = true;
			FeedRatePerUnit = true;
			ConstantVelocity = true;
			G0FeedRate = FEEDRATE_DEFAULT_G0;
			G1FeedRate = FEEDRATE_DEFAULT_G1;
			IsAbsolut = true;
			Plane_axis_0 = X_AXIS;
			Plane_axis_1 = Y_AXIS;
			Plane_axis_2 = Z_AXIS;
			for (register unsigned char i = 0; i < NUM_AXIS; i++) G92Pospreset[i] = 0;
			for (register unsigned char i = 0; i < NUM_AXIS; i++) G54Pospreset[i] = 0;
			for (register unsigned char i = 0; i < NUM_PARAMETER; i++) Parameter[i] = 0;
			ToolHeigtCompensation = 0;
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
			ZeroPresetIdx = _modalstate.ZeroPresetIdx;
		}
	};

	static SModlessState _modlessstate;

	////////////////////////////////////////////////////////
	// Parser structure

	struct SAxisMove
	{
		unsigned char axes;		// plural, each bit for axis
		union {
			struct
			{
				unsigned char I : 1;			// must be bit 0	=> see getIJK();
				unsigned char J : 1;			// must be bit 1
				unsigned char K : 1;			// must be bit 2
				unsigned char F : 1;
				unsigned char R : 1;
				unsigned char Q : 1;
				unsigned char P : 1;
				unsigned char L : 1;
			} bit;
			unsigned char all;
		} bitfield;

		mm1000_t newpos[NUM_AXIS];

		unsigned char GetIJK() { return bitfield.all & 7; }

		SAxisMove(bool getcurrentPosition)
		{
			axes = 0; bitfield.all = 0;
			if (getcurrentPosition)
				CMotionControl::GetPositions(newpos);
			else
			{
				for (register unsigned char i = 0; i < NUM_AXIS; i++) newpos[i] = 0;
			}
		}
	};

	////////////////////////////////////////////////////////

	bool CutterRadiosIsOn()								    { if (_modalstate.CutterRadiusCompensation) { Info(MESSAGE_GCODE_G41G43AreNotAllowedWithThisCommand); return true; } else return false; }

	void Delay(unsigned long ms);
	void SkipCommentNested();

	unsigned long ParseParameter();
	mm1000_t ParseCoordinate();
	param_t ParseParamNo();

	mm1000_t GetParamValue(param_t paramNo);
	void SetParamValue(param_t parmNo);

	unsigned long GetUint32OrParam(unsigned long max);
	unsigned long GetUint32OrParam()						{ return GetUint32OrParam(0xffffffffl); };
	unsigned short GetUint16OrParam()						{ return (unsigned short)GetUint32OrParam(65535); };
	unsigned char GetUint8OrParam()							{ return (unsigned char)GetUint32OrParam(255); };

	mm1000_t GetRelativePosition(mm1000_t pos, axis_t axis)	{ return pos - GetG92PosPreset(axis) - GetG54PosPreset(axis); }
	mm1000_t GetRelativePosition(axis_t axis)				{ return GetRelativePosition(CMotionControl::GetPosition(axis), axis); }
	mm1000_t ToInch(mm1000_t mm100);

	bool CheckAxisSpecified(axis_t axis, unsigned char& axes);
	axis_t CharToAxis(char axis);
	axis_t CharToAxisOffset(char axis);

	unsigned char GetSubCode();


	enum EAxisPosType
	{
		AbsolutWithZeroShiftPosition,
		AbsolutPosition,
		RelativPosition
	};

	mm1000_t ParseCoordinate(axis_t axis, mm1000_t relpos, EnumAsByte(EAxisPosType) posType);

	void GetUint8(unsigned char& value, unsigned char&specified, unsigned char bit);

private:

	void GetFeedrate(SAxisMove& move);
	void GetAxis(axis_t axis, SAxisMove& move, EnumAsByte(EAxisPosType) posType);
	void GetIJK(axis_t axis, SAxisMove& move, mm1000_t offset[2]);
	void GetRadius(SAxisMove& move, mm1000_t& radius);
	void GetR81(SAxisMove& move);
	void GetP81(SAxisMove& move);
	void GetQ81(SAxisMove& move);
	void GetL81(SAxisMove& move, unsigned char& l);

	void GetG92Axis(axis_t axis, unsigned char& count);

	static bool G31TestProbe(void*);

	bool LastCommand();

	void InfoNotImplemented()					{ Info(MESSAGE_GCODE_NotImplemented); }

	void G0001Command(bool isG00);
	void G0203Command(bool isG02);
	void G04Command();
	void G10Command();
	void G171819Command(axis_t axis0, axis_t axis1, axis_t axis2);
	void G20Command()							{ _modalstate.UnitisMm = false; };
	void G21Command()							{ _modalstate.UnitisMm = true; };
	void G28Command();
	void G31Command();
	void G40Command()							{ _modalstate.CutterRadiusCompensation = SModalState::CutterRadiusOff; }
	void G41Command();		// Cutter Radius Compensation left
	void G42Command();		// Cutter Radius Compensation right
	void G43Command();		// Tool Height Compensation 
	void G49Command()							{ _modalstate.ToolHeigtCompensation = 0; }
	void G53Command();
	void G5xCommand(unsigned char idx);
	void G61Command()							{ _modalstate.ConstantVelocity = false; }
	void G64Command()							{ _modalstate.ConstantVelocity = true; }
	void G8xCommand(SAxisMove& move, bool useP, bool useQ, bool useMinQ);
	void G73Command();		// High-speed Peck Drilling for Shallow Holes
	void G81Command();		// Basic drilling canned cycle
	void G82Command();		// Spot Drilling Cycle
	void G83Command();		// Peck Drilling for Deeper Holes
	void G90Command()							{ _modalstate.IsAbsolut = true; }
	void G91Command();
	void G92Command();
	void G98Command()							{ _modalstate.IsG98 = true; };
	void G99Command()							{ _modalstate.IsG98 = false; };

	void M00Command();		// Compulsory stop
	void M01Command();		// Optional stop
	void M02Command();		// End of program
	void M03Command();		// spindle on CW
	void M04Command();		// spindle on CCW
	void M05Command();		// spindle off
	void M06Command();		// Automatic tool change => not supported
	void M07Command();		// coolant on
	void M08Command();		// Coolant on (flood)
	void M09Command();		// coolant off
	void M110Command();

	/////////////////
};

////////////////////////////////////////////////////////
