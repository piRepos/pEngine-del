using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Core.Data.FrameDependency
{
    public interface IDependencyDescriptor
    {
		/// <summary>
		/// Identifier for this instance of descriptor.
		/// </summary>
		long DescriptorID { get; set; }
    }
}
