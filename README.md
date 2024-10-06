# Uni-CSV

## Overview

The **CSV Parser for Unity** is a lightweight and efficient CSV parsing utility designed specifically for Unity projects. It allows easy and flexible parsing of CSV data from both files and strings, supporting multiple delimiter types (comma, tab, semicolon, pipe) and handling common edge cases like quoted fields and multiline data.

## Supported Unity versions

This package runs on Unity **2019.2 or later**.

## Installation

This package can be installed with the built-in Unity Package Manager. Open the UPM window, click on the `+` icon, choose **Add package from git URL...** and paste this URL:

```
https://github.com/Sov3rain/Uni-CSV.git?path=/Assets/uni-csv
```

You can alternatively install a specific version:

```
https://github.com/Sov3rain/Uni-CSV.git?path=/Assets/uni-csv#1.0.0
```

## Usage

### Basic

Returns CSV data as `List<List<string>>`.

You can parse a string using:

```c#
CSVParser.ParseFromString(
    string data, 
    bool header = true, 
    Delimiter delimiter = Delimiter.Auto)
```

or a file using:

```c#
CSVParser.ParseFromPath(
    string path,
    Delimiter delimiter = Delimiter.Auto,
    bool header = true,
    Encoding encoding = null)
```

Both methods have the `header` parameter set to `true` by default. If your CSV file does not contains a header row, set this parameter to `false`.

### Advanced

You can map your CSV to a concrete type using generic methods, which will return an `IEnumerator<T>`. Keep in mind that for the mapping to work properly, the input string or file **must include a header row** when using these generic methods.

```c#
CSVParser.ParseFromString<T>(
    string data,
    Delimiter delimiter = Delimiter.Auto)
```

```c#
CSVParser.ParseFromPath<T>(
    string path,
    Delimiter delimiter = Delimiter.Auto,
    Encoding = null)
```

> Mapping the CSV to a collection of concrete types is performed using reflection, which can affect performance, even though it is only used once on the header row.

### Examples

Getting back a `List<LIst<string>>`:

```c#
var sheet = CSVParser.ParseFromString(csvString);

foreach (var row in sheet)
{
    Debug.Log(string.Join(", ", row));
}
```

Getting back a mapped collection of objects:

```c#
var users = CSVParser.ParseFromString<User>(csvString);

foreach (User user in users)
{
    Debug.Log(user.Username);    
}
```

## Specs

Compliant with [RFC 4180](http://www.ietf.org/rfc/rfc4180.txt).

- Delimiter auto detection (`,`, `;`, `\t` and `|` supported).
- Correctly parse new lines, commas and quotation marks inside cell.
- Escape double quotes.
- Support for some encoding types (default is UTF-8).
- Correctly escapes empty lines and empty content lines.

## Roadmap

- Streaming loading
- CSV Writing

## Tests

This package has a set of tests that can be run with the [Unity Test Framework](https://docs.unity3d.com/Packages/com.unity.test-framework@1.4/manual/index.html).

## Acknowledgment

This package is freely inspired by [GitHub - yutokun/CSV-Parser: CSV Parser for C# without any dependency (on recent platforms).](https://github.com/yutokun/CSV-Parser)
