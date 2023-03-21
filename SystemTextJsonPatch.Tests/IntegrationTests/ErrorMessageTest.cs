using System;
using System.Text.Json;
using SystemTextJsonPatch.Exceptions;
using SystemTextJsonPatch.Operations;
using Xunit;

namespace SystemTextJsonPatch.Tests.IntegrationTests
{
	public class ErrorMessageTest
	{
		protected class Test
		{
			public DateTime UtcDateTime { get; set; }
		}


		// https://github.com/dotnet/aspnetcore/issues/38872
		[Fact]
		public void JsonPatchHasErrorNotUsedInOperationDateTime()
		{
			var incomingOperations = new[]
			{
				new Operation
				{
					Op = "test",
					Path = "/utcDateTime",
					Value = "2000-01-01T01:01:01"
				}
			};

			var jsonOptions = new JsonSerializerOptions()
			{
			};

			var incomingJson = JsonSerializer.Serialize(incomingOperations, jsonOptions);
			var document = JsonSerializer.Deserialize<JsonPatchDocument<Test>>(incomingJson, jsonOptions);

			var existingEntity = new Test { UtcDateTime = new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc) };

			var exception = Assert.Throws<JsonPatchException>(() =>
			{
				document.ApplyTo(existingEntity);
			});

			Assert.Equal("The current value '2000-01-01T01:01:01Z' at path 'utcDateTime' is not equal to the test value '2000-01-01T01:01:01'.", exception.Message);
		}
	}
}
