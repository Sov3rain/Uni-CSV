using System;

namespace UniCSV
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class CsvColumnIndexAttribute : Attribute
    {
        public int Index { get; }

        public CsvColumnIndexAttribute(int index)
        {
            if (index < 0)
                throw new ArgumentException("Index must be non-negative", nameof(index));
                
            Index = index;
        }
    }
}
