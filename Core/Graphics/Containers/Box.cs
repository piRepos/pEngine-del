using System;
using System.Linq;
using System.Collections.Generic;

using pEngine.Core.Graphics.Renderer.Clipping;
using pEngine.Core.Graphics.Renderer.Batches;
using pEngine.Core.Graphics.Renderer;
using pEngine.Core.Graphics.Shading;
using pEngine.Common.DataModel;

namespace pEngine.Core.Graphics.Containers
{
    public class Box : Container
    {

		/// <summary>
		/// Makes a new instance of <see cref="Box"/>.
		/// </summary>
		public Box()
			: base()
		{
			PropertyChanged += BackgroundChange;

			clippingMasks = new Dictionary<LayerMask, Mask>();
		}

		[LoaderFunction]
		private void Initializer(BatchesStore Batches)
		{
			batch = Batches.GetBatch<QuadVertexBatch>(1);

			BuildVertexs();
		}

		#region Assets management

		protected override List<Asset> CalculateAssets()
		{
			var assets = base.CalculateAssets();

			if (Background)
			{
				assets.Insert(0, new Asset
				{
					Transformation = DrawMatrix,
					Shader = new FillShader(),
					Elements = new DrawElement[]
					{
						new DrawElement
						{
							ElementOffset = (int)batch.Indexes.Offset,
							ElementSize = (int)batch.Indexes.Size,
							Primitive = batch.Primitive,
						}
					},
					AlphaBlendingDst = OpenGL.BlendingFactor.OneMinusSrcAlpha,
					AlphaBlendingSrc = OpenGL.BlendingFactor.SrcAlpha,
					ColorBlendingDst = OpenGL.BlendingFactor.Zero,
					ColorBlendingSrc = OpenGL.BlendingFactor.One,
				});
			}

			if (ClipBoundingBox)
			{
				if (FrameBuffered)
				{
					Asset lastAsset = assets[assets.Count - 1];

					lastAsset.ScissorArea = new Common.Math.Rect(ToScreenSpace((Vector2i)Position), (Vector2i)ScaledSize);

					assets[assets.Count - 1] = lastAsset;
				}
				else
				{
					for (int i = 0; i < assets.Count; ++i)
					{
						Asset lastAsset = assets[i];

						if (lastAsset.ScissorArea == null)
						{
							lastAsset.ScissorArea = new Common.Math.Rect(ToScreenSpace((Vector2i)Position), (Vector2i)ScaledSize);
							assets[i] = lastAsset;
						}
					}
				}
			}

			if (clippingMasks.Count > 0)
			{

				if (FrameBuffered)
				{
					Asset lastAsset = assets[assets.Count - 1];

					lastAsset.LayerMasks = clippingMasks
						.Where(x => x.Value.Enabled && x.Value.MaskNode.IsLoaded)
						.Select(x => new ClippingInformations
						{
							MaskTexture = x.Value.MaskNode.VideoBuffer.TargetTexture.DependencyID,
							Operation = x.Value.Operation
						}).ToArray();

					lastAsset.ClippingType = MaskType;

					assets[assets.Count - 1] = lastAsset;
				}
				else
				{
					for (int i = 0; i < assets.Count; ++i)
					{
						Asset lastAsset = assets[i];

						if (lastAsset.LayerMasks == null)
							lastAsset.LayerMasks = new ClippingInformations[0];

						lastAsset.LayerMasks = lastAsset.LayerMasks.Union(clippingMasks.Where(x => x.Value.Enabled && x.Value.MaskNode.IsLoaded).Select(x => new ClippingInformations
						{
							MaskTexture = x.Value.MaskNode.VideoBuffer.TargetTexture.DependencyID,
							Operation = x.Value.Operation
						})).ToArray();

						lastAsset.ClippingType = MaskType;

						assets[i] = lastAsset;
					}
				}

			}

			return assets;
		}

		#endregion

		#region Background management

		private QuadVertexBatch batch;

		/// <summary>
		/// Enables background.
		/// </summary>
		public bool Background { get; set; }

		/// <summary>
		/// Background color
		/// </summary>
		public Color4 BackgroundColor { get; set; }

		private void BackgroundChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "BackgroundColor" && batch != null)
				BuildVertexs();
		}

		private void BuildVertexs()
		{
			batch.Vertexs[0] = new GLVertex { X = 0, Y = 0, R = BackgroundColor.Rf, G = BackgroundColor.Gf, B = BackgroundColor.Bf, A = BackgroundColor.Af };
			batch.Vertexs[1] = new GLVertex { X = 1, Y = 0, R = BackgroundColor.Rf, G = BackgroundColor.Gf, B = BackgroundColor.Bf, A = BackgroundColor.Af };
			batch.Vertexs[2] = new GLVertex { X = 1, Y = 1, R = BackgroundColor.Rf, G = BackgroundColor.Gf, B = BackgroundColor.Bf, A = BackgroundColor.Af };
			batch.Vertexs[3] = new GLVertex { X = 0, Y = 1, R = BackgroundColor.Rf, G = BackgroundColor.Gf, B = BackgroundColor.Bf, A = BackgroundColor.Af };

			batch.InvalidateDependency();
			batch.InvalidationType |= BatchInvalidationType.Vertexs;
		}

		#endregion

		#region Overflow management

		private Dictionary<LayerMask, Mask> clippingMasks;

		/// <summary>
		/// Gets all added masks.
		/// </summary>
		public IEnumerable<Mask> LayerMasks => clippingMasks.Values;

		/// <summary>
		/// Gets or sets the mask type.
		/// </summary>
		public MaskType MaskType { get; set; }

		/// <summary>
		/// If true will be used the bounding box as layer mask.
		/// </summary>
		public bool ClipBoundingBox { get; set; }

		/// <summary>
		/// Adds a layer mask to this object.
		/// </summary>
		/// <param name="mask">Layer mask to add.</param>
		/// <param name="operation">Layer mask operation.</param>
		/// <param name="enabled">Enables or disable the mask.</param>
		public LayerMask AddMask(LayerMask mask, MaskOperation operation, bool enabled = true)
		{
			clippingMasks.Add(mask, new Mask
			{
				MaskNode = mask,
				Operation = operation,
				Enabled = enabled
			});

			Invalidate(InvalidationType.All, InvalidationDirection.Children, this);

			return mask;
		}

		/// <summary>
		/// Remove a layer mask from this object.
		/// </summary>
		/// <param name="mask">Target mask.</param>
		public void RemoveMask(LayerMask mask)
		{
			clippingMasks.Remove(mask);
		}

		/// <summary>
		/// Enables or disable an active layer mask.
		/// </summary>
		/// <param name="mask">Target mask.</param>
		/// <param name="enabled">Enable or disable.</param>
		public void SetMaskEnabled(LayerMask mask, bool enabled)
		{
			var m = clippingMasks[mask];
			m.Enabled = enabled;
			clippingMasks[mask] = m;
		}

		#endregion

	}
}
