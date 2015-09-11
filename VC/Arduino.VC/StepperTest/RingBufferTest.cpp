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

#include "stdafx.h"

#include "RingBufferTest.h"
#include "..\MsvcStepper\MsvcStepper.h"

struct SRingbuffer
{
	double d=0;
	int i=0;
};


void CRingBufferTest::RunTest()
{
	CRingBufferQueue<SRingbuffer, 128> buffer;

	Assert(true, buffer.IsEmpty());

	buffer.NextTail().i = 4711;
	buffer.NextTail().d = 4711.4711;

	Assert(true, buffer.IsEmpty());

	buffer.Enqueue();

	Assert(false, buffer.IsEmpty());
	Assert(1, buffer.Count());

	buffer.NextTail().i = 4712;
	buffer.NextTail().d = 4712.4712;

	buffer.Enqueue();

	Assert(false, buffer.IsEmpty());
	Assert(2, buffer.Count());

	TestRingBufferInsert(10, 60, 0);		// insert at head

	TestRingBufferInsert(10, 60, 60);		// insert at tail (simple enqueue)
	TestRingBufferInsert(10, 60, 59);		// insert at tail-1

	TestRingBufferInsert(0, 60, 30);
	TestRingBufferInsert(128-10, 60, 30);	// buffer overrun

}

void CRingBufferTest::TestRingBufferInsert(unsigned char startidx, unsigned char buffersize, unsigned char insertoffset)
{
	CRingBufferQueue<SRingbuffer, 128> buffer;

	unsigned char i;

	for (i = 0; i < startidx; i++)
	{
		buffer.Enqueue();
		buffer.Dequeue();
	}

	for (i = 0; i < buffersize; i++)
	{
		buffer.NextTail().i = i;
		buffer.Enqueue();
	}

	Assert(buffersize, buffer.Count());

	unsigned char insertat = buffer.NextIndex(buffer.GetHeadPos(), insertoffset);
	buffer.InsertTail(insertat)->i = 2000;

	Assert(buffersize + 1, buffer.Count());

	int expect = 0;

	for (unsigned char idx = buffer.H2TInit(); buffer.H2TTest(idx); idx = buffer.H2TInc(idx))
	{
		if (idx != insertat)
			Assert(expect++,buffer.Buffer[idx].i);
		else
			Assert(2000, buffer.Buffer[idx].i);
	}
}