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

namespace Framework.WebAPI.Filter
{
    using System;
    using System.Net;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;

    public abstract class ExceptionFilterBase : ActionContextLoggerProvider, IExceptionFilter
    {
        protected ExceptionFilterBase(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception != null && !context.ExceptionHandled)
            {
                var response = GetResponse(context);
                if (response.HasValue)
                {
                    context.ExceptionHandled = true;
                    var errorResponseData = new ErrorResponseData(context.Exception);
                    var jsonResult = new JsonResult(errorResponseData)
                    {
                        StatusCode = (int)response.Value.StatusCode
                    };
                    context.Result = jsonResult;
                }
            }
        }

        protected abstract ExceptionResponse? GetResponse(ExceptionContext exceptionContext);

        protected struct ExceptionResponse
        {
            public ExceptionResponse(HttpStatusCode statusCode, Exception exception)
            {
                StatusCode = statusCode;
                Exception  = exception;
            }

            public HttpStatusCode StatusCode { get; }

            public Exception Exception { get; }
        }
    }
}