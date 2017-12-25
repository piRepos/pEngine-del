using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Utils.Memory
{
    public interface ILogicMemory<Type>
    {

		/// <summary>
		/// Logic memory.
		/// </summary>
		Type[] Memory { get; }

    }
}
