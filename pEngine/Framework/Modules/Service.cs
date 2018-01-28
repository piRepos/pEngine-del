using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using pEngine.Utils.Threading;

namespace pEngine.Framework.Modules
{
	public class Service : pObject
	{
		/// <summary>
		/// Makes a new instance of <see cref="Service"/> class.
		/// </summary>
		/// <param name="module">Owner module.</param>
		/// <param name="mainScheduler">Scheduler for this service.</param>
		public Service(Module module, Scheduler mainScheduler)
		{
			Module = module;
			RunningScheduler = mainScheduler;
			Activators = new Dictionary<string, Func<object[], object>>();
		}

		/// <summary>
		/// Gets the owner module.
		/// </summary>
		public Module Module { get; }

		/// <summary>
		/// Scheduler for this service.
		/// </summary>
		public Scheduler RunningScheduler { get; }

		/// <summary>
		/// API activators.
		/// </summary>
		private Dictionary<string, Func<object[], object>> Activators;

		#region Initialization

		/// <summary>
		/// Sets-up every callback and binding.
		/// </summary>
		public virtual void Initialize()
		{
			// - Initialization
			var type = GetType();
			var moduleType = Module.GetType();

			var propsSettings = BindingFlags.Public;
			propsSettings |= BindingFlags.NonPublic;
			propsSettings |= BindingFlags.Instance;

			// - Properties initialization
			var props = type.GetProperties(propsSettings);

			foreach (var prop in props)
			{
				var attr = prop.GetCustomAttribute<ServicePropertyAttribute>();

				if (attr == null)
					continue;

				var extProp = moduleType.GetProperty(attr.ReferencesTo);
				if (prop.CanWrite)
				{
					Activators.Add(prop.Name, (par) =>
					{
						Module.Scheduler.Add(() =>
						{
							extProp.SetValue(Module, prop.GetValue(this));
						});
						return null;
					});
				}
				if (prop.CanRead)
				{
					prop.SetValue(this, extProp.GetValue(Module));

					Module.PropertyChanged += (sender, p) =>
					{
						if (p.PropertyName == attr.ReferencesTo)
						{
							if (RunningScheduler != null)
							{
								RunningScheduler.Add(() =>
								{
									prop.SetValue(this, extProp.GetValue(Module));
								});
							}
							else prop.SetValue(this, extProp.GetValue(Module));
						}
					};
				}
			}

			// - Events initialization
			var events = type.GetEvents(propsSettings);

			foreach (var even in events)
			{
				var attr = even.GetCustomAttribute<ServiceEventAttribute>();

				if (attr == null)
					continue;

				var extEvent = moduleType.GetEvent(attr.ReferencesTo);

				var prop = type.GetField(even.Name, propsSettings);

				if (extEvent != null)
				{
					Action<object[]> call = (par) =>
					{
						if (RunningScheduler != null)
						{
							RunningScheduler.Add(() =>
							{
								((Delegate)prop.GetValue(this)).DynamicInvoke(par);
							});
						}
						else ((Delegate)prop.GetValue(this)).DynamicInvoke(par);
					};

					var del = DowncastDelegate(call, extEvent.EventHandlerType);
					extEvent.AddEventHandler(Module, del);
				}
			}

			// - Actuators
			PropertyChanged += (sender, p) =>
			{
				if (Activators.ContainsKey(p.PropertyName))
					Activators[p.PropertyName](null);
			};
		}

		private Delegate DowncastDelegate(Action<object[]> action, Type delegateType)
		{
			var invokeMethod = delegateType.GetMethod("Invoke");
			var parameters = invokeMethod.GetParameters();

			var loadParameters = parameters.Select(x => Expression.Parameter(x.ParameterType, x.Name)).ToArray();

			var makeArray = Expression.NewArrayInit(typeof(object), loadParameters.Select(x => Expression.Convert(x, typeof(object))));
			
			Expression methodCall = Expression.Invoke(Expression.Constant(action), makeArray);

			Expression convertedMethodCall = invokeMethod.ReturnType == action.Method.ReturnType
					  ? methodCall
					  : Expression.Convert(methodCall, invokeMethod.ReturnType);

			return Expression.Lambda(delegateType, convertedMethodCall, loadParameters).Compile();
		}

		#endregion

	}
}
