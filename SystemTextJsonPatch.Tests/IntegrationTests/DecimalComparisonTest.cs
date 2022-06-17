using System.Text.Json;
using SystemTextJsonPatch.Converters;
using SystemTextJsonPatch.Operations;
using Xunit;

namespace SystemTextJsonPatch.Tests.IntegrationTests
{
    public class DecimalComparisonTest
    {

        [Fact]
        public void Test_Values_Should_Be_Equal_Regardless_Of_Number_Of_Decimal_Zeroes()
        {
            var incomingOperations = new[]
            {
                new Operation
                {
                    op = "test",
                    path = "/decimal",
                    value = 1
                },
                new Operation
                {
                    op = "replace",
                    path = "/decimal",
                    value = 2
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
