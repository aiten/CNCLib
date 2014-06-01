#include "StepperRampsFD.h"

#if defined(__AVR_ATmega2560__) || defined(_MSC_VER) || defined(__SAM3X8E__)

////////////////////////////////////////////////////////

#include "StepperRampsFD_Pins.h"

////////////////////////////////////////////////////////

// only available on Arduino Mega / due

////////////////////////////////////////////////////////

CStepperRampsFD::CStepperRampsFD()
{
	InitMemVar();
}

////////////////////////////////////////////////////////

void CStepperRampsFD::InitMemVar()
{
}

////////////////////////////////////////////////////////

void CStepperRampsFD::Init()
{
	pinMode(X_STEP_PIN, OUTPUT);
	pinMode(X_DIR_PIN, OUTPUT);
	pinMode(X_ENABLE_PIN, OUTPUT);
	pinMode(X_MIN_PIN, INPUT_PULLUP);
	pinMode(X_MAX_PIN, INPUT_PULLUP);

	pinMode(Y_STEP_PIN, OUTPUT);
	pinMode(Y_DIR_PIN, OUTPUT);
	pinMode(Y_ENABLE_PIN, OUTPUT);
	pinMode(Y_MIN_PIN, INPUT_PULLUP);
	pinMode(Y_MAX_PIN, INPUT_PULLUP);

	pinMode(Z_STEP_PIN, OUTPUT);
	pinMode(Z_DIR_PIN, OUTPUT);
	pinMode(Z_ENABLE_PIN, OUTPUT);
	pinMode(Z_MIN_PIN, INPUT_PULLUP);
	pinMode(Z_MAX_PIN, INPUT_PULLUP);

	pinMode(E0_STEP_PIN, OUTPUT);
	pinMode(E0_DIR_PIN, OUTPUT);
	pinMode(E0_ENABLE_PIN, OUTPUT);
	//  pinMode(E0_MIN_PIN,	INPUT_PULLUP);         
	//  pinMode(E0_MAX_PIN,	INPUT_PULLUP);         

	pinMode(E1_STEP_PIN, OUTPUT);
	pinMode(E1_DIR_PIN, OUTPUT);
	pinMode(E1_ENABLE_PIN, OUTPUT);
	//  pinMode(E1_MIN_PIN,	INPUT_PULLUP);         
	//  pinMode(E1_MAX_PIN,	INPUT_PULLUP);         

	pinMode(E2_STEP_PIN, OUTPUT);
	pinMode(E2_DIR_PIN, OUTPUT);
	pinMode(E2_ENABLE_PIN, OUTPUT);
	//  pinMode(E2_MIN_PIN,	INPUT_PULLUP);         
	//  pinMode(E2_MAX_PIN,	INPUT_PULLUP);         

#pragma warning( disable : 4127 )

	WRITE(X_STEP_PIN, PINON);
	WRITE(Y_STEP_PIN, PINON);
	WRITE(Z_STEP_PIN, PINON);
	WRITE(E0_STEP_PIN, PINON);
	WRITE(E1_STEP_PIN, PINON);

#pragma warning( default : 4127 )

	InitMemVar();
	super::Init();
}

////////////////////////////////////////////////////////

void CStepperRampsFD::Step(axis_t axis, bool directionUp, unsigned char count)
{

	// call Step only within Critical region => use _WRITE_NC of fastio.h!!!
#define SETDIR(dirpin)		if (directionUp) _WRITE_NC(dirpin,PINOFF); else _WRITE_NC(dirpin,PINON);
#define OUTSTEP(steppin)	{	_WRITE_NC(steppin, PINOFF); _WRITE_NC(steppin, PINON); }

	switch (axis)
	{
#pragma warning( disable : 4127 )
		case X_AXIS:  SETDIR(X_DIR_PIN);   for (; count != 0; count--) OUTSTEP(X_STEP_PIN); return;
		case Y_AXIS:  SETDIR(Y_DIR_PIN);   for (; count != 0; count--) OUTSTEP(Y_STEP_PIN); return;
		case Z_AXIS:  SETDIR(Z_DIR_PIN);   for (; count != 0; count--) OUTSTEP(Z_STEP_PIN); return;
		case E0_AXIS: SETDIR(E0_DIR_PIN);  for (; count != 0; count--) OUTSTEP(E0_STEP_PIN); return;
		case E1_AXIS: SETDIR(E1_DIR_PIN);  for (; count != 0; count--) OUTSTEP(E1_STEP_PIN); return;
		case E2_AXIS: SETDIR(E1_DIR_PIN);  for (; count != 0; count--) OUTSTEP(E2_STEP_PIN); return;
#pragma warning( default : 4127 )
	}

#undef OUTSTEP
}

////////////////////////////////////////////////////////

void CStepperRampsFD::SetEnable(axis_t axis, unsigned char level)
{

#define SETLEVEL(pin) if (level != 0)	WRITE(pin,PINOFF);	else	WRITE(pin,PINON);
	switch (axis)
	{
#pragma warning( disable : 4127 )
		case X_AXIS:  SETLEVEL(X_ENABLE_PIN); break;
		case Y_AXIS:  SETLEVEL(Y_ENABLE_PIN); break;
		case Z_AXIS:  SETLEVEL(Z_ENABLE_PIN); break;
		case E0_AXIS: SETLEVEL(E0_ENABLE_PIN); break;
		case E1_AXIS: SETLEVEL(E1_ENABLE_PIN); break;
		case E2_AXIS: SETLEVEL(E2_ENABLE_PIN); break;
#pragma warning( default : 4127 )
	}
#undef SETLEVEL

}
////////////////////////////////////////////////////////

unsigned char CStepperRampsFD::GetEnable(axis_t axis)
{
	switch (axis)
	{
#pragma warning( disable : 4127 )
		case X_AXIS:  return READ(X_ENABLE_PIN) == PINON ? 0 : 100;
		case Y_AXIS:  return READ(Y_ENABLE_PIN) == PINON ? 0 : 100;
		case Z_AXIS:  return READ(Z_ENABLE_PIN) == PINON ? 0 : 100;
		case E0_AXIS: return READ(E0_ENABLE_PIN) == PINON ? 0 : 100;
		case E1_AXIS: return READ(E1_ENABLE_PIN) == PINON ? 0 : 100;
		case E2_AXIS: return READ(E2_ENABLE_PIN) == PINON ? 0 : 100;
#pragma warning( default : 4127 )
	}
	return 0;
}

////////////////////////////////////////////////////////

bool  CStepperRampsFD::IsReference(unsigned char referenceid)
{
	switch (referenceid)
	{
		case 0: return READ(X_MIN_PIN) == REF_ON;
		case 1: return READ(X_MAX_PIN) == REF_ON;
		case 2: return READ(Y_MIN_PIN) == REF_ON;
		case 3: return READ(Y_MAX_PIN) == REF_ON;
		case 4: return READ(Z_MIN_PIN) == REF_ON;
		case 5: return READ(Z_MAX_PIN) == REF_ON;
			//		case 6: return READ(E0_MIN_PIN)==REF_ON;
			//		case 7: return READ(E0_MAX_PIN)==REF_ON;
			//		case 8: return READ(E1_MIN_PIN)==REF_ON;
			//		case 9: return READ(E1_MAX_PIN)==REF_ON;
	}
	return false;
}

////////////////////////////////////////////////////////

bool  CStepperRampsFD::IsAnyReference()
{
	return	(_useReference[0] && READ(X_MIN_PIN) == REF_ON) ||
		(_useReference[1] && READ(X_MAX_PIN) == REF_ON) ||
		(_useReference[2] && READ(Y_MIN_PIN) == REF_ON) ||
		(_useReference[3] && READ(Y_MAX_PIN) == REF_ON) ||
		(_useReference[4] && READ(Z_MIN_PIN) == REF_ON) ||
		(_useReference[5] && READ(Z_MAX_PIN) == REF_ON);

}

////////////////////////////////////////////////////////

#endif