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
        public DateTime Now => DateTime.Now;

        public DateTime ToDay => DateTime.Today;
    }
}