using System.Linq;
using NUnit.Framework;

namespace Sov3rain.Tests
{
    [TestFixture]
    public class AttributeMappingTests
    {
        private class PersonWithAttributes
        {
            [CsvColumn("FirstName")]
            public string Name { get; set; }

            [CsvColumn("YearsOld")]
            public int Age { get; set; }

            [CsvColumn("Country")]
            public string Location { get; set; }
        }

        private class NullableTypesClass
        {
            [CsvColumn("Int")]
            public int? NullableInt { get; set; }

            [CsvColumn("Decimal")]
            public decimal? NullableDecimal { get; set; }

            [CsvColumn("String")]
            public string StringValue { get; set; }
        }

        [Test]
        public void ParseFromString_WithAttributes_MapsCorrectly()
        {
            string csvData = "FirstName,YearsOld,Country\nJohn,25,USA";
            var result = CSVParser.ParseFromString<PersonWithAttributes>(csvData).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("John", result[0].Name);
            Assert.AreEqual(25, result[0].Age);
            Assert.AreEqual("USA", result[0].Location);
        }

        [Test]
        public void ParseFromString_WithAttributes_HandlesEmptyValues()
        {
            string csvData = "FirstName,YearsOld,Country\n,25,\nJane,,UK";
            var result = CSVParser.ParseFromString<PersonWithAttributes>(csvData).ToList();

            Assert.AreEqual(2, result.Count);

            Assert.IsEmpty(result[0].Name);
            Assert.AreEqual(25, result[0].Age);
            Assert.IsEmpty(result[0].Location);

            Assert.AreEqual("Jane", result[1].Name);
            Assert.AreEqual(0, result[1].Age);
            Assert.AreEqual("UK", result[1].Location);
        }

        [Test]
        public void ParseFromString_HandlesNullableTypes()
        {
            string csvData = "Int,Decimal,String\n,,\nInvalid,Invalid,Value";
            var result = CSVParser.ParseFromString<NullableTypesClass>(csvData).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.IsNull(result[0].NullableInt);
            Assert.IsNull(result[0].NullableDecimal);
            Assert.AreEqual("Value", result[0].StringValue);
        }
    }
}