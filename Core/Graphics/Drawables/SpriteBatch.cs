using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using pEngine.Common.Timing.Base;
using pEngine.Common.DataModel;
using pEngine.Common.Math;
using pEngine.Common;

using pEngine.Core.Graphics.Renderer.Batches;
using pEngine.Core.Graphics.Containers;
using pEngine.Core.Graphics.Renderer;
using pEngine.Core.Graphics.Textures;
using pEngine.Core.Graphics.Shading;

namespace pEngine.Core.Graphics.Drawables
{
	public class ManagedSprite : Drawable
	{
		/// <summary>
		/// Makes a new instance of <see cref="ManagedSprite"/> class.
		/// </summary>
		/// <param name="texture">Texture part for this sprite.</param>
		public ManagedSprite(ITexture texture)
		{
			Texture = texture;

			PropertyChanged += PropertyChange;
		}

		/// <summary>
		/// Current sprite texture.
		/// </summary>
		public ITexture Texture { get; set; }

		/// <summary>
		/// Internal layout offset.
		/// </summary>
		public Vector2i InternalOffset { get; set; }

		/// <summary>
		/// Sprite blend color.
		/// </summary>
		public Color4 BlendColor { get; set; }

		/// <summary>
		/// True if a property of this sprite is changed.
		/// </summary>
		internal bool Modified { get; set; }

		/// <summary>
		/// Offset in vertex heap.
		/// </summary>
		internal int Offset { get; set; }

		private void PropertyChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Modified = true;
		}

		public override void Update(IFrameBasedClock clock)
		{
			base.Update(clock);
		}
	}

	public class SpriteBatch : Container<ManagedSprite>
	{

		/// <summary>
		/// Sprite batch childrens.
		/// </summary>
		public new ObservableCollection<ManagedSprite> Children => base.Children;

	}


	public class SpriteBatch<Type> : Container<Type>
		where Type : ManagedSprite
	{

		/// <summary>
		/// Makes a new instance of <see cref="SpriteBatch{Type}"/> class.
		/// </summary>
		/// <param name="textureSource">Parent texture for managed sprites.</param>
		public SpriteBatch(ITexture textureSource)
		{
			TextureSource = textureSource;
			Children.CollectionChanged += ChildrenChanged;
		}

		/// <summary>
		/// Current sprite texture.
		/// </summary>
		public ITexture TextureSource { get; set; }

		#region Vertex management

		protected QuadVertexBatch Vertexs { get; private set; }

		[LoaderFunction]
		private void Load(BatchesStore Batches)
		{
			Vertexs = Batches.GetBatch<QuadVertexBatch>(1);

			TextureSource.PropertyChanged += (obj, e) =>
			{
				foreach (Type c in Children)
					c.Modified = true;
			};
		}

		private void ChildrenChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:

					foreach (Type s in e.NewItems)
					{
						if ((s.Texture is DependencyTexture && (s.Texture as DependencyTexture).Reference != TextureSource)
							|| s.Texture is StandaloneTexture && (s.Texture as StandaloneTexture) != TextureSource)
							throw new InvalidOperationException("The ManagedSprite added references to another parent texture.");

						int firstAvaiable = Enumerable.Range(0, int.MaxValue).Except(Children.Select((x) => x.Offset)).FirstOrDefault();

						s.Offset = firstAvaiable;
						s.Modified = true;
					}

					break;
			}

		}

		public override void Update(IFrameBasedClock clock)
		{
			base.Update(clock);

			// TODO: Sort items for zIndex.
			//Children = new ObservableCollection<Type>(Children.OrderBy((x) => x.zIndex));

			int maxIndex = (Children.Count == 0 ? 0 : Children.Max(x => x.Offset)) + 1;
			Vertexs.Resize(maxIndex);

			foreach (Type s in Children)
			{
				if (s.Modified)
					UpdateSpriteValues(s);
			}
		}

		protected virtual void UpdateSpriteValues(Type s)
		{
			Vector2 pos = s.Position / (Vector2)ScaledSize;
			Vector2 siz = s.ScaledSize / (Vector2)ScaledSize;

			Matrix Matrix = Matrix.Identity;

			// - Apply rotation to the matrix
			{
				Vector2 rotationCenter = -ComputeAnchorPosition(s.ScaledSize, s.RotationCustomCenter, s.RotationCenter);

				Matrix *= Matrix.CreateTranslation(rotationCenter.X, rotationCenter.Y, 0);
				Matrix *= Matrix.CreateRotationZ(s.Rotation);
				Matrix *= Matrix.CreateTranslation(-rotationCenter.X, -rotationCenter.Y, 0);
			}

			// - Create translation to requests point
			{
				Vector2 finalPosition = (s.Position + s.InternalOffset);
				if (s.Parent != null)
				{
					// - Apply origin point
					finalPosition *= ComputeVectorDir(s.Anchor, s.Origin);
					finalPosition += ComputeAnchorPosition(ScaledSize, s.CustomOrigin, s.Origin);
				}

				// - Apply anchor point
				finalPosition -= ComputeAnchorPosition(s.ScaledSize, s.CustomAnchor, s.Anchor);

				// - Calculate matrix
				Matrix *= Matrix.CreateTranslation(finalPosition.X, finalPosition.Y, 0);
			}

			// - Scale with parent
			if (s.ScaleWithParent)
			{
				Vector2 scaleReference = ScaleReference;
				Vector2 parentScaledSize = ScaledSize;
				Vector2 scaleFactor = parentScaledSize / scaleReference;
				Matrix *= Matrix.CreateScale(scaleFactor.X, scaleFactor.Y, 0);
			}

			Matrix dm = Matrix.CreateScale(s.ScaledSize.Width, s.ScaledSize.Height, 0) * Matrix;

			Vector2 p1 = Vector2.Zero * dm;
			Vector2 p2 = new Vector2(1, 0) * dm;
			Vector2 p3 = Vector2.One * dm;
			Vector2 p4 = new Vector2(0, 1) * dm;

			int current = s.Offset;

			Vertexs.Vertexs[(current * 4) + 0] = new GLVertex
			{
				X = p1.X / (float)ScaledSize.Width,
				Y = p1.Y / (float)ScaledSize.Height,
				R = s.BlendColor.Rf,
				G = s.BlendColor.Gf,
				B = s.BlendColor.Bf,
				A = s.Visible ? (float)(s.Opacity * s.BlendColor.Af) : 0,
				Tx = s.Texture.TopLeft.X,
				Ty = s.Texture.TopLeft.Y
			};

			Vertexs.Vertexs[(current * 4) + 1] = new GLVertex
			{
				X = p2.X / (float)ScaledSize.Width,
				Y = p2.Y / (float)ScaledSize.Height,
				R = s.BlendColor.Rf,
				G = s.BlendColor.Gf,
				B = s.BlendColor.Bf,
				A = s.Visible ? (float)(s.Opacity * s.BlendColor.Af) : 0,
				Tx = s.Texture.TopRight.X,
				Ty = s.Texture.TopRight.Y
			};

			Vertexs.Vertexs[(current * 4) + 2] = new GLVertex
			{
				X = p3.X / (float)ScaledSize.Width,
				Y = p3.Y / (float)ScaledSize.Height,
				R = s.BlendColor.Rf,
				G = s.BlendColor.Gf,
				B = s.BlendColor.Bf,
				A = s.Visible ? (float)(s.Opacity * s.BlendColor.Af) : 0,
				Tx = s.Texture.BotRight.X,
				Ty = s.Texture.BotRight.Y
			};

			Vertexs.Vertexs[(current * 4) + 3] = new GLVertex
			{
				X = p4.X / (float)ScaledSize.Width,
				Y = p4.Y / (float)ScaledSize.Height,
				R = s.BlendColor.Rf,
				G = s.BlendColor.Gf,
				B = s.BlendColor.Bf,
				A = s.Visible ? (float)(s.Opacity * s.BlendColor.Af) : 0,
				Tx = s.Texture.BotLeft.X,
				Ty = s.Texture.BotLeft.Y
			};

			Vertexs.Invalidated = true;
			Vertexs.InvalidationType |= BatchInvalidationType.Vertexs;

			s.Modified = false;
		}

		#endregion

		#region Assets calculation

		public override IEnumerable<Asset> GetAssets()
		{
			List<Asset> assets = new List<Asset>();

			assets.Add(new Asset
			{
				Transformation = DrawMatrix,
				Shader = new SpriteShader
				{
					Opacity = 1,
					Brightness = 0,
					Contrast = 1,
					TextureAttachment = 0
				},
				Textures = new KeyValuePair<int, long>[]
				{
					new KeyValuePair<int, long>(0, TextureSource.DependencyID)
				},
				Elements = new DrawElement[]
				{
					new DrawElement
					{
						ElementOffset = (int)Vertexs.Indexes.Offset,
						ElementSize = (int)Vertexs.Indexes.Size,
						Primitive = Vertexs.Primitive,
					}
				},
				AlphaBlendingDst = OpenGL.BlendingFactor.OneMinusSrcAlpha,
				AlphaBlendingSrc = OpenGL.BlendingFactor.SrcAlpha,
				ColorBlendingDst = OpenGL.BlendingFactor.OneMinusSrcAlpha,
				ColorBlendingSrc = OpenGL.BlendingFactor.SrcAlpha,
			});

			return assets;
		}

		#endregion
	}
}
