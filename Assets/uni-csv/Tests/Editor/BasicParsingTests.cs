using NUnit.Framework;

namespace Sov3rain.Tests
{
    [TestFixture]
    public class BasicParsingTests
    {
        [Test]
        public void ParseFromString_CommaDelimited_ReturnsCorrectData()
        {
            string csvData = "Name,Age,Location\nJohn,25,USA\nJane,30,UK";
            var result = CSVParser.ParseFromString(csvData, delimiter: CSVParser.Delimiter.Comma);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("John", result[0][0]);
            Assert.AreEqual("25", result[0][1]);
            Assert.AreEqual("Jane", result[1][0]);
        }

        [Test]
        public void ParseFromString_EmptyFile_ReturnsEmptyList()
        {
            string csvData = "";
            var result = CSVParser.ParseFromString(csvData);

            Assert.AreEqual(0, result.Count);
        }
    }
}