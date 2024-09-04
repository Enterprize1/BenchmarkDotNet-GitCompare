using System.Diagnostics;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains;
using BenchmarkDotNet.Toolchains.Results;

namespace BenchmarkDotNet_GitCompare;

public class GitAwareGenerator : IGenerator
{
    private readonly IGenerator _impl;
    private readonly string _gitReference;

    public GitAwareGenerator(IGenerator impl, string gitReference)
    {
        _impl = impl;
        _gitReference = gitReference;
    }

    public GenerateResult GenerateProject(BuildPartition buildPartition, ILogger logger, string rootArtifactsFolderPath)
    {
        var result = _impl.GenerateProject(buildPartition, logger, rootArtifactsFolderPath);

        if (!result.IsGenerateSuccess)
        {
            return result;
        }

        try
        {
            // Cloning to directory outside the project directory to avoid confusion by CsProjGenerator.GetProjectFilePath about which .csproj is correct
            var tempPath = Directory.CreateTempSubdirectory().FullName;
            result = GenerateResult.Success(result.ArtifactsPaths, result.ArtifactsToCleanup.Append(tempPath).ToArray());
            CloneProjectAtGitReference(tempPath, result.ArtifactsPaths);
        }
        catch (Exception e)
        {
            return GenerateResult.Failure(result.ArtifactsPaths, result.ArtifactsToCleanup, e);
        }
        
        return result;
    }

    private void CloneProjectAtGitReference(string tempPath, ArtifactsPaths artifactsPaths)
    {
        var sha = RunGitCommand("rev-parse " + _gitReference, artifactsPaths.BinariesDirectoryPath);
        var gitRoot = RunGitCommand("rev-parse --show-toplevel", artifactsPaths.BinariesDirectoryPath);
        gitRoot = new DirectoryInfo(gitRoot).FullName;
        // TODO: Check if we can have shallow clone
        
        RunGitCommand("clone " + gitRoot + " " + tempPath, artifactsPaths.BinariesDirectoryPath);
        
        RunGitCommand("checkout " + sha, tempPath);
        
        string text = File.ReadAllText(artifactsPaths.ProjectFilePath);
        text = text.Replace(gitRoot, tempPath);
        File.WriteAllText(artifactsPaths.ProjectFilePath, text);
    }

    private string RunGitCommand(string command, string workingDirectory)
    {
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = "git",
            Arguments = command,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory,
        });

        if (process == null)
        {
            throw new GitCommandRunException("Could not start git process");
        }

        // Clone could take a long time
        process.WaitForExit(TimeSpan.FromMinutes(5));

        if (process.ExitCode != 0)
        {
            process.Kill(true);
            throw new GitCommandRunException(message: "Could not execute " + command + "\nResult: " +
                                                      process.StandardOutput.ReadToEnd() + "\nError: " +
                                                      process.StandardError.ReadToEnd());
        }

        return process.StandardOutput.ReadToEnd().Trim();
    }
}

public class GitCommandRunException : Exception
{
    public GitCommandRunException(string message) : base(message)
    {
        
    }
}
