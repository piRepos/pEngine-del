using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Utils.Memory
{
    public interface ILogicMemoryPointer<Type>
    {

		/// <summary>
		/// Offset in the memory.
		/// </summary>
		long Offset { get; }

		/// <summary>
		/// Block size.
		/// </summary>
		long Size { get; }

		/// <summary>
		/// Reference memory.
		/// </summary>
		ILogicMemory<Type> MemoryRef { get; }

		

    }
}
