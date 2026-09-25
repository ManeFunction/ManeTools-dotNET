# Mane Tools for .NET

Tools and extensions for built-in .NET types, with no Unity engine dependencies. Useful for domain-driven design and for separating core logic code from Unity-dependent.

## Features

- **ManeSingleton** - thread-safe singleton with explicit `SetInstance` / `ClearInstance`
- **GlobalEventManager** - in-process event bus keyed by concrete event type
- **MinMaxInt / MinMaxFloat / MinMaxDouble** - unordered numeric intervals, with clamp and contains
- **IRandom / ManeRandom** - seeded random source, plus dice and coin helpers
- **AsyncTools** - poll a condition, wrap callbacks as tasks, observe completion with `OnComplete`
- **HistoryCache** - ring buffer of the last N float or double samples
- **DateTimeTools** - `Min` / `Max` for two `DateTime` values
- **Collection extensions** - `ForEach`, `Shuffle`, `InitWith`, `FillWith`, random sampling, `IsNullOrEmpty`
- **Numeric extensions** - clamp, remap, saturating `int` arithmetic
- **TimeSpan extensions** - clock strings (`HH:MM:SS`, `HH:MM`, `MM:SS`) using total hours or minutes
- **String extensions** - culture-invariant number parsers, `ToUpperFirst`, Slavic counted-noun forms
- **PathTools** - strip the last path component without rewriting `/` vs `\`
- **EnumTools** - list values, detect `[Obsolete]`, parse integer-backed members
- Almost all public API methods are covered with `NUnit` tests.

For detailed info and usage examples of everything in this package, welcome to [projects Wiki](https://github.com/ManeFunction/ManeTools-dotNET/wiki)!

## Installation

I recommend installing this package with the `OpenUPM` CLI. It keeps dependencies and updates easy to manage. If you cannot use `OpenUPM`, download the package and place it anywhere in your Unity project.

Setting up `OpenUPM` for the first time takes a few minutes, but it is worth it. `OpenUPM` is the usual registry for open-source Unity packages and works with Unity’s Package Manager (dependency resolution and updates included).

On Windows, I recommend `Git Bash` (`MINGW`) for CLI work: it is a Unix-like shell, and it is often already installed.

1. Install `OpenUPM` (skip this if you already have it):
   - If you do not have `npm`, install [Node.js](https://nodejs.org) (or on macOS: `brew install node`).
   - In a terminal, run: `npm install -g openupm-cli`.
   - You can then install packages from the `OpenUPM` registry with no extra Unity setup.
1. Install this package:
   - Open a terminal in your Unity project folder: `cd /path/to/your/project`.
   - Run: `openupm add com.manefunction.tools-dotnet`.
   - Switch back to Unity and wait for the package to finish importing.

## Why Preview?

Despite the fact that the code itself is not new, splitting one package into a few - plus a pile of refactoring - is a great way to invent fresh bugs.

Overall it should still be safe for commercial work (the original ManeTools already ships in a few dozen live projects), but let's give this split a little time to surface whatever I missed. Use at your own risk, I suppose.

## Repository info

This repo follows the [Conventional Commits](https://www.conventionalcommits.org/) specification.

[![GitHub Sponsors](https://img.shields.io/github/sponsors/ManeFunction?label=Sponsor&logo=GitHubSponsors&style=flat)](https://github.com/sponsors/ManeFunction)
[![openupm](https://img.shields.io/npm/v/com.manefunction.tools-dotnet?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.manefunction.tools-dotnet/)
[![openupm](https://img.shields.io/badge/dynamic/json?color=brightgreen&label=downloads&query=%24.downloads&suffix=%2Fmonth&url=https%3A%2F%2Fpackage.openupm.com%2Fdownloads%2Fpoint%2Flast-month%2Fcom.manefunction.tools-dotnet)](https://openupm.com/packages/com.manefunction.tools-dotnet/)