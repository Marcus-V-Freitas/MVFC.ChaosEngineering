# Contributing to MVFC.ChaosEngineering

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download) or later
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) running locally
- Git

## Running locally

```sh
git clone https://github.com/Marcus-V-Freitas/MVFC.ChaosEngineering.git
cd MVFC.ChaosEngineering
dotnet restore MVFC.ChaosEngineering.slnx
dotnet build MVFC.ChaosEngineering.slnx --configuration Release
```

## Running tests

The tests use `Aspire.Hosting.Testing` for playground orchestration and require Docker to be running.

```sh
dotnet test tests/MVFC.ChaosEngineering.Tests/MVFC.ChaosEngineering.Tests.csproj --configuration Release
```

## Adding a new Chaos Kind

To implement a new type of fault injection:

1.  **Define the Kind**: Add a new entry to the `ChaosKind` enum in `src/MVFC.ChaosEngineering/Enums/ChaosKind.cs`.
2.  **Implement the Handler**: Create a new class implementing `IChaosHandler` in `src/MVFC.ChaosEngineering/Handlers/`.
3.  **Register the Handler**: Map the new `ChaosKind` to your handler in `ChaosHandlerRegistry.cs`.
4.  **Extend the Builder**: Add the corresponding fluent methods to `ChaosPolicyBuilder.cs` and `ChaosRule.cs` to allow users to configure the new fault.
5.  **Test**: Add unit and integration tests in `tests/MVFC.ChaosEngineering.Tests/`.
6.  **Document**: Update `README.md` and `README.pt-BR.md` tables and diagrams.

## Branch naming

- `feat/` — new feature or Chaos Kind
- `fix/` — bug fix
- `chore/` — dependency update or maintenance
- `docs/` — documentation only
- `test/` — tests only
- `refactor/` — no feature change, no bug fix

Example: `feat/add-network-partition-kind`

## Commit convention

This project follows [Conventional Commits](https://www.conventionalcommits.org/):

- `feat: add BandwidthThrottle kind`
- `fix: fix specificity matching for deep wildcards`
- `docs: update README diagrams`
- `chore: bump Microsoft.Extensions.Http to 9.0.0`
- `test: add integration tests for SlowBody kind`

## Pull Request process

1. Fork and create your branch from `main`
2. Make your changes and ensure all tests pass locally
3. Open a PR against `main` and fill in the PR template
4. Wait for the CI to pass
5. A maintainer will review and merge

