using System.Collections.Generic;

namespace SystemTextJsonPatch;

public class SimpleObjectWithNestedObjectAndDeny
{
    public int IntegerValue { get; set; }
    [DenyPatch]
    public NestedObject NestedObject { get; set; }
    [DenyPatch]
    public SimpleObject SimpleObject { get; set; }
    [DenyPatch]
    public InheritedObject InheritedObject { get; set; }
    [DenyPatch]
    public List<SimpleObject> SimpleObjectList { get; set; }
    [DenyPatch]
    public IList<SimpleObject> SimpleObjectIList { get; set; }

    public SimpleObjectWithDeny Allow { get; set; }
    public SimpleObjectWithNestedObjectAndDeny()
    {
        NestedObject = new NestedObject();
        SimpleObject = new SimpleObject();
        InheritedObject = new InheritedObject();
        SimpleObjectList = new List<SimpleObject>();
        Allow = new SimpleObjectWithDeny();
    }
}
