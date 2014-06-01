#pragma once

//////////////////////////////////////////

#include "Utilities.h"

//////////////////////////////////////////

template <class T, const unsigned char maxsize>		// do not use maxsize > 254 (255 is used internal)
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

	void Enqueue(unsigned char cnt)
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

	bool IsEmpty() const
	{
		return _empty;
	}

	bool IsFull() const
	{
		return !_empty && _head == _nexttail;
	}

	unsigned char Count() const
	{
		if (_empty)
			return 0;

		if (_head < _nexttail)
		{
			return _nexttail - _head;
		}
		return maxsize - _head + _nexttail;
	}

	unsigned char FreeCount() const
	{
		return maxsize - Count();
	}

	// next functions no check if empty or full

	T& Head()                       { return Buffer[GetHeadPos()]; }
	T& Tail()                       { return Buffer[GetTailPos()]; }
	T& NextTail()                   { return Buffer[GetNextTailPos()]; }
	T& NextTail(unsigned char ofs)  { return Buffer[NextIndex(GetNextTailPos(), ofs)]; }

	T* SaveTail()                   { return IsEmpty() ? 0 : &Tail(); }
	T* SaveHead()                   { return IsEmpty() ? 0 : &Head(); }

	T* GetNext(unsigned char idx)	{
		idx = NextIndex(idx);
		return idx != _nexttail && IsInQueue(idx) ? &Buffer[idx] : NULL;
	}
	T* GetPrev(unsigned char idx)	{
		if (idx == _head) return NULL;
		idx = PrevIndex(idx);
		return IsInQueue(idx) ? &Buffer[idx] : NULL;
	}
	unsigned char GetHeadPos() const		{ return _head; }
	unsigned char GetNextTailPos() const	{ return _nexttail; }
	unsigned char GetTailPos() const		{ return PrevIndex(_nexttail); }

	bool IsInQueue(unsigned char idx) const	{
		if (_empty) return false;
		if (_nexttail == _head) return true;
		if (_nexttail > _head) return idx >= _head && idx < _nexttail;
		return idx >= _head || idx < _nexttail;
	}

	// iteration from head to tail (H2T)
	unsigned char H2TInit() const					{ return  _empty ? 255 : _head; }
	bool H2TTest(unsigned char idx) const			{ return  idx != 255; }
	unsigned char H2TInc(unsigned char idx) const	{ idx = NextIndex(idx); return idx == _nexttail ? 255 : idx; }

	// iteration from tail to head (T2H)
	unsigned char T2HInit() const					{ return  _empty ? 255 : GetTailPos(); }
	bool T2HTest(unsigned char idx) const			{ return  idx != 255; }
	unsigned char T2HInc(unsigned char idx) const	{ return idx == _head ? 255 : PrevIndex(idx); }

	void Clear()
	{
		CCriticalRegion crit;
		_head = 0;
		_nexttail = 0;
		_empty = true;
	}

private:
	// often accessed members first => is faster

	volatile unsigned char	_head;      // index of head of queue
	volatile unsigned char	_nexttail;  // index of next free tail (NOT tail position)
	volatile bool			_empty;		// distinguish between full and empty

public:

	T Buffer[maxsize];

	////////////////////////////////////////////////////////

protected:

	unsigned char NextIndex(unsigned char idx) const
	{
		return (unsigned char)((idx + 1)) % (maxsize);
	}

	unsigned char NextIndex(unsigned char idx, unsigned char count) const
	{
		return (unsigned char)((idx + count)) % (maxsize);
	}

	unsigned char PrevIndex(unsigned char idx) const
	{
		return idx == 0 ? (maxsize - 1) : (idx - 1);
	}

	unsigned char PrevIndex(unsigned char idx, unsigned char count) const
	{
		return (idx >= count) ? idx - count : (maxsize)-(count - idx);
	}
};
