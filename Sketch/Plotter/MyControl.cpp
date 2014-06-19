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

	StepperSerial.println(F("Plotter(HA) is starting ... ("__DATE__", "__TIME__")"));

#ifdef __USE_LCD__
	Lcd.Init();
#endif

	CHPGLParser::Init();

	CStepper::GetInstance()->SetLimitMax(0, 55600);  // 6950*8
	CStepper::GetInstance()->SetLimitMax(1, 32000);  // 4000*8
	CStepper::GetInstance()->SetLimitMax(2, 8000);   // 100*8

	CStepper::GetInstance()->SetJerkSpeed(0, 4000);  // 500 * 8?
	CStepper::GetInstance()->SetJerkSpeed(1, 4000);
	CStepper::GetInstance()->SetJerkSpeed(2, 4000);

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
}

void CMyControl::Initialized()
{
	super::Initialized();
	GoToReference();
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

bool CMyControl::Parse()
{
	CHPGLParser hpgl(&_reader);
	return ParseAndPrintResult(&hpgl);
}

////////////////////////////////////////////////////////////
