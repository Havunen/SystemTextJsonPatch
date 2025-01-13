using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using SystemTextJsonPatch.Exceptions;
using SystemTextJsonPatch.Operations;
using Xunit;

namespace SystemTextJsonPatch.IntegrationTests;

public class DictionaryTest
{
	[Fact]
	public void TestIntegerValueIsSuccessful()
	{
		// Arrange
		var model = new IntDictionary();
		model.DictionaryOfStringToInteger["one"] = 1;
		model.DictionaryOfStringToInteger["two"] = 2;
		var patchDocument = new JsonPatchDocument();
		patchDocument.Test("/DictionaryOfStringToInteger/two", 2);

		// Act & Assert
		patchDocument.ApplyTo(model);
	}

	[Fact]
	public void AddIntegerValueSucceeds()
	{
		// Arrange
		var model = new IntDictionary();
		model.DictionaryOfStringToInteger["one"] = 1;
		model.DictionaryOfStringToInteger["two"] = 2;
		var patchDocument = new JsonPatchDocument();
		patchDocument.Add("/DictionaryOfStringToInteger/three", 3);

		// Act
		patchDocument.ApplyTo(model);

		// Assert
		Assert.Equal(3, model.DictionaryOfStringToInteger.Count);
		Assert.Equal(1, model.DictionaryOfStringToInteger["one"]);
		Assert.Equal(2, model.DictionaryOfStringToInteger["two"]);
		Assert.Equal(3, model.DictionaryOfStringToInteger["three"]);
	}

	[Fact]
	public void RemoveIntegerValueSucceeds()
	{
		// Arrange
		var model = new IntDictionary();
		model.DictionaryOfStringToInteger["one"] = 1;
		model.DictionaryOfStringToInteger["two"] = 2;
		var patchDocument = new JsonPatchDocument();
		patchDocument.Remove("/DictionaryOfStringToInteger/two");

		// Act
		patchDocument.ApplyTo(model);

		// Assert
		Assert.Equal(1, model.DictionaryOfStringToInteger.Count);
		Assert.Equal(1, model.DictionaryOfStringToInteger["one"]);
	}

	[Fact]
	public void MoveIntegerValueSucceeds()
	{
		// Arrange
		var model = new IntDictionary();
		model.DictionaryOfStringToInteger["one"] = 1;
		model.DictionaryOfStringToInteger["two"] = 2;
		var patchDocument = new JsonPatchDocument();
		patchDocument.Move("/DictionaryOfStringToInteger/one", "/DictionaryOfStringToInteger/two");

		// Act
		patchDocument.ApplyTo(model);

		// Assert
		Assert.Equal(1, model.DictionaryOfStringToInteger.Count);
		Assert.Equal(1, model.DictionaryOfStringToInteger["two"]);
	}

	[Fact]
	public void ReplaceIntegerValueSucceeds()
	{
		// Arrange
		var model = new IntDictionary();
		model.DictionaryOfStringToInteger["one"] = 1;
		model.DictionaryOfStringToInteger["two"] = 2;
		var patchDocument = new JsonPatchDocument();
		patchDocument.Replace("/DictionaryOfStringToInteger/two", 20);

		// Act
		patchDocument.ApplyTo(model);

		// Assert
		Assert.Equal(2, model.DictionaryOfStringToInteger.Count);
		Assert.Equal(1, model.DictionaryOfStringToInteger["one"]);
		Assert.Equal(20, model.DictionaryOfStringToInteger["two"]);
	}

	[Fact]
	public void CopyIntegerValueSucceeds()
	{
		// Arrange
		var model = new IntDictionary();
		model.DictionaryOfStringToInteger["one"] = 1;
		model.DictionaryOfStringToInteger["two"] = 2;
		var patchDocument = new JsonPatchDocument();
		patchDocument.Copy("/DictionaryOfStringToInteger/one", "/DictionaryOfStringToInteger/two");

		// Act
		patchDocument.ApplyTo(model);

		// Assert
		Assert.Equal(2, model.DictionaryOfStringToInteger.Count);
		Assert.Equal(1, model.DictionaryOfStringToInteger["one"]);
		Assert.Equal(1, model.DictionaryOfStringToInteger["two"]);
	}

	private class Customer
	{
		public string Name { get; set; }
		public Address Address { get; set; }
	}

	private class Address
	{
		public string City { get; set; }
	}

	private class IntDictionary
	{
		public IDictionary<string, int> DictionaryOfStringToInteger { get; } = new Dictionary<string, int>();
		public IDictionary NonGenericDictionary { get; } = new NonGenericDictionary();
	}

	private class CustomerDictionary
	{
		public IDictionary<int, Customer> DictionaryOfIntegerToCustomer { get; } = new Dictionary<int, Customer>();
		public IDictionary NonGenericDictionary { get; } = new NonGenericDictionary();
	}

#if NET7_0_OR_GREATER
	[JsonDerivedType(typeof(Dog), "dog")]
	[JsonDerivedType(typeof(Cat), "cat")]
	private abstract class Animal
	{
	}

	private class Dog : Animal
	{
	}

	private class Cat : Animal
	{
	}

	private class AnimalDictionary
	{
		public IDictionary<int, Animal> DictionaryOfIntToAnimal { get; } = new Dictionary<int, Animal>();
	}
#endif

	[Fact]
	public void TestPocoObjectSucceeds()
	{
		// Arrange
		var key1 = 100;
		var value1 = new Customer() { Name = "James" };
		var model = new CustomerDictionary();
		model.DictionaryOfIntegerToCustomer[key1] = value1;
		var patchDocument = new JsonPatchDocument();
		patchDocument.Test($"/DictionaryOfIntegerToCustomer/{key1}/Name", "James");

		// Act & Assert
		patchDocument.ApplyTo(model);
	}

	[Fact]
	public void TestPocoObjectFailsWhenTestValueIsNotEqualToObjectValue()
	{
		// Arrange
		var key1 = 100;
		var value1 = new Customer() { Name = "James" };
		var model = new CustomerDictionary();
		model.DictionaryOfIntegerToCustomer[key1] = value1;
		var patchDocument = new JsonPatchDocument();
		patchDocument.Test($"/DictionaryOfIntegerToCustomer/{key1}/Name", "Mike");

		// Act
		var exception = Assert.Throws<JsonPatchTestOperationException>(() => { patchDocument.ApplyTo(model); });

		// Assert
		Assert.Equal("The current value 'James' at path 'Name' is not equal to the test value 'Mike'.", exception.Message);
	}

	[Fact]
	public void AddPocoObjectSucceeds()
	{
		// Arrange
		var key1 = 100;
		var value1 = new Customer() { Name = "James" };
		var key2 = 200;
		var value2 = new Customer() { Name = "Mike" };
		var model = new CustomerDictionary();
		model.DictionaryOfIntegerToCustomer[key1] = value1;
		var patchDocument = new JsonPatchDocument();
		patchDocument.Add($"/DictionaryOfIntegerToCustomer/{key2}", value2);

		// Act
		patchDocument.ApplyTo(model);

		// Assert
		Assert.Equal(2, model.DictionaryOfIntegerToCustomer.Count);
		var actualValue1 = model.DictionaryOfIntegerToCustomer[key1];
		Assert.NotNull(actualValue1);
		Assert.Equal("James", actualValue1.Name);
		var actualValue2 = model.DictionaryOfIntegerToCustomer[key2];
		Assert.NotNull(actualValue2);
		Assert.Equal("Mike", actualValue2.Name);
	}

	[Fact]
	public void AddReplacesPocoObjectSucceeds()
	{
		// Arrange
		var key1 = 100;
		var value1 = new Customer() { Name = "Jamesss" };
		var key2 = 200;
		var value2 = new Customer() { Name = "Mike" };
		var model = new CustomerDictionary();
		model.DictionaryOfIntegerToCustomer[key1] = value1;
		model.DictionaryOfIntegerToCustomer[key2] = value2;
		var patchDocument = new JsonPatchDocument();
		patchDocument.Add($"/DictionaryOfIntegerToCustomer/{key1}/Name", "James");

		// Act
		patchDocument.ApplyTo(model);

		// Assert
		Assert.Equal(2, model.DictionaryOfIntegerToCustomer.Count);
		var actualValue1 = model.DictionaryOfIntegerToCustomer[key1];
		Assert.NotNull(actualValue1);
		Assert.Equal("James", actualValue1.Name);
	}

#if NET7_0_OR_GREATER
	[Fact]
	public void AddReplacesPocoObjectWithDifferentTypeSucceeds()
	{
		// Arrange
		var key1 = 100;
		var value1 = new Cat();
		var model = new AnimalDictionary();
		model.DictionaryOfIntToAnimal[key1] = value1;
		var patchDocument = new JsonPatchDocument();
		patchDocument.Add($"/DictionaryOfIntToAnimal/{key1}", new Dog());

		// Act
		patchDocument.ApplyTo(model);

		// Assert
		var actualValue1 = Assert.Single(model.DictionaryOfIntToAnimal).Value;
		Assert.IsType<Dog>(actualValue1);
	}
#endif

	[Fact]
	public void RemovePocoObjectSucceeds()
	{
		// Arrange
		var key1 = 100;
		var value1 = new Customer() { Name = "Jamesss" };
		var key2 = 200;
		var value2 = new Customer() { Name = "Mike" };
		var model = new CustomerDictionary();
		model.DictionaryOfIntegerToCustomer[key1] = value1;
		model.DictionaryOfIntegerToCustomer[key2] = value2;
		var patchDocument = new JsonPatchDocument();
		patchDocument.Remove($"/DictionaryOfIntegerToCustomer/{key1}/Name");

		// Act
		patchDocument.ApplyTo(model);

		// Assert
		var actualValue1 = model.DictionaryOfIntegerToCustomer[key1];
		Assert.Null(actualValue1.Name);
	}

	[Fact]
	public void MovePocoObjectSucceeds()
	{
		// Arrange
		var key1 = 100;
		var value1 = new Customer() { Name = "James" };
		var key2 = 200;
		var value2 = new Customer() { Name = "Mike" };
		var model = new CustomerDictionary();
		model.DictionaryOfIntegerToCustomer[key1] = value1;
		model.DictionaryOfIntegerToCustomer[key2] = value2;
		var patchDocument = new JsonPatchDocument();
		patchDocument.Move($"/DictionaryOfIntegerToCustomer/{key1}/Name", $"/DictionaryOfIntegerToCustomer/{key2}/Name");

		// Act
		patchDocument.ApplyTo(model);

		// Assert
		var actualValue2 = model.DictionaryOfIntegerToCustomer[key2];
		Assert.NotNull(actualValue2);
		Assert.Equal("James", actualValue2.Name);
	}

	[Fact]
	public void CopyPocoObjectSucceeds()
	{
		// Arrange
		var key1 = 100;
		var value1 = new Customer() { Name = "James" };
		var key2 = 200;
		var value2 = new Customer() { Name = "Mike" };
		var model = new CustomerDictionary();
		model.DictionaryOfIntegerToCustomer[key1] = value1;
		model.DictionaryOfIntegerToCustomer[key2] = value2;
		var patchDocument = new JsonPatchDocument();
		patchDocument.Copy($"/DictionaryOfIntegerToCustomer/{key1}/Name", $"/DictionaryOfIntegerToCustomer/{key2}/Name");

		// Act
		patchDocument.ApplyTo(model);

		// Assert
		Assert.Equal(2, model.DictionaryOfIntegerToCustomer.Count);
		var actualValue2 = model.DictionaryOfIntegerToCustomer[key2];
		Assert.NotNull(actualValue2);
		Assert.Equal("James", actualValue2.Name);
	}

	[Fact]
	public void ReplacePocoObjectSucceeds()
	{
		// Arrange
		var key1 = 100;
		var value1 = new Customer() { Name = "Jamesss" };
		var key2 = 200;
		var value2 = new Customer() { Name = "Mike" };
		var model = new CustomerDictionary();
		model.DictionaryOfIntegerToCustomer[key1] = value1;
		model.DictionaryOfIntegerToCustomer[key2] = value2;
		var patchDocument = new JsonPatchDocument();
		patchDocument.Replace($"/DictionaryOfIntegerToCustomer/{key1}/Name", "James");

		// Act
		patchDocument.ApplyTo(model);

		// Assert
		Assert.Equal(2, model.DictionaryOfIntegerToCustomer.Count);
		var actualValue1 = model.DictionaryOfIntegerToCustomer[key1];
		Assert.NotNull(actualValue1);
		Assert.Equal("James", actualValue1.Name);
	}

	[Fact]
	public void ReplacePocoObjectWithEscapingSucceeds()
	{
		// Arrange
		var key1 = "Foo/Name";
		var value1 = 100;
		var key2 = "Foo";
		var value2 = 200;
		var model = new IntDictionary();
		model.DictionaryOfStringToInteger[key1] = value1;
		model.DictionaryOfStringToInteger[key2] = value2;
		var patchDocument = new JsonPatchDocument();
		patchDocument.Replace($"/DictionaryOfStringToInteger/Foo~1Name", 300);

		// Act
		patchDocument.ApplyTo(model);

		// Assert
		Assert.Equal(2, model.DictionaryOfStringToInteger.Count);
		var actualValue1 = model.DictionaryOfStringToInteger[key1];
		var actualValue2 = model.DictionaryOfStringToInteger[key2];
		Assert.Equal(300, actualValue1);
		Assert.Equal(200, actualValue2);
	}

	[Theory]
	[InlineData("test", "DictionaryOfStringToInteger")]
	[InlineData("move", "DictionaryOfStringToInteger")]
	[InlineData("copy", "DictionaryOfStringToInteger")]
	[InlineData("test", "NonGenericDictionary")]
	[InlineData("move", "NonGenericDictionary")]
	[InlineData("copy", "NonGenericDictionary")]
	public void ReadIntegerValueOfMissingKeyThrowsJsonPatchExceptionWithDefaultErrorReporter(string op, string dictionaryPropertyName)
	{
		// Arrange
		var model = new IntDictionary();
		var missingKey = "eight";
		var operation = new Operation<IntDictionary>(
			op,
			path: $"/{dictionaryPropertyName}/{missingKey}",
			from: $"/{dictionaryPropertyName}/{missingKey}",
			value: 8);

		var patchDocument = new JsonPatchDocument<IntDictionary>();
		patchDocument.Operations.Add(operation);

		// Act
		var exception = Assert.Throws<JsonPatchException>(() => { patchDocument.ApplyTo(model); });

		// Assert
		Assert.Equal($"The target location specified by path segment '{missingKey}' was not found.", exception.Message);
	}

	[Theory]
	[InlineData("test", "DictionaryOfStringToInteger")]
	[InlineData("move", "DictionaryOfStringToInteger")]
	[InlineData("copy", "DictionaryOfStringToInteger")]
	[InlineData("test", "NonGenericDictionary")]
	[InlineData("move", "NonGenericDictionary")]
	[InlineData("copy", "NonGenericDictionary")]
	public void ReadIntegerValueOfMissingKeyDoesNotThrowExceptionWithCustomErrorReporter(string op, string dictionaryPropertyName)
	{
		// Arrange
		var patchErrorLogger = new TestErrorLogger<DictionaryTest>();
		var model = new IntDictionary();
		var missingKey = "eight";
		var operation = new Operation<IntDictionary>(
			op,
			path: $"/{dictionaryPropertyName}/{missingKey}",
			from: $"/{dictionaryPropertyName}/{missingKey}",
			value: 8);

		var patchDocument = new JsonPatchDocument<IntDictionary>();
		patchDocument.Operations.Add(operation);

		// Act
		patchDocument.ApplyTo(model, patchErrorLogger.LogErrorMessage);

		// Assert
		Assert.Equal($"The target location specified by path segment '{missingKey}' was not found.", patchErrorLogger.ErrorMessage);
	}

	[Theory]
	[InlineData("test", "DictionaryOfIntegerToCustomer")]
	[InlineData("move", "DictionaryOfIntegerToCustomer")]
	[InlineData("copy", "DictionaryOfIntegerToCustomer")]
	[InlineData("test", "NonGenericDictionary")]
	[InlineData("move", "NonGenericDictionary")]
	[InlineData("copy", "NonGenericDictionary")]
	public void ReadPocoObjectValueOfMissingKeyThrowsJsonPatchExceptionWithDefaultErrorReporter(string op, string dictionaryPropertyName)
	{
		// Arrange
		var model = new CustomerDictionary();
		var missingKey = 8;
		var operation = new Operation<CustomerDictionary>(
			op,
			path: $"/{dictionaryPropertyName}/{missingKey}/Address/City",
			from: $"/{dictionaryPropertyName}/{missingKey}/Address/City",
			value: "Nowhere");

		var patchDocument = new JsonPatchDocument<CustomerDictionary>();
		patchDocument.Operations.Add(operation);

		// Act
		var exception = Assert.Throws<JsonPatchException>(() => { patchDocument.ApplyTo(model); });

		// Assert
		Assert.Equal($"The target location specified by path segment '{missingKey}' was not found.", exception.Message);
	}

	[Theory]
	[InlineData("test", "DictionaryOfIntegerToCustomer")]
	[InlineData("move", "DictionaryOfIntegerToCustomer")]
	[InlineData("copy", "DictionaryOfIntegerToCustomer")]
	[InlineData("test", "NonGenericDictionary")]
	[InlineData("move", "NonGenericDictionary")]
	[InlineData("copy", "NonGenericDictionary")]
	public void ReadPocoObjectValueOfMissingKeyDoesNotThrowExceptionWithCustomErrorReporter(string op, string dictionaryPropertyName)
	{
		// Arrange
		var patchErrorLogger = new TestErrorLogger<DictionaryTest>();
		var model = new CustomerDictionary();
		var missingKey = 8;
		var operation = new Operation<CustomerDictionary>(
			op,
			path: $"/{dictionaryPropertyName}/{missingKey}/Address/City",
			from: $"/{dictionaryPropertyName}/{missingKey}/Address/City",
			value: "Nowhere");

		var patchDocument = new JsonPatchDocument<CustomerDictionary>();
		patchDocument.Operations.Add(operation);

		// Act
		patchDocument.ApplyTo(model, patchErrorLogger.LogErrorMessage);

		// Assert
		Assert.Equal($"The target location specified by path segment '{missingKey}' was not found.", patchErrorLogger.ErrorMessage);
	}
}
