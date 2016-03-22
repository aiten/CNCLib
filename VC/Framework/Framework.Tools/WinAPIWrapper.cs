﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Tools
{
    public class WinAPIWrapper
    {
        [Flags]
        public enum EXECUTION_STATE : uint
        {
            ES_CONTINUOUS = 0x80000000,
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002
        };

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern uint SetThreadExecutionState(EXECUTION_STATE esFlags);

        static public void KeepAlive()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
        }

        static public void AllowIdle()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }

    }
}