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

namespace Framework.Test.Tools
{
    using System.Linq;

    using FluentAssertions;

    using Framework.Tools;

    using Xunit;

    public class ListExtensionTest
    {
        #region Split

        [Fact]
        public void SplitEmptyList()
        {
            int[] testArray = { };

            var result = testArray.Split(100);

            result.Should().NotBeNull();
            result.Count().Should().Be(0);
        }

        [Fact]
        public void SplitElementSize1()
        {
            int[] testArray = { 1, 2, 3, 4 };

            var result = testArray.Split(1);

            result.Should().NotBeNull();
            result.Count().Should().Be(testArray.Length);
        }

        [Fact]
        public void SplitElementNoRemainingList()
        {
            int[] testArray = { 1, 2, 3, 4 };

            var result = testArray.Split(2);

            result.Should().NotBeNull();
            result.Count().Should().Be(testArray.Length / 2);
        }

        [Fact]
        public void SplitElementRemainingList()
        {
            int[] testArray = { 1, 2, 3, 4, 5 };

            var result = testArray.Split(2);

            result.Should().NotBeNull();
            result.Count().Should().Be(3);
        }

        [Fact]
        public void SplitElementOrder()
        {
            int[] testArray = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            var result = testArray.Split(3);

            int compareValue = 1;

            foreach (var list in result)
            {
                foreach (var element in list)
                {
                    element.Should().Be(compareValue++);
                }
            }
        }

        #endregion

        #region SplitBefore

        [Fact]
        public void SplitBeforeEmptyList()
        {
            int[] testArray = { };

            var result = testArray.SplitBefore((e) => e > 100);

            result.Should().NotBeNull();
            result.Count().Should().Be(0);
        }

        [Fact]
        public void SplitBeforeElementOrder()
        {
            int[] testArray = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            var result = testArray.SplitBefore((e) => e % 2 == 0);

            int compareValue = 1;

            foreach (var list in result)
            {
                foreach (var element in list)
                {
                    element.Should().Be(compareValue++);
                }
            }
        }

        [Fact]
        public void SplitBeforeElementFirst1()
        {
            int[] testArray = { 1, 2 };

            var result = testArray.SplitBefore((e) => e % 2 == 0).ToList();

            result.Count.Should().Be(2);
            result[0].Count().Should().Be(1);
            result[0].ElementAt(0).Should().Be(1);
            result[1].ElementAt(0).Should().Be(2);
        }

        [Fact]
        public void SplitBeforeElementFirst2()
        {
            int[] testArray = { 1, 2 };

            var result = testArray.SplitBefore((e) => e % 2 != 0).ToList();

            result.Count.Should().Be(1);
            result[0].Count().Should().Be(2);
            result[0].ElementAt(0).Should().Be(1);
            result[0].ElementAt(1).Should().Be(2);
        }

        #endregion

        #region SplitAfter

        [Fact]
        public void SplitAfterEmptyList()
        {
            int[] testArray = { };

            var result = testArray.SplitAfter((e) => e > 100);

            result.Should().NotBeNull();
            result.Count().Should().Be(0);
        }

        [Fact]
        public void SplitAfterElementOrder()
        {
            int[] testArray = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            var result = testArray.SplitAfter((e) => e % 2 == 0);

            int compareValue = 1;

            foreach (var list in result)
            {
                foreach (var element in list)
                {
                    element.Should().Be(compareValue++);
                }
            }
        }

        [Fact]
        public void SplitAfterElementFirst1()
        {
            int[] testArray = { 1, 2 };

            var result = testArray.SplitAfter((e) => e % 2 != 0).ToList();

            result.Count.Should().Be(2);
            result[0].Count().Should().Be(1);
            result[0].ElementAt(0).Should().Be(1);
            result[1].ElementAt(0).Should().Be(2);
        }

        [Fact]
        public void SplitAfterElementFirst2()
        {
            int[] testArray = { 1, 2 };

            var result = testArray.SplitAfter((e) => e % 2 == 0).ToList();

            result.Count.Should().Be(1);
            result[0].Count().Should().Be(2);
            result[0].ElementAt(0).Should().Be(1);
            result[0].ElementAt(1).Should().Be(2);
        }

        #endregion
    }
}