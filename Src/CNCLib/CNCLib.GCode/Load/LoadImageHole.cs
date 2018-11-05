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

using CNCLib.GCode.Commands;
using CNCLib.Logic.Contracts.DTO;

using Framework.Tools;

namespace CNCLib.GCode.Load
{
    public class LoadImageHole : LoadImageBase
    {
        public int ImageToDotSizeX => LoadOptions.DotSizeX;
        public int ImageToDotSizeY => LoadOptions.DotSizeY;

        protected override double PixelDistX => (double) LoadOptions.DotDistX;
        protected override double PixelDistY => (double) LoadOptions.DotDistY;
        protected          double LaserSize  => (double) LoadOptions.LaserSize;

        public bool UseYShift   => LoadOptions.UseYShift;
        public bool RotateHeart => LoadOptions.RotateHeart;

        public LoadOptions.EHoleType HoleType => LoadOptions.HoleType;

        public double StartLaserDist => 0.15;

        public override void Load()
        {
            PreLoad();

            AddCommentForLaser();

            using (var bx = new System.Drawing.Bitmap(IOHelper.ExpandEnvironmentVariables(LoadOptions.FileName)))
            {
                System.Drawing.Bitmap b;
                switch (bx.PixelFormat)
                {
                    case System.Drawing.Imaging.PixelFormat.Format1bppIndexed:
                        b = ScaleImage(bx);
                        break;

                    case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                    case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:

                        b = ScaleImage(bx);
                        break;

                    default: throw new ArgumentException("Bitmap.PixelFormat not supported");
                }

                WriteGCode(b);
            }

            PostLoad();
        }

        protected override void WriteGCode()
        {
            LaserOff();

            double topPos = SizeY * (PixelSizeY + PixelDistY);

            for (int iy = 0;; iy++)
            {
                double yy = ToYPos(iy);
                if (yy >= SizeY)
                {
                    break;
                }

                for (int ix = 0;; ix++)
                {
                    double y = yy;
                    double x = ToXPos(ix, iy, ref yy);
                    if (x >= SizeX)
                    {
                        break;
                    }

                    if (x >= 0 && x + ImageToDotSizeX <= SizeX && y >= 0 && y + ImageToDotSizeY <= SizeY)
                    {
                        // Rect (x1,y1, ImageToDotSizeX, ImageToDotSizeY)  is on printing area

                        double posx = x * (PixelSizeX + PixelDistX) + ShiftX + PixelDistX / 2;
                        double posy = y * (PixelSizeY + PixelDistY) + ShiftY + PixelDistY / 2;
                        // x,y left,top corner

                        AddCommandX(posx, topPos - posy, GetDotSize(x, y), HoleType, ix);
                    }
                }

                LaserOff();
            }
        }

        private double ToXPos(int ix, int iy, ref double y)
        {
            double xdiff = 0;

            switch (HoleType)
            {
                case LoadOptions.EHoleType.Hexagon:
                case LoadOptions.EHoleType.Square:
                case LoadOptions.EHoleType.Heart:
                case LoadOptions.EHoleType.Circle:
                case LoadOptions.EHoleType.Diamond:
                    if (iy % 2 == 0 && UseYShift)
                    {
                        xdiff = ImageToDotSizeX / 2.0;
                    }

                    break;
            }

            return (ix * ImageToDotSizeX + xdiff);
        }

        private double ToYPos(int iy)
        {
            double ypos = iy * ImageToDotSizeY;
            switch (HoleType)
            {
                case LoadOptions.EHoleType.Hexagon:
                case LoadOptions.EHoleType.Circle:
                    if (UseYShift)
                    {
                        return (ypos * 0.86602540378443864676372317075294);
                    }

                    break;

                case LoadOptions.EHoleType.Diamond:
                    if (UseYShift)
                    {
                        return (ypos * 0.5);
                    }

                    break;
            }

            return (ypos);
        }

        private double FindNearestColorGrayScale(byte colorR, byte colorG, byte colorB)
        {
            // max value is 255
            // 0.2126*255 + 0.7152*255 + 0.0722*255
            return 0.2126 * colorR + 0.7152 * colorG + 0.0722 * colorB;
        }

        private double GetDotSize(double x, double y)
        {
            // max value is 255
            double colorsum = 0;

            for (int dx = 0; dx < ImageToDotSizeX; dx++)
            {
                for (int dy = 0; dy < ImageToDotSizeY; dy++)
                {
                    var col = Bitmap.GetPixel((int) (x + dx), (int) (y + dy));
                    colorsum += FindNearestColorGrayScale(col.R, col.G, col.B);
                }
            }

            // return 0..1
            return (colorsum / (ImageToDotSizeX * ImageToDotSizeY)) / 255.0;
        }

        private void AddCommandX(double x, double y, double size, LoadOptions.EHoleType holetype, int ix)
        {
            // x,y left,top corner
            // size 0..1

            size = size * size; // squared area 

            if (LoadOptions.ImageInvert)
            {
                size = 1.0 - size;
            }

            double pixelX = PixelSizeX * ImageToDotSizeX;
            double pixelY = PixelSizeY * ImageToDotSizeY;
            double scaleX = (pixelX - LaserSize) / pixelX;
            double scaleY = (pixelY - LaserSize) / pixelY;

            switch (HoleType)
            {
                case LoadOptions.EHoleType.Hexagon:
                    size *= 1.0 / 0.86602540378443864676372317075294;
                    break;
            }

            double dotsizeX = size * PixelSizeX * ImageToDotSizeX * scaleX;
            double dotsizeY = size * PixelSizeY * ImageToDotSizeY * scaleY;

            double centerX = x + ImageToDotSizeX / 2.0;
            double centerY = y - ImageToDotSizeY / 2.0;

            switch (holetype)
            {
                case LoadOptions.EHoleType.Hexagon:
                    CreateHexagon(centerX, centerY, dotsizeX / 2.0);
                    break;
                case LoadOptions.EHoleType.Circle:
                    CreateCircle(centerX, centerY, dotsizeX / 2.0);
                    break;
                case LoadOptions.EHoleType.Square:
                    CreateSquare(centerX, centerY, dotsizeX / 2.0, dotsizeY / 2.0);
                    break;
                case LoadOptions.EHoleType.Diamond:
                    CreateDiamond(centerX, centerY, dotsizeX / 2.0, dotsizeY / 2.0);
                    break;
                case LoadOptions.EHoleType.Heart:
                    CreateHeart(centerX, centerY, dotsizeX / 2.0, dotsizeY / 2.0, RotateHeart && ix % 2 == 0);
                    break;
            }

            LaserOff();
        }

        private void CreateToSmallShape(double x, double y)
        {
            Command cc = new G00Command();
            cc.AddVariable('X', ToGCode(x));
            cc.AddVariable('Y', ToGCode(y));
            Commands.Add(cc);
            LaserOn();
            cc = new G01Command();
            cc.AddVariable('X', ToGCode(x));
            cc.AddVariable('Y', ToGCode(y));
            Commands.Add(cc);
        }

        private void CreateSquare(double x, double y, double hsizeX2, double hsizeY2)
        {
            if (hsizeX2 < 0.000001)
            {
                return; // true black do nothing
            }

            if (hsizeX2 * 2 < LaserSize)
            {
                CreateToSmallShape(x, y);
                return;
            }

            Command c = new G00Command();
            c.AddVariable('X', ToGCode(x - hsizeX2));
            c.AddVariable('Y', ToGCode(y - hsizeY2));
            Commands.Add(c);
            LaserOn();

            c = new G01Command();
            c.AddVariable('X', ToGCode(x + hsizeX2));
            c.AddVariable('Y', ToGCode(y - hsizeY2));
            Commands.Add(c);

            c = new G01Command();
            c.AddVariable('X', ToGCode(x + hsizeX2));
            c.AddVariable('Y', ToGCode(y + hsizeY2));
            Commands.Add(c);

            c = new G01Command();
            c.AddVariable('X', ToGCode(x - hsizeX2));
            c.AddVariable('Y', ToGCode(y + hsizeY2));
            Commands.Add(c);

            c = new G01Command();
            c.AddVariable('X', ToGCode(x - hsizeX2));
            c.AddVariable('Y', ToGCode(y - hsizeY2));
            Commands.Add(c);
        }

        private void CreateDiamond(double x, double y, double hsizeX2, double hsizeY2)
        {
            if (hsizeX2 < 0.000001)
            {
                return; // true black do nothing
            }

            if (hsizeX2 * 2 < LaserSize)
            {
                CreateToSmallShape(x, y);
                return;
            }

            Command c = new G00Command();
            c.AddVariable('X', ToGCode(x - hsizeX2 + StartLaserDist));
            c.AddVariable('Y', ToGCode(y));
            Commands.Add(c);
            LaserOn();

            c = new G01Command();
            c.AddVariable('X', ToGCode(x - hsizeX2));
            c.AddVariable('Y', ToGCode(y));
            Commands.Add(c);

            c = new G01Command();
            c.AddVariable('X', ToGCode(x));
            c.AddVariable('Y', ToGCode(y - hsizeY2));
            Commands.Add(c);

            c = new G01Command();
            c.AddVariable('X', ToGCode(x + hsizeX2));
            c.AddVariable('Y', ToGCode(y));
            Commands.Add(c);

            c = new G01Command();
            c.AddVariable('X', ToGCode(x));
            c.AddVariable('Y', ToGCode(y + hsizeY2));
            Commands.Add(c);

            c = new G01Command();
            c.AddVariable('X', ToGCode(x - hsizeX2));
            c.AddVariable('Y', ToGCode(y));
            Commands.Add(c);
        }

        private void CreateHexagon(double x, double y, double radius)
        {
            if (radius < 0.000001)
            {
                return; // true black do nothing
            }

            if (radius * 2 < LaserSize)
            {
                CreateToSmallShape(x, y);
                return;
            }

            double rotateangel = Math.PI / 6.0;

            double startx = x + radius * Math.Cos(rotateangel);
            double starty = y + radius * Math.Sin(rotateangel);

            Command c = new G00Command();
            c.AddVariable('X', ToGCode(startx - StartLaserDist));
            c.AddVariable('Y', ToGCode(starty - StartLaserDist / 2));
            Commands.Add(c);
            LaserOn();

            //for (double rad = Math.PI / 3.0; rad < Math.PI * 2.0 + 0.1; rad += Math.PI / 3.0)
            for (double rad = 0; rad < Math.PI * 2.0 + 0.1; rad += Math.PI / 3.0)
            {
                double radrotated = rad + Math.PI / 6.0;
                c = new G01Command();
                c.AddVariable('X', ToGCode(x + radius * Math.Cos(radrotated)));
                c.AddVariable('Y', ToGCode(y + radius * Math.Sin(radrotated)));

                Commands.Add(c);
            }
        }

        private void CreateCircle(double x, double y, double radius)
        {
            if (radius < 0.000001)
            {
                return; // true black do nothing
            }

            if (radius * 2 < LaserSize)
            {
                CreateToSmallShape(x, y);
                return;
            }

            Command c = new G00Command();
            c.AddVariable('X', ToGCode(x + radius - StartLaserDist));
            c.AddVariable('Y', ToGCode(y));
            Commands.Add(c);
            LaserOn();
            c = new G01Command();
            c.AddVariable('X', ToGCode(x + radius));
            Commands.Add(c);

            c = new G03Command();
            c.AddVariable('I', ToGCode(-radius));
            Commands.Add(c);
        }

        private void CreateHeart(double x, double y, double hsizeX2, double hsizeY2, bool mirror)
        {
            if (hsizeX2 < 0.000001)
            {
                return; // true black do nothing
            }

            if (hsizeX2 * 2 < LaserSize)
            {
                CreateToSmallShape(x, y);
                return;
            }

            hsizeX2 *= 0.9;
            hsizeY2 =  hsizeX2;
            double mr = mirror ? -1.0 : 1.0;

            Command c = new G00Command();
            c.AddVariable('X', ToGCode(x));
            c.AddVariable('Y', ToGCode(y + (hsizeY2 - StartLaserDist) * mr));
            Commands.Add(c);
            LaserOn();

            c = new G01Command();
            c.AddVariable('X', ToGCode(x));
            c.AddVariable('Y', ToGCode(y + hsizeY2 * mr));
            Commands.Add(c);

            if (mirror)
            {
                c = new G03Command();
            }
            else
            {
                c = new G02Command();
            }

            c.AddVariable('X', ToGCode(x + hsizeX2));
            c.AddVariable('Y', ToGCode(y));
            c.AddVariable('I', ToGCode(hsizeX2 / 2.0));
            c.AddVariable('J', ToGCode(-hsizeY2 / 2.0 * mr));
            Commands.Add(c);

            c = new G01Command();
            c.AddVariable('X', ToGCode(x));
            c.AddVariable('Y', ToGCode(y - hsizeY2 * mr));
            Commands.Add(c);

            c = new G01Command();
            c.AddVariable('X', ToGCode(x - hsizeX2));
            c.AddVariable('Y', ToGCode(y));
            Commands.Add(c);

            if (mirror)
            {
                c = new G03Command();
            }
            else
            {
                c = new G02Command();
            }

            c.AddVariable('X', ToGCode(x));
            c.AddVariable('Y', ToGCode(y + hsizeY2 * mr));
            c.AddVariable('I', ToGCode(hsizeX2 / 2.0));
            c.AddVariable('J', ToGCode(hsizeY2 / 2.0 * mr));
            Commands.Add(c);
        }
    }
}