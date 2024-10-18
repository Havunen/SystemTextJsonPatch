using System.Text.Json;
using BenchmarkDotNet.Attributes;
using SystemTextJsonPatch.Benchmark.Benchmarks.Models;

namespace SystemTextJsonPatch.Benchmark.Benchmarks;

public class DeserializeAndApplyToBenchmark
{
	private JsonSerializerOptions _systemTextJsonSerializerOptions;


	[GlobalSetup]
	public void GlobalSetup()
	{
		_systemTextJsonSerializerOptions = new JsonSerializerOptions()
		{
		};
	}

	public static string DeserializePatchDocJson = string.Format(
		"[" +
		"{{\"op\": \"replace\", \"path\": \"Number\", \"value\": 86632}}," +
		"{{\"op\": \"replace\", \"path\": \"Text\", \"value\": \"testing-performance\"}}," +
		"{{\"op\": \"add\", \"path\": \"Amount\", \"value\": 86632.172712}}," +
		"{{\"op\": \"replace\", \"path\": \"Amount2\", \"value\": null}}," +
		"{{\"op\": \"replace\", \"path\": \"SubTestModel\", \"value\": {{\"Id\": 91117, \"Data\": 78}}}}," +
		"{{\"op\": \"test\", \"path\": \"Number\", \"value\": 86632}}," +
		"{{\"op\": \"copy\", \"path\": \"Amount2\", \"from\": \"Amount\"}}," +
		"{{\"op\": \"remove\", \"path\": \"Text\"}}" +
		"]"
	);


	[Benchmark]
	public void SystemTextJsonPatch()
	{
		var patchDoc = JsonSerializer.Deserialize<JsonPatchDocument<TestModel>>(DeserializePatchDocJson, _systemTextJsonSerializerOptions);
		patchDoc?.ApplyTo(new TestModel());
	}

	[Benchmark]
	public void MarvinJsonPatch()
	{
		var patchDoc = Newtonsoft.Json.JsonConvert.DeserializeObject<Marvin.JsonPatch.JsonPatchDocument<TestModel>>(DeserializePatchDocJson);
		patchDoc?.ApplyTo(new TestModel());
	}

	[Benchmark]
	public void AspNetCoreJsonPatch()
	{
		var patchDoc = Newtonsoft.Json.JsonConvert.DeserializeObject<Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<TestModel>>(DeserializePatchDocJson);
		patchDoc?.ApplyTo(new TestModel());
	}
}
