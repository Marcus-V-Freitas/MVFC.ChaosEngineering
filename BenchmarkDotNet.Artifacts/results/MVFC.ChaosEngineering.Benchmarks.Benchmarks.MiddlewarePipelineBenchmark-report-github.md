```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8117/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12500H 2.50GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.100
  [Host] : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3

Toolchain=InProcessNoEmitToolchain  IterationCount=20  WarmupCount=5  

```
| Method                  | Mean     | Error     | StdDev    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------------------ |---------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
| NoChaos                 | 5.332 μs | 0.1426 μs | 0.1585 μs |  1.00 |    0.04 | 0.7324 |      - |   7.25 KB |        1.00 |
| ChaosRegistered_NoMatch | 5.483 μs | 0.1912 μs | 0.2201 μs |  1.03 |    0.05 | 0.7935 | 0.0076 |   7.28 KB |        1.00 |
| ChaosRegistered_Match   | 6.200 μs | 0.1067 μs | 0.1228 μs |  1.16 |    0.04 | 0.8621 | 0.0305 |   7.94 KB |        1.10 |
