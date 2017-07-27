using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Common.Memory
{
    public interface ILogicMemory<Type>
    {

		/// <summary>
		/// Logic memory.
		/// </summary>
		Type[] Memory { get; }

    }
}
