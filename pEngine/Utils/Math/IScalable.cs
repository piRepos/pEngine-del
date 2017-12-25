using System;

namespace pEngine.Utils.Math
{
	/// <summary>
	/// An object that allow to modify the size.
	/// </summary>
	public interface IScalable : ISized
	{

		/// <summary>
		/// Gets or sets the size.
		/// </summary>
		new Vector2i Size { get; set; }

	}
}
