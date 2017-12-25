// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace pEngine.Utils.Memory
{
	using Math = System.Math;

	public class DistributedArray<Type> : ILogicMemory<Type>, IEnumerable<Type>
	{

		/// <summary>
		/// Make a new <see cref="DistributedArray{Type}"/>.
		/// </summary>
		public DistributedArray()
		{
			GrowPercentile = 10.0D;

			HeapData = new Type[0];

			FreeSpaces = new List<ArrayHandler<Type>>();
			UsedSpaces = new List<ArrayHandler<Type>>();
		}

		/// <summary>
		/// Make a new <see cref="DistributedArray{Type}"/>.
		/// </summary>
		/// <param name="GrowFactor">Grow percentile when there's no space.</param>
		public DistributedArray(double GrowFactor)
		{
			GrowPercentile = GrowFactor;

			HeapData = new Type[0];

			FreeSpaces = new List<ArrayHandler<Type>>();
			UsedSpaces = new List<ArrayHandler<Type>>();
		}

		private Type[] HeapData;

		private List<ArrayHandler<Type>> FreeSpaces;

		private List<ArrayHandler<Type>> UsedSpaces;

		/// <summary>
		/// Triggered on heap defragmentation.
		/// </summary>
		public event EventHandler Defragged;

		/// <summary>
		/// Logic memory.
		/// </summary>
		public Type[] Memory => HeapData;

		#region Properties

		/// <summary>
		/// Percentile of vertex to add when the
		/// heap grow
		/// </summary>
		public double GrowPercentile { get; set; }

		/// <summary>
		/// Free vertex count
		/// </summary>
		public long FreeVertexCount =>FreeSpaces.Sum((X) => X.Size);

		/// <summary>
		/// Used vertex count
		/// </summary>
		public long UsedVertexCount => HeapData.Length - FreeVertexCount;

		/// <summary>
		/// Closer to 1 when there are too litle slice of free heap
		/// and closer to 0 then there is One big free block
		/// </summary>
		public double FragmentationIndex
		{
			get
			{
				double maxFree = 0;
				double Free = FreeVertexCount;
				if (Free <= 0) return 1;
				for (int i = FreeSpaces.Count - 1; i >= 0; --i)
				{
					ArrayHandler<Type> V = FreeSpaces[i];
					maxFree = Math.Max(maxFree, V.Size);
				}
				return ((Free - maxFree) / Free);
			}
		}

		#endregion

		#region Space assignment

		/// <summary>
		/// Return an handler to this heap for access
		/// in a region with this size.
		/// </summary>
		/// <param name="Count">Region's size.</param>
		/// <returns>Handler.</returns>
		public ArrayHandler<Type> Alloc(int Count)
		{
			bool spaceFound = false;

			ArrayHandler<Type> ToReturn = null;

			if (Count == 0)
				return null;

			// Searching a free segment of heap
			for (int i = FreeSpaces.Count - 1; i >= 0; --i)
			{
				ArrayHandler<Type> Segment = FreeSpaces[i];
				if (Segment.Size >= Count)
				{
					long remains = Segment.Size - Count;

					// Add space surplus
					if (remains > 0)
						FreeSpaces.Add(new ArrayHandler<Type>(this, Segment.Offset + Count, remains));

					// Prepare return segment
					ToReturn = new ArrayHandler<Type>(this, Segment.Offset, Count);

					// Remove old segment
					FreeSpaces.Remove(Segment);

					spaceFound = true;

					break;
				}
			}

			if (!spaceFound)
			{
				Grow(Count);
				return Alloc(Count);
			}

			UsedSpaces.Add(ToReturn);

			return ToReturn;
		}

		/// <summary>
		/// Free a portion of heap.
		/// </summary>
		/// <param name="V">Handle to this portion.</param>
		public void Free(ArrayHandler<Type> V)
		{
			// !! The vertex segment MUST be used !!

			if (V == null)
				return;

			if (!UsedSpaces.Contains(V))
				throw new Exception("This handler doesn't belongs to this heap.");

			bool Found = false;

			var freeSpace = new ArrayHandler<Type>(this, V.Offset, V.Size);

			// Check if there's continuous free segments
			for (int i = FreeSpaces.Count - 1; i >= 0; --i)
			{
				ArrayHandler<Type> Segment = FreeSpaces[i];
				if (Segment.Offset + Segment.Size == V.Offset)
				{
					freeSpace = new ArrayHandler<Type>(this, Segment.Offset, Segment.Size + freeSpace.Size);
					FreeSpaces.RemoveAt(i--);
					Found = true;
				}
				else if (V.Offset + V.Size == Segment.Offset)
				{
					freeSpace = new ArrayHandler<Type>(this, freeSpace.Offset, freeSpace.Size + Segment.Size);
					FreeSpaces.RemoveAt(i--);
					Found = true;
				}

				if (Found)
					break;
			}

			if (FreeSpaces.FindIndex((X) => (X.Offset <= freeSpace.Offset && X.Size + X.Size >= freeSpace.Offset)) < 0)
				FreeSpaces.Add(freeSpace);
			
			UsedSpaces.Remove(V);
		}

		#endregion

		#region Space management

		/// <summary>
		/// Grow the current heap
		/// </summary>
		/// <param name="nVertex">Min element needs</param>
		public void Grow(int Count)
		{
			// Grow default size
			int GrowSize = (int)((HeapData.Length / 100D) * GrowPercentile);

			// Check if this size is sufficient
			if (GrowSize < Count) GrowSize += Count;

			int oldSize = HeapData.Length;
			Array.Resize(ref HeapData, HeapData.Length + GrowSize);

			ArrayHandler<Type> current = new ArrayHandler<Type>(this, oldSize, GrowSize);

			// Check if there's continuous free segments
			for (int i = 0; i < FreeSpaces.Count; ++i)
			{
				ArrayHandler<Type> Segment = FreeSpaces[i];
				if (Segment.Offset + Segment.Size == current.Offset)
				{
					current = new ArrayHandler<Type>(this, Segment.Offset, Segment.Size + current.Size);
					FreeSpaces.Remove(Segment);
				}
			}

			FreeSpaces.Add(current);
		}
		
		/// <summary>
		/// Defrag heap
		/// </summary>
		public void Defrag()
		{

			// Rebuild all heap
			Type[] newHeap = new Type[HeapData.Length];

			long i = 0;

			// Reset all counters and rebuild heap
			foreach (ArrayHandler<Type> Handler in UsedSpaces)
			{
				Array.Copy(HeapData, Handler.Offset, newHeap, i, Handler.Size);

				Handler.ModifyValues(i, Handler.Size);

				i += Handler.Size;
			}

			FreeSpaces.Clear();

			long RemainingSpace = newHeap.Length - i;

			// Insert free space
			FreeSpaces.Add(new ArrayHandler<Type>(this, i, RemainingSpace));

			HeapData = newHeap;

			Defragged?.Invoke(this, EventArgs.Empty);
		}

		#endregion

		#region Enumerator

		/// <summary>
		/// Get the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<Type> GetEnumerator()
		{
			return ((IEnumerable<Type>)HeapData).GetEnumerator();
		}

		/// <summary>
		/// Get the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return HeapData.GetEnumerator();
		}

		/// <summary>
		/// Get an element of this heap manager.
		/// </summary>
		/// <param name="key">Element index.</param>
		/// <returns>Element.</returns>
		public Type this[int key]
		{
			get
			{
				return HeapData[key];
			}
			set
			{
				HeapData[key] = value;
			}
		}

		public Type[] ToArray()
		{
			return HeapData;
		}

		public uint Count
		{
			get
			{
				return (uint)HeapData.Length;
			}
		}

		#endregion
	}
}
