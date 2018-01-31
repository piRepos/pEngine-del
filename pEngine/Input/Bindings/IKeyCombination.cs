using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Platform.Input;

namespace pEngine.Input.Bindings
{
	public interface IKeyCombination { }
	public interface IKeyCombination<T> : IKeyCombination
	{
		/// <summary>
		/// Key list.
		/// </summary>
		IEnumerable<T> Keys { get; }

		/// <summary>
		/// Hash reappresenting the binding keys.
		/// </summary>
		int KeysHash { get; }
	}
}
