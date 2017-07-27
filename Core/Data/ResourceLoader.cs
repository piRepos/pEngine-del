using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using pEngine.Common.Timing.Base;
using pEngine.Common.Threading;

namespace pEngine.Core.Data
{
    public class ResourceLoader
    {
		pEngine gameHost;

		public ResourceLoader(pEngine host)
		{
			gameHost = host;
		}

		#region Resource managing

		/// <summary>
		/// Load the specified resource.
		/// </summary>
		/// <param name="res">Resource to add.</param>
		public virtual void Load(Resource res)
		{
			lock (res)
			{
				try
				{
					foreach (Resource r in res.InternalDependencies)
					{
						LoadDependencies(r);
					}

					if (!res.IsLoaded || res.Reloadable)
					{
						res.OnLoad();

						res.IsLoaded = true;

						if (res.IsLoaded && !res.IsPartial)
							res.OnComplete();
					}
				}
				catch (Exception e)
				{
					if (!res.OnAbort(res, e))
					{
						throw e;
					}
				}
			}
		}

		/// <summary>
		/// Loads this resource asyncronously.
		/// </summary>
		/// <param name="res">Resource to add.</param>
		public virtual void LoadAsync(Resource res)
		{

			Task loader;

			gameHost.PhysicsLoop.Scheduler.Add(() =>
			{
				loader = new Task(() =>
				{
					lock (res)
					{
						try
						{
							foreach (Resource r in res.InternalDependencies)
							{
								LoadDependencies(r);
							}

							if (!res.IsLoaded || res.Reloadable)
							{
								res.OnLoad();

								gameHost.PhysicsLoop.Scheduler.Add(() =>
								{
									res.IsLoaded = true;

									if (res.IsLoaded && !res.IsPartial)
										res.OnComplete();
								});
							}
						}
						catch (Exception e)
						{
							gameHost.PhysicsLoop.Scheduler.Add(() =>
							{
								if (!res.OnAbort(res, e))
								{
									throw e;
								}
							});
						}
					}
				});

				loader.Start();
			});
		}

		private void LoadDependencies(Resource dependency)
		{
			foreach (Resource r in dependency.InternalDependencies)
			{
				LoadDependencies(r);
			}

			try
			{
				if (!dependency.IsLoaded || dependency.Reloadable)
				{
					dependency.OnLoad();

					dependency.IsLoaded = true;

					if (dependency.IsLoaded && !dependency.IsPartial)
						dependency.OnComplete();
				}
			}
			catch (Exception e)
			{
				if (!dependency.OnAbort(dependency, e))
				{
					throw e;
				}
			}
		}

		#endregion

	}
}
