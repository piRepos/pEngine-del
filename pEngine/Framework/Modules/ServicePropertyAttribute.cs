using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEngine.Framework.Modules
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ServicePropertyAttribute : Attribute
	{
		/// <summary>
		/// This property references to this module property.
		/// </summary>
		public string ReferencesTo { get; }

		/// <summary>
		/// Makes a new instance of <see cref="ServicePropertyAttribute"/> class.
		/// </summary>
		/// <param name="referencesTo">Module property name.</param>
		public ServicePropertyAttribute(string referencesTo)
		{
			ReferencesTo = referencesTo;
		}

	}
}
