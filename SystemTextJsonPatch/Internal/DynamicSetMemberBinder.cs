using System;
using System.Dynamic;

namespace SystemTextJsonPatch.Internal
{
	internal class DynamicSetMemberBinder : SetMemberBinder
	{
		public DynamicSetMemberBinder(string name, bool ignoreCase) : base(name, ignoreCase)
		{
		}

		public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject? errorSuggestion)
		{
			throw new InvalidOperationException(typeof(DynamicSetMemberBinder).FullName + ".FallbackGetMember");
		}
	}
}
