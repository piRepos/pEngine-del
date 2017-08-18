using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Data.FrameDependency;

using pEngine.Core.Physics.Geometry;

namespace pEngine.Core.Graphics.Renderer.Tessellator
{
    public class Mesh : Shape<DrawablePoint>, IDependency<MeshDescriptor>
    {
		/// <summary>
		/// Makes a new instance of <see cref="Mesh"/>.
		/// </summary>
		/// <param name="name">Mesh's name.</param>
		public Mesh(string name)
			: base(name)
		{
		}

		public void Dispose()
		{

		}

		#region Meta

		/// <summary>
		/// Mesh id.
		/// </summary>
		public long MeshID => DependencyID;

		#endregion

		#region Descriptor

		/// <summary>
		/// Generate a descriptor for this dependency.
		/// This descriptor will be used as frame resource.
		/// </summary>
		/// <returns>The dependency descriptor.</returns>
		public MeshDescriptor GetDescriptor()
		{
			return new MeshDescriptor
			{
				Points = Points.ToArray(),
				Edges = Edges.ToArray(),
				Faces = Faces.ToArray(),
				DescriptorID = DependencyID,
				Name = Name,
				Invalidation = InvalidationType
			};
		}

		/// <summary>
		/// Sets the dependency modified.
		/// </summary>
		public void InvalidateDependency()
		{
			if (State == DependencyState.Loaded)
				State = DependencyState.Modified;
		}

		/// <summary>
		/// Actual dependency load state.
		/// </summary>
		public DependencyState State { get; set; }

		/// <summary>
		/// Dependency identifier.
		/// </summary>
		public long DependencyID { get; set; }

		#endregion
	}
}
