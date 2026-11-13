# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.1] - 2026-11-13

### Changed
- **CI Runner Modernization**: Updated GitHub Actions runner from `ubuntu-22.04` to `ubuntu-latest` across all jobs (`test-and-coverage`, `build-and-publish`, and `create-release`).
- **Documentation**: Updated platform badges in root and package README files (`README.md`, `README.pt-BR.md`) to include .NET 11 (`.NET 9 | 10 | 11`).

## [2.0.0] - 2026-11-13

### Added
- **.NET 11 Support**: Added `net11.0` target framework to `MVFC.ChaosEngineering`, tests, benchmarks, and playground projects.
- **CI .NET 11 SDK**: Updated GitHub Actions workflows (`test-and-coverage` and `build-and-publish`) to include .NET 11 (`11.0.x`).
- **Unified Analyzer Suite**: Added `MVFC.Pack.Analyzers` (v4.0.2) via central build targets.
- **Local Tool Manifest**: Added `dotnet-coverage` (18.12.0) and `dotnet-reportgenerator-globaltool` (5.3.11) to `.config/dotnet-tools.json`.
- **Global SDK Configuration**: Added `global.json` SDK configuration with `latestMajor` roll-forward and prerelease support.

### Changed
- **Breaking**: Target framework upgrade across test, benchmark, and playground projects to .NET 11.
- **Case-Insensitive Header Evaluation**: Updated `ChaosRule` header value checks to use `StringComparer.OrdinalIgnoreCase`.
- **Cake Build Modernization**: Updated `build.cake` to execute local tools via `dotnet tool run` instead of downloading binaries into `./tools`.
- **Code Style & Analyzers**:
  - Configured rules in `.editorconfig` to silence `MA0004` (Task.ConfigureAwait) and `RCS1194` (Exception constructors).
  - Configured rule suppression for `AppHost.cs` top-level statement naming (`MA0048`).
- **Dependency Updates**:
  - `cake.tool` updated to 5.1.0.
  - `Aspire.Hosting.*` updated to 13.6.5.
  - `Microsoft.NET.Test.Sdk` updated to 18.12.0.
  - `Microsoft.Testing.Platform` updated to 2.5.3.
  - `xunit.v3` updated to 4.0.3.
  - `Refit` standardized to 16.4.0 across all targets.
  - Pinned `Microsoft.AspNetCore.TestHost` versions for .NET 9 (`[9.0.14]`), .NET 10 (`[10.0.14]`), and .NET 11 (`11.0.0`).
- **Playground Architecture**:
  - Refactored `ChaosPlaygroundExtensions.MapChaosEndpoints` into modular methods (`MapDomainEndpoints`, `MapLatencyEndpoints`, `MapBodyEndpoints`, `MapFaultEndpoints`) complying with method length limits (MA0051).

## [1.3.7] - 2026-11-07

### Added
- **Tag Ancestry Validation**: Added a CI pre-flight check in `build-and-publish` to verify that release tags are ancestors of the `main` branch before packaging.

### Changed
- **PR Workflow Scoping**: Restricted `test-and-coverage` executions to internal pull requests from repository owner (`Marcus-V-Freitas`).
- **Streamlined Workflow Triggers**: Removed direct push triggers on `main`, running test and coverage exclusively on pull requests and releases on tag pushes.

### Removed
- Removed `check-main-push` job from CI workflow.

## [1.3.6] - 2026-11-07

### Added
- **OIDC Trusted Publishing**: Configured `NuGet/login@v1` action for keyless OIDC authentication to NuGet.org.
- **CI Workflow Optimization**: Added `check-main-push` job to prevent redundant test runs on `main` when commits originate from merged pull requests.
- **Dual SDK Support in CI**: Configured .NET 9 and 10 SDKs setup in GitHub Actions.

### Changed
- **CI / CD Pipeline Hardening**:
  - Pinned Cake execution using `dotnet tool restore` and `dotnet tool run dotnet-cake`.
  - Added workflow concurrency control (`cancel-in-progress`) for non-tag runs.
  - Enforced validation checks and safe globbing in NuGet packaging and publishing steps.
  - Scoped PR coverage comments to internal pull requests.
- **Documentation & Badges**:
  - Updated CI and Codecov badges to explicitly track the `main` branch.
  - Standardized markdown table formatting across root and package documentation in both English and Portuguese.

### Removed
- Removed `cleanup-tag-on-failure` workflow job.

## [1.3.5] - 2026-10-31

### Added
- Enabled `CentralPackageTransitivePinningEnabled` in `Directory.Packages.props`.
- Added `MessagePack` package dependency.

### Changed
- Updated NuGet package dependencies (`Microsoft.Testing.Platform`, `Microsoft.NET.Test.Sdk`, `xunit.v3`, `xunit.runner.visualstudio`, `coverlet.collector`, `FluentAssertions`, `SonarAnalyzer.CSharp`, `MinVer`, `Aspire.Hosting.*`, `Microsoft.AspNetCore.TestHost`, and `Refit`).
- Updated `Aspire.AppHost.Sdk` to `13.6.0` in `MVFC.ChaosEngineering.Playground.AppHost`.
- Adjusted test assertions and `IChaosApi` interface to support updated `Refit` nullability and response handling.

## [1.3.4] - 2026-08-30

### Added
- Configured `global.json` and added `Microsoft.Testing.Platform` test runner support.
- Enabled `AspireUseCliBundle` property in `MVFC.ChaosEngineering.Playground.AppHost`.

### Changed
- Updated NuGet package dependencies (`xunit.v3`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `SonarAnalyzer.CSharp`, `Aspire.Hosting.*`, and `Microsoft.AspNetCore.TestHost`).

## [1.3.3] - 2026-06-26

### Changed

- Small certificate fix.

## [1.3.2] - 2026-06-26

### Changed

- Updated NuGet package dependencies.

## [1.3.1] - 2026-04-04

### Added
- **Validation & Probability Tests**: Added granular tests for probability distribution and rule fallback logic in `ChaosMiddlewareTests`.

### Fixed
- **Registry Field Lifetime**: Corrected `ChaosHandlerRegistry` to use instance-based `_handlers` instead of `static`, properly aligning with singleton DI lifetime.

## [1.3.0] - 2026-04-04

### Added
- **DI-Ready Handler System**: Introduced `IChaosHandlerRegistry` interface and support for resolution via Dependency Injection.
- **Advanced Options Support**: Added `ChaosPolicyFactory` to enable dynamic configuration via the ASP.NET Core Options Pattern.
- **Validation Shield**: New validation mechanism in the builder (`ValidateBuild`) to prevent runtime errors with invalid probabilities or parameters.
- **Test Infrastructure Expansion**: Extended coverage for `ChaosPolicyBuilder`, `ChaosHandlerRegistry`, and `ChaosOptions` to ensure 100% architectural reliability.

### Changed
- **Internal State Refactor**: `ChaosPolicyBuilder` now uses internal indexing (`_currentIndex`) for greater robustness in defining chained rules.
- **Middleware Orchestration**: `ChaosMiddleware` simplified to delegate execution to handlers registered in the service container.
- **Standardized Helpers**: Unified `HostHelper` between tests and benchmarks to reflect the new modular architecture.

### Removed
- **Static Registry Coupling**: Moved `ChaosHandlerRegistry` to the registries namespace and removed static dependency in the middleware.

## [1.2.0] - 2026-04-04

### Added
- **Performance Benchmark Suite**: Created a new `benchmarks/` project using BenchmarkDotNet to measure handler resolution, route matching, and middleware overhead.
- **Performance Documentation**: Added detailed performance metrics to `README.md` and `README.pt-BR.md`, showing sub-nanosecond handler resolution and minimal middleware overhead (+3%).
- **Central Package Management Enhancements**: Added support for `BenchmarkDotNet` and framework-specific versions for `Refit` and `TestHost` (.NET 9/10) in `Directory.Packages.props`.

### Changed
- **Refactored `CONTRIBUTING.md`**: Updated to reflect the core middleware architecture, providing a guide for implementing new `ChaosKind` handlers and removing outdated boilerplate.
- **Project Metadata & Linting**: 
    - Updated `.editorconfig` with project-wide suppressions for `CA1822` and `S1144`.
    - Enhanced `Directory.Build.props` with `InternalsVisibleTo` attributes for benchmarks and tests.
    - Cleaned up XML headers across root configuration files.
- **Improved Repository Tracking**: Updated `.gitignore` to track BenchmarkDotNet result reports while maintaining exclusion for logs and build artifacts.
- **Test Infrastructure**: Updated global usings and test helpers to align with the latest architecture.

## [1.1.0] - 2026-04-04

### Changed
- **Architectural Simplification**: Removed `ChaosDecision` structure. Middleware now interacts directly with `ChaosRule` for better performance and reduced coupling.
- **Deterministic Precedence**: Rules are now automatically sorted by specificity (pattern length). More specific rules always win over general wildcards, regardless of registration order.
- **Inclusive Wildcards**: Improved `/**` wildcard matching to correctly include the base path (e.g., `/api/**` now matches `/api`).

### Added
- Added `Deterministic Precedence` details to documentation.

## [1.0.1] - 2026-04-04

### Added
- Created `CONTRIBUTING.md` developer guide.

### Removed
- gitkeep files

## [1.0.0] - 2026-04-04

### Added
- **16 Diverse Chaos Kinds**: Comprehensive support for exceptions, latency, random 5xx errors, timeouts, connection aborts, header injection, service throttling (429), body corruption, and more.
- **Modular Handler Architecture**: High-performance, strategy-based design replacing monolithic execution with dedicated `IChaosHandler` implementations.
- **Fluent Configuration API**: Chainable `ChaosPolicyBuilder` for defining complex rules with route matching and probabilistic execution.
- **Advanced Observability**: Full integration with `System.Diagnostics.Metrics` (OpenTelemetry) and structured logging.
- **Dynamic Exception Factory**: Support for `Func<HttpContext, Exception>` to inject context-aware exceptions.
- **Request Filtering**: Capability to scope chaos injection based on incoming HTTP headers.
- **Environment Gating**: Safety mechanisms to restrict chaos to non-production environments.
- **Playground API**: Modular demonstration project with pre-configured endpoints and .NET Aspire orchestration.
- **100% Test Coverage**: Fully verified library with 100% line and branch coverage.
- **Comprehensive Documentation**: Complete XML comments and bi-lingual (EN/PT-BR) READMEs.

[2.0.0]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/compare/v1.3.7...v2.0.0
[1.3.7]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/compare/v1.3.6...v1.3.7
[1.3.6]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/compare/v1.3.5...v1.3.6
[1.3.5]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/compare/v1.3.4...v1.3.5
[1.3.4]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/compare/v1.3.3...v1.3.4
[1.3.3]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/compare/v1.3.2...v1.3.3
[1.3.2]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/compare/v1.3.1...v1.3.2
[1.3.1]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/compare/v1.3.0...v1.3.1
[1.3.0]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/compare/v1.2.0...v1.3.0
[1.2.0]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/compare/v1.0.1...v1.1.0
[1.0.1]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/releases/tag/v1.0.0
