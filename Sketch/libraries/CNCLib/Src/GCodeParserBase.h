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

//#define FEEDRATE_MIN_ALLOWED	STEPRATETOFEEDRATE(100)		// use VMAX => min is 100Steps/Sec because of CStepper
#define FEEDRATE_MIN_ALLOWED	STEPRATETOFEEDRATE(1)		// use VMAX => min is 1Steps/Sec because of CStepper
#define FEEDRATE_MAX_ALLOWED	feedrate_t(500000)			// 500mm/min is STEPRATETOFEEDRATE(26667) with 3200Steps/rotation(16Steps) 

#define FEEDRATE_MAX_G0			feedrate_t(526518)		// VMAXTOFEEDRATE(((SPEED_MULTIPLIER_4)-5))

#define FEEDRATE_DEFAULT_G0		-FEEDRATE_MAX_G0
#define FEEDRATE_DEFAULT_G1		feedrate_t(100000)						// 100mm/min

#define STEPRATE_REFMOVE		steprate_t(FEEDRATETOSTEPRATE(300000))	// 300*3.2/60 = 16000 Steps/sec with 3200Steps/rotation(16Steps) 

////////////////////////////////////////////////////////

class CGCodeParserBase : public CParser
{
private:

	typedef CParser super;

public:

	CGCodeParserBase(CStreamReader* reader, Stream* output) : super(reader, output)		{  };

	static void SetG0FeedRate(feedrate_t feedrate)			{ _modalstate.G0FeedRate = feedrate; }
	static void SetG1FeedRate(feedrate_t feedrate)			{ _modalstate.G1FeedRate = feedrate; }
	static mm1000_t GetG92PosPreset(axis_t axis)			{ return _modalstate.G92Pospreset[axis]; }

	static void Init()										{ super::Init(); _modalstate.Init();  _modlessstate.Init(); }

protected:

	// overrides to exend parser

	virtual void Parse();
	virtual bool InitParse();
	virtual void CleanupParse();

	virtual bool GCommand(unsigned char gcode);		// check for GCode extension => return true if command is parsed, false to do default
	virtual bool MCommand(unsigned char mcode);
	virtual bool Command(unsigned char ch);

	virtual bool ParseLineNumber(bool setlinenumber);	// line number is ignored! => ret is error
	virtual char SkipSpacesOrComment();

	virtual mm1000_t CalcAllPreset(axis_t axis);
	virtual void CommentMessage(char* )					{ };

protected:

	typedef void(CGCodeParserBase::*LastCommandCB)();

	////////////////////////////////////////////////////////
	// Modal State

	struct SModalState
	{
		long			Linenumber;

		unsigned char	Plane_axis_0;			// x
		unsigned char	Plane_axis_1;			// y 

		unsigned char	Plane_axis_2;			// z
		unsigned char	EvenAlign;			

		bool			UnitisMm;				// g20,g21
		bool			FeedRatePerUnit;		//feedrate per Unit(mm,inch) per min, or per revolution /g94/95

		bool			ConstantVelocity;		// G61 G64
		bool			IsAbsolut;

		feedrate_t		G0FeedRate;
		feedrate_t		G1FeedRate;

		short			SpindleSpeed;			// > 0 CW, < 0 CCW

		mm1000_t		G92Pospreset[NUM_AXIS];

		CGCodeParserBase::LastCommandCB LastCommand;

		void Init()	
		{
			*this = SModalState();		// POD .. Plane Old Daty Type => no Constructor => init with default value = 0
//POD		Linenumber = 0;
//POD		LastCommand = NULL;
			UnitisMm = true;
			FeedRatePerUnit = true;
			ConstantVelocity = true;
			SpindleSpeed = 1000;
			G0FeedRate = FEEDRATE_DEFAULT_G0;
			G1FeedRate = FEEDRATE_DEFAULT_G1;
			IsAbsolut = true;
			Plane_axis_0 = X_AXIS;
			Plane_axis_1 = Y_AXIS;
			Plane_axis_2 = Z_AXIS;
//POD		for (register unsigned char i = 0; i < NUM_AXIS; i++) G92Pospreset[i] = 0;
		}
	};

	static SModalState _modalstate;

	////////////////////////////////////////////////////////
	// Modeless State

	struct SModlessState
	{
//		unsigned char	ZeroPresetIdx;				// 0:g53-, 1:G54-
		void Init()
		{
			*this = SModlessState();		// POD .. Plane Old Daty Type => no Constructor => init with default value = 0
//			ZeroPresetIdx = _modalstate.ZeroPresetIdx;
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
				CMotionControlBase::GetInstance()->GetPositions(newpos);
			else
			{
				for (register unsigned char i = 0; i < NUM_AXIS; i++) newpos[i] = 0;
			}
		}
	};

	////////////////////////////////////////////////////////

	void Sync();											// WaitBusy, sync movement with realtime
	void Wait(unsigned long ms);							// add "wait" in movement queue
	void SkipCommentNested();

	virtual unsigned long ParseParameter();
	mm1000_t ParseCoordinate();

	unsigned long GetUint32OrParam(unsigned long max);
	unsigned long GetUint32OrParam()						{ return GetUint32OrParam(0xffffffffl); };
	unsigned short GetUint16OrParam()						{ return (unsigned short)GetUint32OrParam(65535); };
	unsigned char GetUint8OrParam()							{ return (unsigned char)GetUint32OrParam(255); };

	mm1000_t GetRelativePosition(mm1000_t pos, axis_t axis)	{ return pos - CalcAllPreset(axis); }
	mm1000_t GetRelativePosition(axis_t axis)				{ return GetRelativePosition(CMotionControlBase::GetInstance()->GetPosition(axis), axis); }
	mm1000_t ToInch(mm1000_t mm100);
	mm1000_t FromInch(mm1000_t mm100);

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

	void GetFeedrate(SAxisMove& move);
	void GetAxis(axis_t axis, SAxisMove& move, EnumAsByte(EAxisPosType) posType);

	void InfoNotImplemented()					{ Info(MESSAGE_GCODE_NotImplemented); }

	unsigned long GetDweel();

private:

	void GetIJK(axis_t axis, SAxisMove& move, mm1000_t offset[2]);
	void GetRadius(SAxisMove& move, mm1000_t& radius);

	void GetG92Axis(axis_t axis, unsigned char& count);

	static bool G31TestProbe(void*);

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
	void G31Command();
	void G61Command()							{ _modalstate.ConstantVelocity = false; }
	void G64Command()							{ _modalstate.ConstantVelocity = true; }
	void G90Command()							{ _modalstate.IsAbsolut = true; }
	void G91Command();
	void G92Command();

	void M03Command();		// spindle on CW
	void M04Command();		// spindle on CCW
	void M05Command();		// spindle off
	void M110Command();

	/////////////////

#ifdef _MSC_VER
public:
	static bool _exit;
#endif

};

////////////////////////////////////////////////////////
