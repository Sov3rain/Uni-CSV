using System.Linq;
using NUnit.Framework;

namespace UniCSV.Tests
{
    [TestFixture]
    public class IndexAttributeMappingTests
    {
        private class PersonWithIndexAttributes
        {
            [CsvColumnIndex(0)]
            public string Name { get; set; }

            [CsvColumnIndex(1)]
            public int Age { get; set; }

            [CsvColumnIndex(2)]
            public string Location { get; set; }
        }

        private class MixedAttributesClass
        {
            [CsvColumnIndex(1)]
            public string LastName { get; set; }

            [CsvColumn("Age")]
            public int Age { get; set; }

            [CsvColumnIndex(0)]
            public string FirstName { get; set; }
        }

        private class NullableTypesWithIndexClass
        {
            [CsvColumnIndex(0)]
            public int? NullableInt { get; set; }

            [CsvColumnIndex(1)]
            public decimal? NullableDecimal { get; set; }

            [CsvColumnIndex(2)]
            public string StringValue { get; set; }
        }

        [Test]
        public void ParseFromString_WithIndexAttributes_MapsCorrectly()
        {
            string csvData = "John,25,USA";
            var result = CsvParser.ParseFromString<PersonWithIndexAttributes>(csvData, hasHeader: false).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("John", result[0].Name);
            Assert.AreEqual(25, result[0].Age);
            Assert.AreEqual("USA", result[0].Location);
        }

        [Test]
        public void ParseFromString_WithIndexAttributes_HandlesEmptyValues()
        {
            string csvData = ",25,\nJane,,UK";
            var result = CsvParser.ParseFromString<PersonWithIndexAttributes>(csvData, hasHeader: false).ToList();

            Assert.AreEqual(2, result.Count);

            Assert.IsEmpty(result[0].Name);
            Assert.AreEqual(25, result[0].Age);
            Assert.IsEmpty(result[0].Location);

            Assert.AreEqual("Jane", result[1].Name);
            Assert.AreEqual(0, result[1].Age);
            Assert.AreEqual("UK", result[1].Location);
        }

        [Test]
        public void ParseFromString_WithIndexAttributes_HandlesNullableTypes()
        {
            string csvData = ",,Value\nInvalid,Invalid,Test";
            var result = CsvParser.ParseFromString<NullableTypesWithIndexClass>(csvData, hasHeader: false).ToList();

            Assert.AreEqual(2, result.Count);

            Assert.IsNull(result[0].NullableInt);
            Assert.IsNull(result[0].NullableDecimal);
            Assert.AreEqual("Value", result[0].StringValue);

            Assert.IsNull(result[1].NullableInt);
            Assert.IsNull(result[1].NullableDecimal);
            Assert.AreEqual("Test", result[1].StringValue);
        }

        [Test]
        public void ParseFromString_WithMixedAttributes_MapsCorrectly()
        {
            string csvData = "FirstName,LastName,Age\nJohn,Doe,30";
            var result = CsvParser.ParseFromString<MixedAttributesClass>(csvData, hasHeader: true).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("John", result[0].FirstName);
            Assert.AreEqual("Doe", result[0].LastName);
            Assert.AreEqual(30, result[0].Age);
        }

        [Test]
        public void ParseFromString_WithOutOfRangeIndex_IgnoresProperty()
        {
            var csvData = "John";
            var result = CsvParser.ParseFromString<PersonWithIndexAttributes>(csvData, hasHeader: false).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("John", result[0].Name);
            Assert.AreEqual(0, result[0].Age); // Default value for int
            Assert.IsNull(result[0].Location); // Default value for string
        }
    }
}