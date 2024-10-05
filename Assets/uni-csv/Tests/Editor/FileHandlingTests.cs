using NUnit.Framework;

namespace Sov3rain.Tests
{
    [TestFixture]
    public class FileHandlingTests
    {
        [Test]
        public void ParseFromPath_WithInvalidPath_ThrowsFileNotFoundException()
        {
            Assert.Throws<System.IO.FileNotFoundException>(() =>
                CSVParser.ParseFromPath("nonexistent.csv"));
        }
    }
}