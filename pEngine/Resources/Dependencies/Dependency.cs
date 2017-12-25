using System;
namespace pEngine.Resources.Dependencies
{
    /// <summary>
    /// Set this variable as a dependency resource.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class Dependency : Attribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Dependency"/> needs to be loaded.
        /// </summary>
        public bool Load { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Dependency"/> needs to be loaded in async mode.
        /// </summary>
        public bool Async { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dependency"/> class.
        /// </summary>
        public Dependency()
        {
        }
    }
}
