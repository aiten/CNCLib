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

#define MAXSPINDEL_SPEED	0x7fff

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

	if (IsG92OffsetParam(paramNo,axis))			return GetParamAsPosition(super::_modalstate.G92Pospreset[axis],axis);
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

	
		else if (IsBacklashParam(paramNo,axis))	{	CStepper::GetInstance()->SetBacklash(axis,(mdist_t) GetParamAsMaschine(mm1000, axis));	}
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

mm1000_t CGCodeParser::CalcAllPreset(axis_t axis)			
{ 
	return GetG54PosPreset(axis) + (IsG53Present() ? 0 : super::GetG92PosPreset(axis) + GetToolHeightPosPreset(axis));
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
		case 73:	G73Command(); return true;
		case 81:	G81Command(); return true;
		case 82:	G82Command(); return true;
		case 83:	G83Command(); return true;
		case 98:	G98Command(); return true;
		case 99:	G99Command();  return true;
	}
	return false;
}

////////////////////////////////////////////////////////////

bool CGCodeParser::MCommand(unsigned char mcode)
{
	if (super::MCommand(mcode))
		return true;

	switch (mcode)
	{
		case 0:	M00Command(); return true;
		case 1:	M01Command(); return true;
		case 2:	M02Command(); return true;
		case 6: M06Command(); return true;
		case 7: M07Command(); return true;
		case 8: M08Command(); return true;
		case 9: M09Command(); return true;
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

	super::_modalstate.SpindleSpeed = speed;
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
	_modalstate.G8xR = ParseCoordinate(super::_modalstate.Plane_axis_2, CMotionControl::GetPosition(super::_modalstate.Plane_axis_2), super::_modalstate.IsAbsolut ? AbsolutWithZeroShiftPosition : RelativPosition);
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
		CMotionControl::GetPositions(pos);

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
					move.newpos[super::_modalstate.Plane_axis_0] -= CMotionControl::GetPosition(super::_modalstate.Plane_axis_0);
					move.newpos[super::_modalstate.Plane_axis_1] -= CMotionControl::GetPosition(super::_modalstate.Plane_axis_1);
				}
			}
			else
			{
				pos[super::_modalstate.Plane_axis_0] += move.newpos[super::_modalstate.Plane_axis_0];
				pos[super::_modalstate.Plane_axis_1] += move.newpos[super::_modalstate.Plane_axis_1];
			}

			CMotionControl::MoveAbs(pos, super::_modalstate.G0FeedRate);
			if (CheckError()) { return; }

			// 2. Step: GoTo z(R) (fast)
			pos[super::_modalstate.Plane_axis_2] = _modalstate.G8xR;
			CMotionControl::MoveAbs(pos, super::_modalstate.G0FeedRate);
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
				CMotionControl::MoveAbs(pos, super::_modalstate.G1FeedRate);
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

				CMotionControl::MoveAbs(pos, super::_modalstate.G0FeedRate);
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
	Sync();
	CControl::GetInstance()->Pause();
}

void CGCodeParser::M01Command()
{
	Sync();
	CControl::GetInstance()->Pause();
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

void CGCodeParser::M07Command()
{
	//coolant on
	Sync();
	CControl::GetInstance()->IOControl(CControl::Coolant, 1);
}

////////////////////////////////////////////////////////////

void CGCodeParser::M08Command()
{
	//coolant on (flood)
	Sync();
	CControl::GetInstance()->IOControl(CControl::Coolant, 2);
}

////////////////////////////////////////////////////////////

void CGCodeParser::M09Command()
{
	//coolant off
	Sync();
	CControl::GetInstance()->IOControl(CControl::Coolant, 0);
}

////////////////////////////////////////////////////////////
