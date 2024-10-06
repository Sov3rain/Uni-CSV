using System;

namespace UniCSV
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class CsvColumnAttribute : Attribute
    {
        public string Name { get; }

        public CsvColumnAttribute(string name)
        {
            Name = name;
        }
    }
}