using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Graphics.Renderer.Batches;
using pEngine.Core.Graphics.Renderer;
using pEngine.Core.Graphics.Shading;
using pEngine.Common.DataModel;

using pEngine.Common;

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
		}

		[LoaderFunction]
		private void Initializer(BatchesStore Batches)
		{
			batch = Batches.GetBatch<QuadVertexBatch>(1);

			BuildVertexs();
		}

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

			return assets;
		}
	}
}
