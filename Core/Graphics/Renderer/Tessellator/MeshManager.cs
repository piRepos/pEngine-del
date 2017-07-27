using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Data.FrameDependency;

namespace pEngine.Core.Graphics.Renderer.Tessellator
{
    public class MeshManager : FrameDependencyManager<Mesh, MeshDescriptor>
    {

		/// <summary>
		/// Make a new instance of <see cref="MeshManager"/>.
		/// </summary>
		public MeshManager(pEngine host)
			: base(host)
		{

		}

    }
}
