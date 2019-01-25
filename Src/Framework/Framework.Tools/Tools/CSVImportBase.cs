/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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

namespace Framework.Tools.Tools
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;

    public class CSVImportBase
    {
        private readonly NumberFormatInfo _nfi;

        public Encoding Encoding { get; set; } = Encoding.Default;

        public CSVImportBase()
        {
            // Retrieve a writable NumberFormatInfo object.
            CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US");
            _nfi = enUS.NumberFormat;

            _nfi.NumberDecimalSeparator = ",";
            _nfi.NumberGroupSeparator   = ".";
        }

        public string[][] ReadStringMatrixFromCsv(string fileName, bool skipTitleLine)
        {
            int startLine     = 0;
            int subtractIndex = 0;

            string[] lines     = File.ReadAllLines(fileName, Encoding);
            int      lineCount = lines.Length;
            if (skipTitleLine)
            {
                lineCount--;
                startLine     = 1;
                subtractIndex = 1;
            }

            string[][] elements = new string[lineCount][];
            for (int line = startLine; line < lines.Length; line++)
            {
                elements[line - subtractIndex] = lines[line].Split(';');
            }

            return elements;
        }

        public string ExcelString(string excelField)
        {
            return excelField;
        }

        public short ExcelShort(string excelField)
        {
            return short.Parse(excelField);
        }

        public int ExcelInt(string excelField)
        {
            return int.Parse(excelField);
        }

        public byte ExcelByte(string excelField)
        {
            return byte.Parse(excelField);
        }

        public bool ExcelBool(string excelField)
        {
            switch (excelField)
            {
                case @"0":
                case @"false":
                    return false;
                case @"1":
                case @"true":
                    return true;
                default:
                    throw new FormatException();
            }
        }

        public decimal ExcelDecimal(string excelField)
        {
            return decimal.Parse(excelField, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, _nfi);
        }

        public double ExcelDouble(string excelField)
        {
            return double.Parse(excelField, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, _nfi);
        }

        public DateTime ExcelDateDMY(string excelField)
        {
            try
            {
                // Parse date and time with custom specifier.
                // e.g. string dateString = "19.01.2018";
                string      format   = "dd.MM.yyyy";
                CultureInfo provider = CultureInfo.InvariantCulture;

                return DateTime.ParseExact(excelField, format, provider);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DateTime ExcelDateYMD(string excelField)
        {
            try
            {
                // Parse date and time with custom specifier.
                // e.g. string dateString = "19.01.2018";
                string      format   = "yyyy/MM/dd";
                CultureInfo provider = CultureInfo.InvariantCulture;

                return DateTime.ParseExact(excelField, format, provider);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DateTime? ExcelDateTimeYMD(string excelField)
        {
            if (string.IsNullOrEmpty(excelField))
            {
                return null;
            }

            try
            {
                // Parse date and time with custom specifier.
                // e.g. string dateString = "2017-09-20 00:00:00.000";
                string      format   = "yyyy-MM-dd hh:mm:ss.fff";
                CultureInfo provider = CultureInfo.InvariantCulture;

                return DateTime.ParseExact(excelField, format, provider);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}