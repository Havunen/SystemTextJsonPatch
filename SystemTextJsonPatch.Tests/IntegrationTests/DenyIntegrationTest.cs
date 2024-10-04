using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using SystemTextJsonPatch.Exceptions;
using Xunit;

namespace SystemTextJsonPatch.IntegrationTests;

public class DenyIntegrationTest
{
	[Fact]
	public void TestInList()
	{
		// Arrange
		var targetObject = new SimpleObjectWithNestedObjectAndDeny()
		{
			SimpleObject = new SimpleObject()
			{
				IntegerList = new List<int>() { 1, 2, 3 }
			}
		};

		var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObjectAndDeny>();
		patchDocument.Test(o => o.SimpleObject.IntegerList, 3, 2);

		// Act & Assert
		var exception = Assert.Throws<JsonPatchAccessDeniedException>(() => patchDocument.ApplyTo(targetObject));
		Assert.Equal(nameof(SimpleObjectWithNestedObjectAndDeny.SimpleObject), exception.Property);
		Assert.Equal(nameof(SimpleObjectWithNestedObjectAndDeny), exception.Type);
	}

	[Fact]
	public void AddToComplextTypeListSpecifyIndex()
	{
		// Arrange
		var targetObject = new SimpleObjectWithNestedObjectAndDeny()
		{
			SimpleObjectList = new List<SimpleObject>()
			{
				new SimpleObject
				{
					StringProperty = "String1"
				},
				new SimpleObject
				{
					StringProperty = "String2"
				}
			}
		};

		var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObjectAndDeny>();
		patchDocument.Add(o => o.SimpleObjectList[0].StringProperty, "ChangedString1");

		// Act & Assert
		var exception = Assert.Throws<JsonPatchAccessDeniedException>(() => patchDocument.ApplyTo(targetObject));
		Assert.Equal(nameof(SimpleObjectWithNestedObjectAndDeny.SimpleObjectList), exception.Property);
		Assert.Equal(nameof(SimpleObjectWithNestedObjectAndDeny), exception.Type);
	}

	[Fact]
	public void RemoveFromList()
	{
		// Arrange
		var targetObject = new SimpleObjectWithDeny()
		{
			IntegerList = new List<int>() { 1, 2, 3 }
		};

		var patchDocument = new JsonPatchDocument();
		patchDocument.Remove("IntegerList/2");

		// Act & Assert
		var exception = Assert.Throws<JsonPatchAccessDeniedException>(() => patchDocument.ApplyTo(targetObject));
		Assert.Equal(nameof(SimpleObjectWithDeny.IntegerList), exception.Property);
		Assert.Equal(nameof(SimpleObjectWithDeny), exception.Type);
	}


	[Fact]
	public void InnerObjectDeny()
	{
		// Arrange
		var targetObject = new SimpleObjectWithNestedObjectAndDeny();

		var patchDocument = new JsonPatchDocument();
		patchDocument.Add("Allow/IntegerValue", 1);

		// Act & Assert
		var exception = Assert.Throws<JsonPatchAccessDeniedException>(() => patchDocument.ApplyTo(targetObject));
		Assert.Equal(nameof(SimpleObjectWithDeny.IntegerValue), exception.Property);
		Assert.Equal(nameof(SimpleObjectWithDeny), exception.Type);
	}

}
