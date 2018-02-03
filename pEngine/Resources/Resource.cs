// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using pEngine.Utils.Threading;

namespace pEngine.Resources
{
    /// <summary>
    /// This is the base class for all resources which
    /// need to be loaded.
    /// </summary>
    public abstract partial class Resource : PartialResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        public Resource()
        {
        }

        /// <summary>
        /// If true the resource can be reloaded after first load.
        /// </summary>
        public bool Reloadable { get; protected set; }

        #region Loading

        /// <summary>
        /// Loads this resource.
        /// </summary>
        public virtual void Load(Scheduler scheduler = null)
        {
            try
            {
                loadDependencies(scheduler);

                if (Dependencies.Any(x => x.State != ResourceState.Loaded))
                    throw new MissingMemberException("Some dependency is not loaded.");

                if (State != ResourceState.Loaded || Reloadable)
                {
                    OnLoad();

                    if (scheduler != null)
                    {
                        scheduler.Add(() =>
                        {
                            State = ResourceState.Loaded;

                            OnComplete();
                        });
                    }
                    else
                    {
                        State = ResourceState.Loaded;
                        OnComplete();
                    }
                }
            }
            catch (Exception e)
            {
                if (scheduler != null)
                {
                    scheduler.Add(() =>
                    {
                        State = ResourceState.Aborted;
                        if (!OnAbort(this, e))
                        {
                            throw e;
                        }
                    });
                }
                else
                {
                    State = ResourceState.Aborted;
                    if (!OnAbort(this, e))
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// Loads this resource asyncronously.
        /// </summary>
        public virtual Task LoadAsync(Scheduler scheduler = null)
        {
            var loader = new Task(() =>
            {
                Load(scheduler);
            });

            if (scheduler != null)
            {
                scheduler.Add(() =>
                {
                    loader.Start();
                });
            }
            else loader.Start();

            return loader;
        }

        private void loadDependencies(Scheduler scheduler)
        {
            List<Task> waitList = new List<Task>();
            
            foreach (var dependency in InternalDependencies)
            {
                if (!dependency.Load)
                    continue;

                var resource = dependency.Resource.GetValue(this) as PartialResource;

                if (resource is Resource)
                {
                    Resource res = resource as Resource;

                    if (dependency.Async)
                        waitList.Add(res.LoadAsync(scheduler));
                    else res.Load(scheduler);
                }
            }

            foreach (Task t in waitList)
                t.Wait();
        }

        #endregion
    }
}

