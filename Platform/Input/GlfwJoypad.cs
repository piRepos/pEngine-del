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
    public class GlfwJoypad : IJoypad
    {
        /// <summary>
        /// Makes a new instance of <see cref="GlfwJoypad"/> class.
        /// </summary>
        /// <param name="window">Current context.</param>
        public GlfwJoypad(GlfwWindow window, int index)
        {
            handler = window;
            Index = index;
        }

        /// <summary>
        /// Initialize the device.
        /// </summary>
        public void Initialize()
        {
            Name = Glfw.GetJoystickName((Glfw.Joystick)Index);
        }

        /// <summary>
        /// Private window handler.
        /// </summary>
        private GlfwWindow handler { get; }

        /// <summary>
        /// Joypad device index.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Joypad name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the state for the joypad axys.
        /// </summary>
        public double[] Axes
        {
            get
            {
                var axes = Glfw.GetJoystickAxes((Glfw.Joystick)Index);

                return axes.Select(x => (double)x).ToArray();
            }
        }

        /// <summary>
        /// Gets the state for each joypad button.
        /// </summary>
        public KeyState[] Buttons
        {
            get
            {
                var buttons = Glfw.GetJoystickButtons((Glfw.Joystick)Index);

                return buttons.Select(x => x ? KeyState.Pressed : KeyState.Released).ToArray();
            }
        }


        /// <summary>
        /// Update the state of this element
        /// </summary>
        /// <param name="DeltaTime">Game clock.</param>
        public void Update(IFrameBasedClock Clock)
        {

        }
    }
}
