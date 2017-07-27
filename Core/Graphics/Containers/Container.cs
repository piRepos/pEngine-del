using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

using pEngine.Common.Timing.Base;
using pEngine.Core.Graphics.Renderer;

namespace pEngine.Core.Graphics.Containers
{

	public abstract class Container : Container<Drawable>
	{
		/// <summary>
		/// Sprite batch childrens.
		/// </summary>
		public new ObservableCollection<Drawable> Children => base.Children;
	}

	public abstract class Container<Type> : Drawable 
		where Type : Drawable
    {

		/// <summary>
		/// Makes a new instance of <see cref="Container{Type}"/> class.
		/// </summary>
		public Container()
			: base()
		{
			Children = new ObservableCollection<Type>();
			Children.CollectionChanged += ChindrenChanged;
		}

		#region Children management

		/// <summary>
		/// Container childrens.
		/// </summary>
		protected ObservableCollection<Type> Children { get; }

		private void ChindrenChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			// - Set new object's parent.
			if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
			{
				foreach (Drawable obj in e.NewItems)
				{
					if (obj == null)
						throw new NullReferenceException("Null children are not allowed.");

					obj.Parent = this;
					PerformanceMonitor.AddCollector(obj.PerformanceMonitor);

					if (State == LoadState.Loaded)
						Invalidate(InvalidationType.Assets, InvalidationDirection.Parent, this);
				}
			}
		}

		#endregion

		#region Invalidation

		/// <summary>
		/// Invalidate a property of this object/tree.
		/// </summary>
		/// <param name="type">Property to invalidate.</param>
		/// <param name="propagation">Propagation direction.</param>
		/// <param name="sender">Object sender.</param>
		public override void Invalidate(InvalidationType type, InvalidationDirection propagation, IGameObject sender)
		{
			base.Invalidate(type, propagation, sender);

			if (propagation.HasFlag(InvalidationDirection.Children))
			{
				foreach (Drawable obj in Children)
					obj.Invalidate(type, InvalidationDirection.Children, sender);
			}
		}

		#endregion

		/// <summary>
		/// Update this object physics.
		/// </summary>
		/// <param name="clock">Gameloop clock.</param>
		public override void Update(IFrameBasedClock clock)
		{
			base.Update(clock);

			foreach (Drawable obj in Children)
			{
				obj.Update(clock);
			}
		}

		/// <summary>
		/// Calculate the assets which can be rendered.
		/// </summary>
		/// <returns>The assets.</returns>
		protected override List<Asset> CalculateAssets()
		{
			List<Asset> children = base.CalculateAssets();

			foreach (Drawable obj in Children)
			{
				if (obj.State == LoadState.Loaded)
					children.AddRange(obj.GetAssets());
			}

			return children;
		}
	}
}