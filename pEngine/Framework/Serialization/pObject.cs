using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Framework.Serialization;

namespace pEngine.Framework
{
	using pSerializableAttribute = Serialization.pSerializableAttribute;

	public partial class pObject
	{

		private void generateStateStruct()
		{
			PropertyChanged += PObject_PropertyChanged;
			
			var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
			var properties = GetType().GetProperties(flags);
			var attprops = properties.Where(x => x.GetCustomAttribute<pSerializableAttribute>() != null).ToList();

			pObjectState state = new pObjectState();
			state.Properties = new pObjectProperty[attprops.Count()];

			for (int i = 0; i < state.Properties.Length; ++i)
			{
				state.Properties[i] = new pObjectProperty
				{
					Info = attprops[i],
					Value = attprops[i].GetValue(this)
				};
			}
		}

		private void disposeStateStruct()
		{
			PropertyChanged -= PObject_PropertyChanged;
		}

		/// <summary>
		/// Object serializable data.
		/// </summary>
		public pObjectState ObjectData { get; private set; }

		private void PObject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			var props = ObjectData.Properties.Where(x => x.Info.Name == e.PropertyName);

			if (props.Count() != 1)
				return;

			var prop = props.FirstOrDefault();

			prop.Value = prop.Info.GetValue(this);
		}

	}
}
