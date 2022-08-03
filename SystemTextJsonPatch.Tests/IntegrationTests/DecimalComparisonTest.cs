using System.Text.Json;
using SystemTextJsonPatch.Converters;
using SystemTextJsonPatch.Operations;
using Xunit;

namespace SystemTextJsonPatch.Tests.IntegrationTests
{
    public class DecimalComparisonTest
    {

        [Fact]
        public void TestValuesShouldBeEqualRegardlessOfNumberOfDecimalZeroes()
        {
            var incomingOperations = new[]
            {
                new Operation
                {
                    Op = "test",
                    Path = "/decimal",
                    Value = 1
                },
                new Operation
                {
                    Op = "replace",
                    Path = "/decimal",
                    Value = 2
                }
            };
            var jsonOptions = new JsonSerializerOptions()
            {
                Converters =
                {
                    new JsonPatchDocumentConverterFactory()
                }
            };

            var incomingJson = JsonSerializer.Serialize(incomingOperations, jsonOptions);

            var document = JsonSerializer.Deserialize<JsonPatchDocument<Test>>(incomingJson, jsonOptions);

            var existingEntity = new Test { Decimal = 1M };

            document.ApplyTo(existingEntity);

            Assert.Equal(2, existingEntity.Decimal);

            existingEntity = new Test { Decimal = 1.0M };

            document.ApplyTo(existingEntity);

            Assert.Equal(2, existingEntity.Decimal);

            existingEntity = new Test { Decimal = 1.00M };

            document.ApplyTo(existingEntity);

            Assert.Equal(2, existingEntity.Decimal);

            existingEntity = new Test { Decimal = 1.000M };

            document.ApplyTo(existingEntity);

            Assert.Equal(2, existingEntity.Decimal);

            existingEntity = new Test { Decimal = 1.0000000000000000000M };

            document.ApplyTo(existingEntity);

            Assert.Equal(2, existingEntity.Decimal);
        }

        public class Test
        {
            public decimal Decimal { get; set; }
        }
    }
}
