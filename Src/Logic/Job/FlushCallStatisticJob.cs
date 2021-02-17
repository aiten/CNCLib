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
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CNCLib.Logic.Abstraction;
    using CNCLib.Logic.Statistics;
    using CNCLib.Repository.Abstraction;

    using Framework.Repository.Abstraction;

    using Microsoft.Extensions.Logging;

    public sealed class FlushCallStatisticJob : IFlushCallStatisticJob
    {
        private readonly IUserRepository    _repository;
        private readonly CallStatisticCache _callStatistic;
        private readonly IUnitOfWork        _uow;
        private readonly ILogger            _logger;

        public string            JobName { get; set; }
        public object            Param   { get; set; }
        public CancellationToken CToken  { get; set; }

        public FlushCallStatisticJob(IUnitOfWork uow, IUserRepository repository, CallStatisticCache callStatistic, ILogger<FlushCallStatisticJob> logger)
        {
            _logger        = logger;
            _repository    = repository;
            _callStatistic = callStatistic;
            _uow           = uow;
        }

        public async Task Execute()
        {
            try
            {
                _logger.LogInformation($"Background Task: {JobName}");

                var callStat = _callStatistic.GetCallStatisticsAndClear();

                if (callStat.Any())
                {
                    // we aggregate the counts for different hours => do not store a record overlapping two hours (see consolidation job)

                    var callsPerUser = callStat.GroupBy(
                        c => c.UserId,
                        (k, c) => new { UserId = k, LastLogin = c.Max(cc => cc.CallTime) }
                    ).ToList();

                    using (var trans = _uow.BeginTransaction())
                    {
                        var userEntities = await _repository.GetTracking(callsPerUser.Select(c => c.UserId));
                        var joinedUsers = callsPerUser.Join(
                            userEntities,
                            u => u.UserId,
                            c => c.UserId,
                            (c, u) => new { User = u, Call = c });
                        foreach (var joinedUser in joinedUsers)
                        {
                            if (!joinedUser.User.LastLogin.HasValue || joinedUser.Call.LastLogin >= joinedUser.User.LastLogin)
                            {
                                joinedUser.User.LastLogin = joinedUser.Call.LastLogin;
                            }
                        }

                        await trans.CommitTransactionAsync();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Could not: {JobName}.");
            }
        }

        public async Task SetContext()
        {
            await Task.CompletedTask;
        }
    }
}