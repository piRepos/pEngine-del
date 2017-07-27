using System;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Core.Physics.Geometry
{
	public class Shape : Shape<Point>
	{
		/// <summary>
		/// Makes a new instance of <see cref="Shape"/>.
		/// </summary>
		/// <param name="name">Shape's name.</param>
		public Shape(string name)
			: base(name)
		{
		}
	}

	public class Shape<PointType> where PointType : IPoint
    {

		/// <summary>
		/// Makes a new instance of <see cref="Shape"/>.
		/// </summary>
		/// <param name="name">Shape's name.</param>
		public Shape(string name)
		{
			Name = name;

			Points = new ObservableCollection<PointType>();
			Edges = new ObservableCollection<Edge>();
			Faces = new ObservableCollection<Face>();

			Points.CollectionChanged += PointsChanged;
			Edges.CollectionChanged += EdgesChanged;
			Faces.CollectionChanged += FacesChanged;
		}

		private void PointsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			InvalidationType |= MeshInvalidation.Vertexs;
		}

		private void EdgesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			InvalidationType |= MeshInvalidation.Edges;
		}

		private void FacesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			InvalidationType |= MeshInvalidation.Faces;
		}

		#region Meta


		/// <summary>
		/// Mesh name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Mesh invalidation informations for tessel loader.
		/// </summary>
		public MeshInvalidation InvalidationType { get; private set; }

		/// <summary>
		/// If this shape is invalidated.
		/// </summary>
		public bool Invalidates => InvalidationType != MeshInvalidation.None;

		#endregion

		#region Structure

		/// <summary>
		/// Mesh points array.
		/// </summary>
		public ObservableCollection<PointType> Points { get; }

		/// <summary>
		/// Mesh edges array.
		/// </summary>
		public ObservableCollection<Edge> Edges { get; }

		/// <summary>
		/// Mesh faces array.
		/// </summary>
		public ObservableCollection<Face> Faces { get; }

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

	[Flags]
	public enum MeshInvalidation
	{
		/// <summary>
		/// Mesh isn't changed.
		/// </summary>
		None = 0,

		/// <summary>
		/// Vertexs changed: needs a batch update.
		/// </summary>
		Vertexs = 1 << 0,

		/// <summary>
		/// Edges changed: needs an index buffer update.
		/// </summary>
		Edges = 1 << 1,

		/// <summary>
		/// Faces changed: needs an index buffer update.
		/// </summary>
		Faces = 1 << 2,

		/// <summary>
		/// All in this mesh is changed.
		/// </summary>
		All = Vertexs | Edges
	}
}
