using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Core.Data.FrameDependency
{
	public interface IDependencyReference
    {

		/// <summary>
		/// Dependency identifier.
		/// </summary>
		long DependencyID { get; }

	}
}
