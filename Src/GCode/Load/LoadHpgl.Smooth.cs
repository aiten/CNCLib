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

using System;
using System.Collections.Generic;
using System.Linq;

using CNCLib.Logic.Abstraction.DTO;

using Framework.Drawing;

namespace CNCLib.GCode.Load
{
    public partial class LoadHpgl
    {
        private class Smooth
        {
            public LoadOptions LoadOptions { get; set; }
            public LoadHpgl    LoadX       { get; set; }

            public IList<HpglCommand> SmoothList(IList<HpglCommand> list)
            {
                var newList = new List<HpglCommand>();

                int startIdx = 0;
                while (startIdx < list.Count)
                {
                    var newOpenList = list.Skip(startIdx).TakeWhile(e => !e.IsPenDownCommand);
                    newList.AddRange(newOpenList);
                    startIdx += newOpenList.Count();

                    var line = list.Skip(startIdx).TakeWhile(e => e.IsPenDownCommand);
                    startIdx += line.Count();

                    newList.AddRange(SmoothLine(line));
                }

                CalculateAngles(newList, null);
                return newList;
            }

            int _lineIdx = 1;

            private IList<HpglCommand> SmoothLine(IEnumerable<HpglCommand> line)
            {
                if (LoadX._DEBUG)
                {
                    LoadX.WriteLineToFile(line, _lineIdx++);
                }

                var    list     = new List<HpglCommand>();
                double maxAngle = LoadOptions.SmoothMinAngle.HasValue ? (double)LoadOptions.SmoothMinAngle.Value : (45 * (Math.PI / 180));

                int startIdx = 0;
                while (startIdx < line.Count())
                {
                    // check for angle
                    var linePart = line.Skip(startIdx).TakeWhile(c => Math.Abs(c.DiffLineAngleWithNext ?? (0.0)) < maxAngle);
                    if (linePart.Any())
                    {
                        startIdx += linePart.Count();
                        list.AddRange(SplitLine(linePart));
                    }
                    else
                    {
                        linePart =  line.Skip(startIdx).TakeWhile(c => Math.Abs(c.DiffLineAngleWithNext ?? (0.0)) >= maxAngle);
                        startIdx += linePart.Count();
                        list.AddRange(linePart);
                    }
                }

                return list;
            }

            private IEnumerable<HpglCommand> SplitLine(IEnumerable<HpglCommand> line)
            {
                if (line.Count() < 2)
                {
                    return line;
                }

                Point3D firstFrom = line.ElementAt(0).PointFrom;
                for (int i = 0; i < 100; i++)
                {
                    var newline = SplitLineImpl(line);
                    if (newline.Count() == line.Count())
                    {
                        return newline;
                    }

                    line = newline;
                    CalculateAngles(line, firstFrom);
                }

                return line;
            }

            private IEnumerable<HpglCommand> SplitLineImpl(IEnumerable<HpglCommand> line)
            {
                if (line.Count() < 3)
                {
                    return line;
                }

                var         newline       = new List<HpglCommand>();
                HpglCommand prev          = null;
                double      minLineLength = LoadOptions.SmoothMinLineLength.HasValue ? (double)LoadOptions.SmoothMinLineLength.Value : double.MaxValue;
                double      maxError      = LoadOptions.SmoothMaxError.HasValue ? (double)LoadOptions.SmoothMaxError.Value : 1.0 / 40.0;
                minLineLength /= (double)LoadOptions.ScaleX;
                maxError      /= (double)LoadOptions.ScaleX;

                foreach (var pt in line)
                {
                    double x = (pt.PointTo.X0) - (pt.PointFrom.X0);
                    double y = (pt.PointTo.Y0) - (pt.PointFrom.Y0);

                    double c = Math.Sqrt(x * x + y * y);

                    if (minLineLength <= c)
                    {
                        double alpha     = pt.DiffLineAngleWithNext ?? (prev?.DiffLineAngleWithNext ?? 0.0);
                        double beta      = prev != null ? (prev.DiffLineAngleWithNext ?? 0.0) : alpha;
                        double swapScale = 1.0;

                        if ((alpha >= 0.0 && beta >= 0.0) || (alpha <= 0.0 && beta <= 0.0))
                        {
                        }
                        else
                        {
                            beta      = -beta;
                            swapScale = 0.5;
                        }

                        if ((alpha >= 0.0 && beta >= 0.0) || (alpha <= 0.0 && beta <= 0.0))
                        {
                            double gamma = Math.PI - alpha - beta;

                            double b = Math.Sin(beta) / Math.Sin(gamma) * c;

                            //double a = Math.Sin(alpha) / Math.Sin(gamma) * c;

                            double hc  = b * Math.Sin(alpha) * swapScale;
                            double dc  = Math.Sqrt(b * b - hc * hc);
                            double hc4 = hc / 4.0;

                            if (Math.Abs(hc4) > maxError && Math.Abs(hc4) < c && Math.Abs(dc) < c)
                            {
                                newline.Add(GetNewCommand(pt, dc, hc4));
                            }
                        }
                    }

                    prev = pt;
                    newline.Add(pt);
                }

                return newline;
            }

            /// <summary>
            /// Create a new point based on a existing line (x,y are based on the line vector)
            /// </summary>
            private HpglCommand GetNewCommand(HpglCommand pt, double x, double y)
            {
                double diffAlpha = Math.Atan2(y, x);
                double lineAlpha = diffAlpha + (pt.LineAngle ?? 0);

                double dx = x * Math.Cos(lineAlpha);
                double dy = x * Math.Sin(lineAlpha);

                return new HpglCommand
                {
                    CommandType = pt.CommandType,
                    PointTo     = new Point3D { X = pt.PointFrom.X + dx, Y = pt.PointFrom.Y + dy }
                };
            }

            public static void CalculateAngles(IEnumerable<HpglCommand> list, Point3D firstFrom)
            {
                HpglCommand last = null;
                if (firstFrom != null)
                {
                    last = new HpglCommand
                    {
                        PointTo     = firstFrom,
                        CommandType = HpglCommand.HpglCommandType.PenDown
                    };
                }

                foreach (var cmd in list)
                {
                    cmd.ResetCalculated();
                    if (cmd.IsPointToValid)
                    {
                        if (last != null)
                        {
                            cmd.PointFrom             = last.PointTo;
                            cmd.LineAngle             = Math.Atan2((cmd.PointTo.Y0) - (cmd.PointFrom.Y0), (cmd.PointTo.X0) - (cmd.PointFrom.X0));
                            cmd.DiffLineAngleWithNext = null;

                            if (last.LineAngle.HasValue && cmd.IsPenDownCommand)
                            {
                                last.DiffLineAngleWithNext = last.LineAngle - cmd.LineAngle;
                                if (last.DiffLineAngleWithNext > Math.PI)
                                {
                                    last.DiffLineAngleWithNext -= Math.PI * 2.0;
                                }

                                if (last.DiffLineAngleWithNext < -Math.PI)
                                {
                                    last.DiffLineAngleWithNext += (Math.PI * 2.0);
                                }
                            }
                        }

                        last = cmd;
                    }
                }
            }
        }
    }
}