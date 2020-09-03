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

namespace CNCLib.GCode.Generate.Load
{
    using System.Collections.Generic;
    using System.Linq;

    using Framework.Drawing;

    public partial class LoadHpgl
    {
        private class HpglLine
        {
            public IEnumerable<HpglCommand> PreCommands  { get; set; }
            public IEnumerable<HpglCommand> Commands     { get; set; }
            public IEnumerable<HpglCommand> PostCommands { get; set; }

            //public Polygon2D Polygon { get { Load(); return _polygon; } }

            public double MaxX
            {
                get
                {
                    Load();
                    return _maxX;
                }
            }

            public double MinX
            {
                get
                {
                    Load();
                    return _minX;
                }
            }

            public double MaxY
            {
                get
                {
                    Load();
                    return _maxY;
                }
            }

            public double MinY
            {
                get
                {
                    Load();
                    return _minY;
                }
            }

            public bool IsClosed
            {
                get
                {
                    Load();
                    return _isClosed;
                }
            }

            public bool IsEmbedded(HpglLine to)
            {
                if (ReferenceEquals(this, to))
                {
                    return false;
                }

                bool isRectangleEmbedded = MaxX >= to.MaxX && MinX <= to.MinX && MaxY >= to.MaxY && MinY <= to.MinY;
                if (!isRectangleEmbedded)
                {
                    return false;
                }

                return IsEmbeddedEx(to);
            }

            public bool IsEmbeddedEx(HpglLine to)
            {
                return _polygon.ArePointsInPolygon(to._polygon.Points);
            }

            public int Level => ParentLine?.Level + 1 ?? 0;

            public HpglLine ParentLine { get; set; }

            private void Load()
            {
                if (!_isLoaded)
                {
                    var points = new List<Point2D>();
                    if (Commands != null && Commands.Any())
                    {
                        points.Add(Commands.First().PointFrom);
                        points.AddRange(
                            Commands.Select(
                                c => new Point2D
                                {
                                    X = c.PointTo.X ?? 0.0,
                                    Y = c.PointTo.Y ?? 0.0
                                }));
                    }

                    _polygon  = new Polygon2D { Points = points };
                    _maxX     = _polygon.MaxX;
                    _minX     = _polygon.MinX;
                    _maxY     = _polygon.MaxY;
                    _minY     = _polygon.MinY;
                    _isClosed = _polygon.IsClosed;
                    _isLoaded = true;
                }
            }

            private bool      _isLoaded = false;
            private bool      _isClosed;
            private double    _maxX;
            private double    _minX;
            private double    _maxY;
            private double    _minY;
            private Polygon2D _polygon;
        }
    }
}