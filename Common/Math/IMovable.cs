using System;

namespace pEngine.Common.Math
{
	/// <summary>
	/// An object that can be moved.
	/// </summary>
	public interface IMovable : ISpaced
	{
		
		/// <summary>
		/// Gets or sets the position.
		/// </summary>
		new Vector2i Position { get; set; }

	}
}
