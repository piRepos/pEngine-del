using System;
using System.Linq;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

using pEngine.Framework;
using pEngine.Resources;
using pEngine.Graphics.Renderer;
using System.Collections.Generic;
using pEngine.Utils.Timing.Base;

namespace pEngine.Graphics.Containers
{
	public abstract class Container<T> : Drawable
		where T : Drawable
	{
		/// <summary>
		/// Makes a new instance of <see cref="Container"/> class.
		/// </summary>
		/// <param name="parent">Parent object.</param>
		public Container()
		{

		}

		#region Children management

		/// <summary>
		/// Container childrens.
		/// </summary>
		protected ObservableCollection<T> Children { get; }

		private void ChindrenChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			// - Set new object's parent.
			if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
			{
				foreach (GameObject obj in e.NewItems)
				{
					if (obj == null)
						throw new NullReferenceException("Null children are not allowed.");

					obj.Parent = this;

					if (State == ResourceState.Loaded)
						Invalidate("Graphics", InvalidationDirection.Parent);
				}
			}
		}

		#endregion

		#region Invalidation

		/// <summary>
		/// Invalidate a property of this object/tree.
		/// </summary>
		/// <param name="sender">Object sender.</param>
		/// <param name="channel">Channel to invalidate.</param>
		/// <param name="direction">Propagation direction.</param>
		public override void Invalidate(GameObject sender, string channel, InvalidationDirection direction)
		{
			base.Invalidate(sender, channel, direction);

			if (direction.HasFlag(InvalidationDirection.Children))
			{
				foreach (Drawable obj in Children)
					obj.Invalidate(sender, channel, InvalidationDirection.Children);
			}
		}

		#endregion

		#region Assets calculation

		/// <summary>
		/// Calculate the assets which can be rendered.
		/// </summary>
		public override IEnumerable<Asset> GetAssets()
		{
			List<Asset> assets = new List<Asset>();

			// - Current object assets
			assets.AddRange(base.GetAssets());

			// - Gets all children assets
			foreach (Drawable obj in Children)
				assets.AddRange(obj.GetAssets());

			return assets;
		}

		#endregion

		#region Update

		/// <summary>
		/// Update the state of this element.
		/// </summary>
		/// <param name="DeltaTime">Game clock.</param>
		public override void Update(IFrameBasedClock clock)
		{
			base.Update(clock);

			foreach (Drawable obj in Children)
			{
				obj.Update(clock);
			}
		}

		#endregion
	}

	public abstract class Container : Container<Drawable>
	{
		/// <summary>
		/// Makes a new instance of <see cref="Container"/> class.
		/// </summary>
		/// <param name="parent">Parent object.</param>
		public Container()
		{
			
		}

		/// <summary>
		/// Sprite batch childrens.
		/// </summary>
		public new ObservableCollection<Drawable> Children => base.Children;
	}
}
