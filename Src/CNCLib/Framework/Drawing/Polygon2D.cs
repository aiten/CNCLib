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

namespace Framework.Drawing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Polygon2D
    {
        private Point2D[] _points = new Point2D[0];

        public IEnumerable<Point2D> Points
        {
            get => _points;
            set => _points = value.ToArray();
        }

        public bool   IsClosed => _points.Length >= 2 && _points[0].Compare(_points[_points.Length - 1]);
        public double MaxX     => _points.Max(c => c.X);
        public double MinX     => _points.Min(c => c.X);
        public double MaxY     => _points.Max(c => c.Y);
        public double MinY     => _points.Min(c => c.Y);

        #region PointInPolygon

        // http://alienryderflex.com/polygon/

        public bool IsPointInPolygon(Point2D pt)
        {
            double[] constant, multiple;
            PreCalcPointInPolygon(out constant, out multiple);
            return IsPointInPolygon(pt, constant, multiple);
        }

        public bool ArePointsInPolygon(IEnumerable<Point2D> pts)
        {
            double[] constant, multiple;
            PreCalcPointInPolygon(out constant, out multiple);
            foreach (var pt in pts)
            {
                if (!IsPointInPolygon(pt, constant, multiple))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsPointInPolygon(Point2D pt, double[] constant, double[] multiple)
        {
            int  i, j     = _points.Length - 1;
            bool oddNodes = false;

            for (i = 0; i < _points.Length; i++)
            {
                if ((_points[i].Y < pt.Y && _points[j].Y >= pt.Y || _points[j].Y < pt.Y && _points[i].Y >= pt.Y))
                {
                    oddNodes ^= (pt.Y * multiple[i] + constant[i] < pt.X);
                }

                j = i;
            }

            return oddNodes;
        }

        private void PreCalcPointInPolygon(out double[] constant, out double[] multiple)
        {
            int i, j = _points.Length - 1;

            constant = new double[_points.Length];
            multiple = new double[_points.Length];
            for (i = 0; i < _points.Length; i++)
            {
                if (Math.Abs(_points[j].Y - _points[i].Y) < double.Epsilon)
                {
                    constant[i] = _points[i].X;
                    multiple[i] = 0;
                }
                else
                {
                    constant[i] = _points[i].X - (_points[i].Y * _points[j].X) / (_points[j].Y - _points[i].Y) + (_points[i].Y * _points[i].X) / (_points[j].Y - _points[i].Y);
                    multiple[i] = (_points[j].X - _points[i].X) / (_points[j].Y - _points[i].Y);
                }

                j = i;
            }

            // return j;
        }

        #endregion
    }
}