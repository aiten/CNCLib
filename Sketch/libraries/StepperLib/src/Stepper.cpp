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

#include <stdlib.h>
#include <string.h>

#include <arduino.h>
#include <ctype.h>

#include "Stepper.h"
#include "UtilitiesStepperLib.h"

////////////////////////////////////////////////////////

CStepper::CStepper()
{
	InitMemVar();
}

////////////////////////////////////////////////////////

template<> CStepper* CSingleton<CStepper>::_instance = NULL;
bool CStepper::_emergencyStop = false;

////////////////////////////////////////////////////////

void CStepper::InitMemVar()
{
	register axis_t i;

	// look to ASM => more for() are faster an smaller
	for (i = 0; i < NUM_AXIS; i++)	_current[i] = 0;
	for (i = 0; i < NUM_AXIS; i++)	_calculatedpos[i] = 0;

	for (i = 0; i < NUM_AXIS; i++)	_timerMax[i] = 0;

#if USESLIP  
	for (i=0;i<NUM_AXIS;i++)	_SlipSum[i]=0;
	for (i=0;i<NUM_AXIS;i++)	_Slip[i]=0;
#endif
	for (i = 0; i < NUM_AXIS; i++)	_backlash[i] = 0;
	for (i = 0; i < NUM_AXIS; i++)	_limitMin[i] = 0;

	for (i = 0; i < NUM_AXIS; i++)	_limitMax[i] = 0xffff;
	for (i = 0; i < NUM_AXIS; i++)	_maxJerkSpeed[i] = 100;
	for (i = 0; i < NUM_AXIS; i++)	_stepMode[i] = HalfStep;

	_timerRunning = false;
	_waitFinishMove = false;
	_checkReference = true;
	_timerbacklash = (timer_t) -1;

	_limitCheck = true;
	_totalSteps = 0;
	_timerISRBusy = 0;
	_lastdirection = 0;

	_event = NULL;
	_eventparam = NULL;

	_idleLevel = 0;

	memset(_useReference, 0, sizeof(_useReference));
	_useReference[0] = true;
	_useReference[2] = true;
	_useReference[4] = true;

	// SetDefaultMaxSpeed(2500,100,150); => call to pure virtual

#ifdef _MSC_VER
	MSCInfo = "";
#endif

}

////////////////////////////////////////////////////////

void CStepper::Init()
{
	InitMemVar();
	InitTimer();

	_timerOnIdle = millis();
	SetIdleTimer();

	SetEnableAll(0);
}

////////////////////////////////////////////////////////

void CStepper::Remove()
{
	RemoveTimer();
	SetEnableAll(0);
}

////////////////////////////////////////////////////////

void CStepper::AddEvent(StepperEvent event, void* eventparam, StepperEvent& oldevent, void*& oldeventparam)
{
	oldevent = _event;
	oldeventparam = oldeventparam;
	_event = event;
	_eventparam = eventparam;
}

////////////////////////////////////////////////////////

void CStepper::StartTimer(timer_t timer)
{
	_timerRunning = true;
	HALStartTimer1(timer);
}

////////////////////////////////////////////////////////

void CStepper::SetIdleTimer()
{
	HALStartTimer1(IDLETIMER1VALUE);
	_timerRunning = false;
}

////////////////////////////////////////////////////////

void CStepper::StopTimer()
{
	HALStopTimer1();
	_timerRunning = false;
}

////////////////////////////////////////////////////////

void CStepper::QueueMove(const mdist_t dist[NUM_AXIS], const bool directionUp[NUM_AXIS], timer_t timerMax)
{

	//DumpArray<mdist_t,NUM_AXIS>(F("QueueMove"),dist,false);
	//DumpArray<bool,NUM_AXIS>(F("Dir"),directionUp,false);
	//DumpType<timer_t>(F("tm"),timerMax,true);

	mdist_t steps = 0;

	unsigned char directionmask = 0;
	unsigned char direction = 0;
	unsigned char mask = 1;

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
		Info(MESSAGE_STEPPER_EmptyMoveSkipped);
		return;
	}

	if (IsSetBacklash())
	{
		if ((_lastdirection&directionmask) != direction)
		{
			mdist_t backlashdist[NUM_AXIS] = { 0 };

			mdist_t backlashsteps = 0;
			char mask = 1;
			for (unsigned char i = 0; i < NUM_AXIS; i++)
			{
				if ((_lastdirection&directionmask&mask) != (direction&mask) && dist[i] && _backlash[i])
				{
					backlashdist[i] = _backlash[i];
					if (backlashdist[i] > backlashsteps)
						backlashsteps = backlashdist[i];
				}
				mask *= 2;
			}

			if (backlashsteps)
			{
				// need backlash
				Info(MESSAGE_STEPPER_Backlash);

				while (_movements._queue.IsFull())
				{
					OnWait(MovementQueueFull);
				}

				_movements._queue.NextTail().InitMove(this, _movements._queue.SaveTail(), backlashsteps, backlashdist, directionUp, _timerbacklash);
				_movements._queue.NextTail().SetBacklash();
				_movements._queue.Enqueue();
			}
		}
	}

	// set all bits in lastdirection where axes moves
	_lastdirection &= ~directionmask;
	_lastdirection += direction;

	// wait until free movement buffer

	while (_movements._queue.IsFull())
	{
		OnWait(MovementQueueFull);
	}

	_movements._queue.NextTail().InitMove(this, _movements._queue.SaveTail(), steps, dist, directionUp, timerMax);
	_movements._queue.Enqueue();

	OptimizeMovementQueue(false);

	if (!_timerRunning)
	{
		_movements._queue.Head().CalcNextSteps(false);
		if (_movements._queue.Head().IsFinished())
		{
			// empty move => startup failed
			_movements._queue.Dequeue();
		}
		else
		{
			OnStart();
		}
	}

	if (IsWaitFinishMove())
	{
		WaitBusy();
	}
}

////////////////////////////////////////////////////////

void CStepper::SMovement::InitMove(CStepper*pStepper, SMovement* mvPrev, mdist_t steps, const mdist_t dist[NUM_AXIS], const bool directionUp[NUM_AXIS], timer_t timerMax)
{
	register axis_t i;

	memset(this, 0, sizeof(SMovement));

	_pStepper = pStepper;
	_timerMax = timerMax;

	_steps = steps;
	memcpy(_distance_, dist, sizeof(_distance_));

#ifdef _MSC_VER
	strcpy_s(MSCInfo, _pStepper->MSCInfo);
#endif

	// calculate relative speed for axis => limit speed for axis

	for (i = 0; i < NUM_AXIS; i++)
	{
		if (dist[i])
		{
			unsigned long axistimer = MulDivU32(_timerMax, _steps, dist[i]);
			if (axistimer < (unsigned long)pStepper->_timerMax[i])
			{
				timerMax = (timer_t)MulDivU32(pStepper->_timerMax[i], dist[i], _steps);
				_timerMax = max(timerMax, _timerMax);
			}
		}
	}

	// calculate StepMultiplier and adjust distance

	unsigned char maxMultiplier = CStepper::GetStepMultiplier(_timerMax);
	_lastStepDirCount.all = 0;
	_dirCount.all = 0;

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

			_lastStepDirCount.all *= 16;
			_lastStepDirCount.all += axisdiff;
			_dirCount.all *= 16;
			_dirCount.all += multiplier;

			if (directionUp[i])
			{
				_lastStepDirCount.all += 8;
				_dirCount.all += 8;
			}

			if (i == 0)
				break;
		}
	}
	else
	{
		for (i = NUM_AXIS - 1;; i--)
		{
			_dirCount.all *= 16;
			_dirCount.all += 1;
			if (directionUp[i])
				_dirCount.all += 8;

			if (i == 0)
				break;
		}
		_lastStepDirCount = _dirCount;
	}

#pragma message ("TODO: get acc/dec axis")

	_upAxis = 0;
	_downAxis = 0;

	_timerStart = GetUpTimerAcc();
	_timerStop = GetUpTimerDec();
	_timerRun = _timerMax;

	if (_timerRun > _timerStart)
		_timerStart = _timerRun;

	if (_timerRun > _timerStop)
		_timerStop = _timerRun;

	_state = StateReady;

	_timerJunctionToPrev = (timer_t)-1;	// force optimization

	if (mvPrev)
		CalcMaxJunktionSpeed(mvPrev);

	RampUp(_timerStart);
	RampDown(_timerStop);
	RampRun();

	if (mvPrev == NULL)
		_timerEndPossible = _pStepper->GetTimer(_steps, GetUpTimerAcc());

	//pStepper->Dump(DumpAll);
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
	register DirCount_t count; count.all = _dirCount.all;
	register unsigned char maxmultiplier = 0;

	for (register unsigned char i = 0;; i++)
	{
		maxmultiplier = max(maxmultiplier, ((unsigned char)count.all) % 8);
		if (i == NUM_AXIS - 1)
			break;

		count.all /= 16;
	}
	return maxmultiplier;

}

////////////////////////////////////////////////////////

void CStepper::SMovement::RampRun()
{
	if (_upSteps > _steps || _steps - _upSteps < _downSteps)
	{
		// we cant reach vmax for this movement, cut plateau.
		// leave relation between up and down squared => constant a
		mdist_t toMany = _downSteps + _upSteps - _steps;
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
			timer_t upTimer = GetUpTimer(_timerStart > _timerRun);
			timer_t downTimer = GetUpTimer(_timerStop < _timerRun);

			unsigned long sqUp = (unsigned long)(upTimer)* (unsigned long)(upTimer);
			unsigned long sqDown = (unsigned long)(downTimer)* (unsigned long)(downTimer);

			sqUp /= 0x1000;		// may overrun => divide by 0x1000
			sqDown /= 0x1000;

			unsigned long sum = sqUp + sqDown;

			subUp = mdist_t(RoundMulDivU32(toMany, sqUp, sum));	// round
		}

		if (subUp > _upSteps || (toMany - subUp) > _downSteps)
		{
#ifdef _MSC_VER
#endif
			subUp = _upSteps;
			// return false;
		}

		_upSteps -= subUp;
		_downSteps -= toMany - subUp;

		if (_downSteps > _steps)
		{
#ifdef _MSC_VER
#endif
			_downSteps = _steps;
			// return false;
		}

		_downStartAt = _steps - _downSteps;
	}
}

////////////////////////////////////////////////////////

void CStepper::SMovement::RampH2T(/* SMovement* mvPrev */ SMovement*mvNext)
{
	if (!IsActive()) return;					// Move became inactive by ISR

	RampUp(_timerJunctionToPrev);

	if (mvNext)
	{
		RampDown(mvNext->_timerJunctionToPrev);
	}
	else if (mvNext == NULL)
	{
		RampDown(GetDownTimerDec());
	}

	RampRun();
}

////////////////////////////////////////////////////////

void CStepper::SMovement::RampDown(timer_t timerJunction)
{
	timer_t timerAccDec = GetDownTimerDec();
	if (timerJunction >= timerAccDec)
	{
		_timerStop = max(timerAccDec, _timerRun);	// to v=0
		_downSteps = CStepper::GetDecSteps(_timerRun, timerAccDec);
		_downStartAt = _steps - _downSteps;
	}
	else
	{
		_timerStop = timerJunction;

		if (_timerStop >= _timerRun)							// dec while stop
		{
			// shift down phase with _nDownOffset steps
			_nDownOffset = CStepper::GetDecSteps(_timerStop, timerAccDec);
			_downSteps = CStepper::GetDecSteps(_timerRun, timerAccDec);
			_downStartAt = _steps - _downSteps + _nDownOffset;
		}
		else
		{
			timerAccDec = GetDownTimerAcc();
			_nDownOffset = CStepper::GetAccSteps(_timerStop, timerAccDec);
			//_downStartAt = _steps - 2 - (_nDownOffset - CStepper::GetAccSteps(_timerRun,timerAccDec));
			_downStartAt = _steps - (_nDownOffset - CStepper::GetAccSteps(_timerRun, timerAccDec));
		}
		_downSteps = _steps - _downStartAt;
	}
}

////////////////////////////////////////////////////////

void CStepper::SMovement::RampUp(timer_t timerJunction)
{
	//	Recalc RampUp even if currently running => calc can increase ramp lenght


	timer_t timerAccDec = GetUpTimerAcc();
	_upSteps = CStepper::GetAccSteps(_timerRun, timerAccDec);

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
			timerAccDec = GetUpTimerDec();
			_nUpOffset = GetDecSteps(_timerStart, timerAccDec);
			_upSteps = _nUpOffset - GetDecSteps(_timerRun, timerAccDec);
		}
	}
}

////////////////////////////////////////////////////////
// drill down the junction speed if speed at junction point is not possible

void CStepper::SMovement::AdjustJunktionSpeedH2T(SMovement*mvPrev, SMovement*mvNext)
{
	if (!IsActive()) return;					// Move became inactive by ISR

	if (mvPrev == NULL || IsProcessing())		// no prev or processing (can be if the ISR has switchted to the next move)
	{
		// first "now" executing move
		// do not change _timerrun
#pragma message("TODO: _timerEndPossible depends on Movementstate")

		if (_state <= StateRun)
			_timerEndPossible = max(_timerEndPossible, _timerRun);
		else
			_timerEndPossible = _timerStop;
	}
	else
	{
		_timerEndPossible = _pStepper->GetTimerAccelerating(_steps, mvPrev->_timerEndPossible, GetUpTimerAcc());

		CCriticalRegion crit; // prev operation may take long, in the meantime the ISR has finished a move

		if (IsProcessing())
		{
			// first "now" executing move

#pragma message("TODO: _timerEndPossible depends on Movementstate")

			if (_state <= StateRun)
				_timerEndPossible = max(_timerEndPossible, _timerRun);
			else
				_timerEndPossible = _timerStop;
		}
		else
		{
			if (_timerEndPossible > _timerMax)
			{
				// not faster as required
				_timerRun = max(_timerEndPossible, _timerRun);
			}
		}
	}

	if (mvNext != NULL)
	{
		// next element available, calculate junction speed
		mvNext->_timerJunctionToPrev = max(mvNext->_timerMaxJunction, max(_timerEndPossible, mvNext->_timerJunctionToPrev));
		_timerEndPossible = mvNext->_timerJunctionToPrev;
	}
}

////////////////////////////////////////////////////////
// drill down the junction speed if speed at junction point is not possible
// return (Tail to Head) if entrie has changed, if not, all previous (to head) no change is required

bool CStepper::SMovement::AdjustJunktionSpeedT2H(SMovement*mvPrev, SMovement*mvNext)
{
	if (!IsActive()) return true;					// Move became inactive by ISR

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

	if (IsProcessing())
		return true;					// stop optimizing here - maybe not reached head but ISR has switched to next move

	if (mvPrev != NULL)
	{
		_timerRun = _timerMax;

		// prev element available, calculate junction speed
		timer_t junctiontoPrev = max(_timerMaxJunction, _pStepper->_movements._timerStartPossible);
		if (junctiontoPrev == _timerJunctionToPrev)
			return true;					// nothing changed (prev movementes do not change)

		_timerJunctionToPrev = junctiontoPrev;
		_pStepper->_movements._timerStartPossible = _timerJunctionToPrev;
	}

	return false;
}

////////////////////////////////////////////////////////
// calculate the max junction between two movements- consider jerk - speed is maxspeed - only calculated once (at setup time of movement)

void CStepper::SMovement::CalcMaxJunktionSpeed(SMovement*mvPrev)
{
	// .1 => prev
	// .2 => next(this)

	// default => fastest move (no jerk)
	_timerMaxJunction = min(mvPrev->_timerMax, _timerMax);
	timer_t timerMaxJunction;
	timer_t timerMaxJunctionAcc = mvPrev->GetUpTimerAcc();

	mdist_t s1 = mvPrev->_steps;
	mdist_t s2 = _steps;

	axis_t mainaxis;	// max moving axis	=> no jerk 

	for (mainaxis = 0; mainaxis < NUM_AXIS; mainaxis++)
	{
		if (s1 == mvPrev->GetDistance(mainaxis) && s2 == GetDistance(mainaxis) && mvPrev->GetDirectionUp(mainaxis) == GetDirectionUp(mainaxis))
		{
			_timerMaxJunction = (long(mvPrev->_timerMax) + long(_timerMax)) / 2;
			break;
		}
	}

	timer_t timerMaxJunction_ = _timerMaxJunction;
	/*
		printf("H: s1=%i s2=%i\n", (int)s1, (int)s2);
		for (int ii = 0; ii < NUM_AXIS; ii++)
		{
		printf("%i: Prev:%i%c\tThis:%i%c\n", ii, (int)mvPrev->GetDistance(ii), mvPrev->GetDirectionUp(ii) ? '+' : '-', (int)GetDistance(ii), GetDirectionUp(ii) ? '+' : '-');
		}
		printf("");

		if (mainaxis >= NUM_AXIS)
		{
		//_timerMaxJunction = (long(mvPrev->_timerMax) + long(_timerMax)) / 2;
		//return;
		}
		*/
	for (axis_t i = 0; i < NUM_AXIS; i++)
	{
		if (i != mainaxis)
		{
			mdist_t d1 = mvPrev->GetDistance(i);
			mdist_t d2 = GetDistance(i);

			steprate_t v1 = _pStepper->TimerToSpeed(mvPrev->_timerMax);
			steprate_t v2 = _pStepper->TimerToSpeed(_timerMax);

			if (d1 != s1) v1 = steprate_t(RoundMulDivUInt(v1, d1, s1));
			if (d2 != s2) v2 = steprate_t(RoundMulDivUInt(v2, d2, s2));


			long vdiff;

			if (v1 == 0 || v2 == 0 || mvPrev->GetDirectionUp(i) == GetDirectionUp(i))
			{
				// same direction (v1 and v2 not 0)
				vdiff = v1 > v2 ? v1 - v2 : v2 - v1;

				if (vdiff > long(_pStepper->_maxJerkSpeed[i]))
				{
					// reduce total speed by ratio maxJerk <=> current jerk
					timerMaxJunction = _pStepper->SpeedToTimer(RoundMulDivUInt(_pStepper->TimerToSpeed(timerMaxJunction_), _pStepper->_maxJerkSpeed[i], steprate_t(vdiff)));
					_timerMaxJunction = max(_timerMaxJunction, min(timerMaxJunction, timerMaxJunctionAcc));
				}
			}
			else
			{
				// different direction, add speed
				vdiff = (long)(v1)+(long)(v2);

				if (mainaxis >= NUM_AXIS)
				{
					_timerMaxJunction = timerMaxJunctionAcc;	//Stop and go
				}
				else
				{
					if (vdiff > long(_pStepper->_maxJerkSpeed[i]))
					{
						// reduce total speed by ratio maxJerk <=> current jerk
						timerMaxJunction = _pStepper->SpeedToTimer(RoundMulDivUInt(_pStepper->TimerToSpeed(timerMaxJunction_), _pStepper->_maxJerkSpeed[i], steprate_t(vdiff)));
						_timerMaxJunction = max(_timerMaxJunction, min(timerMaxJunction, timerMaxJunctionAcc));
					}
				}
			}
		}
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
		if (_movements._queue.Buffer[idx].AdjustJunktionSpeedT2H(_movements._queue.GetPrev(idx), _movements._queue.GetNext(idx)))
		{
			idxnochange = idx;
			break;
		}
	}

	////////////////////////////////////
	// calculate junction (max) speed!

	for (idx = idxnochange; _movements._queue.H2TTest(idx); idx = _movements._queue.H2TInc(idx))
	{
		_movements._queue.Buffer[idx].AdjustJunktionSpeedH2T(_movements._queue.GetPrev(idx), _movements._queue.GetNext(idx));
	}

	////////////////////////////////////
	// calcualte trapezoid ramp
	for (idx = idxnochange; _movements._queue.H2TTest(idx); idx = _movements._queue.H2TInc(idx))
	{
		_movements._queue.Buffer[idx].RampH2T(/* _movements._queue.GetPrev(idx), */ _movements._queue.GetNext(idx));
	}
}

////////////////////////////////////////////////////////

void CStepper::OnIdle(unsigned long idletime)
{
	CallEvent(OnIdleEvent, 0);
	if (idletime > TIMEOUTSETIDLE)
	{
		if (GetEnable(X_AXIS) != _idleLevel)
		{
			SetEnableAll(_idleLevel);
			CallEvent(OnDisableEvent, 0);
		}
	}
}

////////////////////////////////////////////////////////

void CStepper::OnStart()
{
	CallEvent(OnStartEvent, 0);

	{
		CCriticalRegion crit;

		SetEnableAll(100);
		Step(false);
	}
}

////////////////////////////////////////////////////////

void CStepper::OnWait(EnumAsByte(EWaitType) wait)
{
	CallEvent(OnWaitEvent, (unsigned char) wait);
}

////////////////////////////////////////////////////////

void CStepper::OnError(const __FlashStringHelper * /* error */)
{
}

////////////////////////////////////////////////////////

void CStepper::OnWarning(const __FlashStringHelper * warning)
{
	StepperSerial.print(MESSAGE_WARNING);
	StepperSerial.println(warning);
}

////////////////////////////////////////////////////////

void CStepper::OnInfo(const __FlashStringHelper * info)
{
	StepperSerial.print((MESSAGE_INFO));
	StepperSerial.println(info);
}

////////////////////////////////////////////////////////

void CStepper::WaitBusy()
{
	while (IsBusy())
	{
		// wait until finish alle movements
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

void CStepper::AbortMove()
{
	CCriticalRegion critical;

	if (_timerRunning)
		SetIdleTimer();

	// sub all pending steps to _totalsteps

	unsigned long steps = _steps.Count();

	for (unsigned char idx = _movements._queue.T2HInit(); _movements._queue.T2HTest(idx); idx = _movements._queue.T2HInc(idx))
	{
		if (_movements._queue.Buffer[idx].IsActive())
		{
			steps += _movements._queue.Buffer[idx]._steps;
			if (_movements._queue.Buffer[idx].IsProcessing())
				steps -= _movementstate._n;
		}
	}

	_totalSteps -= steps;

	_steps.Clear();
	_movements._queue.Clear();

	memcpy(_calculatedpos, _current, sizeof(_calculatedpos));

	_timerOnIdle = millis();
}

////////////////////////////////////////////////////////

void CStepper::EmergencyStopResurrect()
{
	AbortMove();		// make sure nothing is running
	_emergencyStop = false;
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
// called in interrupt => must be "fast"

void CStepper::Step(bool isr)
{
	// interrups are disabled (ISR)
	// or disable: see OnStart

	if (isr  && !_timerRunning)
	{
		// idle Timer
		SetIdleTimer();
		OnIdle(millis() - _timerOnIdle);
		return;
	}

	if (_steps.IsEmpty())
	{
		// start idle timer
		_timerOnIdle = millis();
		SetIdleTimer();
		OnIdle(0);
		return;
	}

	if (_emergencyStop)
	{
		AbortMove();
		return;
	}
	else
	{
		// first send commands to stepper driver
		// calculate all axes and set PINS paralell - DRV 8225 requires 1.9us * 2 per step => sequential is to slow 

		register DirCountAll_t dir_count;	// do not use DirCount_t => AVR do not use registers for struct
		unsigned char axescount[NUM_AXIS];
		unsigned char directionUp = 0;

		{
			const SStepBuffer* stepbuffer = &_steps.Head();
			StartTimer(stepbuffer->Timer - TIMEROVERHEAD);
			dir_count = stepbuffer->DirStepCount.all;
		}

#ifdef _MSC_VER
		StepBegin(&_steps.Head());
#endif
		// div with 256 is faster than 16 (loop shift)

		unsigned char bytedircount=0;
		bool countit = true;
		if (((DirCount_t*) &dir_count)->byte.byteInfo.nocount != 0)
			countit = false;

		for (register unsigned char i = 0;; i++)
		{
			if (i % 2 == 1)
			{
				bytedircount = bytedircount / 16;
			}
			else
			{
				bytedircount = (unsigned char)dir_count; //  &255;
				dir_count /= 256;
			}

			axescount[i] = bytedircount & 7;
			directionUp /=2;

			if (axescount[i])
			{
				if ((bytedircount&8) != 0)
				{
					directionUp += (1<<(NUM_AXIS-1));
					if (countit) _current[i] += axescount[i];
				}
				else
				{
					if (countit) _current[i] -= axescount[i];
				}
			}

			if (i == NUM_AXIS - 1)
				break;
		}

		Step(axescount,directionUp);

		_steps.Dequeue();
	}

	// check reference and calculate next steps

	if ((_checkReference && IsAnyReference()))
	{
		Error(MESSAGE_STEPPER_IsAnyReference);
		SetIdleTimer();
		return;
	}

	static volatile unsigned char reentercount = 0;

	reentercount++;

	if (reentercount != 1)
	{
		_timerISRBusy++;
		reentercount--;
		return;
	}

	EnableInterrupts();

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

	DisableInterrupts();
	reentercount--;
	EnableInterrupts();
}

////////////////////////////////////////////////////////

#if defined (stepperstatic_)

CStepper::SMovementState CStepper::_movementstate;
CRingBufferQueue<CStepper::SStepBuffer, STEPBUFFERSIZE> CStepper::_steps;
CStepper::SMovements CStepper::_movements;
CStepper* CStepper::SMovement::_pStepper;

#endif

////////////////////////////////////////////////////////
// called in interrupt => must be "fast"

bool CStepper::SMovement::CalcNextSteps(bool continues)
{
	// return false if buffer full and nothing calculated.
	do
	{
		if (_state == StateReady)
		{
			_pStepper->_movementstate.Init(_timerStart, _steps, GetMaxStepMultiplier());
		}

		const register mdist_t n = _pStepper->_movementstate._n;
		register unsigned char count = _pStepper->_movementstate._count;

		if (_steps <= n)
		{
			_state = StateDone;
			return true;
		}

		if (_pStepper->_steps.IsFull())
		{
			// cannot add to queue
			return false;
		}

		{
			if (count > 1 && _steps - n <= count)
			{
				// last step with multiplier
				_pStepper->_steps.NextTail().Init(_lastStepDirCount);
				count = (unsigned char)(_steps - n);	// must fit in unsinged char
			}
			else
			{
				register axis_t i;
				register DirCount_t stepcount; stepcount.all = 0;
				register DirCount_t mask; mask.all = 15;

				if (_backlash)
				{
					stepcount.byte.byteInfo.nocount = 1;
				}

				for (i = 0;; i++)
				{
					// Check overflow!
					mdist_t oldadd = _pStepper->_movementstate._add[i];
					_pStepper->_movementstate._add[i] += _distance_[i];
					if (_pStepper->_movementstate._add[i] >= _steps || _pStepper->_movementstate._add[i] < oldadd)
					{
						_pStepper->_movementstate._add[i] -= _steps;
						stepcount.all += mask.all&_dirCount.all;
					}
					if (i == NUM_AXIS - 1)
						break;
					mask.all *= 16;
				}
				_pStepper->_steps.NextTail().Init(stepcount);
			}
		}

		////////////////////////////////////
		// calc new timer

		if (_state == StateReady)
		{
			if (_pStepper->_movementstate._timer == _timerRun)
				_state = StateRun;
			else
			{
				_state = _pStepper->_movementstate._timer > _timerRun ? StateUpAcc : StateUpDec;
				if (_pStepper->_movementstate._count > 1 && _nUpOffset == 0)
				{
					static const unsigned short corrtab[][2] PROGMEM =
					{
						{ 1300, 1402 },
						{ 611, 709 },
						{ 322, 400 },
						{ 307, 405 },
						{ 289, 403 }
					};
					unsigned short mul = pgm_read_word(&corrtab[_pStepper->_movementstate._count - 2][0]);
					unsigned short div = pgm_read_word(&corrtab[_pStepper->_movementstate._count - 2][1]);
					_pStepper->_movementstate._timer = (timer_t)MulDivU32(_pStepper->_movementstate._timer, mul, div);
				}
			}
		}
		else
		{
			if (_state <= StateRun)
			{
				if (n >= _downStartAt)
				{
					_pStepper->_movementstate._rest = 0;
					_state = _timerStop > _pStepper->_movementstate._timer ? StateDownDec : StateDownAcc;
				}
			}

			switch (_state)
			{
				case StateUpAcc:

					// use for float: Cn = Cn-1 - 2*Cn-1 / (4*N + 1)
					// use for INTEGER:
					// In = ((2*In-1)+Rn-1) / (4*N + 1)		=> quot
					// Rn = ((2*In-1)+Rn-1) % (4*N + 1)		=> remainer of division
					// Cn = Cn-1 - In
					if (_pStepper->_movementstate.CalcTimerAcc(_timerRun, n + _nUpOffset, count))
					{
						_state = StateRun;
					}
					break;

				case StateUpDec:

					if (_pStepper->_movementstate.CalcTimerDec(_timerRun, _nUpOffset - n, count))
					{
						_state = StateRun;
					}
					break;

				case StateDownDec:

					// use for float: Cn = Cn-1 + 2*Cn-1 / (4*N - 1)
					// use for INTEGER:
					// In = ((2*In-1)+Rn-1) / (4*N - 1)		=> quot
					// Rn = ((2*In-1)+Rn-1) % (4*N - 1)		=> remainer of division
					// Cn = Cn-1 - In
					_pStepper->_movementstate.CalcTimerDec(_timerStop, _steps - n + _nDownOffset, count);
					break;

				case StateDownAcc:

					_pStepper->_movementstate.CalcTimerAcc(_timerStop, _nDownOffset - (_steps - n - 1), count);
					break;

			}
		}

		_pStepper->_movementstate._sumTimer +=
			_pStepper->_steps.NextTail().Timer = _pStepper->_movementstate._timer*count;

		_pStepper->_movementstate._n = n + count;
		if (count > _pStepper->_movementstate._n)
		{
			// overrun
			_pStepper->_movementstate._n = _steps;
		}

#ifdef _MSC_VER
		{
			SStepBuffer& stepbuffer = _pStepper->_steps.NextTail();
			memcpy(stepbuffer._distance, _distance_, sizeof(stepbuffer._distance));
			stepbuffer._steps = _steps;
			stepbuffer._state = (EState)_state;
			stepbuffer._n = _pStepper->_movementstate._n;
			stepbuffer._count = count;
			strcpy_s(stepbuffer.MSCInfo, MSCInfo);
		}
#endif

		_pStepper->_steps.Enqueue();
	} while (continues);

	return true;
}

////////////////////////////////////////////////////////

void  CStepper::SetEnableAll(unsigned char level)
{
	for (register axis_t i = 0; i < NUM_AXIS; ++i)
	{
		SetEnable(i, level);
	}
}

////////////////////////////////////////////////////////

void CStepper::QueueAndSplitStep(const udist_t dist[NUM_AXIS], const bool directionUp[NUM_AXIS], steprate_t vMax)
{
	_error = NULL;
	register axis_t i;

#if USESLIP
	register signed char slip;
#endif

	for (i = 0; i<NUM_AXIS; i++)
	{
		register long newC = CalcNextPos(_calculatedpos[i], dist[i], directionUp[i]);
		if (_limitCheck)
		{
			// check limit
			if (newC >(long) _limitMax[i] || newC < (long)_limitMin[i])
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
		_calculatedpos[i] = CalcNextPos(_calculatedpos[i], dist[i], directionUp[i]);
	}

	timer_t timerMax = vMax == 0 ? _timerMaxDefault : SpeedToTimer(vMax);
	if (timerMax < _timerMaxDefault)
		timerMax = _timerMaxDefault;

	mdist_t d[NUM_AXIS];
	udist_t steps = 0;

	for (i = 0; i<NUM_AXIS; i++)
	{
		if (dist[i] > steps)
			steps = dist[i];
	}

	_totalSteps += steps;

	unsigned short movecount = 1;
	udist_t pos[NUM_AXIS] = { 0 };

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

		QueueMove(d, directionUp, timerMax);
		if (IsError()) return;
	}

	for (i = 0; i < NUM_AXIS; i++)
	{
		d[i] = (mdist_t)(dist[i] - pos[i]);
	}

	QueueMove(d, directionUp, timerMax);
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
		CRememberOld<bool> OldCheckForReference(&_checkReference, false);
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

	bool ret = false;

	CRememberOld<bool> OldLimitCheck(&_limitCheck, false);
	CRememberOld<bool> OldWaitFinishMove(&_waitFinishMove, false);
	CRememberOld<bool> OldCheckForReference(&_checkReference, false);
	CRememberOld<timer_t> OldBacklashenabled(&_timerbacklash, ((timer_t)-1));

	if (vMax == 0)			vMax = TimerToSpeed(_timerMaxDefault);
	if (maxdist == 0)		maxdist = ((GetLimitMax(axis) - GetLimitMin(axis))*11)/10;	// add 10%
	if (distToRef == 0)		distToRef = 0;
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

	if (!MoveAwayFromReference(axis, referenceid, distIfRefIsOn, vMax))
	{
		Info(MESSAGE_STEPPER_MoveReferenceFailed);
		Error(MESSAGE_STEPPER_MoveReferenceFailed);
		return false;
	}

	// move to reference
	MoveRel(axis, maxdist, vMax);
	if (!MoveUntil(referenceid, true, REFERENCESTABLETIME))
		return false;

	// ref reached => move away
	MoveRel(axis, distIfRefIsOn, vMax);
	if (!MoveUntil(referenceid, false, REFERENCESTABLETIME))
		return false;

	// move distToRef from change
	if (distToRef)
	{
		MoveRel(axis, distToRef, vMax);
		WaitBusy();
	}

	if (toMin)
		SetPosition(axis, GetLimitMin(axis));
	else
		SetPosition(axis, GetLimitMax(axis));

	return ret;
}

////////////////////////////////////////////////////////

void CStepper::MoveAbs(const udist_t d[NUM_AXIS], steprate_t vMax)
{
	udist_t dist[NUM_AXIS];
	bool  directionUp[NUM_AXIS];

	for (register axis_t i = 0; i < NUM_AXIS; ++i)
	{
		directionUp[i] = d[i] >= _calculatedpos[i];
		if (directionUp[i])
			dist[i] = d[i] - _calculatedpos[i];
		else
			dist[i] = _calculatedpos[i] - d[i];
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
	memcpy(D, _calculatedpos, sizeof(_calculatedpos));
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
	memcpy(D, _calculatedpos, sizeof(_calculatedpos));

	va_list arglist;
	va_start(arglist, d);

	while (axis >= 0 && axis < NUM_AXIS)
	{
		D[axis] = d;

#ifdef _MSC_VER
		axis = va_arg(arglist, unsigned short);
		d = va_arg(arglist, udist_t);
#else
		axis = va_arg(arglist, unsigned int);		// only "int" supported on arduion
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

	while (axis >= 0 && axis < NUM_AXIS)
	{
		dist[axis] = d;

#ifdef _MSC_VER
		axis = va_arg(arglist, unsigned short);
		d = va_arg(arglist, sdist_t);
#else
		axis = va_arg(arglist, unsigned int);		// only "int" supported on arduion
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
	_current[axis] = pos;
	_calculatedpos[axis] = pos;
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

//static void DumpTypeBool(const __FlashStringHelper* head, bool value, bool newline)	{ DumpType<bool>(head, value, newline); }
//void DumpArray_udist_t(const __FlashStringHelper* head, const udist_t pos[NUM_AXIS], bool newline) { DumpArray<udist_t, NUM_AXIS>(head,pos,newline); }

void CStepper::Dump(unsigned char options)
{
#ifndef REDUCED_SIZE
	unsigned char i;

	if (options&DumpPos)
	{
		DumpArray<udist_t, NUM_AXIS>(F("pos"), _current, false);
		DumpArray<udist_t, NUM_AXIS>(F("cur"), _calculatedpos, !(options&DumpState));
	}

	if (options&DumpState)
	{
		for (i = 0; i < sizeof(_useReference); i++)
		{
			StepperSerial.print(i == 0 ? F("ES") : F(":ES")); StepperSerial.print(i); StepperSerial.print(F("=")); StepperSerial.print(IsReference(i));
		}
		StepperSerial.print(F(":ANY=")); StepperSerial.print(IsAnyReference());
		DumpArray<bool, NUM_AXIS * 2>(F(":UseReference"), _useReference, false);

		for (i = 0; i < NUM_AXIS; i++)
		{
			StepperSerial.print(i == 0 ? F("L") : F(":L")); StepperSerial.print(i); StepperSerial.print(F("=")); StepperSerial.print((int)GetEnable(i));
		}
		StepperSerial.println();

		DumpType<unsigned long>(F("TotalSteps"), _totalSteps, false);
		DumpType<unsigned int>(F("TimerISRBusy"), _timerISRBusy, false);

		DumpType<bool>(F("TimerRunning"), _timerRunning, false);
		DumpType<bool>(F("CheckReference"), _checkReference, false);
		DumpType<bool>(F("WaitFinishMove"), _waitFinishMove, false);

		DumpType<bool>(F("LimitCheck"), _limitCheck, false);
		DumpArray<udist_t, NUM_AXIS>(F("Min"), _limitMin, false);
		DumpArray<udist_t, NUM_AXIS>(F("Max"), _limitMax, false);

//		DumpArray<EnumAsByte(EStepMode), NUM_AXIS>(F("StepMode"), _stepMode, true);

		DumpType<timer_t>(F("TimerMaxDefault"), _timerMaxDefault, false);

		DumpArray<steprate_t, NUM_AXIS>(F("MaxJerkSpeed"), _maxJerkSpeed, false);
		DumpArray<steprate_t, NUM_AXIS>(F("TimerMax"), _timerMax, false);
		DumpArray<steprate_t, NUM_AXIS>(F("TimerAcc"), _timerAcc, false);
		DumpArray<steprate_t, NUM_AXIS>(F("TimerDec"), _timerDec, true);
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
#ifndef REDUCED_SIZE

	DumpType<unsigned char>(F("Idx"), idx, false);
	if (idx == 0)
	{
		_pStepper->_movementstate.Dump(options);
	}
	DumpType<udist_t>(F("Steps"), _steps, false);
	DumpType<udist_t>(F("State"), _state, false);

	DumpType<DirCountAll_t>(F("DirCount"), _dirCount.all, false);
	DumpType<DirCountAll_t>(F("LastDirCount"), _lastStepDirCount.all, false);
	DumpArray<mdist_t, NUM_AXIS>(F("Dist"), _distance_, false);
	DumpType<mdist_t>(F("UpSteps"), _upSteps, false);
	DumpType<mdist_t>(F("DownSteps"), _downSteps, false);
	DumpType<mdist_t>(F("DownStartAt"), _downStartAt, false);
	DumpType<mdist_t>(F("UpOffset"), _nUpOffset, false);
	DumpType<mdist_t>(F("DownOffset"), _nDownOffset, false);

	DumpType<timer_t>(F("tMax"), _timerMax, false);
	DumpType<timer_t>(F("tRun"), _timerRun, false);
	DumpType<timer_t>(F("tStart"), _timerStart, false);
	DumpType<timer_t>(F("tStop"), _timerStop, false);
	DumpType<timer_t>(F("tEndPossible"), _timerEndPossible, false);
	DumpType<timer_t>(F("tJunctionToPrev"), _timerJunctionToPrev, false);
	DumpType<timer_t>(F("tMaxJunction"), _timerMaxJunction, false);

	if (options&DumpDetails)
	{
		DumpType<axis_t>(F("UpAxis"), _upAxis, false);
		DumpType<axis_t>(F("DownAxis"), _downAxis, false);
	}

	StepperSerial.println();
#endif
}

////////////////////////////////////////////////////////

void CStepper::SMovementState::Dump(unsigned char /* options */)
{
#ifndef REDUCED_SIZE
	DumpType<mdist_t>(F("n"), _n, false);
	DumpType<timer_t>(F("t"), _timer, false);
	DumpType<timer_t>(F("r"), _rest, false);
	DumpType<unsigned long>(F("sum"), _sumTimer, false);
	DumpArray<mdist_t, NUM_AXIS>(F("a"), _add, false);
#endif
}
