using pEngine.Utils.Math;

namespace pEngine.Platform.Monitors
{
    public struct Resolution
    {
        /// <summary>
        /// Resolution buffer size.
        /// </summary>
        public Vector2i ResolutionSize { get; internal set; }

		/// <summary>
		/// Resolution aspect ratio
		/// </summary>
		public double AspectRatio => (double)ResolutionSize.Width / ResolutionSize.Height;

		/// <summary>
		/// Monitor refresh rate.
		/// </summary>
		public int RefreshRate { get; internal set; }

		/// <summary>
		/// Red bits reference for rendering
		/// </summary>
		public int RedBits { get; internal set; }

		/// <summary>
		/// Blue bits reference for rendering
		/// </summary>
		public int BlueBits { get; internal set; }

		/// <summary>
		/// Green bits reference for rendering
		/// </summary>
		public int GreenBits { get; internal set; }

		public override string ToString()
		{
			int r = MathHelpers.GCD(ResolutionSize.Width, ResolutionSize.Height);

			return $"{ResolutionSize.Width} x {ResolutionSize.Height} | {ResolutionSize.Width / r} : {ResolutionSize.Height / r}";
		}
	}
}
