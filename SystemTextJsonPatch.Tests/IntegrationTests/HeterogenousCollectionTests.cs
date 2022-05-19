using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using SystemTextJsonPatch.Converters;
using Xunit;

namespace SystemTextJsonPatch.IntegrationTests;

public class HeterogenousCollectionTests
{
    [Fact]
    public void AddItemToList()
    {
        // Arrange
        var targetObject = new Canvas()
        {
            Items = new List<Shape>()
        };

        var circleJsonNode = JsonNode.Parse(@"{
            ""Type"": ""Circle"",
            ""ShapeProperty"": ""Shape property"",
            ""CircleProperty"": ""Circle property""
        }");

        var patchDocument = new JsonPatchDocument
        {
            Options = new JsonSerializerOptions()
            {
                Converters = { new ShapeJsonConverter(), new JsonPatchDocumentConverterFactory() }
            }
        };

        patchDocument.Add("/Items/-", circleJsonNode);

        // Act
        patchDocument.ApplyTo(targetObject);

        // Assert
        var circle = targetObject.Items[0] as Circle;
        Assert.NotNull(circle);
        Assert.Equal("Shape property", circle.ShapeProperty);
        Assert.Equal("Circle property", circle.CircleProperty);
    }
}

public class ShapeJsonConverter : JsonConverter<Shape>
{
    private const string TypeProperty = "Type";

    private Shape CreateShape(JsonNode JsonNode)
    {
        var typeProperty = JsonNode[TypeProperty];

        switch (typeProperty.GetValue<string>())
        {
            case "Circle":
                return new Circle();

            case "Rectangle":
                return new Rectangle();
        }

        throw new NotSupportedException();
    }

    public override Shape Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var JsonNode = System.Text.Json.Nodes.JsonNode.Parse(ref reader);

        var target = CreateShape(JsonNode);

        target = (Shape)System.Text.Json.JsonSerializer.Deserialize(JsonNode, target.GetType());

        return target;
    }

    public override void Write(Utf8JsonWriter writer, Shape value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
