// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System;
using System.Collections.Generic;

using pEngine.Common;

namespace pEngine.Core.Data
{
	public delegate void ResourceEventHandler(IResource res);
	public delegate void ResourceAbortEventHandler(IResource res, Exception e);

	/// <summary>
	/// A loadable resource.
	/// </summary>
	public interface IResource : IDisposable
    {
        /// <summary>
        /// Triggered on dependencies and resource loaded.
        /// </summary>
        event ResourceEventHandler Completed;

        /// <summary>
        /// Triggered on loading error.
        /// </summary>
        event ResourceAbortEventHandler Aborted;

        /// <summary>
        /// Triggered on resource disposing.
        /// </summary>
        event ResourceEventHandler Deleted;

        #region State

        /// <summary>
        /// Gets a value indicating whether this resource is loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
        bool IsLoaded { get; }

        /// <summary>
        /// Gets a value indicating whether the load process is aborted due to error.
        /// </summary>
        /// <value><c>true</c> if this instance is aborted; otherwise, <c>false</c>.</value>
        bool IsAborted { get; }

        #endregion

		/// <summary>
		/// This resource will wait that all resources in this
		/// list are loaded, then this resource il start to load.
		/// </summary>
		IEnumerable<IResource> InternalDependencies { get; }

		/// <summary>
		/// Used space in RAM memory.
		/// </summary>
		uint UsedSpace { get; }

    }
}
