using System;
using System.Collections.Generic;
using System.Reflection;

namespace pEngine.Framework.Caching
{
	public class DictionaryCache : IDisposable
	{
		public DictionaryCache()
		{
			storage = new Dictionary<MethodBase, object>();
			channels = new Dictionary<string, List<MethodBase>>();
			Enabled = true;
		}

		/// <summary>
		/// Release all references and free the cache.
		/// </summary>
		public void Dispose()
		{
			channels.Clear();
			storage.Clear();
		}

		/// <summary>
		/// Enables caching.
		/// </summary>
		public bool Enabled { get; set; }

		/// <summary>
		/// Cache storage.
		/// </summary>
		private Dictionary<MethodBase, object> storage;

		/// <summary>
		/// Invalidation channels.
		/// </summary>
		private Dictionary<string, List<MethodBase>> channels;

		/// <summary>
		/// Gats the saved value for this method.
		/// </summary>
		/// <param name="function">Key method.</param>
		/// <returns>The cached value.</returns>
		public object GetValue(MethodBase function)
		{
			return storage[function];
		}

		#region Caching

		/// <summary>
		/// Checks if this function is cached.
		/// </summary>
		/// <param name="function">Cached function to check.</param>
		/// <returns>True if this function is cached.</returns>
		public bool IsCached(MethodBase function)
		{
			if (!Enabled)
				return false;

			return storage.ContainsKey(function);
		}

		/// <summary>
		/// Set a value for the specified function.
		/// </summary>
		/// <param name="function">Target function.</param>
		/// <param name="channel">[optional] invalidation channel.</param>
		/// <param name="value">Value to set.</param>
		public void AddToCache(MethodBase function, string channel, object value)
		{
			if (!Enabled)
				return;

			// - Set the new value
			if (storage.ContainsKey(function))
				storage[function] = value;
			else storage.Add(function, value);

			if (!channels.ContainsKey(channel))
				channels.Add(channel, new List<MethodBase>());

			channels[channel].Add(function);
		}

		#endregion

		#region Invalidation

		/// <summary>
		/// Reset the cache associated to this channel.
		/// </summary>
		/// <param name="channel">Channel to invalidate.</param>
		public void Invalidate(string channel)
		{
				foreach (var method in channels[channel])
					storage.Remove(method);
		}

		/// <summary>
		/// Reset the cache associated to this method.
		/// </summary>
		/// <param name="channel">Method to invalidate.</param>
		public void Invalidate(MethodBase method)
		{
			storage.Remove(method);
		}

		#endregion
	}
}
