# GitCompare for BenchmarkDotNet

## Overview

GitCompare for BenchmarkDotNet is an extension package for [BenchmarkDotNet](https://benchmarkdotnet.org/) that allows developers to compare benchmarks across different git references (e.g., commits, branches, tags). This enables you to track the performance impact of code changes over time.

## Installation

To install the package, add it to your project via NuGet:

```shell
dotnet add package Enterprize1.BenchmarkDotNet.GitCompare
```

## Getting Started

To use BenchmarkComparison, you will need to set up your benchmark project with BenchmarkDotNet and then add a new Job for a different git reference.

### Example (Attribute-Based)

```csharp
using BenchmarkDotNet_GitCompare;
using BenchmarkDotNet.Attributes;

[SimpleJob(id: "now")]
[GitJob(gitReference: "HEAD", id: "before", baseline: true)]
[MemoryDiagnoser]
public class Bench
{
    [Benchmark]
    public Task Test()
    {
        return Task.Delay(10);
    }
}
```

### Example (Fluent Config API)
This package also supports the fluent configuration API provided by BenchmarkDotNet by adding a `.WithGitReference()` method to the `Job` class.


## How It Works
Overrides the toolchain of Jobs with a git reference with a small decorator around the original toolchain. The decorator will checkout the git reference before running the benchmarks, change references for the benchmarks to this new checkout and then removes the folder after the benchmarks have completed.

## Limitations
Works for my (simple) use case, but may not work for all. Please open an issue (or PR) if you have a use case that is not supported.