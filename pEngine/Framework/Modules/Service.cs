using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using pEngine.Utils.Threading;
using pEngine.Utils.Timing.Base;
using pEngine.Utils.Timing;

namespace pEngine.Framework.Modules
{
	public class Service : pObject, IUpdatable
	{
		/// <summary>
		/// Makes a new instance of <see cref="Service"/> class.
		/// </summary>
		/// <param name="module">Owner module.</param>
		/// <param name="mainScheduler">Gameloop for this service.</param>
		public Service(Module module, GameLoop mainLoop)
		{
			Module = module;
			RunningLoop = mainLoop;
			Activators = new Dictionary<string, Func<object[], object>>();
		}

		/// <summary>
		/// Gets the owner module.
		/// </summary>
		public Module Module { get; }

		/// <summary>
		/// Scheduler for this service.
		/// </summary>
		public GameLoop RunningLoop { get; }

		/// <summary>
		/// API activators.
		/// </summary>
		private Dictionary<string, Func<object[], object>> Activators;

		/// <summary>
		/// Update the state of this element.
		/// </summary>
		/// <param name="clock">Game clock.</param>
		public virtual void Update(IFrameBasedClock clock)
		{

		}

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

				var extProp = moduleType.GetProperty(attr.ReferencesTo, propsSettings);
				if (extProp.CanWrite)
				{
					Activators.Add(prop.Name, (par) =>
					{
						Module.ModuleLoop.Scheduler.Add(() =>
						{
							extProp.SetValue(Module, prop.GetValue(this));
						});
						return null;
					});
				}
				if (extProp.CanRead)
				{
					prop.SetValue(this, extProp.GetValue(Module));

					Module.PropertyChanged += (sender, p) =>
					{
						if (p.PropertyName == attr.ReferencesTo)
						{
							if (RunningLoop != null)
							{
								RunningLoop.Scheduler.Add(() =>
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
						if (RunningLoop != null)
						{
							RunningLoop.Scheduler.Add(() =>
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
