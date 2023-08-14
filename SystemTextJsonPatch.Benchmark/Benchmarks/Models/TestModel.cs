namespace SystemTextJsonPatch.Benchmark.Benchmarks.Models;

public class TestModel
{
	public int Number { get; set; }
	public string Text { get; set; }
	public decimal Amount { get; set; }
	public decimal? Amount2 { get; set; }
	public SubTestModel SubTestModel { get; set; }
	public ICollection<SubTestModel> SubModels { get; set; } = new List<SubTestModel>();
}

public class SubTestModel
{
	public int Id { get; set; }
	public string Text { get; set; }
	public object Data { get; set; }
}
