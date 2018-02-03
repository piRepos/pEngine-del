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
		/// Triggered on binding state change.
		/// </summary>
		public event EventHandler<ActionEventArgs> OnAction;

		/// <summary>
		/// Send an event for this action state.
		/// </summary>
		/// <param name="action">The current action state.</param>
		protected void TriggerAction(ActionEventArgs action)
		{
			OnAction?.Invoke(this, action);
		}

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
			actionStates = new Dictionary<T, KeyState>();

			if (!typeof(T).IsEnum)
				throw new ArgumentException("The type T must be an enum.");

			T[] values = (T[])Enum.GetValues(typeof(T));

			foreach (T v in values)
			{
				Actions.Add(v, new IKeyCombination[0]);
				actionStates.Add(v, KeyState.Released);
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
							currState = StateComposite(currState, Input.Keyboard.GetButtonState(key));
						break;
					case MouseKeyCombination mComb:
						foreach (var key in mComb.Keys)
							currState = StateComposite(currState, Input.Mouse.GetButtonState(key));
						break;
					case JoypadKeyCombination jComb:
						IJoypad curr = Input.Joypads.Where(x => x.Index == jComb.JoypadID).FirstOrDefault();
						foreach (var key in jComb.Keys)
							currState = StateComposite(currState, curr.Buttons[key]);
						break;
					default:
						break;
				}
			}

			return currState;
		}

		#endregion

		#region Events management

		private readonly Dictionary<T, KeyState> actionStates;

		private void ComputeActionEvent(T action)
		{
			KeyState currState = GetActionState(action);
			if (actionStates[action] != currState)
			{
				actionStates[action] = currState;
				TriggerAction(new ActionEventArgs<T>
				{
					RawBindingAction = (int)(object)action,
					Action = currState
				});
			}
		}

		#endregion

		/// <summary>
		/// Update the state of this element
		/// </summary>
		/// <param name="DeltaTime">Game clock.</param>
		public override void Update(IFrameBasedClock clock)
		{
			base.Update(clock);

			foreach (var action in Actions)
			{
				ComputeActionEvent(action.Key);
			}
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

	public class ActionEventArgs : EventArgs
	{
		/// <summary>
		/// Action integer.
		/// </summary>
		public int RawBindingAction { get; set; }

		/// <summary>
		/// Action state.
		/// </summary>
		public KeyState Action { get; set; }
	}

	public class ActionEventArgs<T> : ActionEventArgs where T : struct, IConvertible
	{
		/// <summary>
		/// Binding action.
		/// </summary>
		public T BindingAction => (T)(object)RawBindingAction;
	}
}
