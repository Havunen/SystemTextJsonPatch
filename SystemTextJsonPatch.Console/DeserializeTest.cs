using System.Text.Json;
using SystemTextJsonPatch.Console.Models;

namespace SystemTextJsonPatch.Console
{
    public class DeserializeTest
    {
        public static string DeserializePatchDocJson = string.Format(
            "[" +
            "{{\"op\": \"replace\", \"path\": \"number\", \"value\": 86632}}," +
            "{{\"op\": \"replace\", \"path\": \"text\", \"value\": \"testing-performance\"}}," +
            "{{\"op\": \"add\", \"path\": \"amount\", \"value\": 86632.172712}}," +
            "{{\"op\": \"replace\", \"path\": \"amount2\", \"value\": null}}," +
            "{{\"op\": \"replace\", \"path\": \"subTestModel\", \"value\": {{\"id\": 91117, \"data\": 78}}}}" +
            "]"
        );

        public static JsonSerializerOptions systemTextJsonSerializerOptions = new JsonSerializerOptions()
        {
            Converters =
            {
                new SystemTextJsonPatch.Converters.JsonPatchDocumentConverterFactory()
            }
        };

        public static void Run()
        {
            var patchDoc = JsonSerializer.Deserialize<JsonPatchDocument<TestModel>>(DeserializePatchDocJson, systemTextJsonSerializerOptions);
            patchDoc.ApplyTo(new TestModel());
        }
    }
}
