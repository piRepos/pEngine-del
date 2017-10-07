using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.DataModel;
using pEngine.Core.Graphics.Renderer;
using pEngine.Core.Graphics.Textures;
using pEngine.Common.Timing.Base;
using pEngine.Core.Graphics.Buffering;

namespace pEngine.Core.Graphics.Containers
{
    public class LayerMask : Container
    {


		/// <summary>
		/// Makes a new instance of <see cref="LayerMask"/> class.
		/// </summary>
		public LayerMask()
			: base()
		{

		}

		[LoaderFunction]
		public void Initialize(TextureStore textures)
		{
			
		}

		#region Properties

		public override VideoBufferSettings VideoBuffer
		{
			get => new VideoBufferSettings
			{
				Enabled = true,
				Bypass = base.VideoBuffer.Bypass,
				Draw = base.VideoBuffer.Draw,
				VideoBuffer = base.VideoBuffer.VideoBuffer
			};
			set
			{
				base.VideoBuffer = new VideoBufferSettings
				{
					Enabled = true,
					Bypass = value.Bypass,
					Draw = value.Draw,
					VideoBuffer = value.VideoBuffer
				};
			}
		}

		#endregion
		
	}
}
