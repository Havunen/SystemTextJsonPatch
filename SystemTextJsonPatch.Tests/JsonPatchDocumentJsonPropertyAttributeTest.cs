using System.Linq;
using System.Text.Json.Serialization;
using Xunit;

namespace SystemTextJsonPatch;

public class JsonPatchDocumentJsonPropertyAttributeTest
{
    [Fact]
    public void AddRespectsJsonPropertyAttribute()
    {
        // Arrange
        var patchDocument = new JsonPatchDocument<JsonPropertyObject>();

        // Act
        patchDocument.Add(p => p.Name, "John");

        // Assert
        var pathToCheck = patchDocument.Operations.First().Path;
        Assert.Equal("/AnotherName", pathToCheck);
    }

    [Fact]
    public void AddRespectsJsonPropertyAttributeWithDotWhitespaceAndBackslashInName()
    {
        // Arrange
        var obj = new JsonPropertyObjectWithStrangeNames();
        var patchDocument = new JsonPatchDocument();

        // Act
        patchDocument.Add("/First Name.", "John");
        patchDocument.Add("Last\\Name", "Doe");
        patchDocument.ApplyTo(obj);

        // Assert
        Assert.Equal("John", obj.FirstName);
        Assert.Equal("Doe", obj.LastName);
    }

    [Fact]
    public void MoveFallsbackToPropertyNameWhenJsonPropertyAttributeNameIsEmpty()
    {
        // Arrange
        var patchDocument = new JsonPatchDocument<JsonPropertyWithNoPropertyName>();

        // Act
        patchDocument.Move(m => m.StringProperty, m => m.StringProperty2);

        // Assert
        var fromPath = patchDocument.Operations.First().From;
        Assert.Equal("/StringProperty", fromPath);
        var toPath = patchDocument.Operations.First().Path;
        Assert.Equal("/StringProperty2", toPath);
    }

    private class JsonPropertyObject
    {
        [JsonPropertyName("AnotherName")]
        public string Name { get; set; }
    }

    private class JsonPropertyObjectWithStrangeNames
    {
        [JsonPropertyName("First Name.")]
        public string FirstName { get; set; }

        [JsonPropertyName("Last\\Name")]
        public string LastName { get; set; }
    }

    private class JsonPropertyWithNoPropertyName
    {
        [JsonPropertyName(null)]
        public string StringProperty { get; set; }

        [JsonPropertyName(null)]
        public string[] ArrayProperty { get; set; }

        [JsonPropertyName(null)]
        public string StringProperty2 { get; set; }

        [JsonPropertyName(null)]
        public string Ssn { get; set; }
    }
}
