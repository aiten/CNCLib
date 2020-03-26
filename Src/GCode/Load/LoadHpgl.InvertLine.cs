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
using Framework.Tools.Clipper;

namespace CNCLib.GCode.Load
{
    public partial class LoadHpgl
    {
        private class InvertLine
        {
            public LoadOptions LoadOptions { get; set; }
            public LoadHpgl    LoadX       { get; set; }

            public IList<HpglCommand> ConvertInvert(IList<HpglCommand> list)
            {
                // split 

                int startIdx = 0;

                var                      lineList     = new List<HpglLine>();
                IEnumerable<HpglCommand> postCommands = null;
                IEnumerable<HpglCommand> preCommands  = list.Skip(startIdx).TakeWhile(e => !e.IsPenCommand);

                startIdx += preCommands.Count();

                while (startIdx < list.Count)
                {
                    HpglLine line = GetHpglLine(list, ref startIdx);

                    if (startIdx >= list.Count && !line.Commands.Any())
                    {
                        postCommands = line.PreCommands;
                    }
                    else
                    {
                        lineList.Add(line);
                    }
                }

                // rearrange

                var lines = OrderLines(lineList);

                // rebuild list

                var newList = new List<HpglCommand>();
                newList.AddRange(preCommands);

                foreach (var line in lines)
                {
                    newList.AddRange(line.PreCommands);
                    newList.AddRange(line.Commands);
                    newList.AddRange(line.PostCommands);
                }

                if (postCommands != null)
                {
                    newList.AddRange(postCommands);
                }

                return newList;
            }

            private IEnumerable<HpglLine> OrderLines(IEnumerable<HpglLine> lines)
            {
                var newList = new List<HpglLine>();
                newList.AddRange(lines.Where(l => !l.IsClosed));
                newList.AddRange(OrderClosedLine(lines.Where(l => l.IsClosed)));

                return newList;
            }

            private void CalcClosedLineParent(IEnumerable<HpglLine> closedLines)
            {
                foreach (var line in closedLines)
                {
                    foreach (var parentLine in closedLines.Where(l => l.IsEmbedded(line)))
                    {
                        if (line.ParentLine == null || line.ParentLine.IsEmbedded(parentLine))
                        {
                            line.ParentLine = parentLine;
                        }
                    }
                }
            }

            const double _scale = 1000;

            private IEnumerable<HpglLine> OrderClosedLine(IEnumerable<HpglLine> closedLines)
            {
                var orderedList = new List<HpglLine>();
                if (closedLines.Any())
                {
                    CalcClosedLineParent(closedLines);
                    int maxLevel = closedLines.Max(l => l.Level);

                    for (int level = maxLevel; level >= 0; level--)
                    {
                        var linesOnLevel = closedLines.Where(l => l.Level == level);

                        if (LoadOptions.CutterSize != 0)
                        {
                            linesOnLevel = OffsetLines(_scale / 2.0 * (double)LoadOptions.CutterSize * ((level % 2 == 0) ? 1.0 : -1.0), linesOnLevel);
                        }

                        orderedList.AddRange(OptimizeDistance(linesOnLevel));
                    }
                }

                return orderedList;
            }

            private IEnumerable<HpglLine> OffsetLines(double offset, IEnumerable<HpglLine> lines)
            {
                var newlines = new List<HpglLine>();

                foreach (var line in lines)
                {
                    newlines.AddRange(OffsetLine(offset, line));
                }

                return newlines;
            }

            private IEnumerable<HpglLine> OffsetLine(double offset, HpglLine line)
            {
                var newlines = new List<HpglLine> { line };

                var co        = new ClipperOffset();
                var solution  = new List<List<IntPoint>>();
                var solution2 = new List<List<IntPoint>>();
                solution.Add(line.Commands.Select(x => new IntPoint(_scale * x.PointFrom.X0, _scale * x.PointFrom.Y0)).ToList());
                co.AddPaths(solution, JoinType.jtRound, EndType.etClosedPolygon);
                co.Execute(ref solution2, offset);
                var existingLine = line;

                foreach (var polygon in solution2)
                {
                    var         newCmds = new List<HpglCommand>();
                    HpglCommand last    = null;

                    foreach (var pt in polygon)
                    {
                        var from = new Point3D { X = pt.X / _scale, Y = pt.Y / _scale };
                        var hpgl = new HpglCommand
                        {
                            PointFrom   = from,
                            CommandType = HpglCommand.HpglCommandType.PenDown
                        };
                        newCmds.Add(hpgl);
                        if (last != null)
                        {
                            last.PointTo = @from;
                        }

                        last = hpgl;
                    }

                    last.PointTo = newCmds.First().PointFrom;

                    if (existingLine == null)
                    {
                        // add new line
                        existingLine = new HpglLine
                        {
                            PreCommands = new List<HpglCommand>
                            {
                                new HpglCommand { CommandType = HpglCommand.HpglCommandType.PenUp }
                            },
                            PostCommands = new List<HpglCommand>(),
                            ParentLine   = line.ParentLine
                        };
                        newlines.Add(existingLine);
                    }

                    existingLine.Commands                                      = newCmds;
                    existingLine.PreCommands.Last(l => l.IsPenCommand).PointTo = newCmds.First().PointFrom;
                    existingLine                                               = null;
                }

                return newlines;
            }

            private static IEnumerable<HpglLine> OptimizeDistance(IEnumerable<HpglLine> lines)
            {
                var newList = new List<HpglLine> { lines.First() };
                var list    = new List<HpglLine>();
                list.AddRange(lines.Skip(1));

                while (list.Any())
                {
                    Point3D  fromPt      = newList.Last().Commands.Last().PointTo;
                    double   maxDist     = double.MaxValue;
                    HpglLine minDistLine = null;

                    foreach (var l in list)
                    {
                        Point3D pt   = l.Commands.First().PointFrom;
                        double  dx   = (pt.X ?? 0.0) - (fromPt.X ?? 0.0);
                        double  dy   = (pt.Y ?? 0.0) - (fromPt.Y ?? 0.0);
                        double  dist = Math.Sqrt(dx * dx + dy * dy);

                        if (dist < maxDist)
                        {
                            maxDist     = dist;
                            minDistLine = l;
                        }
                    }

                    list.Remove(minDistLine);
                    newList.Add(minDistLine);
                }

                return newList;
            }

            private static HpglLine GetHpglLine(IList<HpglCommand> list, ref int startIdx)
            {
                var line = new HpglLine
                {
                    PreCommands = list.Skip(startIdx).TakeWhile(e => !e.IsPenDownCommand)
                };
                startIdx += line.PreCommands.Count();

                line.Commands =  list.Skip(startIdx).TakeWhile(e => e.IsPenDownCommand);
                startIdx      += line.Commands.Count();

                line.PostCommands =  list.Skip(startIdx).TakeWhile(e => false); // always empty
                startIdx          += line.PostCommands.Count();
                return line;
            }
        }
    }
}