# Extension methods to fluently assert the outcome of .NET tests

[![CI](https://github.com/awesomeassertions/AwesomeAssertions.Analyzers/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/awesomeassertions/AwesomeAssertions.Analyzers/actions/workflows/ci.yml)
[![Coverage Status](https://coveralls.io/repos/github/AwesomeAssertions/AwesomeAssertions.analyzers/badge.svg?branch=main)](https://coveralls.io/github/AwesomeAssertions/AwesomeAssertions.analyzers?branch=main)
[![](https://img.shields.io/github/release/awesomeassertions/AwesomeAssertions.Analyzers.svg?label=latest%20release&color=007edf)](https://github.com/awesomeassertions/AwesomeAssertions.Analyzers/releases/latest)
[![](https://img.shields.io/nuget/dt/AwesomeAssertions.Analyzers.svg?label=downloads&color=007edf&logo=nuget)](https://www.nuget.org/packages/AwesomeAssertions.Analyzers)
[![](https://img.shields.io/librariesio/dependents/nuget/AwesomeAssertions.Analyzers.svg?label=dependent%20libraries)](https://libraries.io/nuget/AwesomeAssertions.Analyzers)
[![GitHub Repo stars](https://img.shields.io/github/stars/awesomeassertions/AwesomeAssertions.Analyzers)](https://github.com/awesomeassertions/AwesomeAssertions.Analyzers/stargazers)
[![GitHub contributors](https://img.shields.io/github/contributors/awesomeassertions/AwesomeAssertions.Analyzers)](https://github.com/awesomeassertions/AwesomeAssertions.Analyzers/graphs/contributors)
[![GitHub last commit](https://img.shields.io/github/last-commit/awesomeassertions/AwesomeAssertions.Analyzers)](https://github.com/awesomeassertions/AwesomeAssertions.Analyzers)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/m/awesomeassertions/AwesomeAssertions.Analyzers)](https://github.com/awesomeassertions/AwesomeAssertions.Analyzers/graphs/commit-activity)
[![open issues](https://img.shields.io/github/issues/awesomeassertions/AwesomeAssertions.Analyzers)](https://github.com/awesomeassertions/AwesomeAssertions.Analyzers/issues)

A collection of Analyzers based on the best practices [tips](https://awesomeassertions.org/tips/).

<!-- TODO: do we want to re-introduce this? Do we care? -->
<!-- ![Alt](https://repobeats.axiom.co/api/embed/92fd2e6496fc171c00616eaf672c3c757a1a29ac.svg "Repobeats analytics image") -->

## Analysis and Code Fix in Action

![Demo](assets/demo.gif)

## Install

using the latest stable version:

```powershell
dotnet add package AwesomeAssertions.Analyzers
```

## Docs

- [AwesomeAssertions Analyzer Docs](docs/AwesomeAssertionsAnalyzer.md)
- [MsTest Analyzer Docs](docs/MsTestAnalyzer.md)
- [NUnit4 Analyzer Docs](docs/Nunit4Analyzer.md)
- [NUnit3 Analyzer Docs](docs/Nunit3Analyzer.md)
- [Xunit Analyzer Docs](docs/XunitAnalyzer.md)

## Configuration

© Thanks to https://github.com/meziantou/Meziantou.FluentAssertionsAnalyzers

You can exclude assertion methods using the `.editorconfig` file:

````
[*.cs]
ffa_excluded_methods=M:NUnit.Framework.Assert.Fail|M:NUnit.Framework.Assert.Fail(System.String)
````

## Getting Started

### Build

```bash
dotnet build
```

### Tests

```bash
dotnet test --configuration Release  --filter 'TestCategory=Completed'
```

### Benchmarks

<!-- TODO: point at fork docs -->
https://AwesomeAssertions.github.io/AwesomeAssertions.analyzers/dev/bench/

## Example Usages
- https://github.com/SonarSource/sonar-dotnet/pull/2072
- https://github.com/microsoft/component-detection/pull/634
- https://github.com/microsoft/onefuzz/pull/3314
- https://github.com/chocolatey/choco/pull/2908
