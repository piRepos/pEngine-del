using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Core.Data.FrameDependency
{
    public abstract class KeyFrameDependencyManager<KeyType, DependencyType, DescriptorType>
		: FrameDependencyManager<DependencyType, DescriptorType>
		where DependencyType : IDependency<DescriptorType>
		where DescriptorType : IDependencyDescriptor
	{

		/// <summary>
		/// Makes a new instance of <see cref="KeyFrameDependencyManager{DependencyType, DescriptorType}"/> class.
		/// </summary>
		/// <param name="host">Host engine.</param>
		public KeyFrameDependencyManager(pEngine host)
			: base(host)
		{
			dependencies = new Dictionary<KeyType, DependencyType>();
		}


		#region Keys storage

		private Dictionary<KeyType, DependencyType> dependencies;

		/// <summary>
		/// Gets all dependencies as <see cref="KeyValuePair{KeyType, DependencyType}"/>.
		/// </summary>
		public new Dictionary<KeyType, DependencyType>.Enumerator Dependencies => dependencies.GetEnumerator();

		/// <summary>
		/// Adds a new dependency with a key.
		/// </summary>
		/// <param name="key">Dependency idenfitier key.</param>
		/// <param name="dependency">Dependency to add.</param>
		protected void AddDependency(KeyType key, DependencyType dependency)
		{
			dependencies.Add(key, dependency);

			base.AddDependency(dependency);
		}

		#endregion

		#region Keys management

		protected override void OnDependencyLoad(DependencyType dependency)
		{

		}

		protected override void OnDependencyChange(DependencyType dependency)
		{

		}

		protected override void OnDependencyDispose(DependencyType dependency)
		{
			var dep = dependencies.Where(x => x.Value.DependencyID == dependency.DependencyID);
			dependencies.Remove(dep.FirstOrDefault().Key);
		}

		#endregion

	}
}
