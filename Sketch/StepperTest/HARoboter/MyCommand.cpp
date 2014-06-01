#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include "Settings.h"
#include <HAStepper.h>

#include "Global.h"
#include "MyCommand.h"

////////////////////////////////////////////////////////////

CMyCommand::CMyCommand()
{
}

////////////////////////////////////////////////////////////

void CMyCommand::SetSpeed(SSettings::SSPEED& speed)
{
	speed.max=_x;speed.acc=_y;speed.dec=_z;
}

////////////////////////////////////////////////////////////

void CMyCommand::GoToReference(axis_t axis, sdist_t maxdist, sdist_t distToRef, sdist_t distIfRefIsOn, unsigned char referenceid, udist_t setpos, steprate_t vmax)
{
  Stepper.UseReference(referenceid,true);  
  Stepper.MoveReference(axis,maxdist,distToRef,distIfRefIsOn,referenceid,vmax);
  Stepper.UseReference(referenceid,false);  
  Stepper.SetPosition(axis,setpos);
}

void CMyCommand::GoToReference(axis_t axis)
{
  switch (axis)
  {
	case 0: GoToReference(0, -100000,200,110000, 0, 100000,10000); break;
	case 1: GoToReference(1, -100000,200,110000, 1, 0,10000); break;
	case 2: GoToReference(2, -10000,100,1000,    2, 0,1000);break;
  }
}

////////////////////////////////////////////////////////////
  
bool CMyCommand::Command(char* xbuffer)
{
	unsigned char b1=0,b2=0,b3=0,b4=0;
	int xpos = Stepper.GetPosition(X_AXIS);
	int ypos = Stepper.GetPosition(Y_AXIS);
	int zpos = Stepper.GetPosition(Z_AXIS);

        static  udist_t _storedpos[20][NUM_AXIS] = { { 0,0,0,0,0 }, {0 } };
        static  sdist_t _pos0s[NUM_AXIS] = { 0 };
        static  udist_t _pos0[NUM_AXIS] = { 0 };

        static unsigned char _mpos=0;

	_buffer = xbuffer;
	SkipSpaces();

	// ha extentsion
	if (IsCommand("s ",false)) {	ScanOldParam(); Stepper.SetDefaultMaxSpeed(DefU16(_x,4500),DefU16(_y,200),DefU16(_z,250)); return true; }

	if (IsCommand("r ",false)) {	ScanOldParam(); Stepper.MoveRel(_d);			return true; }
	if (IsCommand("r1",false) || IsCommand("rx",false)) {	ScanOldParam(); Stepper.MoveRel(0,_x,0);		return true; }
	if (IsCommand("r2",false) || IsCommand("ry",false)) {	ScanOldParam(); Stepper.MoveRel(1,_x,0);		return true; }
	if (IsCommand("r3",false) || IsCommand("rz",false)) {	ScanOldParam(); Stepper.MoveRel(2,_x,0);		return true; }
	if (IsCommand("r4",false) || IsCommand("re",false)) {	ScanOldParam(); Stepper.MoveRel(3,_x,0);		return true; }
	if (IsCommand("r5",false) || IsCommand("rf",false)) {	ScanOldParam(); Stepper.MoveRel(4,_x,0);		return true; }

	if (IsCommand("a ",false)) { ScanOldParam(); if (_x!=0 || _y!=0) Stepper.MoveAbs(_ud);			        return true; }
	if (IsCommand("a1",false) || IsCommand("x ",false)) {	ScanOldParam(); Stepper.MoveAbs(0,_x,0);		return true; }
	if (IsCommand("a2",false) || IsCommand("y ",false)) {	ScanOldParam(); Stepper.MoveAbs(1,_x,0);		return true; }
	if (IsCommand("a3",false) || IsCommand("z ",false)) {	ScanOldParam(); Stepper.MoveAbs(2,_x,0);		return true; }
	if (IsCommand("a4",false) || IsCommand("e0",false)) {	ScanOldParam(); Stepper.MoveAbs(3,_x,0);		return true; }
	if (IsCommand("a5",false) || IsCommand("e1",false)) {	ScanOldParam(); Stepper.MoveAbs(4,_x,0);		return true; }

	//if (IsCommand("a ",false)) {	ScanOldParam(); Stepper.MoveAbs(_ud);			return true; }
	if (IsCommand("p1",false) || IsCommand("px",false)) {	ScanOldParam(); Stepper.SetPosition(0,_x);		return true; }
	if (IsCommand("p2",false) || IsCommand("py",false)) {	ScanOldParam(); Stepper.SetPosition(1,_x);		return true; }
	if (IsCommand("p3",false) || IsCommand("pz",false)) {	ScanOldParam(); Stepper.SetPosition(2,_x);		return true; }
	if (IsCommand("p4",false) || IsCommand("pe",false)) {	ScanOldParam(); Stepper.SetPosition(3,_x);		return true; }
	if (IsCommand("p5",false) || IsCommand("pf",false)) {	ScanOldParam(); Stepper.SetPosition(4,_x);		return true; }

	if (IsCommand("ix", false)) { ScanOldParam(); GoToReference(0);            return true; }
	if (IsCommand("iy", false)) { ScanOldParam(); GoToReference(1);            return true; }
	if (IsCommand("iz", false)) { ScanOldParam(); GoToReference(2);            return true; }
	if (IsCommand("i!", false)) { ScanOldParam(); 
                                                      if (Stepper.IsReference(0)) // is between 0 ... 1000000 
                                                        GoToReference(0);
                                                      GoToReference(1);
                                                      GoToReference(0);
                                                      GoToReference(2);        
                                                      return true; }

  	if (IsCommand("X ",false)) {	ScanOldParam(); Stepper.MoveAbs(0,Stepper.GetLimitMax(0)-5,0);
                                                        Stepper.MoveAbs(0,0,0);
                                                      	return true; }
  	if (IsCommand("XY",false)) {	ScanOldParam(); Stepper.MoveAbsEx(0,0,Stepper.GetLimitMax(0)-5,1,Stepper.GetLimitMax(1)-5,255);
                                                        Stepper.MoveAbsEx(0,0,(long)0,1,(long)0,255);
                                                      	return true; }
  	if (IsCommand("XZ",false)) {	ScanOldParam(); Stepper.MoveAbsEx(0, 0,Stepper.GetLimitMax(0)-5,
                                                                             1,Stepper.GetLimitMax(1)-5,
                                                                             2,Stepper.GetLimitMax(2)-5,
                                                                             255);
                                                        Stepper.MoveAbsEx(0,0,(long)0,1,(long)0,2,(long)0,255);
                                                      	return true; }
  	if (IsCommand("Y ",false)) {	ScanOldParam(); Stepper.MoveAbs(1,Stepper.GetLimitMax(1)-5,0);
                                                        Stepper.MoveAbs(1,0,0);
                                                      	return true; }
  	if (IsCommand("Z ",false)) {	ScanOldParam(); Stepper.MoveAbs(2,Stepper.GetLimitMax(2)-5,0);
                                                        Stepper.MoveAbs(2,0,0);
                                                      	return true; }
  	if (IsCommand("E0",false)) {	ScanOldParam(); Stepper.MoveAbs(3,Stepper.GetLimitMax(3)-5,0);
                                                        Stepper.MoveAbs(3,0,0);
                                                      	return true; }
  	if (IsCommand("E1",false)) {	ScanOldParam(); Stepper.MoveAbs(4,Stepper.GetLimitMax(4)-5,0);
                                                        Stepper.MoveAbs(4,0,0);
                                                      	return true; }

	if (IsCommand("mc",false))  {	ScanOldParam(); _mpos=0; return true; }
	if (IsCommand("m-",false))  {	ScanOldParam(); if (_mpos>0) _mpos--; return true; }
	if (IsCommand("m+",false))  {	ScanOldParam(); Stepper.GetPositions(_storedpos[_mpos++]); return true; }
	if (IsCommand("mr",false))  {	ScanOldParam(); Stepper.MoveAbs(_storedpos[_x]);  return true; }
	if (IsCommand("mg",false))  {	ScanOldParam(); for (register unsigned char i=0;i<_mpos;i++) Stepper.MoveAbs(_storedpos[i]);  return true; }

/*
  	if (IsCommand("w ",false)) {	ScanOldParam(); _ud[0] = Stepper.GetLimitMax(0)-5;
                                                        _ud[1] = Stepper.GetLimitMax(1)-5;
                                                        _ud[2] = Stepper.GetLimitMax(2)-5;
                                                        _ud[3] = Stepper.GetLimitMax(3)-5;
                                                        _ud[4] = Stepper.GetLimitMax(4)-5;
                                                        Stepper.MoveAbs(_ud);
                                                        memset(_ud,0,sizeof(_ud));
                                                        Stepper.MoveAbs(_ud);
                                                      	return true; }
*/                                                      

	if (IsCommand("i ",false)) {	ScanOldParam(); Control.Init(true);	return true; }
//	if (IsCommand("rf",false)) {	ScanOldParam(); _stepper->MoveReference(x,y,z);		return 1; }
#ifdef USESLIP
	if (IsCommand("sl",false)) {	ScanOldParam(); Stepper.SetSlip(_d);		return true; }
#endif

	if (IsCommand("! ", false))
	{
                Stepper.AbortMove();
		return true;
	}


	if (IsCommand("- ", false))
	{
		StepperSerial.print(Stepper.GetCurrentPosition(0)); StepperSerial.print(F(":"));
		StepperSerial.print(Stepper.GetCurrentPosition(1)); StepperSerial.print(F(":"));
		StepperSerial.print(Stepper.GetCurrentPosition(2)); StepperSerial.print(F(":"));
		StepperSerial.print(Stepper.GetCurrentPosition(3)); StepperSerial.print(F(":"));
		StepperSerial.print(Stepper.GetCurrentPosition(4)); StepperSerial.println();

		ScanOldParam();
		return true;
	}

	if (IsCommand("?",false)) 
	{	
            Stepper.Dump(CStepper::DumpAll);
		ScanOldParam();
		return true;
	}

    return false;
}














