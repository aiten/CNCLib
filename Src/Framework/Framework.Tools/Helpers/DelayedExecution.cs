////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Framework.Tools.Helpers
{
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

                _timer = new Timer((x) =>
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
                }, this, delayedMs, Timeout.Infinite);
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