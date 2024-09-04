using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace BenchmarkDotNet_GitCompare;

public class GitReferenceColumn : IColumn
{
    public static readonly GitReferenceColumn Instance = new();
    public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
    {
        if (benchmarkCase.Job.Infrastructure.Toolchain is GitAwareToolchain gitAwareToolchain)
        {
            return gitAwareToolchain.GitReference;
        }

        return "-";
    }

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style) => GetValue(summary, benchmarkCase);

    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase)
    {  
        return false;
    }

    public bool IsAvailable(Summary summary)
    {
        return true;
    }

    public string Id => "GitReference";
    public string ColumnName => "Git Reference";
    public bool AlwaysShow => true;
    public ColumnCategory Category => ColumnCategory.Job;
    public int PriorityInCategory => 2;
    public bool IsNumeric => false;
    public UnitType UnitType => UnitType.Dimensionless;
    public string Legend => "Git reference the benchmark was run on";
}