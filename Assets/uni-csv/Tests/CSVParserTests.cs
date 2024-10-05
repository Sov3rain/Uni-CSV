using System.IO;
using UnityEngine;
using NUnit.Framework;

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
}