﻿using System;
using System.Dynamic;
using System.Text.Json;
using SystemTextJsonPatch.Exceptions;
using SystemTextJsonPatch.Operations;
using Xunit;

namespace SystemTextJsonPatch.IntegrationTests;

public class SimpleObjectIntegrationTest
{
	[Fact]
	public void TestDoubleValueProperty()
	{
		// Arrange
		var targetObject = new SimpleObject()
		{
			DoubleValue = 9.8
		};

		var patchDocument = new JsonPatchDocument();
		patchDocument.Test("DoubleValue", 9.8);

		// Act & Assert
		patchDocument.ApplyTo(targetObject);
	}

	[Fact]
	public void CopyStringPropertyToAnotherStringProperty()
	{
		// Arrange
		var targetObject = new SimpleObject()
		{
			StringProperty = "A",
			AnotherStringProperty = "B"
		};

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
		var targetObject = new SimpleObject()
		{
			StringProperty = null,
			AnotherStringProperty = "B"
		};

		var patchDocument = new JsonPatchDocument();
		patchDocument.Copy("StringProperty", "AnotherStringProperty");

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Null(targetObject.AnotherStringProperty);
	}

	[Fact]
	public void MoveIntegerPropertyToAnotherIntegerProperty()
	{
		// Arrange
		var targetObject = new SimpleObject()
		{
			IntegerValue = 2,
			AnotherIntegerValue = 3
		};

		var patchDocument = new JsonPatchDocument();
		patchDocument.Move("IntegerValue", "AnotherIntegerValue");

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal(2, targetObject.AnotherIntegerValue);
		Assert.Equal(0, targetObject.IntegerValue);
	}

	[Fact]
	public void RemoveDecimalPropertyValue()
	{
		// Arrange
		var targetObject = new SimpleObject()
		{
			DecimalValue = 9.8M
		};

		var patchDocument = new JsonPatchDocument();
		patchDocument.Remove("DecimalValue");

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal(0, targetObject.DecimalValue);
	}

	[Fact]
	public void ReplaceGuid()
	{
		// Arrange
		var targetObject = new SimpleObject()
		{
			GuidValue = Guid.NewGuid()
		};

		var newGuid = Guid.NewGuid();
		var patchDocument = new JsonPatchDocument();
		patchDocument.Replace("GuidValue", newGuid);

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal(newGuid, targetObject.GuidValue);
	}

	[Fact]
	public void AddReplacesGuid()
	{
		// Arrange
		var targetObject = new SimpleObject()
		{
			GuidValue = Guid.NewGuid()
		};

		var newGuid = Guid.NewGuid();
		var patchDocument = new JsonPatchDocument();
		patchDocument.Add("GuidValue", newGuid);

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal(newGuid, targetObject.GuidValue);
	}

	// https://github.com/dotnet/aspnetcore/issues/3634
	[Fact]
	public void RegressionAspNetCore3634()
	{
		// Assert
		var document = new JsonPatchDocument();
		document.Move("/Object", "/Object/goodbye");

		dynamic @object = new ExpandoObject();
		@object.hello = "world";

		var target = new RegressionAspNetCore3634Object();
		target.Object = @object;

		// Act
		var ex = Assert.Throws<JsonPatchException>(() => document.ApplyTo(target));

		// Assert
		Assert.Equal("For operation 'move', the target location specified by path '/Object/goodbye' was not found.", ex.Message);
	}

	[Fact]
	public void ShouldFollowCaseInsensitiveSettingOfSystemTextJsonOptions()
	{
		var options = new JsonSerializerOptions()
		{
			PropertyNameCaseInsensitive = true
		};

		// Arrange
		var targetObject = new SimpleObject()
		{
			StringProperty = "old",
		};

		var patchDocument = new JsonPatchDocument<SimpleObject>();
		patchDocument.Operations.Add(new Operation<SimpleObject>("replace", "/stringproperty", null, "test"));
		patchDocument.Options = options;

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal("test", targetObject.StringProperty);
	}

	[Fact]
	public void FailsWhenPropertyCouldNotBeFoundBecauseOfCasing()
	{
		var options = new JsonSerializerOptions()
		{
			PropertyNameCaseInsensitive = false
		};

		// Arrange
		var targetObject = new SimpleObject()
		{
			AnotherStringProperty = "old",
		};

		var patchDocument = new JsonPatchDocument<SimpleObject>();
		patchDocument.Operations.Add(new Operation<SimpleObject>("replace", "/anotherstringproperty", null, "test"));
		patchDocument.Options = options;

		// Act
		var excpetion = Assert.Throws<JsonPatchException>(() => patchDocument.ApplyTo(targetObject));
		Assert.Equal("The target location specified by path segment 'anotherstringproperty' was not found.", excpetion.Message);
	}

	private class RegressionAspNetCore3634Object
	{
		public dynamic Object { get; set; }
	}
}
