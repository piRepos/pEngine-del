using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Resources;
using pEngine.Timing;
using pEngine.Timing.Base;

namespace pEngine.Games
{
	/// <summary>
	/// Base class for all objects used in a game.
	/// </summary>
	public abstract partial class GameObject
	{
		private void InjetServices()
		{
			GameTree gametree = Game.GameTree;

			var propsSettings = BindingFlags.Public;
			propsSettings |= BindingFlags.NonPublic;
			propsSettings |= BindingFlags.Instance;

			var thisType = GetType();
			var properties = thisType.GetProperties(propsSettings);

			foreach (var property in properties)
			{
				var attribute = property.GetCustomAttribute<ServiceAttribute>();

				if (attribute != null)
				{
					Type serviceType = property.PropertyType;
					var service = gametree.GetService(serviceType);

					property.SetValue(this, service);
				}
			}
		}

	}
}
