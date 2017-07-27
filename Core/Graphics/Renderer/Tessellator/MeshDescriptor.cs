﻿using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Data.FrameDependency;

using pEngine.Core.Physics.Geometry;

namespace pEngine.Core.Graphics.Renderer.Tessellator
{
    public struct MeshDescriptor : IDependencyDescriptor
    {

		/// <summary>
		/// Make a new instance of <see cref="Mesh"/>.
		/// </summary>
		/// <param name="vertexs">Number of vertexs.</param>
		/// <param name="edges">Number of edges.</param>
		/// <param name="name">Mesh name (optional).</param>
		public MeshDescriptor(long id, int vertexs, int edges, int faces, string name = "")
		{
			DescriptorID = id;
			Name = name;
			Invalidation = MeshInvalidation.All;

			Points = new DrawablePoint[vertexs];
			Edges = new Edge[edges];
			Faces = new Face[faces];
		}

		#region Meta

		/// <summary>
		/// Mesh id.
		/// </summary>
		public long MeshID => DescriptorID;

		/// <summary>
		/// Descriptor id.
		/// </summary>
		public long DescriptorID { get; set; }

		/// <summary>
		/// Mesh name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Mesh invalidation informations for tessel loader.
		/// </summary>
		public MeshInvalidation Invalidation { get; set; }

		#endregion

		#region Structure

		/// <summary>
		/// Mesh points array.
		/// </summary>
		public DrawablePoint[] Points;

		/// <summary>
		/// Mesh edges array.
		/// </summary>
		public Edge[] Edges;

		/// <summary>
		/// Mesh faces array.
		/// </summary>
		public Face[] Faces;

		#endregion
	}
}
