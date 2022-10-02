using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace SystemTextJsonPatch.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
public class ListAdapter : IAdapter
{
    public bool TryAdd(
        object target,
        string segment,
        JsonSerializerOptions options,
        object? value,
        out string? errorMessage)
    {
        var list = (IList)target;

        if (!TryGetListTypeArgument(list, out var typeArgument, out errorMessage))
        {
            return false;
        }

        if (!TryGetPositionInfo(list, segment, OperationType.Add, out var positionInfo, out errorMessage))
        {
            return false;
        }

        if (!TryConvertValue(value, typeArgument, segment, options, out var convertedValue, out errorMessage))
        {
            return false;
        }

        if (positionInfo.Type == PositionType.EndOfList)
        {
            list.Add(convertedValue);
        }
        else
        {
            list.Insert(positionInfo.Index, convertedValue);
        }

        errorMessage = null;
        return true;
    }

    public bool TryGet(
        object target,
        string segment,
        JsonSerializerOptions options,
        out object? value,
        out string? errorMessage)
    {
        var list = (IList)target;

        if (!TryGetListTypeArgument(list, out _, out errorMessage))
        {
            value = null;
            return false;
        }

        if (!TryGetPositionInfo(list, segment, OperationType.Get, out var positionInfo, out errorMessage))
        {
            value = null;
            return false;
        }

        if (positionInfo.Type == PositionType.EndOfList)
        {
            value = list[list.Count - 1];
        }
        else
        {
            value = list[positionInfo.Index];
        }

        errorMessage = null;
        return true;
    }

    public bool TryRemove(
        object target,
        string segment,
        JsonSerializerOptions options,
        out string? errorMessage)
    {
        var list = (IList)target;

        if (!TryGetListTypeArgument(list, out _, out errorMessage))
        {
            return false;
        }

        if (!TryGetPositionInfo(list, segment, OperationType.Remove, out var positionInfo, out errorMessage))
        {
            return false;
        }

        if (positionInfo.Type == PositionType.EndOfList)
        {
            list.RemoveAt(list.Count - 1);
        }
        else
        {
            list.RemoveAt(positionInfo.Index);
        }

        errorMessage = null;
        return true;
    }

    public bool TryReplace(
        object target,
        string segment,
        JsonSerializerOptions options,
        object? value,
        out string? errorMessage)
    {
        var list = (IList)target;

        if (!TryGetListTypeArgument(list, out var typeArgument, out errorMessage))
        {
            return false;
        }

        if (!TryGetPositionInfo(list, segment, OperationType.Replace, out var positionInfo, out errorMessage))
        {
            return false;
        }

        if (!TryConvertValue(value, typeArgument, segment, options, out var convertedValue, out errorMessage))
        {
            return false;
        }

        if (positionInfo.Type == PositionType.EndOfList)
        {
            list[list.Count - 1] = convertedValue;
        }
        else
        {
            list[positionInfo.Index] = convertedValue;
        }

        errorMessage = null;
        return true;
    }

    public bool TryTest(
        object target,
        string segment,
        JsonSerializerOptions options,
        object? value,
        out string? errorMessage)
    {
        var list = (IList)target;

        if (!TryGetListTypeArgument(list, out var typeArgument, out errorMessage))
        {
            return false;
        }

        if (!TryGetPositionInfo(list, segment, OperationType.Replace, out var positionInfo, out errorMessage))
        {
            return false;
        }

        if (!TryConvertValue(value, typeArgument, segment, options, out var convertedValue, out errorMessage))
        {
            return false;
        }

        var currentValue = list[positionInfo.Index];
        if (!string.Equals(JsonSerializer.SerializeToNode(currentValue)?.ToString(), JsonSerializer.SerializeToNode(convertedValue)?.ToString(), StringComparison.Ordinal)) {
            errorMessage = Resources.FormatValueAtListPositionNotEqualToTestValue(JsonSerializer.SerializeToNode(currentValue)?.ToString(), value, positionInfo.Index);
            return false;
        }

        errorMessage = null;
        return true;
    }

    public bool TryTraverse(
        object target,
        string segment,
        JsonSerializerOptions options,
        out object? nextTarget,
        out string? errorMessage)
    {
        if (target is not IList list)
        {
            nextTarget = null;
            errorMessage = null;
            return false;
        }

        if (!int.TryParse(segment, out var index))
        {
            nextTarget = null;
            errorMessage = Resources.FormatInvalidIndexValue(segment);
            return false;
        }

        if (index < 0 || index >= list.Count)
        {
            nextTarget = null;
            errorMessage = Resources.FormatIndexOutOfBounds(segment);
            return false;
        }

        nextTarget = list[index];
        errorMessage = null;
        return true;
    }

    protected static bool TryConvertValue(
        object? originalValue,
        Type listTypeArgument,
        string _,
        JsonSerializerOptions options,
        out object? convertedValue,
        out string? errorMessage)
    {
        if (!ConversionResultProvider.TryConvertTo(originalValue, listTypeArgument, options, out convertedValue, out string? conversionErrorMessage))
        {
            errorMessage = conversionErrorMessage ?? Resources.FormatInvalidValueForProperty(originalValue);
            return false;
        }
        errorMessage = null;
        return true;
    }

    private static bool TryGetListTypeArgument(IList list, out Type? listTypeArgument, out string? errorMessage)
    {
        // Arrays are not supported as they have fixed size and operations like Add, Insert do not make sense
        var listType = list.GetType();
        if (listType.IsArray)
        {
            errorMessage = Resources.FormatPatchNotSupportedForArrays(listType.FullName);
            listTypeArgument = null;
            return false;
        }

        var genericList = ExtractGenericInterface(listType, typeof(IList<>));
        if (genericList == null)
        {
            errorMessage = Resources.FormatPatchNotSupportedForNonGenericLists(listType.FullName);
            listTypeArgument = null;
            return false;
        }

        listTypeArgument = genericList.GenericTypeArguments[0];
        errorMessage = null;
        return true;
    }

    private static Type? ExtractGenericInterface(Type queryType, Type interfaceType)
    {
        if (queryType == null)
        {
            throw new ArgumentNullException(nameof(queryType));
        }

        if (interfaceType == null)
        {
            throw new ArgumentNullException(nameof(interfaceType));
        }

        if (IsGenericInstantiation(queryType, interfaceType))
        {
            // queryType matches (i.e. is a closed generic type created from) the open generic type.
            return queryType;
        }

        // Otherwise check all interfaces the type implements for a match.
        // - If multiple different generic instantiations exists, we want the most derived one.
        // - If that doesn't break the tie, then we sort alphabetically so that it's deterministic.
        //
        // We do this by looking at interfaces on the type, and recursing to the base type
        // if we don't find any matches.
        return GetGenericInstantiation(queryType, interfaceType);
    }

    private static Type? GetGenericInstantiation(Type queryType, Type interfaceType)
    {
        Type? bestMatch = null;
        var interfaces = queryType.GetInterfaces();
        foreach (var @interface in interfaces)
        {
            if (IsGenericInstantiation(@interface, interfaceType))
            {
                if (bestMatch == null)
                {
                    bestMatch = @interface;
                }
                else if (StringComparer.Ordinal.Compare(@interface.FullName, bestMatch.FullName) < 0)
                {
                    bestMatch = @interface;
                }
            }
        }

        if (bestMatch != null)
        {
            return bestMatch;
        }

        // BaseType will be null for object and interfaces, which means we've reached 'bottom'.
        var baseType = queryType?.BaseType;
        if (baseType == null)
        {
            return null;
        }

        return GetGenericInstantiation(baseType, interfaceType);
    }

    private static bool IsGenericInstantiation(Type candidate, Type interfaceType)
    {
        return
            candidate.IsGenericType &&
            candidate.GetGenericTypeDefinition() == interfaceType;
    }

    private static bool TryGetPositionInfo(
        IList list,
        string segment,
        OperationType operationType,
        out PositionInfo positionInfo,
        out string? errorMessage)
    {
        if (segment == "-")
        {
            positionInfo = new PositionInfo(PositionType.EndOfList, -1);
            errorMessage = null;
            return true;
        }

        if (int.TryParse(segment, out var position))
        {
            if (position >= 0 && position < list.Count)
            {
                positionInfo = new PositionInfo(PositionType.Index, position);
                errorMessage = null;
                return true;
            }
            // As per JSON Patch spec, for Add operation the index value representing the number of elements is valid,
            // where as for other operations like Remove, Replace, Move and Copy the target index MUST exist.

            if (position == list.Count && operationType == OperationType.Add)
            {
                positionInfo = new PositionInfo(PositionType.EndOfList, -1);
                errorMessage = null;
                return true;
            }
            positionInfo = new PositionInfo(PositionType.OutOfBounds, position);
            errorMessage = Resources.FormatIndexOutOfBounds(segment);
            return false;
        }

        positionInfo = new PositionInfo(PositionType.Invalid, -1);
        errorMessage = Resources.FormatInvalidIndexValue(segment);
        return false;
    }

    /// <summary>
    /// This API supports infrastructure and is not intended to be used
    /// directly from your code. This API may change or be removed in future releases.
    /// </summary>
    protected readonly struct PositionInfo
    {
        public PositionInfo(PositionType type, int index)
        {
            Type = type;
            Index = index;
        }

        public PositionType Type { get; }
        public int Index { get; }

        public override bool Equals(object? obj)
        {
            if (obj is PositionInfo another)
            {
                return this.Equals(another);
            }

            return false;
        }

        public bool Equals(PositionInfo another)
        {
            return this.Type == another.Type && this.Index == another.Index;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() ^ Index.GetHashCode();
        }

        public static bool operator ==(PositionInfo left, PositionInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PositionInfo left, PositionInfo right)
        {
            return !(left == right);
        }
    }

    /// <summary>
    /// This API supports infrastructure and is not intended to be used
    /// directly from your code. This API may change or be removed in future releases.
    /// </summary>
    protected enum PositionType
    {
        Index, // valid index
        EndOfList, // '-'
        Invalid, // Ex: not an integer
        OutOfBounds
    }

    /// <summary>
    /// This API supports infrastructure and is not intended to be used
    /// directly from your code. This API may change or be removed in future releases.
    /// </summary>
    protected enum OperationType
    {
        Add,
        Remove,
        Get,
        Replace
    }
}
