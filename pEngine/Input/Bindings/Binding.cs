using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Utils.Timing;
using pEngine.Utils.Timing.Base;

using pEngine.Platform.Input;

namespace pEngine.Input.Bindings
{
	public class Binding : IUpdatable
	{
		/// <summary>
		/// Makes a new instance of <see cref="Binding"/> class.
		/// </summary>
		/// <param name="name">Binding preset name.</param>
		public Binding(string name)
		{
			Name = name;
		}

		/// <summary>
		/// Binding name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Input engine.
		/// </summary>
		protected InputEngine Input { get; private set; }

		/// <summary>
		/// Initialize this key binding.
		/// </summary>
		internal void Initialize(InputEngine input)
		{
			Input = input;
		}

		/// <summary>
		/// Update the state of this element
		/// </summary>
		/// <param name="DeltaTime">Game clock.</param>
		public virtual void Update(IFrameBasedClock clock)
		{
		}
	}

	public class Binding<T> : Binding where T : struct, IConvertible
	{

		/// <summary>
		/// Makes a new instance of <see cref="Binding"/> class.
		/// </summary>
		/// <param name="name">Binding preset name.</param>
		public Binding(string name)
			: base(name)
		{
			Actions = new Dictionary<T, IEnumerable<IKeyCombination>>();

			if (!typeof(T).IsEnum)
				throw new ArgumentException("The type T must be an enum.");

			T[] values = (T[])Enum.GetValues(typeof(T));

			foreach (T v in values)
			{
				Actions.Add(v, new IKeyCombination[0]);
			}
		}

		/// <summary>
		/// Makes a new instance of <see cref="Binding"/> class from another.
		/// </summary>
		/// <param name="name">Binding preset name.</param>
		/// <param name="copy">Source binding.</param>
		public Binding(string name, Binding<T> copy)
			: this(name)
		{
			Actions = new Dictionary<T, IEnumerable<IKeyCombination>>(copy.Actions);
		}

		#region Actions management

		/// <summary>
		/// Binding actions.
		/// </summary>
		protected Dictionary<T, IEnumerable<IKeyCombination>> Actions { get; }

		/// <summary>
		/// Sets a key combination for the specified action.
		/// </summary>
		/// <param name="action">Target action.</param>
		/// <param name="keys">Key combination for action triggering.</param>
		public void SetActionKeys(T action, params IKeyCombination[] keys)
		{
			Actions[action] = keys;
		}

		#endregion

		#region Actions state

		/// <summary>
		/// Gets the state about the specified action.
		/// </summary>
		/// <param name="action">Action to check.</param>
		/// <returns>The state.</returns>
		public KeyState GetActionState(T action)
		{
			List<Enum> buttons = new List<Enum>();

			KeyState currState = KeyState.Holding;
			foreach (var keyComb in Actions[action])
			{
				switch (keyComb)
				{
					case KeyboardKeyCombination kComb:
						foreach (var key in kComb.Keys)
							currState = StateComposite(currState, Input.HardwareKeyboard.GetButtonState(key));
						break;
					case MouseKeyCombination mComb:
						foreach (var key in mComb.Keys)
							currState = StateComposite(currState, Input.HardwareMouse.GetButtonState(key));
						break;
					case JoypadKeyCombination jComb:
						IJoypad curr = Input.HardwareJoypads.Where(x => x.Index == jComb.JoypadID).FirstOrDefault();
						foreach (var key in jComb.Keys)
							currState = StateComposite(currState, curr.Buttons[key]);
						break;
				}
			}

			return currState;
		}

		#endregion

		/// <summary>
		/// Update the state of this element
		/// </summary>
		/// <param name="DeltaTime">Game clock.</param>
		public override void Update(IFrameBasedClock clock)
		{
			base.Update(clock);
		}

		/// <summary>
		/// Gets the lower state between the specified two states.
		/// </summary>
		/// <param name="oldState">Old state.</param>
		/// <param name="newState">New state.</param>
		public static KeyState StateComposite(KeyState oldState, KeyState newState)
		{
			if (oldState > newState)
				return newState;
			return oldState;
		}

	}

	public class ActionEventArgs<T> : EventArgs
	{
		
	}
}
