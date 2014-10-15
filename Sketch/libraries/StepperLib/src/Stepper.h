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
  http://www.gnu.org/licenses/
*/
////////////////////////////////////////////////////////

#pragma once

#include "ConfigurationStepperLib.h"
#include "HAL.h"
#include "RingBuffer.h"
#include "UtilitiesStepperLib.h"

////////////////////////////////////////////////////////
//
// assume step <==> mm
// a  ... acceleration
// d  ... distance in steps 
// v  ... velocity in steps/sec
// v0 ... velocity at start <=> a
// F  ... TimerFrequency in Hz (Arduino 1/8 CPU => 2MHZ)
// c  ... timer value (for step) in multiply of timerFrequency (NOT sec)
// v  ... (1 / F) * c
//
////////////////////////////////////////////////////////

class CStepper : public CSingleton<CStepper>
{
public:
	/////////////////////

	enum EStepMode
	{
		FullStep = 1,
		HalfStep = 2,
		QuarterStep = 4,
		EighthStep = 8,
		SixteenStep = 16,
		ThirtytwoStep = 32,
		SixtyfourStep = 64
	};

	enum EWaitType
	{
		MovementQueueFull,
		WaitBusyCall,

		WaitTimeCritical,										// TimeCritical wait types start here
		WaitReference
	};

	enum EStepperEvent
	{
		OnStartEvent,
		OnIdleEvent,
		OnDisableEvent,					// Disable stepper if inactive
		OnWaitEvent,
		OnErrorEvent,
		OnWarningEvent,
		OnInfoEvent
	};

	#define LevelToProcent(a) (a*100/255)
	#define ProcentToLevel(a) (a*255/100)

	enum ELevel
	{	
		LevelMax=255,
		Level20P = ProcentToLevel(20),
		Level60P = ProcentToLevel(60),
		LevelOff = 0
	};

	typedef bool(*StepperEvent)(CStepper*stepper, void* param, EnumAsByte(EStepperEvent) eventtype, void* addinfo);
	typedef bool(*TestContinueMove)(void* param);

	/////////////////////

protected:

	CStepper();

public:

	virtual void Init();

	bool IsError()												{ return _pod._error != NULL; };
	const __FlashStringHelper * GetError()						{ return _pod._error; }
	void ClearError()											{ _pod._error = NULL; }

	void SetMaxSpeed(axis_t axis, steprate_t vMax)				{ _pod._timerMax[axis] = SpeedToTimer(vMax); }
	void SetAcc(axis_t axis, steprate_t v0Acc)					{ _pod._timerAcc[axis] = SpeedToTimer(v0Acc); }
	void SetDec(axis_t axis, steprate_t v0Dec)					{ _pod._timerDec[axis] = SpeedToTimer(v0Dec); }
	void SetAccDec(axis_t axis, steprate_t v0Acc, steprate_t v0Dec) { SetAcc(axis, v0Acc); SetDec(axis, v0Dec); }

	void SetDefaultMaxSpeed(steprate_t vMax)					{ _pod._timerMaxDefault = SpeedToTimer(vMax); }
	void SetDefaultMaxSpeed(steprate_t vMax, steprate_t v0Acc, steprate_t v0Dec)				{ SetDefaultMaxSpeed(vMax); for (axis_t i = 0; i < NUM_AXIS; i++) { SetAcc(i, v0Acc); SetDec(i, v0Dec); } }
	void SetDefaultMaxSpeed(steprate_t vMax, axis_t axis, steprate_t v0Acc, steprate_t v0Dec)	{ SetDefaultMaxSpeed(vMax); SetAccDec(axis, v0Acc, v0Dec); }

	void SetUsual(steprate_t vMax);

	void SetJerkSpeed(axis_t axis, steprate_t vMaxJerk)			{ _pod._maxJerkSpeed[axis] = vMaxJerk; }
	void SetStepMode(axis_t axis, EnumAsByte(EStepMode) stepMode){ _pod._stepMode[axis] = stepMode; };

	void SetWaitFinishMove(bool wait)                           { _pod._waitFinishMove = wait; };
	bool IsWaitFinishMove() const								{ return _pod._waitFinishMove; }

	void SetCheckForReference(bool check)                       { _pod._checkReference = check; };
	bool IsCheckForReference()  const							{ return _pod._checkReference; }

	void SetBacklash(steprate_t speed)			                { _pod._timerbacklash = SpeedToTimer(speed); };
	bool IsSetBacklash()  const									{ return ((timer_t)-1) != _pod._timerbacklash; }

	bool IsBusy()  const										{ return _pod._timerRunning; };
	void WaitBusy();

	bool CanQueueMovement()	 const								{ return !_movements._queue.IsFull(); }
	unsigned char QueuedMovements()	 const						{ return _movements._queue.Count(); }

	unsigned char GetEnableTimeout(axis_t axis) const			{ return _pod._timeOutEnable[axis]; }
	void SetEnableTimeout(axis_t axis, unsigned char sec) 		{ _pod._timeOutEnable[axis] = sec; }

#ifdef USESLIP
	void SetSlip(int dist[NUM_AXIS]);
#endif

	void SetLimitMax(axis_t axis, udist_t limit)				{ _pod._limitMax[axis] = limit; };
	void SetLimitMin(axis_t axis, udist_t limit)				{ _pod._limitMin[axis] = limit; };

	void SetBacklash(axis_t axis, mdist_t dist)					{ _pod._backlash[axis] = dist; }

	//////////////////////////////
	// shortcut

	void AbortMove();											// Abort all pendinge/current moves, no dec ramp
	void EmergencyStop()										{ _emergencyStop = true; AbortMove(); }
	bool IsEmergencyStop()										{ return _emergencyStop; }
	void EmergencyStopResurrect();

	void UseReference(unsigned char referneceid, bool use)		{ _pod._useReference[referneceid] = use; }
	bool IsUseReference(unsigned char referneceid)				{ return _pod._useReference[referneceid]; }
	debugvirtula bool MoveReference(axis_t axis, unsigned char referenceid, bool toMin, steprate_t vMax, sdist_t maxdist = 0, sdist_t distToRef = 0, sdist_t distIfRefIsOn = 0);
	void SetPosition(axis_t axis, udist_t pos);

	void MoveAbs(const udist_t d[NUM_AXIS], steprate_t vMax = 0);
	void MoveRel(const sdist_t d[NUM_AXIS], steprate_t vMax = 0);

	void MoveAbs(axis_t axis, udist_t d, steprate_t vMax = 0);
	void MoveRel(axis_t axis, sdist_t d, steprate_t vMax = 0);

	void MoveAbsEx(steprate_t vMax, unsigned short axis, udist_t d, ...);	// repeat axis and d until axis not in 0 .. NUM_AXIS-1
	void MoveRelEx(steprate_t vMax, unsigned short axis, sdist_t d, ...);	// repeat axis and d until axis not in 0 .. NUM_AXIS-1
	void Wait(unsigned int sec100);

	bool MoveUntil(TestContinueMove testcontinue, void*param);

	//////////////////////////////

	const udist_t* GetPositions() const							{ return _pod._calculatedpos; }
	void GetPositions(udist_t pos[NUM_AXIS]) const				{ memcpy(pos, _pod._calculatedpos, sizeof(_pod._calculatedpos)); }
	udist_t GetPosition(axis_t axis) const						{ return _pod._calculatedpos[axis]; }

	void GetCurrentPositions(udist_t pos[NUM_AXIS]) const		{ CCriticalRegion crit; memcpy(pos, _pod._current, sizeof(_pod._current)); }
	udist_t GetCurrentPosition(axis_t axis) const				{ CCriticalRegion crit; return (*((volatile udist_t*)&_pod._current[axis])); }

	udist_t GetLimitMax(axis_t axis) const						{ return _pod._limitMax[axis]; }
	udist_t GetLimitMin(axis_t axis) const						{ return _pod._limitMin[axis]; }

	mdist_t GetBacklash(axis_t axis) const						{ return _pod._backlash[axis]; }
	axisArray_t GetLastDirection() const						{ return _pod._lastdirection; }		// check for backlash

	steprate_t GetDefaultVmax() const							{ return TimerToSpeed(_pod._timerMaxDefault); }
	steprate_t GetAcc(axis_t axis) const						{ return TimerToSpeed(_pod._timerAcc[axis]); }
	steprate_t GetDec(axis_t axis) const						{ return TimerToSpeed(_pod._timerDec[axis]); }
	steprate_t GetJerkSpeed(axis_t axis) const					{ return _pod._maxJerkSpeed[axis]; }

	unsigned long GetTotalSteps() const							{ return _pod._totalSteps; }
	unsigned int GetTimerISRBuys() const						{ return _pod._timerISRBusy; }
	unsigned long IdleTime() const								{ return _pod._timerStartOrOnIdle; }

	void AddEvent(StepperEvent event, void* eventparam, StepperEvent& oldevent, void*& oldeventparam);

	////////////////////////////////////////////////////////

private:

	void QueueMove(const mdist_t dist[NUM_AXIS], const bool directionUp[NUM_AXIS], steprate_t vMax);
	void QueueWait(const mdist_t dist, steprate_t vMax);

	void StartTimer();
	void WaitCanQueue();

	long CalcNextPos(udist_t current, udist_t dist, bool directionUp)
	{
		if (directionUp) return (sdist_t)current + (sdist_t)dist;
		return (sdist_t)current - (sdist_t)dist;
	}

	inline void StepOut();
	inline void StartBackground();
	inline void FillStepBuffer();
	void Background();

	void GoIdle();
	void ContinueIdle();

protected:

	bool MoveUntil(unsigned char referenceId, bool referencevalue, unsigned short stabletime);

protected:

	void QueueAndSplitStep(const udist_t dist[NUM_AXIS], const bool directionUp[NUM_AXIS], steprate_t vMax);
	void QueueWait();

	debugvirtula void Step(bool isr);

	debugvirtula void OptimizeMovementQueue(bool force);


	////////////////////////////////////////////////////////

	timer_t GetTimer(mdist_t steps, timer_t timerstart);										// calc "speed" after steps with constant a (from v0 = 0)
	timer_t GetTimerAccelerating(mdist_t steps, timer_t timerv0, timer_t timerstart);			// calc "speed" after steps accelerating with constant a 
	timer_t GetTimerDecelerating(mdist_t steps, timer_t timer, timer_t timerstart);			// calc "speed" after steps decelerating with constant a 

	static mdist_t GetAccSteps(timer_t timer, timer_t timerstart);										// from v=0
	static mdist_t GetDecSteps(timer_t timer, timer_t timerstop)								{ return GetAccSteps(timer, timerstop); } // to v=0

	static mdist_t GetAccSteps(timer_t timer1, timer_t timer2, timer_t timerstart);				// from v1 to v2 (v1<v2)
	static mdist_t GetDecSteps(timer_t timer1, timer_t timer2, timer_t timerstop)				{ return GetAccSteps(timer2, timer1, timerstop); }

	static mdist_t GetSteps(timer_t timer1, timer_t timer2, timer_t timerstart, timer_t timerstop);		// from v1 to v2 (v1<v2 uses acc, dec otherwise)

	unsigned long GetAccelerationFromTimer(mdist_t timerV0);
	unsigned long GetAccelerationFromSpeed(steprate_t speedV0)									{ return GetAccelerationFromSpeed(SpeedToTimer(speedV0)); }

	timer_t SpeedToTimer(steprate_t speed) const;
	steprate_t TimerToSpeed(timer_t timer) const;

	static unsigned char GetStepMultiplier(timer_t timermax);

	void CallEvent(EnumAsByte(EStepperEvent) eventtype, void* addinfo)			{ if (_pod._event) _pod._event(this, _pod._eventparam, eventtype, addinfo); }

protected:

	//////////////////////////////////////////
	// often accessed members first => is faster
	// even size of struct and 2byte alignement

	static bool		_emergencyStop;

	struct POD														// POD .. Plane Old Daty Type => no Constructor => init with default value = 0
	{
		volatile bool	_timerRunning;
		bool			_checkReference;							// check for "IsReference" in ISR (while normal move)

		bool			_waitFinishMove;
		bool			_limitCheck;

		timer_t			_timerbacklash;								// -1 or 0 for temporary enable/disable backlash without setting _backlash to 0

		unsigned long  _totalSteps;									// total steps since start

		unsigned int _timerISRBusy;									// ISR while in ISR
		timer_t _timerMaxDefault;									// timervalue of vMax (if vMax = 0)

		udist_t _current[NUM_AXIS];									// update in ISR
		udist_t _calculatedpos[NUM_AXIS];							// calculated in advanced (use movement queue)

		bool _useReference[NUM_AXIS * 2];							// each axis min and max - used in ISR

		steprate_t _maxJerkSpeed[NUM_AXIS];							// immediate change of speed without ramp (in junction)

		timer_t _timerMax[NUM_AXIS];								// maximum speed of axis
		timer_t _timerAcc[NUM_AXIS];								// acc timer start
		timer_t _timerDec[NUM_AXIS];								// dec timer start

		udist_t _limitMin[NUM_AXIS];
		udist_t _limitMax[NUM_AXIS];

		mdist_t _backlash[NUM_AXIS];								// backlash of each axis (signed mdist_t/2)

		EnumAsByte(EStepMode) _stepMode[NUM_AXIS];					// fullstep, half, ...
		unsigned char _timeOutEnable[NUM_AXIS];						// enabletimeout in sec if no step (0.. disable, always enabled)

		unsigned long _timerStartOrOnIdle;							// timervalue if library start move or goes to Idle
		unsigned long _timerLastCheckEnable;						// timervalue if library start move or goes to Idle

		unsigned char _idleLevel;									// level if idle (0..100)
		axisArray_t	  _lastdirection;								// for backlash

		const __FlashStringHelper * _error;

		StepperEvent	_event;										// event to function
		void*			_eventparam;								// event to funktion-parameter

		unsigned char _timeEnable[NUM_AXIS];						// 0: active, do not turn off, else time to turn off

#ifdef USESLIP
		unsigned int _slipSum[NUM_AXIS];
		int _slip[NUM_AXIS];
#endif
	} _pod;

	struct SMovementState;

	/////////////////////////////////////////////////////////////////////
	// internal ringbuffer for movement optimization

	struct SMovement
	{
	protected:
		friend class CStepper;

		enum EMovementState
		{
			StateReadyMove = 1,									// ready for travel (not executing)
			StateReadyWait = 2,									// ready for none "travel" move (wait move) (not executing)

			StateUpAcc = 11,									// in start phase accelerate
			StateUpDec = 12,									// in start phase decelerate to vmax
			StateRun = 13,										// running (no acc and dec)
			StateDownDec = 14,									// in stop phase decelerate
			StateDownAcc = 15,									// in stop phase accelerate

			StateWait = 21,										// executing wait (do no step)

			StateDone = 0,										// finished
		};

		mdist_t _steps;											// total movement steps (=distance)

		EnumAsByte(EMovementState) _state;						// emums are 16 bit in gcc => force byte
		bool _backlash;											// move is backlash

		mdist_t	_distance_[NUM_AXIS];							// distance adjusted wiht stepmultiplier => use GetDistance(axis)

		DirCount_t _dirCount;
		DirCount_t _lastStepDirCount;

		timer_t _timerMax;										// timer for max requested speed
		timer_t _timerRun;										// copy of _ramp. => modify during rampcalc

		axis_t _upAxis;											// most acceleration/decelerating axis while "up" state
		axis_t _downAxis;										// most acceleration/decelerating axis while "down" state

		struct SRamp											// only modify in CCRiticalRegion
		{
			mdist_t _upSteps;									// steps needed for accelerating from v0
			mdist_t _downSteps;									// steps needed for decelerating to v0
			mdist_t _downStartAt;								// index of step to start with deceleration

			mdist_t _nUpOffset;									// offset of n rampe calculation(acc) 
			mdist_t _nDownOffset;								// offset of n rampe calculation(dec)

			timer_t _timerRun;
			timer_t _timerStart;								// start ramp with speed (tinmerValue)
			timer_t _timerStop;									// stop  ramp with speed (timerValue)

			void RampUp(SMovement* pMovement, timer_t timerRun, timer_t timerJunction);
			void RampDown(SMovement* pMovement, timer_t timerJunction);
			void RampRun(SMovement* pMovement);
		} _ramp;

		timer_t _timerEndPossible;								// timer possible at end of last movement
		timer_t _timerJunctionToPrev;							// used to calculate junction speed, stored in "next" step
		timer_t _timerMaxJunction;								// max possible junction speed, stored in "next" step

		stepperstatic CStepper* _pStepper;						// give access to stepper (not static if multiinstance)  

		timer_t GetUpTimerAcc()									{ return _pStepper->_pod._timerAcc[_upAxis]; }
		timer_t GetUpTimerDec()									{ return _pStepper->_pod._timerDec[_upAxis]; }

		timer_t GetDownTimerAcc()								{ return _pStepper->_pod._timerAcc[_downAxis]; }
		timer_t GetDownTimerDec()								{ return _pStepper->_pod._timerDec[_downAxis]; }

		timer_t GetUpTimer(bool acc)							{ return acc ? GetUpTimerAcc() : GetUpTimerDec(); }
		timer_t GetDownTimer(bool acc)							{ return acc ? GetDownTimerAcc() : GetDownTimerDec(); }

		mdist_t GetDistance(axis_t axis);
		unsigned char GetStepMultiplier(axis_t axis)			{ return (_dirCount >> (axis * 4)) % 8; }
		bool GetDirectionUp(axis_t axis)						{ return ((_dirCount >> (axis * 4)) & 8) != 0; }
		unsigned char GetMaxStepMultiplier();

		bool CanModify() const;									// can modify movement (ramp)

		void SetEndPossibleProcessing();

		void Ramp(SMovement*mvNext);

		void CalcMaxJunktionSpeed(SMovement*mvNext);

		bool AdjustJunktionSpeedT2H(SMovement*mvPrev, SMovement*mvNext);
		void AdjustJunktionSpeedH2T(SMovement*mvPrev, SMovement*mvNext);

		bool CalcNextSteps(bool continues);

	public:

		bool IsActiveMove() const								{ return IsReadyForMove() || IsProcessingMove(); }			// Ready from move or moving
		bool IsReadyForMove() const								{ return _state == StateReadyMove; }						// Ready for move but not started
		bool IsProcessingMove() const							{ return _state >= StateUpAcc || _state <= StateDownAcc; }	// Move is currently processed (in acc,run or dec)
		bool IsUpMove() const									{ return IsProcessingMove() && _state < StateRun; }			// Move in ramp acc state
		bool IsDownMove() const									{ return IsProcessingMove() && _state > StateRun; }			// Move in ramp dec state
		bool IsFinished() const									{ return _state == StateDone; }								// Move finished 

		void InitMove(CStepper*pStepper, SMovement* mvPrev, mdist_t steps, const mdist_t dist[NUM_AXIS], const bool directionUp[NUM_AXIS], timer_t timerMax);
		void InitWait(CStepper*pStepper, mdist_t steps, timer_t timer);

		void SetBacklash()										{ _backlash = true; }

		void Dump(unsigned char queueidx, unsigned char options);

#ifdef _MSC_VER
		char _mvMSCInfo[MOVEMENTINFOSIZE];
#endif

	};

	struct SMovements
	{
		timer_t _timerStartPossible;							// timer for fastest possible start (break at the end)
		CRingBufferQueue<SMovement, MOVEMENTBUFFERSIZE>	_queue;
	};

	stepperstatic struct SMovements _movements;

	/////////////////////////////////////////////////////////////////////
	// internal state of move (used in ISR)

	struct SMovementState
	{
	protected:
		friend class CStepper;

		// state for calculating steps (moving)
		// static for performance on arduino => only one instance allowed

		mdist_t _n;				// step within movement (1-_steps)
		timer_t _timer;			// current timer
		timer_t _rest;			// rest of rampcalculation

		unsigned char _count;	// increment of _n
		char _dummyalign;

		unsigned long _sumTimer;	// for debug

		mdist_t _add[NUM_AXIS];

		void Init(SMovement* pMovement);

		inline bool CalcTimerAcc(timer_t maxtimer, mdist_t n, unsigned char cnt)
		{
			if (maxtimer < _timer)
			{
				mudiv_t udivremainer = mudiv(_timer*(2 * cnt) + _rest, n * 4 + 2 - cnt);
				_rest = udivremainer.rem;
				_timer = _timer - udivremainer.quot;
				if (maxtimer >= _timer)
				{
					_timer = maxtimer;
					return true;
				}
			}
			return false;
		}

		inline bool CalcTimerDec(timer_t mintimer, mdist_t n, unsigned char cnt)
		{
			if (mintimer > _timer)
			{
				if (n <= 1)
				{
					_timer = mintimer;
					return true;
				}
				mudiv_t udivremainer = mudiv(_timer*(2 * cnt) + _rest, n * 4 - 1 - cnt);
				_rest = udivremainer.rem;
				_timer = _timer + udivremainer.quot;
				if (mintimer <= _timer)
				{
					_timer = mintimer;
					return true;
				}
			}
			return false;
		}

	public:

		void Dump(unsigned char options);
	};

	stepperstatic SMovementState _movementstate;

	/////////////////////////////////////////////////////////////////////
	// internal ringbuffer for steps (after calculating acc and dec)

	struct SStepBuffer
	{
	public:
		DirCount_t DirStepCount;								// direction and count
		timer_t	Timer;
#ifdef _MSC_VER
		mdist_t  _distance[NUM_AXIS];							// to calculate relative speed
		mdist_t  _steps;
		SMovement::EMovementState  _state;
		mdist_t  _n;
		unsigned char _count;
		char _spMSCInfo[MOVEMENTINFOSIZE];
#endif
		void Init(DirCount_t dirCount)
		{
			Timer = 0;
			DirStepCount = dirCount;
		};
	};

	stepperstatic CRingBufferQueue<SStepBuffer, STEPBUFFERSIZE>	_steps;

public:

#ifdef _MSC_VER
	const char* MSCInfo;
#endif

	//////////////////////////////////////////

protected:

	debugvirtula void OnIdle(unsigned long idletime);				// called in ISR
	debugvirtula void OnWait(EnumAsByte(EWaitType) wait);			// wait for finish move or movementqueue full
	debugvirtula void OnStart();										// startup of movement

	debugvirtula void OnError(const __FlashStringHelper * error);
	debugvirtula void OnWarning(const __FlashStringHelper * warning);
	debugvirtula void OnInfo(const __FlashStringHelper * info);

	void Error(const __FlashStringHelper * error)				{ _pod._error = error; OnError(error); }
	void Info(const __FlashStringHelper * info)					{ OnInfo(info); }
	void Warning(const __FlashStringHelper * warning)			{ OnWarning(warning); }

public:

	unsigned char ToReferenceId(axis_t axis, bool minRef)		{ return axis * 2 + (minRef ? 0 : 1); }

	virtual bool  IsAnyReference() = 0;
	virtual bool  IsReference(unsigned char referenceid) = 0;

	enum EDumpOptions		// use bit
	{
		DumpAll = 0xff,
		DumpPos = 1,
		DumpState = 2,
		DumpMovements = 8,
		DumpDetails = 128											// detail of each option
	};

	void Dump(unsigned char options);							// options ==> EDumpOptions with bits

	void SetEnableAll(unsigned char level);				// level 0-255
	virtual void SetEnable(axis_t axis, unsigned char level, bool force) = 0;
	virtual unsigned char GetEnable(axis_t axis) = 0;

	static unsigned char ConvertLevel(bool enable)				{ return enable ? (unsigned char)(LevelMax) : (unsigned char)(LevelOff); }

protected:

	bool  MoveAwayFromReference(axis_t axis, unsigned char referenceid, sdist_t dist, steprate_t vMax);
	virtual void MoveAwayFromReference(axis_t axis, sdist_t dist, steprate_t vMax)							{ MoveRel(axis, dist, vMax); };

#ifdef _MSC_VER
	virtual void  StepBegin(const SStepBuffer* step) { step; };
	virtual void  StepEnd() {};
#endif

	virtual void  Step(const unsigned char steps[NUM_AXIS], axisArray_t directionUp) = 0;

private:

	void InitMemVar();

	////////////////////////////////////////////////////////
	// HAL

protected:

	debugvirtula void InitTimer()								{ CHAL::InitTimer1(HandleInterrupt); }
	debugvirtula void RemoveTimer()								{ CHAL::RemoveTimer1(); }

	debugvirtula void StartTimer(timer_t timerB);
	debugvirtula void SetIdleTimer();
	debugvirtula void StopTimer();

	static void HandleInterrupt()								{ GetInstance()->Step(true); }
	static void HandleBackground()								{ GetInstance()->Background(); }

	//////////////////////////////////////////
};
