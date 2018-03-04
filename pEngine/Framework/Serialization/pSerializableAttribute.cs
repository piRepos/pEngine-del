using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEngine.Framework.Serialization
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class pSerializableAttribute : Attribute
	{
		/// <summary>
		/// Makes a new instance of <see cref="pSerializableAttribute"/> class.
		/// </summary>
		public pSerializableAttribute()
		{

		}
	}
}
