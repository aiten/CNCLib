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

#include "..\MsvcStepper\MsvcStepper.h"

#include "CppUnitTest.h"

////////////////////////////////////////////////////////

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace StepperSystemTest
{
	struct SRingbuffer
	{
		double d = 0;
		int i = 0;
	};

	TEST_CLASS(CRingBufferTest)
	{
	public:

		TEST_METHOD(RingBufferTest)
		{
			CRingBufferQueue<SRingbuffer, 128> buffer;

			Assert::AreEqual(true, buffer.IsEmpty());

			buffer.NextTail().i = 4711;
			buffer.NextTail().d = 4711.4711;

			Assert::AreEqual(true, buffer.IsEmpty());

			buffer.Enqueue();

			Assert::AreEqual(false, buffer.IsEmpty());
			Assert::AreEqual((uint8_t)1, buffer.Count());

			buffer.NextTail().i = 4712;
			buffer.NextTail().d = 4712.4712;

			buffer.Enqueue();

			Assert::AreEqual(false, buffer.IsEmpty());
			Assert::AreEqual((uint8_t)2, buffer.Count());
		}

		TEST_METHOD(RingBufferInsertHeadTest)
		{
			TestRingBufferInsert(10, 60, 0);		// insert at head
		}

		TEST_METHOD(RingBufferInsertTailTest)
		{
			TestRingBufferInsert(10, 60, 60);		// insert at tail (simple enqueue)
		}

		TEST_METHOD(RingBufferInsertTailM1Test)
		{
			TestRingBufferInsert(10, 60, 59);		// insert at tail-1
		}

		TEST_METHOD(RingBufferInsertTail2Test)
		{
			TestRingBufferInsert(0, 60, 30);
		}

		TEST_METHOD(RingBufferOverrunTest)
		{
			TestRingBufferInsert(128 - 10, 60, 30);	// buffer overrun
		}

		void TestRingBufferInsert(uint8_t startidx, uint8_t buffersize, uint8_t insertoffset)
		{
			CRingBufferQueue<SRingbuffer, 128> buffer;

			uint8_t i;

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

			Assert::AreEqual(buffersize, buffer.Count());

			uint8_t insertat = buffer.NextIndex(buffer.GetHeadPos(), insertoffset);
			buffer.InsertTail(insertat)->i = 2000;

			Assert::AreEqual((uint8_t)(buffersize + 1), buffer.Count());

			int expect = 0;

			for (uint8_t idx = buffer.H2TInit(); buffer.H2TTest(idx); idx = buffer.H2TInc(idx))
			{
				if (idx != insertat)
					Assert::AreEqual(expect++, buffer.Buffer[idx].i);
				else
					Assert::AreEqual(2000, buffer.Buffer[idx].i);
			}
		}
	};
}