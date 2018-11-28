/*
 * This file is part of Hofer Framework
 * 
 * Copyright (c) Hofer KG 2018
 * 
*/

namespace Framework.Contract
{
    using System;

    public class YearMonth
    {
        public YearMonth()
        {
        }

        public YearMonth(int year, int month)
        {
            Year  = year;
            Month = month;
        }

        public YearMonth(DateTime date)
        {
            Year  = date.Year;
            Month = date.Month;
        }

        public int Year { get; set; }

        public int Month { get; set; }
    }
}