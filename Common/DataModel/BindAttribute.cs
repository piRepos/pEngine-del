using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Common.DataModel
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    class BindAttribute : Attribute
    {
		/// <summary>
		/// Makes a new instance of attribute <see cref="Binding"/>.
		/// </summary>
		public BindAttribute()
		{
		}


    }

	/// <summary>
	/// Binding function delegate.
	/// </summary>
	/// <param name="input">Input value from binding.</param>
	/// <returns>Changed value.</returns>
	public delegate object BindingFunction(object input);
	
	[Flags]
	public enum BindingDirection
	{
		/// <summary>
		/// This property will receive changes.
		/// </summary>
		In = 1 << 0,

		/// <summary>
		/// This property will send changes.
		/// </summary>
		Out = 1 << 1,

		/// <summary>
		/// This property will send and receive changes.
		/// </summary>
		Both = In | Out
	}
}
