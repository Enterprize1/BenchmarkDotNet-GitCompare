using BenchmarkDotNet_GitCompare;
using BenchmarkDotNet.Attributes;

namespace Sample;

[SimpleJob(id: "now")]
[GitJob(id: "before", baseline: true)]
[MemoryDiagnoser]
public class Bench
{
    [Benchmark]
    public Task Test()
    {
        return Task.Delay(100);
    }
}