using System.Collections.Generic;
using System.Text.Json;
using SystemTextJsonPatch.Adapters;
using SystemTextJsonPatch.Internal;
using Xunit;

namespace SystemTextJsonPatch.Test.Adapters;

public class AdapterFactoryTests
{
    [Fact]
    public void GetListAdapterForListTargets()
    {
        // Arrange
        var factory = new AdapterFactory();

        //Act:
        var adapter = factory.Create(new List<string>(), new JsonSerializerOptions());

        // Assert
        Assert.Equal(typeof(ListAdapter), adapter.GetType());
    }
}
