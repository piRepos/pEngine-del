using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

namespace pEngine.Common.Math
{
    public struct Rect : IScalable, IMovable, IEnumerable<Vector2i>, IEquatable<Rect>
    {
		/// <summary>
		/// Create a new <see cref="Rect"/> class.
		/// </summary>
		/// <param name="X">Top left x position.</param>
		/// <param name="Y">Top left y position.</param>
		/// <param name="Width">Rectangle width.</param>
		/// <param name="Height">Rectangle height.</param>
		public Rect(int x, int y, int width, int height)
		{
			Position = new Vector2i(x, y);
			Size = new Vector2i(width, height);
		}

		/// <summary>
		/// Create a new <see cref="Rect"/> class.
		/// </summary>
		/// <param name="Width">Rectangle width.</param>
		/// <param name="Height">Rectangle height.</param>
		public Rect(int width, int height)
		{
			Position = new Vector2i();
			Size = new Vector2i(width, height);
		}

		/// <summary>
		/// Create a new <see cref="Rect"/> class.
		/// </summary>
		/// <param name="size">Rectangle size.</param>
		public Rect(Vector2i size)
		{
			Position = new Vector2i();
			Size = size;
		}

		/// <summary>
		/// Create a new <see cref="Rect"/> from a <see cref="System.Drawing.Rectangle"/>.
		/// </summary>
		/// <param name="rect">Rectangle to copy.</param>
		public Rect(Rectangle rect)
		{
			Position = rect.Location;
			Size = rect.Size;
		}

		/// <summary>
		/// Create a new <see cref="Rect"/> class.
		/// </summary>
		/// <param name="position">Rectangle top left angle position.</param>
		/// <param name="size">Rectangle size.</param>
		public Rect(Vector2i position, Vector2i size)
		{
			Position = position;
			Size = size;
		}

		/// <summary>
		/// Rectangle top-left point position.
		/// </summary>
		public Vector2i Position { get; set; }
		
		/// <summary>
		/// Rectangle size.
		/// </summary>
		public Vector2i Size { get; set; }

		/// <summary>
		/// Rectangle top edge.
		/// </summary>
		public int Top => Position.Y;

		/// <summary>
		/// Rectangle left edge.
		/// </summary>
		public int Left => Position.X;

		/// <summary>
		/// Rectagnle right edge.
		/// </summary>
		public int Right => Position.X + Size.Width;

		/// <summary>
		/// Rectangle bottom edge.
		/// </summary>
		public int Bottom => Position.Y + Size.Height;

		/// <summary>
		/// Top left corner point.
		/// </summary>
		public Vector2i p1 => Position;

		/// <summary>
		/// Top right corner point.
		/// </summary>
		public Vector2i p2 => Position + new Vector2i(Size.Width, 0);

		/// <summary>
		/// Bot right corner point.
		/// </summary>
		public Vector2i p3 => Position + Size;

		/// <summary>
		/// Bot left corner point.
		/// </summary>
		public Vector2i p4 => Position + new Vector2i(0, Size.Height);

		#region Operators

		/// <summary>
		/// Gets a point oh this rectangle.
		/// </summary>
		/// <param name="index">Point index.</param>
		/// <returns>Selected Point.</returns>
		public Vector2i this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return Position;
					case 1: return Position + new Vector2i(Size.Width, 0);
					case 2: return Position + Size;
					case 3: return Position + new Vector2i(0, Size.Height);
					default:
						throw new IndexOutOfRangeException();
				}
			}
		}

		/// <summary>
		/// From .Net standard <see cref="Rectangle"/> to <see cref="Rect"/>.
		/// </summary>
		/// <param name="r">.Net rectangle.</param>
		public static implicit operator Rect(Rectangle r)
		{
			return new Rect(r);
		}

		/// <summary>
		/// To .Net standard <see cref="Rectangle"/> to <see cref="Rect"/>.
		/// </summary>
		/// <param name="r">Engine rectangle.</param>
		public static implicit operator Rectangle(Rect r)
		{
			return new Rectangle(r.Position, r.Size);
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<Vector2i> GetEnumerator()
		{
			yield return Position;
			yield return Position + new Vector2i(Size.Width, 0);
			yield return Position + Size;
			yield return Position + new Vector2i(0, Size.Height);
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>
		/// Check if this rectangle is equals to another.
		/// </summary>
		/// <param name="rect">Rectangle to check.</param>
		/// <returns>If is equals.</returns>
		public bool Equals(Rect rect)
		{
			return rect.Size == Size && rect.Position == Position;
		}

		#endregion
	}
}
