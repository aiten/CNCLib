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
*/
////////////////////////////////////////////////////////

#pragma once

////////////////////////////////////////////////////////

#define MESSAGE_EXPR_EMPTY_EXPR						StepperMessageOne("Empty expression")
#define MESSAGE_EXPR_FORMAT							StepperMessageOne("Expression format error")
#define MESSAGE_EXPR_UNKNOWN_FUNCTION				StepperMessageOne("Unknown function")
#define MESSAGE_EXPR_SYNTAX_ERROR					StepperMessageOne("Syntax error")
#define MESSAGE_EXPR_MISSINGRPARENTHESIS			StepperMessageOne("Missing right parenthesis")
#define MESSAGE_EXPR_ILLEGAL_OPERATOR				StepperMessageOne("Illegal operator")
#define MESSAGE_EXPR_ILLEGAL_FUNCTION				StepperMessageOne("Illegal function")
#define MESSAGE_EXPR_UNKNOWN_VARIABLE				StepperMessageOne("Unknown variable")
#define MESSAGE_EXPR_FRACTORIAL						StepperMessageOne("factorial")


#define MESSAGE_CONTROL_KILLED						StepperMessageOr("K1","Killed - command ignored!")
#define MESSAGE_CONTROL_FLUSHBUFFER					StepperMessageOr("FB","Flush Buffer")
#define MESSAGE_CONTROL_RESULTS						StepperMessageOr(">", " => ")

#define MESSAGE_GCODE_CommentNestingError			StepperMessage("1","Comment nesting error")
#define MESSAGE_GCODE_VaribaleMustStartWithAlpha	StepperMessage("2","varibale must start with alpha")
#define MESSAGE_GCODE_NoValidVaribaleName			StepperMessage("3","no valid varibale name")
#define MESSAGE_GCODE_ParameterDoesntExist			StepperMessage("4","parameter doesnt exist")
#define MESSAGE_GCODE_ParameterNotFound				StepperMessage("6","Parameter not found")
#define MESSAGE_GCODE_UnspportedParameterNumber		StepperMessage("8","unspported # parameter number")
#define MESSAGE_GCODE_ValueGreaterThanMax			StepperMessage("9","value greater than max")
#define MESSAGE_GCODE_LinenumberExpected			StepperMessage("A","linenumber expected")
#define MESSAGE_GCODE_CcommandExpected				StepperMessage("B","command expected")
#define MESSAGE_GCODE_UnsupportedGCommand			StepperMessage("C","unsupported G command")
#define MESSAGE_GCODE_MCodeExpected					StepperMessage("D","m-code expected")
#define MESSAGE_GCODE_UnspportedMCodeIgnored		StepperMessage("E","unspported m code ignored")
#define MESSAGE_GCODE_ParamNoExpected				StepperMessage("F","paramNo expected")
#define MESSAGE_GCODE_EqExpected					StepperMessage("10","= expected")
#define MESSAGE_GCODE_CommandExpected				StepperMessage("11","command expected")
#define MESSAGE_GCODE_IllegalCommand				StepperMessage("12","Illegal command")
#define MESSAGE_GCODE_NoValidTool					StepperMessage("13","No valid tool")
//#define MESSAGE_GCODE_spindleSpeedExceeded			StepperMessage("14","Spindle speed exceeded")
#define MESSAGE_GCODE_AxisNotSupported				StepperMessage("15","axis not supported")
#define MESSAGE_GCODE_AxisAlreadySpecified			StepperMessage("16","axis already specified")
#define MESSAGE_GCODE_ParameterSpecifiedMoreThanOnce StepperMessage("17","parameter specified more than once")
#define MESSAGE_GCODE_AxisOffsetMustNotBeSpecified	StepperMessage("18","Axis offset must not be specified")
#define MESSAGE_GCODE_RalreadySpecified				StepperMessage("1A","R already specified")
#define MESSAGE_GCODE_PalreadySpecified				StepperMessage("1B","P already specified")
#define MESSAGE_GCODE_LalreadySpecified				StepperMessage("1C","L already specified")
#define MESSAGE_GCODE_LmustBe1_255					StepperMessage("1D","L must be 1..255")
#define MESSAGE_GCODE_QalreadySpecified				StepperMessage("1E","Q already specified")
#define MESSAGE_GCODE_QmustBeAPositivNumber			StepperMessage("1F","Q must be a positiv number")
#define MESSAGE_GCODE_FalreadySpecified				StepperMessage("20","F already specified")
#define MESSAGE_GCODE_FeedrateWithG0				StepperMessage("21","F not allowed with G0")
#define MESSAGE_GCODE_IJKandRspecified				StepperMessage("22","IJK and R specified")
#define MESSAGE_GCODE_MissingIKJorR					StepperMessage("23","missing IKJ or R")
#define MESSAGE_GCODE_MissingR						StepperMessage("24","missing R")
#define MESSAGE_GCODE_360withRandMissingAxes		StepperMessage("25","360 with R and missing axes")
#define MESSAGE_GCODE_STATUS_ARC_RADIUS_ERROR		StepperMessage("26","STATUS_ARC_RADIUS_ERROR")
#define MESSAGE_GCODE_LExpected						StepperMessage("27","L expected")
#define MESSAGE_GCODE_PExpected						StepperMessage("28","P expected")
#define MESSAGE_GCODE_UnsupportedLvalue				StepperMessage("29","unsupported L value")
#define MESSAGE_GCODE_UnsupportedCoordinateSystemUseG54Instead StepperMessage("2A","unsupported coordinate system, use G54 instead")
#define MESSAGE_GCODE_G41G43AreNotAllowedWithThisCommand StepperMessage("2B","G41/G42 are not allowed with this command")
#define MESSAGE_GCODE_NoAxesForProbe				StepperMessage("2C","No axes for probe")
#define MESSAGE_GCODE_ProbeOnlyForXYZ				StepperMessage("2D","Probe only for X Y Z")
#define MESSAGE_GCODE_ProbeIOmustBeOff				StepperMessage("2E","Probe-IO must be off")
#define MESSAGE_GCODE_ProbeFailed					StepperMessage("2F","Probe failed")
#define MESSAGE_GCODE_QmustNotBe0					StepperMessage("31","Q must not be 0")
#define MESSAGE_GCODE_RmustBeBetweenCurrentRZ		StepperMessage("32","R must be between current < R < Z")
#define MESSAGE_GCODE_NotImplemented				StepperMessage("33","Not Implemented")

#define MESSAGE_PARSER_EndOfCommandExpected			StepperMessage("34","End of command expected")
#define MESSAGE_PARSER_NotANumber_MissingScale		StepperMessage("35","Not a number: missing scale")
#define MESSAGE_PARSER_NotANumber_MaxScaleExceeded	StepperMessage("36","Not a number: max scale exceeded")
#define MESSAGE_PARSER_ValueGreaterThanMax			StepperMessage("38","value greater than max")
#define MESSAGE_PARSER_NotImplemented				StepperMessage("39","not implemented")
#define MESSAGE_PARSER_NotANumber					StepperMessage("3A","not a number")
#define MESSAGE_PARSER_OverrunOfNumber				StepperMessage("3B","overrun of number")
#define MESSAGE_PARSER_ValueLessThanMin				StepperMessage("3C","value less than min")

#define MESSAGE_GCODE_SExpected						StepperMessage("3D","S expected")
#define MESSAGE_GCODE_IJKVECTORIS0					StepperMessage("3E","Vector IJK is 0")
#define MESSAGE_GCODE_SPECIFIED						StepperMessage("3F","IJK is specified")

#define MESSAGE_PARSER_COLON F(":")

////////////////////////////////////////////////////////

