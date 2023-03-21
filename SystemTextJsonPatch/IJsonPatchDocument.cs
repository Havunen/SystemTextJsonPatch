using System.Collections.Generic;
using System.Text.Json;
using SystemTextJsonPatch.Operations;

namespace SystemTextJsonPatch;

public interface IJsonPatchDocument
{
    JsonSerializerOptions Options { get; set; }

    IList<Operation> GetOperations();
}
