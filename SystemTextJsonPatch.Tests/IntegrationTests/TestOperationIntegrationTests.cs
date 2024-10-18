using System.Text.Json;
using System.Text.Json.Nodes;
using SystemTextJsonPatch.Operations;
using Xunit;

namespace SystemTextJsonPatch.Tests.IntegrationTests
{
	public class TestOperationIntegrationTests
	{
		[Fact]
		public void EmptyStringTest_Dto()
		{
			var patchDocument = new JsonPatchDocument<TestClass>();
			patchDocument.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			patchDocument.Operations.Add(new Operation<TestClass>(op: "test", path: "/string", from: null, value: ""));
			var node = new TestClass() { String = "" };

			patchDocument.ApplyTo(node);
		}

		[Fact]
		public void NullFieldTest_Dto()
		{
			var patchDocument = new JsonPatchDocument<TestClass>();
			patchDocument.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			patchDocument.Operations.Add(new Operation<TestClass>(op: "test", path: "/string", from: null, value: null));
			var node = new TestClass() { String = null };

			patchDocument.ApplyTo(node);
		}

		[Fact]
		public void EmptyStringTest_Json()
		{
			var patchDocument = new JsonPatchDocument<JsonNode>();
			patchDocument.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			patchDocument.Operations.Add(new Operation<JsonNode>(op: "test", path: "/string", from: null, value: ""));
			var node = JsonNode.Parse("{\"string\": \"\"}")!;
			patchDocument.ApplyTo(node);
		}

		[Fact]
		public void NullFieldTest_Json()
		{
			var patchDocument = new JsonPatchDocument<JsonNode>();
			patchDocument.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			patchDocument.Operations.Add(new Operation<JsonNode>(op: "test", path: "/string", from: null, value: null));
			var node = JsonNode.Parse("{\"string\": null}")!;
			patchDocument.ApplyTo(node);
		}

		class TestClass
		{
			public string? String { get; set; }
		}
	}
}
