/*
 * This file is part of KoBe (KostenBenchmark)
 * 
 * Copyright (c) Hofer KG 2018
 * 
*/

using System;
using Framework.Contracts.Shared;

namespace Framework.Tools
{
    public class CurrentDateTime : ICurrentDateTime
    {
        public DateTime Now { get { return DateTime.Now; } }
        public DateTime ToDay { get { return DateTime.Today; } }
    }
}
