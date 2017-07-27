using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

using pEngine.Common.Invocation;

namespace pEngine.Common.Diagnostic
{
	public delegate void PerformanceCollectionEventHandler(IPerformanceCollection performance);

	public interface IPerformanceCollection
	{
		/// <summary>
		/// The collection name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Collection creation time.
		/// </summary>
		DateTime Created { get; }

		/// <summary>
		/// Gets the average time for this performance.
		/// </summary>
		TimeSpan Average { get; }

		/// <summary>
		/// Gets the minimum time.
		/// </summary>
		TimeSpan Min { get; }

		/// <summary>
		/// Gets the maximum time.
		/// </summary>
		TimeSpan Max { get; }

		/// <summary>
		/// Gets the sum time.
		/// </summary>
		TimeSpan Sum { get; }
	}

	public class PerformanceCollection : Stopwatch, IPerformanceCollection
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="PerformanceCollection"/> class.
		/// </summary>
		/// <param name="name">Performance name.</param>
		/// <param name="cacheSize">How many values must be stored.</param>
		public PerformanceCollection(string name, int cacheSize)
		{
			Name = name;

			this.cacheSize = cacheSize;

			Created = DateTime.Now;

			Min = TimeSpan.MaxValue;
			Max = TimeSpan.MinValue;

			collectedValues = new List<TimeSpan>();
		}

		private int cacheSize;
		private List<TimeSpan> collectedValues;

		#region Values

		/// <summary>
		/// The collection name.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Collection creation time.
		/// </summary>
		public DateTime Created { get; }

		/// <summary>
		/// Gets the average time for this performance.
		/// </summary>
		public TimeSpan Average { get { return new TimeSpan(collectedValues.Sum(X => X.Ticks) / collectedValues.Count); } }

		/// <summary>
		/// Gets the minimum time.
		/// </summary>
		public TimeSpan Min { get; private set; }

		/// <summary>
		/// Gets the maximum time.
		/// </summary>
		public TimeSpan Max { get; private set; }

		/// <summary>
		/// Gets the sum time.
		/// </summary>
		public TimeSpan Sum { get; private set; }

		#endregion

		#region Management

		/// <summary>
		/// Start the performance collection.
		/// </summary>
		internal new void Start()
		{
			base.Start();
		}

		/// <summary>
		/// Stop the performance collection.
		/// </summary>
		internal new void Stop()
		{
			base.Stop();

			Min = Elapsed > Min ? Min : Elapsed;
			Max = Elapsed > Max ? Elapsed : Max;
			Sum = Elapsed;

			collectedValues.Add(Elapsed);

			if (collectedValues.Count > cacheSize)
				collectedValues.RemoveAt(0);

			base.Reset();
		}

		/// <summary>
		/// Stop the performance collection and reset all values.
		/// </summary>
		internal new void Reset()
		{
			base.Reset();

			collectedValues.Clear();

			Min = TimeSpan.MaxValue;
			Max = TimeSpan.MinValue;
			Sum = TimeSpan.Zero;
		}

		internal new void Restart()
		{
			Stop();
			Start();
		}

		#endregion
	}

	public class PerformanceCollector : IPerformanceCollection, IEnumerable<IPerformanceCollection>
	{

		public PerformanceCollector(string name)
		{
			collections = new List<IPerformanceCollection>();
			Name = name;
			Created = DateTime.Now;
		}


		/// <summary>
		/// Gets the precision of the average values.
		/// </summary>
		static public int CacheSize { get; private set; } = 20;

		/// <summary>
		/// Occurs when a new performance is created.
		/// </summary>
		public event PerformanceCollectionEventHandler OnNewPerformance;

		/// <summary>
		/// Occurs when a collector starts to collect.
		/// </summary>
		public event PerformanceCollectionEventHandler OnStartCollect;

		private List<IPerformanceCollection> collections;

		/// <summary>
		/// Search an existent collection.
		/// </summary>
		/// <returns>The collection found (null if not found).</returns>
		/// <param name="name">Name of the collection to search.</param>
		public IPerformanceCollection GetCollection(string name)
		{
			IPerformanceCollection currentPerformance;

			currentPerformance = collections.Find(X => X.Name == name);

			return currentPerformance;
		}

		#region Single performance

		/// <summary>
		/// Starts the collecting a time for the specified performance.
		/// !!!! To use in a using block !!!!
		/// </summary>
		/// <returns>An object to dispose for stop the collection.</returns>
		/// <param name="Performance">Performance to collect.</param>
		/// <param name="collectOnRelease">If set to <c>true</c> collect on release build else not.</param>
		public InvokeOnDisposal StartCollect(string performance, bool collectOnRelease = false)
		{
			#if !DEBUG
			if(!collectOnRelease)
				return null;
			#endif

			PerformanceCollection currentPerformance;

			currentPerformance = CreateCollection(performance);

			OnStartCollect?.Invoke(currentPerformance);

			currentPerformance.Start();

			// - Send a request for stop the collection
			return new InvokeOnDisposal(() => currentPerformance.Stop());
		}

		/// <summary>
		/// Creates a new collection.
		/// </summary>
		/// <returns>The collection created.</returns>
		/// <param name="performance">Performance collection name.</param>
		/// <param name="collectOnRelease">If set to <c>true</c> collect on release build else not.</param>
		public PerformanceCollection CreateCollection(string performance)
		{
			IPerformanceCollection currentPerformance;

			currentPerformance = GetCollection(performance);

			if (currentPerformance == null)
			{
				currentPerformance = new PerformanceCollection(performance, CacheSize);
				collections.Add(currentPerformance);

				OnNewPerformance?.Invoke(currentPerformance);
			}

			return currentPerformance as PerformanceCollection;
		}

		#endregion

		#region Collectors

		/// <summary>
		/// Creates a new collection.
		/// </summary>
		/// <returns>The collection created.</returns>
		/// <param name="performance">Performance collection name.</param>
		/// <param name="collectOnRelease">If set to <c>true</c> collect on release build else not.</param>
		public PerformanceCollector CreateCollector(string performance)
		{
			IPerformanceCollection currentPerformance;

			currentPerformance = GetCollection(performance);

			if (currentPerformance == null)
			{
				currentPerformance = new PerformanceCollector(performance);
				collections.Add(currentPerformance);

				OnNewPerformance?.Invoke(currentPerformance);
			}

			return currentPerformance as PerformanceCollector;
		}

		/// <summary>
		/// Add a collector to this collector.
		/// </summary>
		/// <param name="collector">Collector to add.</param>
		public void AddCollector(PerformanceCollector collector)
		{
			if (!collections.Contains(collector))
			{
				collections.Add(collector);
			}
		}

		#endregion

		#region Collector

		/// <summary>
		/// The collection name.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Collection creation time.
		/// </summary>
		public DateTime Created { get; }

		/// <summary>
		/// Gets the average time for this performance.
		/// </summary>
		public TimeSpan Average => new TimeSpan((long)collections.Average(p => p.Average.Ticks));

		/// <summary>
		/// Gets the minimum time.
		/// </summary>
		public TimeSpan Min => new TimeSpan(collections.Min(p => p.Average.Ticks));

		/// <summary>
		/// Gets the maximum time.
		/// </summary>
		public TimeSpan Max => new TimeSpan(collections.Max(p => p.Average.Ticks));

		/// <summary>
		/// Gets the sum time.
		/// </summary>
		public TimeSpan Sum => new TimeSpan(collections.Sum(p => p.Average.Ticks));

		#endregion

		#region IEnumerable

		/// <summary>
		/// Access to a collection by key
		/// </summary>
		/// <param name="key">Key.</param>
		/// <returns>Performance collection.</returns>
		public IPerformanceCollection this[string key]
		{
			get
			{
				return collections.Find(x => x.Name == key);
			}
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<IPerformanceCollection> GetEnumerator()
		{
			return collections.GetEnumerator();
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}
}
