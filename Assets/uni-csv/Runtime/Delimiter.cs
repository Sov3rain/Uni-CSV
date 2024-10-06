using System;
using System.Linq;
using static UniCSV.Delimiter;

namespace UniCSV
{
    public enum Delimiter
    {
        Auto,
        Comma,
        Tab,
        Semicolon,
        Pipe
    }

    public static class DelimiterUtils
    {
        private static readonly char[] COMMON_DELIMITERS = { ',', '\t', ';', '|' };
        
        public static Delimiter DetectDelimiterFromContent(string content)
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
        
        public static char ToChar(this Delimiter delimiter) => delimiter switch
        {
            Comma => ',',
            Tab => '\t',
            Semicolon => ';',
            Pipe => '|',
            _ => throw new ArgumentException($"Unsupported delimiter: {delimiter}")
        };

        public static Delimiter CharToDelimiter(char delimiterChar) => delimiterChar switch
        {
            ',' => Comma,
            '\t' => Tab,
            ';' => Semicolon,
            '|' => Pipe,
            _ => Comma
        };
    }
}