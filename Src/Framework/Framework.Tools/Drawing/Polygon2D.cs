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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Tools.Drawing
{
    public class Polygon2D
    {
        private Point2D[] _points = new Point2D[0];

        public IEnumerable<Point2D> Points { get => _points; set => _points = value.ToArray(); }

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