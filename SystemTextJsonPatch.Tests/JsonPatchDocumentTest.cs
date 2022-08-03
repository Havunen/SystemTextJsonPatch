using System.Linq;
using System.Text.Json;
using SystemTextJsonPatch.Exceptions;
using Xunit;

namespace SystemTextJsonPatch;

public class JsonPatchDocumentTest
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
    {
        Converters =
        {
            new Converters.JsonPatchDocumentConverterFactory()
        }
    };

    [Fact]
    public void InvalidPathAtBeginningShouldThrowException()
    {
        // Arrange
        var patchDocument = new JsonPatchDocument();

        // Act
        var exception = Assert.Throws<JsonPatchException>(() =>
        {
            patchDocument.Add("//NewInt", 1);
        });

        // Assert
        Assert.Equal(
           "The provided string '//NewInt' is an invalid path.",
            exception.Message);
    }

    [Fact]
    public void InvalidPathAtEndShouldThrowException()
    {
        // Arrange
        var patchDocument = new JsonPatchDocument();

        // Act
        var exception = Assert.Throws<JsonPatchException>(() =>
        {
            patchDocument.Add("NewInt//", 1);
        });

        // Assert
        Assert.Equal(
           "The provided string 'NewInt//' is an invalid path.",
            exception.Message);
    }

    [Fact]
    public void NonGenericPatchDocToGenericMustSerialize()
    {
        // Arrange
        var targetObject = new SimpleObject()
        {
            StringProperty = "A",
            AnotherStringProperty = "B"
        };

        var patchDocument = new JsonPatchDocument();
        patchDocument.Copy("StringProperty", "AnotherStringProperty");

        var serialized = JsonSerializer.Serialize(patchDocument, _options);
        var deserialized = JsonSerializer.Deserialize<JsonPatchDocument<SimpleObject>>(serialized, _options);

        // Act
        deserialized.ApplyTo(targetObject);

        // Assert
        Assert.Equal("A", targetObject.AnotherStringProperty);
    }

    [Fact]
    public void GenericPatchDocToNonGenericMustSerialize()
    {
        // Arrange
        var targetObject = new SimpleObject()
        {
            StringProperty = "A",
            AnotherStringProperty = "B"
        };

        var patchDocTyped = new JsonPatchDocument<SimpleObject>();
        patchDocTyped.Copy(o => o.StringProperty, o => o.AnotherStringProperty);

        var patchDocUntyped = new JsonPatchDocument();
        patchDocUntyped.Copy("StringProperty", "AnotherStringProperty");

        var serializedTyped =JsonSerializer.Serialize(patchDocTyped, _options);
        var serializedUntyped =JsonSerializer.Serialize(patchDocUntyped, _options);
        var deserialized = JsonSerializer.Deserialize<JsonPatchDocument>(serializedTyped, _options);

        // Act
        deserialized.ApplyTo(targetObject);

        // Assert
        Assert.Equal("A", targetObject.AnotherStringProperty);
    }

    [Fact]
    public void DeserializationSuccessfulForValidJsonPatchDocument()
    {
        // Arrange
        var doc = new SimpleObject()
        {
            StringProperty = "A",
            DecimalValue = 10,
            DoubleValue = 10,
            FloatValue = 10,
            IntegerValue = 10
        };

        var patchDocument = new JsonPatchDocument<SimpleObject>();
        patchDocument.Replace(o => o.StringProperty, "B");
        patchDocument.Replace(o => o.DecimalValue, 12);
        patchDocument.Replace(o => o.DoubleValue, 12);
        patchDocument.Replace(o => o.FloatValue, 12);
        patchDocument.Replace(o => o.IntegerValue, 12);

        // default: no envelope
        var serialized =JsonSerializer.Serialize(patchDocument, _options);

        // Act
        var deserialized = JsonSerializer.Deserialize<JsonPatchDocument<SimpleObject>>(serialized, _options);

        // Assert
        Assert.IsType<JsonPatchDocument<SimpleObject>>(deserialized);
    }

    [Fact]
    public void DeserializationFailsForInvalidJsonPatchDocument()
    {
        // Arrange
        var serialized = "{\"Operations\": [{ \"op\": \"replace\", \"path\": \"/title\", \"value\": \"New Title\"}]}";

        // Act
        var exception = Assert.Throws<JsonPatchException>(() =>
        {
            var deserialized
                = JsonSerializer.Deserialize<JsonPatchDocument>(serialized, _options);
        });

        // Assert
        Assert.Equal("The JSON patch document was malformed and could not be parsed.", exception.Message);
    }


    [Fact]
    public void DeserializationSucceedsForValidJsonPatchDocumentSingleOperation()
    {
        // Arrange
        var serialized = "[{ \"op\": \"replace\", \"path\": \"/title\", \"value\": \"New Title\"}]";

        // Act
        var deserialized = JsonSerializer.Deserialize<JsonPatchDocument>(serialized, _options);

        // Assert
        Assert.Equal(1, deserialized.Operations.Count);

        var firstOp = deserialized.Operations.First();
        Assert.Equal("replace", firstOp.Op);
        Assert.Equal("/title", firstOp.Path);
        Assert.Equal("New Title", firstOp.Value);
    }

    [Fact]
    public void DeserializationSucceedsForValidJsonPatchDocumentMultipleOperation()
    {
        // Arrange
        var serialized = "[{ \"op\": \"replace\", \"path\": \"/title\", \"value\": \"New Title\"}, { \"op\": \"add\", \"path\": \"/test\", \"value\": \"New Title2\"}]";

        // Act
        var deserialized = JsonSerializer.Deserialize<JsonPatchDocument>(serialized, _options);

        // Assert
        Assert.Equal(2, deserialized.Operations.Count);

        var firstOp = deserialized.Operations.First();
        Assert.Equal("replace", firstOp.Op);
        Assert.Equal("/title", firstOp.Path);
        Assert.Equal("New Title", firstOp.Value);

        var secondOp = deserialized.Operations[1];
        Assert.Equal("add", secondOp.Op);
        Assert.Equal("/test", secondOp.Path);
        Assert.Equal("New Title2", secondOp.Value);
    }

    [Fact]
    public void DeserializationFailsWhenCommaMissing()
    {
        // Arrange
        var serialized = "[{ \"op\": \"replace\", \"path\": \"/title\", \"value\": \"New Title\"} { \"op\": \"add\", \"path\": \"/test\", \"value\": \"New Title2\"}]";

        // Act
        Assert.Throws<JsonException>(() =>
        {
            var deserialized
                = JsonSerializer.Deserialize<JsonPatchDocument<SimpleObject>>(serialized, _options);
        });
    }

    [Fact]
    public void DeserializationFailsForInvalidTypedJsonPatchDocument()
    {
        // Arrange
        var serialized = "{\"Operations\": [{ \"op\": \"replace\", \"path\": \"/title\", \"value\": \"New Title\"}]}";

        // Act
        var exception = Assert.Throws<JsonPatchException>(() =>
        {
            var deserialized
                = JsonSerializer.Deserialize<JsonPatchDocument<SimpleObject>>(serialized, _options);
        });

        // Assert
        Assert.Equal("The JSON patch document was malformed and could not be parsed.", exception.Message);
    }
}
