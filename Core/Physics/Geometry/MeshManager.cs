using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Data.FrameDependency;

namespace pEngine.Core.Physics.Geometry
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
