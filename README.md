# Parallel Thread Test Result

## Performance

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19042.1526 (20H2/October2020Update)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4470.0), X86 LegacyJIT
  DefaultJob : .NET Framework 4.8 (4.8.4470.0), X86 LegacyJIT


```
|                Method |     Mean |   Error |   StdDev | Ratio | Allocated |
|---------------------- |---------:|--------:|---------:|------:|----------:|
|  Parallel1ThreadAsync | 903.8 ms | 2.52 ms |  1.97 ms |  0.99 |         - |
|  Parallel2ThreadAsync | 460.0 ms | 2.38 ms |  2.22 ms |  0.50 |         - |
|  Parallel4ThreadAsync | 232.5 ms | 1.40 ms |  1.17 ms |  0.25 |         - |
|  Parallel8ThreadAsync | 181.1 ms | 4.68 ms | 13.81 ms |  0.20 |         - |
| Parallel16ThreadAsync | 120.0 ms | 0.11 ms |  0.10 ms |  0.13 |   3,277 B |
| Parallel32ThreadAsync | 122.1 ms | 2.35 ms |  2.62 ms |  0.13 |   6,144 B |
|           ParallelFor | 121.8 ms | 0.33 ms |  0.31 ms |  0.13 |  27,853 B |
|            NormalExec | 915.4 ms | 6.23 ms |  5.82 ms |  1.00 |         - |
