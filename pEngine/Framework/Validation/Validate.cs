using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEngine.Framework.Validation
{
	/// <summary>
	/// Validation for properties, it automatically checks if a
	/// property respect the predicate.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class Validate : Attribute
	{
		/// <summary>
		/// Validation predicate.
		/// </summary>
		public Predicate<object> Predicate { get; set; }

		/// <summary>
		/// Exception on validation fails.
		/// </summary>
		public Exception Error { get; set; }

		/// <summary>
		/// Makes a new instance of <see cref="Validate"/> class.
		/// </summary>
		public Validate()
		{
			Predicate = (x) => true;
			Error = new FormatException("This property's validation fails");
		}
	}
}
