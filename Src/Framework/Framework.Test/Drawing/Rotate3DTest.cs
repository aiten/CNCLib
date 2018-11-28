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

namespace Framework.Test.Drawing
{
    using System;

    using FluentAssertions;

    using Framework.Drawing;

    using Xunit;

    public class Rotate3DTest
    {
        [Fact]
        public void NoRotateAngle0()
        {
            var r      = new Rotate3D();
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptDest = r.Rotate(ptSrc);

            (ptDest.X ?? throw new ArgumentException()).Should().Be(1.0);
            (ptDest.Y ?? throw new ArgumentException()).Should().Be(2.0);
            (ptDest.Z ?? throw new ArgumentException()).Should().Be(3.0);
        }

        [Fact]
        public void RotateZAngle180Grad()
        {
            var r      = new Rotate3D(Math.PI, new[] {0, 0, 1.0});
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptDest = r.Rotate(ptSrc);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 5).Should().Be(-1.0);
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 5).Should().Be(-2.0);
            (ptDest.Z ?? throw new ArgumentException()).Should().Be(3.0);
        }

        [Fact]
        public void RotateZAngleMinus180Grad()
        {
            var r      = new Rotate3D(-Math.PI, new[] {0, 0, 1.0});
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptDest = r.Rotate(ptSrc);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 5).Should().Be(-1.0);
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 5).Should().Be(-2.0);
            (ptDest.Z ?? throw new ArgumentException()).Should().Be(3.0);
        }

        [Fact]
        public void RotateZAngle90Grad()
        {
            var r      = new Rotate3D(Math.PI / 2.0, new[] {0, 0, 1.0});
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptDest = r.Rotate(ptSrc);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 5).Should().Be(-2.0);
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 5).Should().Be(1.0);
            (ptDest.Z ?? throw new ArgumentException()).Should().Be(3.0);
        }

        [Fact]
        public void RotateZAngleMinus90Grad()
        {
            var r      = new Rotate3D(-Math.PI / 2.0, new[] {0, 0, 1.0});
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptDest = r.Rotate(ptSrc);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 5).Should().Be(2.0);
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 5).Should().Be(-1.0);
            (ptDest.Z ?? throw new ArgumentException()).Should().Be(3.0);
        }

        [Fact]
        public void RotateXAngle90Grad()
        {
            var r      = new Rotate3D(Math.PI / 2.0, new[] {1.0, 0, 0});
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptDest = r.Rotate(ptSrc);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 5).Should().Be(1.0);
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 5).Should().Be(-3.0);
            Math.Round((ptDest.Z ?? throw new ArgumentException()), 5).Should().Be(2.0);
        }

        [Fact]
        public void RotateXAngleMinus90Grad()
        {
            var r      = new Rotate3D(-Math.PI / 2.0, new[] {1.0, 0, 0});
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptDest = r.Rotate(ptSrc);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 5).Should().Be(1.0);
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 5).Should().Be(3.0);
            Math.Round((ptDest.Z ?? throw new ArgumentException()), 5).Should().Be(-2.0);
        }

        [Fact]
        public void RotateYAngle90Grad()
        {
            var r      = new Rotate3D(Math.PI / 2.0, new[] {0, 1.0, 0});
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptDest = r.Rotate(ptSrc);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 5).Should().Be(-3.0);
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 5).Should().Be(2.0);
            Math.Round((ptDest.Z ?? throw new ArgumentException()), 5).Should().Be(1.0);
        }

        [Fact]
        public void RotateYAngleMinus90Grad()
        {
            var r      = new Rotate3D(-Math.PI / 2.0, new[] {0, 1.0, 0});
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptDest = r.Rotate(ptSrc);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 5).Should().Be(3.0);
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 5).Should().Be(2.0);
            Math.Round((ptDest.Z ?? throw new ArgumentException()), 5).Should().Be(-1.0);
        }

        [Fact]
        public void RotateXyzAngle90Grad()
        {
            var r      = new Rotate3D(Math.PI / 2.0, new[] {1.0, 1.0, 1.0});
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptDest = r.Rotate(ptSrc);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 10).Should().Be(Math.Round(-2.2200846792814621, 10));
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 10).Should().Be(Math.Round(-1.8213672050459184, 10));
            Math.Round((ptDest.Z ?? throw new ArgumentException()), 10).Should().Be(Math.Round(2.3987174742355446,  10));
        }

        [Fact]
        public void RotateXyzAngle90GradAndBack()
        {
            var r1     = new Rotate3D(Math.PI / 2.0,  new[] {1.0, 1.0, 1.0});
            var r2     = new Rotate3D(-Math.PI / 2.0, new[] {1.0, 1.0, 1.0});
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptRot  = r1.Rotate(ptSrc);
            var ptDest = r2.Rotate(ptRot);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 10).Should().Be(1.0);
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 10).Should().Be(2.0);
            Math.Round((ptDest.Z ?? throw new ArgumentException()), 10).Should().Be(3.0);
        }
    }
}