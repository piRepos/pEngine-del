using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Framework;
using pEngine.Framework.Modules;

using pEngine.Utils.Threading;
using pEngine.Utils.Timing.Base;

using pEngine.Platform.Input;

namespace pEngine.Input
{
	public class InputService : Service
	{
		/// <summary>
		/// Makes a new instance of <see cref="InputService"/> class.
		/// </summary>
		/// <param name="module">Owner module.</param>
		/// <param name="mainScheduler">Scheduler for this service.</param>
		public InputService(Module module, Scheduler mainScheduler) 
			: base(module, mainScheduler)
		{
		}


	}

	public class InputEngine : Module
	{
		/// <summary>
		/// Makes a new instance of <see cref="InputEngine"/> class.
		/// </summary>
		/// <param name="host">Game host owner.</param>
		/// <param name="scheduler">Running thread scheduler.</param>
		public InputEngine(GameHost host, Scheduler scheduler) 
			: base(host, scheduler)
		{
            InputStates = new Dictionary<IDevice, InputState>();
		}

		/// <summary>
		/// Gets the keyboard manager.
		/// </summary>
		public IKeyboard HardwareKeyboard => Host.Input.Keyboard;

		/// <summary>
		/// Gets the mouse manager.
		/// </summary>
		public IMouse HardwareMouse => Host.Input.Mouse;

		/// <summary>
		/// Gets the joypads.
		/// </summary>
		public IEnumerable<IJoypad> HardwareJoypads => Host.Input.Joypads;

		/// <summary>
		/// Initialize this module.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

            InputStates.Add(HardwareKeyboard, new InputState());
            InputStates.Add(HardwareMouse, new InputState());

            HardwareKeyboard.OnKeyEvent += HardwareKeyboard_OnKeyEvent;
            HardwareMouse.OnButtonEvent += HardwareMouse_OnButtonEvent;
		}

		/// <summary>
		/// Update the state of this element.
		/// </summary>
		/// <param name="clock">Game clock.</param>
		public override void Update(IFrameBasedClock clock)
		{
			base.Update(clock);

            // - Remove unused joypads
            var removed = InputStates.Keys.Except(HardwareJoypads);
            foreach (IJoypad pad in removed)
                InputStates.Remove(pad);

            // - Update joypad states
            foreach (IJoypad pad in HardwareJoypads)
            {
                if (!InputStates.ContainsKey(pad))
                    InputStates.Add(pad, new InputState());

                for (uint i = 0; i < pad.Buttons.Count(); ++i)
                    InputStates[pad].SetKeyState(i, pad.Buttons[i]);

                for (uint i = 0; i < pad.Axes.Count(); ++i)
                    InputStates[pad].SetPositionState(i, (float)pad.Axes[i]);
            }
		}

        #region Gets state

        /// <summary>
        /// Gets the input states.
        /// </summary>
        private Dictionary<IDevice, InputState> InputStates { get; }

        void HardwareKeyboard_OnKeyEvent(KeyboardKey key, int scancode, KeyState action, KeyModifier modifiers)
        {
            InputStates[HardwareKeyboard].SetKeyState((uint)key, action);
        }

        void HardwareMouse_OnButtonEvent(MouseButton button, KeyState action, KeyModifier modifiers)
        {
            InputStates[HardwareMouse].SetKeyState((uint)button, action);
        }

        #endregion

        #region Service

        /// <summary>
        /// Settings for this module.
        /// </summary>
        public override Service GetSettings(Scheduler mainScheduler)
		{
			InputService s = new InputService(this, mainScheduler);
			s.Initialize();
			return s;
		}

		#endregion
	}
}
