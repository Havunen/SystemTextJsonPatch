using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using Xunit;

namespace SystemTextJsonPatch.Internal;

public class ObjectVisitorTest
{
	private class Class1
	{
		public string Name { get; set; }
		public IList<string> States { get; set; } = new List<string>();
		public IDictionary<string, string> CountriesAndRegions { get; set; } = new Dictionary<string, string>();
		public dynamic Items { get; set; } = new ExpandoObject();
	}

	private class Class1Nested
	{
		public List<Class1> Customers { get; set; } = new List<Class1>();
	}

	public static IEnumerable<object[]> ReturnsListAdapterData
	{
		get
		{
			var model = new Class1();
			yield return new object[] { model, "/States/-", model.States };
			yield return new object[] { model.States, "/-", model.States };

			var nestedModel = new Class1Nested();
			nestedModel.Customers.Add(new Class1());
			yield return new object[] { nestedModel, "/Customers/0/States/-", nestedModel.Customers[0].States };
			yield return new object[] { nestedModel, "/Customers/0/States/0", nestedModel.Customers[0].States };
			yield return new object[] { nestedModel.Customers, "/0/States/-", nestedModel.Customers[0].States };
			yield return new object[] { nestedModel.Customers[0], "/States/-", nestedModel.Customers[0].States };
		}
	}

	[Theory]
	[MemberData(nameof(ReturnsListAdapterData))]
	public void VisitValidPathToArrayReturnsListAdapter(object targetObject, string path, object expectedTargetObject)
	{
		// Arrange
		var visitor = new ObjectVisitor(new ParsedPath(path), new JsonSerializerOptions() { });

		// Act
		var visitStatus = visitor.TryVisit(ref targetObject, out var adapter, out var message);

		// Assert
		Assert.True(visitStatus);
		Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
		Assert.Same(expectedTargetObject, targetObject);
		Assert.IsType<ListAdapter>(adapter);
	}

	public static IEnumerable<object[]> ReturnsDictionaryAdapterData
	{
		get
		{
			var model = new Class1();
			yield return new object[] { model, "/CountriesAndRegions/USA", model.CountriesAndRegions };
			yield return new object[] { model.CountriesAndRegions, "/USA", model.CountriesAndRegions };

			var nestedModel = new Class1Nested();
			nestedModel.Customers.Add(new Class1());
			yield return new object[] { nestedModel, "/Customers/0/CountriesAndRegions/USA", nestedModel.Customers[0].CountriesAndRegions };
			yield return new object[] { nestedModel.Customers, "/0/CountriesAndRegions/USA", nestedModel.Customers[0].CountriesAndRegions };
			yield return new object[] { nestedModel.Customers[0], "/CountriesAndRegions/USA", nestedModel.Customers[0].CountriesAndRegions };
		}
	}

	[Theory]
	[MemberData(nameof(ReturnsDictionaryAdapterData))]
	public void VisitValidPathToDictionaryReturnsDictionaryAdapter(object targetObject, string path, object expectedTargetObject)
	{
		// Arrange
		var visitor = new ObjectVisitor(new ParsedPath(path), new JsonSerializerOptions() { });

		// Act
		var visitStatus = visitor.TryVisit(ref targetObject, out var adapter, out var message);

		// Assert
		Assert.True(visitStatus);
		Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
		Assert.Same(expectedTargetObject, targetObject);
		//Assert.Equal(typeof(DictionaryAdapter<string, string>), adapter.GetType());
	}

	public static IEnumerable<object[]> ReturnsExpandoAdapterData
	{
		get
		{
			var nestedModel = new Class1Nested();
			nestedModel.Customers.Add(new Class1());
			yield return new object[] { nestedModel, "/Customers/0/Items/Name", nestedModel.Customers[0].Items };
			yield return new object[] { nestedModel.Customers, "/0/Items/Name", nestedModel.Customers[0].Items };
			yield return new object[] { nestedModel.Customers[0], "/Items/Name", nestedModel.Customers[0].Items };
		}
	}

	[Theory]
	[MemberData(nameof(ReturnsExpandoAdapterData))]
	public void VisitValidPathToExpandoObjectReturnsExpandoAdapter(object targetObject, string path, object expectedTargetObject)
	{
		// Arrange
		var visitor = new ObjectVisitor(new ParsedPath(path), new JsonSerializerOptions() { });

		// Act
		var visitStatus = visitor.TryVisit(ref targetObject, out var adapter, out var message);

		// Assert
		Assert.True(visitStatus);
		Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
		Assert.Same(expectedTargetObject, targetObject);
		//Assert.Same(typeof(DictionaryAdapter<string, object>), adapter.GetType());
	}

	public static IEnumerable<object[]> ReturnsPocoAdapterData
	{
		get
		{
			var model = new Class1();
			yield return new object[] { model, "/Name", model };

			var nestedModel = new Class1Nested();
			nestedModel.Customers.Add(new Class1());
			yield return new object[] { nestedModel, "/Customers/0/Name", nestedModel.Customers[0] };
			yield return new object[] { nestedModel.Customers, "/0/Name", nestedModel.Customers[0] };
			yield return new object[] { nestedModel.Customers[0], "/Name", nestedModel.Customers[0] };
		}
	}

	[Theory]
	[MemberData(nameof(ReturnsPocoAdapterData))]
	public void VisitValidPathReturnsExpandoAdapter(object targetObject, string path, object expectedTargetObject)
	{
		// Arrange
		var visitor = new ObjectVisitor(new ParsedPath(path), new JsonSerializerOptions() { });

		// Act
		var visitStatus = visitor.TryVisit(ref targetObject, out var adapter, out var message);

		// Assert
		Assert.True(visitStatus);
		Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
		Assert.Same(expectedTargetObject, targetObject);
		//Assert.IsType<PocoAdapter>(adapter);
	}

	[Theory]
	[InlineData("0")]
	[InlineData("-1")]
	public void VisitInvalidIndexToArrayFails(string position)
	{
		// Arrange
		var visitor = new ObjectVisitor(new ParsedPath($"/Customers/{position}/States/-"), new JsonSerializerOptions() { });
		var automobileDepartment = new Class1Nested();
		object targetObject = automobileDepartment;

		// Act
		var visitStatus = visitor.TryVisit(ref targetObject, out var adapter, out var message);

		// Assert
		Assert.False(visitStatus);
		Assert.Equal($"The index value provided by path segment '{position}' is out of bounds of the array size.", message);
	}

	[Theory]
	[InlineData("-")]
	[InlineData("foo")]
	public void VisitInvalidIndexFormatToArrayFails(string position)
	{
		// Arrange
		var visitor = new ObjectVisitor(new ParsedPath($"/Customers/{position}/States/-"), new JsonSerializerOptions() { });
		var automobileDepartment = new Class1Nested();
		object targetObject = automobileDepartment;

		// Act
		var visitStatus = visitor.TryVisit(ref targetObject, out var adapter, out var message);

		// Assert
		Assert.False(visitStatus);
		Assert.Equal($"The path segment '{position}' is invalid for an array index.", message);
	}

	[Fact]
	public void VisitDoesNotValidateFinalPathSegment()
	{
		// Arrange
		var visitor = new ObjectVisitor(new ParsedPath($"/NonExisting"), new JsonSerializerOptions() { });
		var model = new Class1();
		object targetObject = model;

		// Act
		var visitStatus = visitor.TryVisit(ref targetObject, out var adapter, out var message);

		// Assert
		Assert.True(visitStatus);
		Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
		//Assert.IsType<PocoAdapter>(adapter);
	}

	[Fact]
	public void VisitNullInteriorTargetReturnsFalse()
	{
		// Arrange
		var visitor = new ObjectVisitor(new ParsedPath("/States/0"), new JsonSerializerOptions() { });

		// Act
		object target = new Class1() { States = null, };
		var visitStatus = visitor.TryVisit(ref target, out var adapter, out var message);

		// Assert
		Assert.False(visitStatus);
		Assert.Null(adapter);
		Assert.Null(message);
	}

	[Fact]
	public void VisitNullTargetReturnsNullAdapter()
	{
		// Arrange
		var visitor = new ObjectVisitor(new ParsedPath("test"), new JsonSerializerOptions() { });

		// Act
		object target = null;
		var visitStatus = visitor.TryVisit(ref target, out var adapter, out var message);

		// Assert
		Assert.False(visitStatus);
		Assert.Null(adapter);
		Assert.Null(message);
	}
}
