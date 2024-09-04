using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Toolchains.DotNetCli;
using BenchmarkDotNet.Toolchains.Mono;
using BenchmarkDotNet.Toolchains.NativeAot;
using BenchmarkDotNet.Toolchains.Roslyn;

namespace BenchmarkDotNet_GitCompare;

public static class GitJobExtensions
{
    public static Job WithGitReference(this Job job, string gitReference)
    {
        // TODO: Maybe create a new config based off of the job and somehow call the original ToolchainExtensions.GetToolchain from that. Copying for now.  
        var originalToolchain = GetToolchain(job);
        return job.WithToolchain(GitAwareToolchain.From(originalToolchain, gitReference));
    }

    private static IToolchain GetToolchain(Job job)
    {
        return job.Infrastructure.TryGetToolchain(out var toolchain)
            ? toolchain
            : GetToolchain(
                job.ResolveValue(EnvironmentMode.RuntimeCharacteristic, EnvironmentResolver.Instance));
    }

    private static IToolchain GetToolchain(this Runtime runtime)
    {
        switch (runtime)
        {
            case ClrRuntime clrRuntime:
                return clrRuntime.RuntimeMoniker != RuntimeMoniker.NotRecognized
                    ? clrRuntime.RuntimeMoniker.GetToolchain()
                    : CsProjClassicNetToolchain.From(clrRuntime.MsBuildMoniker);

            case MonoRuntime mono:
                if (!string.IsNullOrEmpty(mono.AotArgs))
                    return MonoAotToolchain.Instance;

                return RoslynToolchain.Instance;

            case CoreRuntime coreRuntime:
                if (coreRuntime.RuntimeMoniker != RuntimeMoniker.NotRecognized && !coreRuntime.IsPlatformSpecific)
                    return coreRuntime.RuntimeMoniker.GetToolchain();

                return CsProjCoreToolchain.From(new NetCoreAppSettings(coreRuntime.MsBuildMoniker, null,
                    coreRuntime.Name));

            case NativeAotRuntime nativeAotRuntime:
                return nativeAotRuntime.RuntimeMoniker != RuntimeMoniker.NotRecognized
                    ? nativeAotRuntime.RuntimeMoniker.GetToolchain()
                    : NativeAotToolchain.CreateBuilder().UseNuGet()
                        .TargetFrameworkMoniker(nativeAotRuntime.MsBuildMoniker).ToToolchain();
            default:
                throw new ArgumentOutOfRangeException(nameof(runtime), runtime, "Runtime not supported by GitCompare");
        }
    }

    internal static IToolchain GetToolchain(this RuntimeMoniker runtimeMoniker)
    {
        switch (runtimeMoniker)
        {
            case RuntimeMoniker.Net461:
                return CsProjClassicNetToolchain.Net461;

            case RuntimeMoniker.Net462:
                return CsProjClassicNetToolchain.Net462;

            case RuntimeMoniker.Net47:
                return CsProjClassicNetToolchain.Net47;

            case RuntimeMoniker.Net471:
                return CsProjClassicNetToolchain.Net471;

            case RuntimeMoniker.Net472:
                return CsProjClassicNetToolchain.Net472;

            case RuntimeMoniker.Net48:
                return CsProjClassicNetToolchain.Net48;

            case RuntimeMoniker.Net481:
                return CsProjClassicNetToolchain.Net481;

            case RuntimeMoniker.NetCoreApp20:
                return CsProjCoreToolchain.NetCoreApp20;

            case RuntimeMoniker.NetCoreApp21:
                return CsProjCoreToolchain.NetCoreApp21;

            case RuntimeMoniker.NetCoreApp22:
                return CsProjCoreToolchain.NetCoreApp22;

            case RuntimeMoniker.NetCoreApp30:
                return CsProjCoreToolchain.NetCoreApp30;

            case RuntimeMoniker.NetCoreApp31:
                return CsProjCoreToolchain.NetCoreApp31;
#pragma warning disable CS0618 // Type or member is obsolete
            case RuntimeMoniker.NetCoreApp50:
#pragma warning restore CS0618 // Type or member is obsolete
            case RuntimeMoniker.Net50:
                return CsProjCoreToolchain.NetCoreApp50;

            case RuntimeMoniker.Net60:
                return CsProjCoreToolchain.NetCoreApp60;

            case RuntimeMoniker.Net70:
                return CsProjCoreToolchain.NetCoreApp70;

            case RuntimeMoniker.Net80:
                return CsProjCoreToolchain.NetCoreApp80;

            case RuntimeMoniker.Net90:
                return CsProjCoreToolchain.NetCoreApp90;

            case RuntimeMoniker.NativeAot60:
                return NativeAotToolchain.Net60;

            case RuntimeMoniker.NativeAot70:
                return NativeAotToolchain.Net70;

            case RuntimeMoniker.NativeAot80:
                return NativeAotToolchain.Net80;

            case RuntimeMoniker.NativeAot90:
                return NativeAotToolchain.Net90;

            case RuntimeMoniker.Mono60:
                return MonoToolchain.Mono60;

            case RuntimeMoniker.Mono70:
                return MonoToolchain.Mono70;

            case RuntimeMoniker.Mono80:
                return MonoToolchain.Mono80;

            case RuntimeMoniker.Mono90:
                return MonoToolchain.Mono90;

            default:
                throw new ArgumentOutOfRangeException(nameof(runtimeMoniker), runtimeMoniker,
                    "RuntimeMoniker not supported");
        }
    }

    internal static Runtime GetRuntime(this RuntimeMoniker runtimeMoniker)
    {
        switch (runtimeMoniker)
        {
            case RuntimeMoniker.Net461:
                return ClrRuntime.Net461;
            case RuntimeMoniker.Net462:
                return ClrRuntime.Net462;
            case RuntimeMoniker.Net47:
                return ClrRuntime.Net47;
            case RuntimeMoniker.Net471:
                return ClrRuntime.Net471;
            case RuntimeMoniker.Net472:
                return ClrRuntime.Net472;
            case RuntimeMoniker.Net48:
                return ClrRuntime.Net48;
            case RuntimeMoniker.Net481:
                return ClrRuntime.Net481;
            case RuntimeMoniker.NetCoreApp20:
                return CoreRuntime.Core20;
            case RuntimeMoniker.NetCoreApp21:
                return CoreRuntime.Core21;
            case RuntimeMoniker.NetCoreApp22:
                return CoreRuntime.Core22;
            case RuntimeMoniker.NetCoreApp30:
                return CoreRuntime.Core30;
            case RuntimeMoniker.NetCoreApp31:
                return CoreRuntime.Core31;
            case RuntimeMoniker.Net50:
#pragma warning disable CS0618 // Type or member is obsolete
            case RuntimeMoniker.NetCoreApp50:
#pragma warning restore CS0618 // Type or member is obsolete
                return CoreRuntime.Core50;
            case RuntimeMoniker.Net60:
                return CoreRuntime.Core60;
            case RuntimeMoniker.Net70:
                return CoreRuntime.Core70;
            case RuntimeMoniker.Net80:
                return CoreRuntime.Core80;
            case RuntimeMoniker.Net90:
                return CoreRuntime.Core90;
            case RuntimeMoniker.Mono:
                return MonoRuntime.Default;
            case RuntimeMoniker.NativeAot60:
                return NativeAotRuntime.Net60;
            case RuntimeMoniker.NativeAot70:
                return NativeAotRuntime.Net70;
            case RuntimeMoniker.NativeAot80:
                return NativeAotRuntime.Net80;
            case RuntimeMoniker.NativeAot90:
                return NativeAotRuntime.Net90;
            case RuntimeMoniker.Mono60:
                return MonoRuntime.Mono60;
            case RuntimeMoniker.Mono70:
                return MonoRuntime.Mono70;
            case RuntimeMoniker.Mono80:
                return MonoRuntime.Mono80;
            case RuntimeMoniker.Mono90:
                return MonoRuntime.Mono90;
            default:
                throw new ArgumentOutOfRangeException(nameof(runtimeMoniker), runtimeMoniker,
                    "Runtime Moniker not supported");
        }
    }
}