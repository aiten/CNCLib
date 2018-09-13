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
using CNCLib.Logic.Contracts.DTO;
using Framework.Tools.Clipper;
using Framework.Tools.Drawing;

namespace CNCLib.GCode.Load
{
    public partial class LoadHPGL
    {
        private class InvertLine
        {
            public LoadOptions LoadOptions { get; set; }
            public LoadHPGL    LoadX       { get; set; }

            public IList<HPGLCommand> ConvertInvert(IList<HPGLCommand> list)
            {
                // split 

                int startidx = 0;

                var                      linelist     = new List<HPGLLine>();
                IEnumerable<HPGLCommand> postCommands = null;
                IEnumerable<HPGLCommand> preCommands  = list.Skip(startidx).TakeWhile(e => !e.IsPenCommand);

                startidx += preCommands.Count();

                while (startidx < list.Count)
                {
                    HPGLLine line = GetHPGLLine(list, ref startidx);

                    if (startidx >= list.Count && !line.Commands.Any())
                    {
                        postCommands = line.PreCommands;
                    }
                    else
                    {
                        linelist.Add(line);
                    }
                }

                // rearrange

                var lines = OrderLines(linelist);

                // rebuild list

                var newlist = new List<HPGLCommand>();
                newlist.AddRange(preCommands);

                foreach (var line in lines)
                {
                    newlist.AddRange(line.PreCommands);
                    newlist.AddRange(line.Commands);
                    newlist.AddRange(line.PostCommands);
                }

                if (postCommands != null)
                {
                    newlist.AddRange(postCommands);
                }

                return newlist;
            }

            private IEnumerable<HPGLLine> OrderLines(IEnumerable<HPGLLine> lines)
            {
                var newlist = new List<HPGLLine>();
                newlist.AddRange(lines.Where(l => !l.IsClosed));
                newlist.AddRange(OrderClosedLine(lines.Where(l => l.IsClosed)));

                return newlist;
            }

            private void CalcClosedLineParent(IEnumerable<HPGLLine> closedLines)
            {
                foreach (var line in closedLines)
                {
                    foreach (var parentline in closedLines.Where(l => l.IsEmbedded(line)))
                    {
                        if (line.ParentLine == null || line.ParentLine.IsEmbedded(parentline))
                        {
                            line.ParentLine = parentline;
                        }
                    }
                }
            }

            const double _scale = 1000;

            private IEnumerable<HPGLLine> OrderClosedLine(IEnumerable<HPGLLine> closedLines)
            {
                var orderdlist = new List<HPGLLine>();
                if (closedLines.Any())
                {
                    CalcClosedLineParent(closedLines);
                    int maxlevel = closedLines.Max(l => l.Level);

                    for (int level = maxlevel; level >= 0; level--)
                    {
                        var linesOnLevel = closedLines.Where(l => l.Level == level);

                        if (LoadOptions.LaserSize != 0)
                        {
                            linesOnLevel =
                                OffsetLines(_scale / 2.0 * (double) LoadOptions.LaserSize * ((level % 2 == 0) ? 1.0 : -1.0),
                                            linesOnLevel);
                        }

                        orderdlist.AddRange(OptimizeDistanze(linesOnLevel));
                    }
                }

                return orderdlist;
            }

            private IEnumerable<HPGLLine> OffsetLines(double offset, IEnumerable<HPGLLine> lines)
            {
                var newlines = new List<HPGLLine>();

                foreach (var line in lines)
                {
                    newlines.AddRange(OffsetLine(offset, line));
                }

                return newlines;
            }

            private IEnumerable<HPGLLine> OffsetLine(double offset, HPGLLine line)
            {
                var newlines = new List<HPGLLine> { line };

                var co        = new ClipperOffset();
                var solution  = new List<List<IntPoint>>();
                var solution2 = new List<List<IntPoint>>();
                solution.Add(line.Commands.Select(x => new IntPoint(_scale * x.PointFrom.X0, _scale * x.PointFrom.Y0))
                                 .ToList());
                co.AddPaths(solution, JoinType.jtRound, EndType.etClosedPolygon);
                co.Execute(ref solution2, offset);
                var existingline = line;

                foreach (var polygon in solution2)
                {
                    var         newcmds = new List<HPGLCommand>();
                    HPGLCommand last    = null;

                    foreach (var pt in polygon)
                    {
                        var from = new Point3D { X = pt.X / _scale, Y = pt.Y / _scale };
                        var hpgl = new HPGLCommand
                        {
                            PointFrom   = from,
                            CommandType = HPGLCommand.HPGLCommandType.PenDown
                        };
                        newcmds.Add(hpgl);
                        if (last != null)
                        {
                            last.PointTo = @from;
                        }

                        last = hpgl;
                    }

                    last.PointTo = newcmds.First().PointFrom;

                    if (existingline == null)
                    {
                        // add new line
                        existingline = new HPGLLine
                        {
                            PreCommands = new List<HPGLCommand>
                            {
                                new HPGLCommand { CommandType = HPGLCommand.HPGLCommandType.PenUp }
                            },
                            PostCommands = new List<HPGLCommand>(),
                            ParentLine   = line.ParentLine
                        };
                        newlines.Add(existingline);
                    }

                    existingline.Commands                                      = newcmds;
                    existingline.PreCommands.Last(l => l.IsPenCommand).PointTo = newcmds.First().PointFrom;
                    existingline                                               = null;
                }

                return newlines;
            }

            private static IEnumerable<HPGLLine> OptimizeDistanze(IEnumerable<HPGLLine> lines)
            {
                var newlist = new List<HPGLLine> { lines.First() };
                var list    = new List<HPGLLine>();
                list.AddRange(lines.Skip(1));

                while (list.Any())
                {
                    Point3D  ptfrom      = newlist.Last().Commands.Last().PointTo;
                    double   maxdist     = double.MaxValue;
                    HPGLLine minDistLine = null;

                    foreach (var l in list)
                    {
                        Point3D pt   = l.Commands.First().PointFrom;
                        double  dx   = (pt.X ?? 0.0) - (ptfrom.X ?? 0.0);
                        double  dy   = (pt.Y ?? 0.0) - (ptfrom.Y ?? 0.0);
                        double  dist = Math.Sqrt(dx * dx + dy * dy);

                        if (dist < maxdist)
                        {
                            maxdist     = dist;
                            minDistLine = l;
                        }
                    }

                    list.Remove(minDistLine);
                    newlist.Add(minDistLine);
                }

                return newlist;
            }

            private static HPGLLine GetHPGLLine(IList<HPGLCommand> list, ref int startidx)
            {
                var line = new HPGLLine
                {
                    PreCommands = list.Skip(startidx).TakeWhile(e => !e.IsPenDownCommand)
                };
                startidx += line.PreCommands.Count();

                line.Commands =  list.Skip(startidx).TakeWhile(e => e.IsPenDownCommand);
                startidx      += line.Commands.Count();

                line.PostCommands =  list.Skip(startidx).TakeWhile(e => false); // always empty
                startidx          += line.PostCommands.Count();
                return line;
            }
        }
    }
}