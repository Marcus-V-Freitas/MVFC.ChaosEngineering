```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8117/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12500H 2.50GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3


```
| Method            | Mean      | Error     | StdDev    | Allocated |
|------------------ |----------:|----------:|----------:|----------:|
| ResolveBandwidth  | 0.8165 ns | 0.0315 ns | 0.0279 ns |         - |
| ResolveException  | 0.8222 ns | 0.0485 ns | 0.0454 ns |         - |
| ResolveUnknown    | 0.8364 ns | 0.0352 ns | 0.0312 ns |         - |
| ResolveStatusCode | 0.8407 ns | 0.0222 ns | 0.0197 ns |         - |
| ResolveLatency    | 0.8717 ns | 0.0233 ns | 0.0218 ns |         - |
