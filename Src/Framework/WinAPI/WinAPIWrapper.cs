/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

namespace Framework.WinAPI
{
    using System;
    using System.Runtime.InteropServices;

    public class WinAPIWrapper
    {
        [Flags]
        public enum EXECUTION_STATE : uint
        {
            ES_NONE             = 0,
            ES_CONTINUOUS       = 0x80000000,
            ES_SYSTEM_REQUIRED  = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern uint SetThreadExecutionState(EXECUTION_STATE esFlags);

        static uint OsSetThreadExecutionState(EXECUTION_STATE esFlags)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return SetThreadExecutionState(esFlags);
            }

            return uint.MaxValue;
        }

        public static void KeepAlive()
        {
            OsSetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
        }

        public static void AllowIdle()
        {
            OsSetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }

        public static void ResetTimer()
        {
            OsSetThreadExecutionState(EXECUTION_STATE.ES_NONE);
        }
    }
}