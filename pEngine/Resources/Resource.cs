// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;

using pEngine.Framework;
using pEngine.Utils.Threading;

namespace pEngine.Resources
{
    /// <summary>
    /// This is the base class for all resources which
    /// need to be loaded.
    /// </summary>
    public abstract class Resource : pObject, IResource
    {
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="pEngine.Data.Resource"/> class.
        /// </summary>
        public Resource()
        {
            Dependencies = new ObservableCollection<IResource>();
			Dependencies.CollectionChanged += DependenciesChange;
        }

		/// <summary>
		/// Releases all resource used by the <see cref="Resource"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Resource"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="Resource"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="Resource"/> so the garbage
		/// collector can reclaim the memory that the <see cref="Resource"/> was occupying.</remarks>
		public virtual void Dispose()
        {
			Deleted?.Invoke(this);
        }

        
		/// <summary>
        /// Triggered on dependencies and resource loaded.
        /// </summary>
        public event ResourceEventHandler Completed;

		/// <summary>
		/// Triggered on loading error.
		/// </summary>
		public event ResourceAbortEventHandler Aborted;

		/// <summary>
		/// Triggered on resource disposing.
		/// </summary>
		public event ResourceEventHandler Deleted;

		/// <summary>
		/// Used space in RAM memory.
		/// </summary>
		public abstract uint UsedSpace { get; }

		#region Dependencies

		private void DependenciesChange(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:

					foreach (IResource elem in e.NewItems)
					{
						elem.Aborted += (IResource r, Exception a) => OnAbort(r, a);
						elem.Deleted += (IResource r) => Dispose();
					}

					break;
			}
		}

		/// <summary>
		/// This resource will wait that all resources in this
		/// list are loaded, then this resource il start to load.
		/// </summary>
		protected ObservableCollection<IResource> Dependencies { get; }

		/// <summary>
		/// This resource will wait that all resources in this
		/// list are loaded, then this resource il start to load.
		/// </summary>
		public IEnumerable<IResource> InternalDependencies => Dependencies;

		#endregion

		#region State

		/// <summary>
		/// Gets a value indicating whether this resource is loaded.
		/// </summary>
		public bool IsLoaded { get; internal set; }

		/// <summary>
		/// Gets a value indicating whether this resource is aborted.
		/// </summary>
		public bool IsAborted { get; internal set; }

		/// <summary>
		/// Is true when the loading needs other resource for
		/// completation.
		/// </summary>
		public bool IsPartial { get; protected set; }

		/// <summary>
		/// If true the resource can be reloaded after first load.
		/// </summary>
		public bool Reloadable { get; protected set; }

		/// <summary>
		/// Abort resource loading.
		/// </summary>
		internal virtual bool OnAbort(IResource res, Exception e)
        {
			Aborted?.Invoke(this, e);

			return false;
        }

		/// <summary>
		/// Implements resource loading.
		/// </summary>
		internal abstract void OnLoad();

		/// <summary>
		/// Complete loading.
		/// </summary>
		internal virtual void OnComplete()
		{
			Completed?.Invoke(this);
		}

        #endregion

        #region Loading

        /// <summary>
        /// Loads this resource.
        /// </summary>
        public virtual void Load(Scheduler scheduler)
        {
            try
            {
                foreach (Resource dependency in InternalDependencies)
                {
                    loadDependencies(dependency);
                }

                if (!IsLoaded || Reloadable)
                {
                    OnLoad();

                    scheduler.Add(() =>
                    {
                        IsLoaded = true;

                        if (IsLoaded && !IsPartial)
                            OnComplete();
                    });
                }
            }
            catch (Exception e)
            {
                scheduler.Add(() =>
                {
                    if (!OnAbort(this, e))
                    {
                        throw e;
                    }
                });
            }
        }

        /// <summary>
        /// Loads this resource asyncronously.
        /// </summary>
        public virtual void LoadAsync(Scheduler scheduler)
        {
            scheduler.Add(() =>
            {
                var loader = new Task(() =>
                {
                    try
                    {
                        foreach (Resource r in InternalDependencies)
                        {
                            loadDependencies(r);
                        }

                        if (!IsLoaded || Reloadable)
                        {
                            OnLoad();

                            scheduler.Add(() =>
                            {
                                IsLoaded = true;

                                if (IsLoaded && !IsPartial)
                                    OnComplete();
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        scheduler.Add(() =>
                        {
                            if (!OnAbort(this, e))
                            {
                                throw e;
                            }
                        });
                    }

                });

                loader.Start();
            });
        }

        private void loadDependencies(Resource dependency)
        {
            foreach (Resource dep in dependency.InternalDependencies)
            {
                loadDependencies(dep);
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

