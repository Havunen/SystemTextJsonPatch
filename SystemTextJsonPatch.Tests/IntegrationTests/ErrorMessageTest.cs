using System;
using System.Text.Json;
using SystemTextJsonPatch.Converters;
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
        public void JsonPatch_Has_Error_Not_Used_In_Operation_DateTime()
        {
            var incomingOperations = new[]
            {
                new Operation
                {
                    op = "test",
                    path = "/utcDateTime",
                    value = "2000-01-01T01:01:01"
                }
            };

            var jsonOptions = new JsonSerializerOptions()
            {
                Converters =
                {
                    new JsonPatchDocumentConverterFactory()
                }
            };

            var incomingJson = System.Text.Json.JsonSerializer.Serialize(incomingOperations, jsonOptions);
            var document = System.Text.Json.JsonSerializer.Deserialize<JsonPatchDocument<Test>>(incomingJson, jsonOptions);

            var existingEntity = new Test { UtcDateTime = new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc) };

            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                document.ApplyTo(existingEntity);
            });

            Assert.Equal("The current value '2000-01-01T01:01:01Z' at path 'utcDateTime' is not equal to the test value '2000-01-01T01:01:01'.", exception.Message);
        }
    }
}
