using System.IO;
using System.Linq;
using UnityEngine;
using NUnit.Framework;

namespace Sov3rain.Tests
{
    public class CSVParserTests
    {
        [Test]
        public void ParseFromString_CommaDelimited_ReturnsCorrectData()
        {
            string csvData = "Name,Age,Location\nJohn,25,USA\nJane,30,UK";
            var result = CSVParser.ParseFromString(csvData, true, CSVParser.Delimiter.Comma);

            Assert.AreEqual(2, result.Count); // 2 data rows
            Assert.AreEqual("John", result[0][0]);
            Assert.AreEqual("25", result[0][1]);
            Assert.AreEqual("Jane", result[1][0]);
        }

        [Test]
        public void ParseFromString_AutoDetectDelimiter_SemicolonDelimited()
        {
            string csvData = "Name;Age;Location\nJohn;25;USA";
            var result = CSVParser.ParseFromString(csvData);

            Assert.AreEqual(1, result.Count); // 1 data row
            Assert.AreEqual("John", result[0][0]);
            Assert.AreEqual("25", result[0][1]);
        }

        [Test]
        public void ParseFromString_AutoDetectDelimiter_TabDelimited()
        {
            string csvData = "Name\tAge\tLocation\nJohn\t25\tUSA";
            var result = CSVParser.ParseFromString(csvData);

            Assert.AreEqual(1, result.Count); // 1 data row
            Assert.AreEqual("John", result[0][0]);
            Assert.AreEqual("25", result[0][1]);
        }

        [Test]
        public void ParseFromString_AutoDetectDelimiter_PipeDelimited()
        {
            string csvData = "Name|Age|Location\nJohn|25|USA";
            var result = CSVParser.ParseFromString(csvData);

            Assert.AreEqual(1, result.Count); // 1 data row
            Assert.AreEqual("John", result[0][0]);
            Assert.AreEqual("25", result[0][1]);
        }

        [Test]
        public void ParseFromString_WithEscapedQuotes_ReturnsCorrectData()
        {
            string csvData = "Name,Quote\nJohn,\"He said, \"\"Hello!\"\"\"";
            var result = CSVParser.ParseFromString(csvData, true, CSVParser.Delimiter.Comma);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("He said, \"Hello!\"", result[0][1]);
        }

        [Test]
        public void ParseFromString_EmptyFile_ReturnsEmptyList()
        {
            string csvData = "";
            var result = CSVParser.ParseFromString(csvData);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ParseFromPath_CommaDelimited_ReturnsCorrectData()
        {
            // Simulating file read by writing a temporary file
            string filePath = Path.Combine(Application.persistentDataPath, "test.csv");
            string csvData = "Name,Age,Location\nJohn,25,USA\nJane,30,UK";
            File.WriteAllText(filePath, csvData);

            var result = CSVParser.ParseFromPath(filePath, CSVParser.Delimiter.Comma);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("John", result[0][0]);
            Assert.AreEqual("25", result[0][1]);
            Assert.AreEqual("Jane", result[1][0]);

            // Clean up the file
            File.Delete(filePath);
        }

        [Test]
        public void ParseFromString_CommaDelimited_WithCommasInsideQuotes_ReturnsCorrectData()
        {
            // CSV where a cell contains commas within double quotes
            string csvData = "Name,Quote\nJohn,\"Hello, world!\"\nJane,\"Nice, to, meet, you\"";

            var result = CSVParser.ParseFromString(csvData, true, CSVParser.Delimiter.Comma);

            // There should be 2 data rows
            Assert.AreEqual(2, result.Count);

            // First row check
            Assert.AreEqual("John", result[0][0]);
            Assert.AreEqual("Hello, world!", result[0][1]);

            // Second row check
            Assert.AreEqual("Jane", result[1][0]);
            Assert.AreEqual("Nice, to, meet, you", result[1][1]);
        }

        [Test]
        public void ParseFromString_CommaDelimited_WithEscapedDoubleQuotes_ReturnsCorrectData()
        {
            // CSV where a cell contains escaped double quotes within the text
            string csvData = "Name,Quote\nJohn,\"He said, \"\"Hello!\"\"\"\nJane,\"The \"\"best\"\" day\"";

            var result = CSVParser.ParseFromString(csvData, true, CSVParser.Delimiter.Comma);

            // There should be 2 data rows
            Assert.AreEqual(2, result.Count);

            // First row check
            Assert.AreEqual("John", result[0][0]);
            Assert.AreEqual("He said, \"Hello!\"", result[0][1]);

            // Second row check
            Assert.AreEqual("Jane", result[1][0]);
            Assert.AreEqual("The \"best\" day", result[1][1]);
        }

        private class TestPerson
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public string Location { get; set; }
        }

        private class TestProduct
        {
            public string Name { get; set; }
            public double Price { get; set; }
            public bool InStock { get; set; }
        }

        [Test]
        public void ParseFromString_Generic_ReturnsCorrectlyTypedData()
        {
            string csvData = "Name,Age,Location\nJohn,25,USA\nJane,30,UK";
            var result = CSVParser.ParseFromString<TestPerson>(csvData).ToList();

            Assert.AreEqual(2, result.Count);
            
            // Check first person
            Assert.AreEqual("John", result[0].Name);
            Assert.AreEqual(25, result[0].Age);
            Assert.AreEqual("USA", result[0].Location);
            
            // Check second person
            Assert.AreEqual("Jane", result[1].Name);
            Assert.AreEqual(30, result[1].Age);
            Assert.AreEqual("UK", result[1].Location);
        }

        [Test]
        public void ParseFromString_Generic_HandlesMultipleDataTypes()
        {
            string csvData = "Name,Price,InStock\nApple,1.99,true\nBanana,0.99,false";
            var result = CSVParser.ParseFromString<TestProduct>(csvData).ToList();

            Assert.AreEqual(2, result.Count);
            
            // Check first product
            Assert.AreEqual("Apple", result[0].Name);
            Assert.AreEqual(1.99, result[0].Price);
            Assert.IsTrue(result[0].InStock);
            
            // Check second product
            Assert.AreEqual("Banana", result[1].Name);
            Assert.AreEqual(0.99, result[1].Price);
            Assert.IsFalse(result[1].InStock);
        }

        [Test]
        public void ParseFromPath_Generic_ReturnsCorrectlyTypedData()
        {
            // Simulating file read by writing a temporary file
            string filePath = Path.Combine(Application.persistentDataPath, "test_generic.csv");
            string csvData = "Name,Age,Location\nJohn,25,USA\nJane,30,UK";
            File.WriteAllText(filePath, csvData);

            var result = CSVParser.ParseFromPath<TestPerson>(filePath).ToList();

            Assert.AreEqual(2, result.Count);
            
            // Check first person
            Assert.AreEqual("John", result[0].Name);
            Assert.AreEqual(25, result[0].Age);
            Assert.AreEqual("USA", result[0].Location);

            // Clean up the file
            File.Delete(filePath);
        }

        [Test]
        public void ParseFromPath_Generic_HandlesMultipleDataTypes()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "test_generic_products.csv");
            string csvData = "Name,Price,InStock\nApple,1.99,true\nBanana,0.99,false";
            File.WriteAllText(filePath, csvData);

            var result = CSVParser.ParseFromPath<TestProduct>(filePath).ToList();

            Assert.AreEqual(2, result.Count);
            
            // Check properties and their types
            Assert.IsInstanceOf<string>(result[0].Name);
            Assert.IsInstanceOf<double>(result[0].Price);
            Assert.IsInstanceOf<bool>(result[0].InStock);

            // Clean up the file
            File.Delete(filePath);
        }

        [Test]
        public void ParseFromString_Generic_EmptyFile_ReturnsEmptyList()
        {
            string csvData = "Name,Age,Location";
            var result = CSVParser.ParseFromString<TestPerson>(csvData).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ParseFromString_Generic_AutoDetectDelimiter()
        {
            string csvData = "Name;Age;Location\nJohn;25;USA";
            var result = CSVParser.ParseFromString<TestPerson>(csvData).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("John", result[0].Name);
            Assert.AreEqual(25, result[0].Age);
            Assert.AreEqual("USA", result[0].Location);
        }
    }
}