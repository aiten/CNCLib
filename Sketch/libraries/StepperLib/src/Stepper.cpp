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

#include <stdlib.h>
#include <string.h>

#include <arduino.h>
#include <ctype.h>

#include "Stepper.h"
#include "PushValue.h"
#include "UtilitiesStepperLib.h"

////////////////////////////////////////////////////////

CStepper::CStepper()
{
	_num_axis = NUM_AXIS;
}

////////////////////////////////////////////////////////

template<> CStepper* CSingleton<CStepper>::_instance = NULL;

////////////////////////////////////////////////////////

void CStepper::InitMemVar()
{
	register axis_t i;

	_pod = POD();		//POD init object with 0

	// look to ASM => more for() are faster an smaller

#if USESLIP  
	for (i=0;i<NUM_AXIS;i++)	_SlipSum[i]=0;
	for (i=0;i<NUM_AXIS;i++)	_Slip[i]=0;
#endif

	for (i = 0; i < NUM_AXIS; i++)	_pod._limitMax[i] = 0x00ffffff;	

	_pod._checkReference = true;
	_pod._timerbacklash = (timer_t)-1;

	_pod._limitCheck = true;
	_pod._idleLevel = LevelOff;

	_pod._speedoverride = SpeedOverride100P;

//	SetUsual(28000);	=> reduce size => hard coded
	SetDefaultMaxSpeed(28000, 350, 380);
	for (i = 0; i < NUM_AXIS; i++) { SetJerkSpeed(i, 1000); }

	for (i = 0; i<MOVEMENTBUFFERSIZE; i++) _movements._queue.Buffer[i]._state= SMovement::StateDone;

#ifdef _MSC_VER
	MSCInfo = "";
#endif

}

////////////////////////////////////////////////////////

void CStepper::SetUsual(steprate_t vMax)
{
	// with ProxonMF70
	// maxSteprate ca. 28000
	// acc & dec = 350
	// JerkSpeed = 1000

	const steprate_t defspeed = 28000;
	const steprate_t defacc   = 350;
	const steprate_t defdec   = 380;
	const steprate_t defjerk  = 1000;

	steprate_t jerk = (steprate_t)MulDivU32(vMax, defjerk, defspeed);
	unsigned long sqrt = _ulsqrt_round(vMax * 10000l / defspeed);

	steprate_t acc  = steprate_t(sqrt * defacc / 100l);  
	steprate_t dec  = steprate_t(sqrt * defdec / 100l);  

	// acc and dec must not be y 62 => this is to slow
	if (dec < 62) dec = 62;
	if (acc < 62) acc = 62;

	SetDefaultMaxSpeed(vMax,acc,dec);
	for (axis_t i = 0; i < NUM_AXIS; i++) { SetJerkSpeed(i,jerk); }
}

////////////////////////////////////////////////////////

void CStepper::Init()
{
	InitMemVar();
	InitTimer();
#if defined(__SAM3X8E__)
	CHAL::InitBackground(HandleBackground);
#endif

	GoIdle();

	SetEnableAll(LevelOff);
}

////////////////////////////////////////////////////////

void CStepper::AddEvent(StepperEvent event, void* eventparam, SEvent& oldevent)
{
	oldevent = _event;
	_event._event = event;
	_event._eventParam = eventparam;
}

////////////////////////////////////////////////////////

void CStepper::StartTimer(timer_t timer)
{
	_pod._timerRunning = true;
	CHAL::StartTimer1(timer);
}

////////////////////////////////////////////////////////

void CStepper::SetIdleTimer()
{
	CHAL::StartTimer1(IDLETIMER1VALUE);
	_pod._timerRunning = false;
}

////////////////////////////////////////////////////////

void CStepper::StopTimer()
{
	CHAL::StopTimer1();
	_pod._timerRunning = false;
}

////////////////////////////////////////////////////////

void CStepper::QueueMove(const mdist_t dist[NUM_AXIS], const bool directionUp[NUM_AXIS], timer_t timerMax, unsigned char stepmult)
{
	//DumpArray<mdist_t,NUM_AXIS>(F("QueueMove"),dist,false);
	//DumpArray<bool,NUM_AXIS>(F("Dir"),directionUp,false);
	//DumpType<timer_t>(F("tm"),timerMax,true);

	mdist_t steps = 0;

	axisArray_t directionmask = 0;
	axisArray_t direction = 0;
	axisArray_t mask = 1;

	for (unsigned char i = 0; i < NUM_AXIS; i++)
	{
		if (dist[i])
		{
			directionmask += mask;
			if (directionUp[i])
				direction += mask;
			if (steps < dist[i])
				steps = dist[i];
		}
			mask *= 2;
	}

	if (steps == 0)				// nothing to do
	{
#ifndef REDUCED_SIZE
		Info(MESSAGE_STEPPER_EmptyMoveSkipped);
#endif
		return;
	}

	steps *= stepmult;

	if (IsSetBacklash())
	{
		if ((_pod._lastdirection&directionmask) != direction)
		{
			mdist_t backlashdist[NUM_AXIS] = { 0 };

			mdist_t backlashsteps = 0;
			mask = 1;
			for (unsigned char i = 0; i < NUM_AXIS; i++)
			{
				if ((_pod._lastdirection&directionmask&mask) != (direction&mask) && dist[i] && _pod._backlash[i])
				{
					backlashdist[i] = _pod._backlash[i];
					if (backlashdist[i] > backlashsteps)
						backlashsteps = backlashdist[i];
				}
				mask *= 2;
			}

			if (backlashsteps)
			{
				// need backlash
#ifndef REDUCED_SIZE
				Info(MESSAGE_STEPPER_Backlash);
#endif
				WaitUntilCanQueue();

				_movements._queue.NextTail().InitMove(this, GetPrevMovement(_movements._queue.GetNextTailPos()), backlashsteps, backlashdist, directionUp, _pod._timerbacklash);
				_movements._queue.NextTail().SetBacklash();
			
				EnqueuAndStartTimer(false);
			}
		}
	}

	// set all bits in lastdirection where axes moves
	_pod._lastdirection &= ~directionmask;
	_pod._lastdirection += direction;

	// wait until free movement buffer

	WaitUntilCanQueue();

	_movements._queue.NextTail().InitMove(this, GetPrevMovement(_movements._queue.GetNextTailPos()), steps, dist, directionUp, timerMax);

	EnqueuAndStartTimer(true);
}

////////////////////////////////////////////////////////

void CStepper::QueueWait(const mdist_t dist, timer_t timerMax, bool checkWaitConditional)
{
	WaitUntilCanQueue();
	_movements._queue.NextTail().InitWait(this, dist, timerMax, checkWaitConditional);

	EnqueuAndStartTimer(true);
}

////////////////////////////////////////////////////////

void CStepper::WaitUntilCanQueue()
{
	while (_movements._queue.IsFull())
	{
		OnWait(MovementQueueFull);
	}
}

////////////////////////////////////////////////////////

bool CStepper::StartMovement()
{
	_movements._queue.Head().CalcNextSteps(false);
	if (_movements._queue.Head().IsFinished())
	{
		_movements._queue.Dequeue();
		return false;
	}
	return true;
}

////////////////////////////////////////////////////////

void CStepper::EnqueuAndStartTimer(bool waitfinish)
{
	_movements._queue.Enqueue();

	OptimizeMovementQueue(false);

	if (_pod._timerRunning)
	{
		// situation: wait for last interrupt, need recalc next step for stepbuffer
		CCriticalRegion crit;
		if (_movements._queue.Count()==1 && _steps.IsEmpty())
		{
			StartMovement();
		}
	}
	else
	{
		_pod._timerLastCheckEnable = _pod._timerStartOrOnIdle = millis();

		for (axis_t i = 0; i<NUM_AXIS; i++)
		{
			SetTimeoutAndEnable(i, _pod._timeOutEnable[i], CStepper::LevelMax, true);
/*
			_pod._timeEnable[i] = _pod._timeOutEnable[i];
			if (_pod._timeEnable[i] == 0 && GetEnable(i)!=CStepper::LevelMax)					// enabletimeout == 0 => always enabled, otherwise done in CalcNextSteps
//			if (_pod._timeEnable[i] == 0)
				SetEnable(i, CStepper::LevelMax, true);
*/
		}

		if (StartMovement())
		{
			OnStart();
			CCriticalRegion crit;
			StepRequest(false);
		}
		else
		{
			// empty move => startup failed
			GoIdle();
		}
	}

	if (waitfinish && IsWaitFinishMove())
	{
		WaitBusy();
	}
}

////////////////////////////////////////////////////////

void CStepper::SMovement::InitMove(CStepper*pStepper, SMovement* mvPrev, mdist_t steps, const mdist_t dist[NUM_AXIS], const bool directionUp[NUM_AXIS], timer_t timerMax)
{
	register axis_t i;

	// memset(this, 0, sizeof(SMovement)); => set al memvars!!!
	
	_pStepper = pStepper;
	_pod._move._timerMax = timerMax;

	_backlash = false;

	_steps = steps;
	memcpy(_distance_, dist, sizeof(_distance_));

#ifdef _MSC_VER
	strcpy_s(_mvMSCInfo, _pStepper->MSCInfo);
#endif

	// calculate relative speed for axis => limit speed for axis

	for (i = 0; i < NUM_AXIS; i++)
	{
		mdist_t d = dist[i];
		if (d)
		{
			unsigned long axistimer = MulDivU32(_pod._move._timerMax, _steps, d);
			if (axistimer < (unsigned long)pStepper->_pod._timerMax[i])
			{
				timerMax = (timer_t)MulDivU32(pStepper->_pod._timerMax[i], d, _steps);
				_pod._move._timerMax = max(timerMax, _pod._move._timerMax);
			}
		}
	}

	// and acc/dec values

	_pod._move._timerAcc = 0;
	_pod._move._timerDec = 0;

	for (i = 0; i < NUM_AXIS; i++)
	{
		mdist_t d = dist[i];
		if (d)
		{
			timer_t accdec = (timer_t) MulDivU32(pStepper->_pod._timerAcc[i], d, _steps);
			if (accdec > _pod._move._timerAcc)
				_pod._move._timerAcc = accdec;

			accdec = (timer_t) MulDivU32(pStepper->_pod._timerDec[i], d, _steps);
			if (accdec > _pod._move._timerDec)
				_pod._move._timerDec = accdec;
		}
	}

	// calculate StepMultiplier and adjust distance

	unsigned char maxMultiplier = CStepper::GetStepMultiplier(_pod._move._timerMax);
	_lastStepDirCount = 0;
	_dirCount = 0;

	if (maxMultiplier > 1)
	{
		mdist_t calcfullsteps = _steps / maxMultiplier;
		if (_steps % maxMultiplier == 0)
			calcfullsteps--;

		for (i = NUM_AXIS - 1;; i--)
		{
			unsigned char multiplier = maxMultiplier;
			unsigned char axisdiff = 0;
			if (_distance_[i])
			{
				if (calcfullsteps)
				{
					multiplier = (unsigned char)(_distance_[i] / calcfullsteps);	// should fit in unsinged char!
					if (multiplier > maxMultiplier)
						multiplier = maxMultiplier;
					if ((_distance_[i] % calcfullsteps) != 0 && multiplier < maxMultiplier)
						multiplier++;

					if (multiplier != maxMultiplier)
					{
						_distance_[i] = (mdist_t)MulDivU32(_distance_[i], maxMultiplier, multiplier);
					}
					else if (_distance_[i] != _steps)
					{
						// round up => avoid rounding problems
						if (_distance_[i] % multiplier)
							_distance_[i] = ((_distance_[i] / multiplier) + 1)*multiplier;
					}
#ifdef use16bit
					unsigned long distinit = _steps / maxMultiplier / 2;
					unsigned long distsum = ((unsigned long)_distance_[i]) * ((unsigned long)calcfullsteps);
					mdist_t s = (mdist_t)((distinit + distsum) / ((unsigned long)_steps) * multiplier);
#else
					unsigned long distinit = _steps / maxMultiplier / 2;
					uint64_t distsum = ((uint64_t)_distance_[i]) * ((uint64_t)calcfullsteps);
					mdist_t s = (mdist_t)((distinit + distsum) / ((uint64_t)_steps) * multiplier);
#endif
					axisdiff = (unsigned char)(dist[i] - s);		// must be in range 0..7
				}
				else
				{
					axisdiff = (unsigned char)dist[i];			// must be in range 0..7
				}
			}

			_lastStepDirCount *= 16;
			_lastStepDirCount += axisdiff;
			_dirCount *= 16;
			_dirCount += multiplier;

			if (directionUp[i])
			{
				_lastStepDirCount += 8;
				_dirCount += 8;
			}

			if (i == 0)
				break;
		}
	}
	else
	{
		for (i = NUM_AXIS - 1;; i--)
		{
			_dirCount *= 16;
			_dirCount += 1;
			if (directionUp[i])
				_dirCount += 8;

			if (i == 0)
				break;
		}
		_lastStepDirCount = _dirCount;
	}

	_pod._move._ramp._timerStart = GetUpTimerAcc();
	_pod._move._ramp._timerStop = GetUpTimerDec();
	_pod._move._timerRun = _pod._move._timerMax;

	if (_pod._move._timerRun > _pod._move._ramp._timerStart)
		_pod._move._ramp._timerStart = _pod._move._timerRun;

	if (_pod._move._timerRun > _pod._move._ramp._timerStop)
		_pod._move._ramp._timerStop = _pod._move._timerRun;

	_state = StateReadyMove;

	_pod._move._timerJunctionToPrev = (timer_t)-1;	// force optimization

	bool prevIsMove = mvPrev && mvPrev->IsActiveMove();
	if (prevIsMove)
	{
		CalcMaxJunktionSpeed(mvPrev);
		_pod._move._timerEndPossible = (timer_t)-1;
	}
	else
	{
		_pod._move._timerEndPossible = _pStepper->GetTimer(_steps, GetUpTimerAcc());
	}

	_pod._move._ramp.RampUp(this, _pod._move._timerRun, (timer_t)-1);
	_pod._move._ramp.RampDown(this, (timer_t)-1);
	_pod._move._ramp.RampRun(this);

	//pStepper->Dump(DumpAll);
}

////////////////////////////////////////////////////////

void CStepper::SMovement::InitStop(SMovement* mvPrev, timer_t timer, timer_t dectimer)
{
	// must be a copy off current (executing) move
	*this = *mvPrev;

	mvPrev->_steps = _pStepper->_movementstate._n;		// stop now

	_pod._move._timerDec = dectimer;

	mdist_t downstpes = CStepper::GetDecSteps(timer, dectimer);

	for (unsigned char i=0;i<NUM_AXIS;i++)
	{
		_distance_[i] = (mdist_t) RoundMulDivUInt(_distance_[i],downstpes,_steps);
	}

	_state = SMovement::StateReadyMove;

	_steps = downstpes;

	_pod._move._ramp._timerRun   = timer;

	_pod._move._ramp.RampUp(this,timer, timer);
	_pod._move._ramp.RampDown(this,(timer_t) -1);
}

////////////////////////////////////////////////////////

void CStepper::SMovement::InitWait(CStepper*pStepper, mdist_t steps, timer_t timer, bool checkWaitConditional)
{
	//this is no POD because of methode's => *this = SMovement();		
	memset(this, 0, sizeof(SMovement));	// init with 0

	_pStepper = pStepper;
	_steps = steps;
	_pod._wait._timer = timer;
	_pod._wait._checkWaitConditional = checkWaitConditional;

	_state = StateReadyWait;
}

void CStepper::SMovement::InitIoControl(CStepper*pStepper, unsigned char tool, unsigned short level)
{
	//this is no POD because of methode's => *this = SMovement();		
	memset(this, 0, sizeof(SMovement));	// init with 0

	_pStepper = pStepper;
	_pod._io._tool = tool;
	_pod._io._level = level;
	_state = StateReadyIo;
}

////////////////////////////////////////////////////////

mdist_t CStepper::SMovement::GetDistance(axis_t axis)
{
	if (_distance_[axis])
	{
		register unsigned char multiplier = GetStepMultiplier(axis);
		register unsigned char maxMultiplier = GetMaxStepMultiplier();
		if (multiplier != maxMultiplier)
		{
			return (mdist_t)MulDivU32(_distance_[axis], multiplier, maxMultiplier);
		}
	}
	return _distance_[axis];
}

////////////////////////////////////////////////////////

unsigned char CStepper::SMovement::GetMaxStepMultiplier()
{
	register DirCount_t count = _dirCount;
	register unsigned char maxmultiplier = 0;

	for (register unsigned char i = 0;; i++)
	{
		maxmultiplier = max(maxmultiplier, ((unsigned char)count) % 8);
		if (i == NUM_AXIS - 1)
			break;

		count /= 16;
	}
	return maxmultiplier;
}

////////////////////////////////////////////////////////

void CStepper::SMovement::SRamp::RampUp(SMovement* pMovement, timer_t timerRun, timer_t timerJunction)
{
	_timerRun = timerRun;
	timer_t timerAccDec = pMovement->GetUpTimerAcc();

	if (timerJunction >= timerAccDec) // check from v0=0
	{
		_timerStart = max(timerAccDec, _timerRun);
		_nUpOffset = 0;
		_upSteps = GetAccSteps(_timerRun, timerAccDec);
	}
	else
	{
		// start from v0 != 0
		_timerStart = timerJunction;
		if (_timerStart >= _timerRun)		// acc while start
		{
			_nUpOffset = GetAccSteps(_timerStart, timerAccDec);
			_upSteps = GetAccSteps(_timerRun, timerAccDec) - _nUpOffset;
		}
		else
		{
			_upSteps = CStepper::GetAccSteps(_timerRun, timerAccDec);
			timerAccDec = pMovement->GetUpTimerDec();
			_nUpOffset = GetDecSteps(_timerStart, timerAccDec);
			_upSteps = _nUpOffset - GetDecSteps(_timerRun, timerAccDec);
		}
	}
}

////////////////////////////////////////////////////////

void CStepper::SMovement::SRamp::RampDown(SMovement* pMovement, timer_t timerJunction)
{
	mdist_t steps = pMovement->_steps;
	timer_t timerAccDec = pMovement->GetDownTimerDec();
	if (timerJunction >= timerAccDec)
	{
		_timerStop = max(timerAccDec, _timerRun);	// to v=0
		_downSteps = CStepper::GetDecSteps(_timerRun, timerAccDec);
		_downStartAt = steps - _downSteps;
		_nDownOffset = 0;
	}
	else
	{
		_timerStop = timerJunction;

		if (_timerStop >= _timerRun)							// dec while stop
		{
			// shift down phase with _nDownOffset steps
			_nDownOffset = CStepper::GetDecSteps(_timerStop, timerAccDec);
			_downSteps = CStepper::GetDecSteps(_timerRun, timerAccDec);
			_downStartAt = steps - _downSteps + _nDownOffset;
		}
		else
		{
			timerAccDec = pMovement->GetDownTimerAcc();
			_nDownOffset = CStepper::GetAccSteps(_timerStop, timerAccDec);
			//_downStartAt = _steps - 2 - (_nDownOffset - CStepper::GetAccSteps(_timerRun,timerAccDec));
			_downStartAt = steps - (_nDownOffset - CStepper::GetAccSteps(_timerRun, timerAccDec));
		}
		_downSteps = steps - _downStartAt;
	}
}

////////////////////////////////////////////////////////

void CStepper::SMovement::SRamp::RampRun(SMovement* pMovement)
{
	mdist_t steps = pMovement->_steps;

	if (_upSteps > steps || steps - _upSteps < _downSteps)
	{
		// we cant reach vmax for this movement, cut plateau.
		// leave relation between up and down squared => constant a
		mdist_t toMany = _downSteps + _upSteps - steps;
		mdist_t subUp;

		if (_downSteps == 0)
		{
			subUp = toMany;
		}
		else if (_upSteps == 0)
		{
			subUp = 0;
		}
		else
		{
			timer_t upTimer = pMovement->GetUpTimer(_timerStart > _timerRun);
			timer_t downTimer = pMovement->GetUpTimer(_timerStop < _timerRun);

			unsigned long sqUp = (unsigned long)(upTimer)* (unsigned long)(upTimer);
			unsigned long sqDown = (unsigned long)(downTimer)* (unsigned long)(downTimer);

			sqUp /= 0x1000;		// may overrun => divide by 0x1000
			sqDown /= 0x1000;

			unsigned long sum = sqUp + sqDown;

			subUp = mdist_t(RoundMulDivU32(toMany, sqUp, sum));	// round
		}

		if (subUp > _upSteps || (toMany - subUp) > _downSteps)
		{
			subUp = _upSteps;
			// return false;	=> do not return in case of error, assume "valid" values!
		}

		_upSteps -= subUp;
		_downSteps -= toMany - subUp;

		if (_downSteps > steps)
		{
			_downSteps = steps;
			// return false;	=> do not return in case of error, assume "valid" values!
		}

		_downStartAt = steps - _downSteps;
	}
}

////////////////////////////////////////////////////////

bool CStepper::SMovement::Ramp(SMovement*mvNext)
{
#ifdef _MSC_VER
	assert(IsActiveMove());
	assert(mvNext == NULL || mvNext->IsActiveMove());
#endif
	if (!IsDownMove())
	{
		SRamp tmpramp = _pod._move._ramp;
		tmpramp.RampUp(this, _pod._move._timerRun, _pod._move._timerJunctionToPrev);
		tmpramp.RampDown(this,mvNext ? mvNext->_pod._move._timerJunctionToPrev : GetDownTimerDec());
		tmpramp.RampRun(this);

		CCriticalRegion crit;

		if (IsReadyForMove() ||
			(IsUpMove()  && _pStepper->_movementstate._n <  tmpramp._upSteps) ||		// in acc
		    (IsRunMove() && _pStepper->_movementstate._n <  tmpramp._downStartAt))		// in run
		{
			_pod._move._ramp = tmpramp;
			return true;
		}
	}
	return false;
}

////////////////////////////////////////////////////////
// drill down the junction speed if speed at junction point is not possible

void CStepper::SMovement::AdjustJunktionSpeedH2T(SMovement*mvPrev, SMovement*mvNext)
{
	if (!IsActiveMove()) return;						// Move became inactive by ISR or "WaitState"/"IoControl"

	if (mvPrev == NULL || IsRunOrDownMove())			// no prev or processing (can be if the ISR has switchted to the next move)
	{
		// first "now" executing move
		if (IsRunOrUpMove())
		{
			if (IsRunMove())
			{
				// Option(TODO): we could accelerate with "down" move, 
				_pod._move._timerEndPossible = _pod._move._ramp._timerRun;
			}
			else
			{
				// just continue accelerate to the end of the move
				_pod._move._timerEndPossible = _pStepper->GetTimerAccelerating(_steps, _pod._move._ramp._timerStart, GetUpTimerAcc());
			}
		}
		else
			_pod._move._timerEndPossible = _pod._move._ramp._timerStop;
	}
	else
	{
		_pod._move._timerEndPossible = _pStepper->GetTimerAccelerating(_steps, mvPrev->IsActiveMove() ? (mvPrev->IsProcessingMove() ? mvPrev->_pod._move._ramp._timerStop : mvPrev->_pod._move._timerEndPossible) : -1, GetUpTimerAcc());

		if (_pod._move._timerEndPossible > _pod._move._timerMax)
		{
			// not faster as required
			_pod._move._timerRun = max(_pod._move._timerEndPossible, _pod._move._timerRun);
		}
	}

	if (mvNext != NULL)
	{
#ifdef _MSC_VER
		assert(mvNext->IsActiveMove());
#endif
		// next element available, calculate junction speed
		mvNext->_pod._move._timerJunctionToPrev = max(mvNext->_pod._move._timerMaxJunction, max(_pod._move._timerEndPossible, mvNext->_pod._move._timerJunctionToPrev));
		_pod._move._timerEndPossible = mvNext->_pod._move._timerJunctionToPrev;
	}
	
	if (!Ramp(mvNext))
	{
		// modify of ramp failed => do not modify _pod._move._timerEndPossible
		_pod._move._timerEndPossible = _pod._move._ramp._timerStop;
		if (mvNext != NULL) mvNext->_pod._move._timerJunctionToPrev = _pod._move._ramp._timerStop;
	}
}

////////////////////////////////////////////////////////
// drill down the junction speed if speed at junction point is not possible
// return (Tail to Head) if entrie has changed, if not, all previous (to head) no change is required

bool CStepper::SMovement::AdjustJunktionSpeedT2H(SMovement*mvPrev, SMovement*mvNext)
{
	if (!IsActiveMove()) return !IsActiveIo();				// Move became inactive by ISR or "wait" move or ignore "IOControl

	if (mvNext == NULL)
	{
		// last element in queue, v(end) = 0, we have to stop
		_pStepper->_movements._timerStartPossible = _pStepper->GetTimer(_steps, GetDownTimerDec());
	}
	else
	{
		// calculate new speed at start of move
		// assume _timerStartPossible (of next move) at end
		_pStepper->_movements._timerStartPossible = _pStepper->GetTimerAccelerating(_steps, _pStepper->_movements._timerStartPossible, GetDownTimerDec());
	}

	if (mvPrev != NULL)
	{
		_pod._move._timerRun = _pod._move._timerMax;

		if (!mvPrev->IsActiveMove())
			return true;				// waitstate => no optimize, break here

		// prev element available, calculate junction speed
		timer_t junctiontoPrev = max(_pod._move._timerMaxJunction, _pStepper->_movements._timerStartPossible);
		if (junctiontoPrev == _pod._move._timerJunctionToPrev)
			return true;					// nothing changed (prev movementes do not change)

		_pod._move._timerJunctionToPrev = junctiontoPrev;
		_pStepper->_movements._timerStartPossible = _pod._move._timerJunctionToPrev;
	}

	return false;
}

////////////////////////////////////////////////////////
// calculate the max junction between two movements- consider jerk - speed is maxspeed - only calculated once (at setup time of movement)

void CStepper::SMovement::CalcMaxJunktionSpeed(SMovement*mvPrev)
{

#ifdef _MSC_VER
	assert(IsActiveMove());
	assert(mvPrev==NULL || mvPrev->IsActiveMove());
#endif

	// .1 => prev
	// .2 => next(this)

	// default => fastest move (no jerk)
	_pod._move._timerMaxJunction = min(mvPrev->_pod._move._timerMax, _pod._move._timerMax);
	timer_t timerMaxJunction;
	timer_t timerMaxJunctionAcc = mvPrev->GetUpTimerAcc();

	mdist_t s1 = mvPrev->_steps;
	mdist_t s2 = _steps;

	axis_t mainaxis;	// max moving axis	=> no jerk 

	for (mainaxis = 0; mainaxis < NUM_AXIS; mainaxis++)
	{
		if (s1 == mvPrev->GetDistance(mainaxis) && s2 == GetDistance(mainaxis) && mvPrev->GetDirectionUp(mainaxis) == GetDirectionUp(mainaxis))
		{
			_pod._move._timerMaxJunction = (long(mvPrev->_pod._move._timerMax) + long(_pod._move._timerMax)) / 2;
			break;
		}
	}

	timer_t timerMaxJunction_ = _pod._move._timerMaxJunction;
	/*
		printf("H: s1=%i s2=%i\n", (int)s1, (int)s2);
		for (int ii = 0; ii < NUM_AXIS; ii++)
		{
		printf("%i: Prev:%i%c\tThis:%i%c\n", ii, (int)mvPrev->GetDistance(ii), mvPrev->GetDirectionUp(ii) ? '+' : '-', (int)GetDistance(ii), GetDirectionUp(ii) ? '+' : '-');
		}
		printf("");

		if (mainaxis >= NUM_AXIS)
		{
		//_pod._move._timerMaxJunction = (long(mvPrev->_timerMax) + long(_timerMax)) / 2;
		//return;
		}
		*/
	for (axis_t i = 0; i < NUM_AXIS; i++)
	{
		if (i != mainaxis)
		{
			mdist_t d1 = mvPrev->GetDistance(i);
			mdist_t d2 = GetDistance(i);

			steprate_t v1 = _pStepper->TimerToSpeed(mvPrev->_pod._move._timerMax);
			steprate_t v2 = _pStepper->TimerToSpeed(_pod._move._timerMax);

			if (d1 != s1) v1 = steprate_t(RoundMulDivUInt(v1, d1, s1));
			if (d2 != s2) v2 = steprate_t(RoundMulDivUInt(v2, d2, s2));


			long vdiff;

			if (v1 == 0 || v2 == 0 || mvPrev->GetDirectionUp(i) == GetDirectionUp(i))
			{
				// same direction (v1 and v2 not 0)
				vdiff = v1 > v2 ? v1 - v2 : v2 - v1;

				if (vdiff > long(_pStepper->_pod._maxJerkSpeed[i]))
				{
					// reduce total speed by ratio maxJerk <=> current jerk
					timerMaxJunction = _pStepper->SpeedToTimer(steprate_t(RoundMulDivUInt(_pStepper->TimerToSpeed(timerMaxJunction_), _pStepper->_pod._maxJerkSpeed[i], steprate_t(vdiff))));
					_pod._move._timerMaxJunction = max(_pod._move._timerMaxJunction, min(timerMaxJunction, timerMaxJunctionAcc));
				}
			}
			else
			{
				// different direction, add speed
				vdiff = (long)(v1)+(long)(v2);

				if (mainaxis >= NUM_AXIS)
				{
					_pod._move._timerMaxJunction = timerMaxJunctionAcc;	//Stop and go
				}
				else
				{
					if (vdiff > long(_pStepper->_pod._maxJerkSpeed[i]))
					{
						// reduce total speed by ratio maxJerk <=> current jerk
						timerMaxJunction = _pStepper->SpeedToTimer(steprate_t(RoundMulDivUInt(_pStepper->TimerToSpeed(timerMaxJunction_), _pStepper->_pod._maxJerkSpeed[i], steprate_t(vdiff))));
						_pod._move._timerMaxJunction = max(_pod._move._timerMaxJunction, min(timerMaxJunction, timerMaxJunctionAcc));
					}
				}
			}
		}
	}
}

////////////////////////////////////////////////////////

CStepper::SMovement* CStepper::GetNextMovement(unsigned char idx)
{
	// get next movment which can be optimized (no IOControl)

	if (_movements._queue.IsEmpty()) return NULL;
	while (true)
	{
		idx = _movements._queue.H2TInc(idx);
		if (!_movements._queue.H2TTest(idx))
			return NULL;

		if (!_movements._queue.Buffer[idx].IsActiveIo())
			return &_movements._queue.Buffer[idx];
	}
	return _movements._queue.GetNext(idx);
}

////////////////////////////////////////////////////////

CStepper::SMovement* CStepper::GetPrevMovement(unsigned char idx)
{
	// get previous movment which can be optimized (no IOControl)

	if (_movements._queue.IsEmpty()) return NULL;
	while (true)
	{
		idx = _movements._queue.T2HInc(idx);
		if (!_movements._queue.T2HTest(idx))
			return NULL;

		if (!_movements._queue.Buffer[idx].IsActiveIo())
			return &_movements._queue.Buffer[idx];
	}
}

////////////////////////////////////////////////////////

void CStepper::OptimizeMovementQueue(bool /* force */)
{
	if (_movements._queue.IsEmpty() || _movements._queue.Count() < 2)
		return;

	unsigned char idx;
	unsigned char idxnochange = _movements._queue.H2TInit();

	////////////////////////////////////
	// calculate junction (max) speed!

	for (idx = _movements._queue.T2HInit(); _movements._queue.T2HTest(idx); idx = _movements._queue.T2HInc(idx))
	{
		if (_movements._queue.Buffer[idx].AdjustJunktionSpeedT2H(GetPrevMovement(idx), GetNextMovement(idx)))
		{
			idxnochange = idx;
			break;
		}
	}

	////////////////////////////////////
	// calculate junction (max) speed!
	// and calculate trapezoid ramp (as soon as possible)

	for (idx = idxnochange; _movements._queue.H2TTest(idx); idx = _movements._queue.H2TInc(idx))
	{
		_movements._queue.Buffer[idx].AdjustJunktionSpeedH2T(GetPrevMovement(idx), GetNextMovement(idx));
	}
}

////////////////////////////////////////////////////////

void CStepper::OnIdle(unsigned long idletime)
{
	CallEvent(OnIdleEvent);
	if (idletime > TIMEOUTSETIDLE)
	{
		for (unsigned char x = 0; x < _num_axis; x++)
		{
			if (GetEnable(x) != _pod._idleLevel)
			{
				SetEnableAll(_pod._idleLevel);
				CallEvent(OnDisableEvent);
			}
		}
	}
}

////////////////////////////////////////////////////////

void CStepper::OnStart()
{
	CallEvent(OnStartEvent);
}

////////////////////////////////////////////////////////

void CStepper::OnWait(EnumAsByte(EWaitType) wait)
{
	CallEvent(OnWaitEvent, (void*) (unsigned int) wait);
}

////////////////////////////////////////////////////////

void CStepper::OnError(const __FlashStringHelper * error)
{
	CallEvent(OnErrorEvent, (void*)error);
}

////////////////////////////////////////////////////////

void CStepper::OnWarning(const __FlashStringHelper * warning)
{
	CallEvent(OnWarningEvent, (void*) warning);
	StepperSerial.print(MESSAGE_WARNING);
	StepperSerial.println(warning);
}

////////////////////////////////////////////////////////

void CStepper::OnInfo(const __FlashStringHelper * info)
{
	CallEvent(OnInfoEvent, (void*)info);
	StepperSerial.print((MESSAGE_INFO));
	StepperSerial.println(info);
}

////////////////////////////////////////////////////////

void CStepper::WaitBusy()
{
	while (IsBusy())
	{
		// wait until finish all movements
		OnWait(WaitBusyCall);
	}
}

////////////////////////////////////////////////////////

unsigned long CStepper::GetAccelerationFromTimer(mdist_t timerV0)
{
	// original a = v/t, => a=s/t^2
	// for first step we need c0 => convert to sec, a = 1/t^2
	// a = (F/timer)^2

	unsigned long x = TIMER1FREQUENCE / timerV0;
	return x*x;

	//range: v0=100 => 10000
	//range: v0=400 => 159201
}

////////////////////////////////////////////////////////
// reverse calc n from timervalue

timer_t CStepper::GetTimer(mdist_t steps, timer_t timerstart)
{
	// original v = sqrt(v0^2 + 2 a d)
	// because of int estimation: use factor
	//
	// v = sqrt((v0^2 + 2 a d) / factor^2)
	// v0 = 0

	if (steps > MAXACCDECSTEPS)
		steps = MAXACCDECSTEPS;

	unsigned long a2 = 2 * GetAccelerationFromTimer(timerstart);

	if (ToPrecisionU2(a2) + ToPrecisionU2(steps) > 31)
		return TIMER1VALUEMAXSPEED;

	unsigned long ad = a2 * steps;
	steprate_t v = (steprate_t)(_ulsqrt(((ad) / 93) * 85));

	return SpeedToTimer(v) + 1;	// +1 	empiric tested to get better results
}

////////////////////////////////////////////////////////
// reverse calc n from timervalue

timer_t CStepper::GetTimerAccelerating(mdist_t steps, timer_t timerv0, timer_t timerstart)
{
	// original v = sqrt(v0^2 + 2 a d)
	// because of int estimation: use factor
	//
	// v = sqrt((v0^2 + 2 a d) / factor^2)

	if (steps > MAXACCDECSTEPS)
		steps = MAXACCDECSTEPS;

	unsigned long sqv0 = TimerToSpeed(timerv0);
	sqv0 *= sqv0;

	unsigned long a2 = 2 * GetAccelerationFromTimer(timerstart);

	if (ToPrecisionU2(a2) + ToPrecisionU2(steps) > 31)
		return TIMER1VALUEMAXSPEED;

	unsigned long ad = a2 * steps;
	steprate_t v = (steprate_t)(_ulsqrt(((sqv0 + ad) / 93) * 85));

	return SpeedToTimer(v) + 1;	// +1 	empiric tested to get better results
}

////////////////////////////////////////////////////////
// reverse calc n from timervalue

timer_t CStepper::GetTimerDecelerating(mdist_t steps, timer_t timerv, timer_t timerstart)
{
	// original v = sqrt(v0^2 + 2 a d)
	// because of int estimation: use factor
	//
	// v = sqrt((v0^2 + 2 a d) / factor^2)

	if (steps > MAXACCDECSTEPS)
		steps = MAXACCDECSTEPS;

	unsigned long sqv0 = TimerToSpeed(timerv);
	sqv0 *= sqv0;

	unsigned long a2 = 2 * GetAccelerationFromTimer(timerstart);

	if (ToPrecisionU2(a2) + ToPrecisionU2(steps) > 31)
		return TIMER1VALUEMAXSPEED;

	unsigned long ad = a2 * steps;
	if (sqv0 < ad)
	{
		return (timer_t)-1;
	}

	steprate_t v = (steprate_t)(_ulsqrt(((sqv0 - ad) / 93) * 85));

	return SpeedToTimer(v) + 1;	// +1 	empiric tested to get better results
}

////////////////////////////////////////////////////////
// reverse calc n from timervalue

mdist_t CStepper::GetAccSteps(timer_t timer, timer_t timerstart)
{
	// original: d = v^2 / v0^2
	// tested with execel to fit to timer calcualtion with cn = cn-1 + (2*cn-1) / (4n+1) and use of "integer"

	unsigned long sqA2 = (unsigned long)(timer)* (unsigned long)(timer - 1) * 2;
	unsigned long sqB = (unsigned long)(timerstart)* (unsigned long)(timerstart);

	// factor => tested with excel => timer *= 1.046 => timer^2 *= 1.0941 ( 1.046^2)
	// use int and not float => 93/85 = 1,094117647058824 

	// timerstart*timerstart * 93 must not overrun => sqrt(65536*95536*93) = 6795
	if (timerstart > 6795)	// avoid overrun
	{
		sqB /= 128;
		sqB = MulDivU32(sqB, 93, 85);
		sqB *= 128;
	}
	else
	{
		sqB = MulDivU32(sqB, 93, 85);
	}

	return (mdist_t)(sqB / sqA2);	// ceil
}

////////////////////////////////////////////////////////

mdist_t CStepper::GetAccSteps(timer_t timer1, timer_t timer2, timer_t timerstart)
{
	// timer1 = v1 (slower) => timer1 greater
	// timer2 = v2 (faster)

	if (timer1 < timer2)	// swap if wrong
	{
		timer_t tmp = timer1;
		timer1 = timer2;
		timer2 = tmp;
	}

	return GetAccSteps(timer2, timerstart) - GetAccSteps(timer1, timerstart);
}

////////////////////////////////////////////////////////

mdist_t CStepper::GetSteps(timer_t timer1, timer_t timer2, timer_t timerstart, timer_t timerstop)
{
	if (timer1 > timer2)
	{
		return GetAccSteps(timer1, timer2, timerstart);
	}
	return GetDecSteps(timer1, timer2, timerstop);
}

////////////////////////////////////////////////////////

void CStepper::StopMove(steprate_t v0Dec)
{
	if (_movements._queue.Count() > 0)
	{
		SMovement& mv = _movements._queue.Head();

		if (mv.IsActiveWait())
		{
			CCriticalRegion critical;
			_movementstate._n = mv._steps;
		}
		else
		{
			timer_t dectimer = v0Dec!=0 ? SpeedToTimer(v0Dec) : mv.GetDownTimerDec();

			{
				CCriticalRegion critical;

				// do nothing if move is about to finish
				if (mv.IsDownMove())
					return;

				// remove all not executed moves and create a new one for dec
				// start downramp now

				SubTotalSteps();

				_movements._queue.RemoveTail(_movements._queue.GetHeadPos());

				_movements._queue.NextTail().InitStop(&mv, _movementstate._timer,dectimer);

				_movements._queue.Enqueue();
			}

			WaitBusy();
			memcpy(_pod._calculatedpos, _pod._current, sizeof(_pod._calculatedpos));
		}
	}
}

////////////////////////////////////////////////////////

void CStepper::AbortMove()
{
	CCriticalRegion critical;

	SubTotalSteps();

	_steps.Clear();
	_movements._queue.Clear();

	memcpy(_pod._calculatedpos, _pod._current, sizeof(_pod._calculatedpos));

	GoIdle();
}

////////////////////////////////////////////////////////

void CStepper::PauseMove()
{
	if (_pod._pause == false)
	{
		_pod._pause = true;

#ifndef REDUCED_SIZE

		// insert into queue (where jerke speed to stop move will not exceed) 

		for (unsigned char idx= _movements._queue.H2TInit(); _movements._queue.H2TTest(idx); idx = _movements._queue.H2TInc(idx))
		{
			SMovement& mv = _movements._queue.Buffer[idx];

			bool canInsertAfter = !mv.IsActiveMove();

			if (!canInsertAfter)
			{
				// check jerk speed to stop

				steprate_t speedStop = TimerToSpeed(mv._pod._move._ramp._timerStop);
				mdist_t s = mv.GetSteps();

				canInsertAfter = true;

				for (axis_t x = 0; canInsertAfter && x < NUM_AXIS;x++)
				{
					mdist_t d = mv.GetDistance(x);
					steprate_t v = speedStop;

					if (d != s) v = steprate_t(RoundMulDivUInt(v, d, s));

					canInsertAfter = _pod._maxJerkSpeed[x] > v;
				}
			}

			if (canInsertAfter)
			{
				WaitUntilCanQueue();
				_movements._queue.InsertTail(_movements._queue.NextIndex(idx))->InitWait(this, 0xffff, WAITTIMER1VALUE, true);
				return;
			}
		}

#endif

		// just queue a stop at end of queue
		QueueWait(0xffff, WAITTIMER1VALUE, true);
	}
}

////////////////////////////////////////////////////////

void CStepper::ContinueMove()
{
	_pod._pause = false;
}

////////////////////////////////////////////////////////

void CStepper::SubTotalSteps()
{
#ifndef REDUCED_SIZE

	// sub all pending steps to _totalsteps

	for (unsigned char idx = _movements._queue.T2HInit(); _movements._queue.T2HTest(idx); idx = _movements._queue.T2HInc(idx))
	{
		SMovement& mv=_movements._queue.Buffer[idx];
		if (mv.IsActiveMove())
		{
			_pod._totalSteps -= mv._steps;
			if (mv.IsProcessingMove())
				_pod._totalSteps += _movementstate._n;
		}
	}

#endif
}

////////////////////////////////////////////////////////

void CStepper::EmergencyStopResurrect()
{
	AbortMove();		// make sure nothing is running
	_pod._emergencyStop = false;
	_pod._fatalerror = NULL;
}

////////////////////////////////////////////////////////

unsigned char CStepper::GetStepMultiplier(timer_t timermax)
{
	if (timermax >= TIMER1VALUE(SPEED_MULTIPLIER_2)) return 1;
	if (timermax >= TIMER1VALUE(SPEED_MULTIPLIER_3)) return 2;
	if (timermax >= TIMER1VALUE(SPEED_MULTIPLIER_4)) return 3;
	if (timermax >= TIMER1VALUE(SPEED_MULTIPLIER_5)) return 4;
	if (timermax >= TIMER1VALUE(SPEED_MULTIPLIER_6)) return 5;
	if (timermax >= TIMER1VALUE(SPEED_MULTIPLIER_7)) return 6;
	return 7;
}

////////////////////////////////////////////////////////

inline void CStepper::StepOut()
{
	// called in interrupt => must be "fast"
	// "Out" the Step to the stepper 
	 
	// calculate all axes and set PINS parallel - DRV 8225 requires 1.9us * 2 per step => sequential is to slow 

	DirCount_t dir_count;

	{
		const SStepBuffer* stepbuffer = &_steps.Head();
		StartTimer(stepbuffer->Timer - TIMEROVERHEAD);
		dir_count = stepbuffer->DirStepCount;
	}

#ifdef _MSC_VER
	StepBegin(&_steps.Head());
#endif
	// AVR: div with 256 is faster than 16 (loop shift)

	stepperstatic_avr unsigned char axescount[NUM_AXIS];
	axisArray_t directionUp = 0;

	unsigned char bytedircount = 0;
	bool countit = true;
	if (((DirCountByte_t*)&dir_count)->byte.byteInfo.nocount != 0)
		countit = false;

	for (register unsigned char i = 0;; i++)
	{
#if defined (__AVR_ARCH__)
		if (i % 2 == 1)
		{
			bytedircount = bytedircount / 16;
		}
		else
		{
			bytedircount = (unsigned char)dir_count; //  &255;
			dir_count /= 256;
		}
#else
		bytedircount = dir_count&15;
		dir_count /= 16;
#endif

		axescount[i] = bytedircount & 7;
		directionUp /=2;

		if (axescount[i])
		{
			if ((bytedircount&8) != 0)
			{
				directionUp += (1<<(NUM_AXIS-1));
				if (countit) _pod._current[i] += axescount[i];
			}
			else
			{
				if (countit) _pod._current[i] -= axescount[i];
			}
		}

		if (i == NUM_AXIS - 1)
			break;
	}

	Step(axescount,directionUp^_pod._invertdirection);

	_steps.Dequeue();
}

////////////////////////////////////////////////////////

static volatile bool _backgroundactive = false;

void CStepper::StartBackground()
{
#if defined(__SAM3X8E__)
	// sam3x cannot call timer interrupt nested.
	// we use a other ISR (CAN) as Tail-chaining with lower (priority value is higher) priority and exit the Timer ISR

	if (_backgroundactive)
	{
		_pod._timerISRBusy++;
	}
	else
	{
		CHAL::BackgroundRequest();
	}

#else

	static volatile unsigned char reentercount = 0;

	reentercount++;

	if (reentercount != 1)
	{
		// other ISR is calculating!
#ifndef REDUCED_SIZE
		_pod._timerISRBusy++;
#endif
		reentercount--;
		return;
	}

	// Reenable nested interrupts	=> usual EnableInterrupts
	CHAL::EnableInterrupts();

	// calculate next steps until buffer is full or nothing to do!
	// other timerIRQs may occur
	FillStepBuffer();

	CHAL::DisableInterrupts();
	reentercount--;

#endif
}

////////////////////////////////////////////////////////

void CStepper::FillStepBuffer()
{
	// calculate next steps until buffer is full or nothing to do!
	while (!_movements._queue.IsEmpty())
	{
		if (!_movements._queue.Head().CalcNextSteps(true))		// buffer full => wait (and leave ISR)
			break;

		if (_movements._queue.Head().IsFinished())
		{
			_movements._queue.Dequeue();
		}
	}

	// check if turn off stepper

	unsigned long ms = millis();
	unsigned char diff_sec = (unsigned char)((ms - _pod._timerLastCheckEnable) / 1024);		// div 1024 is faster as 1000
		
	if (diff_sec > 0)
	{
		_pod._timerLastCheckEnable = ms;

		for (axis_t i = 0;i<NUM_AXIS; i++)
		{
			if (_pod._timeEnable[i] != 0)
			{
				if (_pod._timeEnable[i] < diff_sec) // may overrun
					_pod._timeEnable[i] = 0;	
				else
					_pod._timeEnable[i] -= diff_sec;

				SetTimeoutAndEnable(i, _pod._timeEnable[i], _pod._idleLevel, true);
/*
				if (_pod._timeEnable[i] == 0 && GetEnable(i) != _pod._idleLevel)
				{
					CCriticalRegion crit;
					SetEnable(i, _pod._idleLevel, true);
				}
*/
			}
		}
	}
}

////////////////////////////////////////////////////////
// called as Tail-chaining on due (after Step() )
// due: do not check reenter => ISR on due can be only called once (not nested)

void CStepper::Background()
{
	_backgroundactive = true;
	FillStepBuffer();
	_backgroundactive = false;
}

////////////////////////////////////////////////////////

void CStepper::GoIdle()
{
	// start idle timer
	_pod._timerStartOrOnIdle = millis();
	SetIdleTimer();
	OnIdle(0);
}

////////////////////////////////////////////////////////

void CStepper::ContinueIdle()
{
	SetIdleTimer();
	OnIdle(millis() - _pod._timerStartOrOnIdle);
}

////////////////////////////////////////////////////////

void CStepper::StepRequest(bool isr)
{
	// called in interrupt => must be "fast"
	// first send commands to stepper driver
	// afterwards calculate the next steps in background

	// AVR:	 interrups are disabled (ISR) or disable: see OnStart
	// SAM3X:no nested call of ISR 

	if (isr  && !_pod._timerRunning)
	{
		ContinueIdle();
		return;
	}

	if (_steps.IsEmpty())
	{
		GoIdle();
		return;
	}

	if (_pod._emergencyStop)
	{
		AbortMove();
		return;
	}

	StepOut();

	if ((_pod._checkReference && IsAnyReference()))
	{
		FatalError(MESSAGE_STEPPER_IsAnyReference);
		EmergencyStop();
		return;
	}

	// calculate next step 

	StartBackground();
}

////////////////////////////////////////////////////////

#if defined (stepperstatic_)

CStepper::SMovementState CStepper::_movementstate;
CRingBufferQueue<CStepper::SStepBuffer, STEPBUFFERSIZE> CStepper::_steps;
CStepper::SMovements CStepper::_movements;
CStepper* CStepper::SMovement::_pStepper;

#endif

////////////////////////////////////////////////////////

void CStepper::SMovementState::Init(SMovement* pMovement)
{
	mdist_t steps = pMovement->_steps;

	if (pMovement->_state == SMovement::StateReadyMove)
	{
		_count = pMovement->GetMaxStepMultiplier();
		_timer = pMovement->_pod._move._ramp._timerStart;
	}
	else
	{
		_count = 1;
		_timer = pMovement->_pod._wait._timer;
	}
	
	steps = (steps / _count) >> 1;
	for (axis_t i = 0; i < NUM_AXIS; i++)
		_add[i] = steps;
	_n = 0;
	_rest = 0;
#ifndef REDUCED_SIZE
	_sumTimer = 0;
#endif
}

////////////////////////////////////////////////////////

bool CStepper::SMovementState::CalcTimerAcc(timer_t maxtimer, mdist_t n, unsigned char cnt)
{
	// use for float: Cn = Cn-1 - 2*Cn-1 / (4*N + 1)
	// use for INTEGER:
	// In = ((2*In-1)+Rn-1) / (4*N + 1)		=> quot
	// Rn = ((2*In-1)+Rn-1) % (4*N + 1)		=> remainer of division
	// Cn = Cn-1 - In

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

////////////////////////////////////////////////////////

bool CStepper::SMovementState::CalcTimerDec(timer_t mintimer, mdist_t n, unsigned char cnt)
{
	// use for float: Cn = Cn-1 + 2*Cn-1 / (4*N - 1)
	// use for INTEGER:
	// In = ((2*In-1)+Rn-1) / (4*N - 1)		=> quot
	// Rn = ((2*In-1)+Rn-1) % (4*N - 1)		=> remainer of division
	// Cn = Cn-1 - In

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

////////////////////////////////////////////////////////

bool CStepper::SMovement::IsEndWait()
{
	if (_pod._wait._checkWaitConditional)
	{
		// wait only if Stepper is "checkWaitConditional"
		if (!_pStepper->IsPauseMove() && !_pStepper->IsWaitConditional())
		{
			return true;
		}
	}
	return false;
}

////////////////////////////////////////////////////////
// called in interrupt => must be "fast"

bool CStepper::SMovement::CalcNextSteps(bool continues)
{
	// return false if buffer full and nothing calculated.

	register axis_t i;
	do
	{
		CStepper* pStepper = _pStepper;
		SMovementState* pState = &pStepper->_movementstate;

		if (pStepper->_steps.IsFull() || ((_state == SMovement::StateReadyWait || _state == SMovement::StateReadyIo) && pStepper->_steps.Count() > SYNC_STEPBUFFERCOUNT) 
		)
		{
			// cannot add to queue
			return false;
		}

		if (_state == SMovement::StateReadyMove || _state == SMovement::StateReadyWait || _state == SMovement::StateReadyIo)
		{
			// Start of move/wait

			pState->Init(this);

			for (i = 0; i<NUM_AXIS; i++)
			{
				if (_distance_[i] != 0)
				{
					pStepper->SetTimeoutAndEnable(i,0, CStepper::LevelMax, false);
/*
					pStepper->_pod._timeEnable[i] = 0;
					CCriticalRegion crit;
					if (pStepper->GetEnable(i) != CStepper::LevelMax)
						pStepper->SetEnable(i, CStepper::LevelMax, false);
*/
				}
			}

			if (_state == SMovement::StateReadyWait)
			{
				_state = StateWait;

				if (IsEndWait())
					pState->_n = _steps;
			}
			if (_state == SMovement::StateReadyIo)
			{
				pStepper->CallEvent(OnIoEvent, (void*)&_pod._io);
				// pState->_n = _steps; => done by Init()
				// this will end move immediately
			}
		}

		register mdist_t n = pState->_n;
		register unsigned char count = pState->_count;

		if (_steps <= n)
		{
			// End of move/wait/io

			for (i = 0; i<NUM_AXIS; i++)
			{
				if (_distance_[i] != 0)
					pStepper->_pod._timeEnable[i] = pStepper->_pod._timeOutEnable[i];
			}

			_state = StateDone;
			return true;
		}
		
		{
			// calculate f for step-buffer

			if (count > 1 && _steps - n <= count)
			{
				// last step with multiplier
				pStepper->_steps.NextTail().Init(_lastStepDirCount);
				count = (unsigned char)(_steps - n);	// must fit in unsinged char
			}
			else
			{
				register DirCount_t stepcount = 0;
				register DirCount_t mask = 15;

				if (_backlash)
				{
					// ((DirCountByte_t*)&stepcount)->byteInfo.nocount = 1;	=> this force stepcount to be not in register
					DirCountByte_t x = DirCountByte_t(); //POD
					x.byte.byteInfo.nocount = 1;
					stepcount += x.all; 
				}

				for (i = 0;; i++)
				{
					// Check overflow!
					mdist_t oldadd = pState->_add[i];
					pState->_add[i] += _distance_[i];
					if (pState->_add[i] >= _steps || pState->_add[i] < oldadd)
					{
						pState->_add[i] -= _steps;
						stepcount += mask&_dirCount;
					}
					if (i == NUM_AXIS - 1)
						break;
					mask *= 16;
				}
				pStepper->_steps.NextTail().Init(stepcount);
			}
		}

		////////////////////////////////////
		// calc new timer

		if (_state == StateReadyMove)
		{
			if (pState->_timer == _pod._move._ramp._timerRun)
				_state = StateRun;
			else
			{
				_state = pState->_timer > _pod._move._ramp._timerRun ? StateUpAcc : StateUpDec;
				if (pState->_count > 1 && _pod._move._ramp._nUpOffset == 0)
				{
					static const unsigned short corrtab[][2] PROGMEM =
					{
						{ 1300, 1402 },
						{ 611, 709 },
						{ 322, 400 },
						{ 307, 405 },
						{ 289, 403 }
					};
					unsigned short mul = pgm_read_word(&corrtab[pState->_count - 2][0]);
					unsigned short div = pgm_read_word(&corrtab[pState->_count - 2][1]);
					pState->_timer = (timer_t)MulDivU32(pState->_timer, mul, div);
				}
			}
		}
		else if (_state == StateWait)
		{
			if (IsEndWait())
			{
				n = _steps;
			}
		}
		else
		{
			if (_state <= StateRun)
			{
				if (n >= _pod._move._ramp._downStartAt)
				{
					pState->_rest = 0;
					_state = _pod._move._ramp._timerStop > pState->_timer ? StateDownDec : StateDownAcc;
				}
			}

			switch (_state)
			{
				case StateUpAcc:

					if (pState->CalcTimerAcc(_pod._move._ramp._timerRun, n + _pod._move._ramp._nUpOffset, count))
					{
						_state = StateRun;
					}
					break;

				case StateUpDec:

					if (pState->CalcTimerDec(_pod._move._ramp._timerRun, _pod._move._ramp._nUpOffset - n, count))
					{
						_state = StateRun;
					}
					break;

				case StateDownDec:

					pState->CalcTimerDec(_pod._move._ramp._timerStop, _steps - n + _pod._move._ramp._nDownOffset, count);
					break;

				case StateDownAcc:

					pState->CalcTimerAcc(_pod._move._ramp._timerStop, _pod._move._ramp._nDownOffset - (_steps - n - 1), count);
					break;

			}
		}
		
		timer_t t = pState->_timer*count;

#ifndef REDUCED_SIZE
		if (pStepper->GetSpeedOverride()!=CStepper::SpeedOverride100P)
		{
			// slower => increase timer
			unsigned long tl = RoundMulDivU32(t, CStepper::SpeedOverride100P, pStepper->GetSpeedOverride());
			if (tl >= TIMER1MAX)	    t = TIMER1MAX;		// to slow
			else if (tl < TIMER1MIN)    t = TIMER1MIN;		// to fast
			else						t = (timer_t) tl;
		}

		pState->_sumTimer += t;
#endif

		pStepper->_steps.NextTail().Timer = t;

		n += count;
		if (count > n)
		{
			// overrun
			n = _steps;
		}
		pState->_n = n;

#ifdef _MSC_VER
		{
			SStepBuffer& stepbuffer = pStepper->_steps.NextTail();
			memcpy(stepbuffer._distance, _distance_, sizeof(stepbuffer._distance));
			stepbuffer._steps = _steps;
			stepbuffer._state = _state;
			stepbuffer._n = pState->_n;
			stepbuffer._count = count;
			strcpy_s(stepbuffer._spMSCInfo, _mvMSCInfo);
		}
#endif

		pStepper->_steps.Enqueue();
	} while (continues);

	return true;
}

////////////////////////////////////////////////////////

void  CStepper::SetEnableAll(unsigned char level)
{
	for (register axis_t i = 0; i < NUM_AXIS; ++i)
	{
		SetEnable(i, level, true);
	}
}

////////////////////////////////////////////////////////

void CStepper::QueueAndSplitStep(const udist_t dist[NUM_AXIS], const bool directionUp[NUM_AXIS], steprate_t vMax)
{
	_pod._error = NULL;
	register axis_t i;

#if USESLIP
	register signed char slip;
#endif

	for (i = 0; i<NUM_AXIS; i++)
	{
		register long newC = CalcNextPos(_pod._calculatedpos[i], dist[i], directionUp[i]);
		if (_pod._limitCheck)
		{
			// check limit
			if (newC >(long) _pod._limitMax[i] || newC < (long)_pod._limitMin[i])
			{
				Error(MESSAGE_STEPPER_RangeLimit);
				//				StepperSerial.print(F("Error: range limit")); StepperSerial.print(_limitMin[i]); StepperSerial.print(F("<")); StepperSerial.print(newC);; StepperSerial.print(F("<")); StepperSerial.print(_limitMax[i]);
				return;
			}
		}

#if USESLIP
		// calc slip
		if (_refMove == SMNoRefMove)
		{
			if ((_Slip[i]>0 && d[i]>0) || (_Slip[i]<0 && d[i]<0))
			{
				_SlipSum[i] += abs(d[i]);
				slip = (int)_SlipSum[i] / _Slip[i];
				if (slip)
				{
					_SlipSum[i] -= slip*_Slip[i];
					d[i] += slip;
					_currentpos[i] -= slip;
					_StepOffset[i] = (_StepOffset[i] + slip) & 0xf;
				}
			}
		}
#endif

	}

	// now move must not fail => we can calculate next position

	for (i = 0; i < NUM_AXIS; i++)
	{
		_pod._calculatedpos[i] = CalcNextPos(_pod._calculatedpos[i], dist[i], directionUp[i]);
	}

	unsigned char stepmul=1;

	timer_t timerMax = vMax == 0 ? _pod._timerMaxDefault : SpeedToTimer(vMax);

	while (timerMax == (timer_t) -1)
	{
		stepmul++;
		timerMax = SpeedToTimer(vMax*stepmul);
	}

	if (timerMax < _pod._timerMaxDefault)
		timerMax = _pod._timerMaxDefault;

	mdist_t d[NUM_AXIS];
	udist_t steps = 0;

	for (i = 0; i<NUM_AXIS; i++)
	{
		if (dist[i] > steps)
			steps = dist[i];
	}

#ifndef REDUCED_SIZE
	_pod._totalSteps += steps;
#endif

	unsigned short movecount = 1;
	udist_t pos[NUM_AXIS] = { 0 };

	steps *= stepmul;

	if (steps > MAXSTEPSPERMOVE)
	{
		movecount = (unsigned short)(steps / MAXSTEPSPERMOVE);
		if ((steps % MAXSTEPSPERMOVE) != 0)
			movecount++;
	}

	for (unsigned short j = 1; j < movecount; j++)
	{
		for (i = 0; i < NUM_AXIS; i++)
		{
			udist_t newxtpos = RoundMulDivU32(dist[i], j, movecount);
			d[i] = (mdist_t)(newxtpos - pos[i]);
			pos[i] = newxtpos;
		}

		QueueMove(d, directionUp, timerMax, stepmul);
		if (IsError()) return;
	}

	for (i = 0; i < NUM_AXIS; i++)
	{
		d[i] = (mdist_t)(dist[i] - pos[i]);
	}

	QueueMove(d, directionUp, timerMax, stepmul);
}

////////////////////////////////////////////////////////

bool CStepper::MoveUntil(TestContinueMove testcontinue, void*param)
{
	while (IsBusy())
	{
		if (!testcontinue(param))
		{
			AbortMove();
			return true;
		}
		OnWait(WaitReference);
	}
	return false;
}

////////////////////////////////////////////////////////

bool CStepper::MoveUntil(unsigned char referenceId, bool referencevalue, unsigned short stabletime)
{
	unsigned long time = 0;

	while (IsBusy())
	{
		if (IsReference(referenceId) == referencevalue)
		{
			if (time == 0) time = millis() + stabletime;		// allow stabletime == 0
			if (millis() >= time)
			{
				AbortMove();
				return true;
			}
		}
		else
		{
			time = 0;
		}
		OnWait(WaitReference);
	}
	return false;
}

////////////////////////////////////////////////////////

bool CStepper::MoveAwayFromReference(axis_t axis, unsigned char referenceid, sdist_t dist, steprate_t vMax)
{
	if (IsReference(referenceid))
	{
		Info(MESSAGE_STEPPER_IsReferenceIsOn);
		CPushValue<bool> OldCheckForReference(&_pod._checkReference, false);
		MoveAwayFromReference(axis, dist, vMax);

		if (!MoveUntil(referenceid, false, REFERENCESTABLETIME))
			return false;
	}

	return !IsReference(referenceid);
}

////////////////////////////////////////////////////////

bool CStepper::MoveReference(axis_t axis, unsigned char referenceid, bool toMin, steprate_t vMax, sdist_t maxdist, sdist_t distToRef, sdist_t distIfRefIsOn)
{
	WaitBusy();

	CPushValue<bool> OldLimitCheck(&_pod._limitCheck, false);
	CPushValue<bool> OldWaitFinishMove(&_pod._waitFinishMove, false);
	CPushValue<bool> OldCheckForReference(&_pod._checkReference, false);
	CPushValue<timer_t> OldBacklashenabled(&_pod._timerbacklash, ((timer_t)-1));

	if (vMax == 0)			vMax = TimerToSpeed(_pod._timerMaxDefault);
#ifdef use16bit
	if (maxdist == 0)		maxdist = min(GetLimitMax(axis) - GetLimitMin(axis) , 0xfffel* MOVEMENTBUFFERSIZE);	// do not queue
#else
	if (maxdist == 0)		maxdist = ((GetLimitMax(axis) - GetLimitMin(axis)) * 11) / 10;	// add 10%
#endif

	if (distIfRefIsOn == 0)	distIfRefIsOn = maxdist / 8;

	// check diection of move (assume to min)
	if (maxdist > 0)		maxdist = -maxdist;
	if (distToRef < 0)		distToRef = -distToRef;
	if (distIfRefIsOn < 0)	distIfRefIsOn = -distIfRefIsOn;

	if (!toMin)
	{
		// move to max
		distToRef = -distToRef;
		distIfRefIsOn = -distIfRefIsOn;
		maxdist = -maxdist;
	}

	bool ret = false;

	if (MoveAwayFromReference(axis, referenceid, distIfRefIsOn, vMax))
	{
		// move to reference
		MoveRel(axis, maxdist, vMax);
		if (MoveUntil(referenceid, true, REFERENCESTABLETIME))
		{
			// ref reached => move away
			MoveRel(axis, distIfRefIsOn, vMax);
			if (MoveUntil(referenceid, false, REFERENCESTABLETIME))
			{
				// move distToRef from change
				if (distToRef)
				{
					MoveRel(axis, distToRef, vMax);
					WaitBusy();
				}
				ret = true;
			}
		}
	}
	else
	{
		Error(MESSAGE_STEPPER_MoveReferenceFailed);
	}

	// calling this methode always sets position, independent of the result!!!!
	SetPosition(axis, toMin ? GetLimitMin(axis) : GetLimitMax(axis));

	return ret;
}

////////////////////////////////////////////////////////

bool  CStepper::IsAnyReference()
{
	// slow version of IsAnyReference => override and do not call base

	for (axis_t axis = 0; axis < NUM_AXIS; axis++)
	{
		unsigned char referenceidmin = ToReferenceId(axis, true);
		unsigned char referenceidmax = ToReferenceId(axis, false);
		if ((_pod._useReference[referenceidmin] && IsReference(referenceidmin)) || (_pod._useReference[referenceidmax] && IsReference(referenceidmax)))
			return true;
	}
	return false;
}

////////////////////////////////////////////////////////

void CStepper::Wait(unsigned int sec100)
{
	QueueWait(mdist_t(sec100), WAITTIMER1VALUE, false);
}

////////////////////////////////////////////////////////

void CStepper::WaitConditional(unsigned int sec100)
{
	QueueWait(mdist_t(sec100), WAITTIMER1VALUE, true);
}

////////////////////////////////////////////////////////

void CStepper::IoControl(unsigned char tool, unsigned short level)
{
	WaitUntilCanQueue();
	_movements._queue.NextTail().InitIoControl(this, tool,level);

	EnqueuAndStartTimer(true);
}

////////////////////////////////////////////////////////

void CStepper::MoveAbs(const udist_t d[NUM_AXIS], steprate_t vMax)
{
	udist_t dist[NUM_AXIS];
	bool  directionUp[NUM_AXIS];

	for (register axis_t i = 0; i < NUM_AXIS; ++i)
	{
		directionUp[i] = d[i] >= _pod._calculatedpos[i];
		if (directionUp[i])
			dist[i] = d[i] - _pod._calculatedpos[i];
		else
			dist[i] = _pod._calculatedpos[i] - d[i];
	}
	QueueAndSplitStep(dist, directionUp, vMax);
}

////////////////////////////////////////////////////////

void CStepper::MoveRel(const sdist_t d[NUM_AXIS], steprate_t vMax)
{
	udist_t dist[NUM_AXIS];
	bool  directionUp[NUM_AXIS];

	for (register axis_t i = 0; i < NUM_AXIS; ++i)
	{
		directionUp[i] = d[i] >= 0;
		dist[i] = abs(d[i]);
	}
	QueueAndSplitStep(dist, directionUp, vMax);
}

////////////////////////////////////////////////////////

void CStepper::MoveAbs(axis_t axis, udist_t d, steprate_t vMax)
{
	udist_t D[NUM_AXIS] = { 0 };
	memcpy(D, _pod._calculatedpos, sizeof(_pod._calculatedpos));
	D[axis] = d;
	MoveAbs(D, vMax);
}

////////////////////////////////////////////////////////

void CStepper::MoveRel(axis_t axis, sdist_t d, steprate_t vMax)
{
	udist_t  dist[NUM_AXIS] = { 0 };
	bool   directionUp[NUM_AXIS] = { false };
	dist[axis] = abs(d);
	directionUp[axis] = d > 0;
	QueueAndSplitStep(dist, directionUp, vMax);
}

////////////////////////////////////////////////////////
// repeat axis and d until axis not in 0 .. NUM_AXIS

void CStepper::MoveAbsEx(steprate_t vMax, unsigned short axis, udist_t d, ...)
{
	udist_t D[NUM_AXIS] = { 0 };
	memcpy(D, _pod._calculatedpos, sizeof(_pod._calculatedpos));

	va_list arglist;
	va_start(arglist, d);

	while (axis < NUM_AXIS)
	{
		D[axis] = d;

#ifdef _MSC_VER
		axis = va_arg(arglist, unsigned short);
		d = va_arg(arglist, udist_t);
#else
		axis = va_arg(arglist, unsigned int);		// only "int" supported on arduino
		d    = va_arg(arglist, unsigned long);	
#endif
	}

	va_end(arglist);

	MoveAbs(D, vMax);
}

////////////////////////////////////////////////////////
// repeat axis and d until axis not in 0 .. NUM_AXIS

void CStepper::MoveRelEx(steprate_t vMax, unsigned short axis, sdist_t d, ...)
{
	sdist_t  dist[NUM_AXIS] = { 0 };

	va_list arglist;
	va_start(arglist, d);

	while (axis < NUM_AXIS)
	{
		dist[axis] = d;

#ifdef _MSC_VER
		axis = va_arg(arglist, unsigned short);
		d = va_arg(arglist, sdist_t);
#else
		axis = va_arg(arglist, unsigned int);		// only "int" supported on arduino
		d    = va_arg(arglist, unsigned long);	
#endif
	}

	va_end(arglist);

	MoveRel(dist, vMax);
}

////////////////////////////////////////////////////////

void CStepper::SetPosition(axis_t axis, udist_t pos)
{
	WaitBusy();
#ifdef USESLIP   
	_SlipSum[axis] = 0;
#endif   
	_pod._current[axis] = pos;
	_pod._calculatedpos[axis] = pos;
}

////////////////////////////////////////////////////////

#ifdef USESLIP

void CStepper::SetSlip(int d[NUM_AXIS])
{
	memcpy(_Slip,d,sizeof(d[0])*NUM_AXIS);
}

#endif

////////////////////////////////////////////////////////

timer_t CStepper::SpeedToTimer(steprate_t speed) const
{
	if (speed == 0)
		return (timer_t)-1;

	unsigned long timer = TIMER1FREQUENCE / speed;
	if (timer > ((timer_t)-1))
		return (timer_t)-1;

	return (timer_t)timer;
}

////////////////////////////////////////////////////////

steprate_t CStepper::TimerToSpeed(timer_t timer) const
{
	return SpeedToTimer(timer);
}

////////////////////////////////////////////////////////

void CStepper::SetTimeoutAndEnable(axis_t i, unsigned char timeout, unsigned char level, bool force)
{
	_pod._timeEnable[i] = timeout;
	CCriticalRegion crit;
	if (timeout==0 && GetEnable(i) != level)
		SetEnable(i, level, force);
}

////////////////////////////////////////////////////////

//static void DumpTypeBool(const __FlashStringHelper* head, bool value, bool newline)	{ DumpType<bool>(head, value, newline); }
//void DumpArray_udist_t(const __FlashStringHelper* head, const udist_t pos[NUM_AXIS], bool newline) { DumpArray<udist_t, NUM_AXIS>(head,pos,newline); }

void CStepper::Dump(unsigned char options)
{
#ifdef _NO_DUMP
	(void) options;
#else
	unsigned char i;

	if (options&DumpPos)
	{
		DumpArray<udist_t, NUM_AXIS>(F("pos"), _pod._current, false);
		DumpArray<udist_t, NUM_AXIS>(F("cur"), _pod._calculatedpos, !(options&DumpState));
	}

	if (options&DumpState)
	{
		for (i = 0; i < sizeof(_pod._useReference); i++)
		{
			StepperSerial.print(i == 0 ? F("ES") : F(":ES")); StepperSerial.print(i); StepperSerial.print(F("=")); StepperSerial.print(IsReference(i));
		}
		StepperSerial.print(F(":ANY=")); StepperSerial.print(IsAnyReference());
		DumpArray<bool, NUM_AXIS * 2>(F(":UseReference"), _pod._useReference, false);

		for (i = 0; i < NUM_AXIS; i++)
		{
			StepperSerial.print(i == 0 ? F("L") : F(":L")); StepperSerial.print(i); StepperSerial.print(F("=")); StepperSerial.print((int)GetEnable(i));
		}
		StepperSerial.println();

		DumpType<unsigned long>(F("TotalSteps"), _pod._totalSteps, false);
		DumpType<unsigned int>(F("TimerISRBusy"), _pod._timerISRBusy, false);

		DumpType<bool>(F("TimerRunning"), _pod._timerRunning, false);
		DumpType<bool>(F("CheckReference"), _pod._checkReference, false);
		DumpType<bool>(F("WaitFinishMove"), _pod._waitFinishMove, false);

		DumpType<bool>(F("LimitCheck"), _pod._limitCheck, false);
		DumpArray<udist_t, NUM_AXIS>(F("Min"), _pod._limitMin, false);
		DumpArray<udist_t, NUM_AXIS>(F("Max"), _pod._limitMax, false);

//		DumpArray<EnumAsByte(EStepMode), NUM_AXIS>(F("StepMode"), _stepMode, true);

		DumpType<timer_t>(F("TimerMaxDefault"), _pod._timerMaxDefault, false);

		DumpArray<steprate_t, NUM_AXIS>(F("MaxJerkSpeed"), _pod._maxJerkSpeed, false);
		DumpArray<steprate_t, NUM_AXIS>(F("TimerMax"), _pod._timerMax, false);
		DumpArray<steprate_t, NUM_AXIS>(F("TimerAcc"), _pod._timerAcc, false);
		DumpArray<steprate_t, NUM_AXIS>(F("TimerDec"), _pod._timerDec, true);
	}

	if (options&DumpMovements)
	{
		unsigned char idx;
		unsigned char idxnochange = _movements._queue.H2TInit();

		i = 0;
		for (idx = idxnochange; _movements._queue.H2TTest(idx); idx = _movements._queue.H2TInc(idx))
		{
			_movements._queue.Buffer[idx].Dump(i++, options);
		}
	}
#endif
}

////////////////////////////////////////////////////////

void CStepper::SMovement::Dump(unsigned char idx, unsigned char options)
{
#ifdef _NO_DUMP
	(void) idx; (void) options;
#else
	DumpType<unsigned char>(F("Idx"), idx, false);
	if (idx == 0)
	{
		_pStepper->_movementstate.Dump(options);
	}
	DumpType<udist_t>(F("Steps"), _steps, false);
	DumpType<udist_t>(F("State"), _state, false);

	DumpType<DirCount_t>(F("DirCount"), _dirCount, false);
	DumpType<DirCount_t>(F("LastDirCount"), _lastStepDirCount, false);
	DumpArray<mdist_t, NUM_AXIS>(F("Dist"), _distance_, false);
	DumpType<mdist_t>(F("UpSteps"), _pod._move._ramp._upSteps, false);
	DumpType<mdist_t>(F("DownSteps"), _pod._move._ramp._downSteps, false);
	DumpType<mdist_t>(F("DownStartAt"), _pod._move._ramp._downStartAt, false);
	DumpType<mdist_t>(F("UpOffset"), _pod._move._ramp._nUpOffset, false);
	DumpType<mdist_t>(F("DownOffset"), _pod._move._ramp._nDownOffset, false);

	DumpType<timer_t>(F("tMax"), _pod._move._timerMax, false);
	DumpType<timer_t>(F("tRun"), _pod._move._ramp._timerRun, false);
	DumpType<timer_t>(F("tStart"), _pod._move._ramp._timerStart, false);
	DumpType<timer_t>(F("tStop"), _pod._move._ramp._timerStop, false);
	DumpType<timer_t>(F("tEndPossible"), _pod._move._timerEndPossible, false);
	DumpType<timer_t>(F("tJunctionToPrev"), _pod._move._timerJunctionToPrev, false);
	DumpType<timer_t>(F("tMaxJunction"), _pod._move._timerMaxJunction, false);

	if (options&DumpDetails)
	{
		DumpType<timer_t>(F("TimerAcc"), _pod._move._timerAcc, false);
		DumpType<timer_t>(F("TimerDec"), _pod._move._timerDec, false);
	}

	StepperSerial.println();
#endif
}

////////////////////////////////////////////////////////

void CStepper::SMovementState::Dump(unsigned char /* options */)
{
#ifndef _NO_DUMP
	DumpType<mdist_t>(F("n"), _n, false);
	DumpType<timer_t>(F("t"), _timer, false);
	DumpType<timer_t>(F("r"), _rest, false);
	DumpType<unsigned long>(F("sum"), _sumTimer, false);
	DumpArray<mdist_t, NUM_AXIS>(F("a"), _add, false);
#endif
}
