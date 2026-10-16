# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/),
and this project adheres to [Semantic Versioning](https://semver.org/).

## [Unreleased]
### Added
- `IntHistoryCache`, a ring buffer of recent `int` values. `GetAverage` rounds the mean to the nearest `int`, with midpoints away from zero.
- `GetCountedString(one, many)`, an English plural that uses `many` when the count is greater than 1.
- `GlobalRandom`, a shared `IRandom` slot with the same `Instance` / `SetInstance` / `ClearInstance` lifecycle as `ManeSingleton`. The first access stores a `ManeRandom`. Calls through `Instance` are serialized.
- `IRandom.Range01Inc` returns a float in `[0, 1]`, where 1 is a possible result too, next to `Range01` and its `[0, 1)`. `ManeRandom` and `GlobalRandom` implement it, and so does `UnityRandom` in ManeTools-Unity. A custom `IRandom` needs to add it.
- `RandomOrDefault(predicate, random)` returns a uniformly chosen match, or default when nothing matches. Arrays and `List<T>` are scanned directly; other lists use the indexer. Other sequences use one-pass reservoir sampling. The predicate runs once per element.

### Changed
- `ManeRandom` serializes overlapping `Next`, `Range01`, `Range01Inc`, and `Range01Double` calls on the same instance to fix the `System.Random` thread-safety issue.
- `HistoryCache` serializes overlapping `Append`, `Clear`, and `GetAverage` calls on the same cache.
- `GlobalEventManager.RaiseEvent` now also invokes listeners registered for base event types. Matching listeners run in subscription order.
- Renamed constants library from Mane. to ManeConst. to eliminate conflicts with the namespace.
- `ToUpperFirst` was renamed to `ToFirstUpper`, which now takes an optional `CultureInfo` (the current culture when omitted), and `ToFirstUpperInvariant` was added, matching `ToUpper` and `ToUpperInvariant`. The old method always used the current culture, which in a Unity player is the player's OS locale: under Turkish, "item" became "İtem" even in an English UI. Use `ToFirstUpperInvariant` for text that must be the same on every machine. A first letter outside the Basic Multilingual Plane is now uppercased as one unit, where the runtime supports it (.NET does, Unity's Mono does not).
- `ManeSingleton` no longer registers an object from its constructor. `Instance` stores the object only after its constructor has finished, so `SetInstance(new T(...))` can replace a live instance, and other threads never see a half-built one.

### Fixed
- A `ManeSingleton` constructor that threw left the half-built object as the instance for good. The constructor's own exception is now rethrown, not wrapped, and nothing is stored.
- `TryParseToIntEnum` threw `ArgumentException` for enums whose underlying type is not `int` (`byte`, `short`, `long`). It now works for any underlying type and rejects values that only fit after wrapping.
- An exception thrown by an `OnComplete` callback disappeared: it stayed inside an internal continuation task that nothing observed. It is now raised like one from an `async void` method, on the synchronization context the callback ran on (the Unity Console on the main thread), otherwise on the thread pool.
- `CreateTask` with an already cancelled token still started the operation, so its side effects happened although the task was cancelled. It now returns a cancelled task without calling it.
- The Slavic `GetCountedString(one, many, more)` returned `more` for every negative count, although its docs say the sign is ignored. -21 now gives `one` and -2 gives `many`, like 21 and 2.
- `MinMaxFloat` and `MinMaxDouble` misbehaved as dictionary or set keys: `Equals` used a tolerance while the hash used a grid, so two intervals that compared equal could hash differently and a lookup could miss. `Equals` and `GetHashCode` are now exact on the normalized bounds; `==` and `!=` still compare within the tolerance, like Unity's `Vector3`.
- `ManeRandom.Range01` could return exactly `1f` (about 3 in 100 million calls), breaking its `[0, 1)` range, because a double just under 1 rounds up when cast to float. It now draws one of the 2^24 float steps below 1.

## [2.0.0-preview.1] - 2026-08-18
### Added
- Initial release of the extracted .NET-only codebase. Types were moved and refactored out of the legacy Unity-coupled module. Versioning starts at 2.0.0 to mark that split; this is not a new project, it's just a fresh start.
