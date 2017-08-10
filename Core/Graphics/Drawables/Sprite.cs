using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.DataModel;
using pEngine.Common.Math;

using pEngine.Core.Graphics.Renderer.Batches;
using pEngine.Core.Graphics.Shading;
using pEngine.Core.Graphics.Renderer;
using pEngine.Core.Graphics.Textures;

namespace pEngine.Core.Graphics.Drawables
{
    public class Sprite : Drawable
    {

		/// <summary>
		/// Creates a new <see cref="Sprite"/>.
		/// </summary>
		/// <param name="texture">Texture to draw.</param>
		public Sprite(ITexture texture)
		{
			Texture = texture;
			PropertyChanged += GraphicsChanged;
			Texture.PropertyChanged += TextureChange;
		}

		[LoaderFunction]
		private void Load(BatchesStore Batches)
		{
			plane = Batches.GetBatch<QuadVertexBatch>(1);

			BuildVertexs();
		}

		/// <summary>
		/// Releases all resource used by the <see cref="Sprite"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Sprite"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="Sprite"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="Sprite"/> so the garbage
		/// collector can reclaim the memory that the <see cref="Sprite"/> was occupying.</remarks>
		public override void Dispose()
		{
			base.Dispose();
		}

		#region Properties

		/// <summary>
		/// Image contrast.
		/// </summary>
		/// <remarks>
		/// The value's range goes from 0 to infinity;
		/// default value is 1.
		/// </remarks>
		public float Contrast { get; set; } = 1;

		/// <summary>
		/// Image brightness.
		/// </summary>
		/// <remarks>
		/// The value's range goes from -1 to 1;
		/// default value is 0.
		/// </remarks>
		public float Brightness { get; set; } = 0;

		/// <summary>
		/// Way to stretch the texture in the destination sprite size.
		/// </summary>
		public StretchMode Stretch { get; set; } = StretchMode.UniformToFill;

		#endregion

		#region Texture

		/// <summary>
		/// Sprite texture.
		/// </summary>
		public ITexture Texture { get; }

		private void TextureChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (State != LoadState.Loaded)
				return;

			BuildVertexs();
		}

		#endregion

		#region Vertexs

		private void GraphicsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (State != LoadState.Loaded)
				return;

			switch (e.PropertyName)
			{
				case "Texture":
					Texture.PropertyChanged += TextureChange;
					BuildVertexs();
					goto case "AssetsInvalidation";
				case "Opacity":
					BuildVertexs();
					goto case "AssetsInvalidation";
				case "Stretch":
				case "Size":
					BuildVertexs();
					break;
				case "Brightness":
				case "Contrast":
				case "AssetsInvalidation":
					Invalidate(InvalidationType.Assets, InvalidationDirection.Parent, this);
					break;
			}
		}

		public override void Invalidate(InvalidationType type, InvalidationDirection propagation, IGameObject sender)
		{
			BuildVertexs();
			base.Invalidate(type, propagation, sender);
		}

		protected virtual void BuildVertexs()
		{
			Vector2 spriteCenter = ScaledSize / 2;
			Vector2 textureCenter = Texture.Size / 2;

			Vector2 position = Vector2.Zero;
			Vector2 size = Vector2.One;

			if (ScaledSize == Vector2i.Zero)
				return;

			float w = (Texture.Size.Width * (float)ScaledSize.Height) / Texture.Size.Height / ScaledSize.Width;
			float h = (Texture.Size.Height * (float)ScaledSize.Width) / Texture.Size.Width / ScaledSize.Height;

			switch (Stretch)
			{
				case StretchMode.Fill:
					position = Vector2.Zero;
					size = Vector2.One;
					break;
				case StretchMode.None:
					size = Texture.Size / (Vector2)ScaledSize;
					position = (spriteCenter - textureCenter) / ScaledSize;
					break;
				case StretchMode.Uniform:

					if (Texture.Size.Width / (float)ScaledSize.Width < Texture.Size.Height / (float)ScaledSize.Height)
					{
						h = 1;
						size = new Vector2(w, h);
						position = new Vector2((1 - w) / 2, 0);
					}
					else
					{
						w = 1;
						size = new Vector2(w, h);
						position = new Vector2(0, (1 - h) / 2);
					}

					break;
				case StretchMode.UniformToFill:

					if (Texture.Size.Width / (float)ScaledSize.Width >= Texture.Size.Height / (float)ScaledSize.Height)
					{
						h = 1;
						size = new Vector2(w, h);
						position = new Vector2((1 - w) / 2, 0);
					}
					else if (ScaledSize.Height <= Texture.Size.Height)
					{
						w = 1;
						size = new Vector2(w, h);
						position = new Vector2(0, (1 - h) / 2);
					}

					break;
			}

			plane.Vertexs[0] = new GLVertex
			{
				X = position.X,
				Y = position.Y,
				R = 1, G = 1, B = 1,
				A = (float)Opacity,
				Tx = Texture.TopLeft.X,
				Ty = Texture.TopLeft.Y
			};

			plane.Vertexs[1] = new GLVertex
			{
				X = position.X + size.X,
				Y = position.Y, R = 1, G = 1, B = 1,
				A = (float)Opacity,
				Tx = Texture.TopRight.X,
				Ty = Texture.TopRight.Y
			};

			plane.Vertexs[2] = new GLVertex
			{
				X = position.X + size.X,
				Y = position.Y + size.Y,
				R = 1, G = 1, B = 1,
				A = (float)Opacity,
				Tx = Texture.BotRight.X,
				Ty = Texture.BotRight.Y
			};

			plane.Vertexs[3] = new GLVertex
			{
				X = position.X,
				Y = position.Y + size.Y,
				R = 1, G = 1, B = 1,
				A = (float)Opacity,
				Tx = Texture.BotLeft.X,
				Ty = Texture.BotLeft.Y
			};

			plane.Invalidated = true;
			plane.InvalidationType |= BatchInvalidationType.Vertexs;
		}

		#endregion

		#region Asset

		QuadVertexBatch plane;

		protected override List<Asset> CalculateAssets()
		{
			var assets = base.CalculateAssets();

			if (Visible)
			{
				assets.Insert(0, new Asset
				{
					Transformation = DrawMatrix,
					Shader = new SpriteShader
					{
						Opacity = (float)Opacity,
						Brightness = Brightness,
						Contrast = Contrast,
						TextureAttachment = 0
					},
					Textures = new KeyValuePair<int, long>[]
					{
						new KeyValuePair<int, long>(0, Texture.DependencyID)
					},
					Elements = new DrawElement[]
					{
						new DrawElement
						{
							ElementOffset = (int)plane.Indexes.Offset,
							ElementSize = (int)plane.Indexes.Size,
							Primitive = plane.Primitive,
						}
					},
					AlphaBlendingDst = OpenGL.BlendingFactor.OneMinusSrcAlpha,
					AlphaBlendingSrc = OpenGL.BlendingFactor.SrcAlpha,
					ColorBlendingDst = OpenGL.BlendingFactor.OneMinusSrcAlpha,
					ColorBlendingSrc = OpenGL.BlendingFactor.One,
					TargetID = -1
				});
			}

			return assets;
		}

		#endregion

	}

	public enum StretchMode
	{
		/// <summary>
		/// Fill without preserve the ratio.
		/// </summary>
		Fill,

		/// <summary>
		/// Fill preserving the aspect ratio.
		/// </summary>
		UniformToFill,

		/// <summary>
		/// The content is resized to fit in the destination dimensions
		/// while it preserves its native aspect ratio.
		/// </summary>
		Uniform,

		/// <summary>
		/// Will mantains the source size.
		/// </summary>
		None
	}
}
