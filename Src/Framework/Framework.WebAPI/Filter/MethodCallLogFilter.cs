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

namespace Framework.WebAPI.Filter
{
    using System.Diagnostics;

    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;

    public sealed class MethodCallLogFilter : IActionFilter
    {
        private readonly ILogger<MethodCallLogFilter> _logger;

        private Stopwatch _stopWatch;

        public MethodCallLogFilter(ILogger<MethodCallLogFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Debug.Assert(_stopWatch == null, @"OnActionExecuted must be called");
            _stopWatch = Stopwatch.StartNew();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopWatch.Stop();
            _logger.LogDebug($"{context.ActionDescriptor.DisplayName} - {_stopWatch.ElapsedMilliseconds}ms");
        }
    }
}