using System;
using System.Collections.Generic;
using System.Security;
using System.Runtime.InteropServices;
using System.Threading;

using Glfw3;

using pEngine.Context;
using pEngine.Framework;

using pEngine.Platform.Monitors;

namespace pEngine.Platform.Forms
{
    public class GlfwWindow : pObject, IPlatformWindow
	{

        /// <summary>
        /// Makes a new GLFW window.
        /// </summary>
		/// <param name="shared">Parent window.</param>
		public GlfwWindow(GlfwWindow shared)
        {
			SharedContext = shared;
			Size = new Vector2i(700);
            Title = "";
        }

		/// <summary>
		/// Dispose all resources used from this class.
		/// </summary>
		/// <param name="disposing">Dispose managed resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Close();
			}

			base.Dispose(disposing);
		}

		#region Initialization

		/// <summary>
		/// True if window is created.
		/// </summary>
		public bool Initialized { get; private set; }

        /// <summary>
        /// Window handler.
        /// </summary>
        public Glfw.Window Handle { get; private set; }

        /// <summary>
        /// Window dependency.
        /// </summary>
        public GlfwWindow SharedContext { get; private set; }

        /// <summary>
        /// Triggered o window restore / creation.
        /// </summary>
        public event EventHandler OnRestore;

        private void Initialize()
        {
            if (Initialized)
                return;

            OperatingSystem OS = System.Environment.OSVersion;

            if (OS.Platform == PlatformID.Unix)
            {
                Glfw.WindowHint(Glfw.Hint.ContextVersionMajor, 3);
                Glfw.WindowHint(Glfw.Hint.ContextVersionMinor, 2);

                Glfw.WindowHint(Glfw.Hint.OpenglForwardCompat, 1);
                Glfw.WindowHint(Glfw.Hint.OpenglProfile, Glfw.OpenGLProfile.Core);
            }

            Glfw.WindowHint(Glfw.Hint.Samples, 4);
            Glfw.WindowHint(Glfw.Hint.Visible, false);
            Glfw.WindowHint((Glfw.Hint)Glfw.WindowAttrib.Resizable, true);

			GlfwMonitor primaryMonitor = (Monitor ?? Environment.Platform.MainMonitor) as GlfwMonitor;

			Glfw.Monitor monitor = Fullscreen ?	primaryMonitor.Handle : Glfw.Monitor.None;
            Glfw.Window Shared = SharedContext?.Handle ?? Glfw.Window.None;

			Handle = Glfw.CreateWindow(Size.Width, Size.Height, Title, monitor, Shared);

            Glfw.MakeContextCurrent(Handle);

			// - Screen center
			Position = (primaryMonitor.CurrentResolution.ResolutionSize - Size) / 2;

            Initialized = true;

            InitializePositionEvents();
			// InitializeDragDrop();

            Glfw.MakeContextCurrent(Glfw.Window.None);
        }

        #endregion

        #region Management

        /// <summary>
        /// True when window is visible.
        /// </summary>
        public bool Visible { get; private set; }

        /// <summary>
        /// True when needs to be closed.
        /// </summary>
        public bool ShouldClose
        {
            get
            {
				if (!Initialized)
					throw new InvalidOperationException("Window not initialized.");
				
                return Glfw.WindowShouldClose(Handle);
            }
        }

        /// <summary>
        /// Close this window.
        /// </summary>
        public void Close()
        {
            if (!Initialized)
				throw new InvalidOperationException("Window not initialized.");

            Glfw.DestroyWindow(Handle);
            Initialized = false;
            Visible = false;
        }

        /// <summary>
        /// Make visible the window.
        /// </summary>
        public void Show()
        {
            if (!Initialized)
				throw new InvalidOperationException("Window not initialized.");

            Glfw.ShowWindow(Handle);
            Visible = true;
        }

        /// <summary>
        /// Hide this window.
        /// </summary>
        public void Hide()
        {
            if(!Initialized)
				throw new InvalidOperationException("Window not initialized.");

            Glfw.HideWindow(Handle);
            Visible = false;
        }

        /// <summary>
        /// Close and make a new window.
        /// </summary>
        public void Restore()
        {
            if (!Initialized)
				throw new InvalidOperationException("Window not initialized.");

            Close();
            Make();
            Show();

			OnRestore?.Invoke(this, EventArgs.Empty);
		}

        /// <summary>
        /// Initialize this window.
        /// </summary>
        public void Make()
        {
            Visible = false;
            Initialize();
        }

		#endregion

		#region Buffer

        /// <summary>
        /// Current window framebuffer size
        /// </summary>
        public Vector2i BufferSize
        {
            get
            {
                int x = 0, y = 0;
                Glfw.GetFramebufferSize(Handle, out x, out y);
                return new Vector2i(x, y);
            }
        }
        
        #endregion

        #region System messages

        /// <summary>
        /// Handle all system messages.
        /// </summary>
        public void PollMesages(bool blocking)
        {
            if (blocking)
                Glfw.WaitEvents();
            else Glfw.PollEvents();
        }

		#endregion

		#region Resolution & monitor

		private bool fullscreen;

        private Glfw.WindowPosFunc MovFun;
		private Glfw.WindowSizeFunc SizFun;

        private void InitializePositionEvents()
        {
			MovFun = (Glfw.Window P, int X, int Y) =>
			{
				foreach (IMonitor monitor in Environment.Platform.AvaiableMonitors)
				{
					if (Position > monitor.Position && Position < monitor.Position + monitor.CurrentResolution.ResolutionSize)
						Monitor = monitor;
				}

				OnMove?.Invoke(this, new WindowMoveEventArgs { Position = new Vector2i(X, Y) });
			};

			SizFun = (Glfw.Window P, int W, int H) =>
			{
				foreach (IMonitor monitor in Environment.Platform.AvaiableMonitors)
				{
					if (Position > monitor.Position && Position < monitor.Position + monitor.CurrentResolution.ResolutionSize)
						Monitor = monitor;
				}

				OnResize?.Invoke(this, new WindowResizeEventArgs { Size = new Vector2i(W, H) });
			};

			Glfw.SetWindowPosCallback(Handle, MovFun);
			Glfw.SetWindowSizeCallback(Handle, SizFun);
        }

        /// <summary>
        /// Get monitor window position.
        /// </summary>
        public IMonitor Monitor { private set; get; }

        /// <summary>
        /// Gets or sets window position.
        /// </summary>
        public Vector2i Position
        {
            get
            {
                int X, Y;
                Glfw.GetWindowPos(Handle, out X, out Y);
                return new Vector2i(X, Y);
            }
            set
            {
                Glfw.SetWindowPos(Handle, value.X, value.Y);
            }
        }

        /// <summary>
        /// Make window fullscreen or not.
        /// </summary>
        public bool Fullscreen
		{
			get { return fullscreen; }
			set
			{
				if (value != fullscreen)
				{
					fullscreen = value;
					Restore();
				}
			}
		}

        /// <summary>
        /// Triggered on window resize.
        /// </summary>
        public event EventHandler<WindowResizeEventArgs> OnResize;

        /// <summary>
        /// Triggered when window is moved.
        /// </summary>
        public event EventHandler<WindowMoveEventArgs> OnMove;

        /// <summary>
        /// Buffer scaling.
        /// </summary>
        public Vector2 Scaling => BufferSize / Size;

		Vector2i size;

        /// <summary>
        /// Window size.
        /// </summary>
		public Vector2i Size
		{
			get
			{
				return size;
			}
			set
			{
				size = value;
				if (Initialized)
					Glfw.SetWindowSize(Handle, value.X, value.Y);
			}
		}

		#endregion

		#region Context

		/// <summary>
		/// Swap backbuffer with frontbuffer.
		/// </summary>
		public void SwapBuffer()
        {
			if (!Initialized)
				throw new InvalidOperationException("Window not initialized.");
			
            Glfw.SwapBuffers(Handle);
        }

		/// <summary>
		/// Set the context on the current thread.
		/// </summary>
		public void SetContext()
		{
			Glfw.MakeContextCurrent(Handle);
		}

		#endregion

		#region Meta

		string windowTitle;

        /// <summary>
        /// Window title.
        /// </summary>
        public string Title
		{
			get { return windowTitle; }
			set
			{
				if (value != windowTitle)
				{
					windowTitle = value;
					if (Initialized)
						Glfw.SetWindowTitle(Handle, windowTitle);
				}
			}
		}

        #endregion

        #region Iconify

        private Glfw.WindowIconifyFunc IconifyFun;

        /// <summary>
        /// Triggered on iconify switch.
        /// </summary>
        public event EventHandler<WindowIconifyEventArgs> OnIconify;

        /// <summary>
        /// True when window is in icon state.
        /// </summary>
        public bool IsIconified { get; private set; }

        private void Iconify(Glfw.Window handle, bool iconified)
        {
            IsIconified = iconified;
            OnIconify?.Invoke(this, new WindowIconifyEventArgs { Reduced = iconified });
        }

        private void InitializeIconificationEvent(Glfw.Window handle)
        {
            IconifyFun = new Glfw.WindowIconifyFunc(Iconify);
            Glfw.SetWindowIconifyCallback(handle, IconifyFun);
        }

        #endregion

        #region Drag & drop

        Glfw.DropFunc DropDelegate;

        /// <summary>
        /// Triggered on file drop.
        /// </summary>
        public event EventHandler<WindowFileDropEventArgs> OnDrop;

        private void Drop(Glfw.Window window, int num, string[] files)
        {
            OnDrop?.Invoke(this, new WindowFileDropEventArgs { FilesPath = new List<string>(files) });
        }

        private void InitializeDragDrop()
        {
            DropDelegate = new Glfw.DropFunc(Drop);

            Glfw.SetDropCallback(Handle, DropDelegate);
        }

		#endregion
	}
}
