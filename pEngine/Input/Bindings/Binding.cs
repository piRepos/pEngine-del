using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEngine.Input.Bindings
{
	public class Binding
	{
		/// <summary>
		/// Makes a new instance of <see cref="Binding"/> class.
		/// </summary>
		/// <param name="action">Action triggered from this binding.</param>
		/// <param name="keys">Key combination.</param>
		public Binding(Enum action, params Enum[] keys)
		{
			Action = action;
			Keys = keys;
			KeysHash = CalculateKeyHash(Keys);
		}

		/// <summary>
		/// Action associated to this binding.
		/// </summary>
		public Enum Action { get; set; }

		/// <summary>
		/// Key list.
		/// </summary>
		public IEnumerable<Enum> Keys { get; }

		/// <summary>
		/// Hash reappresenting the binding keys.
		/// </summary>
		public int KeysHash { get; }

		/// <summary>
		/// Calculate an hash number for this key combination.
		/// </summary>
		/// <param name="keys">A key combination.</param>
		/// <returns>An hash number</returns>
		static int CalculateKeyHash(IEnumerable<Enum> keys)
		{
			return keys.Sum(x => Convert.ToInt32(x)) % int.MaxValue;
		}
	}
}
