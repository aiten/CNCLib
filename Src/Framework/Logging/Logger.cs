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

namespace Framework.Logging
{
    using System;

    using NLog;

    /// <summary>
    /// Implementation class for ILogger with NLog.
    /// </summary>
    public class Logger : Abstraction.ILogger
    {
        private readonly NLog.Logger _logger;

        public Logger(Type type)
        {
            _logger = LogManager.GetLogger(type.FullName);
        }

        public Logger(string key)
        {
            _logger = LogManager.GetLogger(key);
        }

        public void Trace(string message)
        {
            _logger.Trace(message);
        }

        public void Trace(Exception exception, string message)
        {
            _logger.Trace(exception, message);
        }

        public void Trace(Exception exception)
        {
            _logger.Trace(exception);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Debug(Exception exception, string message)
        {
            _logger.Debug(exception, message);
        }

        public void Debug(Exception exception)
        {
            _logger.Debug(exception);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Info(Exception exception, string message)
        {
            _logger.Info(exception, message);
        }

        public void Info(Exception exception)
        {
            _logger.Info(exception);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void Warn(Exception exception, string message)
        {
            _logger.Warn(exception, message);
        }

        public void Warn(Exception exception)
        {
            _logger.Warn(exception);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(Exception exception, string message)
        {
            _logger.Error(exception, message);
        }

        public void Error(Exception exception)
        {
            _logger.Error(exception);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Fatal(Exception exception, string message)
        {
            _logger.Fatal(exception, message);
        }

        public void Fatal(Exception exception)
        {
            _logger.Fatal(exception);
        }
    }
}