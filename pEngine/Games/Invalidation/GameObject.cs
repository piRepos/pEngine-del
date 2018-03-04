using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEngine.Games
{
	/// <summary>
	/// Base class for all objects used in a game.
	/// </summary>
	public abstract partial class GameObject
	{

		private void initializeCache()
		{

		}

		private void disposeCache()
		{

		}

		/// <summary>
		/// Start an invalidation call from this object on the GameTree channel.
		/// </summary>
		public void InvalidateTree()
		{
			Invalidate(this, "GameTree", InvalidationDirection.Parent);
		}

		/// <summary>
		/// Start an invalidation call from this object on the specified channel.
		/// </summary>
		/// <param name="sender">Who call this invalidation.</param>
		/// <param name="channel">Channel to invalidate.</param>
		public void Invalidate(string channel, InvalidationDirection direction)
		{
			Invalidate(this, channel, direction);
		}

		/// <summary>
		/// Start an invalidation call from this object on the specified channel.
		/// </summary>
		/// <param name="sender">Who call this invalidation.</param>
		/// <param name="channel">Channel to invalidate.</param>
		/// <param name="direction">Propagation direction.</param>
		public virtual void Invalidate(GameObject sender, string channel, InvalidationDirection direction)
		{
			bool propagate = OnInvalidation(sender, channel, direction);

			if (direction.HasFlag(InvalidationDirection.Parent) && Parent != null)
				Parent.Invalidate(sender, channel, direction);
		}

		#region Overridable invalidation

		/// <summary>
		/// Called on object invalidation.
		/// </summary>
		/// <param name="sender">Who call this invalidation.</param>
		/// <param name="channel">Channel invalidated.</param>
		/// <param name="direction">Propagation direction.</param>
		/// <returns>True if invalidation is full handled and propagation must stop.</returns>
		protected virtual bool OnInvalidation(GameObject sender, string channel, InvalidationDirection direction)
		{
			Cache.Invalidate(channel);
			return false;
		}

		#endregion

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
