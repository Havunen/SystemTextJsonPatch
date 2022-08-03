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
            Converters =
            {
                new Converters.JsonPatchDocumentConverterFactory()
            }
        };
    }

    public static string DeserializePatchDocJson = string.Format(
        "[" +
        "{{\"op\": \"replace\", \"path\": \"number\", \"value\": 86632}}," +
        "{{\"op\": \"replace\", \"path\": \"text\", \"value\": \"testing-performance\"}}," +
        "{{\"op\": \"add\", \"path\": \"amount\", \"value\": 86632.172712}}," +
        "{{\"op\": \"replace\", \"path\": \"amount2\", \"value\": null}}," +
        "{{\"op\": \"replace\", \"path\": \"subTestModel\", \"value\": {{\"id\": 91117, \"data\": 78}}}}" +
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