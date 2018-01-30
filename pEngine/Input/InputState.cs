using System;
using System.Collections.Generic;

using pEngine.Platform.Input;

namespace pEngine.Input
{
    internal class InputState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputState"/> class.
        /// </summary>
        public InputState()
        {
			KeyState = new Dictionary<uint, KeyState>();
			PositionState = new Dictionary<uint, float>();
			Events = new Queue<IInputEvent>();
        }

        /// <summary>
        /// Gets the keys state.
        /// </summary>
        public Dictionary<uint, KeyState> KeyState { get; }

        /// <summary>
        /// Gets the position state.
        /// </summary>
        public Dictionary<uint, float> PositionState { get; }

		/// <summary>
		/// Event queue.
		/// </summary>
		public Queue<IInputEvent> Events { get; }

		/// <summary>
		/// Current time in the input thread.
		/// </summary>
		public double Time { get; set; }

        /// <summary>
        /// Sets the state of the key.
        /// </summary>
        /// <param name="key">Key to set.</param>
        /// <param name="state">True if is pressed.</param>
        public void SetKeyState(uint key, KeyState state)
        {
			KeyState[key] = state;
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
                return KeyState[key];
            }
            catch (KeyNotFoundException)
            {
                return Platform.Input.KeyState.Unknow;
            }
        }

        /// <summary>
        /// Sets the state of the position.
        /// </summary>
        /// <param name="posKey">Position key identifier.</param>
        /// <param name="state">State.</param>
        public void SetPositionState(uint posKey, float state)
        {
			PositionState[posKey] = state;
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
                return PositionState[posKey];
            }
            catch (KeyNotFoundException)
            {
                return 0;
            }
        }
    }

	public interface IInputEvent { }

	public struct InputEvent<T> : IInputEvent where T : EventArgs
	{
		/// <summary>
		/// Event name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Input control state.
		/// </summary>
		public T Info { get; set; }
	}
}
