using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Platform.Input;

namespace pEngine.Input.Bindings
{
	public struct MouseKeyCombination : IKeyCombination<MouseButton>, IEquatable<MouseKeyCombination>
	{
		/// <summary>
		/// Empty key combination.
		/// </summary>
		public static MouseKeyCombination Empty => new MouseKeyCombination();

		/// <summary>
		/// Makes a new instance of <see cref="MouseKeyCombination"/> class.
		/// </summary>
		/// <param name="keys">Key combination.</param>
		public MouseKeyCombination(params MouseButton[] keys)
		{
			Keys = keys;
			KeysHash = keys.Sum(x => Convert.ToInt32(x)) % int.MaxValue;
		}

		/// <summary>
		/// Key list.
		/// </summary>
		public IEnumerable<MouseButton> Keys { get; }

		/// <summary>
		/// Hash reappresenting the binding keys.
		/// </summary>
		public int KeysHash { get; }

		#region .NET implementations

		public bool Equals(MouseKeyCombination other)
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
