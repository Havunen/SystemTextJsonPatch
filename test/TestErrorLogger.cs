// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace SystemTextJsonPatch;

public class TestErrorLogger<T> where T : class
{
    public string ErrorMessage { get; set; }

    public void LogErrorMessage(JsonPatchError patchError)
    {
        ErrorMessage = patchError.ErrorMessage;
    }
}
