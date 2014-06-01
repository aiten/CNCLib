#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <StepperSystem.h>
#include "MyControl.h"
#include "HPGLParser.h"
#include "PlotterControl.h"

////////////////////////////////////////////////////////////

void CMyControl::Init()
{
	super::Init();

	CStepper::GetInstance()->SetWaitFinishMove(false);
	CStepper::GetInstance()->SetDefaultMaxSpeed(CHPGLParser::_state.penUp.max, CHPGLParser::_state.penUp.acc, CHPGLParser::_state.penUp.dec);

#if defined(__AVR_ATmega1280__) || defined(__AVR_ATmega2560__) || defined(__SAM3X8E__)
	for (register unsigned char i=0;i<NUM_AXIS*2;i++)
	{
		CStepper::GetInstance()->UseReference(i,false);  
	}
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(X_AXIS, true),true);  
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Y_AXIS, true),true);  
	CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(Z_AXIS, true),true);  
	CStepper::GetInstance()->UseReference(EMERGENCY_ENDSTOP,true);    // not stop
#endif    

	CStepper::GetInstance()->MoveReference(Z_AXIS, CStepper::GetInstance()->ToReferenceId(Z_AXIS, true), true, CStepper::GetInstance()->GetDefaultVmax() / 4);
	CStepper::GetInstance()->MoveReference(X_AXIS, CStepper::GetInstance()->ToReferenceId(X_AXIS, true), true, CStepper::GetInstance()->GetDefaultVmax() / 4);
	CStepper::GetInstance()->MoveReference(Y_AXIS, CStepper::GetInstance()->ToReferenceId(Y_AXIS, true), true, CStepper::GetInstance()->GetDefaultVmax() / 4);
}
////////////////////////////////////////////////////////////

void CMyControl::GoToReference()
{
	super::GoToReference();

	GoToReference(Z_AXIS);
	GoToReference(Y_AXIS);
	GoToReference(X_AXIS);
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference(axis_t axis)
{
#define FEEDRATE_REFMOVE  CStepper::GetInstance()->GetDefaultVmax() / 4  
/*
	if (axis == Z_AXIS)
	{
		// goto max
		CStepper::GetInstance()->MoveReference(axis, CStepper::GetInstance()->ToReferenceId(axis, false), false, FEEDRATE_REFMOVE);
	}
	else
*/
	{
		// goto min
		CStepper::GetInstance()->MoveReference(axis, CStepper::GetInstance()->ToReferenceId(axis, true), true, FEEDRATE_REFMOVE);
	}
}

////////////////////////////////////////////////////////////

void CMyControl::Idle(unsigned int idletime)
{
	Plotter.Idle(idletime);
	super::Idle(idletime);
}

////////////////////////////////////////////////////////////

void CMyControl::Parse()
{
	CHPGLParser hpgl(&_reader);
	ParseAndPrintResult(&hpgl);
}

////////////////////////////////////////////////////////////
