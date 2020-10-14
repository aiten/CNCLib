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

namespace CNCLib.Logic.Job
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Framework.Schedule;

    using CNCLib.Logic.Abstraction;

    using Microsoft.Extensions.Logging;

    public sealed class CleanupJob : ICleanupJob
    {
        private readonly ILogger  _logger;
        private readonly JobState _jobState;

        public object            State  { get; set; }
        public CancellationToken CToken { get; set; }

        public CleanupJob(ILogger<CleanupJob> logger, JobState jobState)
        {
            _logger   = logger;
            _jobState = jobState;
        }

        public async Task Execute()
        {
            try
            {
                _logger.LogInformation($"Background Task: {State},{_jobState.State}");
                await Task.Delay(1000, CToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not cleanup.");
            }
        }

        public async Task SetContext()
        {
            await Task.CompletedTask;
        }
    }
}