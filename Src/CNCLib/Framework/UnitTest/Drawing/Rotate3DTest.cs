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
            var r      = new Rotate3D(Math.PI, new[] { 0, 0, 1.0 });
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptDest = r.Rotate(ptSrc);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 5).Should().Be(-1.0);
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 5).Should().Be(-2.0);
            (ptDest.Z ?? throw new ArgumentException()).Should().Be(3.0);
        }

        [Fact]
        public void RotateZAngleMinus180Grad()
        {
            var r      = new Rotate3D(-Math.PI, new[] { 0, 0, 1.0 });
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptDest = r.Rotate(ptSrc);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 5).Should().Be(-1.0);
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 5).Should().Be(-2.0);
            (ptDest.Z ?? throw new ArgumentException()).Should().Be(3.0);
        }

        [Fact]
        public void RotateZAngle90Grad()
        {
            var r      = new Rotate3D(Math.PI / 2.0, new[] { 0, 0, 1.0 });
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptDest = r.Rotate(ptSrc);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 5).Should().Be(-2.0);
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 5).Should().Be(1.0);
            (ptDest.Z ?? throw new ArgumentException()).Should().Be(3.0);
        }

        [Fact]
        public void RotateZAngleMinus90Grad()
        {
            var r      = new Rotate3D(-Math.PI / 2.0, new[] { 0, 0, 1.0 });
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptDest = r.Rotate(ptSrc);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 5).Should().Be(2.0);
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 5).Should().Be(-1.0);
            (ptDest.Z ?? throw new ArgumentException()).Should().Be(3.0);
        }

        [Fact]
        public void RotateXAngle90Grad()
        {
            var r      = new Rotate3D(Math.PI / 2.0, new[] { 1.0, 0, 0 });
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptDest = r.Rotate(ptSrc);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 5).Should().Be(1.0);
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 5).Should().Be(-3.0);
            Math.Round((ptDest.Z ?? throw new ArgumentException()), 5).Should().Be(2.0);
        }

        [Fact]
        public void RotateXAngleMinus90Grad()
        {
            var r      = new Rotate3D(-Math.PI / 2.0, new[] { 1.0, 0, 0 });
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptDest = r.Rotate(ptSrc);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 5).Should().Be(1.0);
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 5).Should().Be(3.0);
            Math.Round((ptDest.Z ?? throw new ArgumentException()), 5).Should().Be(-2.0);
        }

        [Fact]
        public void RotateYAngle90Grad()
        {
            var r      = new Rotate3D(Math.PI / 2.0, new[] { 0, 1.0, 0 });
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptDest = r.Rotate(ptSrc);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 5).Should().Be(-3.0);
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 5).Should().Be(2.0);
            Math.Round((ptDest.Z ?? throw new ArgumentException()), 5).Should().Be(1.0);
        }

        [Fact]
        public void RotateYAngleMinus90Grad()
        {
            var r      = new Rotate3D(-Math.PI / 2.0, new[] { 0, 1.0, 0 });
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptDest = r.Rotate(ptSrc);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 5).Should().Be(3.0);
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 5).Should().Be(2.0);
            Math.Round((ptDest.Z ?? throw new ArgumentException()), 5).Should().Be(-1.0);
        }

        [Fact]
        public void RotateXyzAngle90Grad()
        {
            var r      = new Rotate3D(Math.PI / 2.0, new[] { 1.0, 1.0, 1.0 });
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptDest = r.Rotate(ptSrc);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 10).Should().Be(Math.Round(-2.2200846792814621, 10));
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 10).Should().Be(Math.Round(-1.8213672050459184, 10));
            Math.Round((ptDest.Z ?? throw new ArgumentException()), 10).Should().Be(Math.Round(2.3987174742355446,  10));
        }

        [Fact]
        public void RotateXyzAngle90GradAndBack()
        {
            var r1     = new Rotate3D(Math.PI / 2.0,  new[] { 1.0, 1.0, 1.0 });
            var r2     = new Rotate3D(-Math.PI / 2.0, new[] { 1.0, 1.0, 1.0 });
            var ptSrc  = new Point3D(1.0, 2.0, 3.0);
            var ptRot  = r1.Rotate(ptSrc);
            var ptDest = r2.Rotate(ptRot);

            Math.Round((ptDest.X ?? throw new ArgumentException()), 10).Should().Be(1.0);
            Math.Round((ptDest.Y ?? throw new ArgumentException()), 10).Should().Be(2.0);
            Math.Round((ptDest.Z ?? throw new ArgumentException()), 10).Should().Be(3.0);
        }
    }
}