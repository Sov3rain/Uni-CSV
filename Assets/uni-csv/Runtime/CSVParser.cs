using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Sov3rain.CSVParser.Delimiter;

namespace Sov3rain
{
    public static class CSVParser
    {
        private static readonly char[] COMMON_DELIMITERS = { ',', '\t', ';', '|' };

        public enum Delimiter
        {
            Auto,
            Comma,
            Tab,
            Semicolon,
            Pipe
        }

        /// <summary>
        /// Load CSV data from a specified path.
        /// </summary>
        /// <param name="path">CSV file path.</param>
        /// <param name="delimiter">Delimiter.</param>
        /// <param name="header">Does this CSV file have a header row</param>
        /// <param name="encoding">Type of text encoding. (default UTF-8)</param>
        public static List<List<string>> ParseFromPath(
            string path,
            Delimiter delimiter = Auto,
            bool header = true,
            Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var data = File.ReadAllText(path, encoding);

            return ParseFromString(data, header, delimiter);
        }

        /// <summary>
        /// Load CSV data from a specified path. Input file must have a header row.
        /// </summary>
        /// <param name="path">CSV file path.</param>
        /// <param name="delimiter">Delimiter.</param>
        /// <param name="encoding">Type of text encoding. (default UTF-8)</param>
        public static IEnumerable<T> ParseFromPath<T>(
            string path,
            Delimiter delimiter = Auto,
            Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var data = File.ReadAllText(path, encoding);

            return ParseFromString<T>(data, delimiter);
        }

        /// <summary>
        /// Load CSV data from string. Input string must have a header row.
        /// </summary>
        /// <param name="data">CSV string</param>
        /// <param name="delimiter">Delimiter.</param>
        public static IEnumerable<T> ParseFromString<T>(string data, Delimiter delimiter = Auto)
        {
            var result = ParseFromString(data, true, delimiter);
            var properties = typeof(T).GetProperties();

            foreach (var row in result)
            {
                T obj = Activator.CreateInstance<T>();

                for (var i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];
                    var value = ConvertValue(row[i], property.PropertyType);

                    property.SetValue(obj, value);
                }

                yield return obj;
            }
        }

        /// <summary>
        /// Load CSV data from string.
        /// </summary>
        /// <param name="data">CSV string</param>
        /// <param name="header">Does this CSV file have a header row</param>
        /// <param name="delimiter">Delimiter.</param>
        public static List<List<string>> ParseFromString(
            string data,
            bool header = true,
            Delimiter delimiter = Auto)
        {
            if (delimiter == Auto)
            {
                delimiter = DetectDelimiterFromContent(data);
            }

            ConvertToCrlf(ref data);

            var sheet = new List<List<string>>();
            var row = new List<string>();
            var cell = new StringBuilder();
            var insideQuoteCell = false;
            var start = 0;

            var delimiterSpan = delimiter.ToChar().ToString().AsSpan();
            var crlfSpan = "\r\n".AsSpan();
            var oneDoubleQuotSpan = "\"".AsSpan();
            var twoDoubleQuotSpan = "\"\"".AsSpan();

            while (start < data.Length)
            {
                var length = start <= data.Length - 2 ? 2 : 1;
                var span = data.AsSpan(start, length);

                if (span.StartsWith(delimiterSpan))
                {
                    if (insideQuoteCell)
                    {
                        cell.Append(delimiter.ToChar());
                    }
                    else
                    {
                        AddCell(row, cell);
                    }

                    start += 1;
                }
                else if (span.StartsWith(crlfSpan))
                {
                    if (insideQuoteCell)
                    {
                        cell.Append("\r\n");
                    }
                    else
                    {
                        AddCell(row, cell);

                        if (IsRowNonEmpty(row))
                        {
                            AddRow(sheet, ref row);
                        }
                        else
                        {
                            row.Clear(); // Reset row if it's just empty
                        }
                    }

                    start += 2;
                }
                else if (span.StartsWith(twoDoubleQuotSpan))
                {
                    cell.Append("\"");
                    start += 2;
                }
                else if (span.StartsWith(oneDoubleQuotSpan))
                {
                    insideQuoteCell = !insideQuoteCell;
                    start += 1;
                }
                else
                {
                    cell.Append(span[0]);
                    start += 1;
                }
            }

            // Add the last cell
            if (IsRowNonEmpty(row) || cell.Length > 0)
            {
                AddCell(row, cell);
                AddRow(sheet, ref row);
            }

            if (header && sheet.Count > 0)
            {
                sheet.RemoveAt(0);
            }

            return sheet;
        }

        private static Delimiter DetectDelimiterFromContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return Comma;

            // Get the first non-empty line
            var firstLine = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                .FirstOrDefault(line => !string.IsNullOrWhiteSpace(line));

            if (string.IsNullOrWhiteSpace(firstLine))
                return Comma;

            var delimiterCounts = COMMON_DELIMITERS
                .ToDictionary(d => d, d => firstLine.Count(c => c == d));

            var mostFrequentDelimiter = delimiterCounts
                .OrderByDescending(kvp => kvp.Value)
                .First();

            if (mostFrequentDelimiter.Value <= 1)
                return Comma;

            return CharToDelimiter(mostFrequentDelimiter.Key);
        }

        private static bool IsRowNonEmpty(List<string> row) =>
            row.Count > 0 &&
            row.Any(cell => !string.IsNullOrWhiteSpace(cell));

        private static void AddCell(List<string> row, StringBuilder cell)
        {
            row.Add(cell.ToString());
            cell.Clear();
        }

        private static void AddRow(List<List<string>> sheet, ref List<string> row)
        {
            sheet.Add(new List<string>(row));
            row.Clear();
        }

        private static void ConvertToCrlf(ref string data)
        {
            data = Regex.Replace(data, @"\r\n|\r|\n", "\r\n");
        }

        private static char ToChar(this Delimiter delimiter) => delimiter switch
        {
            Comma => ',',
            Tab => '\t',
            Semicolon => ';',
            Pipe => '|',
            _ => throw new ArgumentException($"Unsupported delimiter: {delimiter}")
        };

        private static Delimiter CharToDelimiter(char delimiterChar) => delimiterChar switch
        {
            ',' => Comma,
            '\t' => Tab,
            ';' => Semicolon,
            '|' => Pipe,
            _ => Comma
        };

        private static object ConvertValue(string value, Type targetType)
        {
            if (string.IsNullOrWhiteSpace(value))
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

            if (targetType == typeof(string))
                return value;

            if (targetType == typeof(int) || targetType == typeof(int?))
                return int.TryParse(value, out int intResult) ? intResult : null;

            if (targetType == typeof(decimal) || targetType == typeof(decimal?))
                return decimal.TryParse(value, out decimal decimalResult) ? decimalResult : null;

            if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
                return DateTime.TryParse(value, out DateTime dateResult) ? dateResult : null;

            if (targetType == typeof(bool) || targetType == typeof(bool?))
                return bool.TryParse(value, out bool boolResult) ? boolResult : null;

            if (targetType == typeof(double) || targetType == typeof(double?))
                return double.TryParse(value, out double doubleResult) ? doubleResult : null;

            throw new NotSupportedException($"Type {targetType} is not supported for conversion.");
        }
    }
}