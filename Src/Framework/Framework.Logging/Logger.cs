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