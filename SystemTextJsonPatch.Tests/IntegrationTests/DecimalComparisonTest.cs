using System.Text.Json;
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
					Path = "/amount",
					Value = 1
				},
				new Operation
				{
					Op = "replace",
					Path = "/amount",
					Value = 2
				}
			};
			var jsonOptions = new JsonSerializerOptions()
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			};

			var incomingJson = JsonSerializer.Serialize(incomingOperations, jsonOptions);

			var document = JsonSerializer.Deserialize<JsonPatchDocument<TestAmount>>(incomingJson, jsonOptions);

			var existingEntity = new TestAmount { Amount = 1M };

			document.ApplyTo(existingEntity);

			Assert.Equal(2, existingEntity.Amount);

			existingEntity = new TestAmount { Amount = 1.0M };

			document.ApplyTo(existingEntity);

			Assert.Equal(2, existingEntity.Amount);

			existingEntity = new TestAmount { Amount = 1.00M };

			document.ApplyTo(existingEntity);

			Assert.Equal(2, existingEntity.Amount);

			existingEntity = new TestAmount { Amount = 1.000M };

			document.ApplyTo(existingEntity);

			Assert.Equal(2, existingEntity.Amount);

			existingEntity = new TestAmount { Amount = 1.0000000000000000000M };

			document.ApplyTo(existingEntity);

			Assert.Equal(2, existingEntity.Amount);
		}

		public class TestAmount
		{
			public decimal Amount { get; set; }
		}
	}
}
