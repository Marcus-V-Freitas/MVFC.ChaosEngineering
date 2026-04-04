# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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

[1.0.1]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering/releases/tag/v1.0.0
