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
*/
////////////////////////////////////////////////////////

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>
#include <StepperLib.h>

#include "Control.h"
#include "MotionControlBase.h"

#include "GCodeParserBase.h"

////////////////////////////////////////////////////////////

#ifdef _MSC_VER

bool CGCodeParserBase::_exit = false;

#endif

////////////////////////////////////////////////////////

#define MAXSPINDEL_SPEED	0x7fff

////////////////////////////////////////////////////////

#define MACHINE_SCALE		3		// 1/1000mm

#define COORD_SCALE_MM		SCALE_MM
#define COORD_MIN_MM		-999999l
#define COORD_MAX_MM		999999l

#define COORD_SCALE_INCH	SCALE_INCH
#define COORD_MIN_INCH		-9999999l
#define COORD_MAX_INCH		9999999l

#define COORD_MAXSCALE		255			// dont care about max scale => always round and skip

#define FEEDRATE_SCALE		3
#define FEEDRATE_MAXSCALE	255			// dont care about max scale => always round and skip
#define FEEDRATE_MIN		5l
#define FEEDRATE_MAX		99999999l

////////////////////////////////////////////////////////////

struct CGCodeParserBase::SModalState CGCodeParserBase::_modalstate;
struct CGCodeParserBase::SModlessState CGCodeParserBase::_modlessstate;

////////////////////////////////////////////////////////////

bool CGCodeParserBase::Command(char /* ch */ )
{
	return false; 
}

////////////////////////////////////////////////////////////

bool CGCodeParserBase::InitParse()
{
	if (!super::InitParse())
		return false;

	CStepper::GetInstance()->ClearError();
	CMotionControlBase::GetInstance()->ClearError();

	_modlessstate.Init();
	return true;				// continue
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::CleanupParse()
{
	_modlessstate.Init();		// state for no command
}


////////////////////////////////////////////////////////////

void CGCodeParserBase::SkipCommentNested()
{
	uint8_t cnt = 0;
	char*start = (char*)_reader->GetBuffer();

	for (char ch = _reader->GetChar(); ch; ch = _reader->GetNextChar())
	{
		switch (ch)
		{
			case ')': 
			{
				cnt--;
				if (cnt == 0)
				{
					_reader->GetNextChar();
					CommentMessage(start);
					return;
				}
				break;
			}
			case '(': cnt++; break;
		}
	}

	Error(MESSAGE(MESSAGE_GCODE_CommentNestingError));
}

////////////////////////////////////////////////////////////

char CGCodeParserBase::SkipSpacesOrComment()
{
	switch (_reader->SkipSpaces())
	{
		case '(':	SkipCommentNested();	break;
		case '*':
		case ';':	SkipCommentSingleLine(); break;
	}

	return _reader->GetChar();
}

////////////////////////////////////////////////////////////

mm1000_t CGCodeParserBase::ParseCoordinateAxis(axis_t axis)
{
	return ParseCoordinate(IsBitSet(CGCodeParserBase::_modalstate.UnitConvert, axis));
}

////////////////////////////////////////////////////////////

mm1000_t CGCodeParserBase::ParseCoordinate(bool convertUnits)
{
	_reader->SkipSpaces();

	bool convertToInch = convertUnits && !_modalstate.UnitisMm;

	if (_reader->GetChar() == '#')
	{
		_reader->GetNextChar();
		return ParseParameter(convertToInch); // do not convert if already mm1000
	}
	
	if (convertToInch)
	{
		// mm are is CDecimalAsInt with scale 3
		// inch is scale 5

		//	return MulDivI32(GetInt32Scale(COORD_MIN_INCH, COORD_MAX_INCH, COORD_SCALE_INCH, COORD_MAXSCALE), 254, 1000);
		return MulDivI32(GetInt32Scale(COORD_MIN_INCH, COORD_MAX_INCH, COORD_SCALE_INCH, COORD_MAXSCALE), 127, 500);
	}

	return GetInt32Scale(COORD_MIN_MM, COORD_MAX_MM, COORD_SCALE_MM, COORD_MAXSCALE);
};

////////////////////////////////////////////////////////////

mm1000_t CGCodeParserBase::ParseParameter(bool)
{
	Error(MESSAGE(MESSAGE_GCODE_ParameterNotFound));
	return 0;
}

////////////////////////////////////////////////////////////

unsigned long CGCodeParserBase::GetUint32OrParam(unsigned long max)
{
	unsigned long param = 0;
	if (_reader->GetChar() == '#')
	{
		_reader->GetNextChar();
		param = ParseParameter(false);
	}
	else
	{
		param = GetUInt32();
	}

	if (param > max)
	{
		Error(MESSAGE(MESSAGE_GCODE_ValueGreaterThanMax));
		return 0;
	}
	return param;
}

////////////////////////////////////////////////////////////

uint8_t CGCodeParserBase::GetSubCode()
{
	// subcode must follow immediately

	if (_reader->GetChar() != '.' || !IsUInt(_reader->GetNextChar()))
		return 255;

	return GetUInt8();
}

////////////////////////////////////////////////////////////

bool CGCodeParserBase::ParseLineNumber(bool setlinenumber)
{
	if (_reader->SkipSpacesToUpper() == 'N')
	{
		if (!IsUInt(_reader->GetNextChar()))
		{
			Error(MESSAGE(MESSAGE_GCODE_LinenumberExpected));
			return false;
		}
		long linenumber = GetInt32();
		if (setlinenumber && !_reader->IsError())
			_modalstate.Linenumber = linenumber;

		_reader->SkipSpaces();
	}
	return true;
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::MoveStart(bool cutmove)
{ 
	CControl::GetInstance()->CallOnEvent(CControl::OnStartCut, cutmove);
	_modalstate.CutMove = cutmove;
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::ConstantVelocity()
{
	if (!_modalstate.ConstantVelocity)
	{
		Wait(0);
	}
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::Wait(unsigned long ms)
{
	CStepper::GetInstance()->Wait(ms/10);
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::Sync()
{
	CStepper::GetInstance()->WaitBusy();
#ifdef _USE_LCD
	CControl::GetInstance()->Delay(0);
#endif
}

////////////////////////////////////////////////////////////

mm1000_t CGCodeParserBase::CalcAllPreset(axis_t axis)			
{ 
	return GetG92PosPreset(axis); 
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::Parse()
{
	do
	{
		char ch = _reader->GetCharToUpper();
		switch (ch)
		{
			case '\r':
			case '\t':
			case ' ':
			case '(':
			case '*':
			case ';':	SkipSpacesOrComment(); break;
			case 'N':
			{
				if (!ParseLineNumber(true))		return;
				break;
			}
			case 'G':
			{
				if (!IsUInt(_reader->GetNextChar()))
				{
					Error(MESSAGE(MESSAGE_GCODE_CommandExpected));		return;
				}
				if (!GCommand(GetGCode()))
				{
					Error(MESSAGE(MESSAGE_GCODE_UnsupportedGCommand));	return;
				}
				break;
			}
			case 'M':
			{
				if (!IsUInt(_reader->GetNextChar()))
				{
					Error(MESSAGE(MESSAGE_GCODE_MCodeExpected));		return;
				}
				if (!MCommand(GetMCode()))
				{
					Error(MESSAGE(MESSAGE_GCODE_UnspportedMCodeIgnored));	return;
				}
				break;
			}

			default:
#ifdef _MSC_VER
				if (IsToken(F("X"), true, false)) { _exit = true; return; }
#endif
				if (!Command(ch))
				{
					if (!LastCommand())
					{
						Error(MESSAGE(MESSAGE_GCODE_IllegalCommand));
						return;
					}
				}
				break;
		}

		if (CheckError()) return;

	} while (_reader->GetChar() != 0);
}

////////////////////////////////////////////////////////////

bool CGCodeParserBase::GCommand(uint8_t gcode)
{
	switch (gcode)
	{
		case 0:		G00Command(); return true;
		case 1:		G01Command(); return true;
		case 2:		G02Command(); return true;
		case 3:		G03Command(); return true;
		case 4:		G04Command(); return true;
		case 17:	G171819Command(X_AXIS, Y_AXIS, Z_AXIS); return true;
		case 18:	G171819Command(Z_AXIS, X_AXIS, Y_AXIS); return true;
		case 19:	G171819Command(Y_AXIS, Z_AXIS, X_AXIS); return true;
		case 20:	G20Command(); return true;
		case 21:	G21Command();  return true;
		case 28:	G28Command(); return true;
		case 31:	G31Command(); return true;
		case 61:	G61Command(); return true;
		case 64:	G64Command(); return true;
		case 90:	G90Command(); return true;
		case 91:	G91Command(); return true;
		case 92:	G92Command(); return true;
	}
	return false;
}

////////////////////////////////////////////////////////////

bool CGCodeParserBase::MCommand(mcode_t mcode)
{
	switch (mcode)
	{
		// Spindel (+laser)
		case 106:
		case 3:	M0304Command(true);		return true;
		case 4:	M0304Command(false);	return true;
		case 107:
		case 5: M05Command();			return true;

		// coolant
		case 7:	M07Command();		return true;
		case 9:	M09Command();		return true;
	}
	return false;
}

////////////////////////////////////////////////////////////

mm1000_t CGCodeParserBase::ParseCoordinate(axis_t axis, mm1000_t relpos, EnumAsByte(EAxisPosType) posType)
{
	mm1000_t mm = ParseCoordinateAxis(axis);
	switch (posType)
	{
		default:
		case AbsolutWithZeroShiftPosition:	return mm + CalcAllPreset(axis);
		case AbsolutPosition:				return mm; 
		case RelativPosition:				return relpos + mm;
	}
}

////////////////////////////////////////////////////////////

bool CGCodeParserBase::CheckAxisSpecified(axis_t axis, uint8_t& axes)
{
	if (axis >= NUM_AXIS)
	{
		Error(MESSAGE(MESSAGE_GCODE_AxisNotSupported));
		return false;
	}

	if (IsBitSet(axes, axis))
	{
		Error(MESSAGE(MESSAGE_GCODE_AxisAlreadySpecified));
		return false;
	}

	axes += 1 << axis;

	return true;
}

////////////////////////////////////////////////////////////

axis_t CGCodeParserBase::CharToAxis(char axis)
{
	switch (axis)
	{
		case 'X': return X_AXIS;
		case 'Y': return Y_AXIS;
		case 'Z': return Z_AXIS;
		case 'A': return A_AXIS;
		case 'B': return B_AXIS;
		case 'C': return C_AXIS;
		case 'U': return U_AXIS;
		case 'V': return V_AXIS;
		case 'W': return W_AXIS;
		case 'E': return B_AXIS;		// for 3dprinter
		default: return 255;
	}
}

////////////////////////////////////////////////////////////

axis_t CGCodeParserBase::CharToAxisOffset(char axis)
{
	switch (axis)
	{
		case 'I': return X_AXIS;
		case 'J': return Y_AXIS;
		case 'K': return Z_AXIS;
		default: return 255;
	}
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::GetUint8(uint8_t& value, uint8_t&specified, uint8_t bit)
{
	if (IsBitSet(specified, bit))
	{
		Error(MESSAGE(MESSAGE_GCODE_ParameterSpecifiedMoreThanOnce));
		return;
	}

	BitSet(specified, bit);

	_reader->GetNextChar();
	value = GetUInt8();
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::GetAxis(axis_t axis, SAxisMove& move, EnumAsByte(EAxisPosType) posType)
{
	if (!CheckAxisSpecified(axis, move.axes))
		return;

	_reader->GetNextChar();

	move.newpos[axis] = ParseCoordinate(axis, move.newpos[axis], posType);
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::GetIJK(axis_t axis, SAxisMove& move, mm1000_t offset[2])
{
	if (!CheckAxisSpecified(axis, move.bitfield.all))
		return;

	_reader->GetNextChar();

	if (axis == _modalstate.Plane_axis_0)
		offset[0] = ParseCoordinateAxis(axis);
	else if (axis == _modalstate.Plane_axis_1)
		offset[1] = ParseCoordinateAxis(axis);
	else
	{
		Error(MESSAGE(MESSAGE_GCODE_AxisOffsetMustNotBeSpecified));
	}
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::GetRadius(SAxisMove& move, mm1000_t& radius)
{
	if (move.bitfield.bit.R)
	{
		Error(MESSAGE(MESSAGE_GCODE_RalreadySpecified));
		return;
	}
	move.bitfield.bit.R = true;

	_reader->GetNextChar();
	radius = ParseCoordinateAxis(_modalstate.Plane_axis_0);
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::GetFeedrate(SAxisMove& move)
{
	_reader->GetNextChar();

	if (move.bitfield.bit.F)
	{
		Error(MESSAGE(MESSAGE_GCODE_FalreadySpecified));
		return;
	}
	move.bitfield.bit.F = true;

	if (!_modalstate.FeedRatePerUnit) { ErrorNotImplemented(); return; }

	feedrate_t feedrate = GetInt32Scale(FEEDRATE_MIN, FEEDRATE_MAX, FEEDRATE_SCALE, FEEDRATE_MAXSCALE);
	// feedrate is 1/1000mm/min (scale 3) 

	if (!_modalstate.UnitisMm)
	{
//		feedrate = MulDivI32(feedrate, 254, 10);
		feedrate = MulDivI32(feedrate, 127, 5);
	}

	if (CheckError()) { return; }

	feedrate_t minfeedrate = FEEDRATE_MIN_ALLOWED;

	if (feedrate < minfeedrate)				  feedrate = minfeedrate;
	if (feedrate > _modalstate.G1MaxFeedRate) feedrate = _modalstate.G1MaxFeedRate;

	SetG1FeedRate(feedrate);
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::GetG92Axis(axis_t axis, uint8_t& axes)
{
	if (!CheckAxisSpecified(axis, axes))
		return;

	_reader->GetNextChar();
	_modalstate.G92Pospreset[axis] = 0;	// clear this => can use CalcAllPreset
	_modalstate.G92Pospreset[axis] = ParseCoordinateAxis(axis) + CMotionControlBase::GetInstance()->GetPosition(axis) - CalcAllPreset(axis);
}

////////////////////////////////////////////////////////////

bool CGCodeParserBase::LastCommand()
{
	const char* old = _reader->GetBuffer();

	if (_modalstate.LastCommand != NULL)
		(*this.*_modalstate.LastCommand)();

	if (old == _reader->GetBuffer())
	{
		return false;	// ERROR
	}
	return true;
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::G0001Command(bool isG00)
{
	// we assume:
	// g0				no cut move
	// g1 / g2 /g3		cut move (see MoveStart()) 

	// Extention to standard:
	// g0 x10 F  => g0 with feedrate (not CutMove)
	// g0 x10 F1 => error


	_modalstate.LastCommand = isG00 ? &CGCodeParserBase::G00Command : &CGCodeParserBase::G01Command;
	bool useG0Feed = isG00;

	SAxisMove move(true);

	for (char ch = _reader->SkipSpacesToUpper(); ch; ch = _reader->SkipSpacesToUpper())
	{
		axis_t axis;
		if ((axis = CharToAxis(ch)) < NUM_AXIS) GetAxis(axis, move, _modalstate.IsAbsolut ? AbsolutWithZeroShiftPosition : RelativPosition);
		else if (ch == 'F' && !isG00) GetFeedrate(move);
		else if (ch == 'F' && isG00) {
										if (IsInt(_reader->GetNextCharSkipScaces())) 
										{ 
											Error(MESSAGE(MESSAGE_GCODE_FeedrateWithG0)); 
											return; 
										}
										useG0Feed = false; 
									  }
		else break;

		if (CheckError()) { return; }
	}

	if (move.axes)
	{
		MoveStart(!isG00);
		CMotionControlBase::GetInstance()->MoveAbs(move.newpos, useG0Feed ? _modalstate.G0FeedRate : _modalstate.G1FeedRate);
		ConstantVelocity();
	}
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::G0203Command(bool isG02)
{
	_modalstate.LastCommand = isG02 ? &CGCodeParserBase::G02Command : &CGCodeParserBase::G03Command;

	SAxisMove move(true);
	mm1000_t radius;
	mm1000_t offset[2] = { 0, 0 };

	for (char ch = _reader->SkipSpacesToUpper(); ch; ch = _reader->SkipSpacesToUpper())
	{
		axis_t axis;
		if ((axis = CharToAxis(ch)) < NUM_AXIS)				GetAxis(axis, move, _modalstate.IsAbsolut ? AbsolutWithZeroShiftPosition : RelativPosition);
		else if ((axis = CharToAxisOffset(ch)) < NUM_AXIS)	GetIJK(axis, move, offset);
		else if (ch == 'R')									GetRadius(move, radius);
		else if (ch == 'F')									GetFeedrate(move);
		else break;

		if (CheckError()) { return; }
	}

	if (move.bitfield.bit.R && move.GetIJK())		{ Error(MESSAGE(MESSAGE_GCODE_IJKandRspecified)); return; }
	if (!move.bitfield.bit.R && !move.GetIJK())		{ Error(MESSAGE(MESSAGE_GCODE_MissingIKJorR)); return; }
	if (CheckError()) { return; }

	if (move.bitfield.bit.R)
	{
		// Calculate the change in position along each selected axis
		float x = (float)(move.newpos[_modalstate.Plane_axis_0] - CMotionControlBase::GetInstance()->GetPosition(_modalstate.Plane_axis_0));
		float y = (float)(move.newpos[_modalstate.Plane_axis_1] - CMotionControlBase::GetInstance()->GetPosition(_modalstate.Plane_axis_1));
		float r = (float)radius;

		if (x == 0.0 && y == 0.0)						{ Error(MESSAGE(MESSAGE_GCODE_360withRandMissingAxes)); return; }

		// First, use h_x2_div_d to compute 4*h^2 to check if it is negative or r is smaller
		// than d. If so, the sqrt of a negative number is complex and error out.
		float h_x2_div_d = 4 * r*r - x*x - y*y;
		if (h_x2_div_d < 0)								{ Error(MESSAGE(MESSAGE_GCODE_STATUS_ARC_RADIUS_ERROR)); return; }

		// Finish computing h_x2_div_d.
		h_x2_div_d = -sqrt(h_x2_div_d) / hypot(x, y); // == -(h * 2 / d)

		// Invert the sign of h_x2_div_d if the circle is counter clockwise (see sketch below)
		if (!isG02) { h_x2_div_d = -h_x2_div_d; }	//CCW

		if (r < 0.0)
		{
			h_x2_div_d = -h_x2_div_d;
			r = -r; // Finished with r. 
		}

		// Complete the operation by calculating the actual center of the arc
		offset[0] = mm1000_t(0.5*(x - (y*h_x2_div_d)));
		offset[1] = mm1000_t(0.5*(y + (x*h_x2_div_d)));
	}

	MoveStart(true);
	CMotionControlBase::GetInstance()->Arc(move.newpos, offset[0], offset[1], _modalstate.Plane_axis_0, _modalstate.Plane_axis_1, isG02, _modalstate.G1FeedRate);
	ConstantVelocity();
}

////////////////////////////////////////////////////////////

unsigned long CGCodeParserBase::GetDweel()
{
	const char*current = _reader->GetBuffer();
	unsigned long dweelms = GetUint32OrParam();
	if (_reader->GetChar() == '.')
	{
		// this is "sec" and not "ms"
		_reader->ResetBuffer(current);
		dweelms = GetInt32Scale(0,1000000,3,255);
	}
	return dweelms;
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::G04Command()
{
	unsigned long dweelms = 0;

	if (_reader->SkipSpacesToUpper() == 'P')
	{
		_reader->GetNextChar();
		dweelms = GetDweel();
	}

#ifndef REDUCED_SIZE
	if (!ExpectEndOfCommand())		{ return; }
#endif

	Wait(dweelms);
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::G171819Command(axis_t axis0, axis_t axis1, axis_t axis2)
{
	_modalstate.Plane_axis_0 = axis0;
	_modalstate.Plane_axis_1 = axis1;
	_modalstate.Plane_axis_2 = axis2;
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::G28Command()
{
	SAxisMove move(false);

	for (char ch = _reader->SkipSpacesToUpper(); ch; ch = _reader->SkipSpacesToUpper())
	{
		axis_t axis;
		if ((axis = CharToAxis(ch)) < NUM_AXIS) GetAxis(axis, move, AbsolutPosition);
		else break;

		if (CheckError()) { return; }
	}

#ifndef REDUCED_SIZE
	if (!ExpectEndOfCommand())		{ return; }
#endif

	if (move.axes == 0)
	{
		CControl::GetInstance()->GoToReference();
	}
	else
	{
		for (uint8_t axis = 0; axis < NUM_AXIS; axis++)
		{
			if (IsBitSet(move.axes, axis))
			{
				bool usmin = CStepper::GetInstance()->IsUseReference(CStepper::GetInstance()->ToReferenceId(axis, true));
				bool usmax = CStepper::GetInstance()->IsUseReference(CStepper::GetInstance()->ToReferenceId(axis, false));
				if (usmin || usmax)
					CControl::GetInstance()->GoToReference(axis, 0, usmin);
				else
					CStepper::GetInstance()->SetPosition(axis, move.newpos[axis]);
			}
		}
	}
	CMotionControlBase::GetInstance()->SetPositionFromMachine();
}

////////////////////////////////////////////////////////////

bool CGCodeParserBase::G31TestProbe(uintptr_t /* param */)
{
	// return true if probe is not on
	// => continue move to probe position
	// return false if probe is on (probe switch is pressed)

	return CControl::GetInstance()->IOControl(CControl::Probe) == 0;
}

void CGCodeParserBase::G31Command()
{
	// probe
	SAxisMove move(true);

	for (char ch = _reader->SkipSpacesToUpper(); ch; ch = _reader->SkipSpacesToUpper())
	{
		axis_t axis;
		if ((axis = CharToAxis(ch)) < NUM_AXIS) GetAxis(axis, move, _modalstate.IsAbsolut ? AbsolutWithZeroShiftPosition : RelativPosition);
		else if (ch == 'F') GetFeedrate(move);
		else break;

		if (CheckError()) { return; }
	}

	if (move.axes==0)
	{
		Error(MESSAGE(MESSAGE_GCODE_NoAxesForProbe));
		return;
	}

	if ((move.axes&7) == 0)
	{
		Error(MESSAGE(MESSAGE_GCODE_ProbeOnlyForXYZ));
		return;
	}
	
	Sync();

	{
		if (!G31TestProbe(0))
		{
			Error(MESSAGE(MESSAGE_GCODE_ProbeIOmustBeOff));
			return;
		}

//		MoveStart(true);
		CMotionControlBase::GetInstance()->MoveAbs(move.newpos, _modalstate.G1FeedRate);

		if (!CStepper::GetInstance()->MoveUntil(G31TestProbe, 0))
		{
			Error(MESSAGE(MESSAGE_GCODE_ProbeFailed));
			// no return => must set position again
		}
		CMotionControlBase::GetInstance()->SetPositionFromMachine();
	}
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::G91Command()
{
	uint8_t subcode = GetSubCode();

	switch (subcode)
	{
		case 1:		break;	//OK (I,J,K relative) = default
		case 255:	_modalstate.IsAbsolut = false; break;
		default:	ErrorNotImplemented(); break;
	}
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::G92Command()
{
	uint8_t axes = 0;

	for (char ch = _reader->SkipSpacesToUpper(); ch; ch = _reader->SkipSpacesToUpper())
	{
		axis_t axis;
		if ((axis = CharToAxis(ch)) < NUM_AXIS) GetG92Axis(axis, axes);
		else break;

		if (CheckError()) { return; }
	}

	if (axes == 0)
	{
		for (axes = 0; axes < NUM_AXIS; axes++) _modalstate.G92Pospreset[axes] = 0;
	}

	CLcd::InvalidateLcd(); 
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::SpindleSpeedCommand()
{
	_reader->SkipSpaces();
	short speed = (short) GetUint32OrParam(MAXSPINDEL_SPEED);

#ifndef REDUCED_SIZE
	if (IsError()) return;
#endif

	_modalstate.SpindleSpeed = speed;
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::CallIOControl(uint8_t io, unsigned short value)
{
	CStepper::GetInstance()->IoControl(io, value);
}

////////////////////////////////////////////////////////////

void CGCodeParserBase::M0304Command(bool m3)
{
	char ch = _reader->SkipSpacesToUpper();
	if (ch == 'S')
	{
		_reader->GetNextChar();
		SpindleSpeedCommand();
	}

	_modalstate.SpindleOn = true;
	CallIOControl(CControl::Spindel, m3 ? _modalstate.SpindleSpeed : -_modalstate.SpindleSpeed);
}

////////////////////////////////////////////////////////////
