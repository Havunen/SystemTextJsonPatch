namespace SystemTextJsonPatch.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
public class ConversionResult
{
    public ConversionResult(bool canBeConverted, object convertedInstance, string? errorMessage = null)
    {
        CanBeConverted = canBeConverted;
        ConvertedInstance = convertedInstance;
        ErrorMessage = errorMessage;
    }

    public bool CanBeConverted { get; }
    public object ConvertedInstance { get; }
    public string? ErrorMessage { get; }
}
