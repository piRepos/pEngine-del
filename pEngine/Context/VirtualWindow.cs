using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Platform.Monitors;
using pEngine.Timing.Base;
using pEngine.Framework;
using pEngine.Timing;

namespace pEngine.Context
{
	public class VirtualWindow : pObject, IWindow
	{
		/// <summary>
		/// Makes a new instance of <see cref="VirtualWindow"/> class.
		/// </summary>
		/// <param name="parent">Parent window.</param>
		internal VirtualWindow(VirtualWindow parent)
		{
			Parent = parent;
			PropertyChanged += propChanged;
			outboundActions = new Queue<Action<IPlatformWindow>>();
			inboundActions = new Queue<Action>();
		}

		/// <summary>
		/// Makes a new instance of <see cref="VirtualWindow"/> class.
		/// </summary>
		internal VirtualWindow()
			: this(null)
		{
		}

		/// <summary>
		/// Parent shared window.
		/// </summary>
		public VirtualWindow Parent { get; }

		#region Real window handler

		IPlatformWindow handler;

		/// <summary>
		/// 
		/// </summary>
		internal IPlatformWindow RealWindow
		{
			get => handler;
			set
			{
				if (handler != null)
				{
					handler.OnDrop -= Remote_OnDrop;
					handler.OnIconify -= Remote_OnIconify;
					handler.OnMove -= Remote_OnMove;
					handler.OnResize -= Remote_OnResize;
					handler.OnRestore -= Remote_OnRestore;
					handler.PropertyChanged -= Remote_PropertyChanged;
				}

				handler = value;
				handler.OnDrop += Remote_OnDrop;
				handler.OnIconify += Remote_OnIconify;
				handler.OnMove += Remote_OnMove;
				handler.OnResize += Remote_OnResize;
				handler.OnRestore += Remote_OnRestore;
				handler.PropertyChanged += Remote_PropertyChanged;
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Window title.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// True when window is in icon state.
		/// </summary>
		public bool IsIconified { get; }

		/// <summary>
		/// True when window is visible.
		/// </summary>
		public bool Visible { get; }

		/// <summary>
		/// True when needs to be closed.
		/// </summary>
		public bool ShouldClose { get; }

		/// <summary>
		/// Make window fullscreen or not.
		/// </summary>
		public bool Fullscreen { get; set; }

		/// <summary>
		/// Gets monitor window position.
		/// </summary>
		public IMonitor Monitor { get; private set; }

		/// <summary>
		/// Window position.
		/// </summary>
		public Vector2i Position { get; set; }

		/// <summary>
		/// Window size.
		/// </summary>
		public Vector2i Size { get; set; }

		#endregion

		#region Events

		/// <summary>
		/// Triggered o window restore / creation.
		/// </summary>
		public event EventHandler OnRestore;

		/// <summary>
		/// Triggered on window resize.
		/// </summary>
		public event EventHandler<WindowResizeEventArgs> OnResize;

		/// <summary>
		/// Triggered when window is moved.
		/// </summary>
		public event EventHandler<WindowMoveEventArgs> OnMove;

		/// <summary>
		/// Triggered on window iconify.
		/// </summary>
		public event EventHandler<WindowIconifyEventArgs> OnIconify;

		/// <summary>
		/// Triggered on file drop on the window.
		/// </summary>
		public event EventHandler<WindowFileDropEventArgs> OnDrop;

		#endregion

		#region Methods

		/// <summary>
		/// Close this window.
		/// </summary>
		public void Close()
		{
			outboundActions.Enqueue((x) =>
			{
				x.Close();
			});
		}

		/// <summary>
		/// Make visible the window.
		/// </summary>
		public void Show()
		{
			outboundActions.Enqueue((x) =>
			{
				x.Show();
			});
		}

		/// <summary>
		/// Hide this window.
		/// </summary>
		public void Hide()
		{
			outboundActions.Enqueue((x) =>
			{
				x.Hide();
			});
		}

		/// <summary>
		/// Close and make a new window.
		/// </summary>
		public void Restore()
		{
			outboundActions.Enqueue((x) =>
			{
				x.Restore();
			});
		}

		/// <summary>
		/// Initialize this window.
		/// </summary>
		public void Make()
		{
			outboundActions.Enqueue((x) =>
			{
				x.Make();
			});
		}

		#endregion

		Queue<Action<IPlatformWindow>> outboundActions;
		Queue<Action> inboundActions;

		internal void ApplyChanges()
		{
			if (RealWindow == null)
				return;

			while (outboundActions.Count() > 0)
			{
				var act = outboundActions.Dequeue();
				act.Invoke(RealWindow);
			}
		}

		internal void SendChanges()
		{
			if (RealWindow == null)
				return;

			while (inboundActions.Count() > 0)
			{
				var act = inboundActions.Dequeue();
				act.Invoke();
			}
		}

		private void propChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
				case "Title": outboundActions.Enqueue((x) => { x.Title = Title; }); break;
				case "Fullscreen": outboundActions.Enqueue((x) => { x.Fullscreen = Fullscreen; }); break;
				case "Position": outboundActions.Enqueue((x) => { x.Position = Position; }); break;
				case "Size": outboundActions.Enqueue((x) => { x.Size = Size; }); break;
				default:
					break;
			}
		}

		private void Remote_OnRestore(object sender, EventArgs e)
		{
			inboundActions.Enqueue(() => OnRestore?.Invoke(this, e));
		}

		private void Remote_OnResize(object sender, WindowResizeEventArgs e)
		{
			inboundActions.Enqueue(() => OnRestore?.Invoke(this, e));
		}

		private void Remote_OnMove(object sender, WindowMoveEventArgs e)
		{
			inboundActions.Enqueue(() => OnMove?.Invoke(this, e));
		}

		private void Remote_OnIconify(object sender, WindowIconifyEventArgs e)
		{
			inboundActions.Enqueue(() => OnIconify?.Invoke(this, e));
		}

		private void Remote_OnDrop(object sender, WindowFileDropEventArgs e)
		{
			inboundActions.Enqueue(() => OnDrop?.Invoke(this, e));
		}

		private void Remote_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Title": inboundActions.Enqueue(() => { Title = RealWindow.Title; }); break;
				case "Fullscreen": inboundActions.Enqueue(() => { Fullscreen = RealWindow.Fullscreen; }); break;
				case "Position": inboundActions.Enqueue(() => { Position = RealWindow.Position; }); break;
				case "Size": inboundActions.Enqueue(() => { Size = RealWindow.Size; }); break;
				case "Monitor": inboundActions.Enqueue(() => { Monitor = RealWindow.Monitor; }); break;
				default:
					break;
			}
		}
	}
}
