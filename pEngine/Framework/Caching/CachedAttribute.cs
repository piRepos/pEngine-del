using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using MethodBoundaryAspect.Fody.Attributes;

namespace pEngine.Framework.Caching
{
	public class CachedAttribute : OnMethodBoundaryAspect
	{
		/// <summary>
		/// Invalidation channel.
		/// </summary>
		public string Channel { get; set; }

		public override void OnEntry(MethodExecutionArgs arg)
		{
			pObject obj = arg.Instance as pObject;
			DictionaryCache cache = obj.Cache;

			if (!cache.IsCached(arg.Method))
				arg.ExecuteBody = true;
		}

		public override void OnExit(MethodExecutionArgs arg)
		{
			pObject obj = arg.Instance as pObject;
			DictionaryCache cache = obj.Cache;

			if (arg.ExecuteBody)
				cache.AddToCache(arg.Method, Channel, arg.ReturnValue);
			else
				arg.ReturnValue = cache.GetValue(arg.Method);
		}

	}
}
