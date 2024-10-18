using System.Text.Json;
using SystemTextJsonPatch.Console.Models;

namespace SystemTextJsonPatch.Console
{
	public class DeserializeTest
	{
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

		public static void Run()
		{
			var patchDoc = JsonSerializer.Deserialize<JsonPatchDocument<TestModel>>(DeserializePatchDocJson);
			patchDoc.ApplyTo(new TestModel());
		}
	}
}
