using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Glfw3;

using pEngine.Common.Timing.Base;
using pEngine.Platform.Windows;
using pEngine.Core.Input;

namespace pEngine.Platform.Input
{
    public class GlfwDeviceManager : DeviceManager
    {
        Glfw.JoystickFunc joypadCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlfwDeviceManager"/> class.
        /// </summary>
        /// <
        public GlfwDeviceManager(GlfwWindow context)
        {
            handler = context;

            devices = new List<IDevice>();
        }

        /// <summary>
        /// Initialize all input devices.
        /// </summary>
        public override void InitializeDevices()
        {
            // - Add basic devices
            if (!devices.Any(x => x is IKeyboard))
                devices.Add(new GlfwKeyboard(handler));
            if (!devices.Any(x => x is IMouse))
				devices.Add(new GlfwMouse(handler));

            // - Gets all controllers
            devices.AddRange(GetConnectedJoypads());

            // - Add handler for controllers plug
            Glfw.SetJoystickCallback(JoypadConnectionCallback);

            base.InitializeDevices();
        }

        /// <summary>
        /// Private window handler.
        /// </summary>
        private GlfwWindow handler { get; }

        #region Device management

        private List<IDevice> devices;

        /// <summary>
        /// Lists all connected input devices.
        /// </summary>
        public override IEnumerable<IDevice> Devices => devices;

        protected override IJoypad[] GetConnectedJoypads()
        {
            List<GlfwJoypad> pads = new List<GlfwJoypad>();
            for (int i = 0; i <= (int)Glfw.Joystick.JoystickLast; ++i)
            {
                if (Glfw.JoystickPresent((Glfw.Joystick)i))
                    pads.Add(new GlfwJoypad(handler, i));
            }

            return pads.ToArray();
        }

        void JoypadConnectionCallback(Glfw.Joystick x, Glfw.ConnectionEvent state)
        {
            if (state == Glfw.ConnectionEvent.Connected)
                devices.Add(new GlfwJoypad(handler, (int)x));

            if (state == Glfw.ConnectionEvent.Disconnected)
            {
                devices.RemoveAll(g => (g is GlfwJoypad pad) ? pad.Index == (int)x : false);
            }
        }

        #endregion

        public override void UpdateDevices(IFrameBasedClock clock)
        {
            base.UpdateDevices(clock);
        }
    }
}
