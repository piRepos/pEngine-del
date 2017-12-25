// Copyright (c) 2016 PK IT Andrea Demontis
//
//      pEngine / 2D Graphic engine for rythm games.
//

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;

using pEngine.Framework;
using pEngine.Framework.Binding;

namespace pEngine.Resources
{
    /// <summary>
    /// This is the base class for all resources which
    /// need to be loaded.
    /// </summary>
    public abstract partial class PartialResource : pObject, IResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="pEngine.Data.Resource"/> class.
        /// </summary>
        public PartialResource()
        {
            InternalDependencies = new ObservableCollection<ResourceDependencyLoader>();
            checkDependencies();
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Resource"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Resource"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="Resource"/> in an unusable state. After calling
        /// <see cref="Dispose"/>, you must release all references to the <see cref="Resource"/> so the garbage
        /// collector can reclaim the memory that the <see cref="Resource"/> was occupying.</remarks>
        public override void Dispose()
        {
            Disposed?.Invoke(this);
        }

        /// <summary>
        /// Used space in bytes.
        /// </summary>
        [Bindable(Direction = BindingMode.ReadOnly)]
        public virtual uint UsedSpace => 4 + (uint)Dependencies.Sum(x => x.UsedSpace);

        #region State

        /// <summary>
        /// Triggered on dependencies and resource loaded.
        /// </summary>
        public event ResourceEventHandler Loaded;

        /// <summary>
        /// Triggered on loading error.
        /// </summary>
        public event ResourceAbortEventHandler Aborted;

        /// <summary>
        /// Triggered on resource disposing.
        /// </summary>
        public event ResourceEventHandler Disposed;

        /// <summary>
        /// Gets a value indicating whether this resource is loaded.
        /// </summary>
        [Bindable(Direction = BindingMode.ReadOnly)]
        public ResourceState State { get; protected set; }

        /// <summary>
        /// Abort resource loading.
        /// </summary>
        protected virtual bool OnAbort(IResource res, Exception e)
        {
            Aborted?.Invoke(this, e);

            return false;
        }

        /// <summary>
        /// Implements resource loading.
        /// </summary>
        protected abstract void OnLoad();

        /// <summary>
        /// Complete loading.
        /// </summary>
        protected virtual void OnComplete()
        {
            Loaded?.Invoke(this);
        }

        #endregion
    }
}

