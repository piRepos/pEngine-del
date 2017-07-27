using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Common;
using pEngine.Common.Math;

using pEngine.Core.Physics.Geometry;

namespace pEngine.Core.Graphics.Renderer.Tessellator
{
    public struct DrawablePoint : IPoint, IEquatable<DrawablePoint>
    {

		/// <summary>
		/// Makes a new instance of <see cref="Point"/>.
		/// </summary>
		/// <param name="pos">Point position.</param>
		/// <param name="raster">Position in the target texture.</param>
		/// <param name="fill">Point fill color.</param>
		/// <param name="stroke">Point stroke color.</param>
		/// <param name="anchor">True if this point is a curve anchor.</param>
		/// <param name="thickness">Stroke width.</param>
		public DrawablePoint(Vector2 pos, Vector2 raster, Color4 fill = new Color4(), Color4 stroke = new Color4(), bool anchor = false, double thickness = 0)
		{
			Position = pos;
			RasterPosition = raster;
			FillColor = fill;
			StrokeColor = stroke;
			Interpolated = anchor;
			Thickness = thickness;
		}

		/// <summary>
		/// Makes a new instance of <see cref="Point"/>.
		/// </summary>
		/// <param name="pos">Point position.</param>
		/// <param name="fill">Point fill color.</param>
		/// <param name="stroke">Point stroke color.</param>
		/// <param name="anchor">True if this point is a curve anchor.</param>
		/// <param name="thickness">Stroke width.</param>
		public DrawablePoint(Vector2 pos, Color4 fill = new Color4(), Color4 stroke = new Color4(), bool anchor = false, double thickness = 0)
			: this(pos, new Vector2(), fill, stroke, anchor, thickness)
		{
		}

		/// <summary>
		/// Makes a new instance of <see cref="Point"/>.
		/// </summary>
		/// <param name="pos">Point position.</param>
		/// <param name="raster">Position in the target texture.</param>
		public DrawablePoint(Vector2 pos, Vector2 raster)
			: this(pos, new Vector2(), Color4.White)
		{
		}

		/// <summary>
		/// Point position.
		/// </summary>
		public Vector2 Position { get; set; }

		/// <summary>
		/// Point in the target texture.
		/// </summary>
		public Vector2 RasterPosition { get; set; }

		/// <summary>
		/// Fill color for this vertex.
		/// </summary>
		public Color4 FillColor { get; set; }

		/// <summary>
		/// Stroke color for this vertex.
		/// </summary>
		public Color4 StrokeColor { get; set; }

		/// <summary>
		/// If true this is an anchor point for bezier curves.
		/// </summary>
		public bool Interpolated { get; set; }

		/// <summary>
		/// Stroke thickness.
		/// </summary>
		public double Thickness { get; set; }

		/// <summary>
		/// String rappresentation of this <see cref="Point"/>.
		/// </summary>
		/// <returns>A string.</returns>
		public override string ToString()
		{
			string i = Interpolated ? "interpolated" : "no interpolation";
			return $@"({Position})§({RasterPosition})|FC{FillColor}|SC{StrokeColor}|{i}|{Thickness}px";
		}

		#region Equals

		public bool Equals(DrawablePoint obj)
		{
			return this == obj;
		}

		public override bool Equals(Object obj)
		{
			return obj is Point && this == (DrawablePoint)obj;
		}

		public override int GetHashCode()
		{
			return Position.GetHashCode()
				^ RasterPosition.GetHashCode()
				^ FillColor.GetHashCode()
				^ StrokeColor.GetHashCode()
				^ Interpolated.GetHashCode()
				^ Thickness.GetHashCode();
		}

		public static bool operator ==(DrawablePoint x, DrawablePoint y)
		{
			return x.Position == y.Position
				&& x.RasterPosition == y.RasterPosition
				&& x.FillColor == y.FillColor
				&& x.StrokeColor == y.StrokeColor
				&& x.Interpolated == y.Interpolated
				&& x.Thickness == y.Thickness;
		}

		public static bool operator !=(DrawablePoint x, DrawablePoint y)
		{
			return !(x == y);
		}

		#endregion

	}
}
