using BenchmarkDotNet.Characteristics;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains;
using BenchmarkDotNet.Validators;

namespace BenchmarkDotNet_GitCompare;

public class GitAwareToolchain : IToolchain
{
    private readonly IToolchain _impl;
    public string GitReference { get; }

    public GitAwareToolchain(IToolchain impl, string gitReference)
    {
        _impl = impl;
        GitReference = gitReference;
        Generator = new GitAwareGenerator(_impl.Generator, gitReference);
    }
    public IEnumerable<ValidationError> Validate(BenchmarkCase benchmarkCase, IResolver resolver)
    {
        return _impl.Validate(benchmarkCase, resolver);
    }

    public string Name => _impl.Name + " (Git aware)";
    public IGenerator Generator { get; }

    public IBuilder Builder => _impl.Builder;

    public IExecutor Executor => _impl.Executor;

    public bool IsInProcess => _impl.IsInProcess;

    public static IToolchain From(IToolchain original, string gitReference)
        => new GitAwareToolchain(original, gitReference);
    
    public override bool Equals(object? obj)
    {
        if (obj is GitAwareToolchain other)
        {
            return Equals(other);
        }
        
        return obj == this;
    }

    private bool Equals(GitAwareToolchain other)
    {
        return _impl.Equals(other._impl) && GitReference == other.GitReference;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_impl, GitReference);
    }

    public override string ToString()
    {
        return Name;
    }
}
