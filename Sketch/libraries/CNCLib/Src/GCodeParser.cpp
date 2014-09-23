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
*/
////////////////////////////////////////////////////////

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>
#include <StepperLib.h>

#include "Control.h"
#include "HelpParser.h"
#include "MotionControl.h"
#include "ExpressionParser.h"

#include "GCodeParser.h"
#include "GCodeExpressionParser.h"

////////////////////////////////////////////////////////

#define MASCHINE_SCALE		3		// 1/1000mm

#define COORD_SCALE_MM		3
#define COORD_MIN_MM		-999999l
#define COORD_MAX_MM		999999l

#define COORD_SCALE_INCH	4
#define COORD_MIN_INCH		-99999l
#define COORD_MAX_INCH		99999l

#define COORD_MAXSCALE		255			// dont care about max scale => always round and skip

#define FEEDRATE_SCALE		3
#define FEEDRATE_MAXSCALE	255			// dont care about max scale => always round and skip
#define FEEDRATE_MIN		5l
#define FEEDRATE_MAX		99999999l

#define MAXSPINDEL_SPEED	0x7fff

////////////////////////////////////////////////////////////

struct CGCodeParser::SModalState CGCodeParser::_modalstate;
struct CGCodeParser::SModlessState CGCodeParser::_modlessstate;

////////////////////////////////////////////////////////////

bool CGCodeParser::Command(unsigned char /* ch */)			{ return false; }
bool CGCodeParser::SetParamCommand(param_t /* paramNo */)	{ return false; }

////////////////////////////////////////////////////////////

bool CGCodeParser::InitParse()
{
	if (!super::InitParse())
		return false;

	CStepper::GetInstance()->ClearError();

	_modlessstate.Init();
	return true;				// continue
}

////////////////////////////////////////////////////////////

void CGCodeParser::CleanupParse()
{
	_modlessstate.Init();		// state for no command
}


////////////////////////////////////////////////////////////

void CGCodeParser::SkipCommentNested()
{
	unsigned char cnt = 0;

	for (char ch = _reader->GetChar(); ch; ch = _reader->GetNextChar())
	{
		switch (ch)
		{
			case 0:	Error(MESSAGE_GCODE_CommentNestingError);	return;
			case ')': cnt--; break;
			case '(': cnt++; break;
		}

		if (cnt == 0)
		{
			_reader->GetNextChar();
			SkipSpacesOrComment();
			return;
		}
	}
}

////////////////////////////////////////////////////////////

char CGCodeParser::SkipSpacesOrComment()
{
	switch (_reader->SkipSpaces())
	{
		case '(':	
		{
			char*start = (char*)_reader->GetBuffer();
			bool isMsg=TryToken(start,F("(MSG,"), false, true);
			SkipCommentNested(); 
			if (isMsg)
			{
				start += 5;
				while (start+1 < _reader->GetBuffer())
					StepperSerial.print(*(start++));
				StepperSerial.println();
			}
			break;
		}
		case '*':
		case ';':	SkipCommentSingleLine(); break;
	}

	return _reader->GetChar();
}

////////////////////////////////////////////////////////////

param_t CGCodeParser::ParseParamNo()
{
	if (_reader->SkipSpacesToUpper() == '<')		// named parameter
	{
		_reader->GetNextChar();
		char ch = _reader->SkipSpacesToUpper();
		const char* start = _reader->GetBuffer();

		if (!CStreamReader::IsAlpha(ch))
		{
			Error(MESSAGE_GCODE_VaribaleMustStartWithAlpha); return 0;
		}

		while (CStreamReader::IsAlpha(ch) || CStreamReader::IsDigit(ch))
		{
			ch = _reader->GetNextChar();
		}
		const char* end = _reader->GetBuffer();

		if (CStreamReader::IsSpaceOrEnd(ch))
			ch = _reader->SkipSpacesToUpper();

		if (ch != '>')
		{
			Error(MESSAGE_GCODE_NoValidVaribaleName); return 0;
		}

		_reader->GetNextChar();

		CStreamReader::CSetTemporary terminate(end);

		if (start[0] == '_')			// all system parameter start with _
		{
			// see: http://www.linuxcnc.org/docs/devel/html/gcode/overview.html#_predefined_named_parameters_a_id_sec_predefined_named_parameters_a
			axis_t a;
			if (start[2] == 0 && (a = CharToAxis(CStreamReader::Toupper(start[1]))) < NUM_AXIS)
			{
				return PARAMSTART_CURRENTPOS + a;
			}
		}

		Error(MESSAGE_GCODE_ParameterDoesntExist);
		return 0;
	}

	if (IsUInt(_reader->GetChar()))
	{
		return GetUInt16();
	}

	Error(MESSAGE_GCODE_NoValidVaribaleName);;
	return 0;
}

////////////////////////////////////////////////////////////

unsigned long CGCodeParser::ParseParameter()
{
	param_t paramNo = ParseParamNo();
	if (paramNo)
		return GetParamValue(paramNo);

	Error(MESSAGE_GCODE_ParameterNotFound);
	return 0;
}

////////////////////////////////////////////////////////////

static bool IsParam(param_t paramNo, param_t offset, axis_t&axis)
{
	if (paramNo < offset || paramNo >= offset+NUM_AXIS) return false;
	axis = (axis_t) (paramNo-offset);
	return true;
}

static bool IsParam(param_t paramNo, param_t offset)
{
	return paramNo == offset;
}

static bool IsModifyParam(param_t paramNo)								{ return paramNo >= 1 && paramNo <= NUM_PARAMETER; }

// 5161-5169 - G28 Home for (X Y Z A B C U V W)
static bool IsG28HomeParam(param_t paramNo, axis_t&axis)				{ return IsParam(paramNo, PARAMSTART_G28HOME, axis); }

static bool IsG92OffsetParam(param_t paramNo, axis_t&axis)				{ return IsParam(paramNo, PARAMSTART_G92OFFSET, axis); }

// 5221-5230 - Coordinate System 1, G54 (X Y Z A B C U V W R) - R denotes the XY rotation angle around the Z axis 
static bool IsG54OffsetParam(param_t paramNo, axis_t&axis)				{ return IsParam(paramNo, PARAMSTART_G54OFFSET, axis); }

// 5420-5428 - Current Position including offsets in current program units (X Y Z A B C U V W)
static bool IsCurrentPosParam(param_t paramNo, axis_t&axis)				{ return IsParam(paramNo, PARAMSTART_CURRENTPOS, axis); }

// customized extension
static bool IsCurrentAbsPosParam(param_t paramNo, axis_t&axis)			{ return IsParam(paramNo, PARAMSTART_CURRENTABSPOS, axis); }
static bool IsBacklashParam(param_t paramNo, axis_t&axis)				{ return IsParam(paramNo, PARAMSTART_BACKLASH, axis); }
static bool IsBacklashFeedrateParam(param_t paramNo)					{ return IsParam(paramNo, PARAMSTART_BACKLASH_FEEDRATE); }
static bool IsMaxParam(param_t paramNo, axis_t&axis)					{ return IsParam(paramNo, PARAMSTART_MAX, axis); }
static bool IsMinParam(param_t paramNo, axis_t&axis)					{ return IsParam(paramNo, PARAMSTART_MIN, axis); }
static bool IsAccParam(param_t paramNo, axis_t&axis)					{ return IsParam(paramNo, PARAMSTART_ACC, axis); }
static bool IsDecParam(param_t paramNo, axis_t&axis)					{ return IsParam(paramNo, PARAMSTART_DEC, axis); }
static bool IsJerkParam(param_t paramNo, axis_t&axis)					{ return IsParam(paramNo, PARAMSTART_JERK, axis); }

static bool IsControllerFanParam(param_t paramNo)						{ return IsParam(paramNo, PARAMSTART_CONTROLLERFAN); }
static bool IsRapidMoveFeedRate(param_t paramNo)						{ return IsParam(paramNo, PARAMSTART_RAPIDMOVEFEED); }


mm1000_t CGCodeParser::GetParamValue(param_t paramNo)
{
	if (IsModifyParam(paramNo))
		return _modalstate.Parameter[paramNo - 1];

	axis_t axis;

	if (IsG28HomeParam(paramNo,axis))
	{
		mm1000_t pos = CStepper::GetInstance()->GetLimitMin(axis);
		if (CStepper::GetInstance()->IsUseReference(CStepper::GetInstance()->ToReferenceId(axis, false)))	// max refmove
			pos = CStepper::GetInstance()->GetLimitMax(axis);
		return GetParamAsPosition(pos,axis);
	}

	if (IsG92OffsetParam(paramNo,axis))			return GetParamAsPosition(_modalstate.G92Pospreset[axis],axis);
	if (IsG54OffsetParam(paramNo,axis))			return GetParamAsPosition(_modalstate.G54Pospreset[axis],axis);
	if (IsCurrentPosParam(paramNo,axis))		return GetParamAsPosition(GetRelativePosition(axis),axis);

	// customized extension
	if (IsCurrentAbsPosParam(paramNo,axis))		return GetParamAsPosition(CMotionControl::GetPosition(axis),axis);
	if (IsBacklashParam(paramNo,axis))			return GetParamAsPosition(CStepper::GetInstance()->GetBacklash(axis),axis);

	if (IsMaxParam(paramNo, axis))				return GetParamAsPosition(CStepper::GetInstance()->GetLimitMax(axis),axis);
	if (IsMinParam(paramNo, axis))				return GetParamAsPosition(CStepper::GetInstance()->GetLimitMin(axis),axis);
	if (IsAccParam(paramNo, axis))				return CStepper::GetInstance()->GetAcc(axis);
	if (IsDecParam(paramNo, axis))				return CStepper::GetInstance()->GetDec(axis);
	if (IsJerkParam(paramNo, axis))				return CStepper::GetInstance()->GetJerkSpeed(axis);

	Error(MESSAGE_GCODE_ParameterNotFound);
	return 0;
}

////////////////////////////////////////////////////////////

void CGCodeParser::SetParamValue(param_t paramNo)
{
	CGCodeExpressionParser exprpars(this);
	exprpars.Parse();
	if (exprpars.IsError())
		Error(exprpars.GetError());
	else
	{
		axis_t axis;
		mm1000_t mm1000 = CMotionControl::FromDouble(exprpars.Answer);

		if (IsModifyParam(paramNo))				{	_modalstate.Parameter[paramNo - 1] = mm1000;	}
		else if (IsBacklashParam(paramNo,axis))	{	CStepper::GetInstance()->SetBacklash(axis,GetParamAsMaschine(mm1000, axis));	}
		else if (IsBacklashFeedrateParam(paramNo)){	CStepper::GetInstance()->SetBacklash((steprate_t) CMotionControl::ToMachine(0, mm1000*60));		}
		else if (IsControllerFanParam(paramNo))	{	CControl::GetInstance()->IOControl(CControl::ControllerFan,(unsigned short)exprpars.Answer);	}
		else if (IsRapidMoveFeedRate(paramNo))	{	SetG0FeedRate((feedrate_t) (-exprpars.Answer*1000));	}
		else if (IsMaxParam(paramNo,axis))		{	CStepper::GetInstance()->SetLimitMax(axis,GetParamAsMaschine(mm1000, axis));		}
		else if (IsMinParam(paramNo,axis))		{	CStepper::GetInstance()->SetLimitMin(axis,GetParamAsMaschine(mm1000, axis));		}
		else if (IsAccParam(paramNo,axis))		{	CStepper::GetInstance()->SetAcc(axis,(steprate_t) mm1000);	}
		else if (IsDecParam(paramNo,axis))		{	CStepper::GetInstance()->SetDec(axis,(steprate_t) mm1000);	}
		else if (IsJerkParam(paramNo,axis))		{	CStepper::GetInstance()->SetJerkSpeed(axis,(steprate_t) mm1000);	}
		else
		{
			Error(MESSAGE_GCODE_UnspportedParameterNumber);	return;
		}

		// rest of line only comment allowed!
		ExpectEndOfCommand();
	}
}

////////////////////////////////////////////////////////////

mm1000_t CGCodeParser::ParseCoordinate()
{
	_reader->SkipSpaces();

	if (_reader->GetChar() == '#')
	{
		_reader->GetNextChar();
		return (mm1000_t)ParseParameter();
	}

	if (_modalstate.UnitisMm)
		return GetInt32Scale(COORD_MIN_MM, COORD_MAX_MM, COORD_SCALE_MM, COORD_MAXSCALE);

	return FromInch(GetInt32Scale(COORD_MIN_INCH, COORD_MAX_INCH, COORD_SCALE_INCH, COORD_MAXSCALE));
};

////////////////////////////////////////////////////////////

unsigned long CGCodeParser::GetUint32OrParam(unsigned long max)
{
	unsigned long param = 0;
	if (_reader->GetChar() == '#')
	{
		_reader->GetNextChar();
		param = ParseParameter();
	}
	else
	{
		param = GetUInt32();
	}

	if (param > max)
	{
		Error(MESSAGE_GCODE_ValueGreaterThanMax);
		return 0;
	}
	return param;
}

////////////////////////////////////////////////////////////

mm1000_t CGCodeParser::ToInch(mm1000_t mm100)
{
	if (_modalstate.UnitisMm)
		return mm100;

	return MulDivI32(mm100, 254, 100);
}

////////////////////////////////////////////////////////////

mm1000_t CGCodeParser::FromInch(mm1000_t mm100)
{
	if (_modalstate.UnitisMm)
		return mm100;

	return MulDivI32(mm100, 100, 254);
}

////////////////////////////////////////////////////////////

unsigned char CGCodeParser::GetSubCode()
{
	// subcode must follow immediately

	if (_reader->GetChar() != '.' || !IsUInt(_reader->GetNextChar()))
		return 255;

	return GetUInt8();
}

////////////////////////////////////////////////////////////

bool CGCodeParser::ParseLineNumber(bool setlinenumber)
{
	if (_reader->SkipSpacesToUpper() == 'N')
	{
		if (!IsUInt(_reader->GetNextChar()))
		{
			Error(MESSAGE_GCODE_LinenumberExpected);
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

void CGCodeParser::Delay(unsigned long ms)
{
	CStepper::GetInstance()->WaitBusy();
	CControl::GetInstance()->Delay(ms);
}

////////////////////////////////////////////////////////////

mm1000_t CGCodeParser::GetG54PosPreset(axis_t axis)
{
	switch (_modlessstate.ZeroPresetIdx)
	{
		//		default:
		//		case 0: return 0;

		//G54
		case 1: return _modalstate.G54Pospreset[axis];


			//predifined Positions:
			//G55
			// Z always negativ
		case 2:
			if (axis == Z_AXIS) return CMotionControl::ToMm1000(axis, CStepper::GetInstance()->GetLimitMax(axis));
			break;

			//G56
			// Y,Z always negativ
		case 3:
			if (axis == Y_AXIS || axis == Z_AXIS) return CMotionControl::ToMm1000(axis, CStepper::GetInstance()->GetLimitMax(axis));
			break;

			//G57
			// X,Y,Z always negativ
		case 4:
			if (axis == X_AXIS || axis == Y_AXIS || axis == Z_AXIS) return CMotionControl::ToMm1000(axis, CStepper::GetInstance()->GetLimitMax(axis));
			break;

			//G58
			// X/2,Y/2, Z always negativ
		case 5:
			if (axis == X_AXIS || axis == Y_AXIS) return CMotionControl::ToMm1000(axis, CStepper::GetInstance()->GetLimitMax(axis) - (CStepper::GetInstance()->GetLimitMax(axis) - CStepper::GetInstance()->GetLimitMin(axis)) / 2);
			if (axis == Z_AXIS) return CMotionControl::ToMm1000(axis, CStepper::GetInstance()->GetLimitMax(axis));
			break;

			//G59
			// X/2,Y/2
		case 6:
			if (axis == X_AXIS || axis == Y_AXIS) return CMotionControl::ToMm1000(axis, CStepper::GetInstance()->GetLimitMax(axis) - (CStepper::GetInstance()->GetLimitMax(axis) - CStepper::GetInstance()->GetLimitMin(axis)) / 2);
			break;

	}

	// no preset
	return 0;
}

////////////////////////////////////////////////////////////

void CGCodeParser::Parse()
{
	do
	{
		unsigned char ch = _reader->GetCharToUpper();
		switch (ch)
		{
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
					Error(MESSAGE_GCODE_CommandExpected);		return;
				}
				if (!GCommand(GetUInt8()))
				{
					Error(MESSAGE_GCODE_UnsupportedGCommand);	return;
				}
				break;
			}
			case 'M':
			{
				if (!IsUInt(_reader->GetNextChar()))
				{
					Error(MESSAGE_GCODE_MCodeExpected);		return;
				}
				if (!MCommand(GetUInt8()))
				{
					Info(MESSAGE_GCODE_UnspportedMCodeIgnored); return;
				}
				break;
			}
			case 'S':		// spindle speed
			{
				_reader->GetNextChar();
				SpindleSpeedCommand();
				break;
			}
			case 'T':		// tool select
			{
				_reader->GetNextChar();
				ToolSelectCommand();
				break;
			}
			case '#':
			{
				if (!IsUInt(_reader->GetNextChar()))
				{
					Error(MESSAGE_GCODE_ParamNoExpected);
					return;
				}
				param_t paramNo = GetUInt16();
				if (!_reader->IsError() && _reader->SkipSpaces() != '=')
				{
					Error(MESSAGE_GCODE_EqExpected);	return;
				}
				_reader->GetNextCharSkipScaces();
				if (CheckError()) return;

				if (!SetParamCommand(paramNo))
				{
					SetParamValue(paramNo);
				}
				break;
			}

			default:
				if (!Command(ch))
				{
					if (!LastCommand())
					{
						Error(MESSAGE_GCODE_IllegalCommand);
						return;
					}
				}
				break;
		}

		if (CheckError()) return;

	} while (_reader->GetChar() != 0);
}

////////////////////////////////////////////////////////////

bool CGCodeParser::GCommand(unsigned char gcode)
{
	switch (gcode)
	{
		case 0:		G0001Command(true); return true;
		case 1:		G0001Command(false); return true;
		case 2:		G0203Command(true); return true;
		case 3:		G0203Command(false); return true;
		case 10:	G10Command(); return true;
		case 4:		G04Command(); return true;
		case 17:	G171819Command(X_AXIS, Y_AXIS, Z_AXIS); return true;
		case 18:	G171819Command(Z_AXIS, X_AXIS, Y_AXIS); return true;
		case 19:	G171819Command(Y_AXIS, Z_AXIS, X_AXIS); return true;
		case 20:	G20Command(); return true;
		case 21:	G21Command();  return true;
		case 28:	G28Command(); return true;
		case 31:	G31Command(); return true;
		case 53:	G53Command(); return true;
		case 40:	G40Command(); return true;
		case 41:	G41Command(); return true;
		case 42:	G42Command(); return true;
		case 43:	G43Command(); return true;
		case 52:	InfoNotImplemented(); return true;
		case 54:	G5xCommand(1); return true;
		case 55:	G5xCommand(2); return true;
		case 56:	G5xCommand(3); return true;
		case 57:	G5xCommand(4); return true;
		case 58:	G5xCommand(5); return true;
		case 59:	G5xCommand(6); return true;
		case 61:	G61Command(); return true;
		case 64:	G64Command(); return true;
		case 73:	G73Command(); return true;
		case 81:	G81Command(); return true;
		case 82:	G82Command(); return true;
		case 83:	G83Command(); return true;
		case 90:	G90Command(); return true;
		case 91:	G91Command(); return true;
		case 92:	G92Command(); return true;
		case 98:	G98Command(); return true;
		case 99:	G99Command();  return true;
	}
	return false;
}

////////////////////////////////////////////////////////////

bool CGCodeParser::MCommand(unsigned char mcode)
{
	switch (mcode)
	{
		case 0:	M00Command(); return true;
		case 1:	M01Command(); return true;
		case 2:	M02Command(); return true;
		case 3:	M03Command(); return true;
		case 4:	M04Command(); return true;
		case 5: M05Command(); return true;
		case 6: M06Command(); return true;
		case 7: M07Command(); return true;
		case 8: M08Command(); return true;
		case 9: M09Command(); return true;
		case 110:M110Command(); return true;
	}
	return false;
}

////////////////////////////////////////////////////////////

void CGCodeParser::ToolSelectCommand()
{
	_reader->SkipSpaces();
	toolnr_t tool = GetUInt16();
	if (IsError()) return;

#ifndef REDUCED_SIZE

	if (!CGCodeTools::GetInstance()->IsValidTool(tool))
	{
		Info(MESSAGE_GCODE_NoValidTool);
	}

	_modalstate.ToolSelected = tool;

#endif
}

void CGCodeParser::SpindleSpeedCommand()
{
	_reader->SkipSpaces();
	unsigned short speed = GetUInt16();
	if (IsError()) return;

	if (speed > MAXSPINDEL_SPEED)
	{
		Info(MESSAGE_GCODE_SpindleSpeedExceeded);
		speed = MAXSPINDEL_SPEED;
	}

	_modalstate.SpindleSpeed = speed;
}

////////////////////////////////////////////////////////////

mm1000_t CGCodeParser::ParseCoordinate(axis_t axis, mm1000_t relpos, EnumAsByte(EAxisPosType) posType)
{
	switch (posType)
	{
		case AbsolutWithZeroShiftPosition:	return ParseCoordinate() + GetG92PosPreset(axis) + GetG54PosPreset(axis) + GetToolHeightPosPreset(axis);
		case AbsolutPosition:				return ParseCoordinate(); break;
		case RelativPosition:				return relpos + ParseCoordinate();;
	}
	return 0;
}

////////////////////////////////////////////////////////////

bool CGCodeParser::CheckAxisSpecified(axis_t axis, unsigned char& axes)
{
	if (axis >= NUM_AXIS)
	{
		Error(MESSAGE_GCODE_AxisNotSupported);
		return false;
	}

	if (IsBitSet(axes, axis))
	{
		Error(MESSAGE_GCODE_AxisAlreadySpecified);
		return false;
	}

	axes += 1 << axis;

	return true;
}

////////////////////////////////////////////////////////////

axis_t CGCodeParser::CharToAxis(char axis)
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

axis_t CGCodeParser::CharToAxisOffset(char axis)
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

void CGCodeParser::GetUint8(unsigned char& value, unsigned char&specified, unsigned char bit)
{
	if (IsBitSet(specified, bit))
	{
		Error(MESSAGE_GCODE_ParameterSpecifiedMoreThanOnce);
		return;
	}

	BitSet(specified, bit);

	_reader->GetNextChar();
	value = GetUInt8();
}

////////////////////////////////////////////////////////////

void CGCodeParser::GetAxis(axis_t axis, SAxisMove& move, EnumAsByte(EAxisPosType) posType)
{
	if (!CheckAxisSpecified(axis, move.axes))
		return;

	_reader->GetNextChar();

	move.newpos[axis] = ParseCoordinate(axis, move.newpos[axis], posType);
}

////////////////////////////////////////////////////////////

void CGCodeParser::GetIJK(axis_t axis, SAxisMove& move, mm1000_t offset[2])
{
	if (!CheckAxisSpecified(axis, move.bitfield.all))
		return;

	_reader->GetNextChar();

	if (axis == _modalstate.Plane_axis_0)
		offset[0] = ParseCoordinate();
	else if (axis == _modalstate.Plane_axis_1)
		offset[1] = ParseCoordinate();
	else
	{
		Error(MESSAGE_GCODE_AxisOffsetMustNotBeSpecified);
	}
}

////////////////////////////////////////////////////////////

void CGCodeParser::GetRadius(SAxisMove& move, mm1000_t& radius)
{
	if (move.bitfield.bit.R)
	{
		Error(MESSAGE_GCODE_RalreadySpecified);
		return;
	}
	move.bitfield.bit.R = true;

	_reader->GetNextChar();
	radius = ParseCoordinate();
}

////////////////////////////////////////////////////////////

void CGCodeParser::GetR81(SAxisMove& move)
{
	if (move.bitfield.bit.R)
	{
		Error(MESSAGE_GCODE_RalreadySpecified);
		return;
	}
	move.bitfield.bit.R = true;

	_reader->GetNextChar();
	_modalstate.G8xR = ParseCoordinate(_modalstate.Plane_axis_2, CMotionControl::GetPosition(_modalstate.Plane_axis_2), _modalstate.IsAbsolut ? AbsolutWithZeroShiftPosition : RelativPosition);
}

////////////////////////////////////////////////////////////

void CGCodeParser::GetP81(SAxisMove& move)
{
	if (move.bitfield.bit.P)
	{
		Error(MESSAGE_GCODE_PalreadySpecified);
		return;
	}
	move.bitfield.bit.P = true;

	_reader->GetNextChar();
	_modalstate.G8xP = GetUint32OrParam();
}

////////////////////////////////////////////////////////////

void CGCodeParser::GetL81(SAxisMove& move, unsigned char& l)
{
	if (move.bitfield.bit.L)
	{
		Error(MESSAGE_GCODE_LalreadySpecified);
		return;
	}
	move.bitfield.bit.L = true;

	_reader->GetNextChar();
	unsigned long myL = GetUint32OrParam();

	if (myL == 0 || myL > 255)
	{
		Error(MESSAGE_GCODE_LmustBe1_255);
		return;
	}
	l = (unsigned char)myL;
}

////////////////////////////////////////////////////////////

void CGCodeParser::GetQ81(SAxisMove& move)
{
	if (move.bitfield.bit.Q)
	{
		Error(MESSAGE_GCODE_QalreadySpecified);
		return;
	}
	move.bitfield.bit.Q = true;

	_reader->GetNextChar();
	mm1000_t q = ParseCoordinate(_modalstate.Plane_axis_2, 0, AbsolutPosition);

	if (q <= 0)
	{
		Error(MESSAGE_GCODE_QmustBeAPositivNumber);
		return;
	}

	_modalstate.G8xQ = q;
}

////////////////////////////////////////////////////////////

void CGCodeParser::GetFeedrate(SAxisMove& move)
{
	_reader->GetNextChar();

	if (move.bitfield.bit.F)
	{
		Error(MESSAGE_GCODE_FalreadySpecified);
		return;
	}
	move.bitfield.bit.F = true;

	if (!_modalstate.FeedRatePerUnit) { ErrorNotImplemented(); return; }

	feedrate_t feedrate = GetInt32Scale(FEEDRATE_MIN, FEEDRATE_MAX, FEEDRATE_SCALE, FEEDRATE_MAXSCALE);
	// feedrate is 1/1000mm/min (scale 3) 

	if (!_modalstate.UnitisMm)
		feedrate = MulDivI32(feedrate, 254, 10);

	if (CheckError()) { return; }

	if (feedrate < FEEDRATE_MIN_ALLOWED) feedrate = FEEDRATE_MIN_ALLOWED;
	if (feedrate > FEEDRATE_MAX_ALLOWED) feedrate = FEEDRATE_MAX_ALLOWED;

	SetG1FeedRate(feedrate);
}

////////////////////////////////////////////////////////////

void CGCodeParser::GetG92Axis(axis_t axis, unsigned char& axes)
{
	if (!CheckAxisSpecified(axis, axes))
		return;

	_reader->GetNextChar();
	_modalstate.G92Pospreset[axis] = ParseCoordinate() + CMotionControl::GetPosition(axis) - GetG54PosPreset(axis) - GetToolHeightPosPreset(axis);
}

////////////////////////////////////////////////////////////

bool CGCodeParser::LastCommand()
{
	const char* old = _reader->GetBuffer();

	switch (_modalstate.LastCommand)
	{
		case SModalState::LastG00: G0001Command(true); break;
		case SModalState::LastG01: G0001Command(false); break;
		case SModalState::LastG02: G0203Command(true); break;
		case SModalState::LastG03: G0203Command(true);  break;
		case SModalState::LastG73: G73Command();  break;
		case SModalState::LastG81: G81Command();  break;
		case SModalState::LastG82: G82Command(); break;
		case SModalState::LastG83: G83Command(); break;
	}

	if (old == _reader->GetBuffer())
	{
		return false;	// ERROR
	}
	return true;
}

////////////////////////////////////////////////////////////

void CGCodeParser::G0001Command(bool isG00)
{
	_modalstate.LastCommand = isG00 ? SModalState::LastG00 : SModalState::LastG01;

	SAxisMove move(true);

	for (char ch = _reader->SkipSpacesToUpper(); ch; ch = _reader->SkipSpacesToUpper())
	{
		axis_t axis;
		if ((axis = CharToAxis(ch)) < NUM_AXIS) GetAxis(axis, move, _modalstate.IsAbsolut ? AbsolutWithZeroShiftPosition : RelativPosition);
		else if (ch == 'F' && isG00) { Error(MESSAGE_GCODE_FeedrateWithG0); return; }
		else if (ch == 'F' && !isG00) GetFeedrate(move);
		else break;

		if (CheckError()) { return; }
	}

	if (move.axes)
	{
		CMotionControl::MoveAbs(move.newpos, isG00 ? _modalstate.G0FeedRate : _modalstate.G1FeedRate);
		if (!_modalstate.ConstantVelocity)
		{
			Delay(0);
		}
	}
}

////////////////////////////////////////////////////////////

void CGCodeParser::G0203Command(bool isG02)
{
	_modalstate.LastCommand = isG02 ? SModalState::LastG02 : SModalState::LastG03;

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

	if (move.bitfield.bit.R && move.GetIJK())		{ Error(MESSAGE_GCODE_IJKandRspecified); return; }
	if (!move.bitfield.bit.R && !move.GetIJK())		{ Error(MESSAGE_GCODE_MissingIKJorR); return; }
	if (CheckError()) { return; }

	if (move.bitfield.bit.R)
	{
		// Calculate the change in position along each selected axis
		float x = (float)(move.newpos[_modalstate.Plane_axis_0] - CMotionControl::GetPosition(_modalstate.Plane_axis_0));
		float y = (float)(move.newpos[_modalstate.Plane_axis_1] - CMotionControl::GetPosition(_modalstate.Plane_axis_1));
		float r = (float)radius;

		if (x == 0.0 && y == 0.0)						{ Error(MESSAGE_GCODE_360withRandMissingAxes); return; }

		// First, use h_x2_div_d to compute 4*h^2 to check if it is negative or r is smaller
		// than d. If so, the sqrt of a negative number is complex and error out.
		float h_x2_div_d = 4 * r*r - x*x - y*y;
		if (h_x2_div_d < 0)								{ Error(MESSAGE_GCODE_STATUS_ARC_RADIUS_ERROR); return; }

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

	CMotionControl::Arc(move.newpos, offset[0], offset[1], _modalstate.Plane_axis_0, _modalstate.Plane_axis_1, isG02, _modalstate.G1FeedRate);

	if (!_modalstate.ConstantVelocity)
	{
		Delay(0);
	}
}

////////////////////////////////////////////////////////////

void CGCodeParser::G04Command()
{
	unsigned long dweelms = 0;

	if (_reader->SkipSpacesToUpper() == 'P')
	{
		_reader->GetNextChar();
		const char*current = _reader->GetBuffer();
		dweelms = GetUint32OrParam();
		if (_reader->GetChar() == '.')
		{
			// this is "sec" and not "ms"
			_reader->ResetBuffer(current);
			dweelms = GetInt32Scale(0,1000,3,255);
		}
	}

	if (ExpectEndOfCommand())		{ return; }

	Delay(dweelms);
}

////////////////////////////////////////////////////////////

void CGCodeParser::G10Command()
{
	unsigned char specified = 0;
	unsigned char l=0;
	unsigned char p=0;
	SAxisMove move(false);

	for (char ch = _reader->SkipSpacesToUpper(); ch; ch = _reader->SkipSpacesToUpper())
	{
		axis_t axis;
		if ((axis = CharToAxis(ch)) < NUM_AXIS) GetAxis(axis, move, AbsolutPosition);
		else if (ch == 'L')					  GetUint8(l, specified, 0);
		else if (ch == 'P')					  GetUint8(p, specified, 1);
		else break;

		if (CheckError()) { return; }
	}

	if (IsBitClear(specified, 0))		{ Error(MESSAGE_GCODE_LExpected); return; }
	if (IsBitClear(specified, 1))		{ Error(MESSAGE_GCODE_PExpected); return; }

	switch (l)
	{
		default: Error(MESSAGE_GCODE_UnsupportedLvalue); return;
		case 2:
		{
			if (p == 0) { p = _modalstate.ZeroPresetIdx; }		// current
			if (p > 1)  { Error(MESSAGE_GCODE_UnsupportedCoordinateSystemUseG54Instead); return; }

			for (unsigned char axis = 0; axis < NUM_AXIS; axis++)
			{
				if (IsBitSet(move.axes, axis))
				{
					_modalstate.G54Pospreset[axis] = move.newpos[axis];
				}
			}
			break;
		}
	}
}

////////////////////////////////////////////////////////////

void CGCodeParser::G171819Command(axis_t axis0, axis_t axis1, axis_t axis2)
{
	_modalstate.Plane_axis_0 = axis0;
	_modalstate.Plane_axis_1 = axis1;
	_modalstate.Plane_axis_2 = axis2;
}

////////////////////////////////////////////////////////////

void CGCodeParser::G28Command()
{
	SAxisMove move(false);

	for (char ch = _reader->SkipSpacesToUpper(); ch; ch = _reader->SkipSpacesToUpper())
	{
		axis_t axis;
		if ((axis = CharToAxis(ch)) < NUM_AXIS) GetAxis(axis, move, AbsolutPosition);
		else break;

		if (CheckError()) { return; }
	}

	if (ExpectEndOfCommand())		{ return; }

	if (move.axes == 0)
	{
		CControl::GetInstance()->GoToReference();
	}
	else
	{
		for (unsigned char axis = 0; axis < NUM_AXIS; axis++)
		{
			if (IsBitSet(move.axes, axis))
			{
				CControl::GetInstance()->GoToReference(axis);
			}
		}
	}
}

////////////////////////////////////////////////////////////

bool CGCodeParser::G31TestProbe(void* /* param */)
{
	// return true if probe is not on
	// => continue move to probe position
	// return false if probe is on (probe switch is pressed)

	return CControl::GetInstance()->IOControl(CControl::Probe) == 0;
}

void CGCodeParser::G31Command()
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
		Error(MESSAGE_GCODE_NoAxesForProbe);
		return;
	}

	if ((move.axes&7) == 0)
	{
		Error(MESSAGE_GCODE_ProbeOnlyForXYZ);
		return;
	}

	{
		if (!G31TestProbe(NULL))
		{
			Error(MESSAGE_GCODE_ProbeIOmustBeOff);
			return;
		}

		CMotionControl::MoveAbs(move.newpos, _modalstate.G1FeedRate);

		if (!CStepper::GetInstance()->MoveUntil(G31TestProbe, NULL))
		{
			Error(MESSAGE_GCODE_ProbeFailed);
			return;
		}
	}
}

////////////////////////////////////////////////////////////

void CGCodeParser::G41Command()
{
	/*
		if (_reader->SkipSpacesToUpper() == 'D')
		{
		_reader->GetNextChar();
		GetUint16OrParam();			// ignore it
		}
		*/
	ErrorNotImplemented();
}

////////////////////////////////////////////////////////////

void CGCodeParser::G42Command()
{
	/*
	if (_reader->SkipSpacesToUpper() == 'D')
	{
	_reader->GetNextChar();
	GetUint16OrParam();			// ignore it
	}
	*/
	ErrorNotImplemented();
}

////////////////////////////////////////////////////////////

void CGCodeParser::G43Command()
{
	if (_reader->SkipSpacesToUpper() == 'H')
	{
		_reader->GetNextChar();
		toolnr_t tool = GetUint16OrParam();
		if (IsError()) return;

#ifndef REDUCED_SIZE
		if (!CGCodeTools::GetInstance()->IsValidTool(tool))
		{
			Error(MESSAGE_GCODE_NoValidTool); return;
		}
		_modalstate.ToolHeigtCompensation = CGCodeTools::GetInstance()->GetHeight(tool);
#endif
	}
	else
	{
		_modalstate.ToolHeigtCompensation = 0;
	}
}

////////////////////////////////////////////////////////////

void CGCodeParser::G53Command()
{
	_modlessstate.ZeroPresetIdx = 0;
}

////////////////////////////////////////////////////////////

void CGCodeParser::G5xCommand(unsigned char idx)
{
	if (CutterRadiosIsOn()) return;

	_modlessstate.ZeroPresetIdx =
		_modalstate.ZeroPresetIdx = idx;
}

////////////////////////////////////////////////////////////

void CGCodeParser::G8xCommand(SAxisMove& move, bool useP, bool useQ, bool useMinQ)
{
	if (CutterRadiosIsOn()) return;

	unsigned char l = 1;

	for (char ch = _reader->SkipSpacesToUpper(); ch; ch = _reader->SkipSpacesToUpper())
	{
		axis_t axis;
		if ((axis = CharToAxis(ch)) <= Z_AXIS)				GetAxis(axis, move, _modalstate.IsAbsolut ? AbsolutWithZeroShiftPosition : RelativPosition);
		else if (ch == 'R')									GetR81(move);
		else if (ch == 'L')									GetL81(move, l);
		else if (ch == 'F')									GetFeedrate(move);
		else if (ch == 'P' && useP)							GetP81(move);
		else if (ch == 'Q' && useQ)							GetQ81(move);
		else break;

		if (CheckError()) { return; }
	}

	if (!CheckError() && move.axes)
	{
		if (useQ && _modalstate.G8xQ == 0)
		{
			Error(MESSAGE_GCODE_QmustNotBe0);
			return;
		}

		if (IsBitSet(move.axes, _modalstate.Plane_axis_2))
		{
			_modalstate.G8xPlane2 = move.newpos[_modalstate.Plane_axis_2];
		}

		mm1000_t pos[NUM_AXIS];
		CMotionControl::GetPositions(pos);

		mm1000_t origPlane2 = pos[_modalstate.Plane_axis_2];

		bool drillDown = origPlane2 > _modalstate.G8xPlane2;

		//		// r : z(now) <= r <= z(down)
		if ((drillDown && (origPlane2 < _modalstate.G8xR || _modalstate.G8xPlane2 > _modalstate.G8xR)) ||
			(!drillDown && (origPlane2 > _modalstate.G8xR || _modalstate.G8xPlane2 < _modalstate.G8xR)))
		{
			Error(MESSAGE_GCODE_RmustBeBetweenCurrentRZ);
			return;
		}

		for (unsigned char i = 0; i < l; i++)
		{
			// 1. Step: GoTo x:y (fast)
			//          For rel move store relative distance in move.newpos
			if (_modalstate.IsAbsolut || i == 0)
			{
				pos[_modalstate.Plane_axis_0] = move.newpos[_modalstate.Plane_axis_0];
				pos[_modalstate.Plane_axis_1] = move.newpos[_modalstate.Plane_axis_1];
				if (!_modalstate.IsAbsolut)
				{
					move.newpos[_modalstate.Plane_axis_0] -= CMotionControl::GetPosition(_modalstate.Plane_axis_0);
					move.newpos[_modalstate.Plane_axis_1] -= CMotionControl::GetPosition(_modalstate.Plane_axis_1);
				}
			}
			else
			{
				pos[_modalstate.Plane_axis_0] += move.newpos[_modalstate.Plane_axis_0];
				pos[_modalstate.Plane_axis_1] += move.newpos[_modalstate.Plane_axis_1];
			}

			CMotionControl::MoveAbs(pos, _modalstate.G0FeedRate);
			if (CheckError()) { return; }

			// 2. Step: GoTo z(R) (fast)
			pos[_modalstate.Plane_axis_2] = _modalstate.G8xR;
			CMotionControl::MoveAbs(pos, _modalstate.G0FeedRate);
			if (CheckError()) { return; }

			mm1000_t nextPlan2 = _modalstate.G8xR;
			bool finalMove = false;

			while (!finalMove)
			{
				if (useQ)
				{
					if (drillDown)
					{
						nextPlan2 -= _modalstate.G8xQ;
						finalMove = _modalstate.G8xPlane2 >= nextPlan2;
					}
					else
					{
						nextPlan2 += _modalstate.G8xQ;
						finalMove = _modalstate.G8xPlane2 <= nextPlan2;
					}
				}
				else
				{
					finalMove = true;
				}

				if (finalMove)
				{
					nextPlan2 = _modalstate.G8xPlane2;
				}

				// 3. Step: Goto Z (with feedrate)
				pos[_modalstate.Plane_axis_2] = nextPlan2;
				CMotionControl::MoveAbs(pos, _modalstate.G1FeedRate);
				if (CheckError()) { return; }

				// 3.a. Step: Wait
				if (useP &&  _modalstate.G8xP != 0)
				{
					Delay(_modalstate.G8xP);
				}

				// 4. Step: Goto init Z or R (fast) see G98
				if (finalMove)
				{
					pos[_modalstate.Plane_axis_2] = (_modalstate.IsG98) ? origPlane2 : _modalstate.G8xR;
				}
				else if (useMinQ)
				{
					pos[_modalstate.Plane_axis_2] = nextPlan2 + (drillDown ? G73RETRACTION : -G73RETRACTION);
				}
				else
				{
					pos[_modalstate.Plane_axis_2] = _modalstate.G8xR;
				}

				CMotionControl::MoveAbs(pos, _modalstate.G0FeedRate);
				if (CheckError()) { return; }
			}
		}
	}
}

////////////////////////////////////////////////////////////

void CGCodeParser::G73Command()
{
	_modalstate.LastCommand = SModalState::LastG73;

	SAxisMove move(true);
	G8xCommand(move, false, true, true);
}

////////////////////////////////////////////////////////////

void CGCodeParser::G81Command()
{
	_modalstate.LastCommand = SModalState::LastG81;

	SAxisMove move(true);
	G8xCommand(move, false, false, false);
}

////////////////////////////////////////////////////////////

void CGCodeParser::G82Command()
{
	_modalstate.LastCommand = SModalState::LastG82;

	SAxisMove move(true);
	G8xCommand(move, true, false, false);
}

////////////////////////////////////////////////////////////

void CGCodeParser::G83Command()
{
	_modalstate.LastCommand = SModalState::LastG83;

	SAxisMove move(true);
	G8xCommand(move, false, true, false);
}

////////////////////////////////////////////////////////////

void CGCodeParser::G91Command()
{
	unsigned char subcode = GetSubCode();

	switch (subcode)
	{
		case 1:		break;	//OK (I,J,K relative) = default
		case 255:	_modalstate.IsAbsolut = false; break;
		default:	ErrorNotImplemented(); break;
	}
}

////////////////////////////////////////////////////////////

void CGCodeParser::G92Command()
{
	unsigned char axes = 0;

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
}

////////////////////////////////////////////////////////////

void CGCodeParser::M00Command()
{
	Delay(0);
	CControl::GetInstance()->Pause();
}

void CGCodeParser::M01Command()
{
	Delay(0);
	CControl::GetInstance()->Pause();
}

void CGCodeParser::M02Command()
{
}

////////////////////////////////////////////////////////////

void CGCodeParser::M03Command()
{
	Delay(0);
	CControl::GetInstance()->IOControl(CControl::Spindel, _modalstate.SpindleSpeed);
	//spindel on CW
}

////////////////////////////////////////////////////////////

void CGCodeParser::M04Command()
{
	Delay(0);
	CControl::GetInstance()->IOControl(CControl::Spindel, -((short) _modalstate.SpindleSpeed));
	//spindel on CCW
}

////////////////////////////////////////////////////////////

void CGCodeParser::M05Command()
{
	//spindel off
	Delay(0);
	CControl::GetInstance()->IOControl(CControl::Spindel, 0);
}

////////////////////////////////////////////////////////////

void CGCodeParser::M06Command()
{
	// ATC (automatic tool change)	
}

////////////////////////////////////////////////////////////

void CGCodeParser::M07Command()
{
	//coolant on
	Delay(0);
	CControl::GetInstance()->IOControl(CControl::Coolant, 1);
}

////////////////////////////////////////////////////////////

void CGCodeParser::M08Command()
{
	//coolant on (flood)
	Delay(0);
	CControl::GetInstance()->IOControl(CControl::Coolant, 2);
}

////////////////////////////////////////////////////////////

void CGCodeParser::M09Command()
{
	//coolant off
	Delay(0);
	CControl::GetInstance()->IOControl(CControl::Coolant, 0);
}

////////////////////////////////////////////////////////////

void CGCodeParser::M110Command()
{
	// set linenumber

	unsigned long linenumber = 0;

	if (_reader->SkipSpacesToUpper() == 'N')
	{
		_reader->GetNextChar();
		linenumber = GetUInt32();
	}

	if (ExpectEndOfCommand())		{ return; }

	_modalstate.Linenumber = linenumber;
}

