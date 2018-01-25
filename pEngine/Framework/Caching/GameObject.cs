using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEngine.Framework
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
		/// 
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="direction"></param>
		public void Invalidate(string channel, InvalidationDirection direction)
		{
			if (direction.HasFlag(InvalidationDirection.Parent))
				Invalidate(this, channel, InvalidationDirection.Parent);

			if (direction.HasFlag(InvalidationDirection.Children))
				Invalidate(this, channel, InvalidationDirection.Children);
		}

		#region Overridable invalidation

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="channel"></param>
		/// <param name="direction"></param>
		/// <returns></returns>
		protected virtual bool OnInvalidation(GameObject sender, string channel, InvalidationDirection direction)
		{
			Cache.Invalidate(channel);
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="channel"></param>
		/// <param name="direction"></param>
		protected virtual void Invalidate(GameObject sender, string channel, InvalidationDirection direction)
		{
			bool propagate = OnInvalidation(sender, channel, direction);

			if (direction.HasFlag(InvalidationDirection.Parent) && Parent != null)
				Parent.Invalidate(sender, channel, direction);
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
