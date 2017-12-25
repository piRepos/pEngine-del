using System;

namespace pEngine.Utils.Math
{
	/// <summary>
	/// An object that allow to modify the size.
	/// </summary>
	public interface IScalablef : ISizedf
	{

		/// <summary>
		/// Gets or sets the size.
		/// </summary>
		new Vector2 Size { get; set; }

	}
}
