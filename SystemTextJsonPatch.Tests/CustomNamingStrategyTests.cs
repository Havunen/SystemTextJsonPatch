using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using Xunit;

namespace SystemTextJsonPatch;

public class CustomNamingStrategyTests
{
    [Fact]
    public void AddProperty_ToDynamicTestObject_WithCustomNamingStrategy()
    {
        // Arrange
        var options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = new TestNamingStrategy()
        };

        dynamic targetObject = new DynamicTestObject();
        targetObject.Test = 1;

        var patchDocument = new JsonPatchDocument();
        patchDocument.Add("NewInt", 1);
        patchDocument.Options = options;

        // Act
        patchDocument.ApplyTo(targetObject);

        // Assert
        Assert.Equal(1, targetObject.customNewInt);
        Assert.Equal(1, targetObject.Test);
    }

    [Fact]
    public void CopyPropertyValue_ToDynamicTestObject_WithCustomNamingStrategy()
    {
        // Arrange
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new TestNamingStrategy()
        };

        dynamic targetObject = new DynamicTestObject();
        targetObject.customStringProperty = "A";
        targetObject.customAnotherStringProperty = "B";

        var patchDocument = new JsonPatchDocument()
        {
            Options = options
        };
        patchDocument.Copy("StringProperty", "AnotherStringProperty");

        // Act
        patchDocument.ApplyTo(targetObject);

        // Assert
        Assert.Equal("A", targetObject.customAnotherStringProperty);
    }

    [Fact]
    public void MovePropertyValue_ForExpandoObject_WithCustomNamingStrategy()
    {
        // Arrange
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new TestNamingStrategy()
        };

        dynamic targetObject = new ExpandoObject();
        targetObject.customStringProperty = "A";
        targetObject.customAnotherStringProperty = "B";

        var patchDocument = new JsonPatchDocument();
        patchDocument.Move("StringProperty", "AnotherStringProperty");
        patchDocument.Options = options;

        // Act
        patchDocument.ApplyTo(targetObject);
        var cont = targetObject as IDictionary<string, object>;
        cont.TryGetValue("customStringProperty", out var valueFromDictionary);

        // Assert
        Assert.Equal("A", targetObject.customAnotherStringProperty);
        Assert.Null(valueFromDictionary);
    }

    [Fact]
    public void RemoveProperty_DynamicTestObject_WithCustomNamingStrategy()
    {
        // Arrange
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new TestNamingStrategy()
        };

        dynamic targetObject = new DynamicTestObject();
        targetObject.customTest = "A";

        var patchDocument = new JsonPatchDocument();
        patchDocument.Remove("Test");
        patchDocument.Options = options;

        // Act
        patchDocument.ApplyTo(targetObject);

        // Assert
        Assert.Null(targetObject.customTest);
    }

    [Fact]
    public void RemoveProperty_FromDictionaryObject_WithCustomNamingStrategy()
    {
        // Arrange
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new TestNamingStrategy()
        };

        var targetObject = new Dictionary<string, int>()
        {
            { "customTest", 1},
        };

        var patchDocument = new JsonPatchDocument();
        patchDocument.Remove("Test");
        patchDocument.Options = options;

        // Act
        patchDocument.ApplyTo(targetObject);
        var cont = targetObject as IDictionary<string, int>;
        cont.TryGetValue("customTest", out var valueFromDictionary);

        // Assert
        Assert.Equal(0, valueFromDictionary);
    }

    [Fact]
    public void ReplacePropertyValue_ForExpandoObject_WithCustomNamingStrategy()
    {
        // Arrange
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new TestNamingStrategy()
        };

        dynamic targetObject = new ExpandoObject();
        targetObject.customTest = 1;

        var patchDocument = new JsonPatchDocument();
        patchDocument.Replace("Test", 2);
        patchDocument.Options = options;

        // Act
        patchDocument.ApplyTo(targetObject);

        // Assert
        Assert.Equal(2, targetObject.customTest);
    }

    private class TestNamingStrategy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return "custom" + name;
        }
    }
}
