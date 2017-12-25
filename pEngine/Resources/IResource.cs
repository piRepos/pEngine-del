// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System;
using System.Collections.Generic;

namespace pEngine.Resources
{
	public delegate void ResourceEventHandler(IResource res);
	public delegate void ResourceAbortEventHandler(IResource res, Exception e);

    public enum ResourceState
    {
        /// <summary>
        /// The resource is not loaded.
        /// </summary>
        NotLoaded = 0,

        /// <summary>
        /// The resource is loaded.
        /// </summary>
        Loaded = 1,

        /// <summary>
        /// The resource loading is aborted.
        /// </summary>
        Aborted = 2
    }

	/// <summary>
	/// A loadable resource.
	/// </summary>
	public interface IResource : IDisposable
    {
        /// <summary>
        /// Triggered on dependencies and resource loaded.
        /// </summary>
        event ResourceEventHandler Loaded;

        /// <summary>
        /// Triggered on loading error.
        /// </summary>
        event ResourceAbortEventHandler Aborted;

        /// <summary>
        /// Triggered on resource disposing.
        /// </summary>
        event ResourceEventHandler Disposed;

        #region State

        /// <summary>
        /// Gets a value indicating whether this resource is loaded.
        /// </summary>
        ResourceState State { get; }

        #endregion

		/// <summary>
		/// This resource will wait that all resources in this
		/// list are loaded, then this resource il start to load.
		/// </summary>
		IEnumerable<IResource> Dependencies { get; }

		/// <summary>
		/// Used space in RAM memory.
		/// </summary>
		uint UsedSpace { get; }

    }
}
