# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/),
and this project adheres to [Semantic Versioning](https://semver.org/).

## [Unreleased]
### Added
- `IntHistoryCache`, a ring buffer of recent `int` values. `GetAverage` rounds the mean to the nearest `int`, with midpoints away from zero.
- `GetCountedString(one, many)`, an English plural that uses `many` when the count is greater than 1.
- `GlobalRandom`, a shared `IRandom` slot with the same `Instance` / `SetInstance` / `ClearInstance` lifecycle as `ManeSingleton`. The first access stores a `ManeRandom`. Calls through `Instance` are serialized.

### Changed
- `ManeRandom` serializes overlapping `Next`, `Range01`, and `Range01Double` calls on the same instance to fix the `System.Random` thread-safety issue.
- `HistoryCache` serializes overlapping `Append`, `Clear`, and `GetAverage` calls on the same cache.
- `GlobalEventManager.RaiseEvent` now also invokes listeners registered for base event types. Matching listeners run in subscription order.
- Renamed constants library from Mane. to ManeConst. to eliminate conflicts with the namespace.

## [2.0.0-preview.1] - 2026-08-18
### Added
- Initial release of the extracted .NET-only codebase. Types were moved and refactored out of the legacy Unity-coupled module. Versioning starts at 2.0.0 to mark that split; this is not a new project, it's just a fresh start.
