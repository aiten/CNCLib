using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Logic
{
	public class TraceStream : IDisposable
	{
		StreamWriter _traceStream;
		Thread _writeThread;
		List<string> _messagetoWrite = new List<string>();
		AutoResetEvent _autoEvent = new AutoResetEvent(false);

		public TimeSpan FlushTimeOut { get; set; } = new TimeSpan(0, 0, 0, 3, 0);		// 3 sec

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
				bool isEmpty=true;
				lock (_messagetoWrite)
				{
					if (_messagetoWrite.Count > 0)
					{
						msg = _messagetoWrite[0];
						_messagetoWrite.RemoveAt(0);
						isEmpty = _messagetoWrite.Count == 0;
                    }
				}

				if (_traceStream != null)		// may be desposed while lock
				{
					if (msg != null)
					{
						_traceStream.WriteLine(msg);
					}
					else
					{
						// no message => FlushTimeOut
						_traceStream.Flush();
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
			if (_traceStream!= null)
			{ 
				lock(_messagetoWrite)
				{
					_messagetoWrite.Add(string.Format("{0}:\t{1}\t{2}", DateTime.Now, type, message));
					_autoEvent.Set();
                }
			}
		}
	}
}
