using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace pEngine.Utils.Extensions
{
    static class ListExtensions
    {
		/// <summary>
		/// Remove all elements that validate a condition.
		/// </summary>
		/// <param name="condition">Predicate condition.</param>
		/// <returns>Number of removed items.</returns>
		public static int RemoveAll<T>(this ObservableCollection<T> coll, Func<T, bool> condition)
		{
			var itemsToRemove = coll.Where(condition).ToList();

			foreach (var itemToRemove in itemsToRemove)
			{
				coll.Remove(itemToRemove);
			}

			return itemsToRemove.Count;
		}

		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified predicate.
		/// </summary>
		/// <param name="list">The list to take values</param>
		/// <param name="match">The predicate that needs to be matched.</param>
		/// <param name="startIndex">The index to start conditional search.</param>
		/// <returns>The matched item, or the default value for the type if no item was matched.</returns>
		public static T Find<T>(this List<T> list, Predicate<T> match, int startIndex)
		{
			if (!list.IsValidIndex(startIndex)) return default(T);

			int val = list.FindIndex(startIndex, list.Count - startIndex - 1, match);

			return list.ValueAtOrDefault(val);
		}

		/// <summary>
		/// Adds the given item to the list according to standard sorting rules. Do not use on unsorted lists.
		/// </summary>
		/// <param name="list">The list to take values</param>
		/// <param name="item">The item that should be added.</param>
		/// <returns>The index in the list where the item was inserted.</returns>
		public static int AddInPlace<T>(this List<T> list, T item)
		{
			int index = list.BinarySearch(item);
			if (index < 0) index = ~index; // BinarySearch hacks multiple return values with 2's complement.
			list.Insert(index, item);
			return index;
		}

		/// <summary>
		/// Adds the given item to the list according to the comparers sorting rules. Do not use on unsorted lists.
		/// </summary>
		/// <param name="list">The list to take values</param>
		/// <param name="item">The item that should be added.</param>
		/// <param name="comparer">The comparer that should be used for sorting.</param>
		/// <returns>The index in the list where the item was inserted.</returns>
		public static int AddInPlace<T>(this List<T> list, T item, IComparer<T> comparer)
		{
			int index = list.BinarySearch(item, comparer);
			if (index < 0) index = ~index; // BinarySearch hacks multiple return values with 2's complement.
			list.Insert(index, item);
			return index;
		}

		/// <summary>
		/// Check if the specified index is valid in this context.
		/// </summary>
		/// <param name="list">The list to take values</param>
		/// <param name="index">Index to check.</param>
		/// <returns>If the specified index is valid.</returns>
		public static bool IsValidIndex<T>(this List<T> list, int index)
		{
			return index >= 0 && index < list.Count;
		}

		/// <summary>
		/// Validates whether index is valid, before returning the value at the given index.
		/// </summary>
		/// <typeparam name="T">Probably should limit to nullable types.</typeparam>
		/// <param name="list">The list to take values</param>
		/// <param name="index">The index to request values from</param>
		/// <returns>Value at index, else the default value</returns>
		public static T ValueAtOrDefault<T>(this List<T> list, int index)
		{
			return list.IsValidIndex(index) ? list[index] : default(T);
		}

		/// <summary>
		/// Compares every item in list to given list.
		/// </summary>
		public static bool CompareTo<T>(this List<T> list, List<T> list2)
		{
			if (list.Count != list2.Count) return false;

			return !list.Where((t, i) => !EqualityComparer<T>.Default.Equals(t, list2[i])).Any();
		}

	}
}
