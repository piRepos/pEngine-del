using System;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using pEngine.Framework;
using pEngine.Framework.Modules;

using pEngine.Utils.Threading;
using pEngine.Utils.Timing.Base;

using pEngine.Platform.Input;

using pEngine.Input.Virtual;
using pEngine.Input.Bindings;

#pragma warning disable CS0067

namespace pEngine.Input
{
	using DeviceStates = Dictionary<IDevice, InputState>;

	public class InputService : Service
	{
		/// <summary>
		/// Makes a new instance of <see cref="InputService"/> class.
		/// </summary>
		/// <param name="module">Owner module.</param>
		/// <param name="mainScheduler">Scheduler for this service.</param>
		public InputService(Module module, GameLoop mainLoop) 
			: base(module, mainLoop)
		{
			inputHistory = new ConcurrentQueue<DeviceStates>();
			currentInputHistory = new List<DeviceStates>();

			Keyboard = new VirtualKeyboard(currentInputState?[Module.HardwareKeyboard]);
			Mouse = new VirtualMouse(currentInputState?[Module.HardwareMouse]);
			Joypads = new List<VirtualJoypad>();

			foreach (var j in Module.HardwareJoypads)
				Joypads.Add(new VirtualJoypad(currentInputState?[j], j.Index, j.Name));
		}

		public override void Initialize()
		{
			base.Initialize();

			JoypadConnection += (obj, e) =>
			{
				if (e.Connected)
					Joypads.Add(new VirtualJoypad(currentInputState?[e.Joypad], e.Joypad.Index, e.Joypad.Name));
				else
					Joypads.RemoveAt(Joypads.FindIndex(x => x.Index == e.Joypad.Index));
			};

			RunningLoop.Scheduler.AddDelayed(() =>
			{
				while (inputHistory.Count > 0)
				{
					currentInputHistory.Clear();
					inputHistory.TryDequeue(out DeviceStates state);
					currentInputHistory.Add(state);
				}

				Mouse.UpdateCurrentState(currentInputState?[Module.HardwareMouse]);
				Keyboard.UpdateCurrentState(currentInputState?[Module.HardwareKeyboard]);

				Mouse.Update(RunningLoop.Clock);
				Keyboard.Update(RunningLoop.Clock);

				foreach (var j in Joypads)
				{
					j.UpdateCurrentState(currentInputState?[Module.HardwareJoypads.Where(x => x.Index == j.Index).FirstOrDefault()]);
					j.Update(RunningLoop.Clock);
				}


			}, 0, true);
		}

		/// <summary>
		/// Input engine.
		/// </summary>
		public new InputEngine Module => base.Module as InputEngine;

		#region Devices

		/// <summary>
		/// Gets the keyboard manager.
		/// </summary>
		public VirtualKeyboard Keyboard { get; }

		/// <summary>
		/// Gets the mouse manager.
		/// </summary>
		public VirtualMouse Mouse { get; }

		/// <summary>
		/// Gets the joypads.
		/// </summary>
		public List<VirtualJoypad> Joypads { get; }

		/// <summary>
		/// Triggered on a joypad connection or disconnection.
		/// </summary>
		[ServiceEvent("JoypadConnection")]
		public event EventHandler<JoypadConnectionEventArgs> JoypadConnection;

		#endregion

		#region Bindings

		/// <summary>
		/// Key bindings.
		/// </summary>
		[ServiceProperty("bindings")]
		public IEnumerable<Binding> Bindings { get; protected set; }

		/// <summary>
		/// Current active binding.
		/// </summary>
		[ServiceProperty("CurrentBinding")]
		public IEnumerable<Binding> CurrentBinding { get; protected set; }

		/// <summary>
		/// Insert a binding in the input engine.
		/// </summary>
		/// <param name="b">Binding to add.</param>
		[ServiceMethod(ReferencesTo = "AddBinding")]
		public void AddBinding(Binding b) { }

		/// <summary>
		/// Remove the specified binding.
		/// </summary>
		/// <param name="b">Binding to remove.</param>
		[ServiceMethod(ReferencesTo = "RemoveBinding")]
		public void RemoveBinding(Binding b) { }

		/// <summary>
		/// Set the specified binding as current binding.
		/// </summary>
		/// <param name="b">Binding to set.</param>
		[ServiceMethod(ReferencesTo = "SetAsCurrentBnding")]
		public void SetAsCurrentBnding(Binding b) { }

		#endregion

		#region Sharing data

		/// <summary>
		/// Gets the input states.
		/// </summary>
		[ServiceProperty("InputStates")]
		internal DeviceStates SourceState { get; private set; }

		ConcurrentQueue<DeviceStates> inputHistory;
		List<DeviceStates> currentInputHistory;
		DeviceStates currentInputState => currentInputHistory.Count() > 0 ? currentInputHistory.Last() : null;

		/// <summary>
		/// Update the state of this element.
		/// </summary>
		/// <param name="clock">Game clock.</param>
		public override void Update(IFrameBasedClock clock)
		{
			inputHistory.Enqueue(SourceState);
		}

		#endregion
	}

	public class InputEngine : Module
	{
		/// <summary>
		/// Makes a new instance of <see cref="InputEngine"/> class.
		/// </summary>
		/// <param name="host">Game host owner.</param>
		/// <param name="moduleLoop">Running loop.</param>
		public InputEngine(GameHost host, GameLoop moduleLoop) 
			: base(host, moduleLoop)
		{
            InputStates = new DeviceStates();
			bindings = new List<Binding>();
		}

		/// <summary>
		/// Initialize this module.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			Host.Platform.Input.JoypadConnection += (obj, e) =>
			{
				if (!InputStates.ContainsKey(e.Joypad))
					InputStates.Add(e.Joypad, new InputState());

				JoypadConnection?.Invoke(obj, e);
			};

            InputStates.Add(HardwareKeyboard, new InputState());
            InputStates.Add(HardwareMouse, new InputState());

			HardwareKeyboard.OnTypeWithMods += HardwareKeyboard_OnTypeWithMods;
			HardwareKeyboard.OnKeyEvent += HardwareKeyboard_OnKeyEvent;
			HardwareKeyboard.OnType += HardwareKeyboard_OnType;

			HardwareMouse.OnButtonEvent += HardwareMouse_OnButtonEvent;
			HardwareMouse.OnScroll += HardwareMouse_OnScroll;
			HardwareMouse.OnMove += HardwareMouse_OnMove;

		}

		#region Bindings

		private List<Binding> bindings { get; }

		/// <summary>
		/// Current active binding.
		/// </summary>
		public Binding CurrentBinding { get; private set; }

		/// <summary>
		/// Key bindings.
		/// </summary>
		public IEnumerable<Binding> Bindings => bindings;

		/// <summary>
		/// Insert a binding in the input engine.
		/// </summary>
		/// <param name="b">Binding to add.</param>
		public void AddBinding(Binding b)
		{
			b.Initialize(this);



			bindings.Add(b);
		}

		/// <summary>
		/// Set the specified binding as current binding.
		/// </summary>
		/// <param name="b">Binding to set.</param>
		public void SetAsCurrentBnding(Binding b)
		{
			if (!Bindings.Contains(b))
				throw new InvalidOperationException("This binding was not added.");

			CurrentBinding = b;
		}

		/// <summary>
		/// Remove the specified binding.
		/// </summary>
		/// <param name="b">Binding to remove.</param>
		public void RemoveBinding(Binding b)
		{
			bindings.Remove(b);
		}

		#endregion

		#region Devices

		/// <summary>
		/// Gets the keyboard manager.
		/// </summary>
		public IKeyboard HardwareKeyboard => Host.Platform.Input.Keyboard;

		/// <summary>
		/// Gets the mouse manager.
		/// </summary>
		public IMouse HardwareMouse => Host.Platform.Input.Mouse;

		/// <summary>
		/// Gets the joypads.
		/// </summary>
		public IEnumerable<IJoypad> HardwareJoypads => Host.Platform.Input.Joypads;

		/// <summary>
		/// Triggered on a joypad connection or disconnection.
		/// </summary>
		public event EventHandler<JoypadConnectionEventArgs> JoypadConnection;

		#endregion

		#region Gets state

		/// <summary>
		/// Gets the input states.
		/// </summary>
		private DeviceStates InputStates { get; }

		private void HardwareMouse_OnButtonEvent(object sender, MouseKeyEventArgs e)
		{
			InputStates[HardwareMouse].Events.Enqueue(new InputEvent<MouseKeyEventArgs>
			{
				Name = "OnButtonEvent",
				Info = e
			});

			InputStates[HardwareMouse].SetKeyState((uint)e.Key, e.Action);
		}

		private void HardwareMouse_OnScroll(object sender, MouseOffsetEventArgs e)
		{
			InputStates[HardwareMouse].Events.Enqueue(new InputEvent<MouseOffsetEventArgs>
			{
				Name = "OnScroll",
				Info = e
			});
		}

		private void HardwareMouse_OnMove(object sender, MousePositiontEventArgs e)
		{
			InputStates[HardwareMouse].Events.Enqueue(new InputEvent<MousePositiontEventArgs>
			{
				Name = "OnMove",
				Info = e
			});
		}

		private void HardwareKeyboard_OnKeyEvent(object sender, KeyboardKeyEventArgs e)
		{
			InputStates[HardwareKeyboard].Events.Enqueue(new InputEvent<KeyboardKeyEventArgs>
			{
				Name = "OnKeyEvent",
				Info = e
			});

			InputStates[HardwareKeyboard].SetKeyState((uint)e.Key, e.Action);
		}

		private void HardwareKeyboard_OnTypeWithMods(object sender, KeyboardModTypeEventArgs e)
		{
			InputStates[HardwareKeyboard].Events.Enqueue(new InputEvent<KeyboardModTypeEventArgs>
			{
				Name = "OnModType",
				Info = e
			});
		}

		private void HardwareKeyboard_OnType(object sender, KeyboardTypeEventArgs e)
		{
			InputStates[HardwareKeyboard].Events.Enqueue(new InputEvent<KeyboardTypeEventArgs>
			{
				Name = "OnType",
				Info = e
			});
		}

		#endregion

		#region Service

		/// <summary>
		/// Settings for this module.
		/// </summary>
		public override Service GetSettings(GameLoop mainLoop)
		{
			InputService s = new InputService(this, mainLoop);
			s.Initialize();
			Services.Add(s);
			return s;
		}

		#endregion

		/// <summary>
		/// Update the state of this element.
		/// </summary>
		/// <param name="clock">Game clock.</param>
		public override void Update(IFrameBasedClock clock)
		{
			base.Update(clock);

			InputStates[HardwareKeyboard].Time = clock.CurrentTime;
			InputStates[HardwareMouse].Time = clock.CurrentTime;

			// - Remove unused joypads
			var removed = InputStates.Keys.Where(x => x is IJoypad).Except(HardwareJoypads);
            foreach (IJoypad pad in removed)
                InputStates.Remove(pad);

            // - Update joypad states
            foreach (IJoypad pad in HardwareJoypads)
            {
				if (!InputStates.ContainsKey(pad))
					InputStates.Add(pad, new InputState());

				for (uint i = 0; i < pad.Buttons.Count(); ++i)
				{
					if (pad.Buttons[i] != InputStates[pad].GetKeyState(i))
					{
						InputStates[pad].Events.Enqueue(new InputEvent<JoypadKeyEventArgs>
						{
							Name = "OnButtonPress",
							Info = new JoypadKeyEventArgs
							{
								Key = (int)i,
								Action = pad.Buttons[i]
							}
						});
					}

					InputStates[pad].SetKeyState(i, pad.Buttons[i]);
				}

				for (uint i = 0; i < pad.Axes.Count(); ++i)
				{
					if (pad.Axes[i] != InputStates[pad].GetPositionState(i))
					{
						InputStates[pad].Events.Enqueue(new InputEvent<JoypadAxeEventArgs>
						{
							Name = "OnAxeMovement",
							Info = new JoypadAxeEventArgs
							{
								Axe = (int)i,
								Value = pad.Axes[i]
							}
						});
					}

					InputStates[pad].SetPositionState(i, (float)pad.Axes[i]);
				}

				InputStates[pad].Time = clock.CurrentTime;

			}

			// - Update binding
			CurrentBinding?.Update(clock);
		}
	}
}
