using System.Collections.Generic;
using System.Text.Json;
using SystemTextJsonPatch.Operations;
using Xunit;

namespace SystemTextJsonPatch.Tests.IntegrationTests
{
	public class PatchDocumentSerializationTest
	{
		[Fact]
		public void SerializeShouldNotSerializeEmptyFrom()
		{
			var options = new JsonSerializerOptions(JsonSerializerDefaults.General);

			var doc = new JsonPatchDocument(new List<Operation>()
			{
				new Operation("replace", "/value", "from", "myValue"),
				new Operation("replace", "/value", null, "myValue")
			}, options);


			var docJson = System.Text.Json.JsonSerializer.Serialize(doc, options);

			Assert.Equal("[{\"op\":\"replace\",\"path\":\"/value\",\"from\":\"from\",\"value\":\"myValue\"},{\"op\":\"replace\",\"path\":\"/value\",\"value\":\"myValue\"}]", docJson);
		}

		[Fact]
		public void SerializeShouldNotSerializeValueWhenOperationIsRemoveCopyMoveInvalid()
		{
			var options = new JsonSerializerOptions(JsonSerializerDefaults.General);

			var doc = new JsonPatchDocument(new List<Operation>()
			{
				new Operation("remove", "/value", "from", "myValue"),
				new Operation("move", "/value", "from", "myValue"),
				new Operation("invalid", "/value", "from", "myValue"),
				new Operation("copy", "/value", "from", "myValue"),
			}, options);


			var docJson = System.Text.Json.JsonSerializer.Serialize(doc, options);

			Assert.Equal("[{\"op\":\"remove\",\"path\":\"/value\",\"from\":\"from\"},{\"op\":\"move\",\"path\":\"/value\",\"from\":\"from\"},{\"op\":\"invalid\",\"path\":\"/value\",\"from\":\"from\"},{\"op\":\"copy\",\"path\":\"/value\",\"from\":\"from\"}]", docJson);
		}
	}
}
