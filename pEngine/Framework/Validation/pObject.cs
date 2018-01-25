using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using pEngine.Framework.Validation;

namespace pEngine.Framework
{
	public partial class pObject
	{
		#region Gets validations

		private void initializeValidations()
		{
			predicateVector = new Dictionary<string, Func<Exception>>();

			PropertyChanged += checkValidation;

			var type = GetType();
			var props = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

			foreach (var property in props)
			{
				var attribute = property.GetCustomAttribute(typeof(Validate)) as Validate;
				if (attribute != null)
				{
					predicateVector.Add(property.Name, () =>
					{
						var value = property.GetValue(this);
						if (attribute.Predicate.Invoke(value))
							return null;
						else
						{
							return attribute.Error;
						}
					});
				}
			}
		}

		#endregion

		#region Caching & check

		private Dictionary<string, Func<Exception>> predicateVector;

		private void checkValidation(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			try
			{
				var error = predicateVector[e.PropertyName].Invoke();
				if (error != null)
					throw error;
			}
			catch (KeyNotFoundException)
			{ }
		}

		#endregion
	}
}
