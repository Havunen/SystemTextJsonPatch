namespace SystemTextJsonPatch;

public class TestErrorLogger<T> where T : class
{
    public string ErrorMessage { get; set; }

    public void LogErrorMessage(JsonPatchError patchError)
    {
        ErrorMessage = patchError.ErrorMessage;
    }
}
