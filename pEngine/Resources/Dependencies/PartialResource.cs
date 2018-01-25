using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using pEngine.Framework;
using pEngine.Utils.Threading;
using pEngine.Resources.Dependencies;

namespace pEngine.Resources
{
    public abstract partial class PartialResource : pObject
    {
        #region Dependencies

        /// <summary>
        /// This resource will wait that all resources in this
        /// list are loaded, then this resource il start to load.
        /// </summary>
        protected internal ObservableCollection<ResourceDependencyLoader> InternalDependencies { get; }

        /// <summary>
        /// This resource will wait that all resources in this
        /// list are loaded, then this resource il start to load.
        /// </summary>
        public IEnumerable<PartialResource> Dependencies => InternalDependencies.Select(x => x.Resource.GetValue(this) as PartialResource);

        #endregion

        #region Check all dependencies

        private void checkDependencies()
        {
            var type = GetType();
            var properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in properties)
            {
                var attribute = field.GetCustomAttribute(typeof(Dependency)) as Dependency;

                if (attribute != null)
                {
                    InternalDependencies.Add(new ResourceDependencyLoader
                    {
                        Resource = field,
                        Async = attribute.Async,
                        Load = attribute.Load
                    });
                }
            }
        }

        #endregion

        protected internal struct ResourceDependencyLoader
        {
            public bool Load;
            public bool Async;
            public PropertyInfo Resource;
        }

    }
}
