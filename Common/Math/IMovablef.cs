﻿using System;

namespace pEngine.Common.Math
{
	/// <summary>
	/// An object that can be moved.
	/// </summary>
	public interface IMovablef : ISpacedf
	{
		
		/// <summary>
		/// Gets or sets the position.
		/// </summary>
		new Vector2 Position { get; set; }

	}
}
