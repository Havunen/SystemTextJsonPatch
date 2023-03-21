using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;

namespace SeveraRecurrence.Benchmark;

public class Program
{

	public static void Main(string[] args)
	{
		var config = new ManualConfig()
		{
			Options = ConfigOptions.DisableOptimizationsValidator | ConfigOptions.DisableLogFile |
					  ConfigOptions.JoinSummary,
			Orderer = new DefaultOrderer(SummaryOrderPolicy.Declared, MethodOrderPolicy.Declared)
		};

		config.AddJob(Job.Default.WithWarmupCount(2));
		config.AddDiagnoser(MemoryDiagnoser.Default);
		config.AddExporter(DefaultConfig.Instance.GetExporters().ToArray());
		config.AddLogger(DefaultConfig.Instance.GetLoggers().ToArray());
		config.AddColumnProvider(DefaultConfig.Instance.GetColumnProviders().ToArray());

		var summary = BenchmarkRunner.Run(typeof(Program).Assembly, config);
	}
}