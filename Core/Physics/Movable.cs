using System;
using System.Collections.Generic;
using System.ComponentModel;

using pEngine.Common.Math;
using pEngine.Common.Timing.Base;
using pEngine.Core.Graphics.Renderer;

namespace pEngine.Core.Physics
{
	public abstract class Movable : GameObject, IMovablef, IScalablef
    {
		/// <summary>
		/// Makes a new instance of <see cref="Movable"/>.
		/// </summary>
		public Movable()
		{
			PropertyChanged += PositionChange;
			invalidationId = 0;
			ScaleReference = Vector2i.Zero;
		}

		/// <summary>
		/// Triggered after matrix recalculation.
		/// </summary>
		event EventHandler GeometryChanged;

		/// <summary>
		/// Triggered on position change.
		/// </summary>
		event EventHandler PositionChanged;

		/// <summary>
		/// Triggered on size change.
		/// </summary>
		event EventHandler SizeChanged;

		/// <summary>
		/// Triggered on rotation change.
		/// </summary>
		event EventHandler RotationChanged;

		#region Positioning

		/// <summary>
		/// Axes origin: The position is relative to
		/// the selected point.
		/// </summary>
		public Anchor Origin { get; set; }

		/// <summary>
		/// Custom Axes origin: The position is relative to
		/// this point.
		/// </summary>
		public Vector2 CustomOrigin { get; set; }

		/// <summary>
		/// Anchor target to the objext: the position vector
		/// will start from the <see cref="Origin"/> to the <see cref="Anchor"/>.
		/// </summary>
		public Anchor Anchor { get; set; }

		/// <summary>
		/// Anchor target to the objext: the position vector
		/// will start from the <see cref="Origin"/> to the <see cref="Anchor"/>.
		/// </summary>
		public Vector2 CustomAnchor { get; set; }

		/// <summary>
		/// Object position in pixels.
		/// </summary>
		public Vector2 Position { get; set; }

		#endregion

		#region Size

		/// <summary>
		/// If true the object ill be scaled with parent.
		/// </summary>
		public bool ScaleWithParent { get; set; }

		/// <summary>
		/// Grow or shrink this object with the
		/// parent size changes.
		/// </summary>
		public Axes ScaleSize { get; set; }

		/// <summary>
		/// Object size in pixels.
		/// </summary>
		public Vector2 Size { get; set; }

		#endregion

		#region Rotation

		/// <summary>
		/// Rotation center anchor.
		/// </summary>
		public Anchor RotationCenter { get; set; }

		/// <summary>
		/// Rotation center point.
		/// </summary>
		public Vector2i RotationCustomCenter { get; set; }

		/// <summary>
		/// Object rotation.
		/// </summary>
		public Angle Rotation { get; set; }

		#endregion

		#region Reference

		/// <summary>
		/// Referencce size for scaling children.
		/// </summary>
		public Vector2 ScaleReference { get; set; }

		/// <summary>
		/// Gets the relative size if ScaleSize is enabled
		/// otherwise it returns the actual size.
		/// </summary>
		public Vector2 ScaledSize
		{
			get
			{
				Vector2 sizeScale = Vector2i.Zero;
				Movable parent = null;

				// - Check if we have a parent
				if (Parent != null && Parent is Movable)
					parent = Parent as Movable;

				// - Set parent-relative size
				if (ScaleSize.HasFlag(Axes.X) && parent != null)
					sizeScale.X = parent.ScaledSize.X - parent.ScaleReference.X;
				if (ScaleSize.HasFlag(Axes.Y) && parent != null)
					sizeScale.Y = parent.ScaledSize.Y - parent.ScaleReference.Y;

				return Size + sizeScale;
			}
		}

		#endregion

		#region Matrix

		/// <summary>
		/// Object transformation draw matrix.
		/// </summary>
		internal Matrix DrawMatrix { get; set; }

		/// <summary>
		/// Object transformation matrix.
		/// </summary>
		internal Matrix Matrix { get; set; }

		/// <summary>
		/// Object inverse transformation matrix.
		/// </summary>
		internal Matrix MatrixInverse { get; set; }

		private void CalculateMatrix()
		{
			Movable parent = null;
			Matrix = Matrix.Identity;
			DrawMatrix = Matrix.Identity;
			Vector2 scaledSize = ScaledSize;

			// - Check if we have a parent
			if (Parent != null && Parent is Movable)
				parent = Parent as Movable;

			// - Apply rotation to the matrix
			{
				Vector2 rotationCenter = -ComputeAnchorPosition(scaledSize, RotationCustomCenter, RotationCenter);

				Matrix *= Matrix.CreateTranslation(rotationCenter.X, rotationCenter.Y, 0);
				Matrix *= Matrix.CreateRotationZ(Rotation);
				Matrix *= Matrix.CreateTranslation(-rotationCenter.X, -rotationCenter.Y, 0);
			}

			// - Create translation to requests point
			{
				Vector2 finalPosition = Position;
				if (parent != null)
				{
					// - Apply origin point
					finalPosition *= ComputeVectorDir(Anchor, Origin);
					finalPosition += ComputeAnchorPosition(parent.ScaledSize, CustomOrigin, Origin);
				}

				// - Apply anchor point
				finalPosition -= ComputeAnchorPosition(scaledSize, CustomAnchor, Anchor);

				// - Calculate matrix
				Matrix *= Matrix.CreateTranslation(finalPosition.X, finalPosition.Y, 0);
			}

			// - Scale with parent
			if (parent != null && ScaleWithParent)
			{
				Vector2 scaleReference = parent.ScaleReference;
				Vector2 parentScaledSize = parent.ScaledSize;
				Vector2 scaleFactor = parentScaledSize / scaleReference;
				Matrix *= Matrix.CreateScale(scaleFactor.X, scaleFactor.Y, 0);
			}

			// - If we have a parent do relative transformation
			if (parent != null)
				Matrix *= parent.Matrix;

			// - Set object size
			DrawMatrix *= Matrix.CreateScale(scaledSize.Width, scaledSize.Y, 0) * Matrix;
			DrawMatrix *= ToRelativeFloat;

			MatrixInverse = Matrix.Invert(Matrix);
		}

		#endregion

		#region Update logic

		private long invalidationId;

		private void PositionChange(object sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
				case "Size":
					if (ScaleReference == Vector2i.Zero)
						ScaleReference = Size;
					SizeChanged?.Invoke(this, EventArgs.Empty);
					goto case "InvalidateMatrix";
				case "ScaleWithParent":
				case "ScaleSize":
					SizeChanged?.Invoke(this, EventArgs.Empty);
					goto case "InvalidateMatrix";
				case "ScalePosition":
				case "Origin":
				case "Anchor":
				case "Position":
					PositionChanged?.Invoke(this, EventArgs.Empty);
					goto case "InvalidateMatrix";
				case "RotationCenter":
				case "Rotation":
					RotationChanged?.Invoke(this, EventArgs.Empty);
					goto case "InvalidateMatrix";
				case "CustomOrigin":
					if (Origin != Anchor.Custom)
						return;
					PositionChanged?.Invoke(this, EventArgs.Empty);
					goto case "InvalidateMatrix";
				case "CustomAnchor":
					if (Anchor != Anchor.Custom)
						return;
					PositionChanged?.Invoke(this, EventArgs.Empty);
					goto case "InvalidateMatrix";
				case "RotationCustomCenter":
					if (RotationCenter != Anchor.Custom)
						return;
					RotationChanged?.Invoke(this, EventArgs.Empty);
					goto case "InvalidateMatrix";
				case "InvalidateMatrix":
					if (State == LoadState.NotLoaded)
						return;

					Invalidate(InvalidationType.Geometry, InvalidationDirection.Children, this);
					Invalidate(InvalidationType.Assets, InvalidationDirection.Both, this);
					break;
			}
		}

		/// <summary>
		/// Update this object physics.
		/// </summary>
		/// <param name="clock">Gameloop clock.</param>
		public override void Update(IFrameBasedClock clock)
		{
			base.Update(clock);

			if (State == LoadState.Loaded && Host.PhysicsLoop.FrameId > invalidationId)
			{
				CalculateMatrix();
				invalidationId = long.MaxValue;
				GeometryChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Invalidate a property of this object/tree.
		/// </summary>
		/// <param name="type">Property to invalidate.</param>
		/// <param name="propagation">Propagation direction.</param>
		/// <param name="sender">Object sender.</param>
		public override void Invalidate(InvalidationType type, InvalidationDirection propagation, IGameObject sender)
		{
			if (type.HasFlag(InvalidationType.Geometry) && invalidationId == long.MaxValue)
				invalidationId = Host.PhysicsLoop.FrameId;

			base.Invalidate(type, propagation, sender);
		}

		#endregion

		#region Utils

		/// <summary>
		/// This matrix transform pixel space to float -1;1 space.
		/// </summary>
		static internal Matrix ToRelativeFloat { get; set; }

		/// <summary>
		/// Updates the <see cref="ToRelativeFloat"/> matrix.
		/// </summary>
		/// <param name="bufferSize">Window framebuffer size.</param>
		static internal void UpdateMatrixViewport(Vector2i bufferSize)
		{
			ToRelativeFloat = Matrix.Identity;
			ToRelativeFloat *= Matrix.CreateScale(2, 2, 0);
			ToRelativeFloat *= Matrix.CreateTranslation(-bufferSize.Width, -bufferSize.Height, 0);
			ToRelativeFloat *= Matrix.CreateScale(1F / (bufferSize.Width), -1F / (bufferSize.Height), 0);
		}

		/// <summary>
		/// Gets a vector that is calculated using a percentile
		/// value of the object size.
		/// </summary>
		/// <param name="percent">Percentile.</param>
		/// <param name="other">Source object.</param>
		/// <returns>A new vector.</returns>
		public Vector2 PercentileVecByOtherObject(double percent, Movable other)
		{
			return other.Size * (percent / 100);
		}

		/// <summary>
		/// Gets a vector that is calculated using a percentile
		/// value of the parent size.
		/// </summary>
		/// <param name="percent">Percentile.</param>
		/// <param name="other">Source object.</param>
		/// <returns>A new vector.</returns>
		public Vector2 PercentileVecByParent(double percent)
		{
			if (Parent == null)
				throw new InvalidOperationException("Parent is null.");

			if (!(Parent is Movable))
				throw new InvalidOperationException("Parent is not a movable object.");

			return PercentileVecByOtherObject(percent, Parent as Movable);
		}

		/// <summary>
		/// Accepts a vector in local coordinates and converts it to coordinates in another Drawable's space.
		/// </summary>
		/// <param name="input">A vector in local coordinates.</param>
		/// <param name="other">The drawable in which space we want to transform the vector to.</param>
		/// <returns>The vector in other's coordinates.</returns>
		public Vector2i ToSpaceOfOtherObject(Vector2i input, Movable other)
		{
			if (other == this)
				return input;

			return input * Matrix * other.MatrixInverse;
		}

		/// <summary>
		/// Accepts a vector in local coordinates and converts it to coordinates in Parent's space.
		/// </summary>
		/// <param name="input">A vector in local coordinates.</param>
		/// <returns>The vector in Parent's coordinates.</returns>
		public Vector2i ToParentSpace(Vector2i input)
		{
			if (Parent == null)
				throw new InvalidOperationException("Parent is null.");

			if (!(Parent is Movable))
				throw new InvalidOperationException("Parent is not a movable object.");

			return ToSpaceOfOtherObject(input, Parent as Movable);
		}

		/// <summary>
		/// Accepts a vector in local coordinates and converts it to coordinates in screen space.
		/// </summary>
		/// <param name="input">A vector in local coordinates.</param>
		/// <returns>The vector in screen coordinates.</returns>
		public Vector2i ToScreenSpace(Vector2i input)
		{
			return input * Matrix;
		}

		/// <summary>
		/// Convert a position to the local coordinate system from either native or local to another drawable.
		/// This is *not* the same space as the Position member variable (use Parent.GetLocalPosition() in this case).
		/// </summary>
		/// <param name="screenSpacePos">The input position.</param>
		/// <returns>The output position.</returns>
		public Vector2i ToLocalSpace(Vector2i screenSpacePos)
		{
			return screenSpacePos * MatrixInverse;
		}

		// TODO: Implements quads and this functions.

		/// <summary>
		/// Accepts a rectangle in local coordinates and converts it to a quad in Parent's space.
		/// </summary>
		/// <param name="input">A rectangle in local coordinates.</param>
		/// <returns>The quad in Parent's coordinates.</returns>
		/*public Quad ToParentSpace(RectangleF input)
		{
			return Quad.FromRectangle(input) * (DrawInfo.Matrix * Parent.DrawInfo.MatrixInverse);
		}*/

		/// <summary>
		/// Accepts a rectangle in local coordinates and converts it to a quad in screen space.
		/// </summary>
		/// <param name="input">A rectangle in local coordinates.</param>
		/// <returns>The quad in screen coordinates.</returns>
		/*public Quad ToScreenSpace(RectangleF input)
		{
			return Quad.FromRectangle(input) * DrawInfo.Matrix;
		}*/

		/// <summary>
		/// Helper function to compute an absolute position given an absolute size and
		/// a relative <see cref="Graphics.Anchor"/>.
		/// </summary>
		/// <param name="size">Absolute size</param>
		/// <param name="anchor">Relative <see cref="Graphics.Anchor"/></param>
		/// <returns>Absolute position</returns>
		public static Vector2 ComputeAnchorPosition(Vector2 size, Vector2 custom, Anchor anchor)
		{
			Vector2 result = Vector2i.Zero;

			if (anchor == Anchor.Custom)
				return custom;

			if ((anchor & Anchor.x1) > 0)
				result.X = size.X / 2;
			else if ((anchor & Anchor.x2) > 0)
				result.X = size.X;

			if ((anchor & Anchor.y1) > 0)
				result.Y = size.Y / 2;
			else if ((anchor & Anchor.y2) > 0)
				result.Y = size.Y;

			return result;
		}

		/// <summary>
		/// Helper function to compute the vector direction given
		/// a relative <see cref="Graphics.Anchor"/>.
		/// </summary>
		/// <param name="anchor">Relative <see cref="Graphics.Anchor"/></param>
		/// <returns>Vector direction</returns>
		public static Vector2i ComputeVectorDir(Anchor anchor, Anchor origin)
		{
			Vector2i result = Vector2i.One;

			if ((origin & Anchor.x2) > 0)
				result.X = -1;

			if ((origin & Anchor.y2) > 0)
				result.Y = -1;

			return result;
		}

		#endregion

		#region Informations

		/// <summary>
		/// String rappresentation of a gameobject.
		/// </summary>
		/// <returns>String.</returns>
		public override string ToString()
		{
			return base.ToString() + $": ({Position.X};{Position.Y})@[{Size.Width}x{Size.Height}]";
		}

		#endregion

	}

	/// <summary>
	/// General enum to specify an "anchor" or "origin" point from the standard 9 points on a rectangle.
	/// x and y counterparts can be accessed using bitwise flags.
	/// </summary>
	[Flags]
	public enum Anchor
	{
		TopLeft = y0 | x0,
		TopCentre = y0 | x1,
		TopRight = y0 | x2,

		MidLeft = y1 | x0,
		MidCentre = y1 | x1,
		MidRight = y1 | x2,

		BottomLeft = y2 | x0,
		BottomCentre = y2 | x1,
		BottomRight = y2 | x2,

		Centre = MidCentre,

		/// <summary>
		/// The vertical counterpart is at "Top" position.
		/// </summary>
		y0 = 1 << 0,

		/// <summary>
		/// The vertical counterpart is at "Centre" position.
		/// </summary>
		y1 = 1 << 1,

		/// <summary>
		/// The vertical counterpart is at "Bottom" position.
		/// </summary>
		y2 = 1 << 2,

		/// <summary>
		/// The horizontal counterpart is at "Left" position.
		/// </summary>
		x0 = 1 << 3,

		/// <summary>
		/// The horizontal counterpart is at "Centre" position.
		/// </summary>
		x1 = 1 << 4,

		/// <summary>
		/// The horizontal counterpart is at "Right" position.
		/// </summary>
		x2 = 1 << 5,

		/// <summary>
		/// The user is manually updating the outcome, so we shouldn't.
		/// </summary>
		Custom = 1 << 6,
	}

	[Flags]
	public enum Axes
	{
		None = 0,

		X = 1 << 0,
		Y = 1 << 1,

		Both = X | Y,
	}
}
