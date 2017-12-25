using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using pEngine.Utils.Threading;

namespace pEngine.Resources
{
    public class ResourceManager<Type> where Type : Resource
    {

		public ResourceManager()
		{
			resources = new Dictionary<string, Type>();
			resourceScheduler = new Scheduler();
		}

		#region Resource managing

		private Dictionary<string, Type> resources;

		/// <summary>
		/// Used space in RAM memory.
		/// </summary>
		public uint UsedSpace => (uint)resources.Values.Sum(X => X.UsedSpace);

		/// <summary>
		/// All stored resources.
		/// </summary>
		public IEnumerator<Type> Resources => resources.Values.GetEnumerator();

		/// <summary>
		/// Access to a resouce using key string.
		/// </summary>
		/// <param name="key">Resource identifier.</param>
		/// <returns>Resource.</returns>
		public IResource this[string key] => resources[key];

		/// <summary>
		/// Load the specified resource.
		/// </summary>
		/// <param name="name">Identifier for this resource.</param>
		/// <param name="res">Resource to add.</param>
		public virtual void Load(string name, Type res)
		{
			resources.Add(name, res);

			resourceScheduler.Add(() =>
			{
				try
				{
					Stopwatch timeout = new Stopwatch();

					timeout.Start();
					while (!res.IsDependencyLoaded)
					{
						Thread.Sleep(0);

						if (timeout.Elapsed.Seconds > 4)
							throw new TimeoutException("Dependencies loading exceeded the maximum time.");
					}

					res.Load(resourceScheduler);

					if (res.IsLoaded)
						res.Complete();
				}
				catch (Exception e)
				{
					if (!res.Abort(e))
					{
						throw e;
					}
				}
			});
		}

		/// <summary>
		/// Loads this resource asyncronously.
		/// </summary>
		/// <param name="name">Identifier for this resource.</param>
		/// <param name="res">Resource to add.</param>
		public virtual void LoadAsync(string name, Type res)
		{
			resources.Add(name, res);

			Task loader;

			resourceScheduler.Add(() =>
			{
				loader = new Task(() =>
				{
					try
					{
						while (!res.IsDependencyLoaded)
							Thread.Sleep(0);

						res.Load(resourceScheduler);

						resourceScheduler.Add(() =>
						{
							res.IsLoaded = true;

							if (res.IsLoaded)
								res.Complete();
						});
					}
					catch (Exception e)
					{
						resourceScheduler.Add(() =>
						{
							if (!res.Abort(e))
							{
								throw e;
							}
						});
					}
				});

				loader.Start();
			});
		}

		#endregion


		#region Scheduling

		Scheduler resourceScheduler;

		public virtual void Update()
		{
			resourceScheduler.Update();
		}

		#endregion

	}
}
