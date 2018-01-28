using System;
using System.Collections.Generic;

using pEngine.Platform.Input;

namespace pEngine.Input
{
    public class InputState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputState"/> class.
        /// </summary>
        public InputState()
        {
            keyState = new Dictionary<uint, KeyState>();
            positionState = new Dictionary<uint, float>();
        }

        /// <summary>
        /// Gets the keys state.
        /// </summary>
        private Dictionary<uint, KeyState> keyState { get; }

        /// <summary>
        /// Gets the position state.
        /// </summary>
        private Dictionary<uint, float> positionState { get; }

        /// <summary>
        /// Sets the state of the key.
        /// </summary>
        /// <param name="key">Key to set.</param>
        /// <param name="state">True if is pressed.</param>
        public void SetKeyState(uint key, KeyState state)
        {
            keyState[key] = state;
        }

        /// <summary>
        /// Gets the state of the key.
        /// </summary>
        /// <returns><c>true</c>, if key is pressed, <c>false</c> otherwise.</returns>
        /// <param name="key">Key to check.</param>
        public KeyState GetKeyState(uint key)
        {
            try
            {
                return keyState[key];
            }
            catch (KeyNotFoundException)
            {
                return KeyState.Unknow;
            }
        }

        /// <summary>
        /// Sets the state of the position.
        /// </summary>
        /// <param name="posKey">Position key identifier.</param>
        /// <param name="state">State.</param>
        public void SetPositionState(uint posKey, float state)
        {
            positionState[posKey] = state;
        }

        /// <summary>
        /// Gets the state of the position.
        /// </summary>
        /// <returns>The position value.</returns>
        /// <param name="posKey">Position key identifier.</param>
        public float GetPositionState(uint posKey)
        {
            try
            {
                return positionState[posKey];
            }
            catch (KeyNotFoundException)
            {
                return 0;
            }
        }
    }
}
