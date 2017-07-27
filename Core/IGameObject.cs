using System;
using System.Collections.Generic;

using pEngine.Common.Timing.Base;

using pEngine.Core.Graphics.Renderer;

namespace pEngine.Core
{
	public interface IGameObject
	{
		/// <summary>
		/// Update this object physics.
		/// </summary>
		/// <param name="clock">Gameloop clock.</param>
		void Update(IFrameBasedClock clock);

		/// <summary>
		/// Invalidate a property of this object/tree.
		/// </summary>
		/// <param name="type">Property to invalidate.</param>
		/// <param name="propagation">Propagation direction.</param>
		/// <param name="sender">Object sender.</param>
		void Invalidate(InvalidationType type, InvalidationDirection propagation, IGameObject sender);

		/// <summary>
		/// Calculate the assets which can be rendered.
		/// </summary>
		/// <returns>The assets.</returns>
		IEnumerable<Asset> GetAssets();
	}

	[Flags]
	public enum InvalidationType
	{
		/// <summary>
		/// No invalidation.
		/// </summary>
		None = 0,

		/// <summary>
		/// This invalidation specifies that a 
		/// node was added/removed.
		/// </summary>
		Tree = 1 << 0,

		/// <summary>
		/// This invalidation specifies that the 
		/// posizion/rotation/size is changed.
		/// </summary>
		Geometry = 1 << 1,

		/// <summary>
		/// This invalidation specifies that the
		/// color is changed and vertex needs to be modify.
		/// </summary>
		Color = 1 << 2,

		/// <summary>
		/// This invalidation specifies that the
		/// assets are changed from children nodes.
		/// </summary>
		Assets = 1 << 3,

		/// <summary>
		/// If the object is frame buffered and a child is
		/// changed, this invalidation will be thrown.
		/// </summary>
		Buffer = 1 << 4,

		/// <summary>
		/// All invalidations.
		/// </summary>
		All = Tree | Geometry | Color | Buffer | Assets
	}

	[Flags]
	public enum InvalidationDirection
	{
		/// <summary>
		/// No direction.
		/// </summary>
		None = 0,

		/// <summary>
		/// Invalidation will go on the parent to the root.
		/// </summary>
		Parent = 1 << 0,

		/// <summary>
		/// Invalidation will go in all leafs.
		/// </summary>
		Children = 1 << 1,

		/// <summary>
		/// Invalidation will go in all directions.
		/// </summary>
		Both = Parent | Children
	}
}
