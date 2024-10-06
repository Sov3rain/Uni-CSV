using NUnit.Framework;

namespace UniCSV.Tests
{
    [TestFixture]
    public class DelimiterTests
    {
        [Test]
        public void ParseFromString_AutoDetectDelimiter_SemicolonDelimited()
        {
            string csvData = "Name;Age;Location\nJohn;25;USA";
            var result = CsvParser.ParseFromString(csvData);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("John", result[0][0]);
            Assert.AreEqual("25", result[0][1]);
        }

        [TestCase("Name\tAge\tLocation\nJohn\t25\tUSA", TestName = "TabDelimited")]
        [TestCase("Name|Age|Location\nJohn|25|USA", TestName = "PipeDelimited")]
        public void ParseFromString_AutoDetectDelimiter(string csvData)
        {
            var result = CsvParser.ParseFromString(csvData);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("John", result[0][0]);
            Assert.AreEqual("25", result[0][1]);
        }
    }
}