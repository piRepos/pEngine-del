using System;

namespace pEngine.Utils.Math
{
	/// <summary>
	/// An element with a position.
	/// </summary>
	public interface ISpaced
	{

		/// <summary>
		/// Gets the element position.
		/// </summary>
		Vector2i Position { get; }

	}
}
