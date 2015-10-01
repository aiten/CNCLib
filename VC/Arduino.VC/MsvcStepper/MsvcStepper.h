////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/
////////////////////////////////////////////////////////

#pragma once

#define _CRT_SECURE_NO_WARNINGS

#include <stdio.h>
#include <memory.h>

#include "..\..\..\sketch\libraries\StepperLib\src\StepperLib.h"
#include "..\..\..\sketch\libraries\CNCLib\src\MessageCNCLib.h"
#include "..\..\..\sketch\libraries\CNCLib\src\GCodeParserBase.h"

#define _STORETIMEVALUES	100000
#define NUM_AXIS_MVC		5

class CMsvcStepper : public CStepper
{

public:

	CMsvcStepper();

	virtual void OnIdle(unsigned long idletime) override;
	virtual void OnStart() override;
	virtual void OnWait(EnumAsByte(EWaitType) wait) override;

	virtual void Init() override;

	virtual bool  IsReference(unsigned char referenceId) override;
	virtual bool  IsAnyReference() override { return IsReference(0); };

	void MoveRel3(sdist_t dX, sdist_t dY, sdist_t dZ, steprate_t vMax = 0)	{ MoveRelEx(vMax, X_AXIS, dX, Y_AXIS, dY, Z_AXIS, dZ, -1); }
	void MoveAbs3(udist_t X, udist_t Y, udist_t Z, steprate_t vMax = 0)		{ MoveAbsEx(vMax, X_AXIS, X, Y_AXIS, Y, Z_AXIS, Z, -1); }

protected:

	virtual void  StepBegin(const SStepBuffer* step) override;
	virtual void  Step(const unsigned char steps[NUM_AXIS], axisArray_t directionUp) override;
	virtual void  StepRequest(bool isr) override;

	virtual void  SetEnable(axis_t axis, unsigned char level, bool /* force */) override { _level[axis] = level; };
	virtual unsigned char GetEnable(axis_t axis) override		{ return _level[axis]; }

public:
	
	// function for testing purpose (make public)

	struct SMovementX
	{
		SMovement mv;
	};

	SMovementX GetMovement(unsigned char idxfromhead) 
	{
		SMovementX mv;
		idxfromhead = _movements._queue.NextIndex(_movements._queue.GetHeadPos(), idxfromhead);
		mv.mv = _movements._queue.Buffer[idxfromhead];
		return mv;
	}
	unsigned char GetMovementCount()
	{
		return _movements._queue.Count();
	}

private:

	unsigned char _level[NUM_AXIS];

public:

	virtual void StartTimer(timer_t timerB) override;		// 0 => set idle timer (==Timer not running)
	virtual void SetIdleTimer() override;					// set idle Timer
	virtual void StopTimer() override;					// to cancel all timer

	virtual void OptimizeMovementQueue(bool force)  override;
	virtual bool MoveReference(axis_t axis, unsigned char referenceid, bool toMin, steprate_t vMax, sdist_t maxdist, sdist_t distToRef, sdist_t distIfRefIsOn)  override;

	// Test extensions

	void InitTest(const char* filename=NULL);
	void EndTest(const char* filename=NULL);

	bool DelayOptimization;
	bool SplitFile;
	bool UseSpeedSign;
	int  CacheSize;

private:

	void WriteTestResults(const char* filename);

	const char* _filename;
	int _flushcount;

	struct STimerEvent
	{
		timer_t TimerValues;
		int Steps;
		int Count;
		struct SAxis
		{
			int Multiplier;
			int MoveAxis;
			int Distance;
		} Axis[NUM_AXIS_MVC];
		char MSCInfo[MOVEMENTINFOSIZE];
	};

	int _TotalSteps;

	void InitCache()
	{
		_eventIdx = 0;
		memset(_TimerEvents, 0, sizeof(_TimerEvents));
	}

	int _exportIdx;
	int _eventIdx;
	STimerEvent* _TimerEvents;
	int _oldCacheSize;

	int	_sumtime[NUM_AXIS_MVC];
	int _count[NUM_AXIS_MVC];
	int _total[NUM_AXIS_MVC];
	char _speed[NUM_AXIS_MVC][20];
	long long _totaltime;
	int _lasttimer;

	int _refMovestart;

	bool _isReferenceMove;
	unsigned char _isReferenceId;
	int  _referenceMoveSteps;

public:

	void DoISR();
};

