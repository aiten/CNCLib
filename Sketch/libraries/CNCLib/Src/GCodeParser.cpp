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
#include "MotionControl.h"
#include "ExpressionParser.h"

#include "GCodeParser.h"
#include "GCodeExpressionParser.h"
#include "DecimalAsInt.h"

////////////////////////////////////////////////////////////

struct CGCodeParser::SModalState CGCodeParser::_modalstate;
struct CGCodeParser::SModlessState CGCodeParser::_modlessstate;

////////////////////////////////////////////////////////////

bool CGCodeParser::SetParamCommand(param_t /* paramNo */)	{ return false; }

////////////////////////////////////////////////////////////

bool CGCodeParser::InitParse()
{
	if (!super::InitParse())
		return false;

	_modlessstate.Init();
	return true;				// continue
}

////////////////////////////////////////////////////////////

void CGCodeParser::CleanupParse()
{
	_modlessstate.Init();		// state for no command
	super::CleanupParse();
}

////////////////////////////////////////////////////////////

void CGCodeParser::CommentMessage(char* start)
{
	bool isMsg = TryToken(start, F("(MSG,"), false, true);
	if (isMsg)
	{
		start += 5;
		while (start+1 < _reader->GetBuffer())
			StepperSerial.print(*(start++));
		StepperSerial.println();
	}
}

////////////////////////////////////////////////////////////

param_t CGCodeParser::ParseParamNo()
{
	if (_reader->SkipSpacesToUpper() == '<')		// named parameter
	{
		// format: <_varname : axis >
		// e.g.    <_home:x>
		_reader->GetNextChar();
		char ch = _reader->SkipSpacesToUpper();
		const char* start = _reader->GetBuffer();
		const char* colon = NULL;

		if (!CStreamReader::IsAlpha(ch))
		{
			Error(MESSAGE_GCODE_VaribaleMustStartWithAlpha); return 0;
		}

		while (CStreamReader::IsAlpha(ch) || CStreamReader::IsDigit(ch) || ch==':')
		{
			if (ch == ':')
			{
				if (colon != NULL)
				{
					Error(MESSAGE_GCODE_NoValidVaribaleName); return 0;
				}
				colon = _reader->GetBuffer();
			}
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
		axis_t a=X_AXIS;

		if (colon != NULL)
		{
			if (colon[2] != 0 || (a = CharToAxis(CStreamReader::Toupper(colon[1]))) >= NUM_AXIS)
			{
				Error(MESSAGE_GCODE_NoValidVaribaleName); return 0;
			}
		}

		if (start[0] == '_')			// all system parameter start with _
		{
			// see: http://www.linuxcnc.org/docs/devel/html/gcode/overview.html#_predefined_named_parameters_a_id_sec_predefined_named_parameters_a
			if (start[2] == 0 && (a = CharToAxis(CStreamReader::Toupper(start[1]))) < NUM_AXIS)
			{
				return PARAMSTART_CURRENTPOS + a;
			}
		}

		CStreamReader::CSetTemporary terminatecolon(colon ? colon : end);

		const SParamInfo* param = FindParamInfoByText(start);

		if (param != NULL)
		{
			if (pgm_read_byte(&param->_allowaxisofs)==0 && colon != NULL)
			{
				Error(MESSAGE_GCODE_NoValidVaribaleName); return 0;
			}
			return pgm_read_word(&param->_paramNo) + a;
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

mm1000_t CGCodeParser::ParseParameter(bool convertToInch)
{
	param_t paramNo = ParseParamNo();
	if (paramNo)
		return GetParamValue(paramNo, convertToInch);

	Error(MESSAGE_GCODE_ParameterNotFound);
	return 0;
}

////////////////////////////////////////////////////////////

static bool IsModifyParam(param_t paramNo)								{ return paramNo >= 1 && paramNo <= NUM_PARAMETER; }

// 5161-5169 - G28 Home for (X Y Z A B C U V W)
// 5221-5230 - Coordinate System 1, G54 (X Y Z A B C U V W R) - R denotes the XY rotation angle around the Z axis 
// 5420-5428 - Current Position including offsets in current program units (X Y Z A B C U V W)
// customized extension

mm1000_t CGCodeParser::GetParamValue(param_t paramNo, bool convertUnits)
{
	if (IsModifyParam(paramNo))
	{
		if (convertUnits)
			return CMm1000::ConvertFrom(25.4f*_modalstate.Parameter[paramNo - 1]);

		return CMm1000::ConvertFrom(_modalstate.Parameter[paramNo - 1]);
	}

	const SParamInfo*param = FindParamInfoByParamNo(paramNo);

	if (param != NULL)
	{
		axis_t axis = (axis_t) (paramNo - param->_paramNo);
		switch (pgm_read_word(&param->_paramNo))
		{
			case PARAMSTART_G28HOME:
			{
				mm1000_t pos = CStepper::GetInstance()->GetLimitMin(axis);
				if (CStepper::GetInstance()->IsUseReference(CStepper::GetInstance()->ToReferenceId(axis, false)))	// max refmove
					pos = CStepper::GetInstance()->GetLimitMax(axis);
				return GetParamAsPosition(pos, axis);
			}
			case PARAMSTART_G92OFFSET:			return GetParamAsPosition(super::_modalstate.G92Pospreset[axis], axis);
			case PARAMSTART_CURRENTPOS:			return GetParamAsPosition(GetRelativePosition(axis), axis);
			case PARAMSTART_CURRENTABSPOS:		return GetParamAsPosition(CMotionControlBase::GetInstance()->GetPosition(axis), axis);
			case PARAMSTART_BACKLASH:			return GetParamAsPosition(CStepper::GetInstance()->GetBacklash(axis), axis);
			case PARAMSTART_MAX:				return GetParamAsPosition(CStepper::GetInstance()->GetLimitMax(axis), axis);
			case PARAMSTART_MIN:				return GetParamAsPosition(CStepper::GetInstance()->GetLimitMin(axis), axis);
			case PARAMSTART_ACC:				return CStepper::GetInstance()->GetAcc(axis);
			case PARAMSTART_DEC:				return CStepper::GetInstance()->GetDec(axis);
			case PARAMSTART_JERK:				return CStepper::GetInstance()->GetJerkSpeed(axis);

			case PARAMSTART_G54OFFSET + 0 * PARAMSTART_G54FF_OFFSET:
			case PARAMSTART_G54OFFSET + 1 * PARAMSTART_G54FF_OFFSET:
			case PARAMSTART_G54OFFSET + 2 * PARAMSTART_G54FF_OFFSET:
			case PARAMSTART_G54OFFSET + 3 * PARAMSTART_G54FF_OFFSET:
			case PARAMSTART_G54OFFSET + 4 * PARAMSTART_G54FF_OFFSET:
			case PARAMSTART_G54OFFSET + 5 * PARAMSTART_G54FF_OFFSET:
			{
				unsigned char idx = (unsigned char)((pgm_read_word(&param->_paramNo) - PARAMSTART_G54OFFSET) / PARAMSTART_G54FF_OFFSET);
				if (idx < G54ARRAYSIZE)
					return GetParamAsPosition(_modalstate.G54Pospreset[idx][axis], axis);
				break;
			}
			case PARAMSTART_FEEDRATE:			return GetG1FeedRate();
		}
	}

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
		mm1000_t mm1000 = CMm1000::ConvertFrom(exprpars.Answer);
		const SParamInfo*param = FindParamInfoByParamNo(paramNo);

		if (IsModifyParam(paramNo))				{ _modalstate.Parameter[paramNo - 1] = exprpars.Answer; }
		else if (param != NULL)
		{
			axis_t axis = (axis_t)(paramNo - param->_paramNo);
			switch (pgm_read_word(&param->_paramNo))
			{
				case PARAMSTART_BACKLASH:			{ CStepper::GetInstance()->SetBacklash(axis, (mdist_t)GetParamAsMachine(mm1000, axis));	break;  }
				case PARAMSTART_BACKLASH_FEEDRATE:	{ CStepper::GetInstance()->SetBacklash((steprate_t)CMotionControlBase::GetInstance()->ToMachine(0, mm1000 * 60)); break; }
				case PARAMSTART_CONTROLLERFAN:		{ CControl::GetInstance()->IOControl(CControl::ControllerFan, (unsigned short)exprpars.Answer);	break;  }
				case PARAMSTART_RAPIDMOVEFEED:		{ SetG0FeedRate(-CFeedrate1000::ConvertFrom(exprpars.Answer)); break;	}
				case PARAMSTART_MAX:				{ CStepper::GetInstance()->SetLimitMax(axis, GetParamAsMachine(mm1000, axis));	break;	}
				case PARAMSTART_MIN:				{ CStepper::GetInstance()->SetLimitMin(axis, GetParamAsMachine(mm1000, axis));	break;	}
				case PARAMSTART_ACC:				{ CStepper::GetInstance()->SetAcc(axis, (steprate_t)mm1000); break;	}
				case PARAMSTART_DEC:				{ CStepper::GetInstance()->SetDec(axis, (steprate_t)mm1000); break;	}
				case PARAMSTART_JERK:				{ CStepper::GetInstance()->SetJerkSpeed(axis, (steprate_t)mm1000); break; }
				default:							Error(MESSAGE_GCODE_UnspportedParameterNumber);	return;
			}
		}
		else
		{
			Error(MESSAGE_GCODE_UnspportedParameterNumber);	return;
		}

		// rest of line only comment allowed!
		ExpectEndOfCommand();
	}
}
////////////////////////////////////////////////////////////

const CGCodeParser::SParamInfo* CGCodeParser::FindParamInfo(const void*param, bool(*check)(const SParamInfo*, const void*param))
{
	const SParamInfo* item = &_paramdef[0];
	while (item->_paramNo != 0)
	{
		if (check(item, param)) return item;
		item++;
	}

	return NULL;
}

////////////////////////////////////////////////////////////

const CGCodeParser::SParamInfo* CGCodeParser::FindParamInfoByText(const char* text)
{
	return FindParamInfo(text, [](const SParamInfo* p, const void*x) -> bool
	{
		const char* text = p->GetText();
		return text != NULL && strcasecmp_P((const char*)x, text) == 0;
	});
}

////////////////////////////////////////////////////////////

const CGCodeParser::SParamInfo* CGCodeParser::FindParamInfoByParamNo(param_t paramNo)
{
	return FindParamInfo((const void*)paramNo, [](const SParamInfo* p, const void*x) -> bool
	{
		param_t pramamNo = (param_t)x;
		return p->_paramNo == pramamNo ||	// exact same paramno
			(p->_allowaxisofs && p->_paramNo <= pramamNo && p->_paramNo + NUM_AXIS > pramamNo);	// diff with axis
	});
}

////////////////////////////////////////////////////////////

static const char _feedrate[] PROGMEM = "_feedrate";
static const char _g28home[] PROGMEM = "_g92home";
static const char _g54home[] PROGMEM = "_g54home";
static const char _g55home[] PROGMEM = "_g55home";
static const char _g56home[] PROGMEM = "_g56home";
static const char _g57home[] PROGMEM = "_g57home";
static const char _g58home[] PROGMEM = "_g58home";
static const char _g59home[] PROGMEM = "_g59home";

const CGCodeParser::SParamInfo CGCodeParser::_paramdef[] PROGMEM =
{
	{ PARAMSTART_G28HOME,	NULL,			true },
	{ PARAMSTART_G92OFFSET,	_g28home,		true },
	{ PARAMSTART_CURRENTPOS,	NULL,		true },
	{ PARAMSTART_CURRENTABSPOS,	NULL,		true },
	{ PARAMSTART_BACKLASH,	NULL,			true },
	{ PARAMSTART_BACKLASH_FEEDRATE,	NULL,	false },
	{ PARAMSTART_MAX,	NULL,				true },
	{ PARAMSTART_MIN,	NULL,				true },
	{ PARAMSTART_ACC,	NULL,				true },
	{ PARAMSTART_DEC,	NULL,				true },
	{ PARAMSTART_JERK,	NULL,				true },
	{ PARAMSTART_CONTROLLERFAN,	NULL,		false },
	{ PARAMSTART_RAPIDMOVEFEED,	NULL,		false },

	{ PARAMSTART_G54OFFSET + 0 * PARAMSTART_G54FF_OFFSET,	_g54home,	true },
	{ PARAMSTART_G54OFFSET + 1 * PARAMSTART_G54FF_OFFSET,	_g55home,	true },
	{ PARAMSTART_G54OFFSET + 2 * PARAMSTART_G54FF_OFFSET,	_g56home,	true },
	{ PARAMSTART_G54OFFSET + 3 * PARAMSTART_G54FF_OFFSET,	_g57home,	true },
	{ PARAMSTART_G54OFFSET + 4 * PARAMSTART_G54FF_OFFSET,	_g58home,	true },
	{ PARAMSTART_G54OFFSET + 5 * PARAMSTART_G54FF_OFFSET,	_g59home,	true },

	{ PARAMSTART_FEEDRATE,	_feedrate, false },
	{ 0,NULL,false }
};

////////////////////////////////////////////////////////////

mm1000_t CGCodeParser::CalcAllPreset(axis_t axis)			
{ 
	return GetG54PosPreset(axis) + (IsG53Present() ? 0 : super::GetG92PosPreset(axis) + GetToolHeightPosPreset(axis));
}

////////////////////////////////////////////////////////////

mm1000_t CGCodeParser::GetG54PosPreset(axis_t axis)
{
	if (_modlessstate.ZeroPresetIdx > 0)
	{
		return _modalstate.G54Pospreset[_modlessstate.ZeroPresetIdx - 1][axis];
	}
	// no preset
	return 0;
}

////////////////////////////////////////////////////////////

bool CGCodeParser::Command(unsigned char ch)
{
	if (super::Command(ch))
		return true;

	switch (ch)
	{
		case 'S':		// spindle speed
		{
			_reader->GetNextChar();
			SpindleSpeedCommand();
			return true;
		}
		case 'T':		// tool select
		{
			_reader->GetNextChar();
			ToolSelectCommand();
			return true;
		}
		case '#':
		{
			if (!IsUInt(_reader->GetNextChar()))
			{
				Error(MESSAGE_GCODE_ParamNoExpected);
				return true;
			}
			param_t paramNo = GetUInt16();
			if (!_reader->IsError() && _reader->SkipSpaces() != '=')
			{
				Error(MESSAGE_GCODE_EqExpected);	return true;
			}
			_reader->GetNextCharSkipScaces();
			if (CheckError()) return true;

			if (!SetParamCommand(paramNo))
			{
				SetParamValue(paramNo);
			}
			return true;
		}

		case '!':
		case '-':
		case '?':
		case '$': CommandEscape(); return true;
	}
	return false;
}

////////////////////////////////////////////////////////////

bool CGCodeParser::GCommand(unsigned char gcode)
{
	if (super::GCommand(gcode))
		return true;

	switch (gcode)
	{
		case 10:	G10Command(); return true;
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
		case 68:	G68Command(); return true;
		case 69:	G69Command(); return true;
		case 81:	G81Command(); return true;
		case 82:	G82Command(); return true;
		case 83:	G83Command(); return true;
		case 98:	G98Command(); return true;
		case 99:	G99Command();  return true;
	}
	return false;
}

////////////////////////////////////////////////////////////

bool CGCodeParser::MCommand(mcode_t mcode)
{
	if (super::MCommand(mcode))
		return true;

	switch (mcode)
	{
		case 0:	M00Command(); return true;
		case 1:	M01Command(); return true;
		case 2:	M02Command(); return true;
		case 6: M06Command(); return true;
		case 8: M08Command(); return true;
		case 10: M10Command(); return true;
		case 11: M11Command(); return true;
		case 110: M110Command(); return true;
		case 111: M111Command(); return true;
		case 114: M114Command(); return true;
		case 220: M220Command(); return true;
#ifndef REDUCED_SIZE
		case 300: M300Command(); return true;
#endif
	}
	return false;
}

////////////////////////////////////////////////////////////

void CGCodeParser::ToolSelectCommand()
{
	_reader->SkipSpaces();
	toolnr_t tool = GetUInt16();
	if (IsError()) return;

	if (!CGCodeTools::GetInstance()->IsValidTool(tool))
	{
		Info(MESSAGE_GCODE_NoValidTool);
	}

	_modalstate.ToolSelected = tool;
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
	_modalstate.G8xR = ParseCoordinate(super::_modalstate.Plane_axis_2, CMotionControlBase::GetInstance()->GetPosition(super::_modalstate.Plane_axis_2), super::_modalstate.IsAbsolut ? AbsolutWithZeroShiftPosition : RelativPosition);
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
	_modalstate.G8xP = GetDweel();
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
	mm1000_t q = ParseCoordinate(super::_modalstate.Plane_axis_2, 0, AbsolutPosition);

	if (q <= 0)
	{
		Error(MESSAGE_GCODE_QmustBeAPositivNumber);
		return;
	}

	_modalstate.G8xQ = q;
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
			if (p > G54ARRAYSIZE)  { Error(MESSAGE_GCODE_UnsupportedCoordinateSystemUseG54Instead); return; }

			for (unsigned char axis = 0; axis < NUM_AXIS; axis++)
			{
				if (IsBitSet(move.axes, axis))
				{
					_modalstate.G54Pospreset[p-1][axis] = move.newpos[axis];
				}
			}
			break;
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

		if (!CGCodeTools::GetInstance()->IsValidTool(tool))
		{
			Error(MESSAGE_GCODE_NoValidTool); return;
		}
		_modalstate.ToolHeigtCompensation = CGCodeTools::GetInstance()->GetHeight(tool);
	}
	else
	{
		_modalstate.ToolHeigtCompensation = 0;
	}
}

////////////////////////////////////////////////////////////

void CGCodeParser::GetG68IJK(axis_t axis, SAxisMove& move, mm1000_t offset[NUM_AXISXYZ])
{
	if (!CheckAxisSpecified(axis, move.bitfield.all))
		return;

	_reader->GetNextChar();

	offset[axis] = ParseCoordinateAxis(axis);
}

////////////////////////////////////////////////////////////

void CGCodeParser::GetAngleR(SAxisMove& move, mm1000_t& angle)
{
	if (move.bitfield.bit.R)
	{
		Error(MESSAGE_GCODE_RalreadySpecified);
		return;
	}
	move.bitfield.bit.R = true;

	_reader->GetNextChar();
	angle = ParseCoordinate(false);
}
////////////////////////////////////////////////////////////

void CGCodeParser::G68Command()
{
	unsigned char subcode = GetSubCode();

	switch (subcode)
	{
		case 10: G68Ext10Command(); break;
		case 11: G68Ext11Command(); break;
		case 12: G68Ext12Command(); break;
		case 13: G68ExtXXCommand(X_AXIS); break;
		case 14: G68ExtXXCommand(Y_AXIS); break;
		case 15: G68ExtXXCommand(Z_AXIS); break;
		case 255: G68CommandDefault(); break;
		default:	ErrorNotImplemented(); break;
	}

	SetPositionAfterG68G69();
}

////////////////////////////////////////////////////////////

void CGCodeParser::G68CommandDefault()
{
	if (CMotionControl::GetInstance()->IsRotate())
	{
		CMotionControl::GetInstance()->ClearRotate();
	}

	SAxisMove move(true);
	mm1000_t r;
	mm1000_t offset[NUM_AXISXYZ] = { 0, 0, 0 };
	mm1000_t vect[NUM_AXISXYZ] = { 0, 0, 0 };

	for (char ch = _reader->SkipSpacesToUpper(); ch; ch = _reader->SkipSpacesToUpper())
	{
		axis_t axis;
		if ((axis = CharToAxis(ch)) < NUM_AXISXYZ)				GetAxis(axis, move, super::_modalstate.IsAbsolut ? AbsolutWithZeroShiftPosition : RelativPosition);
		else if ((axis = CharToAxisOffset(ch)) < NUM_AXISXYZ)	GetG68IJK(axis, move, vect);
		else if (ch == 'R')										GetAngleR(move, r);
		else break;

		if (CheckError()) { return; }
	}

	if (!move.bitfield.bit.R)			{ Error(MESSAGE_GCODE_MissingR); return; }

	memcpy(offset, move.newpos,sizeof(offset));	// use current position!
/*
	for (unsigned char axis = 0; axis < NUM_AXIS; axis++)
	{
		if (IsBitSet(move.axes, axis))
		{
			offset[axis] = move.newpos[axis];
		}
	}
*/

	if (move.GetIJK())
	{
		//3D
		// see vect with GetG67IJK
		if (vect[0] == 0 && vect[1] && vect[2])			{ Error(MESSAGE_GCODE_IJKVECTORIS0); return; }
	}
	else
	{
		//2D
		vect[super::_modalstate.Plane_axis_2] = 1000;
	}

	CMotionControl::GetInstance()->SetRotate(CMm1000::DegreeToRAD(r),vect,offset);
}

////////////////////////////////////////////////////////////

void CGCodeParser::G68Ext10Command()
{
	// Clear (all and set to 0), no iJK, no xyz

	mm1000_t offset[NUM_AXISXYZ] = { 0, 0, 0 };
	for (unsigned char axis = 0; axis < NUM_AXISXYZ; axis++)
		CMotionControl::GetInstance()->SetRotate2D(axis,0.0);
	CMotionControl::GetInstance()->SetOffset2D(offset);
}

////////////////////////////////////////////////////////////

void CGCodeParser::G68Ext11Command()
{
	// Set Offset (x,y,z), no IJK => xyz:position(with g92, g54,...) - default is current, converted to abs position (g92 has no effect to offset)

	SAxisMove move(true);

	for (char ch = _reader->SkipSpacesToUpper(); ch; ch = _reader->SkipSpacesToUpper())
	{
		axis_t axis;
		if ((axis = CharToAxis(ch)) < NUM_AXISXYZ)	GetAxis(axis, move, super::_modalstate.IsAbsolut ? AbsolutWithZeroShiftPosition : RelativPosition);
		else break;

		if (CheckError()) { return; }
	}

	CMotionControl::GetInstance()->SetOffset2D(move.newpos);
}

////////////////////////////////////////////////////////////

void CGCodeParser::G68Ext12Command()
{
	// with angle+offset (xyz,ijk:angle, no R):   g68.10 x10 k30
	// xyz missing => no chang of offset

	SAxisMove move(true);
	mm1000_t vect[NUM_AXISXYZ] = { 0, 0, 0 };

	for (char ch = _reader->SkipSpacesToUpper(); ch; ch = _reader->SkipSpacesToUpper())
	{
		axis_t axis;
		if ((axis = CharToAxis(ch)) < NUM_AXISXYZ)				GetAxis(axis, move, super::_modalstate.IsAbsolut ? AbsolutWithZeroShiftPosition : RelativPosition);
		else if ((axis = CharToAxisOffset(ch)) < NUM_AXISXYZ)	GetG68IJK(axis, move, vect);
		else break;

		if (CheckError()) { return; }
	}

	if (move.axes)
		CMotionControl::GetInstance()->SetOffset2D(move.newpos);

	for (unsigned char axis = 0; axis < NUM_AXISXYZ; axis++)
	{
		if (IsBitSet(move.GetIJK(), axis))
		{
				// angle
			CMotionControl::GetInstance()->SetRotate2D(axis,CMm1000::DegreeToRAD(vect[axis]));
		}
	}
}

////////////////////////////////////////////////////////////

void CGCodeParser::G68ExtXXCommand(axis_t rotaxis)
{
	// calculate angle(X)

	SAxisMove move(true);
	mm1000_t vect[NUM_AXISXYZ] = { 0, 0, 0 };
	
	for (char ch = _reader->SkipSpacesToUpper(); ch; ch = _reader->SkipSpacesToUpper())
	{
		axis_t axis;
		if ((axis = CharToAxis(ch)) < NUM_AXISXYZ)			  GetAxis(axis, move, super::_modalstate.IsAbsolut ? AbsolutWithZeroShiftPosition : RelativPosition);
		else if ((axis = CharToAxisOffset(ch)) < NUM_AXISXYZ) 
		{
			if (rotaxis == axis)
				Error(MESSAGE_GCODE_SPECIFIED);
			else
				GetG68IJK(axis, move, vect);
		}
		else break;

		if (CheckError()) { return; }
	}

	float pos1=(float) (move.newpos[rotaxis] - CMotionControl::GetInstance()->GetOffset2D(rotaxis));
	float pos2;
	float angle=0;

	axis_t axis2;
	axis_t axis3;

	switch (rotaxis)
	{
		case X_AXIS: axis2=Y_AXIS;axis3=Z_AXIS; break;
		case Y_AXIS: axis2=Z_AXIS;axis3=X_AXIS; break;
		case Z_AXIS: axis2=X_AXIS;axis3=Y_AXIS; break;
		default: InfoNotImplemented(); return;
	}

	if (IsBitSet(move.GetIJK(),axis3))
	{
		// calc angle
		pos2 = (float)(move.newpos[axis2] -  CMotionControl::GetInstance()->GetOffset2D(axis2) - vect[axis3]) ;
		angle = atan2(pos2,pos1);
		CMotionControl::GetInstance()->SetRotate2D(axis3,angle);
		pos1 = hypotf(pos1,pos2);	// correction for 2nd rotation axis
	}
	if (IsBitSet(move.GetIJK(),axis2))
	{
		// calc angle
		pos2 = (float)(move.newpos[axis3] -  CMotionControl::GetInstance()->GetOffset2D(axis3) - vect[axis2]);
		angle = atan2(pos2,pos1);
		CMotionControl::GetInstance()->SetRotate2D(axis2,angle);
	}
}

////////////////////////////////////////////////////////////

void CGCodeParser::G69Command()
{
	if (CMotionControl::GetInstance()->IsRotate())
	{
		CMotionControl::GetInstance()->ClearRotate();
		SetPositionAfterG68G69();
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
	// G54 => idx = 1 => arraysize==1

	if (CutterRadiosIsOn()) return;

	if (idx > G54ARRAYSIZE)
	{
		ErrorNotImplemented();
		return;
	}

	_modlessstate.ZeroPresetIdx = _modalstate.ZeroPresetIdx = idx;
	CLcd::InvalidateLcd(); 
}

////////////////////////////////////////////////////////////

void CGCodeParser::G8xCommand(SAxisMove& move, bool useP, bool useQ, bool useMinQ)
{
	if (CutterRadiosIsOn()) return;

	unsigned char l = 1;

	for (char ch = _reader->SkipSpacesToUpper(); ch; ch = _reader->SkipSpacesToUpper())
	{
		axis_t axis;
		if ((axis = CharToAxis(ch)) <= Z_AXIS)				GetAxis(axis, move, super::_modalstate.IsAbsolut ? AbsolutWithZeroShiftPosition : RelativPosition);
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

		if (IsBitSet(move.axes, super::_modalstate.Plane_axis_2))
		{
			_modalstate.G8xPlane2 = move.newpos[super::_modalstate.Plane_axis_2];
		}

		mm1000_t pos[NUM_AXIS];
		CMotionControlBase::GetInstance()->GetPositions(pos);

		mm1000_t origPlane2 = pos[super::_modalstate.Plane_axis_2];

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
			if (super::_modalstate.IsAbsolut || i == 0)
			{
				pos[super::_modalstate.Plane_axis_0] = move.newpos[super::_modalstate.Plane_axis_0];
				pos[super::_modalstate.Plane_axis_1] = move.newpos[super::_modalstate.Plane_axis_1];
				if (!super::_modalstate.IsAbsolut)
				{
					move.newpos[super::_modalstate.Plane_axis_0] -= CMotionControlBase::GetInstance()->GetPosition(super::_modalstate.Plane_axis_0);
					move.newpos[super::_modalstate.Plane_axis_1] -= CMotionControlBase::GetInstance()->GetPosition(super::_modalstate.Plane_axis_1);
				}
			}
			else
			{
				pos[super::_modalstate.Plane_axis_0] += move.newpos[super::_modalstate.Plane_axis_0];
				pos[super::_modalstate.Plane_axis_1] += move.newpos[super::_modalstate.Plane_axis_1];
			}

			CMotionControlBase::GetInstance()->MoveAbs(pos, super::_modalstate.G0FeedRate);
			if (CheckError()) { return; }

			// 2. Step: GoTo z(R) (fast)
			pos[super::_modalstate.Plane_axis_2] = _modalstate.G8xR;
			CMotionControlBase::GetInstance()->MoveAbs(pos, super::_modalstate.G0FeedRate);
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
				pos[super::_modalstate.Plane_axis_2] = nextPlan2;
				CMotionControlBase::GetInstance()->MoveAbs(pos, super::_modalstate.G1FeedRate);
				if (CheckError()) { return; }

				// 3.a. Step: Wait
				if (useP &&  _modalstate.G8xP != 0)
				{
					Wait(_modalstate.G8xP);
				}

				// 4. Step: Goto init Z or R (fast) see G98
				if (finalMove)
				{
					pos[super::_modalstate.Plane_axis_2] = (_modalstate.IsG98) ? origPlane2 : _modalstate.G8xR;
				}
				else if (useMinQ)
				{
					pos[super::_modalstate.Plane_axis_2] = nextPlan2 + (drillDown ? G73RETRACTION : -G73RETRACTION);
				}
				else
				{
					pos[super::_modalstate.Plane_axis_2] = _modalstate.G8xR;
				}

				CMotionControlBase::GetInstance()->MoveAbs(pos, super::_modalstate.G0FeedRate);
				if (CheckError()) { return; }
			}
		}
	}
}

////////////////////////////////////////////////////////////

void CGCodeParser::G73Command()
{
	super::_modalstate.LastCommand = (LastCommandCB) &CGCodeParser::G73Command;

	SAxisMove move(true);
	G8xCommand(move, false, true, true);
}

////////////////////////////////////////////////////////////

void CGCodeParser::G81Command()
{
	super::_modalstate.LastCommand = (LastCommandCB) &CGCodeParser::G81Command;

	SAxisMove move(true);
	G8xCommand(move, false, false, false);
}

////////////////////////////////////////////////////////////

void CGCodeParser::G82Command()
{
	super::_modalstate.LastCommand = (LastCommandCB) &CGCodeParser::G82Command;
	
	SAxisMove move(true);
	G8xCommand(move, true, false, false);
}

////////////////////////////////////////////////////////////

void CGCodeParser::G83Command()
{
	super::_modalstate.LastCommand = (LastCommandCB) &CGCodeParser::G83Command;

	SAxisMove move(true);
	G8xCommand(move, false, true, false);
}

////////////////////////////////////////////////////////////

void CGCodeParser::M00Command()
{
	//Stop
	Sync();
	CControl::GetInstance()->StopProgram(false);
}

void CGCodeParser::M01Command()
{
	//Optional Stop
	Sync();
	CControl::GetInstance()->StopProgram(true);
}

void CGCodeParser::M02Command()
{
}

////////////////////////////////////////////////////////////

void CGCodeParser::M06Command()
{
	// ATC (automatic tool change)	
}

////////////////////////////////////////////////////////////

void CGCodeParser::M08Command()
{
	//coolant on (flood)
	CallIOControl(CControl::Coolant, CControl::CoolantFlood);
}

////////////////////////////////////////////////////////////

void CGCodeParser::M10Command()
{
	//vacuum on
	CallIOControl(CControl::Vacuum, CControl::VacuumOn);
}

////////////////////////////////////////////////////////////

void CGCodeParser::M11Command()
{
	//vacuum off
	CallIOControl(CControl::Vacuum, CControl::VacuumOff);
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

	if (!ExpectEndOfCommand()) { return; }

	super::_modalstate.Linenumber = linenumber;
}
////////////////////////////////////////////////////////////

void CGCodeParser::M111Command()
{
	// set debug level

	if (_reader->SkipSpacesToUpper() == 'S')
	{
		_reader->GetNextChar();
		_modalstate._debuglevel = GetUInt8();
	}

	if (!ExpectEndOfCommand()) { return; }
}

////////////////////////////////////////////////////////////

void CGCodeParser::M114Command()
{
	unsigned char postype = 0;

	if (_reader->SkipSpacesToUpper() == 'S')
	{
		_reader->GetNextChar();
		postype = GetUInt8();
	}

	_OkMessage = postype == 1 ? PrintRelPosition : PrintAbsPosition;

	if (!ExpectEndOfCommand()) { return; }
}


////////////////////////////////////////////////////////////

void CGCodeParser::M220Command()
{
	// set speed override

	if (_reader->SkipSpacesToUpper() == 'S')
	{
		_reader->GetNextChar();
		unsigned char speedInP = GetUInt8();
		if (IsError()) return;
		CStepper::GetInstance()->SetSpeedOverride(CStepper::PToSpeedOverride(speedInP));
	}
	else
	{
		Error(MESSAGE_GCODE_SExpected);
		return;
	}

	if (!ExpectEndOfCommand()) { return; }
}

////////////////////////////////////////////////////////////

void CGCodeParser::M300Command()
{
	SPlayTone tone[2];
	const SPlayTone* mytone = tone;
	bool fromprogmem = false;

	tone[0].Tone = ToneA4;
	tone[0].Duration = MilliSecToDuration(500);

	tone[1].Tone = ToneEnd;

	if (_reader->SkipSpacesToUpper() == 'S')
	{
		_reader->GetNextChar();
		unsigned int freq = GetUInt16();
		tone[0].Tone = (ETone)FreqToTone(freq);
		if (IsError()) return;

		switch (freq)
		{
			case 1: mytone = SPlayTone::PlayOK; fromprogmem = true; break;
			case 2: mytone = SPlayTone::PlayError; fromprogmem = true; break;
			case 3: mytone = SPlayTone::PlayInfo; fromprogmem = true; break;
		}
	}
	if (!fromprogmem && _reader->SkipSpacesToUpper() == 'P')
	{
		_reader->GetNextChar();
		tone[0].Duration = MilliSecToDuration(GetUInt16());
		if (IsError()) return;
	}

	if (!ExpectEndOfCommand()) { return; }

	if (CLcd::GetInstance())
	{
		CLcd::GetInstance()->Beep(mytone, fromprogmem);
	}
}

////////////////////////////////////////////////////////////

void CGCodeParser::CommandEscape()
{
	if (_reader->GetChar() == '$')
		_reader->GetNextChar();

	CNCLibCommandExtensions();
}

////////////////////////////////////////////////////////////

void CGCodeParser::CNCLibCommandExtensions()
{
	char ch = _reader->SkipSpaces();

	switch (ch)
	{
		case '?':
		{
			_reader->GetNextChar();
			if (!ExpectEndOfCommand()) { return; }

			CStepper::GetInstance()->Dump(CStepper::DumpAll);
			break;
		}
		case '!':
		{
			_reader->GetNextChar();
			if (_reader->IsEOC(SkipSpacesOrComment()))
			{
				CControl::GetInstance()->Kill();
			}
			else
			{
			}

			break;
		}
	}
}

////////////////////////////////////////////////////////////

void CGCodeParser::PrintAbsPosition()
{
	char tmp[16];
	for (unsigned char i = 0; i < NUM_AXIS; i++)
	{
		if (i != 0)
			StepperSerial.print(MESSAGE_PARSER_COLON);
		StepperSerial.print(CMm1000::ToString(CMotionControlBase::GetInstance()->GetPosition(i), tmp, 3));
	}
}

////////////////////////////////////////////////////////////

void CGCodeParser::PrintRelPosition()
{
	char tmp[16];
	for (unsigned char i = 0; i < NUM_AXIS; i++)
	{
		if (i != 0)
			StepperSerial.print(MESSAGE_PARSER_COLON);

		StepperSerial.print(CMm1000::ToString(CMotionControlBase::GetInstance()->GetPosition(i) - CGCodeParser::GetAllPreset(i), tmp, 3));
	}
}

