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

namespace Framework.UnitTest.Tools
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