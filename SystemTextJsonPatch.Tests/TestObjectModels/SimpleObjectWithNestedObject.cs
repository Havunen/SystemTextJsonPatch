using System.Collections.Generic;

namespace SystemTextJsonPatch;

public class SimpleObjectWithNestedObject
{
    public int IntegerValue { get; set; }

    public NestedObject NestedObject { get; set; }

    public SimpleObject SimpleObject { get; set; }

    public InheritedObject InheritedObject { get; set; }

    public List<SimpleObject> SimpleObjectList { get; set; }

    public IList<SimpleObject> SimpleObjectIList { get; set; }

    public SimpleObjectWithNestedObject()
    {
        NestedObject = new NestedObject();
        SimpleObject = new SimpleObject();
        InheritedObject = new InheritedObject();
        SimpleObjectList = new List<SimpleObject>();
    }
}
