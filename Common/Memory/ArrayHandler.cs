// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System;
using System.Collections;
using System.Collections.Generic;

namespace pEngine.Common.Memory
{
	public class ArrayHandler<Type> : ILogicMemoryPointer<Type>, IEnumerable<Type>, IDisposable
	{

		/// <summary>
		/// Make a new <see cref="ArrayHandler{Type}"/>.
		/// </summary>
		/// <param name="Parent">Parent heap.</param>
		/// <param name="Offset">Offset in the parent heap.</param>
		/// <param name="Size">Size of this chunk of data.</param>
		public ArrayHandler(DistributedArray<Type> parent, long offset, long size)
		{
			MemoryRef = memory = parent;
			Size = size;
			Offset = offset;
		}

		/// <summary>
		/// Remove this element.
		/// </summary>
		public void Dispose()
		{
			memory.Free(this);
		}

		#region Handlerù

		private DistributedArray<Type> memory;

		/// <summary>
		/// Logic memory reference.
		/// </summary>
		public ILogicMemory<Type> MemoryRef { get; }

		/// <summary>
		/// Offset in the target memory.
		/// </summary>
		public long Offset { get; private set; }

		/// <summary>
		/// Block size.
		/// </summary>
		public long Size { get; private set; }

		#endregion

		#region Values

		/// <summary>
		/// Update handler values.
		/// </summary>
		/// <param name="Parent">Parent heap.</param>
		/// <param name="Offset">Offset in the parent heap.</param>
		/// <param name="Size">Size of this chunk of data.</param>
		public void ModifyValues(long offset, long size)
		{
			Size = size;
			Offset = offset;
		}

		#endregion

		#region Enumerator

		/// <summary>
		/// Get the handler enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<Type> GetEnumerator()
		{
			for (long i = Offset; i < Offset + Size; ++i)
			{
				yield return MemoryRef.Memory[i];
			}
		}

		/// <summary>
		/// Get the handler enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Get an element of this handler.
		/// </summary>
		/// <param name="key">Element index.</param>
		/// <returns>Element.</returns>
		public Type this[int key]
		{
			get
			{
				if (key > Size || key < 0)
					throw new IndexOutOfRangeException();

				return MemoryRef.Memory[key + Offset];
			}
			set
			{
				MemoryRef.Memory[key + Offset] = value;
			}
		}

		#endregion

	}
}
