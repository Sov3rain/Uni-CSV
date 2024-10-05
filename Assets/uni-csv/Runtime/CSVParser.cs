using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static CSVParser.Delimiter;

public static class CSVParser
{
    private static readonly char[] CommonDelimiters = { ',', '\t', ';', '|' };

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
    /// <param name="hasHeader">Does this CSV file have a header row</param>
    /// <param name="encoding">Type of text encoding. (default UTF-8)</param>
    /// <returns>Nested list that CSV parsed.</returns>
    public static List<List<string>> ParseFromPath(
        string path,
        Delimiter delimiter = Auto,
        bool hasHeader = true,
        Encoding encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var data = File.ReadAllText(path, encoding);

        if (delimiter == Auto)
        {
            delimiter = DetectDelimiterFromContent(data);
        }

        return Parse(data, delimiter, hasHeader);
    }

    /// <summary>
    /// Load CSV data from string.
    /// </summary>
    /// <param name="data">CSV string</param>
    /// <param name="hasHeader">Does this CSV file have a header row</param>
    /// <param name="delimiter">Delimiter.</param>
    /// <returns>Nested list that CSV parsed.</returns>
    public static List<List<string>> ParseFromString(
        string data,
        bool hasHeader = true,
        Delimiter delimiter = Auto)
    {
        if (delimiter == Auto)
        {
            delimiter = DetectDelimiterFromContent(data);
        }

        return Parse(data, delimiter, hasHeader);
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

        var delimiterCounts = CommonDelimiters
            .ToDictionary(d => d, d => firstLine.Count(c => c == d));

        var mostFrequentDelimiter = delimiterCounts
            .OrderByDescending(kvp => kvp.Value)
            .First();

        if (mostFrequentDelimiter.Value <= 1)
            return Comma;

        return CharToDelimiter(mostFrequentDelimiter.Key);
    }

    private static List<List<string>> Parse(string data, Delimiter delimiter, bool hasHeader)
    {
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

        var headerSkipped = false;

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

                    if (hasHeader && !headerSkipped)
                    {
                        row.Clear(); // Discard the header row
                        headerSkipped = true;
                    }
                    else if (IsRowNonEmpty(row))
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

        // Add any remaining row if it's not empty or only delimiters
        if (IsRowNonEmpty(row) || cell.Length > 0)
        {
            AddCell(row, cell);
            AddRow(sheet, ref row);
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
}