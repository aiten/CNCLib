/*
 * This file is part of Hofer Framework
 * 
 * Copyright (c) Hofer KG 2018
 * 
*/

namespace Framework.Logging
{
    using Abstraction;

    /// <summary>
    /// Implementation class for ILogger of "Type" with NLog.
    /// </summary>
    /// <typeparam name="TType">Type for logger.</typeparam>
    public sealed class Logger<TType> : Logger, ILogger<TType>
    {
        public Logger()
            : base(typeof(TType))
        {
        }
    }
}