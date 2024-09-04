using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using Sample;

BenchmarkRunner.Run<Bench>(new FooConfig());


public class FooConfig : ManualConfig
{
    public FooConfig()
    {
        Add(JitOptimizationsValidator.DontFailOnError); // ALLOW NON-OPTIMIZED DLLS

        Add(DefaultConfig.Instance.GetLoggers().ToArray()); // manual config has no loggers by default
        Add(DefaultConfig.Instance.GetExporters().ToArray()); // manual config has no exporters by default
        Add(DefaultConfig.Instance.GetColumnProviders().ToArray()); // manual config has no columns by default;
    }
}
