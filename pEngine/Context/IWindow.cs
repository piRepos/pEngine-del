using System;
using System.Collections.Generic;

using pEngine.Timing;
using pEngine.Utils.Math;
using pEngine.Platform.Monitors;

namespace pEngine.Context
{
    /// <summary>
    /// Genery system window.
    /// </summary>
	public interface IWindow : IPhysicalObject, IScalable, IDisposable
    {
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
        /// Gets monitor window position.
        /// </summary>
        IMonitor Monitor { get; }

        /// <summary>
        /// Triggered o window restore / creation.
        /// </summary>
        event EventHandler OnRestore;

        /// <summary>
        /// Triggered on window resize.
        /// </summary>
        event EventHandler<WindowResizeEventArgs> OnResize;

        /// <summary>
        /// Triggered when window is moved.
        /// </summary>
        event EventHandler<WindowMoveEventArgs> OnMove;

        /// <summary>
        /// Triggered on window iconify.
        /// </summary>
        event EventHandler<WindowIconifyEventArgs> OnIconify;

        /// <summary>
        /// Triggered on file drop on the window.
        /// </summary>
        event EventHandler<WindowFileDropEventArgs> OnDrop;
    }

	public class WindowMoveEventArgs : EventArgs
	{
		/// <summary>
		/// Window position.
		/// </summary>
		public Vector2i Position { get; set; }
	}

	public class WindowResizeEventArgs : EventArgs
	{
		/// <summary>
		/// Window size.
		/// </summary>
		public Vector2i Size { get; set; }
	}

	public class WindowIconifyEventArgs : EventArgs
	{
		/// <summary>
		/// True if window reduced to icon.
		/// </summary>
		public bool Reduced { get; set; }
	}

	public class WindowFileDropEventArgs : EventArgs
	{
		/// <summary>
		/// True if window reduced to icon.
		/// </summary>
		public List<string> FilesPath { get; set; }
	}
}
