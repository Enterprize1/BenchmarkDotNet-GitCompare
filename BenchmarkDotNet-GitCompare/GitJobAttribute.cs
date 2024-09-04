using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace BenchmarkDotNet_GitCompare;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
public class GitJobAttribute : Attribute, IConfigSource // Not extending JobConfigBaseAttribute because we also want to add a column
{
    private const int DefaultValue = -1;

    public GitJobAttribute(
        string gitReference = "HEAD",
        RuntimeMoniker runtimeMoniker = RuntimeMoniker.HostProcess,
        int launchCount = DefaultValue,
        int warmupCount = DefaultValue,
        int iterationCount = DefaultValue,
        int invocationCount = DefaultValue,
        string? id = null,
        bool baseline = false
    )
    {
        Config = new ManualConfig()
            .AddJob(CreateJob(gitReference, id, launchCount, warmupCount, iterationCount, invocationCount, null, baseline, runtimeMoniker))
            .AddColumn(GitReferenceColumn.Instance);
    }
    
    private static Job CreateJob(string gitReference, string? id, int launchCount, int warmupCount, int iterationCount, int invocationCount, RunStrategy? runStrategy,
        bool baseline, RuntimeMoniker runtimeMoniker)
    {
        var job = new Job(id);

        if (launchCount != DefaultValue)
        {
            job = job.WithLaunchCount(launchCount);
        }

        if (warmupCount != DefaultValue)
        {
            job = job.WithWarmupCount(warmupCount);
        }

        if (iterationCount != DefaultValue)
        {
            job = job.WithIterationCount(iterationCount);
        }

        if (invocationCount != DefaultValue)
        {
            job = job.WithInvocationCount(invocationCount);
            
            int unrollFactor = job.Run.ResolveValue(RunMode.UnrollFactorCharacteristic, EnvironmentResolver.Instance);
            if (invocationCount % unrollFactor != 0)
            {
                job.Run.UnrollFactor = 1;
            }
        }

        if (runStrategy != null)
        {
            job.Run.RunStrategy = runStrategy.Value;
        }

        if (baseline)
            job.Meta.Baseline = true;

        if (runtimeMoniker != RuntimeMoniker.HostProcess)
        {
            job = job.WithRuntime(runtimeMoniker.GetRuntime());
        }

        job = job.WithGitReference(gitReference);

        return job.Freeze();
    }

    public IConfig Config { get; }
}