# Changelog

All notable changes to this project will be documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.0] - 2024-10-06

### Added

- `CsvColumnIndexAttribute` to be able to map you CSV by column index.

- Tests for `CsvColumnIndexAttribute`.

### Changed

- Change parameter `header` name to `hasHeader`.

- Make `hasHeader` method parameter non-optional.

## [1.1.1] - 2024-10-06

### Breaking changes

- Renamed the base namespace to `UniCSV`.

- Renamed the primary static class to `CsvParser`.

### Changed

- Moved some parts of the code to their own utility classes and files.

## [1.1.0] - 2024-10-06

### Added

- Mapping a CSV sheet to a collection of concrete types. It allows direct mapping via property or field names, or by using the `[CsvColumn(name)]` attribute.

### Changed

- Overhauled tests suite.

## [1.0.0] - 2024-10-05

### Added

- Initial version of the package.
