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

namespace Framework.Tools
{
    using System;
    using System.Text;

    public static class RandomHelper
    {
        private static readonly Random _randomSeed = new Random();

        /// <summary>
        /// Generates a random string with the given length
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <param name="lowerCase">If true, generate lowercase string</param>
        /// <returns>Random string</returns>
        public static string RandomString(int size, bool lowerCase)
        {
            // StringBuilder is faster than using strings (+=)
            var randStr = new StringBuilder(size);

            // Ascii start position (65 = A / 97 = a)
            int start = lowerCase ? 97 : 65;

            // Add random chars
            for (var i = 0; i < size; i++)
            {
                randStr.Append((char)(26 * _randomSeed.NextDouble() + start));
            }

            return randStr.ToString();
        }

        public static int RandomInt(int min, int max)
        {
            return _randomSeed.Next(min, max);
        }

        public static double RandomDouble()
        {
            return _randomSeed.NextDouble();
        }

        public static double RandomNumber(int min, int max, int digits)
        {
            return Math.Round(_randomSeed.Next(min, max - 1) + _randomSeed.NextDouble(), digits);
        }

        public static bool RandomBool()
        {
            return _randomSeed.NextDouble() > 0.5;
        }

        public static DateTime RandomDate()
        {
            return RandomDate(new DateTime(1900, 1, 1), DateTime.Now);
        }

        public static DateTime RandomDate(DateTime from, DateTime to)
        {
            var range = new TimeSpan(to.Ticks - from.Ticks);
            return from + new TimeSpan((long)(range.Ticks * _randomSeed.NextDouble()));
        }

/*
        public static Color RandomColor()
        {
            return Color.FromRgb((byte)RandomSeed.Next(255), (byte)RandomSeed.Next(255), (byte)RandomSeed.Next(255));
        }
*/
    }
}