using System;
using System.Linq;
using System.Reflection;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using pEngine.Common.DataModel;

namespace pEngine.Core.Data
{
	public class GameObjectLoader
	{

		public GameObjectLoader()
		{
			activators = new ConcurrentDictionary<Type, Activator>();
			services = new ConcurrentDictionary<Type, object>();
		}

		#region Services

		private ConcurrentDictionary<Type, object> services;

		/// <summary>
		/// Gets the cached services.
		/// </summary>
		public IEnumerable<KeyValuePair<Type, object>> Services => services;

		/// <summary>
		/// Adds a service to the loader.
		/// this service will be avaiable for game objects loading.
		/// </summary>
		/// <param name="service">Service.</param>
		public void AddService(object service)
		{
			if (!services.ContainsKey(service.GetType()))
				services[service.GetType()] = service;
		}

		private object getService(Type service)
		{
			if (services.ContainsKey(service))
				return services[service];

			return null;
		}

		#endregion

		#region Register

		private delegate void Activator(GameObjectLoader sender, object instance);

		private ConcurrentDictionary<Type, Activator> activators;

		private void register(Type type)
		{
			List<MethodInfo> loaderMethods = new List<MethodInfo>();

			//
			//	For each class inherited from the main
			//	type i get all initializator methods
			//	and i build a list of initializators.
			//
			Type currentType = type;
			while (currentType != typeof(object))
			{
				MethodInfo[] currentLoaders = getMethods(currentType);

				loaderMethods.AddRange(currentLoaders);

				currentType = currentType.BaseType;
			}

			loaderMethods.Reverse();

			//
			//	For each loader method i must check the
			//	parameters and create a delegate that
			//	send to parameters the requests services
			//
			var initializators = loaderMethods.Select(method =>
			{
				bool permitNullServices = method.GetCustomAttribute<LoaderFunction>().PermitNullServices;
				var parameters = method.GetParameters().Select(parameter => parameter.ParameterType);

				List<object> services = new List<object>();

				foreach (var param in parameters)
				{
					object service = getService(param);

					if (service == null && !permitNullServices)
					{
						throw new InvalidOperationException($"Cannot register '{type.Name}', the service '{param.Name}' is not avaiable.");
					}

					services.Add(service);
				}

				return new Action<object>(instance =>
				{
					method.Invoke(instance, services.ToArray());
				});

			});

			//
			//	Finnally i make a global initializator for this type
			//	and i cache it in the activetors conturrent
			//	dictionary.
			//
			activators[type] = (loader, instance) =>
			{
				foreach (var initializer in initializators)
					initializer(instance);
			};
		}

		private MethodInfo[] getMethods(Type type)
		{
			MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);

			methods = methods.Where(method => method.CustomAttributes.Any(attr => attr.AttributeType == typeof(LoaderFunction))).ToArray();

			return methods;
		}

		#endregion

		#region Loader

		/// <summary>
		/// Loads the game object sync.
		/// </summary>
		/// <param name="obj">Object to load.</param>
		public void LoadSync(IGameObject obj)
		{
			var type = obj.GetType();

			lock (activators)
				if (!activators.ContainsKey(type))
					register(type);

			Activator activator;

			if (!activators.TryGetValue(type, out activator))
				throw new Exception("Dynamic initialization failed badly.");

			activator(this, obj);
		}

		#endregion
	}
}
