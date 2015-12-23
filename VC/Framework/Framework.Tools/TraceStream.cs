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

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Framework.Tools
{
    public class TraceStream : IDisposable
    {
        StreamWriter _traceStream;
        Thread _writeThread;
        List<string> _messagetoWrite = new List<string>();
        AutoResetEvent _autoEvent = new AutoResetEvent(false);

        int _flushcount = 0;

        public TimeSpan FlushTimeOut { get; set; } = new TimeSpan(0, 0, 0, 3, 0);       // 3 sec

        public void EnableTrace(string path)
        {
            _traceStream = new StreamWriter(path);
            _writeThread = new Thread(Write);
            _writeThread.Start();
        }

        private void Write()
        {
            while (_traceStream != null)
            {
                string msg = null;
                bool isEmpty = true;
                lock (_messagetoWrite)
                {
                    if (_messagetoWrite.Count > 0)
                    {
                        msg = _messagetoWrite[0];
                        _messagetoWrite.RemoveAt(0);
                        isEmpty = _messagetoWrite.Count == 0;
                    }
                }

                if (_traceStream != null)       // may be desposed while lock
                {
                    if (msg != null)
                    {
                        _traceStream.WriteLine(msg);
                    }
                    else
                    {
                        // no message => FlushTimeOut
                        _traceStream.Flush();
                        _flushcount++;
                    }

                    if (isEmpty)
                        _autoEvent.WaitOne(FlushTimeOut);
                }
            }
        }

        public void CloseTrace()
        {
            lock (_messagetoWrite)
            {
                if (_traceStream != null)
                {
                    _traceStream.Close();
                    _traceStream.Dispose();
                    _traceStream = null;
                    _autoEvent.Set();
                }
            }
        }

        public void Dispose()
        {
            CloseTrace();
        }

        public void WriteTrace(string type, string message)
        {
            if (_traceStream != null)
            {
                lock (_messagetoWrite)
                {
                    _messagetoWrite.Add(string.Format("{0}:\t{1}\t{2}", DateTime.Now, type, message));
                    _autoEvent.Set();
                }
            }
        }
        public void WriteTraceFlush(string type, string message)
        {
            if (_traceStream != null)
            {
                WriteTrace(type, message);
                ForceFlush();
            }
        }

        public void ForceFlush()
        {
            if (_traceStream != null)
            {
                int flushcount=_flushcount;
                while (flushcount == _flushcount)
                {
                    _autoEvent.Set();
                }
            }
        }
    }
}
