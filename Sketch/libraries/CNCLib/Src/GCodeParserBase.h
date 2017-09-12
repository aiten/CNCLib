////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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
#include "MotionControlBase.h"
#include "Control.h"

////////////////////////////////////////////////////////

#ifdef REDUCED_SIZE
typedef uint8_t mcode_t;
#else
typedef unsigned int mcode_t ;
#endif

typedef uint8_t gcode_t;

////////////////////////////////////////////////////////


#define FEEDRATE_MIN_ALLOWED	STEPRATETOFEEDRATE(1)		// use VMAX => min is 1Steps/Sec because of CStepper
#define FEEDRATE_MAX_ALLOWED	feedrate_t(9999999)			// 

#define FEEDRATE_DEFAULT_G0		-feedrate_t(526518)			// VMAXTOFEEDRATE(((SPEED_MULTIPLIER_4)-5))
#define FEEDRATE_DEFAULT_G1		feedrate_t(100000)			// 100mm/min

#define FEEDRATE_DEFAULT_MAX_G1	feedrate_t(500000)			// 500mm/min is STEPRATETOFEEDRATE(26667) with 3200Steps/rotation(16Steps) 

////////////////////////////////////////////////////////

class CGCodeParserBase : public CParser
{
private:

	typedef CParser super;

public:

	CGCodeParserBase(CStreamReader* reader, Stream* output) : super(reader, output)		{  };

	static void SetG0FeedRate(feedrate_t feedrate)			{ _modalstate.G0FeedRate = feedrate; }
	static void SetG1FeedRate(feedrate_t feedrate)			{ _modalstate.G1FeedRate = feedrate; }
	static void SetG1MaxFeedRate(feedrate_t feedrate)		{ _modalstate.G1MaxFeedRate = feedrate; }
	static mm1000_t GetG92PosPreset(axis_t axis)			{ return _modalstate.G92Pospreset[axis]; }

	static feedrate_t GetG0FeedRate()						{ return _modalstate.G0FeedRate; }
	static feedrate_t GetG1FeedRate()						{ return _modalstate.G1FeedRate; }

	static bool IsMm1000()									{ return _modalstate.UnitisMm; }
	static bool IsInch(axis_t axis)							{ return !IsMm1000() && IsBitSet(_modalstate.UnitConvert,axis);}		

	static bool IsSpindleOn()								{ return _modalstate.SpindleOn; }

	static bool IsCutMove()									{ return _modalstate.CutMove; }
	static short GetSpindleSpeed()							{ return _modalstate.SpindleSpeed; }

	static void Init()										{ super::Init(); _modalstate.Init();  _modlessstate.Init(); }

	static void SetFeedRate(feedrate_t feedrateG0, feedrate_t feedrateG1, feedrate_t feedrateG1max) {	SetG0FeedRate(feedrateG0); SetG1FeedRate(feedrateG1); SetG1MaxFeedRate(feedrateG1max); }
	static void InitAndSetFeedRate(feedrate_t feedrateG0, feedrate_t feedrateG1, feedrate_t feedrateG1max) { Init();  SetG0FeedRate(feedrateG0); SetG1FeedRate(feedrateG1); SetG1MaxFeedRate(feedrateG1max); }

protected:

	// overrides to exend parser

	virtual void Parse() override;
	virtual bool InitParse() override;
	virtual void CleanupParse() override;

	virtual bool GCommand(gcode_t gcode);		// check for GCode extension => return true if command is parsed, false to do default
	virtual bool MCommand(mcode_t mcode);
	virtual bool Command(char ch);

	virtual bool ParseLineNumber();
	virtual char SkipSpacesOrComment() override;

	virtual mm1000_t CalcAllPreset(axis_t axis);
	virtual void CommentMessage(char* )					{ };

protected:

	typedef void(CGCodeParserBase::*LastCommandCB)();

	////////////////////////////////////////////////////////
	// Modal State

	struct SModalState
	{
#ifdef REDUCED_SIZE
		uint16_t		LineNumber;
#else
		long			ReceivedLineNumber;
		long			LineNumber;
#endif

		uint8_t			Plane_axis_0;			// x
		uint8_t			Plane_axis_1;			// y 

		uint8_t			Plane_axis_2;			// z
		uint8_t			UnitConvert;			// bit array convert between inch and mm (a b c is Grad) 			

		bool			UnitisMm;				// g20,g21
		bool			FeedRatePerUnit;		//feedrate per Unit(mm,inch) per min, or per revolution /g94/95

		bool			ConstantVelocity;		// G61 G64
		bool			IsAbsolut;

		feedrate_t		G0FeedRate;
		feedrate_t		G1FeedRate;
		feedrate_t		G1MaxFeedRate;

		short			SpindleSpeed;			// > 0 CW, < 0 CCW

		bool			CutMove;
		bool			SpindleOn;

		mm1000_t		G92Pospreset[NUM_AXIS];

		CGCodeParserBase::LastCommandCB LastCommand;

		void Init()	
		{
			*this = SModalState();		// POD .. Plane Old Data Type => no Constructor => init with default value = 0
//POD		Linenumber = 0;
//POD		LastCommand = NULL;
			UnitisMm = true;
			FeedRatePerUnit = true;
			ConstantVelocity = true;
			SpindleSpeed = 255;			// max of uint8_t (analog out)
			G0FeedRate = FEEDRATE_DEFAULT_G0;
			G1FeedRate = FEEDRATE_DEFAULT_G1;
			G1MaxFeedRate = FEEDRATE_DEFAULT_MAX_G1;
			IsAbsolut = true;
			Plane_axis_0 = X_AXIS;
			Plane_axis_1 = Y_AXIS;
			Plane_axis_2 = Z_AXIS;
			UnitConvert = 1+2+4 + 64+128;				// inch to mm 
//POD		for (register uint8_t i = 0; i < NUM_AXIS; i++) G92Pospreset[i] = 0;
		}
	};

	static SModalState _modalstate;

	////////////////////////////////////////////////////////
	// Modeless State

	struct SModelessState
	{
//		uint8_t	ZeroPresetIdx;				// 0:g53-, 1:G54-
		void Init()
		{
			*this = SModelessState();		// POD .. Plane Old Data Type => no Constructor => init with default value = 0
//			ZeroPresetIdx = _modalstate.ZeroPresetIdx;
		}
	};

	static SModelessState _modlessstate;

	////////////////////////////////////////////////////////
	// Parser structure

	struct SAxisMove
	{
		uint8_t axes;		// plural, each bit for axis
		union {
			struct
			{
				uint8_t I : 1;			// must be bit 0	=> see getIJK();
				uint8_t J : 1;			// must be bit 1
				uint8_t K : 1;			// must be bit 2
				uint8_t F : 1;
				uint8_t R : 1;
				uint8_t Q : 1;
				uint8_t P : 1;
				uint8_t L : 1;
			} bit;
			uint8_t all;
		} bitfield;

		mm1000_t newpos[NUM_AXIS];

		uint8_t GetIJK() { return bitfield.all & 7; }

		SAxisMove(bool getcurrentPosition) 
		{
			axes = 0; bitfield.all = 0;
			if (getcurrentPosition)
				CMotionControlBase::GetInstance()->GetPositions(newpos);
			else
			{
				for (register uint8_t i = 0; i < NUM_AXIS; i++) newpos[i] = 0;
			}
		}
	};

	////////////////////////////////////////////////////////

	void Sync();											// WaitBusy, sync movement with realtime
	void Wait(unsigned long ms);							// add "wait" in movement queue
	void SkipCommentNested();

	void ConstantVelocity();

	virtual mm1000_t ParseParameter(bool convertToInch);
	mm1000_t ParseCoordinate(bool convertUnits);
	mm1000_t ParseCoordinateAxis(axis_t axis);

	unsigned long GetUint32OrParam(unsigned long max);
	unsigned long GetUint32OrParam()						{ return GetUint32OrParam(0xffffffffl); };
	unsigned short GetUint16OrParam()						{ return (unsigned short)GetUint32OrParam(65535); };
	uint8_t GetUint8OrParam()								{ return (uint8_t)GetUint32OrParam(255); };

	mm1000_t GetRelativePosition(mm1000_t pos, axis_t axis)	{ return pos - CalcAllPreset(axis); }
	mm1000_t GetRelativePosition(axis_t axis)				{ return GetRelativePosition(CMotionControlBase::GetInstance()->GetPosition(axis), axis); }

	bool CheckAxisSpecified(axis_t axis, uint8_t& axes);
	axis_t CharToAxis(char axis);
	axis_t CharToAxisOffset(char axis);
	char AxisToChar(axis_t axis);

	uint8_t GetSubCode();

	enum EAxisPosType
	{
		AbsolutWithZeroShiftPosition,
		AbsolutPosition,
		RelativPosition
	};

	mm1000_t ParseCoordinate(axis_t axis, mm1000_t relpos, EnumAsByte(EAxisPosType) posType);

	void GetUint8(uint8_t& value, uint8_t&specified, uint8_t bit);

	void GetFeedrate(SAxisMove& move);
	void GetAxis(axis_t axis, SAxisMove& move, EnumAsByte(EAxisPosType) posType);

	void InfoNotImplemented()					{ Info(MESSAGE_GCODE_NotImplemented); }

	unsigned long GetDweel();

	void GetRadius(SAxisMove& move, mm1000_t& radius);

	void CallIOControl(uint8_t io, unsigned short value);
	void SpindleSpeedCommand();

	void MoveStart(bool cutmove);

	void G31Command(bool probevalue);
	bool ProbeCommand(SAxisMove& move, bool probevalue);

private:

	void GetIJK(axis_t axis, SAxisMove& move, mm1000_t offset[2]);

	void GetG92Axis(axis_t axis, uint8_t& count);

	static bool G31TestProbe(uintptr_t);

	bool LastCommand();

	void G00Command()							{ G0001Command(true); };
	void G01Command()							{ G0001Command(false); };
	void G0001Command(bool isG00);
	void G02Command()							{ G0203Command(true); };
	void G03Command()							{ G0203Command(false); };
	void G0203Command(bool isG02);
	void G04Command();
	void G171819Command(axis_t axis0, axis_t axis1, axis_t axis2);
	void G20Command()							{ _modalstate.UnitisMm = false; };
	void G21Command()							{ _modalstate.UnitisMm = true; };
	void G28Command();
	void G61Command()							{ _modalstate.ConstantVelocity = false; }
	void G64Command()							{ _modalstate.ConstantVelocity = true; }
	void G90Command()							{ _modalstate.IsAbsolut = true; }
	void G91Command();
	void G92Command();

	void M0304Command(bool m3);					// spindle on CW/CCW
	void M05Command()							{ _modalstate.SpindleOn = false; CallIOControl(CControl::SpindleCW, 0); } //Spindle off

	void M07Command()							{ CallIOControl(CControl::Coolant, CControl::CoolantOn); };
	void M09Command()							{ CallIOControl(CControl::Coolant, CControl::CoolantOff); };

	/////////////////

#ifdef REDUCED_SIZE
	mcode_t GetMCode()							{ return GetUInt8(); }
#else
	mcode_t GetMCode()							{ return GetUInt16(); }
#endif

	gcode_t GetGCode()							{ return GetUInt8(); }

	/////////////////

#ifdef _MSC_VER
public:
	static bool _exit;
#endif

};

////////////////////////////////////////////////////////
