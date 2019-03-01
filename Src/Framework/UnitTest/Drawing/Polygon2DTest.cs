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

namespace Framework.UnitTest.Drawing
{
    using FluentAssertions;

    using Framework.Drawing;

    using Xunit;

    public class Polygon2DTest
    {
        private static Polygon2D CreateClosedPolygon()
        {
            return new Polygon2D
            {
                Points = new[]
                {
                    new Point2D { X = 0, Y = 0 }, new Point2D { X = 100, Y = 0 }, new Point2D { X = 100, Y = 100 }, new Point2D { X = 0, Y = 100 }, new Point2D { X = 0, Y = 0 }
                }
            };
        }

        private static Polygon2D CreateOpenPolygon()
        {
            return new Polygon2D
            {
                Points = new[]
                {
                    new Point2D { X = 0, Y = 0 }, new Point2D { X = 100, Y = 0 }, new Point2D { X = 100, Y = 100 }, new Point2D { X = 0, Y = 100 }
                }
            };
        }

        [Fact]
        public void PointInPolygon()
        {
            Polygon2D polygon = CreateClosedPolygon();
            polygon.IsPointInPolygon(new Point2D { X = 50, Y = 50 }).Should().Be(true);
        }

        [Fact]
        public void PointInPolygonOnLine()
        {
            Polygon2D polygon = CreateClosedPolygon();
            polygon.IsPointInPolygon(new Point2D { X = 100, Y = 2 }).Should().Be(true, "point on line is in polygon");
        }

        [Fact]
        public void PointNotInPolygon()
        {
            Polygon2D polygon = CreateClosedPolygon();
            polygon.IsPointInPolygon(new Point2D { X = 1, Y = 100.5 }).Should().Be(false);
            polygon.IsPointInPolygon(new Point2D { X = 100.5, Y = 1 }).Should().Be(false);
            polygon.IsPointInPolygon(new Point2D { X = -0.5, Y = 1 }).Should().Be(false);
            polygon.IsPointInPolygon(new Point2D { X = 1, Y = -0.5 }).Should().Be(false);
        }

        [Fact]
        public void MinMax()
        {
            Polygon2D polygon = CreateClosedPolygon();
            polygon.MaxX.Should().BeApproximately(100.0, double.Epsilon);
            polygon.MaxY.Should().BeApproximately(100.0, double.Epsilon);
            polygon.MinX.Should().BeApproximately(0.0, double.Epsilon);
            polygon.MinY.Should().BeApproximately(0.0, double.Epsilon);
        }

        [Fact]
        public void OpenClosedPolygon()
        {
            Polygon2D polygonClosed = CreateClosedPolygon();
            Polygon2D polygonOpen   = CreateOpenPolygon();
            polygonClosed.IsClosed.Should().Be(true);
            polygonOpen.IsClosed.Should().Be(false);
        }
    }
}