using System;

using pEngine.Utils.Math;

namespace pEngine.Context
{
	public interface IContext : IScalable
	{
		/// <summary>
		/// Window framebuffer scaling based on pixel size.
		/// </summary>
		Vector2 Scaling { get; }

		/// <summary>
		/// Gets the frame buffer size.
		/// </summary>
		Vector2i BufferSize { get; }

		/// <summary>
		/// Handle all system messages.
		/// </summary>
		void PollMesages(bool Blocking);

		/// <summary>
		/// Set the context on the current thread.
		/// </summary>
		void SetContext();

		/// <summary>
		/// Swaps the backbuffer with the frontbuffer.
		/// </summary>
		void SwapBuffer();
	}
}
