using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEngine.Framework.Validation
{
	public class ValidateRange : Validate
	{
		/// <summary>
		/// Minimum value of the range.
		/// </summary>
		public float Min { get; set; } = float.MinValue;

		/// <summary>
		/// Maximum value of the range.
		/// </summary>
		public float Max { get; set; } = float.MaxValue;

		public ValidateRange()
		{
			Predicate = (x) =>
			(float)(Convert.ChangeType(x, typeof(float))) >= Min && (float)(Convert.ChangeType(x, typeof(float))) <= Max;
			Error = new FormatException($"The value of this property bust be between {Min} and {Max}.");
		}

	}
}
