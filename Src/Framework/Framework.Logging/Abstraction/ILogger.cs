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

namespace Framework.Logging.Abstraction
{
    using System;
    using System.ComponentModel;

    public interface ILogger
    {
        void Trace([Localizable(false)] string message);

        void Trace(Exception exception, [Localizable(false)] string message);

        void Trace(Exception exception);

        void Debug([Localizable(false)] string message);

        void Debug(Exception exception, [Localizable(false)] string message);

        void Debug(Exception exception);

        void Info([Localizable(false)] string message);

        void Info(Exception exception, [Localizable(false)] string message);

        void Info(Exception exception);

        void Warn([Localizable(false)] string message);

        void Warn(Exception exception, [Localizable(false)] string message);

        void Warn(Exception exception);

        void Error([Localizable(false)] string message);

        void Error(Exception exception, [Localizable(false)] string message);

        void Error(Exception exception);

        void Fatal([Localizable(false)] string message);

        void Fatal(Exception exception, [Localizable(false)] string message);

        void Fatal(Exception exception);
    }

    /// <summary>
    /// Helper interface that introduces a generic type so we can easily dependency inject 
    /// </summary>
    /// <typeparam name="TType">Type for which the logger is configured.</typeparam>
    /// <inheritdoc />
    public interface ILogger<TType> : ILogger
    {
    }
}