////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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
using FluentAssertions;
using Framework.Tools.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Framework.Test.Drawing
{
    [TestClass]
    public class Rotate3DTest
    {
        [TestMethod]
        public void NoRotateAngle0()
        {
            var r = new Rotate3D();
			var pt_src = new Point3D(1.0, 2.0, 3.0);
			var pt_dest = r.Rotate(pt_src);

            pt_dest.X.Value.Should().Be(1.0);
            pt_dest.Y.Value.Should().Be(2.0);
            pt_dest.Z.Value.Should().Be(3.0);
		}

		[TestMethod]
		public void RotateZAngle180Grad()
		{
			var r = new Rotate3D(Math.PI,new double[] { 0, 0, 1.0 });
			var pt_src = new Point3D(1.0, 2.0, 3.0);
			var pt_dest = r.Rotate(pt_src);

			Assert.AreEqual(-1.0, Math.Round(pt_dest.X.Value,5));
			Assert.AreEqual(-2.0, Math.Round(pt_dest.Y.Value,5));
			Assert.AreEqual(3.0, pt_dest.Z.Value);

		}

		[TestMethod]
		public void RotateZAngleMinus180Grad()
		{
			var r = new Rotate3D(-Math.PI, new double[] { 0, 0, 1.0 });
			var pt_src = new Point3D(1.0, 2.0, 3.0);
			var pt_dest = r.Rotate(pt_src);

			Assert.AreEqual(-1.0, Math.Round(pt_dest.X.Value, 5));
			Assert.AreEqual(-2.0, Math.Round(pt_dest.Y.Value, 5));
			Assert.AreEqual(3.0, pt_dest.Z.Value);
		}

		[TestMethod]
		public void RotateZAngle90Grad()
		{
			var r = new Rotate3D(Math.PI/2.0, new double[] { 0, 0, 1.0 });
			var pt_src = new Point3D(1.0, 2.0, 3.0);
			var pt_dest = r.Rotate(pt_src);

			Assert.AreEqual(-2.0, Math.Round(pt_dest.X.Value, 5));
			Assert.AreEqual(1.0, Math.Round(pt_dest.Y.Value, 5));
			Assert.AreEqual(3.0, pt_dest.Z.Value);
		}
		[TestMethod]
		public void RotateZAngleMinus90Grad()
		{
			var r = new Rotate3D(-Math.PI / 2.0, new double[] { 0, 0, 1.0 });
			var pt_src = new Point3D(1.0, 2.0, 3.0);
			var pt_dest = r.Rotate(pt_src);

			Assert.AreEqual(2.0, Math.Round(pt_dest.X.Value, 5));
			Assert.AreEqual(-1.0, Math.Round(pt_dest.Y.Value, 5));
			Assert.AreEqual(3.0, pt_dest.Z.Value);
		}

		[TestMethod]
		public void RotateXAngle90Grad()
		{
			var r = new Rotate3D(Math.PI / 2.0, new double[] { 1.0, 0, 0 });
			var pt_src = new Point3D(1.0, 2.0, 3.0);
			var pt_dest = r.Rotate(pt_src);

			Assert.AreEqual(1.0, Math.Round(pt_dest.X.Value, 5));
			Assert.AreEqual(-3.0, Math.Round(pt_dest.Y.Value, 5));
			Assert.AreEqual(2.0, Math.Round(pt_dest.Z.Value,5));
		}
		[TestMethod]
		public void RotateXAngleMinus90Grad()
		{
			var r = new Rotate3D(-Math.PI / 2.0, new double[] { 1.0, 0, 0 });
			var pt_src = new Point3D(1.0, 2.0, 3.0);
			var pt_dest = r.Rotate(pt_src);

			Assert.AreEqual(1.0, Math.Round(pt_dest.X.Value, 5));
			Assert.AreEqual(3.0, Math.Round(pt_dest.Y.Value, 5));
			Assert.AreEqual(-2.0, Math.Round(pt_dest.Z.Value,5));
		}

		[TestMethod]
		public void RotateYAngle90Grad()
		{
			var r = new Rotate3D(Math.PI / 2.0, new double[] { 0, 1.0, 0 });
			var pt_src = new Point3D(1.0, 2.0, 3.0);
			var pt_dest = r.Rotate(pt_src);

			Assert.AreEqual(-3.0, Math.Round(pt_dest.X.Value, 5));
			Assert.AreEqual(2.0, Math.Round(pt_dest.Y.Value, 5));
			Assert.AreEqual(1.0, Math.Round(pt_dest.Z.Value, 5));
		}
		[TestMethod]
		public void RotateYAngleMinus90Grad()
		{
			var r = new Rotate3D(-Math.PI / 2.0, new double[] { 0, 1.0, 0 });
			var pt_src = new Point3D(1.0, 2.0, 3.0);
			var pt_dest = r.Rotate(pt_src);

			Assert.AreEqual(3.0, Math.Round(pt_dest.X.Value, 5));
			Assert.AreEqual(2.0, Math.Round(pt_dest.Y.Value, 5));
			Assert.AreEqual(-1.0, Math.Round(pt_dest.Z.Value, 5));
		}
		[TestMethod]
		public void RotateXYZAngle90Grad()
		{
			var r = new Rotate3D(Math.PI / 2.0, new double[] { 1.0, 1.0, 1.0 });
			var pt_src = new Point3D(1.0, 2.0, 3.0);
			var pt_dest = r.Rotate(pt_src);

			Assert.AreEqual(Math.Round(-2.2200846792814621,10), Math.Round(pt_dest.X.Value, 10));
			Assert.AreEqual(Math.Round(-1.8213672050459184, 10), Math.Round(pt_dest.Y.Value, 10));
			Assert.AreEqual(Math.Round(2.3987174742355446, 10), Math.Round(pt_dest.Z.Value, 10));
		}

		[TestMethod]
		public void RotateXYZAngle90GradAndBack()
		{
			var r1 = new Rotate3D(Math.PI / 2.0, new double[] { 1.0, 1.0, 1.0 });
			var r2 = new Rotate3D(-Math.PI / 2.0, new double[] { 1.0, 1.0, 1.0 });
			var pt_src = new Point3D(1.0, 2.0, 3.0);
			var pt_rot  = r1.Rotate(pt_src);
			var pt_dest = r2.Rotate(pt_rot);

			Assert.AreEqual(1.0, Math.Round(pt_dest.X.Value, 10));
			Assert.AreEqual(2.0, Math.Round(pt_dest.Y.Value, 10));
			Assert.AreEqual(3.0, Math.Round(pt_dest.Z.Value, 10));
		}
	}
}
