#pragma once

////////////////////////////////////////////////////////

//#define StepperSerial SerialUSB
#define StepperSerial Serial

////////////////////////////////////////////////////////

#define X_AXIS 0
#define Y_AXIS 1
#define Z_AXIS 2
#define A_AXIS 3	// rotary around X
#define B_AXIS 4	// rotary around Y
#define C_AXIS 5	// rotary around Z
#define U_AXIS 6	// Relative axis parallel to U
#define V_AXIS 7	// Relative axis parallel to V
#define W_AXIS 8	// Relative axis parallel to W

////////////////////////////////////////////////////////

typedef unsigned char axis_t;	// type for "axis"

typedef signed   long sdist_t;	// tpye of stepper coord system (signed)
typedef unsigned long udist_t;	// tpye of stepper coord system (unsigned)

typedef float expr_t;			// type for expression parser

typedef long mm1000_t;			// 1/1000 mm
typedef long feedrate_t;		// mm_1000 / min

////////////////////////////////////////////////////////
//
// Control

#define SERIALBUFFERSIZE	128			// even size 
#define REFERENCESTABLETIME	2			// time in ms for reference must not change (in Reference move) => signal bounce

#define TIMEOUTCALLIDEL		333			// time in ms after move completet to call Idle

#define IDLETIMER0VALUE     TIMER0VALUE(1000)		// AVR dont care ... Timer 0 shared with milli	

#define BLINK_LED			13
#define TIMEOUTBLINK		1000		// blink of led 13

// Stepper

#define IDLETIMER1VALUE		TIMER1VALUE(10)			// Idle timer value (stepper timer not moving)
#define TIMEOUTSETIDLE		1000					// set level after 1000ms

#define MAXSPEED			(65535)					// see range for mdist_t
#define MAXINTERRUPTSPEED	(65535/7)				// maximal possible interrupt rate
#define TIMER1VALUEMAXSPEED	TIMER1VALUE(MAXSPEED)

#define SPEED_MULTIPLIER_1			0
#define SPEED_MULTIPLIER_2			(MAXINTERRUPTSPEED*1)
#define SPEED_MULTIPLIER_3			(MAXINTERRUPTSPEED*2)
#define SPEED_MULTIPLIER_4			(MAXINTERRUPTSPEED*3)
#define SPEED_MULTIPLIER_5			(MAXINTERRUPTSPEED*4)
#define SPEED_MULTIPLIER_6			(MAXINTERRUPTSPEED*5)
#define SPEED_MULTIPLIER_7			(MAXINTERRUPTSPEED*6)

////////////////////////////////////////////////////////

#if defined(__AVR_ATmega1280__) || defined(__AVR_ATmega2560__)

// usual with Ramps1.4

#undef use32bit
#define use16bit

#define NUM_AXIS			5

#define STEPBUFFERSIZE		128		// size 2^x but not 256
#define MOVEMENTBUFFERSIZE	64

////////////////////////////////////////////////////////

#elif defined(__AVR_ATmega328P__)

// usual with SMC800

#undef use32bit
#define use16bit

#define STEPBUFFERSIZE		16		// size 2^x but not 256
#define MOVEMENTBUFFERSIZE	8

#undef NUM_AXIS
#define NUM_AXIS 3

#define REDUCED_SIZE

////////////////////////////////////////////////////////

#elif defined(__SAM3X8E__)

// usual with Ramps FD

//#undef StepperSerial
//#define StepperSerial SerialUSB

#define use32bit
#undef use16bit

#define NUM_AXIS			6

#define STEPBUFFERSIZE		128		// size 2^x but not 256
#define MOVEMENTBUFFERSIZE	64

////////////////////////////////////////////////////////

#elif defined (_MSC_VER)

// test environment only

typedef unsigned long long uint64_t;

#undef use32bit
#define use16bit

//#undef use16bit
//#define use32bit

#define STEPBUFFERSIZE		16
#define MOVEMENTBUFFERSIZE	32

//#undef NUM_AXIS
#define NUM_AXIS 5

#undef REFERENCESTABLETIME
#define REFERENCESTABLETIME	0

#define MOVEMENTINFOSIZE	128

////////////////////////////////////////////////////////

#else
ToDo;
#endif

////////////////////////////////////////////////////////
// Global types and configuration
////////////////////////////////////////////////////////

#if defined(use16bit)

#define MAXSTEPSPERMOVE		0xffff			// split in moves
#define MAXACCDECSTEPS		(0x10000/4 -10)	// max stepps for acc and dec ramp ( otherwise overrun)

typedef unsigned short timer_t;			// timer tpye (16bit)
typedef unsigned short mdist_t;			// tpye for one movement (16bit)
typedef unsigned short steprate_t;		// tpye for speed (Hz), Steps/sec

#define mudiv	udiv
#define mudiv_t	udiv_t

#elif defined(use32bit)

#define MAXSTEPSPERMOVE		0xffffffff	// split in Moves
#define MAXACCDECSTEPS		0x1000000

typedef unsigned long timer_t;			// timer tpye (32bit)
typedef unsigned long mdist_t;			// tpye for one movement (32bit)
typedef unsigned long steprate_t;		// tpye for speed (Hz), Steps/sec

#define mudiv	ldiv
#define mudiv_t	ldiv_t

#endif

/////////////////////////////////////////////////////////////////////////////////////////////////

#ifdef _MSC_VER

#define EnumAsByte(a) a
#define debugvirtula virtual
#define stepperstatic 

#else

#define stepperstatic static
#define stepperstatic_
#define EnumAsByte(a) unsigned char			// use a 8 bit enum (and not 16, see compiler output)
#define debugvirtula						// only used in msvc for debugging - not used on AVR controller 

#endif

#if NUM_AXIS > 3
typedef unsigned long DirCountAll_t;		// 4 bit for eache axis (0..7) count, 8 dirup, see DirCountAll_t
#define DirCountBytes 4
#else
typedef unsigned short DirCountAll_t;		// 4 bit for eache axis (0..7) count, 8 dirup 
#define DirCountBytes 2
#endif

#if NUM_AXIS > 7
#error "NUM_AXIS must be < 8"				// because of last dirCount_t used for info 
#endif

struct DirCountStepByte_t
{
	unsigned char count1 : 3;
	unsigned char dirUp1 : 1;

	unsigned char count2 : 3;
	unsigned char dirUp2 : 1;
};

struct DirCountInfoByte_t
{
	unsigned char count1 : 3;
	unsigned char dirUp1 : 1;

	unsigned char nocount : 1;		// do not count step (e.g. move for backlash
	unsigned char unused1 : 1;
	unsigned char unused2 : 1;
	unsigned char unused3 : 1;
};

struct DirCountByte_t
{
	DirCountStepByte_t byte[DirCountBytes - 1];
	DirCountInfoByte_t byteInfo;
};

union DirCount_t
{
	DirCountByte_t		byte;
	DirCountAll_t		all;
};


////////////////////////////////////////////////////////

#define MESSAGE_OK									F("ok")
#define MESSAGE_ERROR								F("Error: ")
#define MESSAGE_INFO								F("Info: ")
#define MESSAGE_WARNING								F("Warning: ")

#define MESSAGE_CONTROL_KILLED						F("Killed - command ignored!")
#define MESSAGE_CONTROL_FLUSHBUFFER					F("Flush Buffer")

#define MESSAGE_EXPR_EMPTY_EXPR						F("Empty expression")
#define MESSAGE_EXPR_FORMAT							F("Expression format error")
#define MESSAGE_EXPR_UNKNOWN_FUNCTION				F("Unknown function")
#define MESSAGE_EXPR_SYNTAX_ERROR					F("Syntax error")
#define MESSAGE_EXPR_MISSINGRPARENTHESIS			F("Missing right parenthesis")
#define MESSAGE_EXPR_ILLEGAL_OPERATOR				F("Illegal operator")
#define MESSAGE_EXPR_ILLEGAL_FUNCTION				F("Illegal function")
#define MESSAGE_EXPR_UNKNOWN_VARIABLE				F("Unknown variable")
#define MESSAGE_EXPR_FRACTORIAL						F("factorial")

#define MESSAGE_GCODE_CommentNestingError			F("Comment nesting error")
#define MESSAGE_GCODE_VaribaleMustStartWithAlpha	F("varibale must start with alpha")
#define MESSAGE_GCODE_NoValidVaribaleName			F("no valid varibale name")
#define MESSAGE_GCODE_ParameterDoesntExist			F("parameter doesnt exist")
#define MESSAGE_GCODE_NoValidVaribaleName			F("no valid varibale name")
#define MESSAGE_GCODE_ParameterNotFound				F("Parameter not found")
#define MESSAGE_GCODE_ParameterNotFound				F("Parameter not found")
#define MESSAGE_GCODE_UnspportedParameterNumber		F("unspported # parameter number")
#define MESSAGE_GCODE_ValueGreaterThanMax			F("value greater than max")
#define MESSAGE_GCODE_LinenumberExpected			F("linenumber expected")
#define MESSAGE_GCODE_CcommandExpected				F("command expected")
#define MESSAGE_GCODE_UnsupportedGCommand			F("unsupported G command")
#define MESSAGE_GCODE_MCodeExpected					F("m-code expected")
#define MESSAGE_GCODE_UnspportedMCodeIgnored		F("unspported m code ignored")
#define MESSAGE_GCODE_ParamNoExpected				F("paramNo expected")
#define MESSAGE_GCODE_EqExpected					F("= expected")
#define MESSAGE_GCODE_CommandExpected				F("command expected")
#define MESSAGE_GCODE_IllegalCommand				F("Illegal command")
#define MESSAGE_GCODE_NoValidTool					F("No valid tool")
#define MESSAGE_GCODE_SpindleSpeedExceeded			F("Spindle speed exceeded")
#define MESSAGE_GCODE_AxisNotSupported				F("axis not supported")
#define MESSAGE_GCODE_AxisAlreadySpecified			F("axis already specified")
#define MESSAGE_GCODE_ParameterSpecifiedMoreThanOnce	F("parameter specified more than once")
#define MESSAGE_GCODE_AxisOffsetMustNotBeSpecified		F("Axis offset must not be specified")
#define MESSAGE_GCODE_RalreadySpecified				F("R already specified")
#define MESSAGE_GCODE_RalreadySpecified				F("R already specified")
#define MESSAGE_GCODE_PalreadySpecified				F("P already specified")
#define MESSAGE_GCODE_LalreadySpecified				F("L already specified")
#define MESSAGE_GCODE_LmustBe1_255					F("L must be 1..255")
#define MESSAGE_GCODE_QalreadySpecified				F("Q already specified")
#define MESSAGE_GCODE_QmustBeAPositivNumber			F("Q must be a positiv number")
#define MESSAGE_GCODE_FalreadySpecified				F("F already specified")
#define MESSAGE_GCODE_IJKandRspecified				F("IJK and R specified")
#define MESSAGE_GCODE_MissingIKJorR					F("missing IKJ or R")
#define MESSAGE_GCODE_360withRandMissingAxes		F("360 with R and missing axes")
#define MESSAGE_GCODE_STATUS_ARC_RADIUS_ERROR		F("STATUS_ARC_RADIUS_ERROR")
#define MESSAGE_GCODE_LExpected						F("L expected")
#define MESSAGE_GCODE_PExpected						F("P expected")
#define MESSAGE_GCODE_UnsupportedLvalue				F("unsupported L value")
#define MESSAGE_GCODE_UnsupportedCoordinateSystemUseG54Instead		F("unsupported coordinate system, use G54 instead")
#define MESSAGE_GCODE_G41G43AreNotAllowedWithThisCommand F("G41/G42 are not allowed with this command")
#define MESSAGE_GCODE_NoAxesForProbe				F("No axes for probe")
#define MESSAGE_GCODE_ProbeOnlyForXYZ				F("Probe only for X Y Z")
#define MESSAGE_GCODE_ProbeIOmustBeOff				F("Probe-IO must be off")
#define MESSAGE_GCODE_ProbeFailed					F("Probe failed")
#define MESSAGE_GCODE_NoValidTool					F("No valid tool")
#define MESSAGE_GCODE_QmustNotBe0					F("Q must not be 0")
#define MESSAGE_GCODE_RmustBeBetweenCurrentRZ		F("R must be between current < R < Z")
#define MESSAGE_GCODE_NotImplemented				F("Not Implemented")

#define MESSAGE_PARSER_EndOfCommandExpected			F("End of command expected")
#define MESSAGE_PARSER_NotANumber_MissingScale		F("Not a number: missing scale")
#define MESSAGE_PARSER_NotANumber_MaxScaleExceeded	F("Not a number: max scale exceeded")
#define MESSAGE_PARSER_ValueLessThanMin				F("value less than min")
#define MESSAGE_PARSER_ValueGreaterThanMax			F("value greater than max")
#define MESSAGE_PARSER_NotImplemented				F("not implemented")
#define MESSAGE_PARSER_NotANumber					F("not a number")
#define MESSAGE_PARSER_OverrunOfNumber				F("overrun of number")
#define MESSAGE_PARSER_ValueLessThanMin				F("value less than min")
#define MESSAGE_PARSER_ValueGreaterThanMax			F("value greater than max")
#define MESSAGE_PARSER_ValueLessThanMin				F("value less than min")
#define MESSAGE_PARSER_ValueGreaterThanMax			F("value greater than max")

#define MESSAGE_STEPPER_EmptyMoveSkipped			F("EmptyMove skipped")
#define MESSAGE_STEPPER_Backlash					F("Backlash")
#define MESSAGE_STEPPER_IsAnyReference				F("IsAnyReference")
#define MESSAGE_STEPPER_RangeLimit					F("Range limit")
#define MESSAGE_STEPPER_IsReferenceIsOn				F("IsReference is on")
#define MESSAGE_STEPPER_MoveReferenceFailed			F("MoveReference failed")

#define MESSAGE_STEPPER_MoveAwayFromReference		F("Move away from reference")

////////////////////////////////////////////////////////

