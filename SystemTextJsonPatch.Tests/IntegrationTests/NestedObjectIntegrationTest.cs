using System;
using System.Dynamic;
using System.Text.Json;
using SystemTextJsonPatch.Exceptions;
using SystemTextJsonPatch.Operations;
using Xunit;

namespace SystemTextJsonPatch.IntegrationTests;

public class NestedObjectIntegrationTest
{
	[Fact]
	public void ReplaceDtoWithNullCheck()
	{
		// Arrange
		var targetObject = new SimpleObjectWithNestedObjectWithNullCheck()
		{
			SimpleObjectWithNullCheck = new SimpleObjectWithNullCheck()
			{
				StringProperty = "A"
			}
		};

		var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObjectWithNullCheck>();
		patchDocument.Replace(o => o.SimpleObjectWithNullCheck.StringProperty, "B");

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal("B", targetObject.SimpleObjectWithNullCheck.StringProperty);
	}

	[Fact]
	public void ReplaceNestedObjectWithSerialization()
	{
		// Arrange
		var targetObject = new SimpleObjectWithNestedObject()
		{
			IntegerValue = 1
		};
		var options = new JsonSerializerOptions()
		{
		};

		var newNested = new NestedObject() { StringProperty = "B" };
		var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
		patchDocument.Replace(o => o.NestedObject, newNested);
		patchDocument.Options = options;

		var serialized = JsonSerializer.Serialize(patchDocument, options);
		var deserialized = JsonSerializer.Deserialize<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized, options);

		// Act
		deserialized.ApplyTo(targetObject);

		// Assert
		Assert.Equal("B", targetObject.NestedObject.StringProperty);
	}
	
#if NET8_0
	
	[Fact]
	public void SnakeCaseOpMatchesProperty()
	{
		// Arrange
		var targetObject = new SimpleObjectWithNestedObject()
		{
			IntegerValue = 1,
			NestedObject = null
		};
		var options = new JsonSerializerOptions()
		{
			PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
		};

		var newNested = new NestedObject { StringProperty = "B" };
		var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
		patchDocument.Operations.Add(new Operation<SimpleObjectWithNestedObject>("replace", "/nested_object", null, newNested));
		patchDocument.Options = options;

		var serialized = JsonSerializer.Serialize(patchDocument, options);
		var deserialized = JsonSerializer.Deserialize<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized, options);

		// Act
		deserialized.ApplyTo(targetObject);

		// Assert
		Assert.Equal("B", targetObject.NestedObject.StringProperty);
	}

#endif

	[Fact]
	public void CamelCaseOpMatchesExactProperty()
	{
		// Arrange
		var targetObject = new NameCasingTestObject()
		{
			PropertyName = 1, // This is before propertyName and camelCase matches it so it is used.
			propertyName = 1
		};
		var options = new JsonSerializerOptions()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		var patchDocument = new JsonPatchDocument<NameCasingTestObject>();
		patchDocument.Operations.Add(new Operation<NameCasingTestObject>("replace", "/propertyName", null, 2));
		patchDocument.Options = options;

		var serialized = JsonSerializer.Serialize(patchDocument, options);
		var deserialized = JsonSerializer.Deserialize<JsonPatchDocument<NameCasingTestObject>>(serialized, options);

		// Act
		deserialized.ApplyTo(targetObject);

		// Assert
		Assert.Equal(2, targetObject.PropertyName);
		Assert.Equal(1, targetObject.propertyName);
	}

	[Fact]
	public void MatchesExactProperty()
	{
		// Arrange
		var targetObject = new NameCasingTestObject()
		{
			PropertyName = 1,
			propertyName = 1 // exact match
		};
		var options = new JsonSerializerOptions()
		{
		};

		var patchDocument = new JsonPatchDocument<NameCasingTestObject>();
		patchDocument.Operations.Add(new Operation<NameCasingTestObject>("replace", "/propertyName", null, 2));
		patchDocument.Options = options;

		var serialized = JsonSerializer.Serialize(patchDocument, options);
		var deserialized = JsonSerializer.Deserialize<JsonPatchDocument<NameCasingTestObject>>(serialized, options);

		// Act
		deserialized.ApplyTo(targetObject);

		// Assert
		Assert.Equal(1, targetObject.PropertyName);
		Assert.Equal(2, targetObject.propertyName);
	}

	[Fact]
	public void MatchesExactPropertyTestCache()
	{
		CamelCaseOpMatchesExactPropertyAttributeOverride();
		CamelCaseOpMatchesExactProperty();
		MatchesExactProperty();
	}

	[Fact]
	public void CamelCaseOpMatchesExactPropertyAttributeOverride()
	{
		// Arrange
		var targetObject = new AttrNameCasingTestObject()
		{
			PropertyName = 1,
			propertyName = 1
		};
		var options = new JsonSerializerOptions()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		var patchDocument = new JsonPatchDocument<AttrNameCasingTestObject>();
		patchDocument.Operations.Add(new Operation<AttrNameCasingTestObject>("replace", "/propertyName", null, 2));
		patchDocument.Options = options;

		var serialized = JsonSerializer.Serialize(patchDocument, options);
		var deserialized = JsonSerializer.Deserialize<JsonPatchDocument<AttrNameCasingTestObject>>(serialized, options);

		// Act
		deserialized.ApplyTo(targetObject);

		// Assert
		Assert.Equal(2, targetObject.PropertyName); // attribute takes precedence
		Assert.Equal(1, targetObject.propertyName);
	}

	[Fact]
	public void TestStringPropertyInNestedObject()
	{
		// Arrange
		var targetObject = new SimpleObjectWithNestedObject()
		{
			NestedObject = new NestedObject() { StringProperty = "A" }
		};

		var patchDocument = new JsonPatchDocument<NestedObject>();
		patchDocument.Test(o => o.StringProperty, "A");

		// Act
		patchDocument.ApplyTo(targetObject.NestedObject);

		// Assert
		Assert.Equal("A", targetObject.NestedObject.StringProperty);
	}

	[Fact]
	public void TestNestedObject()
	{
		// Arrange
		var targetObject = new SimpleObjectWithNestedObject()
		{
			NestedObject = new NestedObject() { StringProperty = "B" }
		};

		var testNested = new NestedObject() { StringProperty = "B" };
		var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
		patchDocument.Test(o => o.NestedObject, testNested);

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal("B", targetObject.NestedObject.StringProperty);
	}

	[Fact]
	public void AddReplacesExistingStringProperty()
	{
		// Arrange
		var targetObject = new SimpleObjectWithNestedObject()
		{
			SimpleObject = new SimpleObject()
			{
				StringProperty = "A"
			}
		};

		var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
		patchDocument.Add(o => o.SimpleObject.StringProperty, "B");

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal("B", targetObject.SimpleObject.StringProperty);
	}

	[Fact]
	public void AddNewPropertyToExpandoOjectInTypedObject()
	{
		var targetObject = new NestedObject()
		{
			DynamicProperty = new ExpandoObject()
		};

		var patchDocument = new JsonPatchDocument();
		patchDocument.Add("DynamicProperty/NewInt", 1);

		patchDocument.ApplyTo(targetObject);

		Assert.Equal(1, targetObject.DynamicProperty.NewInt);
	}

	[Fact]
	public void RemoveStringProperty()
	{
		// Arrange
		var targetObject = new SimpleObjectWithNestedObject()
		{
			SimpleObject = new SimpleObject()
			{
				StringProperty = "A"
			}
		};

		var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
		patchDocument.Remove(o => o.SimpleObject.StringProperty);

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Null(targetObject.SimpleObject.StringProperty);
	}

	[Fact]
	public void CopyStringPropertyToAnotherStringProperty()
	{
		// Arrange
		var targetObject = new SimpleObjectWithNestedObject()
		{
			SimpleObject = new SimpleObject()
			{
				StringProperty = "A",
				AnotherStringProperty = "B"
			}
		};

		var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
		patchDocument.Copy(o => o.SimpleObject.StringProperty, o => o.SimpleObject.AnotherStringProperty);

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal("A", targetObject.SimpleObject.AnotherStringProperty);
	}

	[Fact]
	public void CopyNullStringPropertyToAnotherStringProperty()
	{
		// Arrange
		var targetObject = new SimpleObjectWithNestedObject()
		{
			SimpleObject = new SimpleObject()
			{
				StringProperty = null,
				AnotherStringProperty = "B"
			}
		};

		var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
		patchDocument.Copy(o => o.SimpleObject.StringProperty, o => o.SimpleObject.AnotherStringProperty);

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Null(targetObject.SimpleObject.AnotherStringProperty);
	}

	[Fact]
	public void CopyDeepClonesObject()
	{
		// Arrange
		var targetObject = new SimpleObjectWithNestedObject()
		{
			SimpleObject = new SimpleObject()
			{
				StringProperty = "A",
				AnotherStringProperty = "B"
			},
			InheritedObject = new InheritedObject()
			{
				StringProperty = "C",
				AnotherStringProperty = "D"
			}
		};

		var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
		patchDocument.Copy(o => o.InheritedObject, o => o.SimpleObject);

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal("C", targetObject.SimpleObject.StringProperty);
		Assert.Equal("D", targetObject.SimpleObject.AnotherStringProperty);
		Assert.Equal("C", targetObject.InheritedObject.StringProperty);
		Assert.Equal("D", targetObject.InheritedObject.AnotherStringProperty);
		Assert.NotSame(targetObject.SimpleObject.StringProperty, targetObject.InheritedObject.StringProperty);
	}

	[Fact]
	public void CopyKeepsObjectType()
	{
		// Arrange
		var targetObject = new SimpleObjectWithNestedObject()
		{
			SimpleObject = new SimpleObject(),
			InheritedObject = new InheritedObject()
		};

		var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
		patchDocument.Copy(o => o.InheritedObject, o => o.SimpleObject);

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal(typeof(InheritedObject), targetObject.SimpleObject.GetType());
	}

	[Fact]
	public void CopyBreaksObjectReference()
	{
		// Arrange
		var targetObject = new SimpleObjectWithNestedObject()
		{
			SimpleObject = new SimpleObject(),
			InheritedObject = new InheritedObject()
		};

		var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
		patchDocument.Copy(o => o.InheritedObject, o => o.SimpleObject);

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.NotSame(targetObject.SimpleObject, targetObject.InheritedObject);
	}

	[Fact]
	public void MoveIntegerValueToAnotherIntegerProperty()
	{
		// Arrange
		var targetObject = new SimpleObjectWithNestedObject()
		{
			SimpleObject = new SimpleObject()
			{
				IntegerValue = 2,
				AnotherIntegerValue = 3
			}
		};

		var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
		patchDocument.Move(o => o.SimpleObject.IntegerValue, o => o.SimpleObject.AnotherIntegerValue);

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal(2, targetObject.SimpleObject.AnotherIntegerValue);
		Assert.Equal(0, targetObject.SimpleObject.IntegerValue);
	}

	[Fact]
	public void MoveKeepsObjectReference()
	{
		// Arrange
		var sDto = new SimpleObject()
		{
			StringProperty = "A",
			AnotherStringProperty = "B"
		};
		var iDto = new InheritedObject()
		{
			StringProperty = "C",
			AnotherStringProperty = "D"
		};
		var targetObject = new SimpleObjectWithNestedObject()
		{
			SimpleObject = sDto,
			InheritedObject = iDto
		};

		var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
		patchDocument.Move(o => o.InheritedObject, o => o.SimpleObject);

		// Act
		patchDocument.ApplyTo(targetObject);

		// Assert
		Assert.Equal("C", targetObject.SimpleObject.StringProperty);
		Assert.Equal("D", targetObject.SimpleObject.AnotherStringProperty);
		Assert.Same(iDto, targetObject.SimpleObject);
		Assert.Null(targetObject.InheritedObject);
	}

	private class SimpleObjectWithNullCheck
	{
		private string _stringProperty;

		public string StringProperty
		{
			get => _stringProperty;

			set
			{
				ExceptionHelper.ThrowIfNull(value, nameof(value));
				_stringProperty = value;
			}
		}
	}

	private class SimpleObjectWithNestedObjectWithNullCheck
	{
		public SimpleObjectWithNullCheck SimpleObjectWithNullCheck { get; set; }

		public SimpleObjectWithNestedObjectWithNullCheck()
		{
			SimpleObjectWithNullCheck = new SimpleObjectWithNullCheck();
		}
	}
}
