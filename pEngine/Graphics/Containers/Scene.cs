using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEngine.Graphics.Containers
{
	public abstract class Scene : Container
	{
		/// <summary>
		/// Makes a new instance of <see cref="Scene"/> class.
		/// </summary>
		public Scene()
		{
		}

		/// <summary>
		/// Gets the layer for this scene.
		/// </summary>
		public Layer Layer => Parent as Layer;

	}
}
