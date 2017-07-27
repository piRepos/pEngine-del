using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.Math;

namespace pEngine.Core.Physics.Geometry
{
    public interface IPoint
    {

		/// <summary>
		/// Point position.
		/// </summary>
		Vector2 Position { get; set; }

	}
}
