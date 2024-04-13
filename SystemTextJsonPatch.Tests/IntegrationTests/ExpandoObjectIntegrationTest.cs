using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using SystemTextJsonPatch.Exceptions;
using Xunit;

namespace SystemTextJsonPatch.IntegrationTests;

public class ExpandoObjectIntegrationTest
{
	[Fact]
	public void AddNewIntProperty()
	{
		// Arrange
		dynamic targetObject = new ExpandoObject();
		targetObject.Test = 1;

		var patchDocument = new JsonPatchDocument();
		patchDocument.Add("NewInt", 1);

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal(1, targetObject.NewInt);
		Assert.Equal(1, targetObject.Test);
	}

	[Fact]
	public void AddNewPropertyToTypedObjectInExpandoObject()
	{
		// Arrange
		dynamic dynamicProperty = new ExpandoObject();
		dynamicProperty.StringProperty = "A";

		var targetObject = new NestedObject()
		{
			DynamicProperty = dynamicProperty
		};

		var patchDocument = new JsonPatchDocument();
		patchDocument.Add("DynamicProperty/StringProperty", "B");

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal("B", targetObject.DynamicProperty.StringProperty);
	}

	[Fact]
	public void AddReplacesExistingProperty()
	{
		// Arrange
		dynamic targetObject = new ExpandoObject();
		targetObject.StringProperty = "A";

		var patchDocument = new JsonPatchDocument();
		patchDocument.Add("StringProperty", "B");

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal("B", targetObject.StringProperty);
	}

	[Fact]
	public void AddReplacesExistingPropertyInNestedExpandoObject()
	{
		// Arrange
		dynamic targetObject = new ExpandoObject();
		targetObject.InBetweenFirst = new ExpandoObject();
		targetObject.InBetweenFirst.InBetweenSecond = new ExpandoObject();
		targetObject.InBetweenFirst.InBetweenSecond.StringProperty = "A";

		var patchDocument = new JsonPatchDocument();
		patchDocument.Add("/InBetweenFirst/InBetweenSecond/StringProperty", "B");

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal("B", targetObject.InBetweenFirst.InBetweenSecond.StringProperty);
	}

	[Fact]
	public void ShouldNotReplacePropertyWithDifferentCase()
	{
		// Arrange
		dynamic targetObject = new ExpandoObject();
		targetObject.StringProperty = "A";

		var patchDocument = new JsonPatchDocument();
		patchDocument.Add("stringproperty", "B");

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal("A", targetObject.StringProperty);
		Assert.Equal("B", targetObject.stringproperty);
	}

	[Fact]
	public void TestIntegerPropertyIsSuccessful()
	{
		// Arrange
		dynamic targetObject = new ExpandoObject();
		targetObject.Test = 1;

		var patchDocument = new JsonPatchDocument();
		patchDocument.Test("Test", 1);

		// Act & Assert
		patchDocument.ApplyTo(targetObject);
	}

	[Fact]
	public void TestEmptyPropertyIsSuccessful()
	{
		// Arrange
		dynamic targetObject = new ExpandoObject();
		targetObject.Test = "";

		var patchDocument = new JsonPatchDocument();
		patchDocument.Test("Test", "");

		// Act & Assert
		patchDocument.ApplyTo(targetObject);
	}

	[Fact]
	public void TestValueAgainstEmptyPropertyThrowsJsonPatchExceptionIsSuccessful()
	{
		// Arrange
		dynamic targetObject = new ExpandoObject();
		targetObject.Test = "";

		var patchDocument = new JsonPatchDocument();
		patchDocument.Test("Test", "TestValue");

		// Act
		var exception = Assert.Throws<JsonPatchTestOperationException>(() => { patchDocument.ApplyTo(targetObject); });

		// Assert
		Assert.Equal("The current value '' at path 'Test' is not equal to the test value 'TestValue'.", exception.Message);
	}

	[Fact]
	public void TestStringPropertyThrowsJsonPatchExceptionIfTestFails()
	{
		// Arrange
		dynamic targetObject = new ExpandoObject();
		targetObject.Test = "Value";

		var patchDocument = new JsonPatchDocument();
		patchDocument.Test("Test", "TestValue");

		// Act
		var exception = Assert.Throws<JsonPatchTestOperationException>(() => { patchDocument.ApplyTo(targetObject); });

		// Assert
		Assert.Equal("The current value 'Value' at path 'Test' is not equal to the test value 'TestValue'.", exception.Message);
	}

	[Fact]
	public void CopyStringPropertyToAnotherStringProperty()
	{
		// Arrange
		dynamic targetObject = new ExpandoObject();

		targetObject.StringProperty = "A";
		targetObject.AnotherStringProperty = "B";

		var patchDocument = new JsonPatchDocument();
		patchDocument.Copy("StringProperty", "AnotherStringProperty");

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal("A", targetObject.AnotherStringProperty);
	}

	[Fact]
	public void CopyNullStringPropertyToAnotherStringProperty()
	{
		// Arrange
		dynamic targetObject = new ExpandoObject();

		targetObject.StringProperty = null;
		targetObject.AnotherStringProperty = "B";

		var patchDocument = new JsonPatchDocument();
		patchDocument.Copy("StringProperty", "AnotherStringProperty");

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.True(((IDictionary<string, object>)targetObject).ContainsKey("AnotherStringProperty"));
		Assert.Null(targetObject.AnotherStringProperty);
	}

	[Fact]
	public void MoveIntegerValueToAnotherIntegerProperty()
	{
		// Arrange
		dynamic targetObject = new ExpandoObject();
		targetObject.IntegerValue = 100;
		targetObject.AnotherIntegerValue = 200;

		var patchDocument = new JsonPatchDocument();
		patchDocument.Move("IntegerValue", "AnotherIntegerValue");

		// Act
		patchDocument.ApplyTo(targetObject);

		Assert.Equal(100, targetObject.AnotherIntegerValue);

		var cont = targetObject as IDictionary<string, object>;
		cont.TryGetValue("IntegerValue", out var valueFromDictionary);

		// Assert
		Assert.Null(valueFromDictionary);
	}

	[Fact]
	public void MoveToNonExistingProperty()
	{
		// Arrange
		dynamic targetObject = new ExpandoObject();
		targetObject.StringProperty = "A";

		var patchDocument = new JsonPatchDocument();
		patchDocument.Move("StringProperty", "AnotherStringProperty");

		// Act
		patchDocument.ApplyTo(targetObject);

		Assert.Equal("A", targetObject.AnotherStringProperty);

		var cont = targetObject as IDictionary<string, object>;
		cont.TryGetValue("StringProperty", out var valueFromDictionary);

		// Assert
		Assert.Null(valueFromDictionary);
	}

	//[Fact]
	//public void RemoveProperty_ShouldFail_IfItDoesntExist()
	//{
	//    // Arrange
	//    dynamic targetObject = new ExpandoObject();
	//    targetObject.Test = 1;

	//    var patchDocument = new JsonPatchDocument();
	//    patchDocument.Remove("NonExisting");

	//    // Act
	//    var exception = Assert.Throws<JsonPatchException>(() =>
	//    {
	//        patchDocument.ApplyTo(targetObject);
	//    });

	//    // Assert
	//    Assert.Equal("The target location specified by path segment 'NonExisting' was not found.", exception.Message);
	//}

	[Fact]
	public void RemoveStringProperty()
	{
		// Arrange
		dynamic targetObject = new ExpandoObject();
		targetObject.Test = 1;

		var patchDocument = new JsonPatchDocument();
		patchDocument.Remove("Test");

		// Act
		patchDocument.ApplyTo(targetObject);

		var cont = targetObject as IDictionary<string, object>;
		cont.TryGetValue("Test", out var valueFromDictionary);

		// Assert
		Assert.Null(valueFromDictionary);
	}

	//[Fact]
	//public void RemoveProperty_MixedCase_ThrowsPathNotFoundException()
	//{
	//    // Arrange
	//    dynamic targetObject = new ExpandoObject();
	//    targetObject.Test = 1;

	//    var patchDocument = new JsonPatchDocument();
	//    patchDocument.Remove("test");

	//    // Act
	//    var t = patchDocument.ApplyTo(targetObject);

	//    // Assert
	//    //Assert.Equal("The target location specified by path segment 'test' was not found.", exception.Message);
	//}

	[Fact]
	public void RemoveNestedProperty()
	{
		// Arrange
		dynamic targetObject = new ExpandoObject();
		targetObject.Test = new ExpandoObject();
		targetObject.Test.AnotherTest = "A";

		var patchDocument = new JsonPatchDocument();
		patchDocument.Remove("Test");

		// Act
		patchDocument.ApplyTo(targetObject);

		var cont = targetObject as IDictionary<string, object>;
		cont.TryGetValue("Test", out var valueFromDictionary);

		// Assert
		Assert.Null(valueFromDictionary);
	}

	//[Fact]
	//public void RemoveNestedPropertyMixedCaseThrowsPathNotFoundException()
	//{
	//	// Arrange
	//	dynamic targetObject = new ExpandoObject();
	//	targetObject.Test = new ExpandoObject();
	//	targetObject.Test.AnotherTest = "A";

	//	var patchDocument = new JsonPatchDocument();
	//	patchDocument.Remove("test");

	//	// Act
	//	// TODO: Different exception type
	//	var exception = Assert.Throws<RuntimeBinderException>(() =>
	//	{
	//		var result = patchDocument.ApplyTo(targetObject);
	//	});

	//	// Assert
	//	//Assert.Equal("The target location specified by path segment 'test' was not found.", exception.Message);
	//}

	[Fact]
	public void ReplaceGuid()
	{
		// Arrange
		dynamic targetObject = new ExpandoObject();
		targetObject.GuidValue = Guid.NewGuid();

		var newGuid = Guid.NewGuid();
		var patchDocument = new JsonPatchDocument();
		patchDocument.Replace("GuidValue", newGuid);

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal(newGuid, targetObject.GuidValue);
	}
}
