////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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

using System;
using System.Text;
using System.Windows.Media;

namespace CNCLib.Wpf.Helpers
{
    public static class RandomHelper
    {
        private static readonly Random RandomSeed = new Random();

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
            var start = (lowerCase) ? 97 : 65;

            // Add random chars
            for (var i = 0; i < size; i++)
                randStr.Append((char)(26 * RandomSeed.NextDouble() + start));

            return randStr.ToString();
        }

        public static int RandomInt(int min, int max)
        {
            return RandomSeed.Next(min, max);
        }

        public static double RandomDouble()
        {
            return RandomSeed.NextDouble();
        }

        public static double RandomNumber(int min, int max, int digits)
        {
            return Math.Round(RandomSeed.Next(min, max - 1) + RandomSeed.NextDouble(), digits);
        }

        public static bool RandomBool()
        {
            return (RandomSeed.NextDouble() > 0.5);
        }

        public static DateTime RandomDate()
        {
            return RandomDate(new DateTime(1900, 1, 1), DateTime.Now);
        }

        public static DateTime RandomDate(DateTime from, DateTime to)
        {
            var range = new TimeSpan(to.Ticks - from.Ticks);
            return from + new TimeSpan((long)(range.Ticks * RandomSeed.NextDouble()));
        }

        public static Color RandomColor()
        {
            return Color.FromRgb((byte)RandomSeed.Next(255), (byte)RandomSeed.Next(255), (byte)RandomSeed.Next(255));
        }
    }
}
