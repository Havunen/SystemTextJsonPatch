using System;
using System.Collections.Generic;
using Xunit;

namespace SystemTextJsonPatch;

public class JsonPatchDocumentGetPathTest
{
	[Fact]
	public void ExpressionTypeMemberAccess()
	{
		// Arrange
		var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();

		// Act
		var path = patchDocument.GetPath(p => p.SimpleObject.IntegerList, "-");

		// Assert
		Assert.Equal("/SimpleObject/IntegerList/-", path);
	}

	[Fact]
	public void ExpressionTypeArrayIndex()
	{
		// Arrange
		var patchDocument = new JsonPatchDocument<int[]>();

		// Act
		var path = patchDocument.GetPath(p => p[3], null);

		// Assert
		Assert.Equal("/3", path);
	}

	[Fact]
	public void ExpressionTypeCall()
	{
		// Arrange
		var patchDocument = new JsonPatchDocument<Dictionary<string, int>>();

		// Act
		var path = patchDocument.GetPath(p => p["key"], "3");

		// Assert
		Assert.Equal("/key/3", path);
	}

	[Fact]
	public void ExpressionTypeParameterNullPosition()
	{
		// Arrange
		var patchDocument = new JsonPatchDocument<SimpleObject>();

		// Act
		var path = patchDocument.GetPath(p => p, null);

		// Assert
		Assert.Equal("/", path);
	}

	[Fact]
	public void ExpressionTypeParameterWithPosition()
	{
		// Arrange
		var patchDocument = new JsonPatchDocument<SimpleObject>();

		// Act
		var path = patchDocument.GetPath(p => p, "-");

		// Assert
		Assert.Equal("/-", path);
	}

	[Fact]
	public void ExpressionTypeConvert()
	{
		// Arrange
		var patchDocument = new JsonPatchDocument<NestedObjectWithDerivedClass>();

		// Act
		var path = patchDocument.GetPath(p => (BaseClass)p.DerivedObject, null);

		// Assert
		Assert.Equal("/DerivedObject", path);
	}

	[Fact]
	public void ExpressionTypeNotSupported()
	{
		// Arrange
		var patchDocument = new JsonPatchDocument<SimpleObject>();

		// Act
		var exception = Assert.Throws<InvalidOperationException>(() =>
		{
			patchDocument.GetPath(p => p.IntegerValue >= 4, null);
		});

		// Assert
		Assert.Equal("The expression '(p.IntegerValue >= 4)' is not supported. Supported expressions include member access and indexer expressions.", exception.Message);
	}
}

internal class DerivedClass : BaseClass
{
	public DerivedClass()
	{
	}
}

internal class NestedObjectWithDerivedClass
{
	public DerivedClass DerivedObject { get; set; }
}

internal class BaseClass
{
}
