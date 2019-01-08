////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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