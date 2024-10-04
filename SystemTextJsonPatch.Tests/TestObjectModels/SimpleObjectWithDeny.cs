using System;
using System.Collections.Generic;

namespace SystemTextJsonPatch;

public class SimpleObjectWithDeny
{
    [DenyPatch]
    public List<int> IntegerList { get; set; }
    [DenyPatch]
    public IList<int> IntegerIList { get; set; }
    [DenyPatch]
    public int IntegerValue { get; set; }
    [DenyPatch]
    public int AnotherIntegerValue { get; set; }
    [DenyPatch]
    public string StringProperty { get; set; }
    [DenyPatch]
    public string AnotherStringProperty { get; set; }
    [DenyPatch]
    public decimal DecimalValue { get; set; }
    [DenyPatch]
    public double DoubleValue { get; set; }
    [DenyPatch]
    public float FloatValue { get; set; }
    [DenyPatch]
    public Guid GuidValue { get; set; }
}
