using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.DataModel;
using pEngine.Core.Graphics.Renderer;
using pEngine.Core.Graphics.Textures;
using pEngine.Common.Timing.Base;
using pEngine.Core.Graphics.Renderer.Clipping;

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

		public override bool FrameBuffered { get { return true; } }

		#endregion

		#region Assets management

		public override IEnumerable<Asset> GetAssets()
		{
			List<Asset> newList = new List<Asset>(base.GetAssets());

			newList.Remove(newList.Last());

			return newList;
		}

		#endregion

		#region Updating

		public override void Update(IFrameBasedClock clock)
		{
			base.Update(clock);
		}

		#endregion
		
	}
}
