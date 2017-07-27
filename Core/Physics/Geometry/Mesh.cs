using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Data.FrameDependency;

namespace pEngine.Core.Physics.Geometry
{
    public class Mesh : IDependency<MeshDescriptor>
    {
		/// <summary>
		/// Makes a new instance of <see cref="Mesh"/>.
		/// </summary>
		/// <param name="name">Mesh's name.</param>
		public Mesh(string name)
		{
			Name = name;

			Points = new ObservableCollection<Point>();
			Edges = new ObservableCollection<Edge>();
			Faces = new ObservableCollection<Face>();

			Points.CollectionChanged += MeshChanged;
			Edges.CollectionChanged += MeshChanged;
			Faces.CollectionChanged += MeshChanged;
		}

		private void MeshChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			Invalidated = true;
		}

		#region Meta

		/// <summary>
		/// Mesh id.
		/// </summary>
		public long MeshID => DependencyID;

		/// <summary>
		/// Mesh name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Mesh invalidation informations for tessel loader.
		/// </summary>
		public MeshInvalidation InvalidationType { get; private set; }

		#endregion

		#region Structure

		/// <summary>
		/// Mesh points array.
		/// </summary>
		public ObservableCollection<Point> Points { get; }

		/// <summary>
		/// Mesh edges array.
		/// </summary>
		public ObservableCollection<Edge> Edges { get; }

		/// <summary>
		/// Mesh faces array.
		/// </summary>
		public ObservableCollection<Face> Faces { get; }

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
		/// True if the resource is changed.
		/// </summary>
		public bool Invalidated { get; set; }

		/// <summary>
		/// Dependency identifier.
		/// </summary>
		public long DependencyID { get; set; }

		#endregion

		/// <summary>
		/// String rappresentation of this object.
		/// </summary>
		/// <returns>The string.</returns>
		public override string ToString()
		{
			return base.ToString() + " : " + Name;
		}
	}
}
