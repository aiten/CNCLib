////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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

using FluentAssertions;
using Framework.Tools.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Framework.Test.Drawing
{
    [TestClass]
    public class Polygon2DTest
    {
        private static Polygon2D CreateClosedPolygon()
        {
            return new Polygon2D
            {
                Points = new[]
                {
                    new Point2D { X = 0, Y   = 0 },
                    new Point2D { X = 100, Y = 0 },
                    new Point2D { X = 100, Y = 100 },
                    new Point2D { X = 0, Y   = 100 },
                    new Point2D { X = 0, Y   = 0 }
                }
            };
        }

        private static Polygon2D CreateOpenPolygon()
        {
            return new Polygon2D
            {
                Points = new[]
                {
                    new Point2D { X = 0, Y   = 0 },
                    new Point2D { X = 100, Y = 0 },
                    new Point2D { X = 100, Y = 100 },
                    new Point2D { X = 0, Y   = 100 }
                }
            };
        }

        [TestMethod]
        public void PointInPolygon()
        {
            Polygon2D polygon = CreateClosedPolygon();
            polygon.IsPointInPolygon(new Point2D { X = 50, Y = 50 }).Should().Be(true);
        }

        [TestMethod]
        public void PointInPolygonOnLine()
        {
            Polygon2D polygon = CreateClosedPolygon();
            polygon.IsPointInPolygon(new Point2D { X = 100, Y = 2 }).Should().Be(true, "point on line is in polygon");
        }

        [TestMethod]
        public void PointNotInPolygon()
        {
            Polygon2D polygon = CreateClosedPolygon();
            polygon.IsPointInPolygon(new Point2D { X = 1, Y = 100.5 }).Should().Be(false);
            polygon.IsPointInPolygon(new Point2D { X = 100.5, Y = 1 }).Should().Be(false);
            polygon.IsPointInPolygon(new Point2D { X = -0.5, Y = 1 }).Should().Be(false);
            polygon.IsPointInPolygon(new Point2D { X = 1, Y = -0.5 }).Should().Be(false);
        }

        [TestMethod]
        public void MinMax()
        {
            Polygon2D polygon = CreateClosedPolygon();
            polygon.MaxX.Should().BeApproximately(100.0, double.Epsilon);
            polygon.MaxY.Should().BeApproximately(100.0, double.Epsilon);
            polygon.MinX.Should().BeApproximately(0.0, double.Epsilon);
            polygon.MinY.Should().BeApproximately(0.0, double.Epsilon);
        }

        [TestMethod]
        public void OpenClosedPolygon()
        {
            Polygon2D polygonClosed = CreateClosedPolygon();
            Polygon2D polygonOpen   = CreateOpenPolygon();
            polygonClosed.IsClosed.Should().Be(true);
            polygonOpen.IsClosed.Should().Be(false);
        }
    }
}