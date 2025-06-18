using System.Text.Json;
using System.Text.Json.Serialization;
using SystemTextJsonPatch.Exceptions;
using Xunit;

namespace SystemTextJsonPatch.Tests.IntegrationTests
{
	public class PatchDocumentParseTest
	{
		public class TestDoc
		{
			public string Value { get; set; }
			public string Other { get; set; }
		}

		[Fact]
		public void DeserializeShouldFollowCaseInsensitiveSettingOfSystemTextJsonOptions()
		{
			var options = new JsonSerializerOptions(JsonSerializerDefaults.General);
			options.PropertyNameCaseInsensitive = true;

			var docJson = JsonSerializer.Deserialize<JsonPatchDocument<TestDoc>>(
				"[{\"oP\":\"rEplAce\",\"pAtH\":\"/valUe\",\"fROm\":\"from\",\"value\":\"myValue\"},{\"op\":\"replace\",\"PATH\":\"/value\",\"VALUE\":\"myValue\"}]",
				options);

			Assert.Equal(2, docJson.Operations.Count);
		}

		[Fact]
		public void DeserializeShouldFollowCaseInsensitiveSettingOfSystemTextJsonOptions2()
		{
			var options = new JsonSerializerOptions(JsonSerializerDefaults.General);
			options.PropertyNameCaseInsensitive = false;
			options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

			Assert.Throws<JsonPatchException>(() =>
				JsonSerializer.Deserialize<JsonPatchDocument<TestDoc>>(
					"[{\"oP\":\"rEplAce\",\"pAtH\":\"/valUe\",\"fROm\":\"from\",\"value\":\"myValue\"},{\"op\":\"replace\",\"PATH\":\"/value\",\"VALUE\":\"myValue\"}]",
					options));
		}

		[Fact]
		public void DeserializeShouldFollowCaseInsensitiveSettingOfSystemTextJsonOptions3()
		{
			var options = new JsonSerializerOptions(JsonSerializerDefaults.General);
			options.PropertyNameCaseInsensitive = false;
			options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

			var doc = JsonSerializer.Deserialize<JsonPatchDocument<TestDoc>>(
				"[{\"op\":\"replace\",\"path\":\"/valUe\",\"from\":\"from\",\"value\":\"myValue\"}]", options);

			Assert.Single(doc.Operations);
		}

		[Fact]
		public void Test()
		{
			Class1 obj = new();
			var sourcePatch = new JsonPatchDocument<Class1>().Replace(c => c.Id, 1000);
			var patchJson = JsonSerializer.Serialize(sourcePatch);
			var deserializedPatchDoc = JsonSerializer.Deserialize(patchJson, Class1SerializerContext.Default.JsonPatchDocumentClass1);

			sourcePatch.ApplyTo(obj); // success
			deserializedPatchDoc.ApplyTo(obj); // JsonPatchTestOperationException: The value '1000' is invalid for target location.

			Assert.Equal(1000, obj.Id);
		}
	}

	public class Class1
	{
		public int? Id { get; set; }
	}

	[JsonSerializable(typeof(JsonPatchDocument<Class1>))]
	public sealed partial class Class1SerializerContext : JsonSerializerContext { }
}
