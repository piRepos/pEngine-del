using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using pEngine.Common.Timing.Base;
using pEngine.Common.DataModel;

using pEngine.Core.Graphics.Buffering;
using pEngine.Core.Graphics.Renderer.Clipping;
using pEngine.Core.Graphics.Textures;
using pEngine.Core.Graphics.Renderer;
using pEngine.Core.Graphics.Shading;
using pEngine.Core.Graphics.Renderer.Batches;
using pEngine.Core.Physics;

using pEngine.Core.Graphics.Drawables;
using pEngine.Core.Graphics.Containers;

using pEngine.Core.Graphics.Renderer.Shading;

namespace pEngine.Core.Graphics
{
    public class Drawable : Movable
    {
		/// <summary>
		/// Makes a new instance of <see cref="Drawable"/>.
		/// </summary>
		public Drawable()
			: base()
		{

			invalidationId = long.MaxValue;

			Visible = true;
			Opacity = 1;
		}

		#region Initialization

		[LoaderFunction]
		private void Load(BatchesStore batches)
		{
			FullQuad = batches.DefaultQuad;
		}

		protected QuadVertexBatch FullQuad;

		#endregion

		#region Visibility

		/// <summary>
		/// Gets or sets the clipping settings.
		/// </summary>
		public ClippingInformations Clipping { get; set; }

		/// <summary>
		/// Gets or sets if this object is visible.
		/// </summary>
		public bool Visible { get; set; }

		/// <summary>
		/// Gets or sets the object opacity.
		/// </summary>
		public double Opacity { get; set; }

		/// <summary>
		/// Gets or sets the depth order relative 
		/// to parent's children
		/// </summary>
		public int zIndex { get; set; }

		#endregion

		#region Assets

		private List<Asset> assetCache;

		/// <summary>
		/// Calculate the assets which can be rendered.
		/// </summary>
		/// <returns>The assets.</returns>
		public override IEnumerable<Asset> GetAssets()
		{
			if (assetCache == null)
			{
				assetCache = new List<Asset>();

				if (LastInvalidatedFrame >= Host.LastRenderedFrame || !FrameBuffered || BypassBuffer)
				{
					assetCache.AddRange(CalculateAssets());

					if (LastInvalidatedFrame >= Host.LastRenderedFrame && FrameBuffered)
						Invalidate(InvalidationType.Assets, InvalidationDirection.Parent, this);
				}

				if (FrameBuffered)
				{
                    int dog = assetCache.Any(x => x.Shader is MaskShader) ? 1 : 0;
					for (int i = 0; i < assetCache.Count - dog; ++i)
					{
						if (assetCache[i].TargetID < 0)
						{
							Asset curr = assetCache[i];
							curr.TargetID = VideoBuffer.DependencyID;
							assetCache[i] = curr;
						}
					}

                    if (!BypassBuffer)
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
                                new KeyValuePair<int, long>(0, VideoBuffer.TargetTexture.DependencyID)
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
			}

			return assetCache;
		}

		protected virtual List<Asset> CalculateAssets()
		{
			return new List<Asset>();
		}

		#endregion

		#region Update logic

		private long invalidationId;
		private bool wasBuffered;

		/// <summary>
		/// Update this object physics.
		/// </summary>
		/// <param name="clock">Gameloop clock.</param>
		public override void Update(IFrameBasedClock clock)
		{
			base.Update(clock);


			if (State == LoadState.Loaded && Host.PhysicsLoop.FrameId > invalidationId)
			{
				assetCache = null;
				invalidationId = long.MaxValue;
			}

			if (wasBuffered != FrameBuffered)
			{
				Invalidate(InvalidationType.Assets, InvalidationDirection.Parent, this);
				wasBuffered = FrameBuffered;
			}

			if (VideoBuffer != null && VideoBuffer.State != Data.FrameDependency.DependencyState.Loaded)
			{
				LastInvalidatedFrame = Host.PhysicsLoop.FrameId;
			}

			if (FrameBuffered && VideoBuffer == null)
			{
				VideoBuffer = Host.VideoBuffers.GetVideoBuffer(Host.Window.BufferSize);
			}
		}

		/// <summary>
		/// Invalidate a property of this object/tree.
		/// </summary>
		/// <param name="type">Property to invalidate.</param>
		/// <param name="propagation">Propagation direction.</param>
		/// <param name="sender">Object sender.</param>
		public override void Invalidate(InvalidationType type, InvalidationDirection propagation, IGameObject sender)
		{
			if (State == LoadState.Loaded && type.HasFlag(InvalidationType.Buffer) && VideoBuffer != null)
			{
				VideoBuffer.Size = Host.Window.BufferSize;
				VideoBuffer.InvalidateDependency();
			}

			if (State == LoadState.Loaded && type.HasFlag(InvalidationType.Color))
				LastInvalidatedFrame = Host.PhysicsLoop.FrameId;

			if (State == LoadState.Loaded && type.HasFlag(InvalidationType.Assets) && invalidationId == long.MaxValue)
				invalidationId = Host.PhysicsLoop.FrameId;


			base.Invalidate(type, propagation, sender);
		}

		#endregion

		#region Loading

		/// <summary>
		/// Loads the game object async.
		/// </summary>
		/// <returns>The loader task.</returns>
		/// <param name="callback">Callback to call when loading is done.</param>
		public Drawable LoadAsync(Game game, Action callback)
		{
			return LoadAsync<Drawable>(game, callback);
		}

		/// <summary>
		/// Loads the game object sync.
		/// </summary>
		public Drawable Load(Game game)
		{
			return Load<Drawable>(game);
		}

		#endregion

		#region Frame Buffering

		/// <summary>
		/// If true this object's pixels will stored in a
		/// frame buffer, and will be renderized the buffer.
		/// </summary>
        public virtual bool FrameBuffered { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:pEngine.Core.Graphics.Drawable"/> dont draw buffer.
        /// </summary>
        /// <value><c>true</c> if dont draw buffer; otherwise, <c>false</c>.</value>
        protected virtual bool BypassBuffer { get; set; }

		/// <summary>
		/// Object video buffer (if the property FrameBuffered is false this property is null).
		/// </summary>
		public VideoBuffer VideoBuffer { get; private set; }

		/// <summary>
		/// Object texture (if the property FrameBuffered is false this property is null).
		/// </summary>
		public ITexture ObjectTexture => VideoBuffer?.TargetTexture;

		/// <summary>
		/// Id of the last frame invalidation.
		/// </summary>
		protected long LastInvalidatedFrame { get; private set; }

		#endregion

	}

}
