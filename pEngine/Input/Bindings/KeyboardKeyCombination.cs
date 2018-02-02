using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Platform.Input;

namespace pEngine.Input.Bindings
{
	public struct KeyboardKeyCombination : IKeyCombination<KeyboardKey>, IEquatable<KeyboardKeyCombination>
	{
		/// <summary>
		/// Empty key combination.
		/// </summary>
		public static KeyboardKeyCombination Empty => new KeyboardKeyCombination();

		/// <summary>
		/// Makes a new instance of <see cref="KeyboardKeyCombination"/> class.
		/// </summary>
		/// <param name="keys">Key combination.</param>
		public KeyboardKeyCombination(params KeyboardKey[] keys)
		{
			Keys = keys;
			KeysHash = keys.Sum(x => Convert.ToInt32(x)) % int.MaxValue;
		}

		/// <summary>
		/// Key list.
		/// </summary>
		public IEnumerable<KeyboardKey> Keys { get; }

		/// <summary>
		/// Hash reappresenting the binding keys.
		/// </summary>
		public int KeysHash { get; }

		#region .NET implementations

		public bool Equals(KeyboardKeyCombination other)
		{
			return Keys.Equals(other.Keys);
		}

		public override int GetHashCode()
		{
			return KeysHash;
		}

		#endregion
	}
}
