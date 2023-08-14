using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using SystemTextJsonPatch.Internal;
using Xunit;

namespace SystemTextJsonPatch;

public class CustomNamingStrategyTests
{
	[Fact]
	public void AddPropertyToDynamicTestObjectWithCustomNamingStrategy()
	{
		// Arrange
		var options = new JsonSerializerOptions()
		{
			PropertyNamingPolicy = new TestNamingStrategy()
		};

		dynamic targetObject = new DynamicTestObject();
		targetObject.Test = 1;

		var patchDocument = new JsonPatchDocument();
		patchDocument.Add("NewInt", 1);
		patchDocument.Options = options;

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal(1, targetObject.customNewInt);
		Assert.Equal(1, targetObject.Test);
	}

	[Fact]
	public void CopyPropertyValueToDynamicTestObjectWithCustomNamingStrategy()
	{
		// Arrange
		var options = new JsonSerializerOptions
		{
			PropertyNamingPolicy = new TestNamingStrategy()
		};

		dynamic targetObject = new DynamicTestObject();
		targetObject.customStringProperty = "A";
		targetObject.customAnotherStringProperty = "B";

		var patchDocument = new JsonPatchDocument()
		{
			Options = options
		};
		patchDocument.Copy("StringProperty", "AnotherStringProperty");

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal("A", targetObject.customAnotherStringProperty);
	}

	[Fact]
	public void MovePropertyValueForExpandoObjectWithCustomNamingStrategy()
	{
		// Arrange
		var options = new JsonSerializerOptions
		{
			PropertyNamingPolicy = new TestNamingStrategy()
		};

		dynamic targetObject = new ExpandoObject();
		targetObject.customStringProperty = "A";
		targetObject.customAnotherStringProperty = "B";

		var patchDocument = new JsonPatchDocument();
		patchDocument.Move("StringProperty", "AnotherStringProperty");
		patchDocument.Options = options;

		// Act
		patchDocument.ApplyTo(targetObject);
		var cont = targetObject as IDictionary<string, object>;
		cont.TryGetValue("customStringProperty", out var valueFromDictionary);

		// Assert
		Assert.Equal("A", targetObject.customAnotherStringProperty);
		Assert.Null(valueFromDictionary);
	}

	[Fact]
	public void RemovePropertyDynamicTestObjectWithCustomNamingStrategy()
	{
		// Arrange
		var options = new JsonSerializerOptions
		{
			PropertyNamingPolicy = new TestNamingStrategy()
		};

		dynamic targetObject = new DynamicTestObject();
		targetObject.customTest = "A";

		var patchDocument = new JsonPatchDocument();
		patchDocument.Remove("Test");
		patchDocument.Options = options;

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Null(targetObject.customTest);
	}

	[Fact]
	public void RemovePropertyFromDictionaryObjectWithCustomNamingStrategy()
	{
		// Arrange
		var options = new JsonSerializerOptions
		{
			PropertyNamingPolicy = new TestNamingStrategy()
		};

		var targetObject = new Dictionary<string, int>()
		{
			{ "customTest", 1 },
		};

		var patchDocument = new JsonPatchDocument();
		patchDocument.Remove("Test");
		patchDocument.Options = options;

		// Act
		patchDocument.ApplyTo(targetObject);
		var cont = targetObject as IDictionary<string, int>;
		cont.TryGetValue("customTest", out var valueFromDictionary);

		// Assert
		Assert.Equal(0, valueFromDictionary);
	}

	[Fact]
	public void ReplacePropertyValueForExpandoObjectWithCustomNamingStrategy()
	{
		// Arrange
		var options = new JsonSerializerOptions
		{
			PropertyNamingPolicy = new TestNamingStrategy()
		};

		dynamic targetObject = new ExpandoObject();
		targetObject.customTest = 1;

		var patchDocument = new JsonPatchDocument();
		patchDocument.Replace("Test", 2);
		patchDocument.Options = options;

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal(2, targetObject.customTest);
	}

	[Fact]
	public void PatchDocumentOfTPathPropertyShouldFollowNamingPolicy()
	{
		var serializerOptions = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		var patch = new JsonPatchDocument<SimpleObject>(new(), serializerOptions);
		patch.Replace((v) => v.StringProperty, "Bob");

		var patchStr = JsonSerializer.Serialize(patch);

		Assert.Equal("[{\"op\":\"replace\",\"path\":\"/stringProperty\",\"value\":\"Bob\"}]", patchStr);
	}

	[Fact]
	public void PatchDocumentOfTPathPropertyShouldFollowNamingPolicyNested()
	{
		var serializerOptions = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		var patch = new JsonPatchDocument<SimpleObjectWithNestedObject>(new(), serializerOptions);
		patch.Replace((v) => v.NestedObject.StringProperty, "Bob");

		var patchStr = JsonSerializer.Serialize(patch);

		Assert.Equal("[{\"op\":\"replace\",\"path\":\"/nestedObject/stringProperty\",\"value\":\"Bob\"}]", patchStr);
	}

	[Fact]
	public void PatchDocumentOfTPathPropertyShouldNotChangeWhenNoOptionsDefined()
	{
		var serializerOptions = new JsonSerializerOptions();

		var patch = new JsonPatchDocument<SimpleObject>(new(), serializerOptions);
		patch.Replace((v) => v.StringProperty, "Bob");

		var patchStr = JsonSerializer.Serialize(patch);

		Assert.Equal("[{\"op\":\"replace\",\"path\":\"/StringProperty\",\"value\":\"Bob\"}]", patchStr);
	}

	private class TestNamingStrategy : JsonNamingPolicy
	{
		public override string ConvertName(string name)
		{
			return "custom" + name;
		}
	}
}
