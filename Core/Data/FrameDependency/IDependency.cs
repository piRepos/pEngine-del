using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Core.Data.FrameDependency
{
    public interface IDependency<DescriptorType> : IDisposable, IDependencyReference where DescriptorType : IDependencyDescriptor
    {
		/// <summary>
		/// Generate a descriptor for this dependency.
		/// This descriptor will be used as frame resource.
		/// </summary>
		/// <returns>The dependency descriptor.</returns>
		DescriptorType GetDescriptor();

		/// <summary>
		/// True if the resource is changed.
		/// </summary>
		bool Invalidated { get; set; }

		/// <summary>
		/// Dependency identifier.
		/// </summary>
        new long DependencyID { get; set; }
	}
}
