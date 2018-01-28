using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using MethodBoundaryAspect.Fody.Attributes;

namespace pEngine.Framework.Modules
{
	public class ServiceMethodAttribute : OnMethodBoundaryAspect
	{
		/// <summary>
		/// This property references to this module property.
		/// </summary>
		public string ReferencesTo { get; set; }

		/// <summary>
		/// Method info cache.
		/// </summary>
		private MethodInfo Method { get; set; }

		public override void OnExit(MethodExecutionArgs arg)
		{
			Service obj = arg.Instance as Service;

			if (Method == null)
			{
				var type = obj.Module.GetType();
				Method = type.GetMethod(ReferencesTo);
			}

			obj.Module.Scheduler.Add(() =>
			{
				if (arg.Arguments.Last() is Action<object>)
				{
					var fun = arg.Arguments.Last() as Action<object>;

					fun(Method.Invoke(obj.Module, arg.Arguments.Take(arg.Arguments.Length - 1).ToArray()));
				}
				else Method.Invoke(obj.Module, arg.Arguments);
			});
		}

	}
}
