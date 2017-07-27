using System;

using pEngine.Common.Math;
using pEngine.Platform.Monitors;

namespace pEngine.Platform.Windows
{

    /// <summary>
    /// Handle file drop on window.
    /// </summary>
    /// <param name="Ref">Window reference.</param>
    /// <param name="Files">Files dropped.</param>
    public delegate void WindowFileDrop(IWindow window, string[] files);

    /// <summary>
    /// Genery system window.
    /// </summary>
	public interface IWindow : IMovable, IScalable, IDisposable
    {

        /// <summary>
        /// Handle all system messages.
        /// </summary>
        void PollMesages(bool Blocking);

        /// <summary>
        /// Close this window.
        /// </summary>
        void Close();

        /// <summary>
        /// Make visible the window.
        /// </summary>
        void Show();

        /// <summary>
        /// Hide this window.
        /// </summary>
        void Hide();

        /// <summary>
        /// Close and make a new window.
        /// </summary>
        void Restore();

        /// <summary>
        /// Initialize this window.
        /// </summary>
        void Make();

		/// <summary>
		/// Set the context on the current thread.
		/// </summary>
		void SetContext();

		/// <summary>
		/// Swaps the backbuffer with the frontbuffer.
		/// </summary>
		void SwapBuffer();

        /// <summary>
        /// Window title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// True when window is in icon state.
        /// </summary>
        bool IsIconified { get; }

        /// <summary>
        /// True when window is visible.
        /// </summary>
        bool Visible { get; }

        /// <summary>
        /// True when needs to be closed.
        /// </summary>
        bool ShouldClose { get; }

        /// <summary>
        /// Make window fullscreen or not.
        /// </summary>
        bool Fullscreen { get; set; }

        /// <summary>
        /// Window framebuffer scaling.
        /// </summary>
        Vector2 Scaling { get; }

		/// <summary>
		/// Gets the frame buffer size.
		/// </summary>
		Vector2i BufferSize { get; }

        /// <summary>
        /// Get monitor window position.
        /// </summary>
        IMonitor Monitor { get; }

        /// <summary>
        /// Triggered o window restore / creation.
        /// </summary>
        event Action OnRestore;

        /// <summary>
        /// Triggered on window resize.
        /// </summary>
        event Action OnResize;

        /// <summary>
        /// Triggered when window is moved.
        /// </summary>
        event Action OnMove;

        /// <summary>
        /// Triggered on window iconify.
        /// </summary>
        event Action OnIconify;

        /// <summary>
        /// Triggered on file drop on the window.
        /// </summary>
        event WindowFileDrop OnDrop;
    }
}
