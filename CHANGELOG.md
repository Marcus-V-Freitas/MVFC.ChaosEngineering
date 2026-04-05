# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.3.0] - 2026-04-04

### Added
- **DI-Ready Handler System**: Introduced `IChaosHandlerRegistry` interface and support for resolution via Dependency Injection.
- **Advanced Options Support**: Added `ChaosPolicyFactory` to enable dynamic configuration via the ASP.NET Core Options Pattern.
- **Validation Shield**: New validation mechanism in the builder (`ValidateBuild`) to prevent runtime errors with invalid probabilities or parameters.
- **Added more test coverage for the new features**: ChaosMiddlewareTests, ChaosPolicyBuilderTests, ChaosHandlerRegistryTests, ChaosOptionsTests, ChaosPlaygroundTests, ChaosBenchmarksTests

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

[1.3.0]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/compare/v1.2.0...v1.3.0
[1.2.0]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/compare/v1.0.1...v1.1.0
[1.0.1]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/releases/tag/v1.0.0
