using System;
using System.Dynamic;

namespace SystemTextJsonPatch.Internal
{
    public class DynamicGetMemberBinder : GetMemberBinder
    {
        public DynamicGetMemberBinder(string name, bool ignoreCase) : base(name, ignoreCase)
        {
        }

        public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject? errorSuggestion)
        {
            throw new InvalidOperationException(typeof(DynamicGetMemberBinder).FullName + ".FallbackGetMember");
        }
    }
}
