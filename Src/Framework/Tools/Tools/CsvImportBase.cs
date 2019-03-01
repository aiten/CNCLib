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

namespace Framework.Tools.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;

    public class CsvImportBase
    {
        private readonly NumberFormatInfo _nfi;

        public Encoding Encoding { get; set; } = Encoding.Default;

        public CsvImportBase()
        {
            // Retrieve a writable NumberFormatInfo object.
            CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US");
            _nfi = enUS.NumberFormat;
        }

        public NumberFormatInfo NumberFormat => _nfi;

        public void SetAustriaNumberFormat()
        {
            _nfi.NumberDecimalSeparator = ",";
            _nfi.NumberGroupSeparator   = ".";
        }

        public IList<IList<string>> ReadStringMatrixFromCsv(string fileName, bool skipTitleLine)
        {
            string[] lines     = File.ReadAllLines(fileName, Encoding);

            var  elements  = new List<IList<string>>();
            bool firstLine = skipTitleLine;

            foreach (var line in lines)
            {
                if (firstLine)
                {
                    firstLine = false;
                }
                else
                {
                    elements.Add(ReadLine(line));
                }
            }

            return elements;
        }

        private IList<string> ReadLine(string line)
        {
            // remark: newline in Quote not implemented 
            var columns = new List<string>();

            var  sb          = new StringBuilder(line.Length);
            char noQuoteChar = '\0';
            char quoteChar   = noQuoteChar;
            char lastCh      = noQuoteChar;

            for (int i = 0; i < line.Length; i++)
            {
                char ch = line[i];

                if (ch == quoteChar)
                {
                    quoteChar = noQuoteChar;
                }
                else if (quoteChar == noQuoteChar && lastCh != '\\' && (ch == '\'' || ch == '"'))
                {
                    quoteChar = ch;
                }
                else if (quoteChar == noQuoteChar && (ch == ';'))
                {
                    columns.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(ch);
                }

                lastCh = ch;
            }

            columns.Add(sb.ToString());

            return columns;
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

        public DateTime ExcelDateOrDateTime(string excelField)
        {
            if (excelField.Length > "yyyy/MM/dd".Length)
            {
                return ExcelDateTimeYMD(excelField);
            }

            return ExcelDateYMD(excelField);
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

        public DateTime ExcelDateTimeYMD(string excelField)
        {
            try
            {
                // Parse date and time with custom specifier.
                // e.g. string dateString = "2017/09/20 00:00:00.000";

                bool   hasFraction = excelField.IndexOf('.') > 0;
                string format      = hasFraction ? "yyyy/MM/dd hh:mm:ss.fff" : "yyyy/MM/dd hh:mm:ss";

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