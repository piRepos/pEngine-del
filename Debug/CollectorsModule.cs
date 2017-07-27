using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.Diagnostic;

namespace pEngine.Debug
{
    public class CollectorsModule
    {

		public CollectorsModule()
		{
			collectors = new ConcurrentDictionary<string, PerformanceCollector>();
		}

		#region Collectors management

		ConcurrentDictionary<string, PerformanceCollector> collectors;

		/// <summary>
		/// Add a collector to the module.
		/// </summary>
		/// <param name="name">Colector name.</param>
		/// <param name="collector">The collector.</param>
		public void AddCollector(string name, PerformanceCollector collector)
		{
			if (collectors.ContainsKey(name))
				return;

			collectors[name] = collector;
		}

		/// <summary>
		/// Remove a collector from the performance collector module.
		/// </summary>
		/// <param name="name">Collector name.</param>
		public void RemoveCollector(string name)
		{
			if (!collectors.ContainsKey(name))
				return;

			PerformanceCollector p;
			collectors.TryRemove(name, out p);
		}

		#endregion

		#region Access

		/// <summary>
		/// All loaded collectors.
		/// </summary>
		public IEnumerable<PerformanceCollector> Collectors => collectors.Values;

		/// <summary>
		/// Gets a collector using the name key.
		/// </summary>
		/// <param name="key">Collector name.</param>
		/// <returns>The collector.</returns>
		public PerformanceCollector this[string key]
		{
			get
			{
				return collectors[key];
			}
		}

		#endregion


	}
}
