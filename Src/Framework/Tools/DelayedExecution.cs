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

namespace Framework.Tools
{
    using System;
    using System.Threading;

    public class DelayedExecution
    {
        Timer _timer;
        bool  _pendingTimer = false;
        bool  _needExecute  = false;

        public void Execute(int delayedMs, Action set, Action execute)
        {
            if (_timer == null)
            {
                // first call, start timer
                _pendingTimer = true;
                _needExecute  = false;
                set();
                execute();

                _timer = new Timer(
                    (x) =>
                    {
                        if (_needExecute)
                        {
                            // "Execute" and start timer again
                            _needExecute = false;
                            execute();
                            _timer.Change(delayedMs, Timeout.Infinite);
                        }
                        else
                        {
                            _pendingTimer = false;
                        }
                    },
                    this,
                    delayedMs,
                    Timeout.Infinite);
            }
            else if (_pendingTimer)
            {
                // last "execute" within delayedMS => do not call execute but "set" new values
                _needExecute = true;
                set();
            }
            else
            {
                // restart again
                _pendingTimer = true;
                _needExecute  = false;
                set();
                execute();
                _timer.Change(delayedMs, Timeout.Infinite);
            }
        }
    }
}