# Uni-CSV | CSV Parser for Unity

## Supported Unity versions

This package runs on 2019.2 or later.

## Installation

This package can be installed with the built-in Unity Package Manager. Open the UPM window, click on the `+` icon, choose **Add package from git URL...** and paste this URL:

```
https://github.example.com/myuser/myrepo.git?path=/Assets/uni-csv
```

## Usage

### Methods

This returns CSV data as `List<List<string>>`.

```c#
CSVParser.LoadFromString(string data)  
```

or

```c#
CSVParser.LoadFromPath(string path, Encoding encoding = null)
```

### Examples

```c#
var sheet = CSVParser.LoadFromString(csvString);

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

Debug.Log(styled.ToString());         // Unity
Console.WriteLine(styled.ToString()); // C# 
```

## Specs

Compliant with [RFC 4180](http://www.ietf.org/rfc/rfc4180.txt).

- Correctly parse new lines, commas, quotation marks inside cell.
- Escaped double quotes.
- Some encoding types. (default UTF-8)

## Beta

- Tab delimiter support

- Async loading

## Development

The repository contains multiple types of newline code. Run `git config core.autocrlf false` in your local repository.

## Why this repo has multiple Unity Examples?

One of the reasons is to check operation in different Unity versions. Another one is to build .unitypackage with CI.

Unity changes a lot between their Tech Streams. It leads different requisites / dependency to the parser. Affected changes below.

| Versions          | Difference                                     |
| ----------------- | ---------------------------------------------- |
| 2019.1 and 2019.2 | Has Scripting Runtime Version selector or not. |
| 2021.1 and 2021.2 | Requires additional DLLs or not.               |

## License

### Unique part of the repository

[CC0](https://creativecommons.org/publicdomain/zero/1.0/) or [Public Domain](LICENSE)

### .NET Runtimes (included in the .unitypackage for old Unity)

[The MIT License](https://github.com/dotnet/runtime/blob/main/LICENSE.TXT)

Copyright (c) .NET Foundation and Contributors