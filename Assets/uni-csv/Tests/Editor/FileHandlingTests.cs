using NUnit.Framework;

namespace UniCSV.Tests
{
    [TestFixture]
    public class FileHandlingTests
    {
        [Test]
        public void ParseFromPath_WithInvalidPath_ThrowsFileNotFoundException()
        {
            Assert.Throws<System.IO.FileNotFoundException>(() =>
                CsvParser.ParseFromPath("nonexistent.csv"));
        }
    }
}