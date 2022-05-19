using System.Text.Json;
using SystemTextJsonPatch.Exceptions;
using SystemTextJsonPatch.Operations;
using Xunit;

namespace SystemTextJsonPatch;

public class JsonPatchDocumentJsonObjectTest
{
    [Fact]
    public void ApplyTo_Array_Add()
    {
        // Arrange
        var model = new ObjectWithJsonNode { CustomData = JsonSerializer.SerializeToNode(new { Emails = new[] { "foo@bar.com" } }) };
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("add", "/CustomData/Emails/-", null, "foo@baz.com"));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Equal("foo@bar.com", model.CustomData["Emails"][0].GetValue<string>());
        Assert.Equal("foo@baz.com", model.CustomData["Emails"][1].GetValue<string>());
    }

    [Fact]
    public void ApplyTo_Model_Test1()
    {
        // Arrange
        var model = new ObjectWithJsonNode { CustomData = JsonSerializer.SerializeToNode(new { Email = "foo@bar.com", Name = "Bar" }) };
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("test", "/CustomData/Email", null, "foo@baz.com"));
        patch.Operations.Add(new Operation<ObjectWithJsonNode>("add", "/CustomData/Name", null, "Bar Baz"));

        // Act & Assert
        Assert.Throws<JsonPatchException>(() => patch.ApplyTo(model));
    }

    [Fact]
    public void ApplyTo_Model_Test2()
    {
        // Arrange
        var model = new ObjectWithJsonNode { CustomData = JsonSerializer.SerializeToNode(new { Email = "foo@bar.com", Name = "Bar" }) };
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("test", "/CustomData/Email", null, "foo@bar.com"));
        patch.Operations.Add(new Operation<ObjectWithJsonNode>("add", "/CustomData/Name", null, "Bar Baz"));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Equal("Bar Baz", model.CustomData["Name"].GetValue<string>());
    }

    [Fact]
    public void ApplyTo_Model_Copy()
    {
        // Arrange
        var model = new ObjectWithJsonNode { CustomData = JsonSerializer.SerializeToNode(new { Email = "foo@bar.com" }) };
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("copy", "/CustomData/UserName", "/CustomData/Email"));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Equal("foo@bar.com", model.CustomData["UserName"].GetValue<string>());
    }

    [Fact]
    public void ApplyTo_Model_Remove()
    {
        // Arrange
        var model = new ObjectWithJsonNode { CustomData = JsonSerializer.SerializeToNode(new { FirstName = "Foo", LastName = "Bar" }) };
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("remove", "/CustomData/LastName", null));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Null(model.CustomData["LastName"]);
    }

    [Fact]
    public void ApplyTo_Model_Move()
    {
        // Arrange
        var model = new ObjectWithJsonNode { CustomData = JsonSerializer.SerializeToNode(new { FirstName = "Bar" }) };
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("move", "/CustomData/LastName", "/CustomData/FirstName"));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Null(model.CustomData["FirstName"]);
        Assert.Equal("Bar", model.CustomData["LastName"].GetValue<string>());
    }

    [Fact]
    public void ApplyTo_Model_Add()
    {
        // Arrange
        var model = new ObjectWithJsonNode();
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("add", "/CustomData/Name", null, "Foo"));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Equal("Foo", model.CustomData["Name"].GetValue<string>());
    }

    [Fact]
    public void ApplyTo_Model_Add_Null()
    {
        // Arrange
        var model = new ObjectWithJsonNode();
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("add", "/CustomData/Name", null, null));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Null(model.CustomData["Name"]);
    }

    [Fact]
    public void ApplyTo_Model_Replace()
    {
        // Arrange
        var model = new ObjectWithJsonNode { CustomData = JsonSerializer.SerializeToNode(new { Email = "foo@bar.com", Name = "Bar" }) };
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("replace", "/CustomData/Email", null, "foo@baz.com"));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Equal("foo@baz.com", model.CustomData["Email"].GetValue<string>());
    }

    [Fact]
    public void ApplyTo_Model_Replace_Null()
    {
        // Arrange
        var model = new ObjectWithJsonNode { CustomData = JsonSerializer.SerializeToNode(new { Email = "foo@bar.com", Name = "Bar" }) };
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("replace", "/CustomData/Email", null, null));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Null(model.CustomData["Email"]);
    }
}
