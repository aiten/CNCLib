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

using System.Collections.Generic;
using System.Linq;
using Framework.Tools.Drawing;

namespace CNCLib.GCode.Load
{
    public partial class LoadHPGL
    {
        private class HPGLLine
        {
            public IEnumerable<HPGLCommand> PreCommands { get; set; }
            public IEnumerable<HPGLCommand> Commands { get; set; }
            public IEnumerable<HPGLCommand> PostCommands { get; set; }

            //public Polygon2D Polygon { get { Load(); return _polygon; } }

            public double MaxX { get { Load();  return _maxX; } }
            public double MinX { get { Load(); return _minX; } }
            public double MaxY { get { Load(); return _maxY; } }
            public double MinY { get { Load(); return _minY; } }

            public bool IsClosed  { get { Load();  return _isClosed;  } }

            public bool IsEmbedded(HPGLLine to)
            {
                if (ReferenceEquals(this, to)) return false;
                bool isRectangleEmbedded =
                        MaxX >= to.MaxX && MinX <= to.MinX &&
                        MaxY >= to.MaxY && MinY <= to.MinY;
                if (!isRectangleEmbedded) return false;
                return IsEmbeddedEx(to);
            }

            public bool IsEmbeddedEx(HPGLLine to)
            {
                // TODO: we test points only!!!
                // but it would be necessary to thes the whole line 

                return _polygon.ArePointsInPolygon(to._polygon.Points);
            }

            public int Level => ParentLine?.Level + 1 ?? 0;

            public HPGLLine ParentLine { get; set; }

            private void Load()
            {
                if (!_isLoaded)
                {
                    var points = new List<Point2D>();
                    if (Commands != null && Commands.Any())
                    {
                        points.Add(Commands.First().PointFrom);
                        points.AddRange(Commands.Select(c => new Point2D() { X = c.PointTo.X ?? 0.0, Y = c.PointTo.Y ?? 0.0 }));
                    }
                    _polygon = new Polygon2D() { Points = points };
                    _maxX = _polygon.MaxX;
                    _minX = _polygon.MinX;
                    _maxY = _polygon.MaxY;
                    _minY = _polygon.MinY;
                    _isClosed = _polygon.IsClosed;
                    _isLoaded = true;
                }
            }

            private bool _isLoaded = false;
            private bool _isClosed;
            private double _maxX;
            private double _minX;
            private double _maxY;
            private double _minY;
            private Polygon2D _polygon;
        }
    }
}
