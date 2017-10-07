using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

using pEngine.Common.Timing.Base;
using pEngine.Core.Graphics.Renderer;
using pEngine.Core.Graphics.Buffering;
using pEngine.Core.Graphics.Renderer.Clipping;
using pEngine.Core.Graphics.Renderer.Batches;
using pEngine.Core.Graphics.Shading;
using pEngine.Core.Graphics.Textures;

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
			if (State == LoadState.Loaded && type.HasFlag(InvalidationType.Buffer) && VideoBuffer.VideoBuffer != null)
			{
				VideoBuffer.VideoBuffer.Size = Host.Window.BufferSize;
				VideoBuffer.VideoBuffer.InvalidateDependency();
				VideoBuffer.Invalidate(Host.PhysicsLoop.FrameId);
			}

			if (State == LoadState.Loaded && type.HasFlag(InvalidationType.Color))
				VideoBuffer.Invalidate(Host.PhysicsLoop.FrameId);
			
			base.Invalidate(type, propagation, sender);

			if (propagation.HasFlag(InvalidationDirection.Children))
			{
				foreach (Drawable obj in Children)
					obj.Invalidate(type, InvalidationDirection.Children, sender);
			}
		}

		#endregion

		#region Assets calculation

		/// <summary>
		/// Calculate the assets which can be rendered.
		/// </summary>
		/// <returns>The assets.</returns>
		protected override List<Asset> CalculateAssets()
		{
			List<Asset> assetCache = new List<Asset>();

			if (VideoBuffer.NeedsRedraw(Host.LastRenderedFrame))
			{
				foreach (Drawable obj in Children)
				{
					if (obj.State == LoadState.Loaded)
						assetCache.AddRange(obj.GetAssets());
				}
				
				Invalidate(InvalidationType.Assets, InvalidationDirection.Parent, this);
			}

			if (VideoBuffer.Enabled)
			{
				int dog = assetCache.Any(x => x.Shader is MaskShader) ? 1 : 0;
				for (int i = 0; i < assetCache.Count - dog; ++i)
				{
					if (assetCache[i].TargetID < 0)
					{
						Asset curr = assetCache[i];
						curr.TargetID = VideoBuffer.VideoBuffer.DependencyID;
						assetCache[i] = curr;
					}
				}

				if (VideoBuffer.Draw)
				{
					assetCache.Add(new Asset
					{
						Transformation = Common.Math.Matrix.CreateScale(Host.Window.BufferSize.Width, Host.Window.BufferSize.Height, 0) * ToRelativeFloat,
						Shader = new FrameBufferShader
						{
							TextureAttachment = 0
						},
						Textures = new KeyValuePair<int, long>[]
						{
								new KeyValuePair<int, long>(0, VideoBuffer.Texture.DependencyID)
						},
						Elements = new DrawElement[]
						{
								new DrawElement
								{
									ElementOffset = (int)FullQuad.Indexes.Offset,
									ElementSize = (int)FullQuad.Indexes.Size,
									Primitive = FullQuad.Primitive,
								}
						},
						AlphaBlendingDst = OpenGL.BlendingFactor.OneMinusSrcAlpha,
						AlphaBlendingSrc = OpenGL.BlendingFactor.SrcAlpha,
						ColorBlendingDst = OpenGL.BlendingFactor.OneMinusSrcAlpha,
						ColorBlendingSrc = OpenGL.BlendingFactor.One,
						TargetID = -1
					});
				}
			}

			return assetCache;
		}

		#endregion

		#region Update
		
		private bool wasBuffered;

		/// <summary>
		/// Update this object physics.
		/// </summary>
		/// <param name="clock">Gameloop clock.</param>
		public override void Update(IFrameBasedClock clock)
		{
			base.Update(clock);

			if (wasBuffered != VideoBuffer.Enabled)
			{
				Invalidate(InvalidationType.Assets, InvalidationDirection.Parent, this);
				wasBuffered = VideoBuffer.Enabled;
			}

			if (VideoBuffer.VideoBuffer != null && VideoBuffer.VideoBuffer.State != Data.FrameDependency.DependencyState.Loaded)
			{
				VideoBuffer.Invalidate(Host.PhysicsLoop.FrameId);
			}

			if (VideoBuffer.Enabled && VideoBuffer.VideoBuffer == null)
			{
				VideoBufferSettings s = VideoBuffer;
				s.VideoBuffer = Host.VideoBuffers.GetVideoBuffer(Host.Window.BufferSize);
				VideoBuffer = s;
			}

			foreach (Drawable obj in Children)
			{
				obj.Update(clock);
			}
		}

		#endregion

		#region Frame buffer

		/// <summary>
		/// Video framebuffer settings.
		/// </summary>
		public virtual VideoBufferSettings VideoBuffer { get; set; }

		/// <summary>
		/// Sets buffering for this object enabled or disabled.
		/// </summary>
		public bool FrameBuffered => VideoBuffer.Enabled;

		/// <summary>
		/// Object texture (if the property FrameBuffered is false this property is null).
		/// </summary>
		public ITexture ObjectTexture => VideoBuffer.Texture;

		#endregion
	}
}