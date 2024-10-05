# Uni-CSV | CSV Parser for Unity

## Supported Unity versions

This package runs on 2019.2 or later.

## Installation

This package can be installed with the built-in Unity Package Manager. Open the UPM window, click on the `+` icon, choose **Add package from git URL...** and paste this URL:

```
https://github.com/Sov3rain/Uni-CSV.git?path=/Assets/uni-csv
```
You can alternatively pin a specific version:
```
https://github.com/Sov3rain/Uni-CSV.git?path=/Assets/uni-csv#v1.0.0
```

## Usage

### Methods

This returns CSV data as `List<List<string>>`.

```c#
CSVParser.ParseFromString(string data)
```

or

```c#
CSVParser.ParseFromPath(string path, Encoding encoding = null)
```

### Examples

```c#
var sheet = CSVParser.ParseFromString(csvString);

var styled = new StringBuilder();
foreach (var row in sheet)
{
    styled.Append("| ");

    foreach (var cell in row)
    {
        styled.Append(cell);
        styled.Append(" | ");
    }

    styled.AppendLine();
}

Debug.Log(styled.ToString());
```

## Specs

Compliant with [RFC 4180](http://www.ietf.org/rfc/rfc4180.txt).

- Correctly parse new lines, commas, quotation marks inside cell.
- Escaped double quotes.
- Some encoding types. (default UTF-8)

## Beta

- Tab delimiter support
- Async loading