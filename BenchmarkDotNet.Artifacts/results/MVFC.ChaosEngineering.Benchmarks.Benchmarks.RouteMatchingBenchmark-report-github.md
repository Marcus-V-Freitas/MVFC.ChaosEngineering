```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8117/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12500H 2.50GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3


```
| Method        | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| NoMatch       |  4.239 ns | 0.0451 ns | 0.0400 ns |  0.95 |    0.01 |      - |         - |          NA |
| ExactMatch    |  4.449 ns | 0.0617 ns | 0.0577 ns |  1.00 |    0.02 |      - |         - |          NA |
| HeaderMatch   | 15.642 ns | 0.1829 ns | 0.1711 ns |  3.52 |    0.06 | 0.0025 |      24 B |          NA |
| WildcardMatch | 18.593 ns | 0.1966 ns | 0.1743 ns |  4.18 |    0.06 |      - |         - |          NA |
