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
  http://www.gnu.org/licenses/
*/
////////////////////////////////////////////////////////

#pragma once

//////////////////////////////////////////

#include "HAL.h"

//////////////////////////////////////////

template <class T, const uint8_t maxsize>		// do not use maxsize > 254 (255 is used internal)
class CRingBufferQueue								// maxxsize should be 2^n (because of % operation)
{
public:

	CRingBufferQueue()
	{
		Clear();
	}

	void Dequeue()
	{
		CCriticalRegion crit;
		_head = NextIndex(_head);
		_empty = _head == _nexttail;
	}

	void Enqueue()
	{
		CCriticalRegion crit;
		_nexttail = NextIndex(_nexttail);
		_empty = false;
	}

	void Enqueue(T value)
	{
		Buffer[GetNextTailPos()] = value;
		Enqueue();
	}

	void EnqueueCount(uint8_t cnt)
	{
		CCriticalRegion crit;
		_nexttail = NextIndex(_nexttail, cnt);
		_empty = false;
	}

	void RemoveTail()
	{
		CCriticalRegion crit;
		_nexttail = PrevIndex(_nexttail);
		_empty = _head == _nexttail;
	}

	void RemoveTail(uint8_t tail)
	{
		CCriticalRegion crit;
		_nexttail = NextIndex(tail);
		_empty = _head == _nexttail;
	}

	bool IsEmpty() const
	{
		return _empty;
	}

	bool IsFull() const
	{
		return !_empty && _head == _nexttail;
	}

	uint8_t Count() const
	{
		if (_empty)
			return 0;

		if (_head < _nexttail)
		{
			return _nexttail - _head;
		}
		return maxsize - _head + _nexttail;
	}

	uint8_t FreeCount() const
	{
		return maxsize - Count();
	}

	T* InsertTail(uint8_t insertat)
	{
		if (IsInQueue(insertat))
		{
			for (uint8_t idx = T2HInit(); T2HTest(idx); idx = T2HInc(idx))
			{
				Buffer[NextIndex(idx)] = Buffer[idx];
				if (insertat == idx)
					break;
			}
		}
		else
		{
			// add tail
			insertat = GetNextTailPos();
		}

		Enqueue();
		return &Buffer[insertat];
	}


	// next functions no check if empty or full

	T& Head()                       { return Buffer[GetHeadPos()]; }
	T& Tail()                       { return Buffer[GetTailPos()]; }
	T& NextTail()                   { return Buffer[GetNextTailPos()]; }
	T& NextTail(uint8_t ofs)  { return Buffer[NextIndex(GetNextTailPos(), ofs)]; }

	T* SaveTail()                   { return IsEmpty() ? 0 : &Tail(); }
	T* SaveHead()                   { return IsEmpty() ? 0 : &Head(); }

	T* GetNext(uint8_t idx)	{
		idx = NextIndex(idx);
		return idx != _nexttail && IsInQueue(idx) ? &Buffer[idx] : NULL;
	}
	T* GetPrev(uint8_t idx)	
	{
		if (idx == _head) return NULL;
		idx = PrevIndex(idx);
		return IsInQueue(idx) ? &Buffer[idx] : NULL;
	}
	uint8_t GetHeadPos() const		{ return _head; }
	uint8_t GetNextTailPos() const	{ return _nexttail; }
	uint8_t GetTailPos() const		{ return PrevIndex(_nexttail); }

	bool IsInQueue(uint8_t idx) const	
	{
		if (_empty) return false;
		if (_nexttail == _head) return true;
		if (_nexttail > _head) return idx >= _head && idx < _nexttail;
		return idx >= _head || idx < _nexttail;
	}

	// iteration from head to tail (H2T)
	uint8_t H2TInit() const					{ return  _empty ? 255 : _head; }
	bool H2TTest(uint8_t idx) const			{ return  idx != 255; }
	uint8_t H2TInc(uint8_t idx) const	{ idx = NextIndex(idx); return idx == _nexttail ? 255 : idx; }

	// iteration from tail to head (T2H)
	uint8_t T2HInit() const					{ return  _empty ? 255 : GetTailPos(); }
	bool T2HTest(uint8_t idx) const			{ return  idx != 255; }
	uint8_t T2HInc(uint8_t idx) const	{ return idx == _head ? 255 : PrevIndex(idx); }

	void Clear()
	{
		CCriticalRegion crit;
		_head = 0;
		_nexttail = 0;
		_empty = true;
	}

private:
	// often accessed members first => is faster

	volatile uint8_t	_head;      // index of head of queue
	volatile uint8_t	_nexttail;  // index of next free tail (NOT tail position)
	volatile bool			_empty;		// distinguish between full and empty

public:

	T Buffer[maxsize];

	////////////////////////////////////////////////////////

public:

	uint8_t NextIndex(uint8_t idx) const
	{
		return (uint8_t)((idx + 1)) % (maxsize);
	}

	uint8_t NextIndex(uint8_t idx, uint8_t count) const
	{
		return (uint8_t)((idx + count)) % (maxsize);
	}

	uint8_t PrevIndex(uint8_t idx) const
	{
		return idx == 0 ? (maxsize - 1) : (idx - 1);
	}

	uint8_t PrevIndex(uint8_t idx, uint8_t count) const
	{
		return (idx >= count) ? idx - count : (maxsize)-(count - idx);
	}
};
