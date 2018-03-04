using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace pEngine.Framework.Serialization
{
	public struct pObjectProperty
	{
		
		/// <summary>
		/// Current property reflection info.
		/// </summary>
		public PropertyInfo Info { get; set; }

		/// <summary>
		/// Current value.
		/// </summary>
		public object Value { get; set; }

	}

	public struct pObjectState
	{

		/// <summary>
		/// All class properties.
		/// </summary>
		public pObjectProperty[] Properties { get; set; }

		/// <summary>
		/// Gets a functions that syncronize the target object with the source object.
		/// </summary>
		/// <param name="target">Target object.</param>
		/// <returns>A function.</returns>
		public Action<pObject> GetSyncDelegate(pObjectState target)
		{
			List<Tuple<pObjectProperty, pObjectProperty>> changed;
			changed = new List<Tuple<pObjectProperty, pObjectProperty>>();

			foreach (var prop in target.Properties)
			{
				var counterparts = Properties.Where(x => x.Info.Equals(prop.Info));
				if (counterparts.Count() == 1)
				{
					var counterpart = counterparts.FirstOrDefault();
					if (!prop.Value.Equals(counterpart))
					{
						changed.Add(new Tuple<pObjectProperty, pObjectProperty>(prop, counterpart));
					}
				}
			}

			return new Action<pObject>(x =>
			{
				foreach (var cprop in changed)
				{
					cprop.Item2.Info.SetValue(x, cprop.Item1.Value);
				}
			});
		}
	}
}
