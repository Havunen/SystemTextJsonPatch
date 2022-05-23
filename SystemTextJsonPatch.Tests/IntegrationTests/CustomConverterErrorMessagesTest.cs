using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using SystemTextJsonPatch.Exceptions;
using Xunit;

namespace SystemTextJsonPatch.Tests.IntegrationTests
{
    public class CustomConverterErrorMessagesTest
    {
        public class CustomJsonConverter : JsonConverter<TestModel>
        {
            public override TestModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new JsonException("My Custom Error Message");
            }

            public override void Write(Utf8JsonWriter writer, TestModel value, JsonSerializerOptions options)
            {
                throw new JsonException("My Custom Error Message");
            }
        }

        public class TestModel
        { 
            public TestModel TestSubModel { get; set; }
        }

        public class TestModel2
        {
            public bool IsActive { get; set; }
        }

        [Fact]
        public void JsonExceptions_From_CustomConverters_Should_Be_Shown_As_Is()
        {
            var serializerOptions = new JsonSerializerOptions()
            {
                Converters =
                {
                    new SystemTextJsonPatch.Converters.JsonPatchDocumentConverterFactory(),
                    new CustomJsonConverter()
                }
            };

            var model = new TestModel();
            var patchString = "[{\"op\": \"replace\", \"path\": \"testSubModel\", \"value\": {}}]";
            var patchDoc = System.Text.Json.JsonSerializer.Deserialize<JsonPatchDocument<TestModel>>(patchString, serializerOptions);


            var ex = Assert.Throws<JsonPatchException>(() => patchDoc.ApplyTo(model));
            Assert.Equal("My Custom Error Message", ex.Message);
        }

        [Fact]
        public void JsonExceptions_From_SystemTextJsonSerializer_Should_Not_Be_Shown()
        {
            var serializerOptions = new JsonSerializerOptions()
            {
                Converters =
                {
                    new SystemTextJsonPatch.Converters.JsonPatchDocumentConverterFactory()
                }
            };

            var model = new TestModel();
            var patchString = "[{\"op\": \"replace\", \"path\": \"testSubModel\", \"value\": \"true\"}]";
            var patchDoc = System.Text.Json.JsonSerializer.Deserialize<JsonPatchDocument<TestModel>>(patchString, serializerOptions);

            // This is to keep consistent with Microsoft.AspNetCore.JsonPatch rather than with System.Text.Json
            var ex = Assert.Throws<JsonPatchException>(() => patchDoc.ApplyTo(model));
            Assert.Equal("The value 'true' is invalid for target location.", ex.Message);
        }
    }
}
