using System;
using System.Linq;
using System.Collections.Generic;

using pEngine.Common.Math;

namespace pEngine.Common.Memory
{
	public delegate void ElementFitEventHandler<T>(T Element);

	public class RectanglePacker<T> where T : IMovable, ISized
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:pEngine.Base.DataStructs.GenericAtlas`1"/> class.
		/// </summary>
		public RectanglePacker()
		{
			NotFitted = new List<T>();
			Objects = new List<T>();
			Tree = new Block();
		}

		/// <summary>
		/// Gets the used space.
		/// </summary>
		/// <value>The used space.</value>
		public Vector2i UsedSpace => Tree.Size;

		/// <summary>
		/// Occurs when element position changes.
		/// </summary>
		public event ElementFitEventHandler<T> OnPositionChange;

		/// <summary>
		/// Occurs when an element can't be fitted.
		/// </summary>
		public event ElementFitEventHandler<T> OnElementNotFit;

		#region Position management

		/// <summary>
		/// Gets the objects added.
		/// </summary>
		public List<T> Objects { get; internal set; }

		/// <summary>
		/// Gets the objects not fitted.
		/// </summary>
		public List<T> NotFitted { get; internal set; }

		/// <summary>
		/// Gets or sets the spacing between each object.
		/// </summary>
		public Vector2i Spacing { get; set; } = Vector2i.Zero;

		/// <summary>
		/// Adds the object to the fit list.
		/// </summary>
		/// <param name="Obj">Object.</param>
		public virtual void AddObjects(params T[] objs)
		{
			foreach (var obj in objs)
			{
				if (!Objects.Contains(obj))
					Objects.Add(obj);
			}
		}

		/// <summary>
		/// Remove the specified bound.
		/// </summary>
		/// <param name="Obj">Object.</param>
		public virtual void Remove(T Obj)
		{
			if (Objects.Contains(Obj))
				Objects.Remove(Obj);
		}

		#endregion

		#region Algorithm

		/// <summary>
		/// Tree struct for rectangle split abstraction.
		/// </summary>
		private class Block
		{
			public Vector2i Position;
			public Vector2i Size;
			public Block Right, Bottom;
			public bool Used;

			public void MakeLeafs()
			{
				Right = new Block();
				Bottom = new Block();
				Right.Used = false;
				Bottom.Used = false;
			}
		}

		/// <summary>
		/// Entry point.
		/// </summary>
		Block Tree;

		/// <summary>
		/// Fit all elements in the area.
		/// </summary>
		public virtual void Fit()
		{
			// If there are no object nothink to do.
			if (Objects.Count == 0)
				return;
			
			// Sort all elements for height.
			Objects = Objects.OrderByDescending(X => X.Size.Height).ToList();

			// Add first object.
			Tree.Size = Objects[0].Size + Spacing;

			Block Node;

			// Foreach object try to fit it.
			foreach (T B in Objects)
			{
				// If we found the space for fit this object
				if ((Node = FindNode(Tree, B.Size + Spacing)) != null)
				{
					// Split the current node and set the new position
					Node = SplitNode(Node, B.Size + Spacing);

					if (B.Position != Node.Position)
						OnPositionChange?.Invoke(B);
					
					B.Position = Node.Position;
				}
				else
				{
					// Else grow the atlas
					Node = GrowNode(B.Size + Spacing);

					// If we can't grow the atlas, this item can't be added
					if (Node == null)
					{
						if (!NotFitted.Contains(B))
							NotFitted.Add(B);
						
						OnElementNotFit?.Invoke(B);
						
						continue;
					}

					if (B.Position != Node.Position)
						OnPositionChange?.Invoke(B);

					// Else space found!
					B.Position = Node.Position;
				}

			}
		}

		#endregion

		#region Node management

		/// <summary>
		/// Finds a free node.
		/// </summary>
		/// <returns>Reference to a free node.</returns>
		/// <param name="Root">Tree root.</param>
		/// <param name="Dim">Size requested.</param>
		Block FindNode(Block Root, Vector2i Dim)
		{
			Block Tmp;
			if (Root.Used)
			{
				if ((Tmp = FindNode(Root.Right, Dim)) != null) return Tmp;
				else return FindNode(Root.Bottom, Dim);
			}
			else if (Dim.Width <= Root.Size.Width && Dim.Height <= Root.Size.Height)
				return Root;
			else return null;
		}

		/// <summary>
		/// Splits a node into 2 parts.
		/// </summary>
		/// <returns>Source node splitted.</returns>
		/// <param name="Node">Node to split.</param>
		/// <param name="Dim">Split size.</param>
		Block SplitNode(Block Node, Vector2i Dim)
		{
			Node.Used = true;

			if (Node.Right == null || Node.Bottom == null)
				Node.MakeLeafs();

			Node.Bottom.Position = new Vector2i(Node.Position.X, Node.Position.Y + Dim.Height);
			Node.Bottom.Size = new Vector2i(Node.Size.Width, Node.Size.Height - Dim.Height);

			Node.Right.Position = new Vector2i(Node.Position.X + Dim.Width, Node.Position.Y);
			Node.Right.Size = new Vector2i(Node.Size.Width - Dim.Width, Dim.Height);

			return Node;
		}

		/// <summary>
		/// Grows the node size.
		/// </summary>
		/// <returns>The node to grow.</returns>
		/// <param name="Dim">New size.</param>
		Block GrowNode(Vector2i Dim)
		{
			bool CanGrowBottom = Dim.Width <= Tree.Size.Width;
			bool CanGrowRight = Dim.Height <= Tree.Size.Height;

			bool ShouldGrowRight = CanGrowRight && (Tree.Size.Height >= Tree.Size.Width + Dim.Width);
			bool ShouldGrowBottom = CanGrowBottom && (Tree.Size.Width >= Tree.Size.Height + Dim.Height);

			if (ShouldGrowRight) return GrowRight(Dim);
			if (ShouldGrowBottom) return GrowDown(Dim);
			if (CanGrowRight) return GrowRight(Dim);
			if (CanGrowBottom) return GrowDown(Dim);

			return null;
		}

		/// <summary>
		/// Grows the right part of a node.
		/// </summary>
		/// <returns>Node grown.</returns>
		/// <param name="Dim">New size.</param>
		Block GrowRight(Vector2i Dim)
		{
			Block Node = new Block();
			Node.Bottom = Tree;
			Node.Right = new Block();
			Tree = Node;

			if (Tree.Right == null || Tree.Bottom == null)
				Tree.MakeLeafs();

			Tree.Size = new Vector2i(Node.Bottom.Size.Width + Dim.Width, Node.Bottom.Size.Height);

			Tree.Right.Position = new Vector2i(Tree.Size.Width - Dim.Width, 0);
			Tree.Right.Size = new Vector2i(Dim.Width, Tree.Size.Height);

			Tree.Used = true;

			if ((Node = FindNode(Tree, Dim)) != null)
				return SplitNode(Node, Dim);
			else
				return null;
		}

		/// <summary>
		/// Grows the bottom part of a node.
		/// </summary>
		/// <returns>Node grown.</returns>
		/// <param name="Dim">New size.</param>
		Block GrowDown(Vector2i Dim)
		{
			Block Node = new Block();
			Node.Right = Tree;
			Node.Bottom = new Block();
			Tree = Node;

			if (Tree.Right == null || Tree.Bottom == null)
				Tree.MakeLeafs();

			Tree.Size = new Vector2i(Node.Right.Size.Width, Node.Right.Size.Height + Dim.Height);

			Tree.Bottom.Position = new Vector2i(0, Tree.Size.Height - Dim.Height);
			Tree.Bottom.Size = new Vector2i(Tree.Size.Width, Dim.Height);

			Tree.Used = true;

			if ((Node = FindNode(Tree, Dim)) != null)
				return SplitNode(Node, Dim);
			else
				return null;

		}

        #endregion
	}
}
