using System.Text.Json;
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

			var docJson = JsonSerializer.Deserialize<JsonPatchDocument<TestDoc>>("[{\"oP\":\"rEplAce\",\"pAtH\":\"/valUe\",\"fROm\":\"from\",\"value\":\"myValue\"},{\"op\":\"replace\",\"PATH\":\"/value\",\"VALUE\":\"myValue\"}]", options);

			Assert.Equal(2, docJson.Operations.Count);
		}

		[Fact]
		public void DeserializeShouldFollowCaseInsensitiveSettingOfSystemTextJsonOptions2()
		{
			var options = new JsonSerializerOptions(JsonSerializerDefaults.General);
			options.PropertyNameCaseInsensitive = false;
			options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

			Assert.Throws<JsonPatchException>(() => JsonSerializer.Deserialize<JsonPatchDocument<TestDoc>>("[{\"oP\":\"rEplAce\",\"pAtH\":\"/valUe\",\"fROm\":\"from\",\"value\":\"myValue\"},{\"op\":\"replace\",\"PATH\":\"/value\",\"VALUE\":\"myValue\"}]", options));
		}

		[Fact]
		public void DeserializeShouldFollowCaseInsensitiveSettingOfSystemTextJsonOptions3()
		{
			var options = new JsonSerializerOptions(JsonSerializerDefaults.General);
			options.PropertyNameCaseInsensitive = false;
			options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

			var doc = JsonSerializer.Deserialize<JsonPatchDocument<TestDoc>>("[{\"op\":\"replace\",\"path\":\"/valUe\",\"from\":\"from\",\"value\":\"myValue\"}]", options);

			Assert.Single(doc.Operations);
		}
	}
}

