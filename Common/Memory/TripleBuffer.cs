using System;
using System.Threading;

using pEngine.Common.Invocation;

namespace pEngine.Common.Memory
{
    public class TripleBuffer<T>
    {
		/// <summary>
		/// Makes a new instance of <see cref="TripleBuffer{T}"/>.
		/// </summary>
		public TripleBuffer()
		{
			closeDelegate = Close;
		}

		/// <summary>
		/// Internal buffer.
		/// </summary>
		private ObjectUsage<T>[] buffer = new ObjectUsage<T>[3];

		/// <summary>
		/// finish delegate cache for performance improvement.
		/// </summary>
		private readonly Action<ObjectUsage<T>> closeDelegate;

		private int writePointer;
		private int readPointer;
		private int freePointer = -1;

		/// <summary>
		/// Get an access to the buffer.
		/// </summary>
		/// <param name="usage">Specifies the access usage.</param>
		/// <returns>An usage instance.</returns>
		public ObjectUsage<T> Get(UsageType usage)
		{
			switch(usage)
			{
				case UsageType.Write:

					lock(buffer)
					{
						// - Find the free buffer for write
						while (buffer[writePointer]?.Usage == UsageType.Read || writePointer == freePointer)
							writePointer = (writePointer + 1) % 3;
					}

					// - If buffer is null instance it and set it on Write
					if (buffer[writePointer] == null)
					{
						ObjectUsage<T> obj = new ObjectUsage<T>
						{
							Usage = UsageType.Write
						};

						obj.Action = () => Close(obj);

						buffer[writePointer] = obj;
					}
					else
					{
						buffer[writePointer].Usage = UsageType.Write;
					}

					return buffer[writePointer];

				case UsageType.Read:

					if (freePointer < 0) return null;

					lock (buffer)
					{
						readPointer = freePointer;
						buffer[readPointer].Usage = UsageType.Read;
					}

					return buffer[readPointer];
			}

			return null;
		}

		private void Close(ObjectUsage<T> obj)
		{
			switch(obj.Usage)
			{
				case UsageType.Read:

					lock (buffer)
						buffer[readPointer].Usage = UsageType.None;

					break;

				case UsageType.Write:

					lock (buffer)
					{
						buffer[writePointer].Usage = UsageType.None;
						freePointer = writePointer;
					}

					break;
			}
		}

	}

	public class ObjectUsage<T> : InvokeOnDisposal
	{

		public ObjectUsage()
		{

		}

		/// <summary>
		/// Current usage.
		/// </summary>
		public UsageType Usage { get; set; }

		/// <summary>
		/// Current object.
		/// </summary>
		public T Value { get; set; }

		/// <summary>
		/// Pointer index
		/// </summary>
		public int Index { get; set; }

	}

	/// <summary>
	/// Describe the object usage in memory, write or read.
	/// </summary>
	public enum UsageType
	{
		None,
		Read,
		Write
	}
}
