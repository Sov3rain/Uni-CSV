using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using static UniCSV.Delimiter;

namespace UniCSV
{
    public static class CsvParser
    {
        /// <summary>
        /// Load CSV data from a specified path.
        /// </summary>
        /// <param name="path">CSV file path.</param>
        /// <param name="delimiter">Delimiter.</param>
        /// <param name="hasHeader">Does this CSV file have a header row</param>
        /// <param name="removeHeader">Remove header row from the result</param>
        /// <param name="encoding">Type of text encoding. (default UTF-8)</param>
        public static List<List<string>> ParseFromPath(
            string path,
            bool hasHeader,
            bool removeHeader = true,
            Delimiter delimiter = Auto,
            Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var data = File.ReadAllText(path, encoding);

            return ParseFromString(data, hasHeader, removeHeader, delimiter);
        }

        /// <summary>
        /// Load CSV data from a specified path. Input file must have a header row.
        /// </summary>
        /// <param name="path">CSV file path.</param>
        /// <param name="hasHeader">Does this CSV file have a header row</param>
        /// <param name="delimiter">Delimiter.</param>
        /// <param name="encoding">Type of text encoding. (default UTF-8)</param>
        public static IEnumerable<T> ParseFromPath<T>(
            string path,
            bool hasHeader,
            Delimiter delimiter = Auto,
            Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var data = File.ReadAllText(path, encoding);

            return ParseFromString<T>(data, hasHeader, delimiter);
        }

        /// <summary>
        /// Load CSV data from string. Input string must have a header row.
        /// </summary>
        /// <param name="data">CSV string</param>
        /// <param name="hasHeader">Does this CSV file have a header row</param>
        /// <param name="delimiter">Delimiter.</param>
        public static IEnumerable<T> ParseFromString<T>(string data, bool hasHeader, Delimiter delimiter = Auto)
        {
            var result = ParseFromString(data, hasHeader, false, delimiter);

            if (hasHeader && result.Count <= 1) // If we only have a header row or empty data
                yield break;

            var headers = hasHeader && result.Count > 0 ? result[0] : null;
            var properties = typeof(T).GetProperties();
            var propertyMap = new Dictionary<int, PropertyInfo>();

            // Map by header name first if headers exist
            if (headers is not null)
            {
                var nameBasedProperties = properties.ToDictionary(
                    prop => prop.GetCustomAttribute<CsvColumnAttribute>()?.Name ?? prop.Name,
                    prop => prop
                );

                for (int i = 0; i < headers.Count; i++)
                {
                    if (nameBasedProperties.TryGetValue(headers[i], out PropertyInfo prop))
                    {
                        propertyMap[i] = prop;
                    }
                }
            }

            foreach (var prop in properties)
            {
                // Check for index-based mapping
                var indexAttr = prop.GetCustomAttribute<CsvColumnIndexAttribute>();
                if (indexAttr != null && !propertyMap.ContainsValue(prop))
                {
                    propertyMap[indexAttr.Index] = prop;
                }
            }
            
            var startRow = hasHeader ? 1 : 0;

            for (var i = startRow; i < result.Count; i++)
            {
                var row = result[i];
                T obj = Activator.CreateInstance<T>();

                for (int j = 0; j < row.Count; j++)
                {
                    if (propertyMap.TryGetValue(j, out PropertyInfo prop))
                    {
                        var value = ConvertValue(row[j], prop.PropertyType);
                        prop.SetValue(obj, value);
                    }
                }

                yield return obj;
            }
        }

        /// <summary>
        /// Load CSV data from string.
        /// </summary>
        /// <param name="data">CSV string</param>
        /// <param name="hasHeader">Does this CSV file have a header row</param>
        /// <param name="removeHeader">Remove header row from the result</param>
        /// <param name="delimiter">Delimiter.</param>
        public static List<List<string>> ParseFromString(
            string data,
            bool hasHeader,
            bool removeHeader = true,
            Delimiter delimiter = Auto)
        {
            if (delimiter == Auto)
            {
                delimiter = DelimiterUtils.DetectDelimiterFromContent(data);
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

            if (hasHeader && removeHeader && sheet.Count > 0)
            {
                sheet.RemoveAt(0);
            }

            if (hasHeader && !removeHeader && sheet.Count == 1)
            {
                sheet.Clear();
            }

            return sheet;
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

        private static object ConvertValue(string value, Type targetType)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                if (targetType == typeof(string))
                    return string.Empty;

                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }

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